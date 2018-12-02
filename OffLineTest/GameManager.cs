using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if !UNITY_EDITOR && UNITY_IOS
using System.Runtime.InteropServices;
#endif
using System.Threading;

public class GameManager : MonoBehaviour
{
	private static GameManager _inst = null;
	public static GameManager Inst { get { return _inst; } }

#if UNITY_EDITOR || PREGAME_TEST || DT_TEST
	// 각 인자는 큰따옴표로 구분하고, 배열의 배열 형식으로 입력한다.
	[SerializeField] private string changeDT;	// Ex) [["StageData","CamFOV","1","45"],["StageData","CamPos","1","[0,10,-10]"],["ml.ko","Text","SkillData.SkillHelpText.1","test"]]
#endif

	public event Action evtDownloadStart = null;
	public event Action evtDownloadCompleted = null;
	public event Action<int, int> evtDownloadProgress = null;

	public event Action<bool, Action> evtReadyTouchToStart = null;
	public event Action evtAssetInitCompleted = null;
	public event Action evtGameLoginCompleted = null;

	private DateTime m_oldTime;
	private Timer _ChatCloseTimer;

	private bool _pauseSatus;
	public bool PauseSatus{ get { return _pauseSatus; }}

#if !UNITY_EDITOR && UNITY_IOS
	private Timer _ArenaCloseTimer;

	[DllImport("__Internal")]
	private static extern bool IsEnterBackground();
#endif

	void Awake()
	{
		useGUILayout = false;
		_inst = this;
		_ChatCloseTimer = new Timer((obj) => { ChatManager.Inst.CloseWS(); }, null, Timeout.Infinite, Timeout.Infinite);
#if !UNITY_EDITOR && UNITY_IOS
		_ArenaCloseTimer = new Timer((obj) => { CheckAndCloseArenaSocket(); }, null, Timeout.Infinite, Timeout.Infinite);
#endif
	}
	
	void Start()
	{
		Global.Inst.evtGlobalInitialized += onGlobalInitialized;
	}

	void OnDestroy()
	{
		Global.Inst.evtGlobalInitialized -= onGlobalInitialized;
	}

	private void onGlobalInitialized()
	{
		Debug.Log(Logger.Write("onGlobalInitialized."));

#if UNITY_EDITOR || PREGAME_TEST || DT_TEST
		// UpdateDT
		if (!string.IsNullOrEmpty(changeDT))
		{
			object obj = MiniJSON.Json.Deserialize(changeDT);
			Type type = obj.GetType();
			if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)))
			{
				IList list = obj as IList;
				for (int i = 0; i < list.Count; ++i)
				{
					object value = list[i];
					Type valueType = value.GetType();
					if (valueType.IsGenericType && (valueType.GetGenericTypeDefinition() == typeof(List<>)))
					{
						IList valueList = list[i] as IList;
						if (valueList != null && valueList.Count == 4)
							Datatable.Inst.UpdateDT(valueList[0].ToString(), valueList[1].ToString(), valueList[2].ToString(), valueList[3], false);
						else
						{
							Debug.LogError(Logger.Write("Invalid Change DT."));
						}
					}
					else
					{
						Debug.LogError(Logger.Write("Invalid Change DT."));
					}
				}

				Datatable.Inst.PostProcessUpdateDT();
			}
			else
			{
				Debug.LogError(Logger.Write("Invalid Change DT."));
			}
		}
#endif
		
		Giant.Util.ApplyFpsGrade();

		// Set Thread Count
		int threadCount = Mathf.Max(Datatable.Inst.settingData.GlobalThreadCount, 1);
		AL.PersistentStore.Inst.SetDownloadMaxCount(threadCount);
		Global.threadMaxCount = threadCount;

		DownloadManager.Inst.DoDownload(() => {
			if (evtDownloadStart != null)
				evtDownloadStart();
		}, () => {
			if (evtDownloadCompleted != null)
				evtDownloadCompleted();
			BaseOperatorUnit.instance.Initialize();
			AssetBundleManager.Inst.Initialize(OnAssetBundleInitCompleted, DownloadManager.Inst.RetryDownload);
		}, evtDownloadProgress);
	}

	private void OnAssetBundleInitCompleted()
	{
		if (evtAssetInitCompleted != null)
			evtAssetInitCompleted();
	}

	private void DoGameLogin()
	{
		// Game Login
		if (!LoginManager.Inst.isLogined)
		{
			LoginManager.Inst.Login(() =>
			{
				Invoke("OnGameLoginCompleted", 0f);
			}, () =>
			{
				Invoke("OnGameLoginFailed", 0f);
			}, ()=>
			{
				Invoke("OnTokenVerifyFailed", 0f);
			});
		}
		else
		{
			Invoke("OnGameLoginCompleted", 0f);
		}
	}

	private void DoIpCheck()
	{
		// OffLineTest
		DoGameLogin();
		return;

		Giant.Util.CheckPublicIp((isSuccess) =>
		{
			if (isSuccess)
				DoGameLogin();
			else
				SuperOverlayPopupManager.Inst.ShowOneButtonPopup(UITextEnum.BUILTIN_NETWORK_FAIL).SetClickButtonAction(DoIpCheck).SetAlias(PopupAlias.SuperOverlayNetworkPopup);
		});
	}

	private void OnGameLoginCompleted()
	{
#if !UNITY_EDITOR
		StopCoroutine("CheckTimeChanged");
		StartCoroutine("CheckTimeChanged");
#endif
		
		if(evtGameLoginCompleted != null)
			evtGameLoginCompleted();
	}

	private void OnTokenVerifyFailed()
	{
#if UNITY_EDITOR || HIDDEN_LOGIN_TEST
		DoGameLogin();
#else
		if(!HiveManager.Inst.IsGuestAccount())
		{
			HiveManager.Inst.DoHiveLogout((success)=>{
				if(success)
					DoHiveManualLogin();	
				else
					Invoke("OnTokenVerifyFailed", 0f);	
			});
		}
		else
		{
			Invoke("OnGameLoginFailed", 0f);	
		}
#endif
	}

	private void OnGameLoginFailed()
	{
		evtReadyTouchToStart(false, ()=>{
			DoGameLogin();
		});
	}

#if UNITY_EDITOR || PREGAME_TEST || DT_TEST
	public void SetChangeDT(string changeDT)
	{
		this.changeDT = changeDT;
	}
#endif
	
	private void OnApplicationPause(bool pauseStatus)
	{
		_pauseSatus = pauseStatus;
		HiveManager.Inst.RegisterLocalPush(pauseStatus);
		if (pauseStatus)
		{
			_ChatCloseTimer.Change(Constant.ChatSocketTimeout, Timeout.Infinite);

#if !UNITY_EDITOR && UNITY_IOS
			_ArenaCloseTimer.Change(Constant.ArenaSocketTimeout, Constant.ArenaSocketTimeout);
#else
			CheckAndCloseArenaSocket();
#endif
		}
		else
		{
			_ChatCloseTimer.Change(Timeout.Infinite, Timeout.Infinite);
			if (LoginManager.Inst != null && LoginManager.Inst.isLogined)
				ChatManager.Inst.ConnectWS();
			
#if !UNITY_EDITOR && UNITY_IOS
			_ArenaCloseTimer.Change(Timeout.Infinite, Timeout.Infinite);
#endif
		}

		HiveManager.Inst.UnRegisterDormantUserPush();
		if (LoginManager.Inst != null && LoginManager.Inst.isLogined)
			HiveManager.Inst.RegisterDormantUserPush();
	}
	
	private void CheckAndCloseArenaSocket()
	{
#if !UNITY_EDITOR && UNITY_IOS
		if (!IsEnterBackground())
			return;
#endif
		GiantHandler.Inst.CloseArenaSocket();
	}

	public void OnStartLoginFromPreGame()
	{
		DoIpCheck();
	}

	public void OnStartHiveLogin()
	{
#if UNITY_EDITOR || HIDDEN_LOGIN_TEST
		evtReadyTouchToStart(false, ()=>{
			DoIpCheck();
		});
#else
		if (HiveManager.Inst.AfterLogout)
		{
			DoHiveManualLogin();	
		}
		else
		{
			if (hive.AuthV4.isAutoSignIn())
			{
				HiveManager.Inst.DoHiveAutoLogin((errorCode) => {
					if (errorCode == hive.ResultAPI.ErrorCode.SUCCESS) 
					{
						evtReadyTouchToStart(false, ()=>{
							DoIpCheck();
						});
					}
					else 
					{
						if (HiveManager.Inst.IsConnectedFromChina())
						{
#if UNITY_ANDROID
							DoHiveManualLogin();	
#else
							DoHiveImpliedLogin();
#endif
						}
						else
						{
							DoHiveImpliedLogin();
						}
					}
				});	
			}
			else
			{
				if (HiveManager.Inst.IsConnectedFromChina())
				{
#if UNITY_ANDROID
					DoHiveManualLogin();	
#else
					DoHiveImpliedLogin();
#endif
				}
				else
				{
					DoHiveImpliedLogin();
				}
			}			
		}
#endif
	}

	private void DoHiveImpliedLogin()
	{
		HiveManager.Inst.DoHiveImpliedLogin((errorCode) => {
			if(errorCode == hive.ResultAPI.ErrorCode.SUCCESS)
			{
				if (evtReadyTouchToStart != null)
				{
					evtReadyTouchToStart(false, ()=>{
						DoIpCheck();
					});
				}
			}
			else
			{
				DoHiveManualLogin();
			}
		});
	}

	private void DoHiveManualLogin()
	{
		if (evtReadyTouchToStart != null)
			evtReadyTouchToStart(true, ()=>{
				HiveManager.Inst.DoHiveManualLogin((errorCode)=>{
					switch(errorCode)
					{
					case hive.ResultAPI.ErrorCode.SUCCESS:
						DoIpCheck();	
						break;
					case hive.ResultAPI.ErrorCode.CANCELED:
						DoHiveManualLogin();
						break;
					case hive.ResultAPI.ErrorCode.NETWORK:
						SuperOverlayPopupManager.Inst.ShowOneButtonPopup(UITextEnum.COMMON_NETWORK_FAIL).SetClickButtonAction(DoHiveManualLogin);
						break;
					default:
						DoHiveManualLogin();
						break;
					}
				});	
			});
	}

	public void Purchase(int idGoods, bool isBuffFlag, int idHero, Action<bool> resultAction)
	{
		Datatable.Goods goodsData = Datatable.Inst.dtGoods[idGoods];

#if !UNITY_EDITOR && !HIDDEN_LOGIN_TEST
		if (goodsData.InappID > 0)
		{
			Datatable.Inapp inappData = Datatable.Inst.dtInapp[goodsData.InappID];

			string marketPid;
#if UNITY_ANDROID
			marketPid = inappData.ItemIDGoogle;
#elif UNITY_IOS
			marketPid = inappData.ItemIDApple;
#endif
			HiveManager.Inst.PurchaseHiveInapp(marketPid, (result, iapReceipt)=>{
				if(result.isSuccess())
				{
					GiantHandler.Inst.doPurchase(goodsData.ID, isBuffFlag, idHero, iapReceipt).SetSuccessAction(() =>
					{
						resultAction(true);
						HiveManager.Inst.PurchaseComplete(goodsData, iapReceipt.product.marketPid);
					}).SetFailureAction((error)=>{
						if (error.ToString() == ServerError.ALREADY_PROCESSED_RECEIPT)
							HiveManager.Inst.PurchaseComplete(goodsData, iapReceipt.product.marketPid);
						resultAction(false);
					});
				}
				else
				{	
					switch(result.errorCode)
					{
					case hive.ResultAPI.ErrorCode.NEED_INITIALIZE:
						{
#if UNITY_ANDROID
							SystemPopupManager.Inst.ShowTitleOneButtonPopup(UITextEnum.PURCHASE_INVALID_MARKET_INFO, UITextEnum.PURCHASE_ERROR_MARKET_DISCONNECTED_AOS);
#elif UNITY_IOS
							SystemPopupManager.Inst.ShowTitleOneButtonPopup(UITextEnum.PURCHASE_INVALID_MARKET_INFO, UITextEnum.PURCHASE_ERROR_MARKET_DISCONNECTED_IOS);
#endif
							HiveManager.Inst.IsConnectedMarket = false;	
						}
						break;
					case hive.ResultAPI.ErrorCode.NETWORK:
						SystemPopupManager.Inst.ShowOneButtonPopup(UITextEnum.PURCHASE_ERROR_NETWORK);
						break;
					case hive.ResultAPI.ErrorCode.NOT_SUPPORTED:
						SystemPopupManager.Inst.ShowOneButtonPopup(UITextEnum.PURCHASE_ERROR_NOT_SUPPORTED);
						break;
					case hive.ResultAPI.ErrorCode.IN_PROGRESS:
						SystemPopupManager.Inst.ShowOneButtonPopup(UITextEnum.PURCHASE_ERROR_IN_PROGRESS);
						break;
					case hive.ResultAPI.ErrorCode.CANCELED:
						SystemPopupManager.Inst.ShowOneButtonPopup(UITextEnum.PURCHASE_ERROR_CANCELED);
						break;
					case hive.ResultAPI.ErrorCode.NEED_RESTORE:
						{
							GameManager.Inst.RestoreInappPurchase(null);
						}
						break;
					case hive.ResultAPI.ErrorCode.INVALID_PARAM:
					case hive.ResultAPI.ErrorCode.INVALID_SESSION:
					case hive.ResultAPI.ErrorCode.RESPONSE_FAIL:
							SystemPopupManager.Inst.ShowOneButtonPopup(Datatable.Inst.GetUIText(UITextEnum.PURCHASE_ERROR_NORMAL, result.errorCode));
						break;
					default:
						SystemPopupManager.Inst.ShowOneButtonPopup(Datatable.Inst.GetUIText(UITextEnum.PURCHASE_ERROR_NORMAL, result.errorCode));
						break;
					}
					Debug.Log(Logger.Write(Util.StringFormat("Purchase Message : {0}, errorMessage : {1}", result.message, result.errorMessage)));
					resultAction(false);
				}
			});		
		}
		else
		{
			GiantHandler.Inst.doPurchase(idGoods, isBuffFlag, idHero, null).SetSuccessAction(() =>
			{
				HiveManager.Inst.PurchaseComplete(goodsData, null);
				if(resultAction != null)
					resultAction(true);
			});
		}
#else
		GiantHandler.Inst.doPurchase(idGoods, isBuffFlag, idHero, null).SetSuccessAction(() =>
		{
			if(resultAction != null)
				resultAction(true);
		});
#endif
	}

	public void RestoreInappPurchase(Action resultAction)
	{
		hive.IAPV4.restore((result, iapv4ReceiptList)=>{
			if (result.errorCode != hive.ResultAPI.ErrorCode.NOT_OWNED) 
			{
				StartCoroutine(ProcessRestorePurchase(iapv4ReceiptList, ()=>{
					if(resultAction != null)
						resultAction();
				}));
			}
			else
			{
				if(resultAction != null)
					resultAction();
			}
		});	
	}

	IEnumerator ProcessRestorePurchase(List<hive.IAPV4.IAPV4Receipt> iapv4ReceiptList, Action resultAction)
	{
		var enumerator = iapv4ReceiptList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Debug.Log("ProcessRestorePurchase IAPV4Receipt " + enumerator.Current.toString());
			List<KeyValuePair<int, Datatable.Inapp>> listInapp = Datatable.Inst.dtInapp.Where(i => (i.Value.ItemIDGoogle == enumerator.Current.product.marketPid.ToString() || i.Value.ItemIDApple == enumerator.Current.product.marketPid.ToString())).ToList();
			List<KeyValuePair<int, Datatable.Goods>> listGoods = Datatable.Inst.dtGoods.Where(i => i.Value.InappID == listInapp[0].Value.ID).ToList();

			Debug.Log("Purchase id : " + listGoods[0].Value.ID.ToString() + "enumerator.Current.bypassInfo : " + enumerator.Current.bypassInfo);
			yield return GiantHandler.Inst.doPurchase(listGoods[0].Value.ID, false, -1, enumerator.Current).SetSuccessAction(() =>
			{
				if(listGoods[0].Value.ByMail == 0)
					GlobalOverlayPopupManager.Inst.ShowRewardResultPopup(listGoods[0].Key);
				else
					SystemPopupManager.Inst.ShowOneButtonPopup(Datatable.Inst.GetUIText(UITextEnum.PURCHASE_SEND_POSTBOX, listGoods[0].Value.Name)).SetClickButtonAction(() => { UITutorialEventManager.instance.CheckTriggers(EVENT_TRIGGER.BUY_COUNT_GOTCHA); });

				HiveManager.Inst.PurchaseComplete(listGoods[0].Value, enumerator.Current.product.marketPid);

			}).SetFailureAction((error) =>
			{
				if (error.ToString() == ServerError.ALREADY_PROCESSED_RECEIPT)
				{
					HiveManager.Inst.PurchaseComplete(listGoods[0].Value, enumerator.Current.product.marketPid);
				}
			}).WaitForResponse();
		}
		if(resultAction != null)
			resultAction();
	}

	public void StartCheckNotice()
	{
		StopCoroutine("CheckNotice");
		StartCoroutine("CheckNotice");
	}

	private IEnumerator CheckTimeChanged()
	{
		m_oldTime = DateTime.Now;
		STWaitForSecondsRealtime waitTime = new STWaitForSecondsRealtime();
		bool play = true;
		while (play)
		{
			if (LoginManager.Inst.isLogined)
			{
				System.TimeSpan timeDiff = DateTime.Now - m_oldTime;
				if (Math.Abs(timeDiff.TotalSeconds) > Constant.TimeChangeCheckSeconds)
				{
					long oldSvrTime = GiantHandler.Inst.serverTime;
					yield return GiantHandler.Inst.doGetServerTime().SetSuccessAction(() =>
					{
						if (Math.Abs(GiantHandler.Inst.serverTime - oldSvrTime) > (Constant.TimeChangeCheckSeconds * 1000))
						{
							SystemPopupManager.Inst.ShowOneButtonPopup(UITextEnum.POPUP_LOCAL_TIME_CHANGED).SetClickButtonAction(() =>
							{
								BaseOperatorUnit.instance.RestartApp();
							});
							play = false;
						}
					}).WaitForResponse();
				}
				m_oldTime = DateTime.Now;
			}	
			yield return waitTime.SetWaitTime(1f);
		}
	}

	private IEnumerator CheckNotice()
	{
		ChatManager.Inst.InitNotice = true;
		while (true)
		{
			for (int i = 0; i < ChatManager.Inst.noticeDataList.Count; ++i)
			{
				NoticeData data = ChatManager.Inst.noticeDataList[i];

				for(int j = 0; j < data.noticeTimeList.Count; ++j)
				{
					if (data.stopActivation == 0 && data.noticeTimeList[j] < GiantHandler.Inst.serverTime)
					{
						switch ((NOTICE_TYPE)data.noticeType)
						{
						case NOTICE_TYPE.ALL:
							SystemOverlayPopupManager.Inst.ShowTickerPopup(data._id, data.GetMessage(), Datatable.Inst.settingData.TickerShowTime);
							ChatManager.Inst.AddChatSystemMessage(data.GetMessage(), CHAT_TYPE.NORMAL);
							break;
						case NOTICE_TYPE.CHATTING:
							ChatManager.Inst.AddChatSystemMessage(data.GetMessage(), CHAT_TYPE.NORMAL);
							break;
						case NOTICE_TYPE.TICKER:
							SystemOverlayPopupManager.Inst.ShowTickerPopup(data._id, data.GetMessage(), Datatable.Inst.settingData.TickerShowTime);
							break;
						default:
							break;
						}
						data.noticeTimeList.RemoveAt(j--);
					}	
				}

				if (data.endDate < GiantHandler.Inst.serverTime)
					ChatManager.Inst.noticeDataList.RemoveAt(i--);
			}
			yield return null;	
		}
	}
}

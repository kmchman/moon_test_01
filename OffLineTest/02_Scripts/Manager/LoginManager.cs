using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LoginManager : MonoBehaviour
{
	private static LoginManager _inst = null;
	public static LoginManager Inst { get { return _inst; } }
	
	public bool isLogined { get; set; }
	public bool isLoginProcessing { get; private set; }

	private Action loginSuccess;
	private Action tokenVerifyFail;
	private Action loginFail;

	void Awake()
	{
		useGUILayout = false;
		_inst = this;

		isLogined = false;
		isLoginProcessing = false;
	}
	
	public void Login(Action loginSuccess, Action loginFail, Action tokenVerifyFail)
	{
		isLogined = false;
		Global.Inst.loadState = Global.LoadState.GameLogin;
		this.loginSuccess = loginSuccess;
		this.loginFail = loginFail;
		this.tokenVerifyFail = tokenVerifyFail;

		LoginProcess();
	}

	private void LoginProcess()
	{
		Debug.Log(Logger.Write("Game Login Processing."));

		isLoginProcessing = true;

		Global.Inst.InvokeLoadingStart();

		// OffLineTest
		LoginSubProcess();
		return;

#if UNITY_EDITOR || HIDDEN_LOGIN_TEST
		if(!GiantHandler.Inst.LoadAccount())
		{
			GiantHandler.Inst.CreateHiddenLoginToken();
		}
			
		// Hidden login
		GiantHandler.Inst.doHiddenLogin().SetSuccessAction(LoginSubProcess).SetFailureAction(LoginFail);
#else
		// login
		GiantHandler.Inst.doLogin().SetSuccessAction(LoginSubProcess).SetFailureAction(LoginFail);
#endif
	}

	private void LoginSubProcess()
	{
		// OffLineTest
		LoginSuccess();
		return;

		Action getGuildDataAction = () => {
			// guild data
			GiantHandler.Inst.doGetGuildData().SetSuccessAction(LoginSuccess).SetFailureAction((error) => {
				// 길드 정보 받는 걸 실패하더라도 게임에 진입할 수 있도록 한다.
				GiantHandler.DefaultRequestFailure(error);
				LoginSuccess();
			});
		};

		Action updateSeasonData = () => {
			AccountDataStore.instance.pvpSeasonInfo.UpdateLogin(() => {
				AccountDataStore.instance.arenaSeasonInfo.UpdateLogin(() => {
					AccountDataStore.instance.duelSeasonInfo.UpdateLogin(() => {
						AccountDataStore.instance.ValidateQuest(getGuildDataAction);
					});
				});
			});
		};

		// getUser
		GiantHandler.Inst.doGetUser().SetSuccessAction(() => {
			// takeMasterLevelUpTreasureChest - 레벨업을 했지만, 레벨업 보너스 보물 상자를 못 받고 게임이 꺼진 경우 다시 접속했을 때 연출 없이 보상만 다시 받도록 한다.
			if (AccountDataStore.instance.IsStackedTreasureChest())
				GiantHandler.Inst.doTakeMasterLevelUpTreasureChest().SetSuccessAction(updateSeasonData);
			else
				updateSeasonData();
		}).SetFailureAction(LoginFail);
	}

	private void LoginSuccess()
	{
		// OffLineTest
		isLogined = true;
		isLoginProcessing = false;

		Global.Inst.InvokeLoadingEnd();

		if (loginSuccess != null)
			loginSuccess();
		return;

		Debug.Log(Logger.Write("Game Login Success."));

		ChatManager.Inst.ClearMessageItems();
		ChatManager.Inst.ConnectWS();
		GiantHandler.Inst.doSilentCheckPost().SetPollingInterval(Datatable.Inst.settingData.CheckPostPollingInterval);
		AccountDataStore.instance.InitAchievementEffect();

#if !UNITY_EDITOR && !HIDDEN_LOGIN_TEST
		hive.ResultAPI result = hive.Promotion.setEngagementReady(true);
#endif
		HiveManager.Inst.UnRegisterDormantUserPush();
		HiveManager.Inst.RegisterDormantUserPush();
		
		isLogined = true;
		isLoginProcessing = false;

		Global.Inst.InvokeLoadingEnd();

		if (loginSuccess != null)
			loginSuccess();
	}

	private void LoginFail(object error)
	{
		Debug.LogError(Logger.Write("Game Login Failed."));

		isLoginProcessing = false;

		Global.Inst.InvokeLoadingEnd();

		if (error.Equals(ServerError.TOKEN_ERROR))
		{
			SystemPopupManager.Inst.ShowOneButtonPopup(UITextEnum.SVR_LOGIN_NOMATCH).SetClickButtonAction(() => {
#if UNITY_EDITOR || HIDDEN_LOGIN_TEST
				GiantHandler.Inst.CreateHiddenLoginToken();
				LoginProcess();
#else
				loginFail();
#endif
			});
		}
		else if(error.Equals(ServerError.HIVE_ERROR))
		{
			SystemPopupManager.Inst.ShowOneButtonPopup(Datatable.Inst.GetServerErrorText(error.ToString())).SetClickButtonAction(() => {
				tokenVerifyFail();
			});
		}
		else
		{
			SystemPopupManager.Inst.ShowOneButtonPopup(Datatable.Inst.GetServerErrorText(error.ToString())).SetClickButtonAction(() => {
				loginFail();
			});
		}
	}
}

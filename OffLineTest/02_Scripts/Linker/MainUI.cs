using UnityEngine;
using System.Collections;
using STDOTweenExtensions;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MainUI : MonoBehaviour
{
	[SerializeField] private PostListPopup		m_PostListPopupPrefab;
	[SerializeField] private GameSettingPopup 	m_GameSettingPrefab;
	[SerializeField] private CommunityPopup 	m_GameUICommunityPrefab;
	[SerializeField] private HotTimePopup 		m_HotTimePopupPrefab;

	[SerializeField] private GameObject 		m_WorldBossBetaSeasonItemObject;
	[SerializeField] private STGrayScaleGroup	m_ArenaGrayScaleGroup;
	[SerializeField] private STGrayScaleGroup	m_DuelGrayScaleGroup;
	[SerializeField] private STGrayScaleGroup	m_GameFormSettingGrayScaleGroup;
	[SerializeField] private GameObject			m_ObjectNoti;
	[SerializeField] private GameObject			m_ObjectChatNoti;
	[SerializeField] private GameObject			m_ObjectPostNoti;
	[SerializeField] private GameObject			m_ObjectPvpNoti;
	[SerializeField] private GameObject			m_ObjectWorldBossNoti;
	[SerializeField] private GameObject			m_ObjectWorldBossExNoti;
	[SerializeField] private GameObject 		m_ObjectFolderButtons;
	[SerializeField] private STImage 			m_ImageMenu;
	[SerializeField] private SpriteSet 			m_IconStateSpriteSet;
	[SerializeField] private HudType[] 			m_FolderNotiObjs;

	[SerializeField] private List<GameObject>	m_ListButtons;

	private void Start()
	{
		// OffLineTest
		return;
		HideFolderBtns();
		UpdateUserData(UserDataFlag.User | UserDataFlag.Area);
		UpdatePvpLogNoti();
		UpdateArenaDuelGrayScale();
		UpdateGameFormSettingGrayScale();
	}

	private void OnEnable()
	{
		GiantHandler.Inst.updateUserDelegate += UpdateUserData;
		ChatManager.Inst.guildNotiAction += OnReceiveGuildNoti;
		AccountDataStore.instance.pvpLog.updateDelegate += UpdatePvpLogNoti;
		CheckPvpDataRequest();
		CheckMarketReviewPopup();

		m_ObjectChatNoti.SetActive(ChatManager.Inst.IsOnGuildChatNotice());

		m_WorldBossBetaSeasonItemObject.SetActive(Datatable.Inst.isWorldBossBetaSeason);

		StartCoroutine(AccountDataStore.instance.pvpSeasonInfo.UpdateLobby());
		StartCoroutine(AccountDataStore.instance.arenaSeasonInfo.UpdateLobby());
		StartCoroutine(AccountDataStore.instance.duelSeasonInfo.UpdateLobby());

		CheckWorldBossInfo();
	}

	private void OnDisable()
	{
		GiantHandler.Inst.updateUserDelegate -= UpdateUserData;
		ChatManager.Inst.guildNotiAction -= OnReceiveGuildNoti;
		AccountDataStore.instance.pvpLog.updateDelegate -= UpdatePvpLogNoti;
	}

	private void UpdateUserData(UserDataFlag updateFlag)
	{
		if ((updateFlag & UserDataFlag.User) != 0)
		{
			m_ObjectPostNoti.SetActive(AccountDataStore.instance.user.postCount > 0);
			m_ObjectWorldBossNoti.SetActive(AccountDataStore.instance.user.WorldBossNotiCheck());
			m_ObjectWorldBossExNoti.SetActive(AccountDataStore.instance.user.WorldBossNotiCheck());
			UpdateMenuNoti();
		}
		if ((updateFlag & UserDataFlag.Area) != 0)
		{
			UpdateArenaDuelGrayScale();
			UpdateGameFormSettingGrayScale();
		}
	}

	private void OnReceiveGuildNoti()
	{
		m_ObjectChatNoti.SetActive(true);
	}

	private void ShowPostListPopup()
	{
		if (GlobalPopupManager.Inst != null)
			GlobalPopupManager.Inst.ShowPopup(m_PostListPopupPrefab);
	}

	public void OnClickBtnChat()
	{
		System.Action hideNotiAction = () =>
		{
			m_ObjectChatNoti.SetActive(false);
		};

		bool check = AccountDataStore.instance.IsOnGuildMembership();
		GiantHandler.Inst.doGetGuildData().SetSuccessAction(() =>
		{
			if (!check && AccountDataStore.instance.IsOnGuildMembership())
				ChatManager.Inst.JoinChat(CHAT_TYPE.GUILD, AccountDataStore.instance.guildData._id);
			GlobalPopupManager.Inst.ShowPopup(m_GameUICommunityPrefab, hideNotiAction);
		});
	}

	public void OnClickBtnGuildAttack()
	{
		if (AccountDataStore.instance.IsOnGuildMembership())
		{
			if (Datatable.Inst.IsUnlockGuildBuilding((int)GUILD_BUILDING.BATTLE_TOWER))
			{
				if (AccountDataStore.instance.guildBattleData.placement == 1)
				{
					if (BaseOperatorUnit.instance.sceneType == PlaySceneType.GuildBase)
					{
						PanelManager.inst.PushPanel(PanelType.GuildBattle, false, GuildBattleTabType.BattleJoin);
					}
					else
					{
						AccountDataStore.instance.isGuildBattleJoin = true;		
						BaseOperatorUnit.instance.LoadLevel_Async(PlaySceneType.GuildBase, false);
					}						
				}
				else
				{
					GlobalSystemRewardMsgHandler.instance.ShowMessage(Datatable.Inst.GetUIText(UITextEnum.BATTLE_TOWER_ORGANIZE_MSG));
				}
			}
			else
			{
				string levelStr = Datatable.Inst.GetUIText(UITextEnum.COMMON_LEVEL_NOSIZE, Datatable.Inst.dtGuildBuilding[(int)GUILD_BUILDING.BATTLE_TOWER].EnableHallLv);
				GlobalSystemRewardMsgHandler.instance.ShowMessage(UITextEnum.GUILD_BATTLE_LEVEL_LIMIT_MESSGE, levelStr);
			}
		}
		else
		{
			GlobalSystemRewardMsgHandler.instance.ShowMessage(Datatable.Inst.GetUIText(UITextEnum.GUILD_NON_SIGNUP_MESSAGE));
		}
		HideFolderBtns();
	}

	public void OnClickBtnPostList()
	{
		GiantHandler.Inst.doRequestPostList().SetSuccessAction(ShowPostListPopup);
		HideFolderBtns();
	}

	public void OnClickBtnGameSetting()
	{
		GlobalPopupManager.Inst.ShowPopup(m_GameSettingPrefab);
		HideFolderBtns();
	}

	public void OnClickBtnNotice()
	{
		HiveManager.Inst.ShowPromotion(hive.PromotionType.NEWS, true);
		HideFolderBtns();
	}

	public void OnClickBtnInventory()
	{
		PanelManager.inst.PushPanel(PanelType.Inventory);
		HideFolderBtns();
	}

	public void OnClickBtnPvpUI()
	{
		PanelManager.inst.PushPanel(PanelType.PvpInfo);	
		HideFolderBtns();
	}

	public void OnClickBtnArena()
	{
		if (!AccountDataStore.instance.clientData.isOpenArena)
		{
			GlobalSystemRewardMsgHandler.instance.ShowMessage(Datatable.Inst.GetUIText(UITextEnum.LOBBY_MAIN_AREA_DISABLE_MODE, Datatable.Inst.GetAreaName(Datatable.Inst.settingData.ArenaSearchIDForUnlock)));
			return;
		}

		PanelManager.inst.PushPanel(PanelType.Arena);
		HideFolderBtns();
	}

	public void OnClickBtnDuel()
	{
		if (!AccountDataStore.instance.clientData.isOpenArena)
		{
			GlobalSystemRewardMsgHandler.instance.ShowMessage(Datatable.Inst.GetUIText(UITextEnum.LOBBY_MAIN_AREA_DISABLE_MODE, Datatable.Inst.GetAreaName(Datatable.Inst.settingData.ArenaSearchIDForUnlock)));
			return;
		}

		PanelManager.inst.PushPanel(PanelType.Duel);
		HideFolderBtns();
	}

	public void OnClickBtnWorldBoss()
	{
		PanelManager.inst.PushPanel(PanelType.WorldBoss);
		HideFolderBtns();
	}

	public void OnClickBtnGameFormSetting()
	{
		int idSearch = Datatable.Inst.settingData.DeckSlotEnableArea;
		if (!AccountDataStore.instance.IsExistArea(idSearch))
		{
			GlobalSystemRewardMsgHandler.instance.ShowMessage(UITextEnum.PVP_OPEN_CONDITION, Datatable.Inst.GetAreaName(idSearch));
			return;
		}

		PanelManager.inst.PushPanel(PanelType.GameFormSetting);
		HideFolderBtns();
	}

	public void OnClickBtnFolder()
	{
		if (m_ObjectFolderButtons.activeSelf)
			HideFolderBtns();
		else
			ShowFolderBtns();
	}

	public void OnClickBtnHideFolderButtons()
	{
		HideFolderBtns();
	}

	public void OnClickBtnHotTime()
	{
		GlobalPopupManager.Inst.ShowPopup(m_HotTimePopupPrefab);
	}

	public void OnFolderDeselect(BaseEventData data)
	{
		Giant.Util.Deselect(data, m_ListButtons, HideFolderBtns);
	}

	private void HideFolderBtns()
	{
		m_ObjectFolderButtons.SetActive(false);
		BackButtonManager.Inst.SetBackButtonCB(null);
		m_ImageMenu.sprite = m_IconStateSpriteSet.GetSprite(SpriteType.IconState_MenuFolder_Off);
	}

	private void ShowFolderBtns()
	{
		EventSystem.current.SetSelectedGameObject(m_ObjectFolderButtons);
		m_ObjectFolderButtons.SetActive(true);
		m_ImageMenu.sprite = m_IconStateSpriteSet.GetSprite(SpriteType.IconState_MenuFolder_On);
		BackButtonManager.Inst.SetBackButtonCB(() =>
		{
			HideFolderBtns();
		});
	}

	private void CheckPvpDataRequest()
	{
		if (AccountDataStore.instance.LastPvpDataRequest + (Datatable.Inst.settingData.PvpInfoRequestTime * 1000) < GiantHandler.Inst.serverTime)
		{
			GiantHandler.Inst.doRequestPvpHeroLog().SetSuccessAction(() =>{
				AccountDataStore.instance.LastPvpDataRequest = GiantHandler.Inst.serverTime;
			});
		}
	}

	private void CheckMarketReviewPopup()
	{
		if (HiveManager.Inst.AvailableReviewPopup)
		{
			HiveManager.Inst.ShowReviewPopup();
			HiveManager.Inst.AvailableReviewPopup = false;
		}
	}

	private void UpdateArenaDuelGrayScale()
	{
		bool isActive = !AccountDataStore.instance.clientData.isOpenArena;
		m_ArenaGrayScaleGroup.SetActive(isActive);
		m_DuelGrayScaleGroup.SetActive(isActive);
	}

	private void UpdateGameFormSettingGrayScale()
	{
		m_GameFormSettingGrayScaleGroup.SetActive(!AccountDataStore.instance.IsExistArea(Datatable.Inst.settingData.DeckSlotEnableArea));
	}

	private void UpdatePvpLogNoti()
	{
		m_ObjectPvpNoti.SetActive(AccountDataStore.instance.pvpLog.CheckRevengeNoti());
		UpdateMenuNoti();
	}

	private void UpdateMenuNoti()
	{
		for (int i = 0; i < m_FolderNotiObjs.Length; ++i)
		{
			if (HudManager.inst.IsActiveHudObject(m_FolderNotiObjs[i]))
			{
				switch (m_FolderNotiObjs[i])
				{
				case HudType.PVPButton:
					if (AccountDataStore.instance.pvpLog.CheckRevengeNoti())
					{
						m_ObjectNoti.SetActive(true);
						return;
					}
					break;
				case HudType.AchievementButton:
					if (AccountDataStore.instance.GetAchievementClearCount(0) + AccountDataStore.instance.GetAchievementClearCount(1) > 0)
					{
						m_ObjectNoti.SetActive(true);
						return;
					}
					break;
				case HudType.WorldBossButton:
					if (AccountDataStore.instance.user.WorldBossNotiCheck())
					{
						m_ObjectNoti.SetActive(true);
						return;
					}
					break;
				}	
			}
		}
		m_ObjectNoti.SetActive(false);
	}

	private void CheckWorldBossInfo()
	{
		// OffLineTest
		return;
		StopCoroutine("CheckTime");
		if (AccountDataStore.instance.worldBossSeasonInfo.isStartNextSeason)
		{
			GiantHandler.Inst.doWorldBossSeasonInfo().SetSuccessAction(CheckWorldBossInfo);
		}
		else if (AccountDataStore.instance.worldBossSeasonInfo.isEnableSeasonReward)
		{
			GiantHandler.Inst.doWorldBossSeasonReward().SetSuccessAction(CheckWorldBossInfo);
		}
		else
		{
			StartCoroutine("CheckTime");
		}
	}

	private IEnumerator CheckTime()
	{
		if (AccountDataStore.instance.worldBossSeasonInfo.seasonCalcEndDate > GiantHandler.Inst.serverTime)
		{
			yield return new WaitForSecondsRealtime((AccountDataStore.instance.worldBossSeasonInfo.seasonCalcEndDate - GiantHandler.Inst.serverTime) * 0.001f);
			if (AccountDataStore.instance.worldBossSeasonInfo.isEnableSeasonReward)
				GiantHandler.Inst.doWorldBossSeasonReward().SetSuccessAction(CheckWorldBossInfo);
			else
				CheckWorldBossInfo();
		}
		else if (AccountDataStore.instance.worldBossSeasonInfo.startDateNext != 0)
		{
			yield return new WaitForSecondsRealtime((AccountDataStore.instance.worldBossSeasonInfo.startDateNext - GiantHandler.Inst.serverTime) * 0.001f);
			GiantHandler.Inst.doWorldBossSeasonInfo().SetSuccessAction(()=>{
				if (!AccountDataStore.instance.worldBossSeasonInfo.isStartNextSeason)
					CheckWorldBossInfo();
			});
		}
	}
}

using UnityEngine;
using System;
using System.Collections;

public class LobbySceneHandler : MonoBehaviour
{
	[SerializeField] private DungeonFloorSelectPopup		m_DungeonFloorSelectPopup;
	
	private void Start()
	{
		WorldMapHandler.inst.clickEnterAreaItemAction += OnClickWorldMapEnterAreaItem;
		WorldMapHandler.inst.clickEnterAreaItemInfoAction += OnClickWorldMapEnterAreaItemInfo;

		PanelManager.inst.willShowPanelAction += OnWillShowPanel;

		// 탐사메인으로 바로가지 않아 퀘스트 갱신이 꼬이는 패널들을 제어
		InStageInfoType type = InStageInfoType.None;
		if (BaseOperatorUnit.instance.prevSceneType == PlaySceneType.PreGame)
		{
			type = (InStageInfoType)DeviceSaveManager.Inst.LoadInStageInfoData().Type;
			if (type == InStageInfoType.Arena || type == InStageInfoType.HeroRevenge)
				QuickQuestInfoHandler.isUpdateQuest = false;
		}

		int idSearch = DeviceSaveManager.Inst.LoadCurrentSearchID();
		if (idSearch == 0)
			idSearch = AccountDataStore.instance.GetCurrentIDSearch();
		if (idSearch > 0)
		{
			WorldMapHandler.inst.UpdateOcean(AccountDataStore.instance.GetAreaData(idSearch).difficulty);
			PanelManager.inst.PushPanel(PanelType.ExplorationMain, false, idSearch);
		}
		else
		{
			PanelManager.inst.PushPanel(PanelType.LobbyMain);
		}

		if (BaseOperatorUnit.instance.prevSceneType == PlaySceneType.PreGame)
		{
			switch (type)
			{
			case InStageInfoType.HeroRevenge:
				PanelManager.inst.PushPanel(PanelType.PvpInfo, false, PvpMainTabType.PvpDefenseLog);
				break;
			case InStageInfoType.Arena:
				PanelManager.inst.PushPanel(PanelType.LobbyMain, false, -1, false, true);
				PanelManager.inst.PushPanel(PanelType.Arena);
				break;
			}
			QuickQuestInfoHandler.isUpdateQuest = true;
		}
			
		if (!PanelManager.IsShowCurrentPanel(PanelType.ExplorationMain))
			SoundManager.instance.PlayBGM(SoundBGM.ExplorationNormal);
		
		UITutorialEventManager.instance.CheckTrigger(EVENT_TRIGGER.INITIAL);
		UITutorialEventManager.instance.CheckAllTrigger();

		// OffLineTest
		return;
		if (!ChatManager.Inst.InitNotice)
			GameManager.Inst.StartCheckNotice();

#if !UNITY_EDITOR && !HIDDEN_LOGIN_TEST
		if(!HiveManager.Inst.IsIAPInit())
		{
			HiveManager.Inst.ConnectMarket((success) =>
			{
				HiveManager.Inst.RequestProductionInfo((resultSuccess)=>{
					if(resultSuccess)
					{
						GameManager.Inst.RestoreInappPurchase(null);
					}
				});	
			});	
		}	
#endif
	}

	private void OnDestroy()
	{
		if (AutoExplorationManager.Inst.isPlaying)
			AutoExplorationManager.Inst.HideAutoExplorationPopup();
		
		WorldMapHandler.inst.clickEnterAreaItemAction -= OnClickWorldMapEnterAreaItem;
		WorldMapHandler.inst.clickEnterAreaItemInfoAction -= OnClickWorldMapEnterAreaItemInfo;

		PanelManager.inst.willShowPanelAction -= OnWillShowPanel;
	}

	private void OnClickWorldMapEnterAreaItem(AreaEnterItemType type, int idDungeon, int idSearch)
	{
		if (type == AreaEnterItemType.Arena)
		{
			PanelManager.inst.PushPanel(PanelType.GameFormEdit, false, AccountDataStore.instance.arena.epData, Vector3.zero);
			return;
		}

		if (AccountDataStore.instance.IsExistEps(idSearch))
		{
			if (type != AreaEnterItemType.Dungeon || !AccountDataStore.instance.IsClearDungeon(idSearch))
			{
				if (AccountDataStore.instance.GetCurrentIDSearch() == idSearch)
					PanelManager.inst.PopPanel();
				else
					PanelManager.inst.PushPanel(PanelType.ExplorationMain, true, idSearch);
				return;
			}
		}

		if (type == AreaEnterItemType.Dungeon)
			ClickEnterDungeonItem(idDungeon, idSearch);
		else
			ClickEnterAreaItem(idSearch);
	}
	
	private void ClickEnterDungeonItem(int idDungeon, int idSearch)
	{
		// 한 번도 탐사하지 않았거나 던전 초기화를 한 경우 던전 시작층 선택 팝업을 띄운다.
		GiantAreaData areaData = AccountDataStore.instance.GetCurrentDungeonArea(idDungeon);
		
		Action<PopupUI, int, bool> okAction = (curPopup, idNewSearch, isReset) => {
			// 소모 비용 체크
			if (!AccountDataStore.instance.CheckEnoughCostType(PlayerDataFieldType.DungeonToken, 1))
				return;
			// 던전 탐사
			AccountDataStore.instance.DoSearchArea(idNewSearch, isReset, (isLevelUp, isFeverReadyStart, isChangeFloor) => {
				if (curPopup != null)
					curPopup.Hide();
				MoveExplorationMain(idNewSearch, isLevelUp, isFeverReadyStart, isChangeFloor);
			});
		};
		
		Action<bool> resetAction = (isReset) => {
			// 던전 시작층 선택 팝업
			DungeonFloorSelectPopup popup = GlobalPopupManager.Inst.ShowPopup(m_DungeonFloorSelectPopup, idDungeon);
			popup.SetOnClickFloorItem((idNewSearch) => { okAction(popup, idNewSearch, isReset); });
		};
		
		if (areaData == null)
		{
			resetAction(false);
		}
		else if (areaData.eps.Count <= 0)
		{
			SystemPopupManager.Inst.ShowCheckLeftSystemPopup(Datatable.Inst.GetUIText(UITextEnum.LOBBY_MAIN_DUNGEON_CONTINUE, areaData.nowFloor), Datatable.Inst.GetUIText(UITextEnum.COMMON_DUNGEON_RESET)).SetOnClickOKButton((isChecked) =>
			{
				if (isChecked)
					resetAction(true);
				else
					// 던전 탐사
					okAction(null, areaData.idSearch, false);
			});
		}
		else
		{
			MoveExplorationMain(areaData.idSearch);
		}
	}
	
	private void ClickEnterAreaItem(int idSearch)
	{
		// 소모 비용 체크
		int cost = Datatable.Inst.GetAreaCost(idSearch);
		if (!AccountDataStore.instance.CheckEnoughCostType(PlayerDataFieldType.Gold, cost))
			return;
		// 지역 탐사
		AccountDataStore.instance.DoSearchArea(idSearch, false, (isLevelUp, isFeverReadyStart, isChangeFloor) => { MoveExplorationMain(idSearch, isLevelUp, isFeverReadyStart, isChangeFloor); } );
	}
	
	private void MoveExplorationMain(int idSearch, bool isLevelUp = false, bool isFeverReadyStart = false, bool isChangeFloor = false)
	{
		PanelManager.inst.PushPanel(PanelType.ExplorationMain, true, idSearch, true, isLevelUp, isFeverReadyStart, isChangeFloor);
	}

	private void OnClickWorldMapEnterAreaItemInfo(int idSearch)
	{
		PanelManager.inst.PushPanel(PanelType.ExplorationMonsterInfo, false, idSearch);
	}

	private void OnWillShowPanel(PanelManager.PanelData panelData)
	{
		switch (panelData.type)
		{
		case PanelType.LobbyMain:
			WorldMapHandler.inst.ExitArea(false);
			WorldMapHandler.inst.SetStatic(false);
			WorldMapHandler.inst.SetBlur(WorldMapHandler.BlurType.None);
			break;

		case PanelType.ExplorationMain:
			int idSearch = (int)panelData.panelValues[0];
			WorldMapHandler.inst.EnterArea(idSearch, (panelData.panelValues.Length > 1) ? (bool)panelData.panelValues[1] : false, true);
			if (Datatable.Inst.GetIDDungeon(idSearch) > 0)
				SetWorldMapStaticImage(SpriteType.Exploration_World_Map_Dungeon);
			else
			{
				WorldMapHandler.inst.SetStatic(false);
				WorldMapHandler.inst.SetBlur(WorldMapHandler.BlurType.Low);
			}
			break;

		case PanelType.ExplorationMonsterInfo:
			WorldMapHandler.inst.EnterArea((int)panelData.panelValues[0], false, false);
			WorldMapHandler.inst.SetBlur(WorldMapHandler.BlurType.Low);
			break;

		case PanelType.DetailHero:
		case PanelType.Gather:
		case PanelType.Purchase:
		case PanelType.Achievement:
		case PanelType.Core:
		case PanelType.Inventory:
		case PanelType.GuildTarget:
			WorldMapHandler.inst.SetBlur(WorldMapHandler.BlurType.High);
			break;

		case PanelType.GameFormEdit:
			if (panelData.panelValues != null && panelData.panelValues.Length >= 2)
			{
				GiantEPData epData = (GiantEPData)panelData.panelValues[0];
				switch ((EP_CATEGORY)epData.category)
				{
				case EP_CATEGORY.PVP_ARENA_BATTLE:
					WorldMapHandler.inst.EnterArena();
					break;
				case EP_CATEGORY.STAGE_WORLDBOSS:
					SetWorldMapStaticImage(SpriteType.Exploration_World_Map_WorldBoss);
					break;
				}
			}
			WorldMapHandler.inst.SetBlur(WorldMapHandler.BlurType.Low);
			break;
			
		case PanelType.GameFormSetting:
			WorldMapHandler.inst.SetBlur(WorldMapHandler.BlurType.High);
			break;

		case PanelType.Arena:
			SetWorldMapStaticImage(SpriteType.Exploration_World_Map_Arena);
			break;

		case PanelType.WorldBoss:
			SetWorldMapStaticImage(SpriteType.Exploration_World_Map_WorldBoss);
			break;
		}
	}

	private void SetWorldMapStaticImage(SpriteType spriteType)
	{
		WorldMapHandler.inst.SetStatic(true);
		WorldMapHandler.inst.SetStaticImage(spriteType);
	}
}
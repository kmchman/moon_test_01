using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using STDOTweenExtensions;
using STExtensions;
using EnumExtensions;

public class GameFormEditPanel : Panel, INeedCheckItems
{
	// Popup_GameResult 의 한 번 더 버튼과 동일해야 하고, 모든 전투에 대해 체크한다.
	private static int[] CheckIDItems = new int[] { SpecialItemID.Gold, SpecialItemID.Cash, SpecialItemID.Ether, SpecialItemID.CoreInventory };
	private static int[] CheckCount = new int[] { 0, 0, 0, 0 };

	[SerializeField] private GameFormSelectPopup m_GameFormSelectPopupPrefab;

	[SerializeField] private GameObject m_EnemyObject;
	[SerializeField] private Text m_EnemyNameText;

	[SerializeField] private RewardSlotsItem m_RewardSlotsItem;

	[SerializeField] private GameObject m_HeroListPrefab;
	[SerializeField] private Transform m_HeroListParent;

	[SerializeField] private GameObject m_EnemyCardObject;
	[SerializeField] private GameObject m_EnemyCardPrefab;
	[SerializeField] private Vector3 m_EnemyCardPos;
	[SerializeField] private Vector3 m_BaseEnemyCardPos;
	[SerializeField] private GameObject m_GameFormEditCharSlotPrefab;
	[SerializeField] private Transform m_GameFormEditCharSlotParent;
	[SerializeField] private Button m_StartBattleBtn;
	[SerializeField] private STImage m_StartBattleDecoImage;
	[SerializeField] private GameObject m_RetryObj;
	[SerializeField] private UICostButtonLinker m_RetryCost;
	[SerializeField] private UICostButtonLinker m_StartInstantCost;

	[SerializeField] private STText m_StageEnemyCombatPointText;
	[SerializeField] private STText [] m_MyCombatPointTexts;	// 0 : 한 줄, 1 : 두 줄

	// Pvp
	[SerializeField] private GameObject m_PvpObject;
	[SerializeField] private STText m_EnemyLevelText;
	[SerializeField] private PvpGradeItem m_EnemyPvpGradeItem;
	[SerializeField] private Transform m_EnemyCharSlotParent;
	[SerializeField] private STText m_PvpCombatPointText;
	[SerializeField] private STText m_MyLevelText;
	[SerializeField] private STText m_MyNameText;
	[SerializeField] private PvpGradeItem m_MyPvpGradeItem;

	// Gahter
	[SerializeField] private GameFormEditGatherInfoItem m_GatherInfoItem;

	// Race
	[SerializeField] private GameObject m_RaceDescObject;
	[SerializeField] private STText m_RaceDescText;
	[SerializeField] private GameFormEditRaceBoxItem m_RaceBoxItem;

	// World Boss
	[SerializeField] private GameObject m_WorldBossObject;
	[SerializeField] private GameObject m_WorldBossStartObject;
	[SerializeField] private STText m_WorldBossTokenText;

	// Guild Battle
	[SerializeField] private GameObject m_GuildBattleObject;

	// Fatal Skill
	[SerializeField] private STGauge m_FatalSkillGauge;

	// Tab
	[SerializeField] private GameFormEditTabInfoItem m_TabInfoItem;
	
	// AutoSetting
	[SerializeField] private GameObject m_AutoSkillEnableSettingButton;
	[SerializeField] private GameObject m_AutoSkillEnableOnImage;
	[SerializeField] private GameObject m_AutoSkillEnableOffImage;

	// GameForm
	[SerializeField] private GameObject m_GameFormNumberObject;
	[SerializeField] private STText m_GameFormNumberText;
	[SerializeField] private GameObject m_AutoExplorationObject;
	[SerializeField] private GameObject m_GameFormSelectButtonObject;

	[SerializeField] private SpriteSet m_IconSpriteSet;

#if UNITY_EDITOR || PREGAME_TEST
	public Action onClickStartAction;
#endif

	private HeroList m_HeroList;

	private GiantHeroData [] sortedHeroDatas = null;
	private int [] sortedHeroIDs = null;
	private int [] gameFormationIds = new int[Constant.FormMemberMax];
	private List<int> eliteIndices = new List<int>();
	private List<int> tempEliteIndices = null;
	private List<int> supportIndices = new List<int>();
	private Dictionary<int, double> myCombatPoints = new Dictionary<int, double>();	// Key : idHero, Value : combat point
	private List<GameFormEditCharListItem> m_EliteSlots = new List<GameFormEditCharListItem>();
	private List<GameFormEditCharListItem> m_SupportSlots = new List<GameFormEditCharListItem>();
	private List<GameFormEditCharListItem> m_PvpEliteSlots = new List<GameFormEditCharListItem>();
	private bool[] m_IsAutoSkillEnables = new bool[Constant.FormEliteMax];

	private GiantEPData m_EpData;
	private ExplorationEnemyCardItem m_EnemyCardItem;
	private GameFormBattleType m_BattleType;
	private int m_GameFormIndex;
	private GameFormData m_GameFormData;
	private int m_MaxEliteHeroCount;
	private int m_MaxSupportHeroCount;
	private bool isValidForm;
	private bool isBattleCost;
	private bool isUsableHero;
	private bool isShowAutoSkillEnable;
	private int battleCostItemID;
	private int battleCostValue;
	private int curMyCombatPointIndex;

	private GameObject stageEnemyCombatPointObj;
	private GameObject [] myCombatPointObjs;

	protected override void Awake()
	{
		base.Awake();

		m_EnemyCardItem = Giant.Util.MakeItem<ExplorationEnemyCardItem>(m_EnemyCardPrefab, m_EnemyCardObject.transform, false);

		stageEnemyCombatPointObj = m_StageEnemyCombatPointText.transform.parent.gameObject;
		myCombatPointObjs = new GameObject [m_MyCombatPointTexts.Length];
		for (int i = 0; i < m_MyCombatPointTexts.Length; ++i)
			myCombatPointObjs[i] = m_MyCombatPointTexts[i].transform.parent.gameObject;
	}

	protected void OnDisable()
	{
		for (int i = 0; i < m_EliteSlots.Count; ++i)
		{
			m_EliteSlots[i].SetGrayScale(false);
			m_EliteSlots[i].ShowAutoSkillEnableSettingItem(false);
			GameObjectPool.Push(m_EliteSlots[i].gameObject);
		}

		for (int i = 0; i < m_SupportSlots.Count; ++i)
		{
			m_SupportSlots[i].SetGrayScale(false);
			m_SupportSlots[i].ShowAutoSkillEnableSettingItem(false);
			GameObjectPool.Push(m_SupportSlots[i].gameObject);
		}

		for (int i = 0; i < m_PvpEliteSlots.Count; ++i)
		{
			m_PvpEliteSlots[i].SetGrayScale(false);
			m_PvpEliteSlots[i].ShowAutoSkillEnableSettingItem(false);
			GameObjectPool.Push(m_PvpEliteSlots[i].gameObject);
		}

		m_EliteSlots.Clear();
		m_SupportSlots.Clear();
		m_PvpEliteSlots.Clear();

		m_RewardSlotsItem.Release();
	}

	protected override void Show(params object[] values)
	{
		ShowPanel((GiantEPData)values[0], (Vector3)values[1]);
	}

	protected override void Hide(params object[] values)
	{
		switch ((EP_CATEGORY)m_EpData.category)
		{
		case EP_CATEGORY.GATHER_HERB:
		case EP_CATEGORY.GATHER_VEIN:
		case EP_CATEGORY.GATHER_TREE:
			break;
		default:
			SaveDeck();
			break;
		}

		HidePanel();
	}

	public void ShowPanel(GiantEPData epData, Vector3 cardPosition)
	{
		myCombatPoints.Clear();
	
		SetCommon(epData);

		MakeHeroList();
		m_HeroList.onClickHeroListItem += OnClickHeroListItem;

		isShowAutoSkillEnable = false;
		SetAutoSkillEnableButton(isShowAutoSkillEnable);

		m_AutoSkillEnableSettingButton.gameObject.SetActive(!epData.isGather && AccountDataStore.instance.IsExistArea(Datatable.Inst.settingData.AutoSettingEnableArea));

		if (m_BattleType != GameFormBattleType.None)
		{
			bool isGameFormAutoIndex;
			bool isRaceBattle = (EP_CATEGORY)m_EpData.category == EP_CATEGORY.RACE_BATTLE;
			if (!isRaceBattle && AutoExplorationManager.Inst.isPlaying)
			{
				isGameFormAutoIndex = true;
				DeviceSaveManager.Inst.LoadGameFormAutoData(m_BattleType.GetGameFormType(), out m_GameFormIndex, out m_GameFormData);
			}
			else
				DeviceSaveManager.Inst.LoadGameFormData(m_BattleType, m_EpData.myRace, out m_GameFormData, out m_GameFormIndex, out isGameFormAutoIndex);

			if (!isRaceBattle && AccountDataStore.instance.IsExistArea(Datatable.Inst.settingData.DeckSlotEnableArea))
			{
				m_GameFormNumberObject.SetActive(true);
				m_GameFormNumberText.text = (m_GameFormIndex + 1).ToString();
				m_AutoExplorationObject.SetActive(isGameFormAutoIndex);
				m_GameFormSelectButtonObject.SetActive(true);
			}
			else
			{
				m_GameFormNumberObject.SetActive(false);
				m_GameFormSelectButtonObject.SetActive(false);
			}
		}
		else
		{
			m_GameFormNumberObject.SetActive(false);
			m_GameFormSelectButtonObject.SetActive(false);
		}

		CONTENT_TYPE contentType = CONTENT_TYPE.NONE;

		switch ((EP_CATEGORY)epData.category)
		{
		case EP_CATEGORY.STAGE_MONSTER:
		case EP_CATEGORY.STAGE_BOSS:
		case EP_CATEGORY.STAGE_EXTRABOSS:
		case EP_CATEGORY.STAGE_DUNGEON_TARGET:
			ShowStage(epData, cardPosition);
			break;

		case EP_CATEGORY.PVP_HERO_BATTLE:
		case EP_CATEGORY.PVP_HERO_REVENGE:
			contentType = CONTENT_TYPE.PVP;
			ShowPvP(epData);
			break;

		case EP_CATEGORY.PVP_BASE_BATTLE:
		case EP_CATEGORY.PVP_BASE_REVENGE:
			contentType = CONTENT_TYPE.RAID;
			ShowBaseRaid(epData, cardPosition);
			break;

		case EP_CATEGORY.GATHER_HERB:
		case EP_CATEGORY.GATHER_VEIN:
		case EP_CATEGORY.GATHER_TREE:
			ShowGather(epData, cardPosition);
			break;
			
		case EP_CATEGORY.RACE_BATTLE:
			ShowRace(epData, cardPosition);
			break;

		case EP_CATEGORY.STAGE_WORLDBOSS:
			ShowWorldBoss(epData);
			break;

		case EP_CATEGORY.GUILD_BATTLE:
			ShowGuildBattle(epData);
			break;
		}

		AccountDataStore.instance.currentContentType = contentType;

		StartCoroutine("CheckAutoExploration");

		CompleteShowAnimation();
	}

	private void HidePanel()
	{
#if UNITY_EDITOR || PREGAME_TEST
		if (m_PanelManager != null)
			SavePanelData(m_EpData, m_EnemyCardItem.transform.position);
#else
		SavePanelData(m_EpData, m_EnemyCardItem.transform.position);
#endif
		
		m_HeroList.onClickHeroListItem -= OnClickHeroListItem;
		m_HeroList.Close();

		GameObjectPool.Push(m_HeroList.gameObject);
		m_HeroList = null;

		AccountDataStore.instance.currentContentType = CONTENT_TYPE.NONE;

		CompleteHideAnimation();
	}

	private void SetCommon(GiantEPData epData)
	{
		m_EpData = epData;

		m_BattleType = epData.gameFormBattleType;
		m_MaxEliteHeroCount = epData.maximumEliteHeroCount;
		m_MaxSupportHeroCount = epData.maximumSupportHeroCount;

		m_EnemyObject.SetActive(false);
		m_EnemyCardObject.SetActive(false);
		m_PvpObject.SetActive(false);
		m_RewardSlotsItem.gameObject.SetActive(false);
		m_RaceDescObject.SetActive(false);
		m_RaceBoxItem.gameObject.SetActive(false);
		m_WorldBossObject.SetActive(false);
		m_WorldBossStartObject.SetActive(false);
		m_GuildBattleObject.SetActive(false);

		m_StartBattleDecoImage.gameObject.SetActive(false);
		m_StartBattleBtn.image.material = null;
		m_StartInstantCost.gameObject.SetActive(false);
		m_RetryObj.SetActive(false);

		if (m_EpData.isGather)
		{
//			HudManager.inst.SetTitle(UITextEnum.GAMEFORMEDIT_GATHER_TITLE);
			m_GatherInfoItem.gameObject.SetActive(true);
			m_FatalSkillGauge.gameObject.SetActive(false);
		}
		else
		{
//			HudManager.inst.SetTitle(UITextEnum.GAMEFORMEDIT_NORMAL_TITLE);
			m_GatherInfoItem.gameObject.SetActive(false);
			m_FatalSkillGauge.gameObject.SetActive(true);
			m_FatalSkillGauge.fillAmount = (float)AccountDataStore.instance.clientData.fatalSkillPoint / Constant.FatalSkillPointMax;
		}

		stageEnemyCombatPointObj.SetActive(false);
	}

	private void MakeHeroList()
	{
		if (m_HeroList != null)
			return;

		m_HeroList = GameObjectPool.Pop(m_HeroListPrefab, false, m_HeroListParent, false).GetComponent<HeroList>();
		m_HeroList.rectTransformRef.SetAsFirstSibling();
	}

	private IEnumerator CheckAutoExploration()
	{
		if (!AutoExplorationManager.Inst.isPlaying)
			yield break;

		yield return new WaitForSeconds(Datatable.Inst.settingData.AutoExplorationWaitTime);

		m_StartBattleBtn.onClick.Invoke();

		if (!isValidForm || !isUsableHero)
		{
			yield return new WaitForSeconds(Datatable.Inst.settingData.AutoExplorationWaitTime);

			PanelManager.inst.PopPanel();
		}
		else
			yield return AutoExplorationManager.Inst.WaitForResponse();
	}

	private void ShowStage(GiantEPData epData, Vector3 cardPosition)
	{
		m_EnemyObject.SetActive(true);
		stageEnemyCombatPointObj.SetActive(true);

		if (epData.GetMoreBattleCost(false, out battleCostItemID, out battleCostValue))
		{
			isBattleCost = true;
			m_RetryObj.SetActive(true);
			m_RetryCost.SetEnoughCostType(battleCostItemID, battleCostValue);
			m_StartBattleDecoImage.gameObject.SetActive(false);
		}
		else
		{
			isBattleCost = false;
			m_StartBattleDecoImage.gameObject.SetActive(true);
			m_StartBattleDecoImage.sprite = m_IconSpriteSet.GetSprite(SpriteType.Icon_Battle);
		}

		// items
		m_RewardSlotsItem.gameObject.SetActive(true);
		m_RewardSlotsItem.SetRewards(epData.rewards);

		// enemy info
		m_EnemyNameText.text = epData.nickname;
		m_StageEnemyCombatPointText.text = Datatable.Inst.GetCombatPointString(epData.combatPoint);

		// 탐사 카드 이동 연출
		m_EnemyCardObject.SetActive(true);
		m_EnemyCardItem.SetData(epData, false);
		m_EnemyCardItem.transform.position = cardPosition;
		m_EnemyCardItem.transform.DOLocalMoveXY(m_EnemyCardPos.x, m_EnemyCardPos.y, 1f);

		// tab
		m_TabInfoItem.SetStageEPData(epData);

		InitForm();

		UITutorialEventManager.instance.CheckTrigger(EVENT_TRIGGER.JOIN_FORM_EDIT);
	}

	private void ShowPvP(GiantEPData epData)
	{
		m_EnemyObject.SetActive(true);
		m_PvpObject.SetActive(true);
		m_StartBattleDecoImage.gameObject.SetActive(true);
		m_StartBattleDecoImage.sprite = m_IconSpriteSet.GetSprite(SpriteType.Icon_Battle);

		if (epData.GetMoreBattleCost(false, out battleCostItemID, out battleCostValue))
		{
			isBattleCost = true;
			m_RetryObj.SetActive(true);
			m_RetryCost.SetEnoughCostType(battleCostItemID, battleCostValue);
			m_StartBattleDecoImage.gameObject.SetActive(false);
		}
		else
		{
			isBattleCost = false;
			m_StartBattleDecoImage.gameObject.SetActive(true);
			m_StartBattleDecoImage.sprite = m_IconSpriteSet.GetSprite(SpriteType.Icon_Battle);
		}

		GiantEPPVPData pvp = epData.pvp;

		// enemy info
		m_EnemyLevelText.text = pvp.level.ToString();
		m_EnemyNameText.text = pvp.GetNickname();
		m_EnemyPvpGradeItem.SetData(pvp.grade);
		m_PvpCombatPointText.text = Datatable.Inst.GetCombatPointString(epData.combatPoint, true);

		// my info
		GiantUserData user = AccountDataStore.instance.user;
		m_MyLevelText.text = user.level.ToString();
		m_MyNameText.text = user.GetNickname();
		m_MyPvpGradeItem.SetData(user.pvpHeroGrade);

		// tab
		m_TabInfoItem.HideChangeMode();

		InitPvpForm();

		InitForm();

		UITutorialEventManager.instance.CheckTrigger(EVENT_TRIGGER.JOIN_FORM_EDIT);
	}

	private void ShowBaseRaid(GiantEPData epData, Vector3 cardPosition)
	{
		isBattleCost = false;

		m_EnemyObject.SetActive(true);
		m_StartBattleDecoImage.gameObject.SetActive(true);
		m_StartBattleDecoImage.sprite = m_IconSpriteSet.GetSprite(SpriteType.Icon_Battle);

		GiantPlantData plantData = epData.baseRaid.mainPlant;

		// enemy info
		m_EnemyNameText.text = plantData.name;

		// 탐사 카드 이동 연출
		m_EnemyCardObject.SetActive(true);
		m_EnemyCardItem.SetData(epData, false);
		m_EnemyCardItem.transform.position = cardPosition;

		switch ((EP_CATEGORY)epData.category)
		{
		case EP_CATEGORY.PVP_BASE_BATTLE:
			m_EnemyCardItem.transform.DOLocalMoveXY(m_BaseEnemyCardPos.x, m_BaseEnemyCardPos.y, 1f);
			break;

		case EP_CATEGORY.PVP_BASE_REVENGE:
			m_EnemyCardItem.transform.localPosition = new Vector3(m_BaseEnemyCardPos.x, m_BaseEnemyCardPos.y, 1f);
			break;
		}

		// tab
		m_TabInfoItem.SetRaidEPData(epData);
		
		InitForm();

		UITutorialEventManager.instance.CheckTrigger(EVENT_TRIGGER.JOIN_FORM_EDIT);
	}

	private void ShowGather(GiantEPData epData, Vector3 cardPosition)
	{
		isBattleCost = false;

		m_StartBattleDecoImage.gameObject.SetActive(true);
		switch ((EP_CATEGORY)epData.category)
		{
		case EP_CATEGORY.GATHER_HERB:
			m_StartBattleDecoImage.sprite = m_IconSpriteSet.GetSprite(SpriteType.Icon_Herb);
			break;
		case EP_CATEGORY.GATHER_VEIN:
			m_StartBattleDecoImage.sprite = m_IconSpriteSet.GetSprite(SpriteType.Icon_Vein);
			break;
		case EP_CATEGORY.GATHER_TREE:
			m_StartBattleDecoImage.sprite = m_IconSpriteSet.GetSprite(SpriteType.Icon_Tree);
			break;
		default:
			m_StartBattleDecoImage.gameObject.SetActive(false);
			break;
		}

		m_EnemyObject.SetActive(true);

		m_GatherInfoItem.gameObject.SetActive(true);
		m_GatherInfoItem.SetHero(null);
		m_GatherInfoItem.SetData(epData);

		m_RewardSlotsItem.gameObject.SetActive(true);
		m_RewardSlotsItem.SetRewards(epData.rewards);

		for (int i = 0; i < myCombatPointObjs.Length; ++i)
			myCombatPointObjs[i].SetActive(false);
		
		// enemy info
		m_EnemyNameText.text = epData.nickname;

		// 탐사 카드 이동 연출
		m_EnemyCardObject.SetActive(true);
		m_EnemyCardItem.SetData(epData, false);
		m_EnemyCardItem.transform.position = cardPosition;
		m_EnemyCardItem.transform.DOLocalMoveXY(m_EnemyCardPos.x, m_EnemyCardPos.y, 1f);

		// tab
		m_TabInfoItem.HideChangeMode();

		int idHero = 0;
		if (AutoExplorationManager.Inst.isPlaying)
		{
			GameFormData data5 = DeviceSaveManager.Inst.LoadGameFormAutoData(GameFormType.Deck5);
			GameFormData data10 = DeviceSaveManager.Inst.LoadGameFormAutoData(GameFormType.Deck10);
			GameFormData dataPvp = DeviceSaveManager.Inst.LoadGameFormAutoData(GameFormType.Pvp);

			GiantHeroData [] sortedHeroDatas = AccountDataStore.instance.GetAllHeroDatas();
			Array.Sort(sortedHeroDatas, GiantHeroData.Sort);

			for (int i = sortedHeroDatas.Length - 1; i >= 0; --i)
			{
				GiantHeroData heroData = sortedHeroDatas[i];
				if (heroData.isGatherDispatch || (data5 != null && FindIndex(data5.idHeroes, heroData.idHero) >= 0) || (data10 != null && FindIndex(data10.idHeroes, heroData.idHero) >= 0) || (dataPvp != null && FindIndex(dataPvp.idHeroes, heroData.idHero) >= 0))
					continue;
				idHero = heroData.idHero;
				break;
			}
		}

		if (idHero > 0)
		{
			SelectedHeroData [] selectedHeroDatas = new SelectedHeroData[1];
			SelectedHeroData data = new SelectedHeroData();
			data.idHero = idHero;
			data.selectType = HeroSelectType.Elite;
			selectedHeroDatas[0] = data;
			m_HeroList.OpenHeroList(HeroSelectMode.Gather, 0, 1, selectedHeroDatas, true, true);
		}
		else
			m_HeroList.OpenHeroList(HeroSelectMode.Gather, 0, 1, null, false, false);

		CheckStartGather();

		UITutorialEventManager.instance.CheckTrigger(EVENT_TRIGGER.JOIN_ASSIGN_HERO_SELECT);
	}
	
	private void ShowRace(GiantEPData epData, Vector3 cardPosition)
	{
		isBattleCost = false;

		m_EnemyObject.SetActive(true);
		m_RaceDescObject.SetActive(true);
		m_RaceDescText.text = Datatable.Inst.GetUIText(UITextEnum.GAMEFORMEDIT_RACEBATTLE_DESC, Datatable.Inst.GetUIText(UITextEnum.RACE_START + epData.myRace));
		
		m_RaceBoxItem.SetData();
		
		m_StartBattleDecoImage.gameObject.SetActive(true);
		m_StartBattleDecoImage.sprite = m_IconSpriteSet.GetSprite(SpriteType.Icon_Battle);

		// enemy info
		m_EnemyNameText.text = epData.nickname;

		// 탐사 카드 이동 연출
		m_EnemyCardObject.SetActive(true);
		m_EnemyCardItem.SetData(epData, false);
		m_EnemyCardItem.transform.position = cardPosition;
		m_EnemyCardItem.transform.DOLocalMoveXY(m_BaseEnemyCardPos.x, m_BaseEnemyCardPos.y, 1f);

		// tab
		m_TabInfoItem.SetRaceEPData(epData);

		InitForm();

		UITutorialEventManager.instance.CheckTrigger(EVENT_TRIGGER.JOIN_FORM_EDIT);
	}

	private void ShowWorldBoss(GiantEPData epData)
	{
		m_EnemyObject.SetActive(true);
		m_WorldBossObject.SetActive(true);

		int itemCount = AccountDataStore.instance.GetItemCount(SpecialItemID.WorldBossToken);
		if (itemCount > 0)
		{
			isBattleCost = false;
			m_WorldBossStartObject.SetActive(true);
			m_WorldBossTokenText.text = Util.StringFormat(StringFormat.COMMON_CURRENT_MAX, itemCount, Datatable.Inst.settingData.WorldBossToken);
		}
		else
		{
			isBattleCost = true;
			battleCostItemID = SpecialItemID.Cash;
			battleCostValue = Datatable.Inst.settingData.WorldBossTokenCash;
			m_StartInstantCost.gameObject.SetActive(true);
			m_StartInstantCost.SetEnoughCostType(battleCostItemID, battleCostValue);
		}
		
		// enemy info
		m_EnemyNameText.text = epData.nickname;

		// tab
		m_TabInfoItem.SetWorldBossEPData(epData);

		InitForm();

		UITutorialEventManager.instance.CheckTrigger(EVENT_TRIGGER.JOIN_FORM_EDIT);
	}

	private void ShowGuildBattle(GiantEPData epData)
	{
		isBattleCost = false;

		m_GuildBattleObject.SetActive(true);
		m_StartBattleDecoImage.gameObject.SetActive(true);
		m_StartBattleDecoImage.sprite = m_IconSpriteSet.GetSprite(SpriteType.Icon_Battle);

		// tab
		m_TabInfoItem.SetGuildBattleEPData(epData);

		InitForm();

		StartCoroutine("UpdateFormGrayScale");

		UITutorialEventManager.instance.CheckTrigger(EVENT_TRIGGER.JOIN_FORM_EDIT);
	}

	private void InitForm()
	{
		curMyCombatPointIndex = (m_MaxSupportHeroCount > 0) ? 1 : 0;

		for (int i = 0; i < myCombatPointObjs.Length; ++i)
			myCombatPointObjs[i].SetActive(i == curMyCombatPointIndex);
		
		for (int i = 0; i < m_MaxEliteHeroCount; ++i)
		{
			GameFormEditCharListItem item = GameObjectPool.Pop(m_GameFormEditCharSlotPrefab, false, m_GameFormEditCharSlotParent, false).GetComponent<GameFormEditCharListItem>();
			item.Init(true, i, SelectFormMemberSlot);
			m_EliteSlots.Add(item);
		}

		for (int i = 0; i < m_MaxSupportHeroCount; ++i)
		{
			GameFormEditCharListItem item = GameObjectPool.Pop(m_GameFormEditCharSlotPrefab, false, m_GameFormEditCharSlotParent, false).GetComponent<GameFormEditCharListItem>();
			item.Init(false, m_MaxEliteHeroCount + i, SelectFormMemberSlot);
			m_SupportSlots.Add(item);
		}

		bool isRaceBattle = (EP_CATEGORY)m_EpData.category == EP_CATEGORY.RACE_BATTLE;

		if (m_GameFormData == null)
			m_GameFormData = m_BattleType.GetNewGameFormData();
		Array.Copy(m_GameFormData.idHeroes, gameFormationIds, m_GameFormData.idHeroes.Length);
		Array.Copy(m_GameFormData.isAutoSkillEnables, m_IsAutoSkillEnables, m_GameFormData.isAutoSkillEnables.Length);

		if (sortedHeroDatas == null || sortedHeroDatas.Length != AccountDataStore.instance.heroes.Count)
		{
			sortedHeroDatas = AccountDataStore.instance.GetAllHeroDatas();
			Array.Sort(sortedHeroDatas, GiantHeroData.SortByProtect);
			sortedHeroIDs = new int[sortedHeroDatas.Length];
			for (int i = 0; i < sortedHeroDatas.Length; ++i)
				sortedHeroIDs[i] = sortedHeroDatas[i].idHero;
		}
		else
		{
			for (int i = 0; i < sortedHeroDatas.Length; ++i)
				sortedHeroDatas[i] = AccountDataStore.instance.GetHeroData(sortedHeroDatas[i].idHero);
		}

		eliteIndices.Clear();
		supportIndices.Clear();

		// 종족 전투가 아닌 경우에만 편성 가능한 주전 슬롯보다 보유 히어로가 더 적거나 같은 경우 편성 가능한 빈 슬롯에 편성되지 않은 히어로를 자동 편성시킨다.
		bool isAuto = !isRaceBattle && AccountDataStore.instance.GetHeroCount() <= m_MaxEliteHeroCount;
		bool [] isSelectSlots = isAuto ? new bool [sortedHeroDatas.Length] : null;
		int index, idHero;
		for (int i = 0; i < Constant.FormMemberMax; ++i)
		{
			if (!IsValidHeroSlot(i))
				continue;
			idHero = gameFormationIds[i];
			if (idHero != 0 && FindIndex(gameFormationIds, idHero) == i && (index = FindIndex(sortedHeroIDs, idHero)) >= 0)
			{
				if (isSelectSlots != null)
					isSelectSlots[index] = true;
				AddItem(i, index, idHero);
			}
			else
				gameFormationIds[i] = 0;
		}

		if (isAuto)
		{
			for (int i = 0; i < gameFormationIds.Length; ++i)
			{
				if (!IsValidHeroSlot(i) || gameFormationIds[i] != 0)
					continue;
				index = Array.FindIndex(isSelectSlots, (value) => { return !value; });
				if (index < 0)
					break;
				gameFormationIds[i] = sortedHeroDatas[index].idHero;
				isSelectSlots[index] = true;
				AddItem(i, index, gameFormationIds[i]);
			}
		}

		eliteIndices.Sort();
		supportIndices.Sort();

		UpdateForm(true);

		SaveDeck();

		OpenHeroList();
	}

	private void InitPvpForm()
	{
		for (int i = 0; i < m_MaxEliteHeroCount; ++i)
		{
			GameFormEditCharListItem item = GameObjectPool.Pop(m_GameFormEditCharSlotPrefab, false, m_EnemyCharSlotParent, false).GetComponent<GameFormEditCharListItem>();
			item.Init(true, i, null);
			m_PvpEliteSlots.Add(item);
		}

		GiantHeroData[] sortedHeroDatas = m_EpData.pvp.hero.Values.ToArray();
		Array.Sort(sortedHeroDatas, GiantHeroData.SortByProtect);

		if (sortedHeroDatas.Length > m_MaxEliteHeroCount)
		{
			Debug.LogErrorFormat("Pvp.Hero.Count({0}) > {1}", sortedHeroDatas.Length, m_MaxEliteHeroCount);
		}
		
		int count = Mathf.Min(sortedHeroDatas.Length, m_MaxEliteHeroCount);
		for (int i = 0; i < count; ++i)
			m_PvpEliteSlots[i].SetData(sortedHeroDatas[i], false);

		for (int i = count; i < m_MaxEliteHeroCount; ++i)
			m_PvpEliteSlots[i].ResetData();
	}

	private void AddItem(int slotIndex, int sortedHeroIndex, int idHero)
	{
		if (slotIndex < Constant.FormEliteMax)
			eliteIndices.Add(sortedHeroIndex);
		else
			supportIndices.Add(sortedHeroIndex);
	}

	private void OnClickHeroListItem(HeroListItem item)
	{
		if (!gameObject.activeSelf)
			return;

		switch ((EP_CATEGORY)m_EpData.category)
		{
		case EP_CATEGORY.GATHER_HERB:
		case EP_CATEGORY.GATHER_VEIN:
		case EP_CATEGORY.GATHER_TREE:
			ClickHeroListItem_Gather(item);
			break;
		default:
			ClickHeroListItem_Battle(item);
			break;
		}
	}

	private void ClickHeroListItem_Battle(HeroListItem item)
	{
		int index = FindIndex(sortedHeroIDs, item.GetHeroData().idHero);
		if (eliteIndices.IndexOf(index) >= 0)
		{
			SoundManager.instance.PlayEffect(SoundEffect.HeroOutForm);
			eliteIndices.Remove(index);
		}
		else if (supportIndices.IndexOf(index) >= 0)
		{
			SoundManager.instance.PlayEffect(SoundEffect.HeroOutForm);
			supportIndices.Remove(index);
		}
		else
		{
			SoundManager.instance.PlayEffect(SoundEffect.HeroInForm);

			if (eliteIndices.Count < m_MaxEliteHeroCount)
			{
				SoundManager.instance.PlayEffect(SoundEffect.HeroInForm);
				eliteIndices.Add(index);
				eliteIndices.Sort();
			}
			else if (supportIndices.Count < m_MaxSupportHeroCount)
			{
				SoundManager.instance.PlayEffect(SoundEffect.HeroInForm);
				supportIndices.Add(index);
				supportIndices.Sort();
			}
		}

		UpdateForm(false);
	}

	private void ClickHeroListItem_Gather(HeroListItem item)
	{
		SoundManager.instance.PlayEffect(SoundEffect.HeroInForm);

		m_GatherInfoItem.SetHero(item.GetHeroData());

		CheckStartGather();
	}

	public void SelectFormMemberSlot(int slotIndex)
	{
		int idHero;
		if (slotIndex < Constant.FormEliteMax)
		{
			if (slotIndex >= eliteIndices.Count)
				return;
			idHero = sortedHeroIDs[eliteIndices[slotIndex]];
			eliteIndices.RemoveAt(slotIndex);
		}
		else
		{
			slotIndex -= Constant.FormEliteMax;
			if (slotIndex >= supportIndices.Count)
				return;
			idHero = sortedHeroIDs[supportIndices[slotIndex]];
			supportIndices.RemoveAt(slotIndex);
		}

		m_HeroList.ReleaseItem(idHero);

		UpdateForm(false);
	}

	public void OnClickStartBattle()
	{
		switch ((EP_CATEGORY)m_EpData.category)
		{
		case EP_CATEGORY.GATHER_HERB:
		case EP_CATEGORY.GATHER_VEIN:
		case EP_CATEGORY.GATHER_TREE:
			ClickStartGather();
			break;

		default:
			ClickStartBattle();
			break;
		}
	}

	public bool OnNeedCheckItems(out int[] idItems, out int[] count)
	{
		idItems = count = null;

		switch ((EP_CATEGORY)m_EpData.category)
		{
		case EP_CATEGORY.STAGE_MONSTER:
		case EP_CATEGORY.STAGE_BOSS:
		case EP_CATEGORY.PVP_BASE_BATTLE:
		case EP_CATEGORY.PVP_HERO_BATTLE:
		case EP_CATEGORY.PVP_BASE_REVENGE:
		case EP_CATEGORY.PVP_HERO_REVENGE:
		case EP_CATEGORY.RACE_BATTLE:
		case EP_CATEGORY.STAGE_EXTRABOSS:
		case EP_CATEGORY.STAGE_DUNGEON_TARGET:
			idItems = CheckIDItems;
			count = CheckCount;
			return true;

		default:
			return false;
		}
	}
		
	public void OnClickAutoSettingButton()
	{
		isShowAutoSkillEnable = !isShowAutoSkillEnable;
		for (int i = 0; i < m_EliteSlots.Count; ++i)
		{
			m_EliteSlots[i].ShowAutoSkillEnableSettingItem(isShowAutoSkillEnable);
		}
		for (int i = 0; i < m_SupportSlots.Count; ++i)
		{
			m_SupportSlots[i].ShowAutoSkillEnableSettingItem(isShowAutoSkillEnable);
		}
		SetAutoSkillEnableButton(isShowAutoSkillEnable);
	}

	private void SetAutoSkillEnableButton(bool isShowAutoSkillEnable)
	{
		m_AutoSkillEnableOnImage.SetActive(isShowAutoSkillEnable);
		m_AutoSkillEnableOffImage.SetActive(!isShowAutoSkillEnable);
	}

	public void OnClickGameFormSelect()
	{
		Action<int> clickAction = (index) => { SelectGameForm(index); };
		GlobalPopupManager.Inst.ShowPopup(m_GameFormSelectPopupPrefab, m_BattleType, m_GameFormIndex, clickAction);
	}

	private void SelectGameForm(int gameFormIndex)
	{
		bool isChangeIndex = (m_GameFormIndex != gameFormIndex);
		m_GameFormIndex = gameFormIndex;

		bool isGameFormAutoIndex;
		DeviceSaveManager.Inst.LoadGameFormData(m_BattleType.GetGameFormType(), m_GameFormIndex, out m_GameFormData, out isGameFormAutoIndex);

		if (isChangeIndex)
		{
			m_GameFormNumberText.text = (m_GameFormIndex + 1).ToString();
			m_AutoExplorationObject.SetActive(isGameFormAutoIndex);
		}

		if (m_GameFormData == null)
			m_GameFormData = m_BattleType.GetNewGameFormData();
		Array.Copy(m_GameFormData.idHeroes, gameFormationIds, m_GameFormData.idHeroes.Length);
		Array.Copy(m_GameFormData.isAutoSkillEnables, m_IsAutoSkillEnables, m_GameFormData.isAutoSkillEnables.Length);

		eliteIndices.Clear();
		supportIndices.Clear();

		int index, idHero;
		for (int i = 0; i < Constant.FormMemberMax; ++i)
		{
			if (!IsValidHeroSlot(i))
				continue;
			idHero = gameFormationIds[i];
			if (idHero != 0 && FindIndex(gameFormationIds, idHero) == i && (index = FindIndex(sortedHeroIDs, idHero)) >= 0)
				AddItem(i, index, idHero);
			else
				gameFormationIds[i] = 0;
		}

		eliteIndices.Sort();
		supportIndices.Sort();

		UpdateForm(true);

		m_HeroList.Close();

		OpenHeroList();
	}

	private void ClosePanel()
	{
		PanelManager.inst.PopPanel();
	}

	private void ClickStartBattle()
	{
		if (!isValidForm)
		{
			GlobalSystemRewardMsgHandler.instance.ShowMessage(UITextEnum.COMMON_SELECT_HERO);
			return;
		}

		if (!isUsableHero)
		{
			GlobalSystemRewardMsgHandler.instance.ShowMessage(UITextEnum.COMMON_INCLUDE_DISABLE_HERO);
			return;
		}

#if UNITY_EDITOR || PREGAME_TEST
		string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
		if (sceneName.StartsWith("Game"))
		{
			Save();

			HidePanel();

			if (onClickStartAction != null)
				onClickStartAction();
			return;
		}
#endif
		StopCoroutine("UpdateFormGrayScale");

		if (isBattleCost)
		{
			if (m_EpData.category == (int)EP_CATEGORY.STAGE_WORLDBOSS)
			{
				if (!m_StartInstantCost.IsEnoughCost)
				{
					m_StartInstantCost.DefaultNotEnoughAction();
					return;
				}
			}
			else if (!m_RetryCost.IsEnoughCost)
			{
				m_RetryCost.DefaultNotEnoughAction();
				return;
			}
		}

		CheckGameFormAndBattle();
	}

	private void ClickStartGather()
	{
		if (m_GatherInfoItem.data == null)
		{
			GlobalSystemRewardMsgHandler.instance.ShowMessage(UITextEnum.COMMON_SELECT_HERO);
			return;
		}
		
		GiantHandler.Inst.doSuccGatherEp(m_EpData.idSearch, m_EpData.epNum, m_GatherInfoItem.data.idHero).SetSuccessAction(() =>
		{
			ClosePanel();
			UITutorialEventManager.instance.CheckTrigger(EVENT_TRIGGER.GATHERING);
		});
	}

	private bool IsChangeEliteIndices()
	{
		if (!tempEliteIndices.SequenceEqual(eliteIndices))
		{
			tempEliteIndices.Clear();
			tempEliteIndices.AddRange(eliteIndices);
			return true;
		}
		return false;
	}

	private void UpdateForm(bool isInit)
	{
		if (isInit)
		{
			tempEliteIndices = new List<int>();
			tempEliteIndices.AddRange(eliteIndices);
		}
		else if (IsChangeEliteIndices())
		{
			for (int i = 0; i < m_IsAutoSkillEnables.Length; ++i)
			{
				m_IsAutoSkillEnables[i] = true;
			}
		}

		int minRank = m_EpData.isGuildBattle ? Datatable.Inst.settingData.GBMinHeroRank : 0;
		double totalCombatPoint = 0;
		int index;
		GiantHeroData heroData;
		for (int i = 0; i < eliteIndices.Count; ++i)
		{
			index = eliteIndices[i];
			heroData = sortedHeroDatas[index];
			m_EliteSlots[i].SetData(heroData).SetGrayScale(IsNotUsableHero(heroData));
			m_EliteSlots[i].CheckUsableData(heroData, IsPenaltyHero(heroData), m_EpData.worldBossData, minRank);
			m_EliteSlots[i].SetAutoSkillEnableSetting(m_IsAutoSkillEnables[i], true);
			totalCombatPoint += GetMyHeroCombatPoint(index);
		}

		for (int i = eliteIndices.Count; i < m_MaxEliteHeroCount; ++i)
		{
			m_EliteSlots[i].ResetData();
		}
		
		if (m_MaxSupportHeroCount > 0)
		{
			for (int i = 0; i < supportIndices.Count; ++i)
			{
				index = supportIndices[i];
				heroData = sortedHeroDatas[index];
				m_SupportSlots[i].SetData(heroData).SetGrayScale(IsNotUsableHero(heroData));
				m_SupportSlots[i].CheckUsableData(heroData, IsPenaltyHero(heroData), m_EpData.worldBossData, minRank);
				m_SupportSlots[i].SetAutoSkillEnableSetting(false, false);
				totalCombatPoint += GetMyHeroCombatPoint(index);
			}
			for (int i = supportIndices.Count; i < m_MaxSupportHeroCount; ++i)
				m_SupportSlots[i].ResetData();
		}

		CheckStartBattle();

		m_HeroList.SetSelectType((m_MaxSupportHeroCount <= 0 || eliteIndices.Count < m_MaxEliteHeroCount) ? HeroSelectType.Elite : HeroSelectType.Supporter);

		int heroCount = eliteIndices.Count + supportIndices.Count;
		m_MyCombatPointTexts[curMyCombatPointIndex].text = Datatable.Inst.GetCombatPointString(heroCount <= 0 ? 0 : (long)(totalCombatPoint / heroCount), true);
	}

	private void OpenHeroList()
	{
		SelectedHeroData [] selectedHeroDatas = new SelectedHeroData[eliteIndices.Count + supportIndices.Count];
		SelectedHeroData data;
		for (int i = 0; i < eliteIndices.Count; ++i)
		{
			data = new SelectedHeroData();
			data.idHero = sortedHeroIDs[eliteIndices[i]];
			data.selectType = HeroSelectType.Elite;
			selectedHeroDatas[i] = data;
		}
		for (int i = 0; i < supportIndices.Count; ++i)
		{
			data = new SelectedHeroData();
			data.idHero = sortedHeroIDs[supportIndices[i]];
			data.selectType = HeroSelectType.Supporter;
			selectedHeroDatas[i + eliteIndices.Count] = data;
		}

		int maxHeroCount = m_MaxEliteHeroCount + m_MaxSupportHeroCount;
		switch ((EP_CATEGORY)m_EpData.category)
		{
		case EP_CATEGORY.RACE_BATTLE:
			m_HeroList.OpenHeroList(HeroSelectMode.Race, m_EpData.myRace, maxHeroCount, selectedHeroDatas, false);
			break;
		case EP_CATEGORY.STAGE_WORLDBOSS:
			m_HeroList.OpenHeroList(HeroSelectMode.WorldBoss, 0, maxHeroCount, selectedHeroDatas, false, true, m_EpData.worldBossData);
			break;
		case EP_CATEGORY.GUILD_BATTLE:
			m_HeroList.OpenHeroList(HeroSelectMode.GuildBattle, 0, maxHeroCount, selectedHeroDatas, false);
			break;
		default:
			m_HeroList.OpenHeroList(HeroSelectMode.Battle, 0, maxHeroCount, selectedHeroDatas, false);
			break;
		}
	}

	private IEnumerator UpdateFormGrayScale()
	{
		if (isUsableHero)
			yield break;
		
		GiantHeroData heroData;

		while (true)
		{
			for (int i = 0; i < eliteIndices.Count; ++i)
			{
				heroData = sortedHeroDatas[eliteIndices[i]];
				m_EliteSlots[i].SetGrayScale(IsNotUsableHero(heroData));
			}
			if (m_MaxSupportHeroCount > 0)
			{
				for (int i = 0; i < supportIndices.Count; ++i)
				{
					heroData = sortedHeroDatas[supportIndices[i]];
					m_SupportSlots[i].SetGrayScale(IsNotUsableHero(heroData));
				}
			}

			CheckStartBattle();

			yield return null;
		}
	}

	private double GetMyHeroCombatPoint(int sortedHeroIndex)
	{
		if (myCombatPoints.ContainsKey(sortedHeroIndex))
			return myCombatPoints[sortedHeroIndex];

		float statMultiplier = m_EpData.category == (int)EP_CATEGORY.RACE_BATTLE && m_EpData.myRace != sortedHeroDatas[sortedHeroIndex].race ? Constant.RaceBattlePenalty : 1f;
		List<Datatable.WorldBossOption> worldBossOptions = m_EpData.category == (int)EP_CATEGORY.STAGE_WORLDBOSS ? m_EpData.worldBossOptions : null;
		double combatPoint = Giant.Util.GetMyHeroCombatPoint(sortedHeroDatas[sortedHeroIndex], statMultiplier, worldBossOptions);
		myCombatPoints.Add(sortedHeroIndex, combatPoint);
		return combatPoint;
	}

	private bool IsValidHeroSlot(int index)
	{
		if (index < 0 || index >= Constant.FormMemberMax)
			return false;
		if (index < Constant.FormEliteMax)
			return index < m_MaxEliteHeroCount;
		return (index - Constant.FormEliteMax) < m_MaxSupportHeroCount;
	}

	private void CheckStartGather()
	{
		isValidForm = m_GatherInfoItem.data != null;
		isUsableHero = true;
		m_StartBattleBtn.image.SetGrayScale(!isValidForm);
	}

	private void CheckStartBattle()
	{
		isValidForm = IsValidGameFormation();
		isUsableHero = IsUsableHeroes();
		m_RetryCost.SetEnableAndEnough(isValidForm && isUsableHero, m_RetryCost.IsEnoughCost);
	}

	private void CheckGameFormAndBattle()
	{
		int total = eliteIndices.Count + supportIndices.Count;
		if (total < m_MaxEliteHeroCount + m_MaxSupportHeroCount && total < AccountDataStore.instance.GetEnableBattleHeroCount(m_EpData.worldBossData, m_EpData.isGuildBattle))
			SystemPopupManager.Inst.ShowTwoButtonPopup(UITextEnum.GAMEFORMEDIT_INCOMPLETE_ALERT).SetClickOKButtonAction(SaveAndCheckBattle);
		else
			SaveAndCheckBattle();
	}

	private void SaveAndCheckBattle()
	{
		Save();
		CheckCostBattle();
	}

	private void CheckCostBattle()
	{
		if (isBattleCost)
		{
			if (m_EpData.category == (int)EP_CATEGORY.STAGE_WORLDBOSS)
			{
				int idItem = SpecialItemID.Cash;
				int cost = Datatable.Inst.settingData.WorldBossTokenCash;
				SystemPopupManager.Inst.ShowTwoButtonCostPopup(UITextEnum.GAMEFORMEDIT_WORLDBOSS_USE_CASH, idItem, cost).SetClickOKButtonAction(StartBattle);
			}
			else
				SystemPopupManager.Inst.ShowTwoButtonPopup(Datatable.Inst.GetUIText(UITextEnum.BATTLE_RESULT_RETRY_ALERT, battleCostValue, Datatable.Inst.dtItemBase[battleCostItemID].Name)).SetClickOKButtonAction(StartBattle);
		}
		else
			StartBattle();
	}

	private bool CheckPvpBattle()
	{
		bool isOnPvpCheckingTime = AccountDataStore.instance.pvpSeasonInfo.IsOnPvpCheckingTime();
		if (isOnPvpCheckingTime)
			SystemPopupManager.Inst.ShowOneButtonPopup(Datatable.Inst.GetServerErrorText(ServerError.PVP_HERO_SUMMARY)).SetClickButtonAction(ClosePanel);
		return !isOnPvpCheckingTime;
	}
	
	private bool CheckBaseRaidBattle()
	{
		// 베이스 레이드에 참가할 수 있는 유효 시간이 지난 경우
		if (!m_EpData.baseRaid.isAbleJoin)
		{
			GiantHandler.Inst.doFailEp(m_EpData.idSearch, m_EpData.epNum).SetSuccessAction(() =>
			{
				SystemPopupManager.Inst.ShowOneButtonPopup(UITextEnum.BASE_RAID_TIME_OVER).SetClickButtonAction(ClosePanel);
			});
			return false;
		}

		// 자이언트가 존재하지 않는 경우
		if (m_EpData.baseRaid.mainGiant == null)
		{
			GiantHandler.Inst.doResultBaseRaid(m_EpData.idSearch, m_EpData.epNum, true).SetSuccessAction(() =>
			{
				if (AutoExplorationManager.Inst.isPlaying)
					AutoExplorationManager.Inst.AddBattleCount(true);
				
				string uiText = string.Empty;
				int count = 0;

				GiantRewardData [] rewards = AccountDataStore.instance.GetRewardDataList();
				if (rewards.Length > 0)
				{
					uiText = rewards[0].name;
					count = rewards[0].value;
				}
				else
				{
					uiText = Datatable.Inst.GetUIText(UITextEnum.RESOURCE_START + m_EpData.baseRaid.mainPlant.idPlant);
					count = 0;
				}
				AccountDataStore.instance.UpdateDailyQuestAction(DAILY_QUEST_OBJECTIVE.COUNT_USER_BATTLE, 1, m_EpData.category)
					.UpdateSubQuestAction(QUEST_OBJECTIVE.PVP_BASE_BATTLE, 0, 1).RunQuestClientDataUpdate(()=>{
								SystemPopupManager.Inst.ShowOneButtonPopup(Datatable.Inst.GetUIText(UITextEnum.BASE_RAID_WIN_NO_GIANT, uiText, count)).SetClickButtonAction(ClosePanel);
							});
			});
			return false;
		}

		return true;
	}

	private bool CheckRevengeRaidBattle()
	{
		// 베이스 레이드에 참가할 수 있는 유효 시간이 지난 경우
		if (m_EpData.baseRaid.raidStartTime != 0 && !m_EpData.baseRaid.isAbleJoin)
		{
			SystemPopupManager.Inst.ShowOneButtonPopup(UITextEnum.BASE_RAID_TIME_OVER).SetClickButtonAction(ClosePanel);
			return false;
		}

		// 자이언트가 존재하지 않는 경우
		if (m_EpData.baseRaid.mainGiant == null)
		{
			GiantHandler.Inst.doStartRevengeRaid(m_EpData.baseRaid.uidBaseRaid).SetSuccessAction(() => {
				GiantHandler.Inst.doResultRevengeRaid(true).SetSuccessAction(() =>
				{
					string uiText = string.Empty;
					int count = 0;

					GiantRewardData [] rewards = AccountDataStore.instance.GetRewardDataList();
					if (rewards.Length > 0)
					{
						uiText = rewards[0].name;
						count = rewards[0].value;
					}
					else
					{
						uiText = Datatable.Inst.GetUIText(UITextEnum.RESOURCE_START + m_EpData.baseRaid.mainPlant.idPlant);
						count = 0;
					}
					AccountDataStore.instance.UpdateDailyQuestAction(DAILY_QUEST_OBJECTIVE.COUNT_USER_BATTLE, 1, m_EpData.category)
						.UpdateSubQuestAction(QUEST_OBJECTIVE.PVP_BASE_BATTLE, 0, 1).RunQuestClientDataUpdate(()=>{
								SystemPopupManager.Inst.ShowOneButtonPopup(Datatable.Inst.GetUIText(UITextEnum.BASE_RAID_WIN_NO_GIANT, uiText, count)).SetClickButtonAction(ClosePanel);
							});
				});
			});
			return false;
		}

		return true;
	}

	private bool CheckWorldBossBattle()
	{
		if (AccountDataStore.instance.worldBossSeasonInfo.isOnSeason)
			return true;
		SystemPopupManager.Inst.ShowOneButtonPopup(Datatable.Inst.GetServerErrorText(ServerError.NOT_WORLD_BOSS_SEASON));
		return false;
	}

	private bool CheckGuildBattle()
	{
		if (Datatable.Inst.GetCurrentGBSeason() < 0)
		{
			SystemPopupManager.Inst.ShowOneButtonPopup(Datatable.Inst.GetServerErrorText(ServerError.NOT_GUILD_BATTLE_PERIOD));
			return false;
		}
		return true;
	}

	private void StartBattle()
	{
		switch ((EP_CATEGORY)m_EpData.category)
		{
		case EP_CATEGORY.STAGE_MONSTER:
		case EP_CATEGORY.STAGE_BOSS:
		case EP_CATEGORY.STAGE_EXTRABOSS:
		case EP_CATEGORY.STAGE_DUNGEON_TARGET:
			GiantHandler.Inst.doStartBattle(m_EpData.idSearch, m_EpData.epNum).SetSuccessAction(MoveNext);
			break;
		case EP_CATEGORY.PVP_BASE_BATTLE:
			if (CheckBaseRaidBattle())
				MoveNext();
			break;
		case EP_CATEGORY.PVP_HERO_BATTLE:
			if (CheckPvpBattle())
				GiantHandler.Inst.doStartPvpHero(m_EpData.idSearch, m_EpData.epNum).SetSuccessAction(MoveNext);
			break;
		case EP_CATEGORY.PVP_BASE_REVENGE:
			if (CheckRevengeRaidBattle())
				GiantHandler.Inst.doStartRevengeRaid(m_EpData.baseRaid.uidBaseRaid).SetSuccessAction(MoveNext);
			break;
		case EP_CATEGORY.PVP_HERO_REVENGE:
			if (CheckPvpBattle())
				GiantHandler.Inst.doStartRevengePvpHero(m_EpData.pvp.uidPvpHeroLog).SetSuccessAction(MoveNext);
			break;
		case EP_CATEGORY.RACE_BATTLE:
			MoveNext();
			break;
		case EP_CATEGORY.STAGE_WORLDBOSS:
			if (CheckWorldBossBattle())
				GiantHandler.Inst.doStartWorldBoss(m_EpData.idSearch, m_EpData.epNum).SetSuccessAction(MoveNext);
			break;
		case EP_CATEGORY.GUILD_BATTLE:
			if (CheckGuildBattle())
				GiantHandler.Inst.doStartGuildBattle(m_EpData.gid, m_EpData.value, Giant.Util.GetDeckList(m_GameFormData.idHeroes)).SetSuccessAction(MoveNext);
			break;
		default:
			MoveNext();
			break;
		}
	}

	private void MoveNext()
	{
		SoundManager.instance.StopBGM();
		BaseOperatorUnit.instance.LoadLevel_Async(PlaySceneType.PreGame, true);
	}

	private void Save()
	{
		SaveInStageData();
		SaveDeck();
	}

	private void SaveInStageData()
	{
		InStageInfoData stageInfo	= new InStageInfoData();
		switch ((EP_CATEGORY)m_EpData.category)
		{
		case EP_CATEGORY.PVP_BASE_REVENGE:
			stageInfo.Type			= (int)InStageInfoType.BaseRevenge;
			stageInfo.UID			= m_EpData.baseRaid.uidBaseRaid;
			break;
		case EP_CATEGORY.PVP_HERO_REVENGE:
			stageInfo.Type			= (int)InStageInfoType.HeroRevenge;
			stageInfo.UID			= m_EpData.pvp.uidPvpHeroLog;
			break;
		case EP_CATEGORY.GUILD_BATTLE:
			stageInfo.Type			= (int)InStageInfoType.GuildBattle;
			break;
		default:
			stageInfo.Type			= (int)InStageInfoType.Default;
			stageInfo.SearchID		= m_EpData.idSearch;
			stageInfo.epNum			= m_EpData.epNum;
			break;
		}

		string json = Util.JsonEncode2(stageInfo);
		DeviceSaveManager.Inst.SaveInStageData(json);
	}

	private void SaveDeck()
	{
		for (int i = 0; i < eliteIndices.Count; ++i)
			m_GameFormData.idHeroes[i] = sortedHeroIDs[eliteIndices[i]];
		for (int i = eliteIndices.Count; i < Constant.FormEliteMax; ++i)
			m_GameFormData.idHeroes[i] = 0;
		if (m_MaxSupportHeroCount > 0)
		{
			for (int i = 0; i < supportIndices.Count; ++i)
				m_GameFormData.idHeroes[Constant.FormEliteMax + i] = sortedHeroIDs[supportIndices[i]];
			for (int i = supportIndices.Count; i < Constant.FormSupportMax; ++i)
				m_GameFormData.idHeroes[Constant.FormEliteMax + i] = 0;
		}

		bool isRaceBattle = (EP_CATEGORY)m_EpData.category == EP_CATEGORY.RACE_BATTLE;
		int myRace = (isRaceBattle) ? m_EpData.myRace : 0;
		for (int i = 0; i < Constant.FormEliteMax; ++i)
			m_GameFormData.isAutoSkillEnables[i] = m_EliteSlots[i].isAutoSkillEnable;
		if (!isRaceBattle && AutoExplorationManager.Inst.isPlaying)
			DeviceSaveManager.Inst.SaveGameFormData(m_BattleType.GetGameFormType(), m_GameFormData, m_GameFormIndex, false);
		else
			DeviceSaveManager.Inst.SaveGameFormData(m_BattleType, myRace, m_GameFormData, m_GameFormIndex);
	}

	private bool IsValidGameFormation()
	{
		int count = eliteIndices.Count + supportIndices.Count;
		return (count > 0) && (count <= m_MaxEliteHeroCount + m_MaxSupportHeroCount);
	}

	private bool IsUsableHeroes()
	{
		for (int i = 0; i < eliteIndices.Count; ++i) 
		{
			if (IsNotUsableHero(sortedHeroDatas[eliteIndices[i]]))
				return false;
		}

		for (int i = 0; i < supportIndices.Count; ++i)
		{
			if (IsNotUsableHero(sortedHeroDatas[supportIndices[i]]))
				return false;
		}

		return true;
	}
	
	private bool IsNotUsableHero(GiantHeroData heroData)
	{
		if (heroData.isGatherDispatch)
			return true;
		if (m_EpData.isWorldBoss && m_EpData.worldBossData.IsHeroBan(heroData))
			return true;
		if (m_EpData.isGuildBattle)
		{
			if (heroData.isBroken || !Datatable.Inst.IsFitGuildBattleRank(heroData.rank))
				return true;
		}
		return false;
	}
	
	private bool IsPenaltyHero(GiantHeroData heroData)
	{
		return (m_EpData.category == (int)EP_CATEGORY.RACE_BATTLE && m_EpData.myRace != heroData.race);
	}

	private int FindIndex(int [] array, int value)
	{
		for (int i = 0; i < array.Length; ++i)
		{
			if (array[i] == value)
				return i;
		}
		return -1;
	}
}

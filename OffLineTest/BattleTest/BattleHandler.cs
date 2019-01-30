using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using AL;
using System.IO;

public class BattleHandler : MonoBehaviour {

	private static BattleHandler _inst = null;
	public static BattleHandler Inst { get { return _inst; } }

	[SerializeField] private BattleUIHandler uiHandler;
	[SerializeField] private UnityEngine.Object baseOperatorUnitPrefab;
	[SerializeField] private int idStage;
	[SerializeField] private int difficulty = (int)StageDifficulty.Normal;

	[HideInInspector] private int mapNumber;

	[HideInInspector] public BattleGameHandler gameHandler { get; private set; }
	[HideInInspector] public CameraController cameraController { get; private set; }

	private GiantEPData epData = null;
	public Action evtStartLogin;
	private bool isPreGameTest = false;

	private Datatable.StageData stageData = null;
	private StageHelperData stageHelperData;
	private Datatable.BattleSceneData battleSceneData = null;

	private bool isBossBattle { get; set; }
	public bool isArenaRealBattle { get; private set; }
	public AutoBattleType autoBattleType { get; set; }
	public int battleSpeedIndex { get; set; }

	private EP_CATEGORY epCategory { get { return (EP_CATEGORY)epData.category; } }

	private void Start()
	{
		cameraController = GetComponent<CameraController>();

		if (BaseOperatorUnit.instance == null)
		{
			isPreGameTest = true;
			Instantiate(baseOperatorUnitPrefab);
			StartCoroutine(InitGameManager());
		}
		else
		{
			isPreGameTest = false;
			onGameLoginCompleted();
		}
	}

	private IEnumerator InitGameManager()
	{
		while (GameManager.Inst == null)
			yield return null;

		GameManager.Inst.evtAssetInitCompleted += onGamePrepareCompleted;
		GameManager.Inst.evtGameLoginCompleted += onGameLoginCompleted;
		evtStartLogin += GameManager.Inst.OnStartLoginFromPreGame;
	}

	private void OnDestroy()
	{
		if (GameManager.Inst != null)
		{
			GameManager.Inst.evtAssetInitCompleted -= onGamePrepareCompleted;
			GameManager.Inst.evtGameLoginCompleted -= onGameLoginCompleted;
			evtStartLogin -= GameManager.Inst.OnStartLoginFromPreGame;
		}
	}

	private void onGameLoginCompleted()
	{
		Init();
	}

	private void Init()
	{
		// Load Level
		CheckEP(LoadLevelAsync);
	}

	private void LoadLevelAsync()
	{
		string sceneName = Util.StringFormat(StringFormat.GAME_SCENENAME, mapNumber.ToString(ValueFormat.SCENE_NUMBER));
//		gameState = GameState.GameLoad;
#if MAPLOADING_TEST
		loadingStartTime = Time.realtimeSinceStartup;
#endif
#if ORIGINALMAP_TEST
		BaseOperatorUnit.instance.LoadLevel_Async(sceneName, true, onSceneLoaded);
#else
		sceneName = "Game0101";
		BaseOperatorUnit.instance.LoadLevelFromAssetBundle_Async(sceneName, sceneName, true, onSceneLoaded, onSceneLoadFailed);
#endif
	}

	private void onSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
//		if (gameState != GameState.GameLoad)
//			return;

#if MAPLOADING_TEST
		float end = Time.realtimeSinceStartup;
		float delta = end - loadingStartTime;
		Debug.LogWarningFormat("Game{0} : {1}", mapNumber, delta);
#endif
		gameHandler = FindObjectOfType<BattleGameHandler>();
		if (gameHandler != null)
			gameHandler.SetHandler(this, uiHandler);

		LoadStage();

#if UNITY_EDITOR || PREGAME_TEST
		if (isPreGameTest)
			EditGameForm();
//		else
//			LoadStageAfter();
#else
//		LoadStageAfter();
#endif
	}

	private void LoadStage()
	{
		StageHelperHandler stageHelperHandler = gameHandler.stageHelperHandler;
		if (stageHelperHandler == null)
		{
			Debug.LogError("StageHelperHandler is not found.");
			return;
		}

		switch (epCategory)
		{
		case EP_CATEGORY.PVP_BASE_BATTLE:
		case EP_CATEGORY.PVP_HERO_BATTLE:
		case EP_CATEGORY.PVP_BASE_REVENGE:
		case EP_CATEGORY.PVP_HERO_REVENGE:
		case EP_CATEGORY.RACE_BATTLE:
		case EP_CATEGORY.PVP_ARENA_BATTLE:
		case EP_CATEGORY.GUILD_BATTLE:
		case EP_CATEGORY.PVP_DUEL_BATTLE:
			stageHelperData = stageHelperHandler.GetData(battleSceneData.StageHelperIndex);
			break;
		default:
			stageHelperData = stageHelperHandler.GetData(stageData.StageHelperIndex);
			break;
		}

		if (stageHelperData.enemiesPos == null || stageHelperData.enemiesPos.Count <= 0)
		{
			Debug.LogWarning("StageHelperIndex is not found."); 
			stageHelperData = stageHelperHandler.GetFirstData();
		}

		SetupCameraController();
	}

	private void onSceneLoadFailed()
	{
		SuperOverlayPopupManager.Inst.ShowOneButtonPopup(UITextEnum.BATTLE_INVALID);
	}

	public void EditGameForm()
	{
		BaseOperatorUnit.instance.FinishLoadProgress();

//		gameState = GameState.GameFormEdit;

		cameraController.ResetCameraData();
		cameraController.SetEnableControl(false);

		uiHandler.ShowGameFormEdit(epData);
	}

	private bool CheckStageData(int idStage)
	{
		if (!Datatable.Inst.dtStageData.ContainsKey(idStage) || !Datatable.Inst.dtStageEnemies.ContainsKey(idStage))
		{
			Debug.LogError(Logger.Write(Util.StringFormat("Check your StageData or StageEnemies! ID : {0}", idStage)));
			return false;
		}
		// StageData 설정
		stageData = Datatable.Inst.dtStageData[idStage];
		mapNumber = stageData.Scene;
//		fatalGaugeRate = stageData.FatalGaugeRate;
		return true;
	}

	private bool CheckBattleSceneData(int epCategory, int idBuilding)
	{
		// BattleSceneData 설정
		battleSceneData = Datatable.Inst.GetBattleSceneData(epCategory, idBuilding);
		if (battleSceneData == null)
			return false;
		mapNumber = battleSceneData.Scene;
		return true;
	}

	private void CheckEPData(GiantEPData epData)
	{
		if (epData == null)
			return;
		
		switch ((EP_CATEGORY)epData.category)
		{
		case EP_CATEGORY.STAGE_MONSTER:
		case EP_CATEGORY.STAGE_BOSS:
		case EP_CATEGORY.STAGE_EXTRABOSS:
		case EP_CATEGORY.STAGE_DUNGEON_TARGET:
			if (!CheckStageData(epData.value))
				return;
			break;
		case EP_CATEGORY.PVP_BASE_BATTLE:
		case EP_CATEGORY.PVP_BASE_REVENGE:
			if (!CheckBattleSceneData(epData.category, epData.baseRaid.mainPlant.idPlant))
				return;
			break;
		case EP_CATEGORY.PVP_HERO_BATTLE:
		case EP_CATEGORY.PVP_HERO_REVENGE:
		case EP_CATEGORY.PVP_ARENA_BATTLE:
		case EP_CATEGORY.PVP_DUEL_BATTLE:
			if (epData.pvp == null || epData.pvp.hero.Count <= 0)
				return;
			if (!CheckBattleSceneData(epData.category, 0))
				return;
			break;
		case EP_CATEGORY.RACE_BATTLE:
			if (!CheckBattleSceneData(epData.category, 0))
				return;
			break;
//		case EP_CATEGORY.STAGE_WORLDBOSS:
//			worldBossData = epData.worldBossData;
//			if (worldBossData == null || !CheckStageData(worldBossData.idStage))
//				return;
//			if (Datatable.Inst.dtWorldBossRage.ContainsKey(idNextWorldBossRage + 1))
//				++idNextWorldBossRage;
//			worldBossBestDamage = AccountDataStore.instance.user.worldBossMaxDamage;
//			break;
		case EP_CATEGORY.GUILD_BATTLE:
			if (!CheckBattleSceneData(epData.category, 0))
				return;
			break;
		default:
			return;
		}

		CONTENT_TYPE contentType = CONTENT_TYPE.NONE;

		// 보스전 및 실시간 전투 여부는 미리 계산해둔다.
		switch ((EP_CATEGORY)epData.category)
		{
		case EP_CATEGORY.STAGE_BOSS:
		case EP_CATEGORY.STAGE_EXTRABOSS:
			isBossBattle = true;
			isArenaRealBattle = false;
			break;
		case EP_CATEGORY.PVP_BASE_BATTLE:
		case EP_CATEGORY.PVP_BASE_REVENGE:
			contentType = CONTENT_TYPE.RAID;
			isBossBattle = true;
			isArenaRealBattle = false;
			break;
		case EP_CATEGORY.PVP_HERO_BATTLE:
		case EP_CATEGORY.PVP_HERO_REVENGE:
			contentType = CONTENT_TYPE.PVP;
			isBossBattle = false;
			isArenaRealBattle = false;
			break;
		case EP_CATEGORY.STAGE_DUNGEON_TARGET:
			isBossBattle = Datatable.Inst.GetIsBossStage(epData.value);
			isArenaRealBattle = false;
			break;
		case EP_CATEGORY.PVP_ARENA_BATTLE:
			contentType = CONTENT_TYPE.ARENA;
			isBossBattle = false;
			isArenaRealBattle = !AccountDataStore.instance.arena.isBot;
//			CheckChangeStageHelperPos();
			break;
		case EP_CATEGORY.STAGE_WORLDBOSS:
			isBossBattle = true;
			isArenaRealBattle = false;
			break;
		case EP_CATEGORY.GUILD_BATTLE:
			isBossBattle = true;
			isArenaRealBattle = false;
			break;
		default:
			isBossBattle = false;
			isArenaRealBattle = false;
			break;
		}

		AccountDataStore.instance.currentContentType = contentType;

		this.epData = epData;
	}

	private void CheckEP(Action cb)
	{
		Action cb2 = () => {
#if UNITY_EDITOR || MAPLOADING_TEST
			string filePath = PersistentStore.GetPath("gamescene.json");
			if (File.Exists(filePath))
			{
				byte[] b = Util.ReadFile(filePath);
				string json = Util.Utf8Decode(b);
				if (!string.IsNullOrEmpty(json))
					mapNumber = int.Parse(json);
			}
#endif
			cb();
		};

		epData = null;

#if UNITY_EDITOR || PREGAME_TEST
		if (isPreGameTest)
		{
			if (Datatable.Inst.dtStageData.ContainsKey(idStage) && Datatable.Inst.dtStageEnemies.ContainsKey(idStage))
			{
				stageData = Datatable.Inst.dtStageData[idStage];

				epData = new GiantEPData();
				epData.idSearch = stageData.ValidArea * 10 + difficulty;
				epData.category = stageData.Type;
				if (stageData.Type == (int)EP_CATEGORY.STAGE_WORLDBOSS)
					epData.value = AccountDataStore.instance.worldBossSeasonInfo.idSeason;
				else
					epData.value = idStage;
				epData.epNum = -1;

				CheckEPData(epData);
			}
		}
//		else
//			CheckStageInfoData();
#else
		CheckStageInfoData();
#endif

		if (epData != null)
		{
//			battleSpeedIndex = DeviceSaveManager.Inst.LoadGameBattleSpeed();
//
//			// 듀얼에서는 완전 자동이다.
//			if (isDuelBattle)
//				autoBattleType = AutoBattleType.All;
//			else
//				autoBattleType = DeviceSaveManager.Inst.LoadGameAutoBattleType();
//
//			// 실시간 전투에서는 전투 배속을 기본값으로 설정한다.
//			if (isArenaBattle || !Datatable.Inst.IsUnlockContents(UNLOCK_CONTENTS.MAXIMUM_BATTLE_SPEED) && battleSpeedIndex >= Datatable.Inst.dtBattleSpeed.Count - 1)
//				battleSpeedIndex = 0;

			autoBattleType = AutoBattleType.All;
			battleSpeedIndex = 0;
			cb2();
		}
		else
		{
//			InitFailed();
		}
	}

	private void SetupCameraController()
	{
		// 카메라 설정
		if (Camera.main == null)
			return;

		Camera camera = Camera.main;
		cameraController.SetCamera(camera);

//		uiHandler.SetupWorldCamera(camera);
//		AdjustStageCamera(camera);
	}

	private void onGamePrepareCompleted()
	{
		if(evtStartLogin != null)
			evtStartLogin();
	}
}

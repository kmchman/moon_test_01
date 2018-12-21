using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
[System.Serializable]
#endif
//public class AggroData
//{
//	public CharacterBase character;
//	public long aggroValue;
//}
//
//public class AggroActionData
//{
//	public CharacterBase character;
//	public int skillLv;
//	public DAMAGE_TYPE attackType;
//	public double attackPower;		// Function 혹은 타겟마다 계산
//	public float aggroRate;
//	public int aggroAdd;
//
//	public int skillIndex;
//	public int skillHitCount;
//
//	public bool isChain;
//	public List<CharacterBase> targets;
//
//	public void CopyFrom(AggroActionData other)
//	{
//		character		= other.character;
//		skillLv			= other.skillLv;
//		attackType		= other.attackType;
//		attackPower		= other.attackPower;
//		aggroRate		= other.aggroRate;
//		aggroAdd		= other.aggroAdd;
//
//		skillIndex		= other.skillIndex;
//		skillHitCount	= other.skillHitCount;
//
//		// 복사 안 되는 항목
//		isChain			= false;
//		if (targets != null)
//			targets.Clear();
//	}
//}
	
public class AnimationClipPairs : List<KeyValuePair<AnimationClip, AnimationClip>>
{
	public AnimationClipPairs(int capacity) : base(capacity) {}

	public int FindIndex(string name)
	{
		return FindIndex((clipPair) => { return clipPair.Key.name.Equals(name); });
	}

	public bool IsOriginalClip(int index)
	{
		return base[index].Key.Equals(base[index].Value);
	}

	public AnimationClip this[string name]
	{
		get { return Find(clipPair => clipPair.Key.name.Equals(name)).Value; }
		set
		{
			int index = FindIndex(name);
			if (index >= 0)
				base[index] = new KeyValuePair<AnimationClip, AnimationClip>(base[index].Key, value);
		}
	}

	public new AnimationClip this[int index]
	{
		get { return base[index].Value; }
		set { base[index] = new KeyValuePair<AnimationClip, AnimationClip>(base[index].Key, value); }
	}
}

public class ConditionSkillData
{
	public int skillIndex;
	public float lastTime;
	public int count;
}

//public class BasicBuffData
//{
//	public int idBuff;
//	
//	public bool isShow;
//	public bool isEnd;
//	public SkillEffect effect1;
//	public SkillEffect effect2;
//	
//	public bool isPossibleDispel { get { return Datatable.Inst.dtBuff[idBuff].IsDispelled; } }
//}
//
//public class BuffStackData : BasicBuffData
//{
//	public int startFrame;
//	public bool isAura;
//	public bool isBuff;
//	public float endTime;
//
//	public float endEffectTime;
//
//	public List<BuffData> buffDatas = new List<BuffData>();
//	public List<BuffData> removeBuffDatas = new List<BuffData>();
//}

//public class BuffData
//{
//	public int uid;
//
//	public CharacterBase from;
//	public double atk;
//
//	public List<BuffElementData> buffElementDatas = new List<BuffElementData>();
//}
//
//public class BuffElementData
//{
//	public int uid;					// BUFF_TIME.CONT 만 유효
//	public int idBuff;
//	public int idBuffElement;
//	public int skillIndex;
//	public bool isImmune;
//	public double param;			// 중첩 적용된 값
//	public CharacterBase from;
//	public Coroutine coroutine;
//
//	public string buffText { get {
//		if (isImmune)
//			return Datatable.Inst.GetUIText(UITextEnum.BATTLE_PLAY_IMMUNE);
//		return Datatable.Inst.GetBuffElementText(idBuffElement);
//	} }
//}
//
//public class ArenaBuffData : BasicBuffData
//{
//	public int count;
//	
//	public bool isAura;
//	public bool isBuff;
//	public long endTime;
//	
//	public long endEffectTime;
//	public Material symbolMaterial;
//	
//	public CharacterBase from;
//	public CharacterBase target;
//	public int idFunction;
//	public int skillLv;
//	public double atk;
//	
//	public List<BuffElementData> buffElementDatas = new List<BuffElementData>();
//	
//	public List<ArenaBattleSplit> buffResults;		// for tick
//}
//
//public class GameCharInfo
//{
//	public int uiSlotIndex;
//	public CharacterBase character;
//	public Datatable.ActiveSkill touchSkillData;
//	public Datatable.ActiveSkill fatalSkillData;
//	public int fatalSkillLv;
//	public TouchSkillTimeType touchSkillTimeType;
//	public bool isCanControl;
//	public bool isEnableTouch;
//	public int chargedTouchSkillCount;
//	public int usedTouchSkillCount;
//	public float chargeTime;
//	public float reuseTime;
//}

[System.Serializable]
public class TargetEffectInfo
{
	public SKILL_TYPE skillType;
	public int effectSubjectNum;
	public string effectPrefabName;
	public string soundEffectName;
}

[System.Serializable]
public class ObjectiveEffectInfo
{
	public SKILL_TYPE skillType;
	public string effectPrefabName;
}

//public class GameResultArguments
//{
//	public GiantEPData epData;
//	public GameResult result;
//	public bool isRaidTimeOver;
//	public int stageStar;
//	public string bossFaceImageName;
//	public float bossHPPercent;
//	public int racePoint;
//	public int oldGrade;
//	public long bossTotalDamage;
//	public float bossTotalDamagePercent;
//	public bool isWorldBossNewRecord;
//	public List<GiantRewardData> rewards;
//	public GiantHeroData [] heroDatas;
//	public GameStatData [] myStatDatas;
//	public GameStatData [] enemyStatDatas;
//	public Dictionary<int, int> killDatas;
//
//	public void SetData(GiantEPData epData, GameResult result, bool isRaidTimeOver, int stageStar, string bossFaceImageName, float bossHPPercent, int racePoint, int oldGrade,
//		List<GiantRewardData> rewards, GiantHeroData [] heroDatas, GameStatData [] myStatDatas, GameStatData [] enemyStatDatas, Dictionary<int, int> killDatas,
//		long bossTotalDamage, float bossTotalDamagePercent, bool isWorldBossNewRecord)
//	{
//		this.epData = epData;
//		this.result = result;
//		this.isRaidTimeOver = isRaidTimeOver;
//		this.stageStar = stageStar;
//		this.bossFaceImageName = bossFaceImageName;
//		this.bossHPPercent = bossHPPercent;
//		this.racePoint = racePoint;
//		this.oldGrade = oldGrade;
//		this.rewards = rewards;
//		this.heroDatas = heroDatas;
//		this.myStatDatas = myStatDatas;
//		this.enemyStatDatas = enemyStatDatas;
//		this.killDatas = killDatas;
//		this.bossTotalDamage = bossTotalDamage;
//		this.bossTotalDamagePercent = bossTotalDamagePercent;
//		this.isWorldBossNewRecord = isWorldBossNewRecord;
//	}
//}

public class GameFormData
{
	public int [] idHeroes;
	public bool [] isAutoSkillEnables;
}

// 추후 삭제 예정
public struct GameFormation
{
	public int [] idCharDatas;
}

// 추후 삭제 예정
public struct AutoSettingSkillEnable
{
	public bool [] isAutoSkillEnableSettings;
}

public class PvpRevengeNotiData
{
	public List<long> pvpNotiList = new List<long>();
}

public class SupportedLanguage
{
	public List<string> supportedLanguageList = new List<string>();
}

public struct DangerSkillData
{
	public bool isStarted;
	public float animLength;
	public float elapsedTime;
}

//class EnemiesInfo
//{
//	public Datatable.StageEnemiesInfo stageEnemiesInfo;
//	public GiantHeroData heroData;
//	public Datatable.CharData charData;
//	
//	public static int SortEnemiesInfo(EnemiesInfo lhs, EnemiesInfo rhs)
//	{
//		if (lhs.charData != null && rhs.charData != null)
//		{
//			if (lhs.charData.Protect == rhs.charData.Protect)
//				return SortSubEnemiesInfo(lhs, rhs);
//			return rhs.charData.Protect.CompareTo(lhs.charData.Protect);
//		}
//		if (lhs.charData == rhs.charData)
//			return SortSubEnemiesInfo(lhs, rhs);
//		return (lhs.charData == null) ? 1 : -1;
//	}
//
//	private static int SortSubEnemiesInfo(EnemiesInfo lhs, EnemiesInfo rhs)
//	{
//		if (lhs.stageEnemiesInfo != null)
//			return lhs.stageEnemiesInfo.FakeKey.CompareTo(rhs.stageEnemiesInfo.FakeKey);
//		return rhs.heroData.idHero.CompareTo(lhs.heroData.idHero);
//	}
//	
//	public static int SortHeroIndex(EnemiesInfo lhs, EnemiesInfo rhs)
//	{
//		if (lhs.heroData != null && rhs.heroData != null)
//			return lhs.heroData.index.CompareTo(rhs.heroData.index);
//		return 0;
//	}
//}

public class GameStatData
{
	public int characterIndex;

	public string faceImage;
	public int role;
	public int rankFrame;
	public int rankValue;
	public int evo;
	public int limitBreak;

	public int curLv;
	public int curExp;
	public float curExpRate;	// 0 ~ 1 사이의 값
	public int nextLv;
	public int nextExp;
	public float nextExpRate;	// 0 ~ 1 사이의 값

	public long [] gameStats = new long[(int)GameStatType.Max];

	public void SetData(int characterIndex, string faceImage, int role, int rankFrame, int rankValue, int evo, int limitBreak, int curLv, int curExp, float curExpRate)
	{
		this.characterIndex = characterIndex;
		this.faceImage = faceImage;
		this.role = role;
		this.rankFrame = rankFrame;
		this.rankValue = rankValue;
		this.evo = evo;
		this.limitBreak = limitBreak;
		this.curLv = this.nextLv = curLv;
		this.curExp = this.nextExp = curExp;
		this.curExpRate = this.nextExpRate = curExpRate;
	}

	public bool isMyTeam { get { return characterIndex >= 0; } }
	public bool isEnemy { get { return characterIndex < 0; } }

	public void SetNextData(int nextLv, int nextExp, float nextExpRate)
	{
		this.nextLv = nextLv;
		this.nextExp = nextExp;
		this.nextExpRate = nextExpRate;
	}

	public long GetStat(GameStatType statType)
	{
		int index = (int)statType;
		if (gameStats.Length < 0 || gameStats.Length <= index)
			return 0;
		return gameStats[index];
	}
}

[System.Serializable]
public struct IntVector2
{
	public int x;
	public int y;

	public IntVector2(int initX, int initY)
	{
		x = initX;
		y = initY;
	}

	public static IntVector2 Zero()
	{
		return new IntVector2(0, 0);
	}

	public override int GetHashCode()
	{
		return x ^ y;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is IntVector2))
			return false;

		IntVector2 vector = (IntVector2)obj;
		return x.Equals(vector.x) && y.Equals(vector.y);
	}

	public static bool operator ==(IntVector2 a, IntVector2 b) { return a.Equals(b); }
	public static bool operator !=(IntVector2 a, IntVector2 b) { return !a.Equals(b); }

	public static IntVector2 operator +(IntVector2 a, IntVector2 b) { return new IntVector2(a.x + b.x, a.y + b.y); }
	public static IntVector2 operator -(IntVector2 a, IntVector2 b) { return new IntVector2(a.x - b.x, a.y - b.y); }
	public static IntVector2 operator *(IntVector2 a, IntVector2 b) { return new IntVector2(a.x * b.x, a.y * b.y); }
	public static IntVector2 operator /(IntVector2 a, IntVector2 b) { return new IntVector2(a.x / b.x, a.y / b.y); }
}

public struct Vector
{
	public float x;
	public float y;
	public float z;

	public void Set(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public void Set(Vector3 v)
	{
		x = v.x;
		y = v.y;
		z = v.z;
	}

	public Vector3 GetVector3()
	{
		return new Vector3(x, y, z);
	}
}

[System.Serializable]
public struct StageHelperData
{
	public int id;

	// 월드 좌표 기준
	public Vector3 cameraPos;
	public Vector3 cameraRot;	// eular angle
	public float cameraFOV;
	public float cameraZoomLimit;
	public List<Vector3> enemiesPos;
	public List<Vector3> towersPos;
	public List<Vector3> charsPos;
	public List<Vector3> enemySummonsPos;
	public List<Vector3> mySummonsPos;
}

[System.Flags]
public enum FocusTouchSkillFlag
{
	None		= 0,
	TimeEnd		= 0x1,
	ActionEnd	= 0x2,
}

public struct FocusTouchSkillData
{
	public FocusTouchSkillFlag flag;
	public bool isFatalSkill;
}

public struct QuestIconData
{
	public bool isFace;
	public bool isItem;
	public bool isBoss;
	public int frameIndex;
	public int lv;
	public int objTarget;
	public CHARACTER_TYPE objCharType;
	public SpriteType iconSpriteType;

	public Color GetRankColor()
	{
		if(frameIndex == 0)
			return GlobalDataStore.Inst.GetGameColor(ColorType.EmptyAdd);
		else
			return GlobalDataStore.Inst.GetGameColor(ColorType.Rank1 + frameIndex - 1);
	}

	public void Clear()
	{
		isFace = isItem = isBoss = false;
		frameIndex = lv = objTarget = 0;
		objCharType = CHARACTER_TYPE.ENDINDEX;
		iconSpriteType = SpriteType.None;
	}
}

//public class QuestItemDrops
//{
//	public Dictionary<int, List<Datatable.QuestItemDrop>> dicItemDrops = new Dictionary<int, List<_Datatable.QuestItemDrop>>();	// Key : ItemID (QuestItem)
//
//	public void Add(Datatable.QuestItemDrop data)
//	{
//		int key = data.QuestItem;
//		if (!dicItemDrops.ContainsKey(key))
//			dicItemDrops.Add(key, new List<_Datatable.QuestItemDrop>());
//		dicItemDrops[key].Add(data);
//	}
//}

public struct SelectedHeroData
{
	public int idHero;
	public HeroSelectType selectType;
}

public class ZipDatabase
{
	public string [] zipFileNames;
	public Dictionary<string, int> hashFileDatas;		// Key : hash file name, Value : zip file name index

	public int GetZipFileIndex(string hash)
	{
		int index;
		if (hashFileDatas.TryGetValue(hash, out index))
			return index;
		return -1;
	}

	public string GetZipFileName(string hash)
	{
		if (hashFileDatas.ContainsKey(hash))
			return zipFileNames[hashFileDatas[hash]];
		return string.Empty;
	}
}

public class AssetBundleDatabase
{
	public string [] assetbundleNames;
	public Dictionary<string, int> assetDatas;		// Key : asset name, Value : asset bundle name index
	public List<string> faceSpriteAtlasNames;
	
	public string GetAssetBundleName(string assetName)
	{
		if (assetDatas.ContainsKey(assetName))
			return assetbundleNames[assetDatas[assetName]];
		return string.Empty;
	}
	
	public string GetFaceAssetBundleName(int index)
	{
		return GetAssetBundleName(faceSpriteAtlasNames[index]);
	}
}

public class PlantLostRewardData
{
	public int 			count;
	public bool 		lost;
}

public class SettingData
{
	public SoundSettingData soundData;
	public GameSettingData gameData;
	
	public string ToJson()
	{
		return Util.JsonEncode2(this);
	}
}

public class SoundSettingData
{
	public bool 		isMuteSoundEft;
	public bool 		isMuteSoundBGM;	
	public float 		volumeEffect;
	public float 		volumeBGM;

	public SoundSettingData()
	{
		Reset();
	}

	public void Reset()
	{
		isMuteSoundEft		 	= false;
		isMuteSoundBGM		 	= false;
		volumeEffect			= 0.5f;
		volumeBGM		 		= 0.5f;
	}
}
	
public class GameSettingData
{
	public bool 		isAllAlert;
	public bool 		isNoticeAlert;
	public bool 		isNightAlert;
	public bool 		isPreventSleep;
	public int 			fpsGrade;			// 1 : low, 2: mid, 3: high,  default : high	
	public string 		currentLanguage;

	public long 		savedMidNightTime;
	public bool 		isShopCrmOff;

	public Dictionary<int, bool> gameAlerts = new Dictionary<int, bool>();

	public GameSettingData()
	{
		Reset();
	}

	public void Reset()
	{
		isAllAlert		 		= true;
		isNoticeAlert		 	= true;
		isNightAlert		 	= false;
		isPreventSleep 			= true;
		currentLanguage 		= string.Empty;
		isShopCrmOff 			= false;
		fpsGrade 				= 2;

		for (int i = (int)SETTING_PUSH.FREE_CHEST; i < (int)SETTING_PUSH.ENDINDEX; ++i)
		{
			gameAlerts[i]		= true;
		}
	}
}
	
public enum CHAT_FUNC { 
	INIT						= 1,        
	STATE,
	MESSAGE,
	SYSTEM_MSG,
	REQ_JOIN,
	JOIN,
	LEAVE,
	ERROR,
	HISTORY,
	NOTICE,
	BUFF_EVENT,
	NONE,
}

public enum CHAT_TYPE { 
	NORMAL						= 1,        
	GUILD,  			          
	ALL,
	NONE,
}

public enum CHAT_SUB_TYPE { 
	
	CHANGE_SETTING				= 1,        
	SIGNUP,  			          
	DENY_SIGNUP,
	ACCEPT_SIGNUP,
	CHANGE_AUTH, 			   
	EXILE_SIGNUP, 			   
	SECEDE_SIGNUP, 	
	REQUEST_SIGNUP,
	DONATE_STOCK,
	FULL_STOCK_NOTI,
	UPGRADE_BUILDING,
	TARGET_GUILD_SHARE,
	JOIN_GUILD_BATTLE_REWARD,	
	REJOIN_GUILD_BATTLE,	
	GET_COIN_DEFENSE_BONUS,	
	GET_COIN_BROKEN_GUILD,	
	NONE,
}

public class CraftItemData
{
	public int idItem;
	public int count;
}

public class MainQuestStageData
{
	public int idMainQuest;
	public int idStage;
}

public class AutoExplorationSettingData
{
	public int searchCount;
	public int stopItemType;
	public int stopCardType;

	public int playMonsterStageType;
	public int playBattleType;
	public int playRewardType;
	public int playGatherType;

	public bool isAlarm;

	public AutoExplorationSettingData Reset()
	{
		searchCount = 10;
		stopItemType = 1 << (int)AutoExplorationStopItemType.Ether | 1 << (int)AutoExplorationStopItemType.Item | 1 << (int)AutoExplorationStopItemType.Core;
		stopCardType = 0;

		playMonsterStageType = 1 << (int)AutoExplorationPlayMonsterStageType.Grade3 | 1 << (int)AutoExplorationPlayMonsterStageType.Grade4;
		playBattleType = 1 << (int)AutoExplorationPlayBattleType.Target | 1 << (int)AutoExplorationPlayBattleType.NotPerfectClear | 1 << (int)AutoExplorationPlayBattleType.SubQuest;
		playRewardType = 0;
		playGatherType = 0;

		isAlarm = true;
		return this;
	}
}

//public class AutoExplorationResultData
//{
//	public float startTime;
//	public int startGold;
//	public int startEther;
//	public int searchCount;
//	public int feverCount;
//	public int succBattleCount;
//	public int failBattleCount;
//
//	public float time;
//	public int gold;
//	public int ether;
//
//	private bool isStart;
//
//	public void Start(GiantUserData user)
//	{
//		isStart = true;
//
//		startTime = Time.unscaledTime;
//		startGold = user.tGold;
//		startEther = user.ether;
//		searchCount = feverCount = succBattleCount = failBattleCount = 0;
//
//		time = 0;
//		gold = 0;
//		ether = 0;
//	}
//
//	public bool Check(GiantUserData user)
//	{
//		if (!isStart)
//			return false;
//		time = Time.unscaledTime - startTime;
//		gold = user.tGold - startGold;
//		ether = user.ether - startEther;
//		return true;
//	}
//
//	public void Stop()
//	{
//		isStart = false;
//	}
//
//	public void AddSearchCount()
//	{
//		++searchCount;
//	}
//
//	public void AddFeverCount()
//	{
//		++feverCount;
//	}
//
//	public void AddBattleCount(bool isWin)
//	{
//		if (isWin)
//			++succBattleCount;
//		else
//			++failBattleCount;
//	}
//}

//public class SeasonInfo
//{
//	protected SeasonInfoType seasonInfoType;
//
//	public int 		idSeason;
//	public long 	startDate;
//	public long 	endDate;
//	public long 	resetDate;
//	public long		boardPostDate;
//
//	public int		idSeasonNext;
//	public long		startDateNext;
//
//	private bool	isReceiveSeasonReward;
//
//	public SeasonInfo(SeasonInfoType type)
//	{
//		seasonInfoType = type;
//	}
//
//	public bool IsNewSeason()
//	{
//		return idSeasonNext != 0 && GiantHandler.Inst.serverTime >= startDateNext;
//	}
//
//	public bool IsShowRanking()
//	{
//		return IsOnSeason() || (!IsCheckingTime() && GiantHandler.Inst.serverTime < boardPostDate);
//	}
//
//	public bool IsOnSeason()
//	{
//		return GiantHandler.Inst.serverTime >= startDate && GiantHandler.Inst.serverTime < endDate;
//	}
//
//	public bool IsBoardPostTime()
//	{
//		return GiantHandler.Inst.serverTime >= endDate && GiantHandler.Inst.serverTime < boardPostDate;
//	}
//
//	public bool IsCheckingTime()
//	{
//		return GiantHandler.Inst.serverTime >= endDate && GiantHandler.Inst.serverTime < resetDate;
//	}
//
//	// resetDate ~ boardPostDate : 시즌 보상을 받을 수 있는 기간
//	public bool IsSeasonRewardTime()
//	{
//		return GiantHandler.Inst.serverTime >= resetDate && GiantHandler.Inst.serverTime < boardPostDate;
//	}
//
//	// startDate ~ endDate + calculateStartTime : 전투 보상을 받을 수 있는 기간
//	public bool IsRewardTime()
//	{
//		return GiantHandler.Inst.serverTime >= startDate && GiantHandler.Inst.serverTime < endDate + GetCalculateStartTime() * 60000;
//	}
//
//	public int GetCalculateStartTime()
//	{
//		switch (seasonInfoType)
//		{
//		case SeasonInfoType.Arena:
//			return Datatable.Inst.settingData.ArenaResetCalculateStart;
//		}
//		return 0;
//	}
//
//	public bool IsBoardPostOverTime()
//	{
//		return GiantHandler.Inst.serverTime >= boardPostDate;
//	}
//
//	public void UpdateLogin(Action cb)
//	{
//		switch (seasonInfoType)
//		{
//		case SeasonInfoType.Arena:
//			Datatable.Inst.UpdateSeason(ARENADUEL_TYPE.ARENA, this);
//			break;
//		case SeasonInfoType.Duel:
//			Datatable.Inst.UpdateSeason(ARENADUEL_TYPE.DUEL, this);
//			break;
//		}
//		UpdateSeasonAfter();
//		ReceiveSeasonReward(cb);
//	}
//
//	public IEnumerator UpdateLobby(Action cb = null)
//	{
//		if (IsNewSeason())
//		{
//			switch (seasonInfoType)
//			{
//			case SeasonInfoType.Arena:
//				Datatable.Inst.UpdateSeason(ARENADUEL_TYPE.ARENA, this);
//				yield return GiantHandler.Inst.doArenaSeasonInfo().WaitForResponse();
//				break;
//			case SeasonInfoType.Duel:
//				Datatable.Inst.UpdateSeason(ARENADUEL_TYPE.DUEL, this);
//				yield return GiantHandler.Inst.doDuelSeasonInfo().SetSuccessAction(cb).WaitForResponse();
//				break;
//			}
//			UpdateSeasonAfter();
//		}
//		ReceiveSeasonReward(cb);
//	}
//
//	protected virtual void UpdateSeasonAfter()
//	{
//		isReceiveSeasonReward = false;
//	}
//
//	private void ReceiveSeasonReward(Action cb)
//	{
//		Action successAction = ()  => {
//			if (cb != null)
//				cb();
//		};
//
//		Action<object> failureAction = (error) => {
//			GiantHandler.DefaultRequestFailure(error);
//			if (cb != null)
//				cb();
//		};
//
//		if (!isReceiveSeasonReward && IsSeasonRewardTime())
//		{
//			switch (seasonInfoType)
//			{
//			case SeasonInfoType.Arena:
//				GiantHandler.Inst.doArenaSeasonReward().SetSuccessAction(successAction).SetFailureAction(failureAction);
//				break;
//			case SeasonInfoType.Duel:
//				GiantHandler.Inst.doDuelSeasonReward().SetSuccessAction(successAction).SetFailureAction(failureAction);
//				break;
//			}
//		}
//		else
//		{
//			successAction();
//		}
//	}
//}
//
//public class ArenaSeasonInfo : SeasonInfo
//{
//	public ArenaSeasonInfo() : base(SeasonInfoType.Arena)
//	{
//	}
//
//	private Dictionary<int, List<Datatable.ArenaGrade>> m_ArenaGradeByGradeGroup = new Dictionary<int, List<Datatable.ArenaGrade>>();	// 끝 ~ 1까지 내림차순
//	public List<Datatable.ArenaGrade> GetArenaGradeByGradeGroup()
//	{
//		int gradeGroup;
//		Datatable.ArenaDuelSeason season;
//		if (Datatable.Inst.dtArenaDuelSeason.TryGetValue(idSeason, out season))
//			gradeGroup = season.GradeGroup;
//		else
//			return null;
//		List<Datatable.ArenaGrade> arenaGrades;
//		if (m_ArenaGradeByGradeGroup.TryGetValue(gradeGroup, out arenaGrades))
//			return arenaGrades;
//
//		List<Datatable.ArenaGrade> tempArenaGrades = new List<Datatable.ArenaGrade>();
//		m_ArenaGradeByGradeGroup.Add(gradeGroup, tempArenaGrades);
//		Datatable.ArenaGrade arenaGrade;
//		var enumerator = Datatable.Inst.dtArenaGrade.Values.GetEnumerator();
//		while (enumerator.MoveNext())
//		{
//			arenaGrade = enumerator.Current;
//			if (arenaGrade.GradeGroup != gradeGroup)
//				continue;
//			tempArenaGrades.Add(arenaGrade);
//		}
//
//		tempArenaGrades.Sort((x, y) => {
//			if (x.ConditionPoint != y.ConditionPoint)
//				return y.ConditionPoint.CompareTo(x.ConditionPoint);
//			if (x.ConditionRanking != y.ConditionRanking)
//				return y.ConditionRanking.CompareTo(x.ConditionRanking);
//			return 0;
//		});
//
//		return tempArenaGrades;
//	}
//
//	public Datatable.ArenaGrade GetMyGradeData()
//	{
//		int myGrade = AccountDataStore.instance.user.arenaGrade;
//		return GetGradeData(myGrade);
//	}
//
//	public int GetGrade(int point, int ranking)
//	{
//		Datatable.ArenaGrade arenaGrade = GetGradeData(point, ranking);
//		if (arenaGrade != null)
//			return arenaGrade.Grade;
//		return 0;
//	}
//
//	public Datatable.ArenaGrade GetGradeData(int point, int ranking)
//	{
//		int grade = 0;
//		var list = GetArenaGradeByGradeGroup();
//		for (int i = 0; i < list.Count; ++i)
//		{
//			var data = list[i];
//			if (grade < data.Grade && point >= data.ConditionPoint && (data.ConditionRanking == 0 || ranking <= data.ConditionRanking))
//				grade = data.Grade;
//		}
//		return GetGradeData(grade);
//	}
//	
//	public Datatable.ArenaGrade GetGradeData(int grade)
//	{
//		var list = GetArenaGradeByGradeGroup();
//		for (int i = 0; i < list.Count; ++i)
//		{
//			if (list[i].Grade == grade)
//				return list[i];
//		}
//		return null;
//	}
//
//	public float GetGradeRate(float point, int grade)
//	{
//		var list = GetArenaGradeByGradeGroup();
//		int index = list.Count - grade;
//		int nextIndex = index - 1;
//		if (index < 0 || index >= list.Count)
//			return 0f;
//		if (nextIndex < 0)
//			return 1f;
//		Datatable.ArenaGrade curData = list[index];
//		Datatable.ArenaGrade nextData = list[nextIndex];
//		return (float)(point - curData.ConditionPoint) / (nextData.ConditionPoint - curData.ConditionPoint);
//	}
//}
//
//public class DuelSeasonInfo : SeasonInfo
//{
//	private int [] idBannedHeroes = new int[Constant.HeroBanMax];
//	private int raceBuff;
//
//	public DuelSeasonInfo() : base(SeasonInfoType.Duel)
//	{
//	}
//
//	protected override void UpdateSeasonAfter()
//	{
//		base.UpdateSeasonAfter();
//
//		Datatable.ArenaDuelSeason season;
//		if (Datatable.Inst.dtArenaDuelSeason.TryGetValue(idSeason, out season))
//		{
//			raceBuff = season.RaceBuff;
//
//			var data = Datatable.Inst.dtDuelHeroBan[season.HeroBanGroup];
//			idBannedHeroes[0] = data.BannedHeroID_0;
//			idBannedHeroes[1] = data.BannedHeroID_1;
//			idBannedHeroes[2] = data.BannedHeroID_2;
//		}
//		else
//		{
//			raceBuff = (int)RACE.NONE;
//
//			for (int i = 0; i < idBannedHeroes.Length; ++i)
//				idBannedHeroes[i] = 0;
//		}
//	}
//
//	public int GetRaceBuff()
//	{
//		return raceBuff;
//	}
//
//	public int [] GetBannedHeroIDs()
//	{
//		return idBannedHeroes;
//	}
//
//	public bool IsBannedHero(int idHero)
//	{
//		for (int i = 0; i < idBannedHeroes.Length; ++i)
//		{
//			if (idHero == idBannedHeroes[i])
//				return true;
//		}
//		return false;
//	}
//}

public enum PanelType
{
	LobbyMain,
	ExplorationMain,
	DetailHero,
	GameFormEdit,
	Purchase,
	BaseMain,
	DetailGiantAndTower,
	DetailPlant,
	Gather,
	NPC,
	Laboratory,
	Achievement,
	Core,
	ExplorationMonsterInfo,
	Inventory,
	PvpInfo,
	WorkRoom,
	Arena,
	DeckSetting,
	GuildBase,
	GuildHall,
	GuildShop,
	GuildStorage,
	WorldBoss,
	GuildBattle,
	GuildLaboratory,
	GuildBattleDefenseSetting,
	GuildTarget,
	GameFormSetting,
	Duel,
	EndIndex,
}

public enum ColorType
{
	Rank1						= 0,
	Rank2,
	Rank3,
	Rank4,
	Rank5,
	Rank6,

	Highlight					= 6,
	Progress,
	Complete,

	Disable						= 9,
	Caution,
	Warning,
	Normal,

	Darklight					= 13,

	Gold						= 14,
	Ether,
	Cash,

	ThumbAlpha					= 17,

	DarkEther					= 18,
	Lumber,
	Cubic,

	Empty						= 21,

	Gauge						= 22,

	HP							= 23,
	EnemyHP						= 24,

	New							= 25,
	Base						= 26,
	EmptyAdd					= 27,

	Elite						= 28,
	Supporter,

	Up							= 30,
	Down,

	HeroExp						= 32,

	SkillFatal					= 33,
	SkillTouch,
	SkillAuto,

	BaseExp						= 36,

	Attack						= 37,
	Defense,
	Support,

	Evolution					= 40,

	Physical					= 41,
	Magical,
	Rate,
	Real,

	HPBack						= 45,
	EnemyHPBack,
	MyTeamList,
	EnemyTeamList,

	Core						= 49,
	Soul,

	Boss						= 51,

	FeverStar					= 52,

	ItemEquipEnable				= 53,
	ItemEquipDisable			= 54,

	ExtraEp						= 55,
	RankUp						= 56,
	Notice						= 57,
	
	AreaNormal					= 58,
	AreaHard,

	Fatigue0					= 60,
	Fatigue1,
	Fatigue2,
	Fatigue3,
	Fatigue4,
	
	Dungeon						= 65,
	GameHeroPlacement			= 66,
	SkillReuse					= 67,

	Rank1Background				= 68,
	Rank2Background,
	Rank3Background,
	Rank4Background,
	Rank5Background,
	Rank6Background,

	PvpRewardRank0				= 74,
	PvpRewardRank1				= 75,
	PvpRewardRank2				= 76,
	PvpRewardRank3				= 77,
	PvpRewardRank4				= 78,
	PvpRewardRank5				= 79,
	PvpRewardRank6				= 80,
	PvpRewardRank7				= 81,

	Giant						= 82,

	None						= 99999,
}

public enum SpriteType
{
	None						= 0,

	Icon_BaseExp				= 1,
	Icon_Gold,
	Icon_Ether,
	Icon_FeverStar_Large,
	Icon_Cash,
	Icon_DarkEther,
	Icon_Lumber,
	Icon_Cubic,
	Icon_FeverStar_Small,
	Icon_Battle,
	Icon_Herb,
	Icon_Vein,
	Icon_Tree,
	Icon_PostDefault,
	Icon_RandomItem,
	Icon_HeroExp,
	Icon_Ethereum,
	Icon_ArenaCurrency,
	Icon_DungeonToken,
	Icon_GuildPoint,
	Icon_GuildCoin,

	Icon_FeverStar_Disable		= 51,

	IconFrame_0					= 100,
	IconFrame_1					= 101,
	IconFrame_2,
	IconFrame_3,
	IconFrame_4,
	IconFrame_5,
	IconFrame_6,
	IconFrame_7,

	IconFrame_Skill				= 150,
	IconFrame_Inscription,
	IconFrame_BackgroundRank_0,

	RankFrame_Default			= 200,
	RankFrame_1					= 201,
	RankFrame_2,
	RankFrame_3,
	RankFrame_4,
	RankFrame_5,
	RankFrame_6,
	RankFrame_7,

	RankFrame_Fever				= 250,
	RankFrame_Item,

	IconRace_0					= 280,
	IconRace_1,
	IconRace_2,
	IconRace_3,
	IconRace_4,
	IconRace_5,
	IconRace_6,
	IconRace_7,
	IconRace_8,

	IconRole_1					= 301,
	IconRole_2,
	IconRole_3,
	IconRole_4,
	IconRole_5,
	IconRole_6,
	IconRole_7,

	IconAssignRole_1			= 311,
	IconAssignRole_2,
	IconAssignRole_3,

	IconSign_ArrowHorizontal_01	= 321,
	IconSign_Minus,
	IconSign_New,

	IconQuest_Hero				= 401,
	IconQuest_Search,
	IconQuest_Treasure,
	IconQuest_Base_Battle,
	IconQuest_PVP_Battle,
	IconQuest_Extraboss,
	IconQuest_Inscription,

	IconRank_1					= 501,
	IconRank_2,
	IconRank_3,
	IconRank_4,
	IconRank_5,
	IconRank_6,

	IconHeroStar_0				= 600,
	IconHeroStar_1				= 601,

	IconSelect_Elite			= 604,
	IconSelect_Supporter,

	IconHero_Penalty			= 611,
	IconHero_Ban,
	IconHero_Ban_Rank,

	Tab_Unselect				= 701,
	Tab_Select,
	Tab_Disable,

	IconBase_UsedBuilding		= 801,
	IconBase_UsedBuilding_2,
	IconBase_UsedBuilding_3,
	IconBase_UsedBuilding_4,

	CardBackground_Normal		= 901,
	CardBackground_Merchant,
	CardBackground_Fever,
	CardBackground_PVP,
	CardBackground_Base,
	CardBackground_Disable		= 909,

	IconCardGold_0				= 910,
	IconCardGold_1,
	IconCardGold_2,
	IconCardGold_3,

	IconCardEther_0				= 920,
	IconCardEther_1,
	IconCardEther_2,
	IconCardEther_3,

	IconCardCash_0				= 930,
	IconCardCash_1,

	IconTreasureBox_0			= 940,
	IconTreasureBox_1,
	IconTreasureBox_2,
	IconTreasureBox_3,

	IconTreasureBox_Open_0		= 950,
	IconTreasureBox_Open_1,
	IconTreasureBox_Open_2,
	IconTreasureBox_Open_3,

	IconTreasureBox_Shadow_0	= 960,
	IconTreasureBox_Shadow_1,
	IconTreasureBox_Shadow_2,
	IconTreasureBox_Shadow_3,

	IconCard_Header_Boss_01		= 970,
	IconCard_Header_PVP,
	IconCard_Header_Extra_Boss,
	
	CardBackground_Race0		= 980,
	CardBackground_Race1,
	CardBackground_Race2,
	CardBackground_Race3,
	CardBackground_Race4,
	CardBackground_Race5,
	CardBackground_Race6,
	CardBackground_Race7,
	CardBackground_Race8,

	FaceCard_BaseRaid_NoGiant	= 990,

	RankBackground_Default		= 1000,
	RankBackground_1			= 1001,
	RankBackground_2,
	RankBackground_3,
	RankBackground_4,
	RankBackground_5,
	RankBackground_6,
	RankBackground_7,

	IconState_Lock				= 1201,
	IconState_LockAdd,
	IconState_Plus,
	IconState_Check_0,
	IconState_Check_1,
	IconState_Check_2,
	IconState_Check_3,
	IconState_Lock_Hall,
	IconState_UnLock_Hall,
	IconState_Add_Hall,
	IconState_MenuFolder_On,
	IconState_MenuFolder_Off,

	IconNoti_Equip				= 1301,
	IconNoti_LevelUp,
	IconNoti_Skill,
	IconNoti_Costume,
	IconNoti_Empty,

	IconNoti_Red				= 1310,
	IconNoti_Purple,

	IconNoti_MainEvo			= 1320,
	IconNoti_MainEquip,

	Purchase_Crystal_Icon_1		= 1401,
	Purchase_Crystal_Icon_2,
	Purchase_Crystal_Icon_3,
	Purchase_Crystal_Icon_4,
	Purchase_Crystal_Icon_5,
	Purchase_Crystal_Icon_6,

	Purchase_Box_Free			= 1500,
	Purchase_Box_Hero01			= 1501,
	Purchase_Box_Item01,
	Purchase_Box_Core01,
	Purchase_Box_CoreHero01,

	Purchase_Box_Free_Open		= 1550,
	Purchase_Box_Hero01_Open	= 1551,
	Purchase_Box_Item_Open,
	Purchase_Box_Core_Open,
	Purchase_Box_CoreHero_Open,

	Quest_Empty_Frame			= 1600,
	Quest_Normal_Frame,

	Laboratory_Background_0		= 1700,
	Laboratory_Background_1,			
	Laboratory_Background_2,
	Laboratory_Background_3,

	GlobalButton_1				= 1710,
	GlobalButton_2,				
	GlobalButton_3,
	GlobalButton_4,
	GlobalButton_5,
	GlobalButton_6,

	GameFormEditButton_Reward	= 1720,
	GameFormEditButton_Battle,

	NPC_CraftBackGround_0		= 1800,
	NPC_CraftBackGround_1		= 1801,

	NPC_CraftIcon_Default		= 1810,
	NPC_CraftIcon_0				= 1811,
	NPC_CraftIcon_1,
	NPC_CraftIcon_2,
	NPC_CraftIcon_3,
	NPC_CraftIcon_4,

	NPC_Card_Frame					= 1820,
	NPC_Card_BG,

	NPC_Craft_Role_Icon_Default		= 1830,
	NPC_Craft_Role_Icon_1			= 1831,
	NPC_Craft_Role_Icon_2,
	NPC_Craft_Role_Icon_3,
	NPC_Craft_Role_Icon_4,
	NPC_Craft_Role_Icon_5,

	NPC_Craft_Inscription_BG		= 1840,

	NPC_Empty_Frame_Icon_Normal		= 1850,
	NPC_Empty_Frame_Icon_Inscription,

	Search_Buff_Type_Empty_Icon_1	= 2000,
	Search_Buff_Type_Empty_Icon_2,

	Base_Navigation_Button_BG_Help	= 2100,
	Base_Navigation_Button_BG_Normal,
	Base_Navigation_Button_BG_Save,
	Base_Navigation_Button_BG_Full,

	Base_Navigation_Button_Icon_Default 	= 2110,
	Base_Navigation_Button_Icon_1 			= 2111,
	Base_Navigation_Button_Icon_2,
	Base_Navigation_Button_Icon_3,
	Base_Navigation_Button_Icon_4,
	Base_Navigation_Button_Icon_5,
	Base_Navigation_Button_Icon_6,
	Base_Navigation_Button_Icon_7,
	Base_Navigation_Button_Icon_8,

	GameSetting_Sound_On			= 2200,
	GameSetting_Sound_Off,
	GameSetting_CheckBox_On,

	PVP_Grade_0						= 2300,
	PVP_Grade_1,
	PVP_Grade_2,
	PVP_Grade_3,
	PVP_Grade_4,
	PVP_Grade_5,
	PVP_Grade_6,
	PVP_Grade_7,
	PVP_Grade_8,
	PVP_Grade_9,
	PVP_Grade_10,

	Hero_SkillListItem_BG_0			= 2400,
	Hero_SkillListItem_BG_1,

	Core_Rank_Frame_Default			= 2500,
	Core_Rank_Frame_1				= 2501,	
	Core_Rank_Frame_2,
	Core_Rank_Frame_3,
	Core_Rank_Frame_4,
	Core_Rank_Frame_5,
	Core_Rank_Frame_Unique,

	Core_Rank_BG_Default			= 2510,
	Core_Rank_BG_1					= 2511,
	Core_Rank_BG_2,
	Core_Rank_BG_3,
	Core_Rank_BG_4,
	Core_Rank_BG_5,
	Core_Rank_BG_Unique,

	Core_Like						= 2530,
	Core_DisLike,

	Core_Select_Base				= 2540,
	Core_Select_Material,

	Quest_Type_Icon_Hero_Number		= 2600,
	Quest_Type_Icon_Search_Count,
	Quest_Type_Icon_Hero_Get_Treasure,
	
	Pvp_Crown_Icon_0				= 2800,
	Pvp_Crown_Icon_1,
	Pvp_Crown_Icon_2,
	Pvp_Crown_Icon_3,
	Pvp_Crown_Icon_4,

	Dungeon_Clear_Area_BG_Image_0	= 2900,

	Arena_Grade_Icon_Default		= 3000,
	Arena_Grade_Icon_0,
	Arena_Grade_Icon_1,
	Arena_Grade_Icon_2,
	Arena_Grade_Icon_3,
	Arena_Grade_Icon_4,
	Arena_Grade_Icon_5,
	Arena_Grade_Icon_6,
	Arena_Grade_Icon_7,
	Arena_Grade_Icon_8,
	Arena_Grade_Icon_9,

	BM_Mix_Default_BG				= 3100,
	BM_Mix_Default_Frame,
	BM_Mix_Random_Icon,
	BM_Mix_Select_Icon,
	BM_Mix_Hero_Icon,
	BM_Mix_Core_Icon,

	Donation_Icon_DarkEther_0		= 3200,
	Donation_Icon_DarkEther_1,
	Donation_Icon_DarkEther_2,
	Donation_Icon_Lumber_0,
	Donation_Icon_Lumber_1,
	Donation_Icon_Lumber_2,
	Donation_Icon_Cubic_0,
	Donation_Icon_Cubic_1,
	Donation_Icon_Cubic_2,

	Daily_Quest_Icon_Search			= 3300,
	Daily_Quest_Icon_Monster_Kill,
	Daily_Quest_Icon_Boss_Kill,
	Daily_Quest_Icon_Fever_Star,
	Daily_Quest_Icon_Treasure_Box,
	Daily_Quest_Icon_Gather_Tree,
	Daily_Quest_Icon_Gather_Herb,
	Daily_Quest_Icon_Gather_Vein,
	Daily_Quest_Icon_Gold_Npc_Shop,
	Daily_Quest_Icon_Like_Up,
	Daily_Quest_Icon_Fever_Search,
	Daily_Quest_Icon_Search_Buff_Elixer,
	Daily_Quest_Icon_Search_Buff_Scroll,
	Daily_Quest_Icon_Core_Enchant,
	Daily_Quest_Icon_Core_Evolution,
	Daily_Quest_Icon_User_Battle_Base,
	Daily_Quest_Icon_User_Battle_PVP,
	Daily_Quest_Icon_User_Battle_Arena,
	Daily_Quest_Icon_Race_Battle,

	Core_Content_Stat_Start			= 3400,
	Core_Content_Stat_Arena,
	Core_Content_Stat_PVP,
	Core_Content_Stat_Raid,

	TargetInfo_Line01				= 3500,
	TargetInfo_Line02,
	TargetInfo_Line03,
		
	Base_Production0_Building		= 10000,
	Base_Production0_Building_2,
	Base_Production0_Building_3,
	Base_Production0_Building_4,

	Base_Storage0_Building			= 10100,
	Base_Storage0_Building_2,
	Base_Storage0_Building_3,
	Base_Storage0_Building_4,

	Base_ResourceIcon0_Building		= 10200,
	Base_ResourceIcon0_Building_2,
	Base_ResourceIcon0_Building_3,
	Base_ResourceIcon0_Building_4,

	Base_ResourceIcon1_Building		= 10210,
	Base_ResourceIcon1_Building_2,
	Base_ResourceIcon1_Building_3,
	Base_ResourceIcon1_Building_4,

	Base_ResourceIcon2_Building		= 10220,
	Base_ResourceIcon2_Building_2,
	Base_ResourceIcon2_Building_3,
	Base_ResourceIcon2_Building_4,

	Base_IconBuilding				= 10230,
	Base_IconBuilding_2,
	Base_IconBuilding_3,				
	Base_IconBuilding_4,				

	Base_Builing_Hall				= 10240,
	Base_Builing_Refinery,
	Base_Builing_Lumbermill,
	Base_Builing_Mine,
	Base_Builing_GiantBuilding,
	Base_Builing_Research,
	Base_Builing_Workshop,

	Base_Hall_Refinery_Open			= 10250,
	Base_Hall_Lumbermill_Open,
	Base_Hall_Mine_Open,
	Base_Hall_GiantBuilding_Open,
	Base_Hall_Research_Open,
	Base_Hall_WorkShop_Open,

	Base_Hall_Refinery_Production	= 10260,
	Base_Hall_Lumbermill_Production,
	Base_Hall_Mine_Production,

	Base_Hall_Refinery_Storage		= 10270,
	Base_Hall_Lumbermill_Storage,
	Base_Hall_Mine_Storage,

	Base_Hall_Tower_Add				= 10280,
	Base_Hall_WorkShop_Slot,

	Core_List_Check					= 10300,
	Core_List_Uncheck,

	Max_Wealth_Item_Gold			= 10310,
	Max_Wealth_Item_Ether,
	Max_Wealth_Item_DungeonToken,

	Base_Plant_Button_Upgrade		= 10400,
	Base_Plant_Button_InstantlyComplete,

	Guild_Base_Storage_Dark			= 10501,
	Guild_Base_Storage_Lumber,
	Guild_Base_Storage_Cubic,

	Exploration_World_Map_1			= 90000,
	Exploration_World_Map_2,
	Exploration_World_Map_3,
	Exploration_World_Map_4,
	Exploration_World_Map_5,

	Base_BG_Default					= 90100,
	Base_BG_Detail_Default,
	Base_BG_Detail_DarkEther,
	Base_BG_Detail_Lumber,
	Base_BG_Detail_Cubic,

	Exploration_Blur_World_Map_1	= 91000,
	Exploration_Blur_World_Map_2,
	Exploration_Blur_World_Map_3,
	Exploration_Blur_World_Map_4,
	Exploration_Blur_World_Map_5,

	Hero_Blur_World_Map_1			= 91100,
	Hero_Blur_World_Map_2,
	Hero_Blur_World_Map_3,
	Hero_Blur_World_Map_4,
	Hero_Blur_World_Map_5,
	
	Exploration_World_Area_1		= 91200,
	Exploration_World_Area_2,
	Exploration_World_Area_3,
	Exploration_World_Area_4,
	Exploration_World_Area_5,
	Exploration_World_Area_6,
	Exploration_World_Area_7,
	Exploration_World_Area_8,
	Exploration_World_Area_9,
	Exploration_World_Area_10,
	Exploration_World_Arena			= 91299,

	Exploration_World_Ocean_Normal	= 91300,
	Exploration_World_Ocean_Hard,
	Exploration_World_Map_Dungeon,
	Exploration_World_Map_WorldBoss,
	Exploration_World_Map_Arena,
	Exploration_World_Map_Duel,

	BUILDING_NONE	 				= 91400,        
	BUILDING_HALL,
	BUILDING_REFINERY,
	BUILDING_LUMBERMILL,
	BUILDING_MINE,
	BUILDING_GIANTBUILDING,
	BUILDING_RESEARCH,
	BUILDING_WORKSHOP,

	Guild_World_Map_1				= 91500,
	Guild_World_Map_2,				
	Guild_World_Map_Blur_1,			
	Guild_World_Map_Blur_2,			
	
	Icon_Exploration_World_Dungeon_1= 92200,

	Exploration_World_Area_Arena	= 92300,
	Exploration_World_Area_Duel,
}

public enum ImageFillOrigin
{
	None,
	Left,
	Right,
	Bottom,
	Top,
}

public enum StageDifficulty
{
	Normal						= 0,
	Hard,
}

public enum GameResult
{
	None,
	Win,
	Lose,
	Draw,
}

public enum GameCharacterType
{
	None,
	MyCharacter,
	EnemyCharacter,
}

public enum MoveType
{
	GeneralMove					= 0,		// 사거리가 모자라면 이동하는 일반 이동
	Warp						= 1,		// 타겟까지 순간이동
	Rush						= 2,		// 돌진형
	Stop						= 3,		// 멈춤 (dt 와는 무관함)
}

public enum TouchSkillTimeType
{
	None,
	Charge,
	Reuse,
	Block,
}

public enum GameState			// 게임 상태
{
	None,
	GameLoad,
	GameFormEdit,
	Start,
	PlayBefore,					// Play 직전
	Play,
	EndWait,					// End 상태가 되기 전 종료 대기 상태
	End,
	Result,
	Clear,
}

public enum AnimState
{
	None,
	Idle,
	Run,
	Skill,
	Dead,
	Damage,
	Knockback,
	Stun,
	Win,
}

public enum NextActionType
{
	None,
	Turn,
	Skill,
}

public enum BoneType
{
	Master,
	Spine,
	Head,
	Max,
}

public enum GameStatType
{
	// 기본
	Damage = 1,					// 피해량
	Damaged,					// 받은 피해량
	Heal,						// 치유량
	Healed,						// 받은 치유량
	NormalSkillCount,			// 일반공격(치유) 횟수 (애니메이션이 플레이된 횟수)
	FatalSkillCount,			// 필살기 횟수 (애니메이션이 플레이된 횟수)
	TouchSkillCount,			// 터치스킬 횟수 (애니메이션이 플레이된 횟수)
	AutoSkillCount,				// 자동스킬 횟수 (애니메이션이 플레이된 횟수)
	Kill,						// 최종 킬 수 (소환된 캐릭터를 죽인 건 카운트하지 않는다)

	// 공격
	DPS = 10,					// 피해량 / 전투 시간으로 계산된 값
	DamageCount,				// 공격 횟수 : 피해 function이 target 별로 시도된 횟수
	HitCount,					// 적중 횟수 : 피해 function이 target 별로 적중된 횟수, 비중도 표시(적중 횟수 / 공격 횟수로 계산된 값)
	CriticalDamageCount,		// 치명타 횟수 : 피해 function이 target 별로 치명타로 적중된 횟수, 비중도 표시(치명타 횟수 / 공격 횟수로 계산된 값)
	IgnoreDefCount,				// 관통 횟수 : 피해 function이 target 별로 방어 관통을 일으킨 횟수, 비중도 표시(관통 횟수 / 공격 횟수로 계산된 값)
	PhysicalDamage,				// 물리 피해량, 비중도 표시
	MagicalDamage,				// 마법 피해량, 비중도 표시 (물리 피해량 + 마법 피해량 = 100%)
	NormalSkillDamage,			// 일반 피해 : 평타에 의한 피해량, 비중도 표시
	TouchSkillDamage,			// 터치 스킬 피해 : 터치 스킬에 의한 피해량, 비중도 표시
	AutoSkillDamage,			// 자동 스킬 피해 : 자동 스킬에 의한 피해량, 비중도 표시 (일반 피해 + 필살기 피해 + 터치 스킬 피해 + 자동 스킬 피해 = 100%)
	FatalSkillDamage = 20,		// 필살기 피해 : 필살기에 의한 피해량, 비중도 표시

	// 방어
	DefDamage,					// 방어 피해량 : 각각의 받은 피해를 방어율로 감소시킨 피해량의 총합, 비중도 표시((방어 피해량) / (받은 피해량 + 방어 피해량))
	DamagedCount,				// 받은 공격 횟수
	CriticalDamagedCount,		// 받은 치명타 횟수, 비중도 표시(받은 치명타 횟수 / 받은 공격 횟수)
	DodgeCount, 				// 회피 횟수 (면역은 카운트하지 않는다), 비중도 표시(회피 횟수 / 받은 공격 횟수)
	Aggro1Count,				// 위협 수준 역전 : 어그로를 역전시킨 횟수

	// 지원
	HPS,						// 치유량 / 전투 시간으로 계산된 값
	HealCount,					// 치유 횟수 : 치유 function이 target별로 들어간 횟수(생명력 흡수(흡혈)는 카운트하지 않는다)
	CriticalHealCount,			// 치유 치명타 횟수 : 치유 function이 target별로 치명타로 들어간 횟수, 비중도 표시(치유 치명타 횟수 / 치유 횟수로 계산된 값)
	NormalSkillHeal,			// 일반치유량 : 평타에 의한 치유량, 비중도 표시
	TouchSkillHeal = 30,		// 터치스킬 치유량 : 터치 스킬에 의한 치유량, 비중도 표시
	AutoSkillHeal,				// 자동스킬 치유량 : 자동 스킬에 의한 치유량, 비중도 표시 (일반치유량 + 필살기 치유량 + 터치스킬 치유량 + 자동스킬 치유량 = 100%)
	FatalSkillHeal,				// 필살기 치유량 : 필살기에 의한 치유량, 비중도 표시
	HealedCount,				// 받은 치유 횟수
	BuffCount,					// 버프 횟수
	DebuffCount,				// 디버프 횟수

	Max,
}

public enum TooltipType
{
	Skill,
	Item,
	Post,
	Info,
	Core,
	Hero,
	Giant,
	Tower,
	GuildBuilding,
	TargetCore,
	TargetItem,
	HeroBan,
	Max,
}

public enum TooltipPrefabType
{
	Info,
	Core,
	BaseExp,
	Max
}

public enum PlantSlotType
{
	Production,
	Storage,
}

public enum PlantSpriteType
{
	Production,
	Storage,
	Resource01,
	Resource02,
	Resource03,
}

public enum QuestPanelType
{
	Normal,
	Extend,
	Max,
}

public enum QuestSlotType
{
	Lock,
	Empty,
	Exist,
	Disable,
}

public enum HeroSelectMode
{
	Detail,
	Battle,
	Gather,
	PVP,
	Race,
	WorldBoss,
	GuildBattle,
	HeroEquipSwap,
	GameFormSetting,
	Duel,
}

public enum HeroSelectType
{
	Default,
	Elite,
	Supporter,
}

public enum HeroMainNotiType
{
	None,
	Main,
	Equip,
}

public enum HeroNotiType
{
	Equip,
	LevelUp,
	Skill,
}

public enum GiantTowerSelectMode
{
	Default,
	Edit,
}

public enum GiantTowerTabType
{
	Giant,
	Tower,
	Max,
}

public enum GameSettingTabType
{
	Account,
	Option,
	Notice,
	Help,
}

public enum ItemSlotState
{
	LowLevel,
	CanEquip,
	CanProduce,
	NoItem,
	Equipped,
	NoHero,
}

[System.Flags]
public enum UserDataFlag
{
	None				= 0,

	User				= 1 << 0,
	Area				= 1 << 1,
	Hero				= 1 << 2,
	Item				= 1 << 3,
	EventLog			= 1 << 4,
	MainQuestLog		= 1 << 5,
	SubQuest			= 1 << 6,
	SubQuestLog			= 1 << 7,
	StageLog			= 1 << 8,
	TreasureChest		= 1 << 9,
	PurchaseGoods		= 1 << 10,
	ClientData			= 1 << 11,
	ResearchSkill		= 1 << 12,
	AchievementLog		= 1 << 13,
	BaseStorage			= 1 << 14,
	SearchBuff			= 1 << 15,
	Core				= 1 << 16,
	Npc					= 1 << 17,
	Gather				= 1 << 18,
	RaceBattleChest		= 1 << 19,
	DungeonMission		= 1 << 20,
	DailyQuest			= 1 << 21,

	All					= int.MaxValue,
}

[System.Flags]
public enum BaseDataFlag
{
	None				= 0,

	Base				= 1 << 0,
	Hall				= 1 << 1,
	Plant				= 1 << 2,
	Giant				= 1 << 3,
	Tower				= 1 << 4,
	LootingLog			= 1 << 5,

	All					= int.MaxValue,
}

public enum SoundBGM
{
	None,
	Title,
	ExplorationNormal,
	ExplorationFever,
	BattleNormal,
	BattleBoss,
	BaseNormal,
	GuildBaseNormal,
}

public enum SoundEffect
{
	None = 0,

	ButtonA = 1,
	ButtonB,

	ExploreButton = 100,

	ShowEPCard = 1000,
	NewTarget,
	FindTarget,
	TargetClear,
	TargetMark,
	QuestMark,
	OpenEPCardGold,
	OpenEPCardCash,
	OpenEPCardEther,
	OpenEPCardItem,
	CollectGold,
	CollectCash,
	CollectEther,
	CollectItem,
	ThrowReward,
	ExplorationBaseBattle,
	ExplorationExtraEP,
	ExplorationFever,
	DungeonMoveFloor,
	UnlockContents,
	ExplorationTreasureBox,
	ExplorationWorldBoss,
	
	RefillGold = 2000,
	BaseLevelup,
	ShowMerchant,
	ItemSummon,
	ClearArea,

	HeroLevelup = 3000,
	HeroLevelupGauge,
	HeroSkillup,
	HeroEquipItem,
	HeroMakeItem,
	HeroRankup,
	HeroEvolution,
	HeroSummon,

	QuestAdd = 4000,
	QuestGiveUp,
	QuestComplete,

	HeroInForm = 5000,
	HeroOutForm,
	ResultGood,
	ResultBad,

	BattleWin = 6000,
	BattleLose,
	BattleWarning,
	BattleDanger,
}

public enum ChangeAnchorType 
{
	Left,
	Right,
	Top,
	Bottom,
	Center
}

public enum WsMessageType
{
	Ws_Open,
	Ws_Close,
	Ws_Error,
	Ws_Message,
}

public enum RewardItemAnimationType 
{
	None,
	Single,
	TreasureBox,
	Gotcha,
	Achievement,
}

public enum UpgradeStatus 
{
	NORMAL,
	UPGRADING,
	COMPLETE,
}
	
public enum InscriptionType 
{
	Lock,
	Empty,
	Active,
}

public enum FeverState
{
	Normal,
	Ready,
	Active,
}

public enum BaseNavigationButtonType
{
	Info = 1,
	Upgrade,
	Save,
	Defense,
	Laboratory,
	GiantTowerSetting,
	Craft,
	GuildDonation,
}

public enum SaveBtnState
{
	None,
	Empty,
	Full,
	Enable,
}

public enum GuildAuthority
{
	Master = 0,
	SubMaster,
	Elder,
	Normal,
}

public enum GuildJoinMehod
{
	Public,
	Approval,
	Private,
}
	
public enum CoreListItemSelectType
{
	NotSelect = 1,
	InfoSelect,
	BaseSelect,
	MaterialSelect,
}

public enum CoreListType
{
	Normal,
	Sale,
	EquipList,
	HeroEvolutionMaterial,
	CoreEnchantBase,
	CoreEnchantMaterial,
	CoreEvolutionBase,
	CoreEvolutionMaterial,
	CoreRandomEvolutionBase,
	CoreRandomEvolutionMaterial,
	CoreSynthesisBase,
	CoreSynthesisMaterial,
	CoreSmelt,
	CoreDecomposition,
}

public enum CoreListSortType
{
	Evo = 0,
	IDHero,
	IDCore,
}

public enum CoreListNotSelectReason
{
	None							= 0,
	EquipDisable					= 1 << 1,
	NormalEvolutionBaseDisable		= 1 << 2,
	NormalEvolutionMaterialDisable	= 1 << 3,
	RandomEvolutionDisable			= 1 << 4,
	EnchantBaseDisable				= 1 << 5,
	EnchantMaterialDisable			= 1 << 6,
	AlreadyMaxEnchant				= 1 << 7,
	AlreadyMaxEvo					= 1 << 8,
	EnoughEnchant					= 1 << 9,
	DiffEvo							= 1 << 10,
	HeroDiffChar					= 1 << 11,
	CoreDiffChar					= 1 << 12,
	Equiped							= 1 << 13,
	Locked							= 1 << 14,
	EnchantMaterialEvoHigh			= 1 << 15,
	EnchantDiffEvoOver				= 1 << 16,
	SynthesisEvoNotEnough			= 1 << 17,
	SmeltDisable					= 1 << 18,
	DecompositionDisable			= 1 << 19,
	SynthesisDisable				= 1 << 20,

}

public enum CoreFilterRowType
{
	Race	= 0,	
	Role 	= 8,	
	Evo		= 14,	
	Rank	= 20,	
	Lock	= 25,	
}

public enum CoreBatchRowType
{
	Evo,
	Rank,
	Enchant,
	EnchantPigEvo,
}

public enum EvolutionResultPopupType
{
	Hero,
	CoreNormal,
	CoreRandom,
	CoreRankUp,
	CoreSynthesis,
}

public enum TutorialListFocusType
{
	None,
	ID,
	Index,
}

public enum TutorialSmartFocusType
{
	None,
	EnableEvolutionHero,		// 진화 가능한 히어로
	LowLevelHero,				// 최저 레벨 히어로
	ExistHeroCore,				// 보유하고 있는 히어로와 같은 히어로 코어
}

public enum RewardResultPopupType
{
	GotchaBox,
	Post,
	RaceBox,
	Gather,
	SubQuest,
	Arena,
	WorkRoom,
	SelectSummon,
	GoalGoods,
	DailyGoods,
	Purchase,
	GuildShop,
	WorldBossBase,
	Duel,
	Decomposition,
}

public enum QuestObjectivePrefabType
{
	Stage = 0,
	Item,
	ETC,
	None,
}

public enum RewardItemPrefabType
{
	Default = 0,
	Core,
	Hero,
	BMMix,
	None,
}

public enum InfoTooltipType
{
	FeverGauge = 1,
	FindExtraExploration,
	HardModeNormalTargetGauge,
	HardModeExtraBossGauge,
	BaseExp,
	GoldInfo,
	CashInfo,
}

public enum InfoTooltipWidthType
{
	Large = 0,
	Small = 1,
}

public enum RectTransformWorldPivotType
{
	Max,
	Min,
	Center,
}
	
enum Select_Week_Type
{
	LastWeek = 0,
	ThisWeek,
}

public enum WorkRoomSlotState
{
	Empty,
	Waiting,
	Making,
	Complete,
	Lock,
}

public enum NPCSlotState
{
	Lock,
	Empty,
	Assign,
}

public enum AreaEnterItemType
{
	Area,
	Dungeon,
	Arena,
	Duel,
}
	
public enum PurchaseGoalState
{
	NotOpen,
	Open,
	ReadyToGet,
	Complete,
}

public enum DailyGoodsState
{
	NotPurchased,
	AvailableReward,
	DailyCoolTimeLimit,
	ExpiredReward,
}

public enum SelectSummonType
{
	Core,
	Hero,
	Item
}

public enum TextLimitType
{
	NickName,
	GuildName,
}

public enum BMMixSideIconType
{
	None = 0,
	Select,
	Random
}

public enum BMIconType
{
	HeroDecided = 1,
	HeroRandom,
	HeroSelect,
	CoreDecided,
	CoreRandeom,
	CoreSelect
}

public enum SeasonInfoType
{
	Arena = 0,
	Duel,
}

public enum PostType
{
	Bonus = 0,
	Purchase,
	HiveItem,
	None,
}

public enum NotiType
{
	Red,
	Purple,
	None
}

public enum PveCheckType
{
	None,
	Stat,
	SkillDamage,
	SkillHeal,
}

public enum FeverReadyType
{
	FeverStarMax = 1,
	FeverTarget,
	FeverEx,
}

public enum TimeLimitSaleType
{
	TimeLimitOnSale,
	TimeLimitOnClose,
	NoneTimeLimit,
}

public enum PvpRewardTabType
{
	RewardSeason,
	RewardGrade,
	RewardDaily,
}

public enum PvpMainTabType
{
	PvpRanking,
	PvpReward,
	PvpAttackLog,
	PvpDefenseLog,
	PvpDefenseDeck,
}
	
public enum QuestType
{
	SubQuest,
	DailyQuest,
	None,
}

public enum CommunityTabType
{
	CommunityNormal,
	CommunityGuild,
}

public enum DailyAttendanceItemState
{
	DailyAttendanceStateNormal,
	DailyAttendanceStateOpen,
	DailyAttendanceStateComplete,
}

public enum GuildBuildingStatus
{
	GuildBuildingNormal,
	GuildBuildingLackOfMoney,
	GuildBuildingLockLevel,
}

public enum WorldBossSubPageType
{
	Main,
	Ranking,
	Reward
}

public enum WorldBossRankingType
{
	Total,
	Best
}

public enum ChatMsgType
{
	HeroEvo,
	Normal,
}

public enum GiantSlotStatus
{
	Alive,
	Empty,
	Defeat,
	Lock,
}

public enum GuildBattleTabType
{
	Upgrade,
	Organize,
	BattleJoin,
}

public enum BaseResource
{
	DarkEther,
	Lumber,
	Cubic,
	Max,
}

public enum AutoExplorationStopReason
{
	SearchCount,
	UserAction,
	ServerError,
	NotEnoughCost,
	StopItem,
	StopCard,
	Tutorial,
}

public enum AutoExplorationFilterType
{
	StopItem,
	StopCard,
	PlayMonsterStage,
	PlayBattle,
	PlayBattleReward,
	PlayGather,
}

public enum AutoExplorationStopItemType
{
	Ether,
	Item,
	Core,
}

public enum AutoExplorationStopCardType
{
	None = -1,
	Target,
	EXBoss,
	GuildTarget,
	WorldBoss,
	NotPerfectClear,
	Pvp,
	BaseRaid,
	RaceBattle,
}

public enum AutoExplorationPlayMonsterStageType
{
	Grade1,
	Grade2,
	Grade3,
	Grade4,
}

public enum AutoExplorationPlayBattleType
{
	Target,
	Boss,
	NotPerfectClear,
	SubQuest,
	Pvp,
	BaseRaid,
	RaceBattle,
}

public enum AutoExplorationPlayRewardType
{
	Core,
	EquipSubItem,
	EvoItem,
}

public enum AutoExplorationPlayGatherType
{
	Herb,
	Vein,
	Tree,
}

public enum GameFormType
{
	Deck5,
	Deck10,
	Pvp,
	RaceBattle,
	Arena,
	Duel,
}

public enum GameFormBattleType
{
	None,
	Stage5,
	Stage10,
	BaseRaid,
	Pvp,
	WorldBoss,
	GuildBattle,
	RaceBattle,
	Arena,
	Duel,
}

public enum TargetUserDeckType
{
	Pvp,
	Arena,
	WorldBoss,
	Duel,
}

public enum AutoBattleType
{
	Off,
	Touch,
	All,
}

public enum CostumeStatus
{
	Buyable,
	Equiped,
	UnEquiped,
	SaleExpired,
	Unreceived,
	EmptyHeroUnEquiped,
	InvalidMarketInfo,
}

public enum TargetProgressStatus
{
	Clear,
	NotClear,
	CurrStage,
}

public enum DuelDailyRewardState
{
	NotReady,
	Ready,
	Received,
}
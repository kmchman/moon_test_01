//Auto generated. Do not edit

public class GeneratedType {
	public static System.Type[] types = new System.Type[] {
		typeof(ABNORMAL_STATE),
		typeof(PARENT_STAT),
		typeof(CHILD_STAT),
		typeof(INDEPENDENT_STAT),
		typeof(ETC_STAT),
		typeof(BUFF_TYPE),
		typeof(BUFF_TIME),
		typeof(STAT_TYPE),
		typeof(STAT_DISPLAY),
		typeof(ROLE),
		typeof(DAMAGE_TYPE),
		typeof(ITEM_TYPE),
		typeof(SKILL_TYPE),
		typeof(SKILL_USE),
		typeof(SKILL_OBJECTIVE),
		typeof(SKILL_TARGET),
		typeof(PROJECTILE),
		typeof(SKILL_CONDITION),
		typeof(SKILL_FUNCTION),
		typeof(SKILL_AUTO),
		typeof(EVENT_TRIGGER),
		typeof(QUEST_OBJECTIVE),
		typeof(SEARCH_TYPE),
		typeof(DAILY_QUEST_OBJECTIVE),
		typeof(CHARACTER_TYPE),
		typeof(EP_CATEGORY),
		typeof(EP_ADVENT_CONDITION),
		typeof(UNLOCK_CONTENTS),
		typeof(HERO_LIST_STATE),
		typeof(TOUCH_GUIDE_TYPE),
		typeof(ACHIEVEMENT_TYPE),
		typeof(BONUS_AGGRO),
		typeof(RESOURCE),
		typeof(GOLD_SALES_TYPE),
		typeof(RACE),
		typeof(RESEARCH_GROUP),
		typeof(CALCULATE_TYPE),
		typeof(CRAFT_TYPE),
		typeof(SEARCH_BUFF_TYPE),
		typeof(SB_ELIXIR_TYPE),
		typeof(SB_SCROLL_TYPE),
		typeof(ADD_SEARCH_RATE),
		typeof(CORE_RANK),
		typeof(CORE_TYPE),
		typeof(DIALOG_TYPE),
		typeof(SETTING_PUSH),
		typeof(BUILDING),
		typeof(BASE_UNLOCK),
		typeof(BUFF_EVENT_GROUP),
		typeof(BUFF_EVENT),
		typeof(PURCHASE_GROUP),
		typeof(PURCHASE_TAG),
		typeof(PURCHASE_PROVIDE_TYPE),
		typeof(PURCHASE_TYPE),
		typeof(PURCHASE_GOAL_CONDITION),
		typeof(WEBEVENT_TYPE),
		typeof(POPUP_CONDITION),
		typeof(POPUP_APPEAR_TYPE),
		typeof(POPUP_STYLE),
		typeof(DECK_TYPE),
		typeof(CONTENT_TYPE),
		typeof(GUILD_BUILDING),
		typeof(BUY_COUNT_INIT_TYPE),
		typeof(ARENADUEL_TYPE),
	};
}

public enum ABNORMAL_STATE { 
	NONE                         = 0,        // <상태이상><>
	UNABLE_SKILL                 = 1,        // <상태이상><스킬 금지>
	UNABLE_SPELL                 = 2,        // <상태이상><침묵>
	UNABLE_NONSPELL              = 3,        // <상태이상><비주문 금지>
	UNABLE_MOVE                  = 4,        // <상태이상><이동 금지>
	KNOCKBACK                    = 5,        // <상태이상><밀어내기>
	DEADLYHIT                    = 6,        // <상태이상><강타당함>
	BLANK                        = 7,        // <상태이상><없음>
	EXILED                       = 8,        // <상태이상><추방>
	STUN                         = 9,        // <상태이상><기절>
	SHIELD                       = 10,       // <상태이상><보호막>
	IMMUNE                       = 11,       // <상태이상><면역>
	IMMUNE_PHYSICAL              = 12,       // <상태이상><물리 면역>
	IMMUNE_MAGICAL               = 13,       // <상태이상><마법 면역>
	PROVOKED                     = 14,       // <상태이상><도발당함>
	PROVOKE                      = 15,       // <상태이상><집중공격>
	PULLED                       = 16,       // <상태이상><당겨오기>
	THORNS_PHYSICAL              = 17,       // <상태이상><물리 피해를 받으면 가해자에게 피해를 입힘>
	THORNS_MAGICAL               = 18,       // <상태이상><마법 피해를 받으면 가해자에게 피해를 입힘>
	ENDINDEX                     = 19,       
}

public enum PARENT_STAT { 
	STR                          = 1,        // <부모 스탯><힘(1=1), 실수로 표시>
	DEX                          = 2,        // <부모 스탯><민첩(1=1), 실수로 표시>
	INT                          = 3,        // <부모 스탯><지능(1=1), 실수로 표시>
	VIT                          = 4,        // <부모 스탯><활력(1=1), 실수로 표시>
	ENDINDEX                     = 5,        
}

public enum CHILD_STAT { 
	MAX_HP                       = 1,        // <자식 스탯><최대 체력(1=1), 실수로 표시>
	PHYSICAL                     = 2,        // <자식 스탯><물리 공격력(1=1), 실수로 표시>
	PHYSICAL_DEF                 = 3,        // <자식 스탯><물리 방어력(1=1), 실수로 표시>
	PHYSICAL_CRITICAL_CHANCE     = 4,        // <자식 스탯><물리 치명타 확률(1=0.1%), %로 표시>
	MAGICAL                      = 5,        // <자식 스탯><마법 공격력(1=1), 실수로 표시>
	MAGICAL_DEF                  = 6,        // <자식 스탯><마법 방어력(1=1), 실수로 표시>
	MAGICAL_CRITICAL_CHANCE      = 7,        // <자식 스탯><마법 치명타 확률(1=0.1%), %로 표시>
	ENDINDEX                     = 8,        
}

public enum INDEPENDENT_STAT { 
	ANIM_SPEED                   = 1,        // <독립 스탯><공격 속도(1=0.1%), +%로 표시>
	MOVE_SPEED                   = 2,        // <독립 스탯><이동 속도(1=0.1%), +%로 표시>
	COOLTIME                     = 3,        // <독립 스탯><모든 회복 속도(1=0.1%), +%로 표시>
	PHYSICAL_CRITICAL_DAMAGE     = 4,        // <독립 스탯><물리 치명타 피해(1=0.1%), +%로 표시>
	MAGICAL_CRITICAL_DAMAGE      = 5,        // <독립 스탯><마법 치명타 피해(1=0.1%), +%로 표시>
	HIT_RATE                     = 6,        // <독립 스탯><적중(1=0.1%), +%로 표시>
	DODGE                        = 7,        // <독립 스탯><회피(1=0.1%), %로 표시>
	IGNORE_PHYSICAL_DEF          = 8,        // <독립 스탯><물리 관통 확률(1=0.1%, 타겟의 방어력을 0으로 만들 확률), %로 표시>
	IGNORE_MAGICAL_DEF           = 9,        // <독립 스탯><마법 관통 확률(1=0.1%, 타겟의 방어력을 0으로 만들 확률), %로 표시>
	VAMPIRIC                     = 10,       // <독립 스탯><생명력 흡수(1=0.1%), %로 표시>
	DEF_MASTERY                  = 11,       // <독립 스탯><방어 숙련도(1=0.1%, 치명타 기회에서 -), +%로 표시>
	HEALED_AMPLIFY               = 12,       // <독립 스탯><받는 치유량(1=0.1%), +%로 표시>
	DAMAGED_AMPLIFY              = 13,       // <독립 스탯><받는 모든 피해(1=0.1%), +%로 표시>
	PHYSICAL_DAMAGED_AMPLIFY     = 14,       // <독립 스탯><받는 물리 피해(1=0.1%), +%로 표시>
	MAGICAL_DAMAGED_AMPLIFY      = 15,       // <독립 스탯><받는 마법 피해(1=0.1%), +%로 표시>
	DAMAGE_AMPLIFY               = 16,       // <독립 스탯><입히는 모든 피해(1=0.1%), +%로 표시>
	PHYSICAL_DAMAGE_AMPLIFY      = 17,       // <독립 스탯><입히는 물리 피해(1=0.1%), +%로 표시>
	MAGICAL_DAMAGE_AMPLIFY       = 18,       // <독립 스탯><입히는 마법 피해(1=0.1%), +%로 표시>
	EVO_DAMAGED                  = 19,       // <독립 스탯><진화 피해 저항(1=0.1%), +%로 표시>
	EVO_DAMAGE                   = 20,       // <독립 스탯><진화 피해 증가(1=0.1%), +%로 표시>
	EVO_HEAL                     = 21,       // <독립 스탯><진화 치유량 증가(1=0.1%), +%로 표시>
	NORMAL_COOLTIME              = 22,       // <독립 스탯><일반 공격 회복 속도(1=0.1%), +%로 표시>
	AUTO_COOLTIME                = 23,       // <독립 스탯><자동 스킬 회복 속도(1=0.1%), +%로 표시>
	TOUCH_COOLTIME               = 24,       // <독립 스탯><터치 스킬 회복 속도(1=0.1%), +%로 표시>
	FATAL_CHARGE                 = 25,       // <독립 스탯><필살기 회복 속도(1=0.1%), +%로 표시>
	GB_MAX_HP_MULTIPLIER         = 26,       // <독립 스탯><길드전 체력 증가 배수,기본 값은 1, 길드전에서만 사용, n배로 표시>
	ENDINDEX                     = 27,       
}

public enum ETC_STAT { 
	CUR_HP                       = 1,        // <기타 스탯><체력>
	CUR_HP_RATE                  = 2,        // <기타 스탯><체력을 비율로>
	ENDINDEX                     = 3,        
}

public enum BUFF_TYPE { 
	ABNORMAL_STATE               = 1,        // <버프 구성 요소><상태 이상>
	PARENT_STAT                  = 2,        // <버프 구성 요소><부모 스탯>
	CHILD_STAT                   = 3,        // <버프 구성 요소><자식 스탯>
	INDEPENDENT_STAT             = 4,        // <버프 구성 요소><독립 스탯>
	ETC_STAT                     = 5,        // <버프 구성 요소><기타 스탯>
	ENDINDEX                     = 6,        
}

public enum BUFF_TIME { 
	CONT                         = 1,        // <버프 적용 타이밍 방식><지속시간동안 계속>
	TICK                         = 2,        // <버프 적용 타이밍 방식><지속시간동안 틱마다>
	ONCE                         = 3,        // <버프 적용 타이밍 방식><지속시간동안 1번만>
	ENDINDEX                     = 4,        
}

public enum STAT_TYPE { 
	PARENT_STAT                  = 1,        // <스탯><부모 스탯>
	CHILD_STAT                   = 2,        // <스탯><자식 스탯>
	INDEPENDENT_STAT             = 3,        // <스탯><독립 스탯>
	ETC_STAT                     = 4,        // <스탯><기타 스탯>
	ENDINDEX                     = 5,        
}

public enum STAT_DISPLAY { 
	RATE                         = 1,        // <서치 버프 스탯 표시><*100>
	TRATE                        = 2,        // <서치 버프 스탯 표시></10>
	REAL                         = 3,        // <서치 버프 스탯 표시><실수로 표시>
	ENDINDEX                     = 4,        
}

public enum ROLE { 
	NONE                         = 0,        // <역할><없음>
	TANK                         = 1,        // <역할><탱커>
	MELEE_DEAL                   = 2,        // <역할><물리 근접 딜러>
	RANGE_DEAL                   = 3,        // <역할><물리 원거리 딜러>
	MAGIC_DEAL                   = 4,        // <역할><마법 딜러>
	HEAL                         = 5,        // <역할><힐러>
	GIANT                        = 6,        // <역할><자이언트>
	TOWER                        = 7,        // <역할><타워>
	NPC                          = 8,        // <역할><NPC>
	ENDINDEX                     = 9,        
}

public enum DAMAGE_TYPE { 
	NONE_DAMAGE                  = 0,        // <데미지 타입><무속성>
	PHYSICAL_DAMAGE              = 1,        // <데미지 타입><물리 데미지>
	MAGICAL_DAMAGE               = 2,        // <데미지 타입><마법 데미지>
	ENDINDEX                     = 3,        
}

public enum ITEM_TYPE { 
	NONE                         = 0,        // <아이템 타입><없음>
	MASTER_EXP                   = 1,        // <아이템 타입><사용자 경험치>
	FEVER_STAR                   = 2,        // <아이템 타입><피버스타 (Val0:수량, Val1:지역)>
	GOLD                         = 3,        // <아이템 타입><골드>
	CASH                         = 4,        // <아이템 타입><캐시>
	ETHER                        = 5,        // <아이템 타입><에테르>
	DARKETHER                    = 6,        // <아이템 타입><다크에테르>
	LUMBER                       = 7,        // <아이템 타입><목재>
	CUBIC                        = 8,        // <아이템 타입><큐빅>
	GIANT_SUMMON                 = 9,        // <아이템 타입><자이언트>
	HERO_SUMMON                  = 10,       // <아이템 타입><히어로 소환>
	CORE_SUMMON                  = 11,       // <아이템 타입><코어 소환>
	EQUIPMENT                    = 12,       // <아이템 타입><장비>
	MATERIAL                     = 13,       // <아이템 타입><소재 (머터리얼)>
	INSCRIPTION_SCROLL           = 14,       // <아이템 타입><각인 인챈트 주문서>
	ELIXIR                       = 15,       // <아이템 타입><엘릭서 (탐사버프 아이템)>
	SCROLL                       = 16,       // <아이템 타입><스크롤 (탐사버프 아이템)>
	QUEST                        = 17,       // <아이템 타입><퀘스트 아이템>
	ETHEREUM                     = 18,       // <아이템 타입><에테륨>
	DUNGEON_TOKEN                = 19,       // <아이템 타입><던전토큰 (남은 탐사 횟수)>
	ARENA_GAUGE                  = 20,       // <아이템 타입><아레나 보상 게이지>
	ARENA_POINT                  = 21,       // <아이템 타입><아레나 점수>
	HERO_SELECT                  = 22,       // <아이템 타입><히어로 선택>
	CORE_SELECT                  = 23,       // <아이템 타입><코어 선택>
	ITEM_SUMMON                  = 24,       // <아이템 타입><아이템 소환>
	ITEM_SELECT                  = 25,       // <아이템 타입><아이템 선택>
	ARENA_CURRENCY               = 26,       // <아이템 타입><아레나 명예점수>
	GOLD_FREE                    = 27,       // <아이템 타입><무료 골드>
	GOLD_PAID                    = 28,       // <아이템 타입><유료 골드>
	CASH_FREE                    = 29,       // <아이템 타입><무료 캐시>
	CASH_PAID                    = 30,       // <아이템 타입><유료 캐시>
	CORESTAT_CHANGE              = 31,       // <아이템 타입><코어옵션 변경티켓>
	WORLDBOSS_TOKEN              = 32,       // <아이템 타입><월드보스 토큰(월드보스와 전투시 소모)>
	TREASURE_CHEST               = 33,       // <아이템 타입><상자아이템>
	GUILD_POINT                  = 34,       // <아이템 타입><길드포인트>
	GUILD_COIN                   = 35,       // <아이템 타입><길드코인>
	EVOELEMENT_MATERIAL          = 36,       // <아이템 타입><진화 원소>
	EQUIP_MATERIAL               = 37,       // <아이템 타입><장비 조각>
	EVOELEMENT_SUMMON            = 38,       // <아이템 타입><진화 원소 소환>
	EVOELEMENT_SUMMON2           = 39,       // <아이템 타입><특정 진화 원소 소환>
	COSTUME                      = 40,       // <아이템 타입><코스튬>
	ENDINDEX                     = 41,       // <아이템 타입><>
}

public enum SKILL_TYPE { 
	NORMAL_SKILL                 = 1,        // <스킬 타입><일반 스킬>
	FATAL_SKILL                  = 2,        // <스킬 타입><필살기>
	TOUCH_SKILL                  = 3,        // <스킬 타입><터치 스킬>
	ACTIVE_SKILL1                = 4,        // <스킬 타입><액티브1>
	ACTIVE_SKILL2                = 5,        // <스킬 타입><액티브2>
	ACTIVE_SKILL3                = 6,        // <스킬 타입><액티브3>
	ACTIVE_SKILL4                = 7,        // <스킬 타입><액티브4>
	PASSIVE_SKILL1               = 8,        // <스킬 타입><패시브1>
	PASSIVE_SKILL2               = 9,        // <스킬 타입><패시브2>
	PASSIVE_SKILL3               = 10,       // <스킬 타입><패시브3>
	ENDINDEX                     = 11,       
}

public enum SKILL_USE { 
	NONE                         = 0,        // <스킬 사용 형태><없음>
	FATAL                        = 1,        // <스킬 사용 형태><필살기>
	TOUCH                        = 2,        // <스킬 사용 형태><터치>
	COOL                         = 3,        // <스킬 사용 형태><쿨타임이 지나면 사용되는 액티브>
	AURA                         = 4,        // <스킬 사용 형태><생존해 있는 동안 오라 형태로 버프 호출>
	CONDITION                    = 5,        // <스킬 사용 형태><조건 발동>
	ENDINDEX                     = 6,        
}

public enum SKILL_OBJECTIVE { 
	SELF                         = 1,        // <스킬 목표><나 자신>
	ALLY_MAXLOSSHP               = 2,        // <스킬 목표><아군 중에 잃은 HP가 가장 많은>
	ALLY_MINLOSSHP               = 3,        // <스킬 목표><아군 중에 잃은 HP가 가장 적은>
	ALLY_CLOSEST                 = 4,        // <스킬 목표><아군 중에 가장 가까운>
	ALLY_FARTHEST                = 5,        // <스킬 목표><아군 중에 가장 먼>
	ALLY_RANDOM                  = 6,        // <스킬 목표><아군 중에 아무나>
	ALLY_RANDOMEXCEPTME          = 7,        // <스킬 목표><아군 중에 나를 제외하고 아무나>
	ALLY_MAXAGGRO                = 8,        // <스킬 목표><내가 가장 높은 어그로를 느끼는 적이 가장 큰 어그로를 느끼는 아군>
	ALLY_SMARTHEAL               = 9,        // <스킬 목표><힐 할만한 적절한 대상>
	ENEMY_MAXAGGRO               = 10,       // <스킬 목표><적 중에 가장 높은 어그로를 느끼는>
	ENEMY_MINHP                  = 11,       // <스킬 목표><적 중에 hp가 가장 적은>
	ENEMY_MAXHP                  = 12,       // <스킬 목표><적 중에 hp가 가장 많은>
	ENEMY_CLOSEST                = 13,       // <스킬 목표><적 중에 가장 가까운>
	ENEMY_FARTHEST               = 14,       // <스킬 목표><적 중에 가장 먼>
	ENEMY_RANDOM                 = 15,       // <스킬 목표><적 중에 아무나>
	ENEMY_SMARTDAMAGE            = 16,       // <스킬 목표><점사할 만한 적당한 대상>
	ENEMY_SMARTPROVOKED          = 17,       // <스킬 목표><위험한 적>
	ALLY_MAXDEBUFF               = 18,       // <스킬 목표><디버프가 가장 많은 아군>
	ENEMY_MAXBUFF                = 19,       // <스킬 목표><버프가 가장 많은 적>
	ENDINDEX                     = 20,       
}

public enum SKILL_TARGET { 
	OBJ_SINGLE                   = 1,        // <스킬 타겟><OBJ 단일>
	OBJ_CIRCLE_ALLY              = 2,        // <스킬 타겟><OBJ를 중심으로 한 원 안의 아군>
	OBJ_CIRCLE_ENEMY             = 3,        // <스킬 타겟><OBJ를 중심으로 한 원 안의 적>
	OBJ_LINE_ALLY                = 4,        // <스킬 타겟><시전자에서 시작하여 OBJ를 관통하는 반직선 상의 아군>
	OBJ_LINE_ENEMY               = 5,        // <스킬 타겟><시전자에서 시작하여 OBJ를 관통하는 반직선 상의 적>
	ALLY_ALL                     = 6,        // <스킬 타겟><아군 전체>
	SELF                         = 7,        // <스킬 타겟><나>
	ENEMY_ALL                    = 8,        // <스킬 타겟><적 전체>
	ALLY_SUMMON_ALL              = 9,        // <스킬 타겟><아군의 모든 소환수>
	ENEMY_SUMMON_ALL             = 10,       // <스킬 타겟><적의 모든 소환수>
	ALLY_DEAD_ALL                = 11,       // <스킬 타겟><모든 죽은 아군>
	ENEMY_DEAD_ALL               = 12,       // <스킬 타겟><모든 죽은 적>
	ENDINDEX                     = 13,       
}

public enum PROJECTILE { 
	NONE                         = 0,        // <스킬 투사체 및 궤적><없음>
	STRAIGHT                     = 1,        // <스킬 투사체 및 궤적><직선>
	PARABOLA                     = 2,        // <스킬 투사체 및 궤적><커브>
	PARABOLA_FOOL                = 3,        // <스킬 투사체 및 궤적><3과 궤적은 같지만 투사체가 회전하지 않음>
	ENDINDEX                     = 4,        
}

public enum SKILL_CONDITION { 
	DAMAGED                      = 1,        // <스킬 발동 조건><맞았을 때>
	DODGE                        = 2,        // <스킬 발동 조건><공격을 회피했을 때>
	ENEMY_DEAD                   = 3,        // <스킬 발동 조건><적이 죽었을 때(내가 안 죽여도 됨)>
	ALLY_DEAD                    = 4,        // <스킬 발동 조건><우리 편이 죽었을 때>
	CRITICAL_DAMAGED             = 5,        // <스킬 발동 조건><치명타를 맞았을 때>
	ATTACK                       = 6,        // <스킬 발동 조건><공격 성공 했을 때>
	CRITICAL_ATTACK              = 7,        // <스킬 발동 조건><치명타를 때렸을 때>
	DIE                          = 8,        // <스킬 발동 조건><죽을 때>
	ENDINDEX                     = 9,        
}

public enum SKILL_FUNCTION { 
	DAMAGE                       = 1,        // <스킬 펑션><공격>
	HEAL                         = 2,        // <스킬 펑션><치유>
	DEBUFF                       = 3,        // <스킬 펑션><디버프>
	BUFF                         = 4,        // <스킬 펑션><버프>
	DAMAGE_RATE                  = 5,        // <스킬 펑션><적이 잃은 체력 비례한 공격>
	SUMMON                       = 6,        // <스킬 펑션><소환물 소환>
	DISPEL_DEBUFF                = 7,        // <스킬 펑션><디버프 해제>
	DISPEL_BUFF                  = 8,        // <스킬 펑션><버프 해제>
	DAMAGE_CHAIN                 = 9,        // <스킬 펑션><연쇄 데미지>
	HEAL_CHAIN                   = 10,       // <스킬 펑션><연쇄 치유>
	SUMMON_KILL                  = 11,       // <스킬 펑션><소환수 제거>
	DAMAGE_REPEAT                = 12,       // <스킬 펑션><반복 데미지>
	HEAL_REPEAT                  = 13,       // <스킬 펑션><반복 치유>
	RESURRECT                    = 14,       // <스킬 펑션><부활>
	DAMAGE_TOGETHER              = 15,       // <스킬 펑션><스킬에 맞은 캐릭터의 수에 따라 데미지를 나눠 입힘>
	ENDINDEX                     = 16,       
}

public enum SKILL_AUTO { 
	NORMAL                       = 1,        // <터치 스킬 자동 사용 조건><일반>
	PROVOKE                      = 2,        // <터치 스킬 자동 사용 조건><적 중에 우리 편을 타겟하는 적이 하나라도 있을 때>
	HEAL                         = 3,        // <터치 스킬 자동 사용 조건><우리 편 중에 체력을 많이 상실한 영웅이 있을 때>
	RESURRECT                    = 4,        // <터치 스킬 자동 사용 조건><우리 편 중에 하나라도 죽은 영웅이 있을 때>
	ENDINDEX                     = 5,        
}

public enum EVENT_TRIGGER { 
	INITIAL                      = 1,        // <이벤트 트리거><처음부터 있는 이벤트>
	CLEAR_MAINQUEST              = 2,        // <이벤트 트리거><[v1] ID의 메인퀘스트 클리어>
	CLEAR_SUBQUEST               = 3,        // <이벤트 트리거><[v1] ID의 서브퀘스트 클리어>
	MASTER_LV                    = 4,        // <이벤트 트리거><사용자레벨 [v1]이상 [v2]이하>
	UNLOCK_SEARCH_ID             = 5,        // <이벤트 트리거><[v1] SearchID(특정난이도 지역) 언락>
	HERO_EVO                     = 6,        // <이벤트 트리거><[v1] ID의 히어로 진화등급이 [v2]이상이고 다음 단계로 진화할 수 있는 소환석을 충분히 보유하고 있으면>
	CLEAR_EVENT                  = 7,        // <이벤트 트리거><[v1] ID의 이벤트 완료>
	CURRENT_SEARCH_ID            = 8,        // <이벤트 트리거><현재 위치가 [v1] Search ID>
	OWN_ITEM                     = 9,        // <이벤트 트리거><[v1] ID의 아이템 [v2]개 이상을 인벤에 보유>
	HERO_NUMBER                  = 10,       // <이벤트 트리거><히어로 보유수 [v1]인 이상>
	ANY_HERO_REACHED_RANK        = 11,       // <이벤트 트리거><[v1] 랭크이상의 히어로 존재>
	ANY_HERO_REACHED_LV          = 12,       // <이벤트 트리거><[v1] 레벨이상의 히어로 존재>
	HERO_SUMMONED                = 13,       // <이벤트 트리거><[v1] ID의 히어로 보유>
	HERO_LV                      = 14,       // <이벤트 트리거><[v1] ID의 히어로 레벨이 [v2]이상>
	HERO_RANK                    = 15,       // <이벤트 트리거><[v1] ID의 히어로 랭크가 [v2]이상>
	SEARCH_COUNT                 = 16,       // <이벤트 트리거><[v1] SearchID(특정난이도 지역)의 누적 탐사 횟수 [v2]회 이상>
	EP_FIRST_FIND                = 17,       // <이벤트 트리거><[v1] ID인 EP종류(Category+Grade) 발견>
	CLEAR_ALL_EP                 = 18,       // <이벤트 트리거><현재 탐사중인 곳의 모든 EP 클리어>
	CLEAR_STAGE                  = 19,       // <이벤트 트리거><[v1] ID의 스테이지를 클리어>
	ANY_HERO_EQUIPED_6ITEMS      = 20,       // <이벤트 트리거><아이템 6종을 장착한 히어로 존재>
	ACTIVATE_SKILL               = 21,       // <이벤트 트리거><전투 중 히어로의 스킬이 활성화되면>
	ACTIVATE_FATAL               = 22,       // <이벤트 트리거><전투 중 히어로의 필살기가 활성화되면>
	ACTIVATE_BOSS_SKILL_WARNING  = 23,       // <이벤트 트리거><전투 중 보스 스킬 경보가 나타나면>
	STAGE_FIND                   = 24,       // <이벤트 트리거><탐사결과에 [v1] ID인 스테이지 존재>
	TREASURE_BOX_FIND            = 25,       // <이벤트 트리거><탐사결과에 [v1] Grade인 보물상자 존재>
	START_BOSS_BATTLE            = 26,       // <이벤트 트리거><보스 스테이지 시작 (투입 안내용)>
	ITEM_EQUIP                   = 27,       // <이벤트 트리거><[v1]히어로가 [v2]번 아이템슬롯(1~6)에 아이템을 장착한 상태면>
	ITEM_UNEQUIP                 = 28,       // <이벤트 트리거><[v1]히어로가 [v2]번 아이템슬롯에 아이템을 장착하지 않은 상태면>
	JOIN_FORM_EDIT               = 29,       // <이벤트 트리거><편성화면에 진입했으면>
	JOIN_ASSIGN_HERO_SELECT      = 30,       // <이벤트 트리거><편성화면에 진입했으면>
	START_SUBQUEST               = 31,       // <이벤트 트리거><[v1] ID의 서브퀘스트가 시작됨 (진행중 상태가 됨)>
	IS_SEARCH_MAP                = 32,       // <이벤트 트리거><현재 탐사화면이면 & 팝업이 없으면>
	IS_BASE_MAP                  = 33,       // <이벤트 트리거><현재 베이스화면이면 & 팝업이 없으면>
	GET_SOUL                     = 34,       // <이벤트 트리거><뽑기 결과 소울획득이 발생하면>
	FIND_QUEST_MARK              = 35,       // <이벤트 트리거><탐사에서 느낌표가 나오면>
	ANY_SUBQUEST_CLEAR           = 36,       // <이벤트 트리거><서브퀘스트를 아무거나 완료했으면>
	QUEST_SLOT_BUY_READY         = 37,       // <이벤트 트리거><구매 가능한 퀘스트 슬롯이 존재하면>
	CORE_MAX_ENCHANT             = 38,       // <이벤트 트리거><아무 코어나 +5까지 강화하면>
	ANY_HERO_EVO_UP              = 39,       // <이벤트 트리거><아무 히어로나 진화를 수행하면>
	GET_CORE_IN_BATTLE           = 40,       // <이벤트 트리거><전투에서 코어를 획득했으면>
	ANY_CORE_EQUIP               = 41,       // <이벤트 트리거><아무 히어로나 코어를 장착하면>
	CHECK_NOTHING_EVO_POPUP      = 42,       // <이벤트 트리거><진화 팝업이 닫혔으면>
	DEFEAT_BATTLE                = 43,       // <이벤트 트리거><전투에서 패배하면>
	LOW_GOLD                     = 44,       // <이벤트 트리거><보유 골드가 리필 가능액수면>
	ANY_EXEP_FIND                = 45,       // <이벤트 트리거><아무 엑스트라 EP든지 발견하면>
	FEVER_GAUGE_MAX              = 46,       // <이벤트 트리거><피버게이지 MAX가 되면>
	CHECK_FEVER                  = 47,       // <이벤트 트리거><피버 상태를 체크 [v1]이 0-피버없음, 1-피버레디, 2-피버발동>
	SET_FEVER_TARGET             = 48,       // <이벤트 트리거><새 피버타겟 부여>
	SET_FEVER_TARGET_NO_CLEAR    = 49,       // <이벤트 트리거><[보류] 이전 피버타겟을 클리어하지 못한 상태로 새 피버타겟 부여 (유효시간 경과 케이스)>
	FEVER_TARGET_CLEAR           = 50,       // <이벤트 트리거><피버타겟 클리어>
	FEVER_TARGET_FIND            = 51,       // <이벤트 트리거><피버타겟에 해당하는 EP 출현>
	ANY_SEARCH_BUFF_ACTIVATE     = 52,       // <이벤트 트리거><아무거나 탐사버프 활성화>
	SEARCH_BUFF_FINISH           = 53,       // <이벤트 트리거><탐사버프중 아무거나 종료>
	NPC_FIND                     = 54,       // <이벤트 트리거><ID가 [v1]인 NPC 출현>
	NPC_CONTACT                  = 55,       // <이벤트 트리거><ID가 [v1]인 NPC 처음 만남 (EP카드터치)>
	GATHERING                    = 56,       // <이벤트 트리거><채집 파견중이 1건 이상 존재하면>
	GATHER_FINISH                = 57,       // <이벤트 트리거><아무 채집이나 완료시키면>
	OPEN_PLANT_UI                = 58,       // <이벤트 트리거><[v1] ID 플랜트의 유니트(내부건물) 업그레이드 UI가 나타나면>
	OPEN_LAB_UI                  = 59,       // <이벤트 트리거><OPEN_LAB_UI 연구소의 연구목록 UI가 나타나면>
	OPEN_WORKSHOP_UI             = 60,       // <이벤트 트리거><공방의 제작 UI가 나타나면>
	ANY_PLANT_FINISH_READY       = 61,       // <이벤트 트리거><아무거나 플랜트 유니트(내부건물) 중 하나가 완료대기>
	PLANT_PRODUCTION_LV          = 62,       // <이벤트 트리거><[v1] ID 플랜트의 생산소 첫 슬롯이 [v2] Lv 달성>
	PLANT_STORAGE_LV             = 63,       // <이벤트 트리거><[v1] ID 플랜트의 저장소 첫 슬롯이 [v2] Lv 달성>
	ANY_PLANT_START_BUILD        = 64,       // <이벤트 트리거><아무거나 플랜트 유니트 중 하나를 [v1] Lv로 업그레이드 시작하면>
	ANY_PLANT_LV                 = 65,       // <이벤트 트리거><아무 플랜트 유니트 중 하나의 최고레벨이 [v1] Lv 달성>
	BUILD_PLANT_FULLSET          = 66,       // <이벤트 트리거><플랜트 3종의 생산소, 저장소 총 6곳이 각 1개 이상 (총 6개 이상) 완성>
	ANY_RESEARCH_FINISH          = 67,       // <이벤트 트리거><연구소의 연구가 아무거나 완료되면>
	BASE_BE_ATTACKED             = 68,       // <이벤트 트리거><베이스가 공격당했으면>
	BUY_COUNT_GOTCHA             = 69,       // <이벤트 트리거><[v1] ID의 가차박스를 [v2]회이상 구입했으면>
	UPGRADE_HALL                 = 70,       // <이벤트 트리거><타운홀이 [v1] 레벨이 되었으면>
	GET_TOWER                    = 71,       // <이벤트 트리거><타워를 [v1]개 이상 보유했으면>
	SET_GIANT                    = 72,       // <이벤트 트리거><Type Number가 [v1]인 플랜트(2~4)에 방어 자이언트가 셋팅됐으면>
	CHECK_EVO_TUTORIAL2          = 73,       // <이벤트 트리거><[v1] ID의 히어로가 2성상태고 해당 히어로의 1성 1랭 코어가 2개 이상 & [v2] 캐릭터ID의 1성 2랭 코어가 20개 이상 보유중이면>
	ENDINDEX                     = 74,       
}

public enum QUEST_OBJECTIVE { 
	KILL_MONSTER                 = 1,        // <퀘스트 목표><[Target] ID의 몬스터 [Num] 마리 처치>
	OWN_ITEM                     = 2,        // <퀘스트 목표><[Target] ID의 아이템 [Num]개 인벤에 보유>
	HERO_NUMBER                  = 3,        // <퀘스트 목표><모든 영웅 보유수 [Num]이상>
	SUMMON_HERO                  = 4,        // <퀘스트 목표><[Target] ID의 히어로 보유(소환완료)>
	HERO_LV                      = 5,        // <퀘스트 목표><[Target] ID의 히어로 [Num]레벨 이상 달성>
	HERO_RANK                    = 6,        // <퀘스트 목표><[Target] ID의 히어로 [Num]랭크 이상 달성>
	PROMOTE_ITEM                 = 7,        // <퀘스트 목표><[Target] ID의 아이템을 합성으로 [Num]회 제작>
	SEARCH_COUNT                 = 8,        // <퀘스트 목표><[Target] SearchID에서 탐사 [Num]회 수행 (퀘스트 활성화 이후부터 카운트)>
	GET_TREASURE                 = 9,        // <퀘스트 목표><등급무관 보물상자 획득 [Num]회 이상>
	CLEAR_STAGE                  = 10,       // <퀘스트 목표><[Target] ID 스테이지를 [Num]회 클리어 (실 사용시 Num은 항상 1회로만 설정)>
	PVP_BASE_BATTLE              = 11,       // <퀘스트 목표><베이스 공략전 (침략) [Num]회 성공하기 (퀘스트 활성화 이후부터 카운트)>
	PVP_HERO_BATTLE              = 12,       // <퀘스트 목표><히어로 pvp [Num]회 승리하기 (퀘스트 활성화 이후부터 카운트)>
	EXTRACT_ITEM                 = 13,       // <퀘스트 목표><[Target] ID 광물 [Num]회 분해 완료 (퀘스트 활성화 이후부터 카운트)>
	GATHER_ITEM                  = 14,       // <퀘스트 목표><파견 결과로 [Target] ID 아이템을 [Num]개 획득 (퀘스트 활성화 이후부터 카운트)>
	MIX_ITEM                     = 15,       // <퀘스트 목표><파견 결과로 [Target] ID 아이템을 [Num]개 획득 (퀘스트 활성화 이후부터 카운트)>
	INSCRIPTION                  = 16,       // <퀘스트 목표><[Target] 히어로에 각인 [Num]개 이상 완료>
	CLEAR_EXTRABOSS              = 17,       // <퀘스트 목표><EX보스 전투에서 [Num]회 승리 (퀘스트 활성화 이후부터 카운트)>
	ENDINDEX                     = 18,       
}

public enum SEARCH_TYPE { 
	NORMAL                       = 1,        // <데일리 퀘스트의 탐사타입><보통 난이도 지역 탐사>
	HARD                         = 2,        // <데일리 퀘스트의 탐사타입><어려움 난이도 지역 탐사>
	ALL_AREA                     = 3,        // <데일리 퀘스트의 탐사타입><난이도 무관 지역 탐사 (보통+어려움)>
	DUNGEON                      = 4,        // <데일리 퀘스트의 탐사타입><던전 탐사>
	ALL                          = 5,        // <데일리 퀘스트의 탐사타입><모든 탐사>
	ENDINDEX                     = 6,        
}

public enum DAILY_QUEST_OBJECTIVE { 
	COUNT_SEARCH                 = 1,        // <데일리 퀘스트><노멀/하드/던전에서 탐사 [num]회>
	COUNT_MONSTER_KILL           = 2,        // <데일리 퀘스트><노멀/하드/던전 에서 [target]등급(1~4) 몬스터 스테이지 [num]회 클리어>
	COUNT_BOSS_KILL              = 3,        // <데일리 퀘스트><노멀/하드에서 보스 스테이지 [num]회 클리어>
	COUNT_FEVER_STAR             = 4,        // <데일리 퀘스트><노멀/하드에서 재탐사로 피버스타 [num]개 획득>
	COUNT_TREASURE_BOX           = 5,        // <데일리 퀘스트><노멀/하드/던전에서 등급무관 보물상자 [num]회 획득>
	COUNT_GATHER                 = 6,        // <데일리 퀘스트><[target] EP의 채집물을 (11:약초, 12:광맥, 13:나무) [num]회 채집 완료>
	COUNT_GOLD_NPC_SHOP          = 7,        // <데일리 퀘스트><NPC상점에서 지출 합산 [num]골드>
	COUNT_LIKE_UP_NPC            = 8,        // <데일리 퀘스트><NPC 호감도 [num] 높이기>
	COUNT_FEVER_SEARCH           = 9,        // <데일리 퀘스트><피버 활성화 상태로 [num]회 탐사>
	COUNT_BUFF_SEARCH            = 10,       // <데일리 퀘스트><어떤 것이든 [target] 탐사버프(1:비약,2:주문서) 걸려있는 상태로 [num]회 탐사>
	COUNT_CORE_ENCHANT           = 11,       // <데일리 퀘스트><코어 강화 합계 +[num] 달성>
	COUNT_CORE_EVO               = 12,       // <데일리 퀘스트><[target] 등급(별갯수)의 코어 진화 [num]회>
	COUNT_USER_BATTLE            = 13,       // <데일리 퀘스트><[target] 유저배틀 컨텐츠(8:약탈, 9:PVP, 20:아레나)에서 [num]회 도전>
	COUNT_RACE_BATTLE            = 14,       // <데일리 퀘스트><던전에서 종족전투 [num]회 도전>
	ENDINDEX                     = 15,       
}

public enum CHARACTER_TYPE { 
	BOSS                         = 1,        // <캐릭터 타입><보스>
	MONSTER                      = 2,        // <캐릭터 타입><몬스터>
	HERO                         = 3,        // <캐릭터 타입><영웅>
	GIANT                        = 4,        // <캐릭터 타입><자이언트>
	TOWER                        = 5,        // <캐릭터 타입><타워>
	SUMMON                       = 6,        // <캐릭터 타입><소환물>
	NPC                          = 7,        // <캐릭터 타입><NPC>
	RACE_HERO                    = 8,        // <캐릭터 타입><종족 전투 전용>
	COSTUME                      = 9,        // <캐릭터 타입><코스튬>
	ENDINDEX                     = 10,       
}

public enum EP_CATEGORY { 
	MERCHANT                     = 1,        // <EP 카테고리><떠돌이 상인>
	STAGE_MONSTER                = 2,        // <EP 카테고리><몬스터 스테이지>
	STAGE_BOSS                   = 3,        // <EP 카테고리><보스 스테이지>
	GOLD                         = 4,        // <EP 카테고리><골드EP - 피버의 골드량 x5 효과를 받지 않음>
	TREASURE_SINGLE              = 5,        // <EP 카테고리><싱글 트레져>
	TREASURE_NORMAL              = 6,        // <EP 카테고리><보물상자>
	FEVER                        = 7,        // <EP 카테고리><피버>
	PVP_BASE_BATTLE              = 8,        // <EP 카테고리><PVP 베이스 공략전>
	PVP_HERO_BATTLE              = 9,        // <EP 카테고리><PVP 히어로 파티 배틀>
	GUILD_TARGET                 = 10,       // <EP 카테고리><길드전 타겟>
	GATHER_HERB                  = 11,       // <EP 카테고리><채집-약초>
	GATHER_VEIN                  = 12,       // <EP 카테고리><채집-광맥>
	GATHER_TREE                  = 13,       // <EP 카테고리><채집-나무>
	NPC                          = 14,       // <EP 카테고리><NPC>
	PVP_BASE_REVENGE             = 15,       // <EP 카테고리><약탈 복수>
	PVP_HERO_REVENGE             = 16,       // <EP 카테고리><PVP 복수>
	RACE_BATTLE                  = 17,       // <EP 카테고리><종족 전투>
	STAGE_EXTRABOSS              = 18,       // <EP 카테고리><EX보스>
	STAGE_DUNGEON_TARGET         = 19,       // <EP 카테고리><던전타겟 스테이지>
	PVP_ARENA_BATTLE             = 20,       // <EP 카테고리><실시간 전투>
	STAGE_WORLDBOSS              = 21,       // <EP 카테고리><월드보스 스테이지>
	GUILD_BATTLE                 = 22,       // <EP 카테고리><길드 전투>
	PVP_DUEL_BATTLE              = 23,       // <EP 카테고리><듀얼 전투>
	PVP_ARENADUEL_BATTLE         = 24,       // <EP 카테고리><데일리 퀘스트(아레나와 듀얼 카운트) 체크를 위한 가상의 EP>
	ENDINDEX                     = 25,       
}

public enum EP_ADVENT_CONDITION { 
	SEARCH_COUNT                 = 1,        // <EP Array 출현조건 타입><탐사 횟수 (val = 횟수)>
	MASTER_LV                    = 2,        // <EP Array 출현조건 타입><사용자레벨 (val = 레벨)>
	QUEST_ING                    = 3,        // <EP Array 출현조건 타입><메인퀘스트 진행중 (val = Quest ID)>
	QUEST_CLEAR                  = 4,        // <EP Array 출현조건 타입><메인퀘스트 클리어 (val = Quest ID)>
	QUEST_NO_CLEAR               = 5,        // <EP Array 출현조건 타입><메인퀘스트를 아직 클리어하지 않았으면 (val = Quest ID)>
	FEVER_READY                  = 6,        // <EP Array 출현조건 타입><피버레디 상태면 (val = 피버레디 Grade)>
	FEVER_ACTIVE                 = 7,        // <EP Array 출현조건 타입><피버발동 상태면 (val은 사용안함)>
	SUBQUEST_ING                 = 8,        // <EP Array 출현조건 타입><서브퀘스트 진행중 (val = Quest ID)>
	EXTRABOSS_ACTIVE             = 9,        // <EP Array 출현조건 타입><EX보스 활성화 (Val에 areaID를 넣을시 해당 area가 아닐경우 TREASURE_SINGLE로 대체)>
	HARD_QUEST_ING               = 10,       // <EP Array 출현조건 타입><하드모드 메인퀘스트 진행중 (val = Quest ID)>
	ENDINDEX                     = 11,       
}

public enum UNLOCK_CONTENTS { 
	NONE                         = 0,        // <언락 컨텐츠><아무 것도 아님>
	RANK_UP                      = 1,        // <언락 컨텐츠><랭크 업>
	SKILL_UP                     = 2,        // <언락 컨텐츠><스킬 탭>
	LV_UP_BY_ETHER               = 3,        // <언락 컨텐츠><레벨업 탭>
	HERO_QUEST                   = 4,        // <언락 컨텐츠><히어로 퀘스트>
	FREE_TREASURE                = 5,        // <언락 컨텐츠><무료상자 리젠>
	ONE_MORE                     = 6,        // <언락 컨텐츠><클리어한 스테이지 한번 더 플레이>
	MOVE_TO_BASE                 = 7,        // <언락 컨텐츠><베이스로 이동>
	INSCRIPTION                  = 8,        // <언락 컨텐츠><각인>
	SEARCH_BUFF                  = 9,        // <언락 컨텐츠><탐사버프>
	CORE                         = 10,       // <언락 컨텐츠><코어>
	MAXIMUM_BATTLE_SPEED         = 11,       // <언락 컨텐츠><3배속 전투>
	EQUIP_ALL                    = 12,       // <언락 컨텐츠><히어로 장비 일괄 장착>
	SKILL_UP_ALL                 = 13,       // <언락 컨텐츠><히어로 스킬 일괄 레벨업>
	DAILY_QUEST                  = 14,       // <언락 컨텐츠><데일리 퀘스트>
	ENDINDEX                     = 15,       
}

public enum HERO_LIST_STATE { 
	VISIBLE                      = 1,        // <목록에서의 영웅 상태><영웅 리스트에서 보임>
	INVISIBLE                    = 2,        // <목록에서의 영웅 상태><영웅 리스트에서 보이지 않다가, 영웅이나 해당 소환석을 획득하면 보임>
	SYSTEM                       = 3,        // <목록에서의 영웅 상태><절대 안 보이며, 생성도 안됨>
	ENDINDEX                     = 4,        
}

public enum TOUCH_GUIDE_TYPE { 
	HOLD                         = 1,        // <터치 가이드 타입><홀드>
	CONTINUE                     = 2,        // <터치 가이드 타입><컨티뉴>
	CALL                         = 3,        // <터치 가이드 타입><콜. 뒤이어 정의된 버튼을 포함한 UI 패널을 띄우기만 함>
	ENDINDEX                     = 4,        
}

public enum ACHIEVEMENT_TYPE { 
	MASTER_LV                    = 1,        // <업적 타입><사용자레벨>
	UNLOCK_SEARCH_ID             = 2,        // <업적 타입><특정 난이도의 지역 개방>
	HERO_NUMBER                  = 3,        // <업적 타입><히어로 보유수>
	GIANT_NUMBER                 = 4,        // <업적 타입><자이언트 보유 수>
	BEST_EVO                     = 5,        // <업적 타입><최고 진화 히어로의 별 개수>
	ACCUM_STAR                   = 6,        // <업적 타입><누적 별 개수 (반복 스테이지의 획득 별 포함)>
	ACCUM_SEARCH                 = 7,        // <업적 타입><누적 탐사 횟수>
	ACCUM_ETHER                  = 8,        // <업적 타입><누적 에테르 획득량>
	ACCUM_GOLD                   = 9,        // <업적 타입><누적 골드 획득량 (캐시구매분 포함)>
	ACCUM_TREASURE_BOX           = 10,       // <업적 타입><탐사에서 획득한 보물상자 누적 개수>
	PVP_BASE_WIN                 = 11,       // <업적 타입><다른 플레이어의 코어 플랜트 파괴 횟수>
	PVP_BASE_DEFENSE             = 12,       // <업적 타입><코어 플랜트 방어 횟수>
	PVP_HERO_WIN                 = 13,       // <업적 타입><히어로 PVP 승리 횟수>
	STAR_SEARCH_ID               = 14,       // <업적 타입><특정 난이도 지역의 획득 별 개수. 누적아님>
	DUNGEON_TREASURE_BOX         = 15,       // <업적 타입><특정 던전에서 보물상자 획득 개수>
	DUNGEON_TARGET_CLEAR         = 16,       // <업적 타입><특정 던전에서 타겟 클리어 횟수>
	DUNGEON_GRADE3_KILL          = 17,       // <업적 타입><특정 던전에서 GRADE3 스테이지 클리어 횟수 (보스제외)>
	DUNGEON_GRADE4_KILL          = 18,       // <업적 타입><특정 던전에서 GRADE4 스테이지 클리어 횟수 (보스제외)>
	DUNGEON_BOSS_KILL            = 19,       // <업적 타입><특정 던전에서 보스 스테이지 클리어 횟수>
	DUNGEON_FLOOR_CLEAR          = 20,       // <업적 타입><특정 던전의 특정 층 클리어>
	RACEBATTLE_ACCUM_SCORE       = 21,       // <업적 타입><모든 종족전투의 스코어 누적 합산치>
	RACEBATTLE_RECORD            = 22,       // <업적 타입><특정 종족전투의 최고 스코어 기록>
	ENDINDEX                     = 23,       
}

public enum BONUS_AGGRO { 
	INIT_TANK                    = 1,        // <추가 어그로><탱커 시작 어그로>
	INIT_MELEE                   = 2,        // <추가 어그로><밀리 시작 어그로>
	ENDINDEX                     = 3,        
}

public enum RESOURCE { 
	DARKETHER                    = 1,        // <베이스 자원><다크 에테르>
	LUMBER                       = 2,        // <베이스 자원><목재>
	CUBIC                        = 3,        // <베이스 자원><큐빅>
	ENDINDEX                     = 4,        
}

public enum GOLD_SALES_TYPE { 
	COUNT_UP                     = 1,        // <골드상품 타입><구매횟수별 가격변동 상품>
	LIMIT                        = 2,        // <골드상품 타입><한정판매 상품>
	ENDINDEX                     = 3,        
}

public enum RACE { 
	NONE                         = 0,        // <종족><종족 없음>
	ANCIENT_GIANT                = 1,        // <종족><고대 자이언트>
	HUMAN                        = 2,        // <종족><인간>
	ORC                          = 3,        // <종족><오크>
	DARK_ELF                     = 4,        // <종족><다크 엘프>
	UNDEAD                       = 5,        // <종족><언데드>
	BEAST                        = 6,        // <종족><야수족>
	DRAGON                       = 7,        // <종족><용족>
	MACHINE                      = 8,        // <종족><기계>
	ENDINDEX                     = 9,        
}

public enum RESEARCH_GROUP { 
	ATTACK                       = 1,        // <연구스킬 그룹><공격형>
	DEFENSE                      = 2,        // <연구스킬 그룹><방어형>
	SUPPORT                      = 3,        // <연구스킬 그룹><지원형>
	GIANT                        = 4,        // <연구스킬 그룹><자이언트>
	ENDINDEX                     = 5,        
}

public enum CALCULATE_TYPE { 
	PLUS                         = 1,        // <계산방식><더하기>
	MINUS                        = 2,        // <계산방식><빼기>
	RATE_PLUS                    = 3,        // <계산방식><같은 스탯에 대해서 (1+sum(RATE_PLUS값들)-sum(RATE_MINUS값들))의 과정을 거친 다음 곱한다.>
	RATE_MINUS                   = 4,        // <계산방식><같은 스탯에 대해서 (1+sum(RATE_PLUS값들)-sum(RATE_MINUS값들))의 과정을 거친 다음 곱한다.>
	ENDINDEX                     = 5,        
}

public enum CRAFT_TYPE { 
	BREWING                      = 1,        // <크래프트 타입><제약>
	SPELL_SCRIBING               = 2,        // <크래프트 타입><주문서 제작>
	ALCHEMY_MIX                  = 3,        // <크래프트 타입><연금술_합성>
	ALCHEMY_EXTRACT              = 4,        // <크래프트 타입><연금술_분해>
	INSCRIPTION                  = 5,        // <크래프트 타입><각인>
	VEIN                         = 6,        // <크래프트 타입><광맥 (서버 처리용)>
	HERB                         = 7,        // <크래프트 타입><약초 (서버 처리용)>
	TREE                         = 8,        // <크래프트 타입><나무 (서버 처리용)>
	ENDINDEX                     = 9,        
}

public enum SEARCH_BUFF_TYPE { 
	SB_ELIXIR_TYPE               = 1,        // <탐사버프 타입><엘릭서>
	SB_SCROLL_TYPE               = 2,        // <탐사버프 타입><주문서>
	ENDINDEX                     = 3,        
}

public enum SB_ELIXIR_TYPE { 
	SEARCH_GOLD                  = 1,        // <탐사버프 엘릭서 타입><서버 / 탐사결과 싱글트레져와 트레져박스의 골드에 Val을 곱해줌>
	SEARCH_ETHER                 = 2,        // <탐사버프 엘릭서 타입><서버 / 탐사결과 싱글트레져와 트레져박스의 에테르 생성시 Val을 곱해줌>
	SEARCH_COST                  = 3,        // <탐사버프 엘릭서 타입><서버, 클라 / 탐사비용에 Val을 곱해줌>
	SEARCH_FEVERSTAR             = 4,        // <탐사버프 엘릭서 타입><서버 / 탐사시 획득하는 피버스타에 Val을 곱해줌>
	SEARCH_DUNGEON_ADD_EP        = 5,        // <탐사버프 엘릭서 타입><서버 / 던전의 EP 출현 개수 최소값이 Val만큼 증가>
	SEARCH_ABILITY               = 6,        // <탐사버프 엘릭서 타입><서버, 클라 / 계산된 탐사력을 Val만큼 증가시킴>
	RATE_RAREMONSTER             = 7,        // <탐사버프 엘릭서 타입><서버 / 확률보정 변수 RAREMONSTER의 값을 Val로 설정>
	RATE_BOSS                    = 8,        // <탐사버프 엘릭서 타입><서버 / 확률보정 변수 BOSS의 값을 Val로 설정>
	RATE_TREASUREBOX             = 9,        // <탐사버프 엘릭서 타입><서버 / 확률보정 변수 TREASUREBOX의 값을 Val로 설정>
	ENDINDEX                     = 10,       
}

public enum SB_SCROLL_TYPE { 
	SCROLL_ATTACK                = 1,        // <탐사버프 주문서 타입><클라 / 물리,마법 공격력에 Val을 곱해줌>
	SCROLL_DEF                   = 2,        // <탐사버프 주문서 타입><클라 / 물리,마법 방어력에 Val을 곱해줌>
	SCROLL_MAX_HP                = 3,        // <탐사버프 주문서 타입><클라 / 최대체력에 Val을 곱해줌>
	SCROLL_HEALED_AMPLIFY        = 4,        // <탐사버프 주문서 타입><클라 / 받는 회복량 증가 스탯에 Val을 더해줌>
	SCROLL_PHYSICAL_CRITICAL     = 5,        // <탐사버프 주문서 타입><클라 / 물리 크리티컬 확률에 Val을 더해줌>
	SCROLL_MAGICAL_CRITICAL      = 6,        // <탐사버프 주문서 타입><클라 / 마법 크리티컬 확률에 Val을 더해줌>
	SCROLL_NORMAL_COOLTIME       = 7,        // <탐사버프 주문서 타입><클라 / 평타의 쿨타임에 Val을 더해줌 (구현필요)>
	SCROLL_AUTO_COOLTIME         = 8,        // <탐사버프 주문서 타입><클라 / 자동 스킬의 쿨타임에 Val을 더해줌 (구현필요)>
	SCROLL_TOUCH_COOLTIME        = 9,        // <탐사버프 주문서 타입><클라 / 터치 스킬의 쿨타임에 Val을 더해줌 (수정필요)>
	SCROLL_FATAL_CHARGE          = 10,       // <탐사버프 주문서 타입><클라 / 필살기 게이지 증가값에 Val을 더해줌 (구현필요)>
	SCROLL_EXP_BONUS             = 11,       // <탐사버프 주문서 타입><서버 / 전투 승리시 획득하는 히어로XP에 Val을 곱해줌>
	SCROLL_ETHER_BONUS           = 12,       // <탐사버프 주문서 타입><서버 / 전투 승리시 획득하는 에테르에 Val을 곱해줌>
	SCROLL_ITEM_BONUS            = 13,       // <탐사버프 주문서 타입><서버 / 전리품 아이템개수 랜덤범위의 최대값에 Val을 더해줌>
	SCROLL_FEVERSTAR_BONUS       = 14,       // <탐사버프 주문서 타입><서버 / 전투 승리시 획득 별개수 + Val만큼 피버스타 지급>
	ENDINDEX                     = 15,       
}

public enum ADD_SEARCH_RATE { 
	FEVER                        = 1,        // <탐사 출현확률 보정 타입><피버의 추가 출현비율>
	RATE_RAREMONSTER             = 7,        // <탐사 출현확률 보정 타입><3,4Grade 몬스터의 추가 출현비율>
	RATE_BOSS                    = 8,        // <탐사 출현확률 보정 타입><보스 몬스터의 추가 출현비율>
	RATE_TREASUREBOX             = 9,        // <탐사 출현확률 보정 타입><보물상자의 추가 출현비율>
	ENDINDEX                     = 10,       
}

public enum CORE_RANK { 
	C                            = 1,        // <코어 랭크><코어 랭크>
	B                            = 2,        // <코어 랭크><코어 랭크>
	A                            = 3,        // <코어 랭크><코어 랭크>
	S                            = 4,        // <코어 랭크><코어 랭크>
	SS                           = 5,        // <코어 랭크><코어 랭크>
	ENDINDEX                     = 6,        
}

public enum CORE_TYPE { 
	HERO                         = 1,        // <코어 타입><히어로 코어>
	ENCHANT                      = 2,        // <코어 타입><강화돼지>
	EVO                          = 3,        // <코어 타입><진화돼지>
	RANK                         = 4,        // <코어 타입><랭크돼지>
	MIGHTY                       = 5,        // <코어 타입><만능진화돼지>
	EQUIP                        = 6,        // <코어 타입><장착전용코어>
	UNIQUE                       = 7,        // <코어 타입><유니크코어>
	ENDINDEX                     = 8,        
}

public enum DIALOG_TYPE { 
	CENTER                       = 1,        // <다이얼로그 타입><화면 중앙에 3D 포트레이트+중앙상단 말풍선>
	MINI                         = 2,        // <다이얼로그 타입><대형 포트레이트 없이 2D 썸네일+하단 말풍선>
	ENDINDEX                     = 3,        
}

public enum SETTING_PUSH { 
	FREE_CHEST                   = 1,        // <푸시 설정 옵션><무료 상자 충전>
	GATHER_COMPLETE              = 2,        // <푸시 설정 옵션><파견 완료>
	RESEARCH_COMPLETE            = 3,        // <푸시 설정 옵션><연구 완료>
	WORKSHOP_COMPLETE            = 4,        // <푸시 설정 옵션><제작 완료>
	BUILDING_UPGRADE             = 5,        // <푸시 설정 옵션><업그레이드 완료>
	ENDINDEX                     = 6,        
}

public enum BUILDING { 
	NONE                         = 0,        // <베이스 건물><아무 것도 아님>
	HALL                         = 1,        // <베이스 건물><타운홀>
	REFINERY                     = 2,        // <베이스 건물><정제소>
	LUMBERMILL                   = 3,        // <베이스 건물><제재소>
	MINE                         = 4,        // <베이스 건물><채굴장>
	GIANTBUILDING                = 5,        // <베이스 건물><자이언트관리>
	RESEARCH                     = 6,        // <베이스 건물><연구소>
	WORKSHOP                     = 7,        // <베이스 건물><공방>
	GUILD_BASE                   = 8,        // <베이스 건물><길드 베이스>
	ENDINDEX                     = 9,        // <베이스 건물><>
}

public enum BASE_UNLOCK { 
	OPEN                         = 1,        // <베이스 컨텐츠 개방><건물 오픈>
	ADD_PRODUCTION               = 2,        // <베이스 컨텐츠 개방><생산소 추가>
	ADD_STORAGE                  = 3,        // <베이스 컨텐츠 개방><저장소 추가>
	ADD_TOWER                    = 4,        // <베이스 컨텐츠 개방><타워 추가>
	ADD_SLOT                     = 5,        // <베이스 컨텐츠 개방><슬롯 추가>
	ENDINDEX                     = 15,       // <베이스 컨텐츠 개방><>
}

public enum BUFF_EVENT_GROUP { 
	SEARCH                       = 1,        // <버프 이벤트 그룹><탐사>
	HERO                         = 2,        // <버프 이벤트 그룹><히어로>
	BASE                         = 3,        // <버프 이벤트 그룹><베이스>
	ASSIGN                       = 4,        // <버프 이벤트 그룹><파견소>
	TELEPOD                      = 5,        // <버프 이벤트 그룹><텔레팟>
	ENDINDEX                     = 6,        // <버프 이벤트 그룹><>
}

public enum BUFF_EVENT { 
	NONE                         = 0,        // <버프 이벤트><버프이벤트 없음>
	HERO_EXP                     = 1,        // <버프 이벤트><히어로 경험치 획득량 n% 증가>
	SEARCH_GOLD                  = 2,        // <버프 이벤트><일반탐사에서 골드카드의 골드 획득량 n%증가>
	SEARCH_FEVER_GOLD            = 3,        // <버프 이벤트><피버에서 골드카드의 골드 획득량 n%증가>
	SEARCH_ETHER                 = 4,        // <버프 이벤트><일반탐사에서 에테르카드의 에테르 획득량 n% 증가>
	SEARCH_FEVER_ETHER           = 5,        // <버프 이벤트><피버에서 에테르카드의 에테르 획득량 n% 증가>
	TREASUREBOX_RATE             = 6,        // <버프 이벤트><보물상자 출현 확률 n% 증가>
	FEVERSTAR_ADD                = 7,        // <버프 이벤트><피버스타 획득개수 n개 추가 지급>
	FEVERCOUNT_ADD               = 8,        // <버프 이벤트><피버카운트 n개 추가 지급>
	PARTY_ATTACK                 = 9,        // <버프 이벤트><파티 전원의 모든 공격력 n% 증가>
	RAID_REWARD                  = 10,       // <버프 이벤트><약탈 성공 시 획득 보상 n% 증가>
	SEARCH_COST                  = 11,       // <버프 이벤트><탐사/재탐사 비용 n% 할인>
	HERO_LEVEL_COST              = 12,       // <버프 이벤트><히어로 레벨업 비용 n% 할인>
	HERO_SKILL_COST              = 13,       // <버프 이벤트><스킬 레벨업 비용 n% 할인>
	HERO_EVO_COST                = 14,       // <버프 이벤트><진화 비용 n% 할인>
	HERO_ENCHANT_RATE            = 15,       // <버프 이벤트><각인 강화 성공 확률 n% 상승>
	CORE_ENCHANT_COST            = 16,       // <버프 이벤트><코어 강화 시 소모되는 에테르량 n% 감소>
	CORE_ENCHANT_RATE            = 17,       // <버프 이벤트><코어 강화 성공확률 n% 증가>
	DARKETHER_PRODUCE            = 18,       // <버프 이벤트><생산되는 다크에테르 n% 증가>
	LUMBER_PRODUCE               = 19,       // <버프 이벤트><생산되는 목재 n% 증가>
	CUBIC_PRODUCE                = 20,       // <버프 이벤트><생산되는 큐빅 n% 증가>
	BUILDING_COST                = 21,       // <버프 이벤트><모든 생산 건물 업그레이드 비용 n% 할인>
	ATTACK_RESEARCH_COST         = 22,       // <버프 이벤트><공격형 연구비용 n% 할인>
	DEFENSE_RESEARCH_COST        = 23,       // <버프 이벤트><방어형 연구비용 n% 할인>
	SUPPORT_RESEARCH_COST        = 24,       // <버프 이벤트><지원형 연구비용 n% 할인>
	GOTCHA_HERO_CORE_COST        = 25,       // <버프 이벤트><히어로/코어, 코어 소환 크리스탈 소모량 n% 할인>
	GOTCHA_EQUIP_COST            = 26,       // <버프 이벤트><장비 부스터 소환 크리스탈 소모량 n% 할인>
	GOLD_REFILL                  = 27,       // <버프 이벤트><무료 리필 골드 n% 추가 지급>
	GOLD_COST                    = 28,       // <버프 이벤트><골드 구매 크리스탈 소모 개수 n% 할인>
	ENDINDEX                     = 29,       // <버프 이벤트><>
}

public enum PURCHASE_GROUP { 
	HIT_ITEM                     = 1,        // <텔레팟 항목><인기상품>
	SUMMON                       = 2,        // <텔레팟 항목><소환>
	CASH                         = 3,        // <텔레팟 항목><크리스탈>
	GOLD                         = 4,        // <텔레팟 항목><골드>
	PACKAGE                      = 5,        // <텔레팟 항목><패키지>
	ONLY_CASH                    = 6,        // <텔레팟 항목><크리스탈 전용>
	SPECIAL                      = 7,        // <텔레팟 항목><특별상점>
	ARENA                        = 8,        // <텔레팟 항목><명예상점>
	RACE_BATTLE                  = 9,        // <텔레팟 항목><전투상자>
	COSTUME_HERO                 = 10,       // <텔레팟 항목><코스튬>
	HIDDEN                       = 11,       // <텔레팟 항목><히든>
	ENDINDEX                     = 12,       // <텔레팟 항목><>
}

public enum PURCHASE_TAG { 
	NEW                          = 1,        // <구매용 태그><New>
	HOT                          = 2,        // <구매용 태그><Hot>
	EVENT                        = 3,        // <구매용 태그><Event>
	ENDINDEX                     = 4,        // <구매용 태그><>
}

public enum PURCHASE_PROVIDE_TYPE { 
	INSTANT                      = 1,        // <지급방식><구매 즉시 지급>
	DAILY                        = 2,        // <지급방식><매일 지급>
	ENDINDEX                     = 3,        // <지급방식><>
}

public enum PURCHASE_TYPE { 
	CASH                         = 1,        // <상품 타입><캐쉬>
	GOLD                         = 2,        // <상품 타입><골드>
	GOTCHA                       = 3,        // <상품 타입><뽑기형>
	DAILY                        = 4,        // <상품 타입><매일상품>
	GOAL                         = 5,        // <상품 타입><달성상품>
	FREE_TREASURE                = 6,        // <상품 타입><보급품>
	OFFERWALL                    = 7,        // <상품 타입><오퍼월>
	COSTUME                      = 8,        // <상품 타입><코스튬>
	ENDINDEX                     = 9,        // <상품 타입><>
}

public enum PURCHASE_GOAL_CONDITION { 
	MAINQUEST                    = 1,        // <달성상품 조건><메인퀘스트 클리어>
	MASTERLEVEL                  = 2,        // <달성상품 조건><마스터레벨 달성>
	ENDINDEX                     = 3,        // <달성상품 조건><>
}

public enum WEBEVENT_TYPE { 
	LOGIN_COUNT                  = 1,        // <웹이벤트 타입><매일 로그인 횟수>
	HERO2_HOLD_COUNT             = 2,        // <웹이벤트 타입><2성히어로 보유횟수>
	HERO3_HOLD_COUNT             = 3,        // <웹이벤트 타입><3성히어로 보유횟수>
	HERO4_HOLD_COUNT             = 4,        // <웹이벤트 타입><4성히어로 보유횟수>
	BATTLE_COUNT                 = 5,        // <웹이벤트 타입><전투 횟수>
	HERO_LVUP_COUNT              = 6,        // <웹이벤트 타입><히어로 레벨업 횟수>
	EQUIP_COUNT                  = 7,        // <웹이벤트 타입><장착 횟수>
	SHOP_ITEM                    = 8,        // <웹이벤트 타입><아이템 샵>
	ENDINDEX                     = 9,        // <웹이벤트 타입><>
}

public enum POPUP_CONDITION { 
	ENTER_TELEPOD                = 1,        // <팝업 조건><텔레팟 입장 시>
	ENDINDEX                     = 2,        // <팝업 조건><>
}

public enum POPUP_APPEAR_TYPE { 
	ROLLING                      = 1,        // <팝업 등장스타일><하나만 순차적으로 등장>
	ENDINDEX                     = 2,        // <팝업 등장스타일><매일 1회 발생>
}

public enum POPUP_STYLE { 
	ONE_BUTTON                   = 1,        // <팝업 출력 스타일><상품 하나>
	TWO_BUTTON                   = 2,        // <팝업 출력 스타일><상품 둘>
	ENDINDEX                     = 3,        // <팝업 출력 스타일><>
}

public enum DECK_TYPE { 
	BATTLE_5                     = 1,        // <덱저장타입><5인 전투>
	BATTLE_10                    = 2,        // <덱저장타입><10인 전투>
	ARENA                        = 3,        // <덱저장타입><아레나>
	PVP                          = 4,        // <덱저장타입><PVP>
	WORLDBOSS                    = 5,        // <덱저장타입><월드보스>
	GUILD_BATTLE                 = 6,        // <덱저장타입><길드전>
	BASE_RAID                    = 7,        // <덱저장타입><약탈>
	DUEL                         = 8,        // <덱저장타입><듀얼>
	ENDINDEX                     = 9,        // <덱저장타입><>
}

public enum CONTENT_TYPE { 
	NONE                         = 0,        // <컨텐츠 타입><없음>
	ARENA                        = 1,        // <컨텐츠 타입><아레나>
	PVP                          = 2,        // <컨텐츠 타입><PVP>
	RAID                         = 3,        // <컨텐츠 타입><약탈>
	ENDINDEX                     = 4,        // <컨텐츠 타입><>
}

public enum GUILD_BUILDING { 
	NONE                         = 0,        // <길드 건물><>
	GUILD_HALL                   = 1,        // <길드 건물><길드홀>
	STORAGE_DARK                 = 2,        // <길드 건물><다크에테르 저장고>
	STORAGE_LUMBER               = 3,        // <길드 건물><목재 저장고>
	STORAGE_CUBIC                = 4,        // <길드 건물><큐빅 저장고>
	GUILD_SHOP                   = 5,        // <길드 건물><길드샵>
	GUILD_LABORATORY             = 6,        // <길드 건물><길드전 연구소>
	BATTLE_TOWER                 = 7,        // <길드 건물><전쟁의 탑>
	GUILD_WORKSHOP               = 8,        // <길드 건물><길드 공방>
	GUILD_QUEST                  = 9,        // <길드 건물><모험가의 전당>
	CRISTAL_VEIN                 = 10,       // <길드 건물><크리스탈 광맥>
	MAZE_OF_TIME                 = 11,       // <길드 건물><시간의 미궁>
	GUILD_TELEPOD                = 12,       // <길드 건물><길드 텔레팟>
	ASTROLOGY_RESEARCH           = 13,       // <길드 건물><점성술 연구소>
	ENDINDEX                     = 14,       // <길드 건물><>
}

public enum BUY_COUNT_INIT_TYPE { 
	NO_INIT                      = 1,        // <구매횟수 초기화 방식><초기화되지 않음>
	PERSONAL                     = 2,        // <구매횟수 초기화 방식><개인이 구매한 시점 + 쿨타임기준 기준>
	PERSONAL_DAY                 = 3,        // <구매횟수 초기화 방식><개인이 구매한 날의 00시 + 쿨타임기준 기준>
	MONTHLY                      = 4,        // <구매횟수 초기화 방식><구매한 달(m) 기준>
	ENDINDEX                     = 5,        // <구매횟수 초기화 방식><>
}

public enum ARENADUEL_TYPE { 
	NONE                         = 0,        // <아레나 듀얼 타입><>
	ARENA                        = 1,        // <아레나 듀얼 타입><아레나>
	DUEL                         = 2,        // <아레나 듀얼 타입><듀얼>
	ENDINDEX                     = 3,        // <아레나 듀얼 타입><>
}

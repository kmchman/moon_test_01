/////////////////////////////////////////
// Export To ABSW_EnumType.xlsm
// Last Update : 2019-07-24:17:11:10
/////////////////////////////////////////

namespace Table
{
    public enum eValueType
    {
        None,
        Absolute,
        Ratio,
        MaxCount,
    }

    public enum eItemType
    {
        None,
        Life,
        AbyssCash,
        Exp,
        Crops,
        Goods,
        Area_Extend,
        Storage_Extend,
        Building_Active,
        Breed_Rate,
        MaxCount,
    }

    public enum eBuildingType
    {
        None,
        Stone,
        Factory,
        Farmland,
        Farm,
        House,
        Breed,
        Quest,
        Storage,
        Deco,
        MaxCount,
    }

    public enum eSubObjectType
    {
        None,
        EmptyField,
        Crop,
        Terrain,
        Road,
        Pavement,
        Barn,
        Mine,
        Factory,
        House,
        Fish,
        Deco,
        Village,
        Hall,
        Feed,
        Breed,
        Quest1,
        Quest2,
        Quest3,
        Quest4,
        Product,
        Stone,
        MaxCount,
    }

    public enum eFishType
    {
        None,
        Normal,
        Breeding,
        Attribute,
        Event,
        Cash,
        Npc,
        MaxCount,
    }

    public enum eFishSizeType
    {
        None,
        Small,
        Medium,
        Big,
        MaxCount,
    }

    public enum eBeaconType
    {
        None,
        Beacon_Decoy,
        Beacon_Banish,
        MaxCount,
    }

    public enum eProfileType
    {
        None,
        SNS_FACEBOOK,
        SNS_TWITTER,
        Fish,
        CostumeSet,
        MaxCount,
    }

    public enum eFishPoolType
    {
        None,
        ZoneA,
        ZoneB,
        ZoneC,
        ZoneD,
        ZoneE,
        ZoneF,
        ZoneG,
        ZoneH,
        ZoneI,
        ZoneJ,
        ZoneK,
        ZoneL,
        ZoneM,
        ZoneN,
        ZoneO,
        ZoneP,
        ZoneQ,
        ZoneR,
        ZoneS,
        ZoneT,
        ZoneU,
        ZoneV,
        ZoneW,
        ZoneX,
        ZoneY,
        ZoneZ,
        MaxCount,
    }

    public enum eOverWrapType
    {
        None,
        BaseTile,
        Tile,
        Road,
        Functional,
        FunctionalDeco,
        Deco,
        Villiager,
        Effect,
        MaxCount,
    }

    public enum eBuyingType
    {
        None,
        Skin,
        Each,
        MaxCount,
    }

    public enum eUnlockCondition
    {
        None,
        MaxCount,
    }

    public enum eMaterialType
    {
        None,
        Life,
        AbyssCash,
        Crop,
        Fish,
        AD,
        Production_Farm,
        Production_Factory,
        MaxCount,
    }

    public enum eProductionType
    {
        None,
        First,
        Second,
        Third,
        MaxCount,
    }

    public enum eAttribute
    {
        None,
        Ground,
        Fire,
        Water,
        Air,
        MaxCount,
    }

    public enum eBuildState
    {
        None,
        Progress,
        Finish,
        MaxCount,
    }

    public enum eSupplyType
    {
        None,
        Sale,
        Inventory,
        MaxCount,
    }

    public enum eCashType
    {
        None,
        Life,
        AbyssCash,
        MaxCount,
    }

    public enum eRewardType
    {
        None,
        AbyssCash,
        Exp,
        Life,
        MaxCount,
    }

    public enum eColliderType
    {
        None,
        Sphere,
        Capsule,
          Box,
        Cross,
        MaxCount,
    }

    public enum ePosition
    {
        None,
        Top,
        Bottom,
        Center,
        Weapon,
        Custom_1,
        Custom_2,
        Custom_3,
        Custom_4,
        MaxCount,
    }

    public enum eEffectAttachType
    {
        None,
        Pos,
        Pos_Rot,
        Link,
        MaxCount,
    }

    public enum eResourceType
    {
        None,
        Sprite,
        Texture,
        MaxCount,
    }

    public enum eGameMode
    {
        None,
        Single,
        Multi_1vs1,
        Multi_2vs2,
        Multi_3vs3,
        Village,
        MaxCount,
    }

    public enum eGameModeRule
    {
        None,
        IDLE,
        MaxCount,
    }

    public enum eGlobalRuleType
    {
        None,
        APPVERSION,
        ANDROIDVERSIONCODE,
        IOSBUILDNUMBER,
        GAMEID,
        GAMEKEY,
        SERVER_URL_TEST,
        GOOGLE_PLAYSTORE,
        APPLE_APPSTORE,
        URL_TWITTER,
        URL_FACEBOOK,
        MaxCount,
    }

    public enum eUIMenuType
    {
        None,
        House,
        Factory,
        Farm,
        Farmland,
        Deco,
        Special,
        Fish,
        MaxCount,
    }

    public enum eStorageType
    {
        None,
        Storage,
        MaxCount,
    }

    public enum eStorageFilterType
    {
        None,
        Crops,
        Goods,
        Construction,
        MaxCount,
    }

    public enum eConstantType
    {
        None,
        Immediately_AbyssCash_AreaExtend,
        Immediately_AbyssCash_Crop,
        Immediately_AbyssCash_Farm,
        Immediately_AbyssCash_Product,
        Immediately_AbyssCash_Build,
        Immediately_AbyssCash_QuestCooltime,
        Immediately_AbyssCash_BreedCooltime,
        Immediately_AbyssCash_BreedDeliveryTime,
        Immediately_Time_AreaExtend,
        Immediately_Time_Crop,
        Immediately_Time_Farm,
        Immediately_Time_Product,
        Immediately_Time_Build,
        Immediately_Time_QuestCooltime,
        Immediately_Time_BreedCooltime,
        Immediately_Time_BreedDeliveryTime,
        Breed_Unlock_Level,
        Breed_NpcFish_Lv_Up,
        Breed_NpcFish_CumulativeNum,
        Breed_NpcFish_CumulativeNum_Up,
        Breed_NpcFish_Mutant,
        Breed_NpcFish_Mutant_Up,
        Breed_Rate_Up,
        Fish_Sell_Cost_Rate,
        Default_Fish_Population,
        QuestType01_Breed_Fish_Rate,
        QuestType01_Dialogue_Num,
        QuestType01_Reward_Ratio_Min,
        QuestType01_Reward_Ratio_Max,
        QuestType02_Extend_01_Level,
        QuestType02_Extend_02_Level,
        QuestType02_Extend_01_Cost,
        QuestType02_Extend_02_Cost,
        Friend_MaxNum,
        Friend_All_MaxNum,
        Friend_Recommend_MaxNum,
        Friend_Recommend_Level_High,
        Friend_Recommend_Level_Low,
        Friend_Recommend_Cooltime,
        Friend_RecentHelp_MaxNum,
        Friend_RecentHelp_Duration,
        FreeGift_Offer_MaxNum,
        Npc_Quest_Cooltime,
        Npc_Quest_Probability,
        Npc_Breed_Cooltime,
        Npc_Breed_Probability,
        MaxCount,
    }

}

using UnityEngine;

using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using AL;

using STExtensions;

using DG.Tweening;

public class GiantHandler
{
	private static GiantHandler _inst = new GiantHandler();
	public static GiantHandler Inst { get { return _inst; } }
	
	public Action<UserDataFlag> updateUserDelegate = null;
	public Action<BaseDataFlag> updateBaseDelegate = null;
	public Action<bool> onInscriptionEnchantEndDelegate = null;
	public Action<List<GiantCoreEnchantResult>> onCoreEnchantEndDelegate = null;
	public Action extraBossResetTimeDelegate = null;

	public Action<DeckInfo> deckInfoDelegate = null;

	private HTTPClient ctx = null;
	private object	m_LockObject = new object();
	private bool	m_IsWsConnecting;

	private class PrivateData
	{
		public long uidUser = 0;
		public string passcode = string.Empty;

		public void SetData(long uid, string pw)
		{
			uidUser = uid;
			passcode = pw;
		}
	}

	private PrivateData privateData = new PrivateData();

	private class RequestData
	{
		public STRequest req;
		public string json;
		public bool isClearReward;
		public int timeout;
	}
	
	private Queue<RequestData> waitingRequests = new Queue<RequestData>();
	private Queue<RequestData> readyRequests = new Queue<RequestData>();
	
	public string resourceURL { get; private set; }
	public string resourceID { get; private set; }

	public string chatServerURL { get; private set; }
	public string publicIp { get; set;}
	public int timeOffset { get; set;}

	public bool isRequesting { get { return STRequest.IsWait(); } }

	private long timeGap;
	private int timeFrameCount;
	private long curServerTime;

	private WSClientST m_ArenaWSClient = null;
	public string recvArenaResult { get; private set; }
	public bool isArenaResultByDisconnected { get; private set; }

#if UNITY_EDITOR || HIDDEN_LOGIN_TEST
	private const int editorLogin = 1;
	
	private static string cfgFile = "private.json";
	
	private bool LoadAccountConfig()
	{
		return Giant.Util.LoadJsonFile(cfgFile, privateData);
	}
	
	private void SaveAccountConfig()
	{
		Giant.Util.SaveJsonFile(cfgFile, privateData, false);
	}
	
	public bool LoadAccount()
	{
		return (LoadAccountConfig() && privateData.uidUser != 0 && privateData.passcode != string.Empty);
	}

	public void CreateHiddenLoginToken()
	{
		privateData.uidUser = 0;
		privateData.passcode = SystemInfo.deviceUniqueIdentifier + "-" + Giant.Util.GetLocalTimeString();
		SaveAccountConfig();
	}
#endif
	
	public long GetUidUser()
	{
		return privateData.uidUser;
	}

	public bool IsMyUidUser(long uidUser)
	{
		return uidUser == privateData.uidUser;
	}

	public string GetGuildID()
	{
		return AccountDataStore.instance.guildData._id;
	}

	public void Init(string serverURL)
	{
		ctx = new HTTPClient(this, serverURL);
		ctx.SetUseCustomTimeout(true);
		ctx.SetUseSeqToken(true);
	}
	
	public void Close()
	{
		CloseArenaSocket();
		
		while (waitingRequests.Count > 0)
			readyRequests.Enqueue(waitingRequests.Dequeue());
		
		STRequest.StopAllRequest();
	}

	#region request
	public STRequest Request(string json, bool isClearReward = true, int timeout = -1) { return Request(json, isClearReward, timeout, null); }
	public STRequest Request(string json, bool isClearReward, int timeout, STRequest request)
	{
		// OffLineTest
		return STRequest.Request();
		if (request == null)
			request = STRequest.Request();
		
		if (isRequesting)
		{
			request.OtherWait();
			
			RequestData reqData = (readyRequests.Count > 0) ? readyRequests.Dequeue() : new RequestData();
			reqData.req = request;
			reqData.json = json;
			reqData.isClearReward = isClearReward;
			reqData.timeout = timeout;
			waitingRequests.Enqueue(reqData);
			
			return request;
		}
		
		SendRequest(json, isClearReward, timeout, request);

		return request;
	}

	public STRequest SilentRequest(string json, int timeout = -1) { return SilentRequest(json, timeout, null); }
	public STRequest SilentRequest(string json, int timeout, STRequest request)
	{
		if (request == null)
			request = STRequest.Request();

		request.SetSilentMode(true);

		Request(json, false, timeout, request);

		return request;
	}
	
	public void ExecuteNextRequest()
	{
		if (waitingRequests.Count <= 0)
			return;
		RequestData reqData = waitingRequests.Dequeue();
		readyRequests.Enqueue(reqData);
		SendRequest(reqData.json, reqData.isClearReward, reqData.timeout, reqData.req);
	}

	private void SendRequest(string json, bool isClearReward, int timeout, STRequest request)
	{
		request.Init(json);
		request.SetTimeout(timeout);
		
		if (isClearReward)
			AccountDataStore.instance.ClearReward();

		Action<Action> retry = null;
		if (!request.isSilent)
			retry = (clickAction) => { DefaultRequestRetry(request, clickAction); };
		ctx.ALERequest(json, (result) => { request.OnResponse(result); }, timeout, retry);
	}
	
	public static void DefaultRequestRetry(STRequest request, Action clickAction = null)
	{
		Action retry = () => {
			request.ReleaseHold();
			if (clickAction != null)
				clickAction();
		};
		
		if (request.isCancel)
		{
			request.Return();
			return;
		}
		
		if (request.isHiddenMaxRetry)
		{
			request.Hold();
			SuperOverlayPopupManager.Inst.ShowOneButtonPopup(UITextEnum.BUILTIN_NETWORK_FAIL, UITextEnum.BUILTIN_OK).SetClickButtonAction(retry).SetAlias(PopupAlias.SuperOverlayNetworkPopup);
		}
		else
		{
			request.Hold();
			retry();	// Hidden Retry
		}
	}

	public static void DefaultRequestFailure(object result, Action clickAction = null)
	{
		SystemOneButtonPopup popup = SuperOverlayPopupManager.Inst.ShowOneButtonPopup(Datatable.Inst.GetServerErrorText(result.ToString()));
		popup.SetAlias(PopupAlias.SuperOverlayPopup);
		if (clickAction != null)
			popup.SetClickButtonAction(clickAction);
	}

	// header
	struct GuideHeader			// 유저 정보를 받기 전에 Login, ServerList 에 사용될 패킷 헤더
	{
		public string			type;
		public string			cmd;
		public string			ip;
	}

	struct UserHeader			// 유저 정보를 받은 후 일반적인 통신에 사용될 패킷 헤더
	{
		public string			type;
		public string			cmd;
		public long				uidUser;
		public string			token;
		public string			ip;

#if UNITY_EDITOR || HIDDEN_LOGIN_TEST
		public int				loginType;
#endif
	}

	private string GetGuideHeaderString(string type, string cmd)
	{
		GuideHeader header	= new GuideHeader();
		header.type			= type;
		header.cmd			= cmd;
		header.ip 			= publicIp;
		return Util.JsonEncode2(header);
	}

	private string GetUserHeaderString(string type, string cmd)
	{
		UserHeader header	= new UserHeader();
		header.type			= type;
		header.cmd			= cmd;
		header.uidUser		= privateData.uidUser;
		header.ip 			= publicIp;

#if UNITY_EDITOR || HIDDEN_LOGIN_TEST
		header.loginType 	= editorLogin;
		header.token		= privateData.passcode;
#else
		header.token		= HiveManager.Inst.PlayerInfo.playerToken;
#endif
		return Util.JsonEncode2(header);
	}

	// header + param
	private string GetPacketString(string header, string param)
	{
		if (string.IsNullOrEmpty(param))
			return header;
		StringBuilder stringBuilder = Giant.Util.GetStringBuilder();
		stringBuilder.Append(header, 0, header.Length - 1);
		stringBuilder.AppendFormat(",\"param\":{0}{1}", param, '}');
		return stringBuilder.ToString();
	}

	// server list
	public STRequest doServerList()
	{
		return Request(GetGuideHeaderString("req", "serverList"));
	}

	// login
	struct LoginParam
	{
		public Int64			idPlayer;
		public string			idApp;
		public string			did;
		public string			playerToken;
		public string 			market;
		public string 			deviceName;
	}

	public STRequest doLogin()
	{
		LoginParam param	= new LoginParam();

		hive.Logger.log("_playerInfo = " + HiveManager.Inst.PlayerInfo.toString() + "\n");

		param.playerToken 	= HiveManager.Inst.PlayerInfo.playerToken;
		param.idPlayer 		= HiveManager.Inst.PlayerInfo.playerId;
		param.did 			= HiveManager.Inst.PlayerInfo.did;
		param.idApp 		= Application.identifier;
		param.deviceName	= SystemInfo.deviceModel;
		param.market 		= GetMarket();

		ctx.CreateSeqToken();

		string json = GetPacketString(GetGuideHeaderString("req", "login"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doHiddenLogin()
	{
		ctx.CreateSeqToken();

		string json = GetPacketString(GetUserHeaderString("req", "hiddenLogin"), null);
		return Request(json);
	}

	public STRequest doGetServerTime()
	{
		string json = GetPacketString(GetGuideHeaderString("req", "getServerTime"), null);
		return Request(json);
	}

	public STRequest doResetAccount()
	{
		string json = GetPacketString(GetUserHeaderString("req", "initUserData"), null);
#if UNITY_EDITOR || HIDDEN_LOGIN_TEST
		return Request(json).SetSuccessAction(CreateHiddenLoginToken);
#else
		return Request(json);
#endif
	}

	// general
	struct GetUserParam
	{
		public string			platform;
		public int 				isGuest;
		public string			local;
		public string			version;
	}

	public STRequest doGetUser()
	{
		GetUserParam param	= new GetUserParam();
		param.platform 		= GetPlatform();
		param.isGuest 		= HiveManager.Inst.IsGuestAccount() ? 1 : 0;

		// TODO : Get Locale
		param.version		= Application.version;
		param.local			= "KR";

		string json = GetPacketString(GetUserHeaderString("req", "getUser"), Util.JsonEncode2(param));
		return Request(json);
	}

	struct CheckUserParam
	{
		public long idPlayer;
	}

	public STRequest doCheckUserData(long idPlayer)
	{
		CheckUserParam param = new CheckUserParam();
		param.idPlayer = idPlayer;

		string json = GetPacketString(GetGuideHeaderString("req", "checkUserData"), Util.JsonEncode2(param));
		return Request(json);
	}

	// change nickname
	struct ChangeNickname
	{
		public string nickname;
	}

	public STRequest doChangeNickname(string nickname)
	{
		ChangeNickname param = new ChangeNickname();
		param.nickname = nickname;

		string json = GetPacketString(GetUserHeaderString("req", "userNickname"), Util.JsonEncode2(param));
		return Request(json);
	}

	// Guild
	struct GuildParam
	{
		public string gid;
	}

	struct CreateGuild
	{
		public string name;
		public string lang;
	}

	struct SignUpGuild
	{
		public string gid;
		public string msg;
	}

	struct SuggestGuild
	{
		public string lang;
	}

	struct SearchGuild
	{
		public string name;
	}

	struct ChangeAuthority
	{
		public string gid;
		public long targetUid;
		public int targetAuthority;
	}

	struct ExileMember
	{
		public string gid;
		public long targetUid;
	}

	struct GuildDonate
	{
		public string gid;
		public int idItem;
		public long donationNum;
		public int isCash;
	}

	struct ChangeGuildSetting
	{
		public string gid;
		public string introduction;
		public string rule;
		public int joinLevel;
		public int joinMethod;
	}

	struct GuildBuyItem
	{
		public string gid;
		public int shopKey;
		public int count;
	}

	struct GuildUpgradeBuilding
	{
		public string gid;
		public int idBuilding;
	}

	public STRequest doBuyItemGuildShop(int shopKey, int count)
	{
		GuildBuyItem param 		= new GuildBuyItem();
		param.gid 				= GetGuildID();
		param.shopKey 			= shopKey;
		param.count 			= count;

		string json = GetPacketString(GetUserHeaderString("req", "buyItemInGuildShop"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doUpgradeGuildBuilding(int idBuilding)
	{
		GuildUpgradeBuilding param 		= new GuildUpgradeBuilding();
		param.gid 						= GetGuildID();
		param.idBuilding 				= idBuilding;

		string json = GetPacketString(GetUserHeaderString("req", "upgradeGuildBuilding"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doGetGuildData()
	{
		string json = GetPacketString(GetUserHeaderString("req", "getGuildData"), null);
		return Request(json);
	}

	public STRequest doGetGuildInfo(string gid)
	{
		GuildParam param 		= new GuildParam();
		param.gid 				= gid;

		string json = GetPacketString(GetUserHeaderString("req", "getGuildInfo"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doDonateGuild(int idItem, int donationNum, int isCash)
	{
		GuildDonate param 		= new GuildDonate();
		param.gid 				= GetGuildID();
		param.idItem 			= idItem;
		param.donationNum 		= donationNum;
		param.isCash 			= isCash;

		string json = GetPacketString(GetUserHeaderString("req", "donateStockForGuild"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doCreateGuild(string guildName)
	{
		CreateGuild param 		= new CreateGuild();
		param.name 				= guildName;
		param.lang 				= DeviceSaveManager.Inst.GetLanguage();

		string json = GetPacketString(GetUserHeaderString("req", "createGuild"), Util.JsonEncode2(param));
		return Request(json);	
	}

	public STRequest doSecedeGuild(string gid)
	{
		GuildParam param 		= new GuildParam();
		param.gid 				= gid;

		string json = GetPacketString(GetUserHeaderString("req", "secedeGuild"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doSignUpGuild(string gid, string msg)
	{
		SignUpGuild param 		= new SignUpGuild();
		param.gid			 	= gid;
		param.msg 				= msg;

		string json = GetPacketString(GetUserHeaderString("req", "reqSignUpGuild"), Util.JsonEncode2(param));
		return Request(json);	
	}

	public STRequest doChangeAuthority(string gid, long targetUid, int targetAuthority)
	{
		ChangeAuthority param 	= new ChangeAuthority();
		param.gid 				= gid;
		param.targetUid 		= targetUid;
		param.targetAuthority 	= targetAuthority;

		string json = GetPacketString(GetUserHeaderString("req", "changeAuthority"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doExileMember(string gid, long targetUid)
	{
		ExileMember param 		= new ExileMember();
		param.gid 				= gid;
		param.targetUid 		= targetUid;

		string json = GetPacketString(GetUserHeaderString("req", "exileMember"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doChangeGuildSetting(string gid, string intro, string rule, int joinLevel, int joinMethod)
	{
		ChangeGuildSetting param 	= new ChangeGuildSetting();
		param.gid 					= gid;
		param.introduction 			= intro;
		param.rule 					= rule;
		param.joinLevel 			= joinLevel;
		param.joinMethod 			= joinMethod;

		string json = GetPacketString(GetUserHeaderString("req", "changeGuildSetting"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doSuggestGuild()
	{
		SuggestGuild param 		= new SuggestGuild();
		param.lang 				= DeviceSaveManager.Inst.GetLanguage();

		string json = GetPacketString(GetUserHeaderString("req", "suggestGuild"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doSearchGuild(string name)
	{
		SearchGuild param 		= new SearchGuild();
		param.name 				= name;

		string json = GetPacketString(GetUserHeaderString("req", "searchGuild"), Util.JsonEncode2(param));
		return Request(json);
	}

	struct SetGiantInSlotParam
	{
		public string gid;
		public int idSlot;
		public int idGiant;
		public List<int> towerList;
	}

	public STRequest doSetGiantInSlot(int idSlot, int idGiant, List<int> towerList)
	{
		SetGiantInSlotParam param = new SetGiantInSlotParam();
		param.gid				= GetGuildID();
		param.idSlot			= idSlot;
		param.idGiant			= idGiant;
		param.towerList			= towerList;

		string json = GetPacketString(GetUserHeaderString("req", "setGiantInSlot"), Util.JsonEncode2(param));
		return Request(json);
	}

	struct RemoveGiantInSlotParam
	{
		public string gid;
		public int idSlot;
	}

	public STRequest doRemoveGiantInSlotSkill(int idSlot)
	{
		RemoveGiantInSlotParam param = new RemoveGiantInSlotParam();
		param.gid				= GetGuildID();
		param.idSlot			= idSlot;

		string json = GetPacketString(GetUserHeaderString("req", "removeGiantInSlot"), Util.JsonEncode2(param));
		return Request(json);
	}

	struct ModifyGiantOrderInSlotParam
	{
		public string gid;
		public Dictionary<int, long> changeOrder;
	}

	public STRequest doModifyGiantOrderInSlot(Dictionary<int, long> changeOrder)
	{
		ModifyGiantOrderInSlotParam param = new ModifyGiantOrderInSlotParam();
		param.gid				= GetGuildID();
		param.changeOrder		= changeOrder;

		string json = GetPacketString(GetUserHeaderString("req", "modifyGiantOrderInSlot"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doCompleteOrganizeGuildBattle()
	{
		GuildParam param		= new GuildParam();
		param.gid				= GetGuildID();

		string json = GetPacketString(GetUserHeaderString("req", "completeOrganizeGuildBattle"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doReOrganizeGuildBattle()
	{
		GuildParam param		= new GuildParam();
		param.gid				= GetGuildID();

		string json = GetPacketString(GetUserHeaderString("req", "reOrganizeGuildBattle"), Util.JsonEncode2(param));
		return Request(json);
	}
	
	public STRequest doGetTargetGuildList()
	{
		GuildParam param		= new GuildParam();
		param.gid				= GetGuildID();

		string json = GetPacketString(GetUserHeaderString("req", "getTargetGuildList"), Util.JsonEncode2(param));
		return Request(json);
	}
		
	struct TargetGuildParam
	{
		public string gid;
		public string targetGid;
	}

	public STRequest doDeleteShareTargetGuild(string targetGid)
	{
		TargetGuildParam param = new TargetGuildParam();
		param.gid				= GetGuildID();
		param.targetGid			= targetGid;

		string json = GetPacketString(GetUserHeaderString("req", "deleteShareTargetGuild"), Util.JsonEncode2(param));
		return Request(json);
	}
	
	public STRequest doGetTargetGuildDetailInfo(string targetGid)
	{
		TargetGuildParam param = new TargetGuildParam();
		param.gid				= GetGuildID();
		param.targetGid			= targetGid;

		string json = GetPacketString(GetUserHeaderString("req", "getTargetGuildDetailInfo"), Util.JsonEncode2(param));
		return Request(json);
	}

	struct ShareTargetGuildParam
	{
		public string gid;
		public string targetGid;
		public int idSearch;
		public int epNum;
	}

	public STRequest doShareTargetGuild(string targetGid, int idSearch, int epNum)
	{
		ShareTargetGuildParam param = new ShareTargetGuildParam();
		param.gid				= GetGuildID();
		param.targetGid			= targetGid;
		param.idSearch			= idSearch;
		param.epNum				= epNum;

		string json = GetPacketString(GetUserHeaderString("req", "shareTargetGuild"), Util.JsonEncode2(param));
		return Request(json);
	}

	struct GBResearchSkillParam
	{
		public string gid;
		public int idSkill;
	}

	public STRequest doUpgradeGBResearchSkill(int idSkill)
	{
		GBResearchSkillParam param = new GBResearchSkillParam();
		param.gid				= GetGuildID();
		param.idSkill			= idSkill;

		string json = GetPacketString(GetUserHeaderString("req", "upgradeGBResearchSkill"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doGetGuildBattleLog()
	{
		GuildParam param		= new GuildParam();
		param.gid				= GetGuildID();

		string json = GetPacketString(GetUserHeaderString("req", "getGuildBattleLog"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doGetGuildBattleFullyLog()
	{
		GuildParam param		= new GuildParam();
		param.gid				= GetGuildID();

		string json = GetPacketString(GetUserHeaderString("req", "getGuildBattleFullyLog"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doGetGuildBattleDefenseLog(string targetGid)
	{
		GuildParam param		= new GuildParam();
		param.gid				= targetGid;

		string json = GetPacketString(GetUserHeaderString("req", "getGuildBattleDefenseLog"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doGuildBattleRanking()
	{
		string json = GetPacketString(GetUserHeaderString("req", "guildBattleRanking"), null);
		return Request(json);
	}

	// Guild Chat

	struct GetReqInfo
	{
		public string reqId;
	}

	struct GuildMemberParam
	{
		public string gid;
		public string reqId;
	}

	public STRequest doGetReqUser(string idReq)
	{
		GetReqInfo param = new GetReqInfo();
		param.reqId = idReq;

		string json = GetPacketString(GetUserHeaderString("req", "getReqUser"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doAcceptMember(string idReq)
	{
		GuildMemberParam param 	= new GuildMemberParam();
		param.gid 			= GetGuildID();
		param.reqId 		= idReq;

		string json = GetPacketString(GetUserHeaderString("req", "acceptMember"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doDenyMemberReq(string idReq)
	{
		GuildMemberParam param = new GuildMemberParam();
		param.gid 			= GetGuildID();
		param.reqId 		= idReq;

		string json = GetPacketString(GetUserHeaderString("req", "denyMemberReq"), Util.JsonEncode2(param));
		return Request(json);
	}

	// Research
	struct ResearchParam
	{
		public int idResearchSkill;
		public int isCash;
	}

	public STRequest doUpgradeResearch(int researchID, int isCash)
	{
		ResearchParam param = new ResearchParam();
		param.idResearchSkill = researchID;
		param.isCash = isCash;

		string json = GetPacketString(GetUserHeaderString("req", "upgradeResearch"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doInstantlyCompleteResearch()
	{
		string json = GetPacketString(GetUserHeaderString("req", "instantlyCompleteResearch"), null);
		return Request(json);
	}

	public STRequest doCompleteResearch()
	{
		string json = GetPacketString(GetUserHeaderString("req", "completeResearch"), null);
		return Request(json);
	}

	public STRequest doGetBase()
	{
		string json = GetPacketString(GetUserHeaderString("req", "getBase"), null);
		return Request(json);
	}

	public STRequest doGetBaseResource()
	{
		string json = GetPacketString(GetUserHeaderString("req", "getBaseResource"), null);
		return Request(json, false);
	}
	
	// explore
	struct OpenAreaParam
	{
		public int idSearch;
	}
	
	struct SearchAreaParam
	{
		public int idSearch;
	}
	
	struct SearchDungeonParam
	{
		public int idSearch;
		public bool reset;
	}
	
	struct ResetDungeonParam
	{
		public int idSearch;
	}
	
	struct ClearDungeonTargetParam
	{
		public int idSearch;
		public int idFloor;
	}

	struct AreaEpParam
	{
		public int idSearch;
		public int epNum;
	}

	struct SuccEpParam
	{
		public int idSearch;
		public int epNum;
	}

	struct SuccTreasuresParam
	{
		public int idSearch;
		public int[] epNumList;
	}

	struct SetPvpDeck
	{
		public List<int> pvpDeckList;
	}

	struct StartPvpHero
	{
		public int idSearch;
		public int epNum;
	}
	
	struct ResultPvpHero
	{
		public int idSearch;
		public int epNum;
		public int battleResult;
		public Dictionary<int, int> userPvpHeroList;
		public Dictionary<int, int> enemyPvpHeroList;
		public Dictionary<int, List<int>> saveDeck;
	}
	
	struct RevengePvpHero
	{
		public long uidLog;
	}

	struct DeckInfoParam
	{
		public long uidUser;
	}

	struct ResultRevengePvpHero
	{
		public long uidLog;
		public int battleResult;
		public List<int> userPvpHeroList;
		public Dictionary<int, List<int>> saveDeck;
	}
	
	struct ResultRaceBattleParam
	{
		public int idSearch;
		public int epNum;
		public int point;
	}

	struct PvpHeroRankingParam
	{
		public int reqBeforeSeason;
	}

	struct SuccBattleEpParam
	{
		public int idSearch;
		public int epNum;
		public int stageStar;
		public List<int> party;
		public Dictionary<int, List<int>> saveDeck;
	}

	struct FailBattleEpParam
	{
		public int idSearch;
		public int epNum;
		public Dictionary<int, List<int>> saveDeck;
	}

	struct SuccGatherEpParam
	{
		public int idSearch;
		public int epNum;
		public int idHero;
	}
	
	struct BaseRaidParam
	{
		public int idSearch;
		public int epNum;
		public float hpPercent;
		public Dictionary<int, List<int>> saveDeck;
	}

	struct RevengeRaidParam
	{
		public float hpPercent;
		public Dictionary<int, List<int>> saveDeck;
	}

	struct StartWorldBossParam
	{
		public int idSearch;
		public int epNum;
	}

	struct ResultWorldBossParam
	{
		public int idSearch;
		public int epNum;
		public long damage;
		public int[] idHeros;
		public long [] damageLog;
		public Dictionary<int, List<int>> saveDeck;
	}

	struct StartGuildBattleParam
	{
		public string gid;
		public string targetGid;
		public int idSlot;
		public List<int> heroList;
	}

	struct ResultGuildBattleParam
	{
		public string gid;
		public string targetGid;
		public int idSlot;
		public long damage;
		public Dictionary<int, List<int>> saveDeck;
	}

	struct DuelRequestParam
	{
		public int isCash;
		public int[] idHeros;
	}

	struct DuelResultParam
	{
		public int result;
		public Dictionary<int, List<int>> saveDeck;
	}

	struct BuyEpMerchantParam
	{
		public int idSearch;
		public int epNum;
		public int slotNum;
		public int itemNum;
	}

	struct AssignComplete
	{
		public List<int> heroList;
	}

	struct NPCAssignParam
	{
		public int idSearch;
		public int epNum;
		public int craftType;
		public int idHero;
		public int craftCount;
		public int idItem;
		public int slotNum;
	}

	struct NPCInscriptionParam
	{
		public int idSearch;
		public int epNum;
		public int craftType;
		public int idHero;
		public int slotNum;
	}

	struct NPCAssignCompleteParam
	{
		public int idHero;
		public int craftType;
	}

	public STRequest doOpenArea(int idSearch)
	{
		OpenAreaParam param = new OpenAreaParam();
		param.idSearch	= idSearch;

		string json = GetPacketString(GetUserHeaderString("req", "openArea"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doSearchArea(int idSearch)
	{
		SearchAreaParam param = new SearchAreaParam();
		param.idSearch			= idSearch;

		string json = GetPacketString(GetUserHeaderString("req", "searchArea"), Util.JsonEncode2(param));
		return Request(json);
	}
	
	public STRequest doSearchDungeon(int idSearch, bool isReset)
	{
		SearchDungeonParam param = new SearchDungeonParam();
		param.idSearch			= idSearch;
		param.reset				= isReset;

		string json = GetPacketString(GetUserHeaderString("req", "searchDungeon"), Util.JsonEncode2(param));
		return Request(json);
	}
	
	public STRequest doClearDungeonTarget(int idSearch, int idFloor)
	{
		ClearDungeonTargetParam param = new ClearDungeonTargetParam();
		param.idSearch			= idSearch;
		param.idFloor			= idFloor;

		string json = GetPacketString(GetUserHeaderString("req", "clearDungeonTarget"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doSuccEp(int idSearch, int epNum)
	{
		SuccEpParam param	= new SuccEpParam();
		param.idSearch 		= idSearch;
		param.epNum			= epNum;
		
		string json = GetPacketString(GetUserHeaderString("req", "succEp"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doSuccBattleEp(int idSearch, int epNum, int stageStar, List<int> party, DECK_TYPE deckType, int [] deck, int deckCount)
	{
		SuccBattleEpParam param	= new SuccBattleEpParam();
		param.idSearch 		= idSearch;
		param.epNum			= epNum;
		param.stageStar		= stageStar;
		param.party			= party;

		List<int> saveDeck = AccountDataStore.instance.ChangeSaveDeck(deckType, deck, deckCount);
		if (saveDeck != null)
		{
			param.saveDeck	= new Dictionary<int, List<int>>();
			param.saveDeck.Add((int)deckType, saveDeck);
		}

		string json = GetPacketString(GetUserHeaderString("req", "succEp"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doFailBattleEp(int idSearch, int epNum, DECK_TYPE deckType, int [] deck, int deckCount)
	{
		FailBattleEpParam param	= new FailBattleEpParam();
		param.idSearch 		= idSearch;
		param.epNum			= epNum;

		List<int> saveDeck = AccountDataStore.instance.ChangeSaveDeck(deckType, deck, deckCount);
		if (saveDeck != null)
		{
			param.saveDeck	= new Dictionary<int, List<int>>();
			param.saveDeck.Add((int)deckType, saveDeck);
		}

		string json = GetPacketString(GetUserHeaderString("req", "failEp"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doSuccGatherEp(int idSearch, int epNum, int idHero)
	{
		SuccGatherEpParam param	= new SuccGatherEpParam();
		param.idSearch 	= idSearch;
		param.epNum		= epNum;
		param.idHero	= idHero;

		string json = GetPacketString(GetUserHeaderString("req", "succEp"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doSuccTreasures(int idSearch, int[] epNumList)
	{
		SuccTreasuresParam param 	= new SuccTreasuresParam();
		param.idSearch 				= idSearch;
		param.epNumList 			= epNumList;

		string json = GetPacketString(GetUserHeaderString("req", "succTreasures"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doFailEp(int idSearch, int epNum)
	{
		AreaEpParam param	= new AreaEpParam();
		param.idSearch 	= idSearch;
		param.epNum		= epNum;
		
		string json = GetPacketString(GetUserHeaderString("req", "failEp"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doRequestBaseRaid(int idSearch, int epNum)
	{
		AreaEpParam param	= new AreaEpParam();
		param.idSearch 		= idSearch;
		param.epNum			= epNum;

		string json = GetPacketString(GetUserHeaderString("req", "requestBaseRaid"), Util.JsonEncode2(param));
		return Request(json);
	}

	// hpPercent : 깎은 hp 퍼센트값 (0~1 사이의 값)
	public STRequest doResultBaseRaid(int idSearch, int epNum, bool isWin)
	{
		BaseRaidParam param		= new BaseRaidParam();
		param.idSearch			= idSearch;
		param.epNum				= epNum;
		param.hpPercent			= isWin ? 1f : 0f;

		string json = GetPacketString(GetUserHeaderString("req", "resultBaseRaid"), Util.JsonEncode2(param));
		return Request(json);
	}
	
	public STRequest doResultBaseRaid(int idSearch, int epNum, float hpPercent, DECK_TYPE deckType , int [] deck, int deckCount)
	{
		BaseRaidParam param		= new BaseRaidParam();
		param.idSearch			= idSearch;
		param.epNum				= epNum;
		param.hpPercent			= hpPercent;

		List<int> saveDeck = AccountDataStore.instance.ChangeSaveDeck(deckType, deck, deckCount);
		if (saveDeck != null)
		{
			param.saveDeck		= new Dictionary<int, List<int>>();
			param.saveDeck.Add((int)deckType, saveDeck);
		}

		string json = GetPacketString(GetUserHeaderString("req", "resultBaseRaid"), Util.JsonEncode2(param));
		return Request(json);
	}
	
	public STRequest doResultRevengeRaid(bool isWin)
	{
		RevengeRaidParam param 	= new RevengeRaidParam();
		param.hpPercent			= isWin ? 1f : 0f;

		string json = GetPacketString(GetUserHeaderString("req", "resultRevengeRaid"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doResultRevengeRaid(float hpPercent, DECK_TYPE deckType, int [] deck, int deckCount)
	{
		RevengeRaidParam param	= new RevengeRaidParam();
		param.hpPercent			= hpPercent;

		List<int> saveDeck = AccountDataStore.instance.ChangeSaveDeck(deckType, deck, deckCount);
		if (saveDeck != null)
		{
			param.saveDeck		= new Dictionary<int, List<int>>();
			param.saveDeck.Add((int)deckType, saveDeck);
		}

		string json = GetPacketString(GetUserHeaderString("req", "resultRevengeRaid"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doStartWorldBoss(int idSearch, int epNum)
	{
		StartWorldBossParam param	= new StartWorldBossParam();
		param.idSearch				= idSearch;
		param.epNum					= epNum;

		string json = GetPacketString(GetUserHeaderString("req", "startWorldBoss"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doResultWorldBoss(int idSearch, int epNum, long damage, int [] idHeros, long [] damageLogs, DECK_TYPE deckType, int [] deck, int deckCount)
	{
		ResultWorldBossParam param	= new ResultWorldBossParam();
		param.idSearch				= idSearch;
		param.epNum					= epNum;
		param.damage				= damage;
		param.idHeros				= idHeros;
		param.damageLog				= damageLogs;

		List<int> saveDeck = AccountDataStore.instance.ChangeSaveDeck(deckType, deck, deckCount);
		if (saveDeck != null)
		{
			param.saveDeck			= new Dictionary<int, List<int>>();
			param.saveDeck.Add((int)deckType, saveDeck);
		}

		string json = GetPacketString(GetUserHeaderString("req", "resultWorldBoss"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doStartGuildBattle(string targetGid, int idSlot, List<int> heroList)
	{
		StartGuildBattleParam param	= new StartGuildBattleParam();
		param.gid					= GetGuildID();
		param.targetGid				= targetGid;
		param.idSlot				= idSlot;
		param.heroList				= heroList;

		string json = GetPacketString(GetUserHeaderString("req", "startGuildBattle"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doResultGuildBattle(string targetGid, int idSlot, long damage, DECK_TYPE deckType, int [] deck, int deckCount)
	{
		ResultGuildBattleParam param	= new ResultGuildBattleParam();
		param.gid					= GetGuildID();
		param.targetGid				= targetGid;
		param.idSlot				= idSlot;
		param.damage				= damage;

		List<int> saveDeck = AccountDataStore.instance.ChangeSaveDeck(deckType, deck, deckCount);
		if (saveDeck != null)
		{
			param.saveDeck			= new Dictionary<int, List<int>>();
			param.saveDeck.Add((int)deckType, saveDeck);
		}

		string json = GetPacketString(GetUserHeaderString("req", "resultGuildBattle"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doDuelRequest(bool isCash, int [] idHeros)
	{
		DuelRequestParam param		= new DuelRequestParam();
		param.isCash				= isCash ? 1 : 0;
		param.idHeros				= idHeros;

		string json = GetPacketString(GetUserHeaderString("req", "duelRequest"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doDuelResult(int result, DECK_TYPE deckType, int [] deck, int deckCount)
	{
		DuelResultParam param		= new DuelResultParam();
		param.result				= result;

		List<int> saveDeck = AccountDataStore.instance.ChangeSaveDeck(deckType, deck, deckCount);
		if (saveDeck != null)
		{
			param.saveDeck			= new Dictionary<int, List<int>>();
			param.saveDeck.Add((int)deckType, saveDeck);
		}

		string json = GetPacketString(GetUserHeaderString("req", "duelResult"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doDuelDailyReward()
	{
		string json = GetPacketString(GetUserHeaderString("req", "duelDailyReward"), null);
		return Request(json);
	}

	public STRequest doRequestPvpHeroLog()
	{
		string json = GetPacketString(GetUserHeaderString("req", "requestPvpHeroLog"), null);
		return Request(json);
	}

	public STRequest doRequestPvpHeroRanking()
	{
		PvpHeroRankingParam param = new PvpHeroRankingParam();

		if(AccountDataStore.instance.pvpSeasonInfo.lastWeekRankRefresh)
			param.reqBeforeSeason = 1;	

		string json = GetPacketString(GetUserHeaderString("req", "pvpHeroRanking"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doSetPvpDeck(List<int>deckList)
	{
		SetPvpDeck param = new SetPvpDeck();
		param.pvpDeckList = deckList;

		string json = GetPacketString(GetUserHeaderString("req", "setPvpDeck"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doStartPvpHero(int idSearch, int epNum)
	{
		StartPvpHero param	= new StartPvpHero();
		param.idSearch		= idSearch;
		param.epNum			= epNum;

		string json = GetPacketString(GetUserHeaderString("req", "startPvpHero"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doResultPvpHero(int idSearch, int epNum, bool isWin, Dictionary<int, int> heroDic, Dictionary<int, int> enemyDic, DECK_TYPE deckType, int [] deck, int deckCount)
	{
		ResultPvpHero param			= new ResultPvpHero();
		param.idSearch				= idSearch;
		param.epNum					= epNum;
		param.battleResult 			= isWin ? 1 : 0;
		param.userPvpHeroList	 	= heroDic;
		param.enemyPvpHeroList 		= enemyDic;

		List<int> saveDeck = AccountDataStore.instance.ChangeSaveDeck(deckType, deck, deckCount);
		if (saveDeck != null)
		{
			param.saveDeck			= new Dictionary<int, List<int>>();
			param.saveDeck.Add((int)deckType, saveDeck);
		}
		
		string json = GetPacketString(GetUserHeaderString("req", "resultPvpHero"), Util.JsonEncode2(param));
		return Request(json);
	}
	
	public STRequest doRequestRevengePvpHero(long uidLog)
	{
		RevengePvpHero param		= new RevengePvpHero();
		param.uidLog				= uidLog;

		string json = GetPacketString(GetUserHeaderString("req", "requestRevengePvpHero"), Util.JsonEncode2(param));
		return Request(json);
	}
	
	public STRequest doStartRevengePvpHero(long uidLog)
	{
		RevengePvpHero param		= new RevengePvpHero();
		param.uidLog				= uidLog;
		
		string json = GetPacketString(GetUserHeaderString("req", "startRevengePvpHero"), Util.JsonEncode2(param));
		return Request(json).SetSuccessAction(() => {
			PvpUserLog pvpUserLog = AccountDataStore.instance.GetPvpDefenseLog(uidLog);
			if (pvpUserLog != null)
				pvpUserLog.StartRevenge();
		});
	}

	public STRequest doResultRevengePvpHero(long uidLog, bool isWin, List<int> userPvpHeroList, DECK_TYPE deckType, int [] deck, int deckCount)
	{
		ResultRevengePvpHero param	= new ResultRevengePvpHero();
		param.uidLog				= uidLog;
		param.battleResult			= isWin ? 1 : 0;
		param.userPvpHeroList		= userPvpHeroList;

		List<int> saveDeck = AccountDataStore.instance.ChangeSaveDeck(deckType, deck, deckCount);
		if (saveDeck != null)
		{
			param.saveDeck			= new Dictionary<int, List<int>>();
			param.saveDeck.Add((int)deckType, saveDeck);
		}
		
		string json = GetPacketString(GetUserHeaderString("req", "resultRevengePvpHero"), Util.JsonEncode2(param));
		return Request(json).SetSuccessAction(() => {
			PvpUserLog pvpUserLog = AccountDataStore.instance.GetPvpDefenseLog(uidLog);
			if (pvpUserLog != null)
				pvpUserLog.ResultRevenge(isWin);
		});
	}
	
	public STRequest doResultRaceBattle(int idSearch, int epNum, int point)
	{
		ResultRaceBattleParam param	= new ResultRaceBattleParam();
		param.idSearch				= idSearch;
		param.epNum					= epNum;
		param.point					= point;

		string json = GetPacketString(GetUserHeaderString("req", "resultRaceBattle"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doPvpHeroSeasonInfo()
	{
		string json = GetPacketString(GetUserHeaderString("req", "pvpHeroSeasonInfo"), null);	
		return Request(json);
	}

	public STRequest doRequestPvpWeeklyReward()
	{
		string json = GetPacketString(GetUserHeaderString("req", "pvpHeroSeasonReward"), null);	
		return Request(json);
	}

	public STRequest doPvpHeroDeckInfo(long uidUser)
	{
		DeckInfoParam param = new DeckInfoParam();
		param.uidUser = uidUser; 

		string json = GetPacketString(GetUserHeaderString("req", "pvpHeroDeckInfo"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doArenaSeasonInfo()
	{
		string json = GetPacketString(GetUserHeaderString("req", "arenaSeasonInfo"), null);	
		return Request(json);
	}

	public STRequest doArenaSeasonReward()
	{
		string json = GetPacketString(GetUserHeaderString("req", "arenaSeasonReward"), null);	
		return Request(json);
	}

	public STRequest doArenaRanking()
	{
		string json = GetPacketString(GetUserHeaderString("req", "arenaRanking"), null);
		return Request(json);
	}

	public STRequest doArenaDeckInfo(long uidUser)
	{
		DeckInfoParam param = new DeckInfoParam();
		param.uidUser = uidUser; 

		string json = GetPacketString(GetUserHeaderString("req", "arenaDeckInfo"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doDuelSeasonInfo()
	{
		string json = GetPacketString(GetUserHeaderString("req", "duelSeasonInfo"), null);	
		return Request(json);
	}

	public STRequest doDuelSeasonReward()
	{
		string json = GetPacketString(GetUserHeaderString("req", "duelSeasonReward"), null);	
		return Request(json);
	}

	public STRequest doDuelRanking()
	{
		string json = GetPacketString(GetUserHeaderString("req", "duelRanking"), null);
		return Request(json);
	}

	public STRequest doDuelDeckInfo(long uidUser)
	{
		DeckInfoParam param = new DeckInfoParam();
		param.uidUser = uidUser; 

		string json = GetPacketString(GetUserHeaderString("req", "duelDeckInfo"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doDuelTakeReward()
	{
		string json = GetPacketString(GetUserHeaderString("req", "duelTakeReward"), null);
		return Request(json);
	}

	public STRequest doStartBattle(int idSearch, int epNum)
	{
		AreaEpParam param	= new AreaEpParam();
		param.idSearch		= idSearch;
		param.epNum			= epNum;

		string json = GetPacketString(GetUserHeaderString("req", "startBattle"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doStartRevengeRaid(long uidBaseRaid)
	{
		RevengeRaidInfoParam param	= new RevengeRaidInfoParam();
		param.uidBaseRaid = uidBaseRaid;

		string json = GetPacketString(GetUserHeaderString("req", "startRevengeRaid"), Util.JsonEncode2(param));
		return Request(json);
	}

	struct CheckStatParam
	{
		public int idHero;
		public int content;				// CONTENT_TYPE
		public GiantCheckStat stat;
		public Dictionary<int, float> buffEvent;
		public float statRate;			// Default : 1f
		public int idWorldBossSeason;
	}

	public STRequest doCheckStat(int idHero, CONTENT_TYPE contentType, GiantCheckStat stat, Dictionary<int, float> buffEvent, float statRate, int idWorldBossSeason)
	{
		CheckStatParam param = new CheckStatParam();
		param.idHero				= idHero;
		param.content				= (int)contentType;
		param.stat					= stat;
		param.buffEvent				= buffEvent;
		param.statRate				= statRate;
		param.idWorldBossSeason		= idWorldBossSeason;

		string json = GetPacketString(GetUserHeaderString("req", "checkStat"), Util.JsonEncode2(param));
		return SilentRequest(json);
	}

	struct CheckSkillParam
	{
		public int idHero;
		public int idFunction;
		public GiantSkillSplitMini hit;
		public GiantCheckStat casterStat;
		public GiantCheckStat targetStat;
	}

	public STRequest doCheckSkill(int idHero, int idFunction, GiantSkillSplitMini hit, GiantCheckStat casterStat, GiantCheckStat targetStat)
	{
		CheckSkillParam param = new CheckSkillParam();
		param.idHero				= idHero;
		param.idFunction			= idFunction;
		param.hit					= hit;
		param.casterStat			= casterStat;
		param.targetStat			= targetStat;

		string json = GetPacketString(GetUserHeaderString("req", "checkSkill"), Util.JsonEncode2(param));
		return SilentRequest(json);
	}

	public STRequest doBuyEpMerchant(int idSearch, int epNum, int slotNum, int count)
	{
		BuyEpMerchantParam param	= new BuyEpMerchantParam();
		param.idSearch 	= idSearch;
		param.epNum		= epNum;
		param.slotNum	= slotNum;
		param.itemNum	= count;
		
		string json = GetPacketString(GetUserHeaderString("req", "buyEpMerchant"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doCompleteAssign(List<int> heroList)
	{
		AssignComplete param = new AssignComplete();
		param.heroList = heroList;

		string json = GetPacketString(GetUserHeaderString("req", "completeAssign"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doInstantlyCompleteAssign(int idHero)
	{
		HeroParam param		= new HeroParam();
		param.idHero		= idHero;

		string json = GetPacketString(GetUserHeaderString("req", "instantlyCompleteAssign"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doCancelAssign(int idHero)
	{
		HeroParam param		= new HeroParam();
		param.idHero		= idHero;

		string json = GetPacketString(GetUserHeaderString("req", "cancelAssign"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doCraftAssign(int idSearch, int epNum, int craftType, int idHero, int craftCount, int idItem)
	{
		NPCAssignParam param = new NPCAssignParam();
		param.idSearch 		= idSearch;
		param.epNum 		= epNum;
		param.craftType		= craftType;
		param.idHero		= idHero;
		param.craftCount	= craftCount;
		param.idItem		= idItem;

		string json = GetPacketString(GetUserHeaderString("req", "craftAssign"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doInscriptionAssign(int idSearch, int epNum, int craftType, int idHero, int slotIndex)
	{
		NPCInscriptionParam param = new NPCInscriptionParam();
		param.idSearch 		= idSearch;
		param.epNum 		= epNum;
		param.craftType		= craftType;
		param.idHero		= idHero;
		param.slotNum		= slotIndex;

		string json = GetPacketString(GetUserHeaderString("req", "craftAssign"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doInscriptionEnchant(int idHero, int slotIndex)
	{
		NPCInscriptionParam param = new NPCInscriptionParam();
		param.idHero		= idHero;
		param.slotNum		= slotIndex;

		string json = GetPacketString(GetUserHeaderString("req", "inscriptionEnchant"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doResetInscriptionEnchant(int idHero, int slotIndex)
	{
		NPCInscriptionParam param = new NPCInscriptionParam();
		param.idHero		= idHero;
		param.slotNum		= slotIndex;

		string json = GetPacketString(GetUserHeaderString("req", "resetInscriptionEnchant"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doCheckSalesGold()
	{
		string json = GetPacketString(GetUserHeaderString("req", "checkSalesGold"), null);
		return Request(json);
	}

	public STRequest doReceiveRefillGold()
	{
		string json = GetPacketString(GetUserHeaderString("req", "receiveRefillGold"), null);
		return Request(json);
	}

	public STRequest doPurchaseDungeonToken()
	{
		string json = GetPacketString(GetUserHeaderString("req", "purchaseDungeonToken"), null);
		return Request(json);
	}

	public STRequest doExtraBossCountReset()
	{
		string json = GetPacketString(GetUserHeaderString("req", "extraBossCountReset"), null);
		return Request(json);
	}

	// item
	struct CraftItemParam
	{
		public List<CraftItemData> itemList;
		public int isCash;
	}

	struct ItemEquipParam
	{
		public int idHero;
		public int equipNum;
	}

	struct ItemUse
	{
		public int idItem;
	}

	struct UseCash
	{
		public int isCash;
	}

	public STRequest doCraftItem(List<CraftItemData> craftItemDataList, int isCash)
	{
		CraftItemParam param	= new CraftItemParam();
		param.itemList			= craftItemDataList;
		param.isCash 			= isCash;
		
		string json = GetPacketString(GetUserHeaderString("req", "craftItem"), Util.JsonEncode2(param));
		return Request(json);
	}
	
	public STRequest doCraftItem(int _idItem, int _count, int isCash)
	{
		List<CraftItemData> list	= new List<CraftItemData>();
		CraftItemData data			= new CraftItemData();
		data.idItem					= _idItem;
		data.count					= _count;

		list.Add(data);
		
		return doCraftItem(list, isCash);
	}
	
	public STRequest doCraftItem(CraftMaterialItemData[] craftMaterialItemDataList, int isCash)
	{
		List<CraftItemData> list = new List<CraftItemData>();
		for (int i = craftMaterialItemDataList.Length - 1; i >= 0; --i)
		{
			CraftItemData data			= new CraftItemData();
			data.idItem					= craftMaterialItemDataList[i].ItemID;
			data.count					= craftMaterialItemDataList[i].ItemCount;
			list.Add(data);
		}

		return doCraftItem(list, isCash);
	}
	
	public STRequest doEquipItem(int _idHero, int _equipNum)
	{
		ItemEquipParam param 	= new ItemEquipParam();
		param.idHero 			= _idHero;
		param.equipNum 			= _equipNum;

		string json = GetPacketString(GetUserHeaderString("req", "equipItem"), Util.JsonEncode2(param));
		return Request(json);
	}
	
	public STRequest doEquipItemAll(int _idHero)
	{
		HeroParam param			= new HeroParam();
		param.idHero			= _idHero;
		
		string json = GetPacketString(GetUserHeaderString("req", "equipItemAll"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doUseItem(int _idItem)
	{
		ItemUse param			= new ItemUse();
		param.idItem 			= _idItem;

		string json = GetPacketString(GetUserHeaderString("req", "useItem"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doUpgradeHall(int isCash)
	{
		UseCash param = new UseCash();
		param.isCash = isCash;
		string json = GetPacketString(GetUserHeaderString("req", "upgradeHall"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doCompleteUpgradeHall()
	{
		string json = GetPacketString(GetUserHeaderString("req", "completeUpgradeHall"), null);
		return Request(json);
	}

	public STRequest doInstantlyCompleteUpgradeHall()
	{
		string json = GetPacketString(GetUserHeaderString("req", "instantlyCompleteUpgradeHall"), null);
		return Request(json);
	}

	// hero
	struct HeroParam
	{
		public int idHero;
	}

	struct EtherLevelParam
	{
		public int idHero;
		public int upLevel;
	}

	struct SkillParam
	{
		public int idHero;
		public int idSkill;
	}

	struct AllSkillParam
	{
		public int idHero;
		public Dictionary<int, int> upSkill;
	}

	struct EquipSwapParam
	{
		public int idSource;
		public int idTarget;
	}
		
	public STRequest doEquipSwap(int _idHeroSource, int _idHeroTarget)
	{
		EquipSwapParam param = new EquipSwapParam();
		param.idSource = _idHeroSource;
		param.idTarget = _idHeroTarget;

		string json = GetPacketString(GetUserHeaderString("req", "equipSwap"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doRankupHero(int _idHero)
	{
		HeroParam param = new HeroParam();
		param.idHero	= _idHero;
		
		string json = GetPacketString(GetUserHeaderString("req", "rankupHero"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doLimitBreakHero(int _idHero)
	{
		HeroParam param = new HeroParam();
		param.idHero	= _idHero;

		string json = GetPacketString(GetUserHeaderString("req", "limitBreakHero"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doEtherLevelUpHero(int _idHero, int _upLevel)
	{
		EtherLevelParam param = new EtherLevelParam();
		param.idHero	= _idHero;
		param.upLevel	= _upLevel;

		string json = GetPacketString(GetUserHeaderString("req", "etherLevelUpHero"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doSkillUpHero(int _idHero, int _idSkill)
	{
		SkillParam param = new SkillParam();
		param.idHero	= _idHero;
		param.idSkill	= _idSkill;

		string json = GetPacketString(GetUserHeaderString("req", "skillUpHero"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doAllSkillUpHero(int _idHero, Dictionary<int, int> _idSkill)
	{
		AllSkillParam param = new AllSkillParam();
		param.idHero 	= _idHero;
		param.upSkill	= _idSkill;

		string json = GetPacketString(GetUserHeaderString("req", "skillUpHeroAll"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doRecoveryGBBroken(int idHero)
	{
		HeroParam param = new HeroParam();
		param.idHero	= idHero;

		string json = GetPacketString(GetUserHeaderString("req", "recoveryGBBroken"), Util.JsonEncode2(param));
		return Request(json);
	}

	// quest
	struct SubQuestParam
	{
		public int idSubQuest;
	}

	// main quest
	struct CompleteMainQuestParam
	{
		public int difficulty;
	}

	public STRequest doCompleteMainQuest(int difficulty)
	{
		CompleteMainQuestParam param = new CompleteMainQuestParam();
		param.difficulty = difficulty;

		string json = GetPacketString(GetUserHeaderString("req", "completeMainQuest"), Util.JsonEncode2(param));
		return Request(json);
	}

	// sub quest
	public STRequest doRequestSubQuest(int idSubQuest)
	{
		SubQuestParam param	= new SubQuestParam();
		param.idSubQuest	= idSubQuest;

		string json = GetPacketString(GetUserHeaderString("req", "requestSubQuest"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doCompleteSubQuest(int idSubQuest)
	{
		SubQuestParam param	= new SubQuestParam();
		param.idSubQuest	= idSubQuest;

		string json = GetPacketString(GetUserHeaderString("req", "completeSubQuest"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doGiveupSubQuest(int idSubQuest)
	{
		SubQuestParam param	= new SubQuestParam();
		param.idSubQuest	= idSubQuest;

		string json = GetPacketString(GetUserHeaderString("req", "giveupSubQuest"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doPurchaseSubQuestSlot()
	{
		string json = GetPacketString(GetUserHeaderString("req", "purchaseSubQuestSlot"), null);
		return Request(json);
	}

	// dailyQuest
	struct DailyQuestIDParam
	{
		public int idDailyQuest;	
	}

	public STRequest doResetDailyQuest()
	{
		string json = GetPacketString(GetUserHeaderString("req", "resetDailyQuest"), null);
		return Request(json);
	}

	public STRequest doRequestDailyQuest(int idDailyQuest)
	{
		DailyQuestIDParam param = new DailyQuestIDParam();
		param.idDailyQuest = idDailyQuest;

		string json = GetPacketString(GetUserHeaderString("req", "requestDailyQuest"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doCompleteDailyQuest(int idDailyQuest)
	{
		DailyQuestIDParam param = new DailyQuestIDParam();
		param.idDailyQuest = idDailyQuest;

		string json = GetPacketString(GetUserHeaderString("req", "completeDailyQuest"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doGiveupDailyQuest(int idDailyQuest)
	{
		DailyQuestIDParam param = new DailyQuestIDParam();
		param.idDailyQuest = idDailyQuest;

		string json = GetPacketString(GetUserHeaderString("req", "giveupDailyQuest"), Util.JsonEncode2(param));
		return Request(json);
	}

	// achievement
	struct AchievementParam
	{
		public int idAchievement;	
		public int clearNum;	
	}

	public STRequest doCompleteAchievement(int achievementID, int clearNum)
	{
		AchievementParam param = new AchievementParam();
		param.idAchievement = achievementID;
		param.clearNum = clearNum;

		string json = GetPacketString(GetUserHeaderString("req", "completeAchievement"), Util.JsonEncode2(param));
		return Request(json);
	}

	// DungeonMission
	struct DungeonMissionParam
	{
		public int idMission;	
		public int level;	
	}

	public STRequest doCompleteDungeonMission(int idAchievement, int clearNum)
	{
		DungeonMissionParam param = new DungeonMissionParam();
		param.idMission = idAchievement;
		param.level = clearNum;

		string json = GetPacketString(GetUserHeaderString("req", "completeMission"), Util.JsonEncode2(param));
		return Request(json);
	}

	// event trigger
	struct EventTriggerParam
	{
		public int idEvent;
	}

	public STRequest doCompleteEvent(int idEvent)
	{
		EventTriggerParam param = new EventTriggerParam();
		param.idEvent			= idEvent;

		string json = GetPacketString(GetUserHeaderString("req", "completeEvent"), Util.JsonEncode2(param));
		return Request(json, false);
	}

	struct RaceTreasureParam
	{
		public int idSlot;
	}

	public STRequest doTakeFreeTreasureChest()
	{
		string json = GetPacketString(GetUserHeaderString("req", "takeFreeTreasureChest"), null);
		return Request(json);
	}

	public STRequest doTakeMasterLevelUpTreasureChest()
	{
		string json = GetPacketString(GetUserHeaderString("req", "takeMasterLevelUpTreasureChest"), null);
		return Request(json);
	}

	public STRequest doStartRaceBattleChest(int idSlot)
	{
		RaceTreasureParam param = new RaceTreasureParam();
		param.idSlot = idSlot;

		string json = GetPacketString(GetUserHeaderString("req", "startRaceBattleChest"), Util.JsonEncode2(param));
		return Request(json);
	}
	
	public STRequest doTakeRaceBattleChest(int idSlot)
	{
		RaceTreasureParam param = new RaceTreasureParam();
		param.idSlot = idSlot;

		string json = GetPacketString(GetUserHeaderString("req", "takeRaceBattleChest"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doInstantlyRaceBattleChest(int idSlot)
	{
		RaceTreasureParam param = new RaceTreasureParam();
		param.idSlot = idSlot;

		string json = GetPacketString(GetUserHeaderString("req", "instantlyRaceBattleChest"), Util.JsonEncode2(param));
		return Request(json);
	}

	// client data
	public STRequest doClientData(GiantClientData clientData)
	{
		string json = GetPacketString(GetUserHeaderString("req", "setClientData"), Util.JsonEncode2(clientData));
		return Request(json, false);
	}

	// purchase
	struct PurchaseGoodsParam
	{
		public int idGoods;
		public int buffFlag;		// 1 : 해당 상품의 버프 적용 여부
		public int idHero;			// Extra Soul Hero
		public string purchaseBypassInfo;
		public string platform;
		public string osVersion;
		public string version;
		public string mac_address;
		public string nationality;
		public string currency;
		public double productPrice;
		public string productName;
		public string vid;
#if UNITY_EDITOR || HIDDEN_LOGIN_TEST
		public int DebugForceSuccess;
#endif
	}

	public STRequest doPurchase(int _idGoods, bool isBuffFlag, int idHero, hive.IAPV4.IAPV4Receipt iapReceipt)
	{
		PurchaseGoodsParam param = new PurchaseGoodsParam();
		param.idGoods = _idGoods;
		param.buffFlag = isBuffFlag ? 1 : 0;
		if (_idGoods == Datatable.Inst.settingData.SurplusSoulGoodsID)
			param.idHero = idHero;
		if (iapReceipt != null)
		{
			param.purchaseBypassInfo = iapReceipt.bypassInfo;
			param.currency = iapReceipt.product.currency;
			param.productPrice = iapReceipt.product.price;
			param.productName = iapReceipt.product.title;

#if UNITY_ANDROID
			hive.IAPV4.IAPV4ReceiptGoogle iapGoogle = iapReceipt as hive.IAPV4.IAPV4ReceiptGoogle;
			if (iapGoogle != null)
				param.vid = iapGoogle.vid;
#endif
		}

#if UNITY_EDITOR || HIDDEN_LOGIN_TEST
		param.DebugForceSuccess = 1;
#else
		param.nationality = HiveManager.Inst.ConnectCountry;
#endif
		param.platform = GetPlatform();
		param.osVersion = Giant.Util.GetOSVersion();
		param.version = Application.version;

		string json = GetPacketString(GetUserHeaderString("req", "purchase"), Util.JsonEncode2(param));
		return Request(json, true, Constant.PurchaseRequestTimeout);
	}

	// AttendCheck
	public STRequest doClaimAttendCheck()
	{
		string json = GetPacketString(GetUserHeaderString("req", "claimAttendCheckReward"), null);
		return Request(json);
	}

	// ClaimGoal
	struct ClaimGoalParam
	{
		public int idGoodsGoal;
	}

	public STRequest doClaimGoal(int idGoodsGoal)
	{
		ClaimGoalParam param;
		param.idGoodsGoal = idGoodsGoal;

		string json = GetPacketString(GetUserHeaderString("req", "claimGoal"), Util.JsonEncode2(param));
		return Request(json);
	}

	// ClaimDaily
	struct ClaimDaily
	{
		public int idGoods;
	}

	public STRequest doClaimDaily(int idGoods)
	{
		ClaimDaily param;
		param.idGoods = idGoods;

		string json = GetPacketString(GetUserHeaderString("req", "claimDaily"), Util.JsonEncode2(param));
		return Request(json);
	}

	// post
	struct ReceivePostListParam
	{
		public string[] receivePostList;
	}

	struct ReceiveSelectCoreParam
	{
		public string[] receivePostList;
		public int idCore;
	}

	struct ReceiveSelectHeroParam
	{
		public string[] receivePostList;
		public int idHero;
	}

	struct ReceiveSelectItemParam
	{
		public string[] receivePostList;
		public int idItem;
	}

	public STRequest doRequestPostList()
	{
		string json = GetPacketString(GetUserHeaderString("req", "requestPostList"), null);
		return Request(json);
	}

	public STRequest doReceivePost(params string[] idPosts)
	{
		ReceivePostListParam param = new ReceivePostListParam();
		param.receivePostList = idPosts;

		string json = GetPacketString(GetUserHeaderString("req", "receivePost"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doReceiveSelectCore(int idCore, params string[] idPosts)
	{
		ReceiveSelectCoreParam param = new ReceiveSelectCoreParam();
		param.receivePostList = idPosts;
		param.idCore = idCore;

		string json = GetPacketString(GetUserHeaderString("req", "receivePost"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doReceiveSelectHero(int idHero, params string[] idPosts)
	{
		ReceiveSelectHeroParam param = new ReceiveSelectHeroParam();
		param.receivePostList = idPosts;
		param.idHero = idHero;
		
		string json = GetPacketString(GetUserHeaderString("req", "receivePost"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doReceiveSelectItem(int idItem, params string[] idPosts)
	{
		ReceiveSelectItemParam param = new ReceiveSelectItemParam();
		param.receivePostList = idPosts;
		param.idItem = idItem;

		string json = GetPacketString(GetUserHeaderString("req", "receivePost"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doSilentCheckPost()
	{
		string json = GetPacketString(GetUserHeaderString("req", "checkPost"), null);
		return SilentRequest(json);
	}

	// plant
	struct PlantParam
	{
		public int idPlant;
	}

	struct MountPlantParam
	{
		public int idPlant;
		public int idGiant;
		public int[] towerList;
	}

	struct RevengeRaidInfoParam
	{
		public long uidBaseRaid;
	}

	struct PlantSlotParam
	{
		public int idPlant;
		public int slotNum;
		public int isCash;
	}

	struct PlantUpgradeParam
	{
		public int idPlant;
		public int slotNum;
		public char slotType;
	}

	public STRequest doMountPlant(int idPlant, int idGiant, int [] towerList)
	{
		MountPlantParam param = new MountPlantParam();
		param.idPlant		= idPlant;
		param.idGiant		= idGiant;
		param.towerList		= towerList;

		string json = GetPacketString(GetUserHeaderString("req", "mountPlant"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doExtractResource(int idPlant)
	{
		PlantParam param = new PlantParam();
		param.idPlant		= idPlant;
		
		string json = GetPacketString(GetUserHeaderString("req", "extractResource"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doRequestBaseRaidLog()
	{
		string json = GetPacketString(GetUserHeaderString("req", "requestBaseRaidLog"), null);
		return Request(json);
	}

	public STRequest doRequestRevengeRaid(long uidBaseRaid)
	{
		RevengeRaidInfoParam param = new RevengeRaidInfoParam();
		param.uidBaseRaid = uidBaseRaid;

		string json = GetPacketString(GetUserHeaderString("req", "requestRevengeRaid"), Util.JsonEncode2(param));
		return Request(json);
	}
		
	public STRequest doUpgradeProduction(int idPlant, int slotNum, int isCash)
	{
		PlantSlotParam param = new PlantSlotParam();
		param.idPlant		= idPlant;
		param.slotNum		= slotNum;
		param.isCash		= isCash;

		string json = GetPacketString(GetUserHeaderString("req", "upgradeProduction"), Util.JsonEncode2(param));
		return Request(json);
	}
		
	public STRequest doCompleteUpgradeProduction(int idPlant, int slotNum)
	{
		PlantSlotParam param = new PlantSlotParam();
		param.idPlant		= idPlant;
		param.slotNum		= slotNum;

		string json = GetPacketString(GetUserHeaderString("req", "completeUpgradeProduction"), Util.JsonEncode2(param));
		return Request(json);
	}
		
	public STRequest doUpgradeStorage(int idPlant, int slotNum, int isCash)
	{
		PlantSlotParam param = new PlantSlotParam();
		param.idPlant		= idPlant;
		param.slotNum		= slotNum;
		param.isCash 		= isCash;

		string json = GetPacketString(GetUserHeaderString("req", "upgradeStorage"), Util.JsonEncode2(param));
		return Request(json);
	}
		
	public STRequest doCompleteUpgradeStorage(int idPlant, int slotNum)
	{
		PlantSlotParam param = new PlantSlotParam();
		param.idPlant		= idPlant;
		param.slotNum		= slotNum;

		string json = GetPacketString(GetUserHeaderString("req", "completeUpgradeStorage"), Util.JsonEncode2(param));
		return Request(json);
	}
		
	public STRequest doInstantlyCompleteUpgrade(int idPlant, int slotNum, PlantSlotType slotType)
	{
		PlantUpgradeParam param = new PlantUpgradeParam();
		param.idPlant		= idPlant;
		param.slotNum		= slotNum;
		param.slotType		= (slotType == PlantSlotType.Production) ? 'p' : 's';

		string json = GetPacketString(GetUserHeaderString("req", "instantlyCompleteUpgrade"), Util.JsonEncode2(param));
		return Request(json);
	}

	// giant
	struct EtherLevelGiantParam
	{
		public int idGiant;
		public int upLevel;
		public int isCash;
	}

	public STRequest doEtherLevelUpGiant(int idGiant, int upLevel, int isCash)
	{
		EtherLevelGiantParam param = new EtherLevelGiantParam();
		param.idGiant	= idGiant;
		param.upLevel	= upLevel;
		param.isCash 	= isCash;

		string json = GetPacketString(GetUserHeaderString("req", "levelUpGiant"), Util.JsonEncode2(param));
		return Request(json);
	}

	// tower
	struct TowerParam
	{
		public int idTower;
		public int isCash;
	}
		
	public STRequest doPurchaseTower(int idTower, int isCash)
	{
		TowerParam param = new TowerParam();
		param.idTower	= idTower;
		param.isCash	= isCash;

		string json = GetPacketString(GetUserHeaderString("req", "purchaseTower"), Util.JsonEncode2(param));
		return Request(json);
	}
	
	public STRequest doLevelUpTower(int idTower, int isCash)
	{
		TowerParam param = new TowerParam();
		param.idTower	= idTower;
		param.isCash 	= isCash;

		string json = GetPacketString(GetUserHeaderString("req", "levelUpTower"), Util.JsonEncode2(param));
		return Request(json);
	}

	// inventory
	class SaleItemParam
	{
		public Dictionary<int, int> itemList;
	}

	public STRequest doSaleItem(Dictionary<int, int> dic)
	{
		SaleItemParam param = new SaleItemParam();
		param.itemList = dic;

		string json = GetPacketString(GetUserHeaderString("req", "saleItem"), Util.JsonEncode2(param));
		return Request(json);
	}

	// search buff

	struct CancelSearchBuffParam
	{
		public int buffType;
	}

	public STRequest doSearchBuffCancel(int _buffType)
	{
		CancelSearchBuffParam param = new CancelSearchBuffParam();
		param.buffType = _buffType;

		string json = GetPacketString(GetUserHeaderString("req", "cancelBuff"), Util.JsonEncode2(param));
		return Request(json);
	}
		
	// work room
	struct WorkRoomIDSlotParam
	{
		public int idSlot;
	}

	struct CreateWorkRoomSlotItem
	{
		public int idTransmute;
		public int choiceIdItem;
	}

	struct WorkRoomIDSlotsParam
	{
		public List<int> idSlotList;
	}

	public STRequest doUnlockWorkRoomSlotByCash(int idSlot)
	{
		WorkRoomIDSlotParam param = new WorkRoomIDSlotParam();
		param.idSlot = idSlot;

		string json = GetPacketString(GetUserHeaderString("req", "unlockWorkRoomSlotByCash"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doCreateWorkRoomSlotItem(int idTransmute, int choiceIdItem = 0)
	{
		CreateWorkRoomSlotItem param = new CreateWorkRoomSlotItem();
		param.idTransmute = idTransmute;
		param.choiceIdItem = choiceIdItem;

		string json = GetPacketString(GetUserHeaderString("req", "createWorkRoomSlotItem"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doCancelWorkRoomSlotItem(int idSlot)
	{
		WorkRoomIDSlotParam param = new WorkRoomIDSlotParam();
		param.idSlot = idSlot;

		string json = GetPacketString(GetUserHeaderString("req", "cancelWorkRoomSlotItem"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doCompleteWorkRoomSlotItem(int idSlot)
	{
		WorkRoomIDSlotParam param = new WorkRoomIDSlotParam();
		param.idSlot = idSlot;

		string json = GetPacketString(GetUserHeaderString("req", "completeWorkRoomSlotItem"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doInstantlyCompleteWorkRoomSlotItem()
	{
		string json = GetPacketString(GetUserHeaderString("req", "instantlyCompleteWorkRoomSlotItem"), null);
		return Request(json);
	}

	public STRequest doUpdateWorkRoomSlotList()
	{
		string json = GetPacketString(GetUserHeaderString("req", "updateWorkRoomSlotList"), null);
		return Request(json);
	}

	public STRequest doCompleteAllWorkRoomSlotItem(List<int> idSlotList)
	{
		WorkRoomIDSlotsParam param = new WorkRoomIDSlotsParam();
		param.idSlotList = idSlotList;
		string json = GetPacketString(GetUserHeaderString("req", "completeAllWorkRoomSlotItem"), Util.JsonEncode2(param));
		return Request(json);
	}

	// Costume
	struct CostumeParam
	{
		public int idHero;
		public int idCostume;
	}

	public STRequest doEquipCostume(int idHero, int idCostume)
	{
		CostumeParam param = new CostumeParam();
		param.idHero = idHero;
		param.idCostume = idCostume;
		string json = GetPacketString(GetUserHeaderString("req", "equipCostume"), Util.JsonEncode2(param));
		return Request(json);
	}

	// Core
	struct CoreEvoHeroParam
	{
		public int idHero;
		public int idCore;
	}

	struct CoreEquipParam
	{
		public int idHero;
		public int idCore;
		public int idSlot;
	}

	struct CoreCancelEquipParam
	{
		public int idHero;
		public int idSlot;
	}

	struct CoreEvolutionParam
	{
		public int baseCore;
		public int materialCore;
	}

	struct SetHeroCoresParam
	{
		public int idHero;
		public Dictionary<int, int> core;
	}

	struct CoreEnchantParam
	{
		public int baseCore;
		public List<int> materialCore;
	}

	struct CoreIDParam
	{
		public int idCore;
	}

	struct UniqueCoreIDParam
	{
		public int baseCore;
	}

	struct CoreSaleParam
	{
		public List<int> saleCoreList;
	}

	struct CoreInventoryExtendParam
	{
		public int count;
	}

	struct SynthesizeCore
	{
		public int baseCore;
		public int materialCore;
	}

	struct CoreStatChangeParam
	{
		public int idCore;
		public int optionIndex;
	}

	public STRequest doSynthesizeCore(int idBaseCore, int idMaterialCore)
	{
		SynthesizeCore param = new SynthesizeCore();
		param.baseCore = idBaseCore;
		param.materialCore = idMaterialCore;

		string json = GetPacketString(GetUserHeaderString("req", "synthesizeCore"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doSmeltCore(int idBaseCore)
	{
		UniqueCoreIDParam param = new UniqueCoreIDParam();
		param.baseCore = idBaseCore;

		string json = GetPacketString(GetUserHeaderString("req", "smeltCore"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doDecomposeCore(int idBaseCore)
	{
		UniqueCoreIDParam param = new UniqueCoreIDParam();
		param.baseCore = idBaseCore;

		string json = GetPacketString(GetUserHeaderString("req", "decomposeCore"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doChangeOldCore(int idCore)
	{
		CoreIDParam param = new CoreIDParam();
		param.idCore = idCore;

		string json = GetPacketString(GetUserHeaderString("req", "changeOldCore"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doEvoHero(int idHero)
	{
		HeroParam param = new HeroParam();
		param.idHero = idHero;

		string json = GetPacketString(GetUserHeaderString("req", "evoHero"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doEquipCore(int idHero, int idCore, int idSlot)
	{
		CoreEquipParam param = new CoreEquipParam();
		param.idHero = idHero;
		param.idCore = idCore;
		param.idSlot = idSlot;

		string json = GetPacketString(GetUserHeaderString("req", "equipCore"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doCancelEquipCore(int idHero, int idSlot)
	{
		CoreCancelEquipParam param = new CoreCancelEquipParam();
		param.idHero = idHero;
		param.idSlot = idSlot;

		string json = GetPacketString(GetUserHeaderString("req", "cancelEquipCore"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doSetHeroCores(int idHero, Dictionary<int, int> core)
	{
		SetHeroCoresParam param = new SetHeroCoresParam();
		param.idHero = idHero;
		param.core = core;

		string json = GetPacketString(GetUserHeaderString("req", "setHeroCores"), Util.JsonEncode2(param));
		return Request(json);
	}
		
	public STRequest doNormalEvoCore(int idBase, int idMaterial)
	{
		CoreEvolutionParam param = new CoreEvolutionParam();
		param.baseCore = idBase;
		param.materialCore = idMaterial;

		string json = GetPacketString(GetUserHeaderString("req", "inheritEvoCore"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doRandomEvoCore(int idBase, int idMaterial)
	{
		CoreEvolutionParam param = new CoreEvolutionParam();
		param.baseCore = idBase;
		param.materialCore = idMaterial;

		string json = GetPacketString(GetUserHeaderString("req", "randomEvoCore"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doEnchantCore(int idBase, List<int> idMaterial)
	{
		CoreEnchantParam param = new CoreEnchantParam();
		param.baseCore = idBase;
		param.materialCore = idMaterial;

		string json = GetPacketString(GetUserHeaderString("req", "enchantCore"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doLockCore(int idCore)
	{
		CoreIDParam param = new CoreIDParam();
		param.idCore = idCore;

		string json = GetPacketString(GetUserHeaderString("req", "lockCore"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doUnlockCore(int idCore)
	{
		CoreIDParam param = new CoreIDParam();
		param.idCore = idCore;

		string json = GetPacketString(GetUserHeaderString("req", "unlockCore"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doCoreInventoryUP(int count)
	{
		CoreInventoryExtendParam param = new CoreInventoryExtendParam();
		param.count = count;

		string json = GetPacketString(GetUserHeaderString("req", "coreMaxLevelUp"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doSaleCore(List<int> saleCoreList)
	{
		CoreSaleParam param = new CoreSaleParam();
		param.saleCoreList = saleCoreList;

		string json = GetPacketString(GetUserHeaderString("req", "saleCore"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doCoreStatChangeParam(int idCore, int optionIndex)
	{
		CoreStatChangeParam param = new CoreStatChangeParam();
		param.idCore = idCore;
		param.optionIndex = optionIndex;

		string json = GetPacketString(GetUserHeaderString("req", "changeCoreOption"), Util.JsonEncode2(param));
		return Request(json);
	}

	// arena
	struct DoMatchingArenaParam
	{
		public int[] idHeros;
		public Dictionary<int, List<int>> saveDeck;
	}

	public STRequest doMatchingArena(int[] idHeros, DECK_TYPE deckType, int deckCount)
	{
		DoMatchingArenaParam param = new DoMatchingArenaParam();
		param.idHeros = idHeros;

		List<int> saveDeck = AccountDataStore.instance.ChangeSaveDeck(deckType, idHeros, deckCount);
		if (saveDeck != null)
		{
			param.saveDeck	= new Dictionary<int, List<int>>();
			param.saveDeck.Add((int)deckType, saveDeck);
		}

		string json = GetPacketString(GetUserHeaderString("req", "arenaMatchEnter"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doTakeArenaReward()
	{
		string json = GetPacketString(GetUserHeaderString("req", "takeArenaReward"), null);
		return Request(json);
	}
		
	public STRequest doRequestArenaBot()
	{
		string json = GetPacketString(GetUserHeaderString("req", "requestArenaBot"), null);
		return Request(json); 
	}

	struct ResultArenaBotParam
	{
		public int result;
	}

	public STRequest doResultArenaBot(int result)
	{
		ResultArenaBotParam param = new ResultArenaBotParam();
		param.result = result;

		string json = GetPacketString(GetUserHeaderString("req", "resultArenaBot"), Util.JsonEncode2(param));
		return Request (json);
	}

	// WorldBoss
	struct WorldBossBaseReward
	{
		public int idWorldBossChest;
	}

	struct WorldBossRankingParam
	{
		public int reqBeforeSeason;
	}

	public STRequest doWorldBossDeckInfo(long uidUser)
	{
		DeckInfoParam param = new DeckInfoParam();
		param.uidUser = uidUser; 

		string json = GetPacketString(GetUserHeaderString("req", "worldBossDeckInfo"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doTakeWorldBossChest(int idWorldBossChest)
	{
		WorldBossBaseReward param = new WorldBossBaseReward();
		param.idWorldBossChest = idWorldBossChest;
		
		string json = GetPacketString(GetUserHeaderString("req", "takeWorldBossChest"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doWorldBossSeasonReward()
	{
		string json = GetPacketString(GetUserHeaderString("req", "worldBossSeasonReward"), null);
		return Request(json);
	}

	public STRequest doWorldBossRanking(bool isOldSeason = false)
	{
		WorldBossRankingParam param = new WorldBossRankingParam();
		param.reqBeforeSeason = isOldSeason ? 1 : 0;

		string json = GetPacketString(GetUserHeaderString("req", "worldBossRanking"), Util.JsonEncode2(param));
		return Request(json);
	}

	public STRequest doWorldBossSeasonInfo()
	{
		string json = GetPacketString(GetUserHeaderString("req", "worldBossSeasonInfo"), null);
		return Request(json);
	}
	#endregion

	#region event
	public void evtServerList(IDictionary dic)
	{
		if (dic.Contains("resServerUrl"))
			resourceURL = dic["resServerUrl"].ToString();
		if (dic.Contains("chatServerUrl"))
			chatServerURL = dic["chatServerUrl"].ToString();
		if (dic.Contains("offset"))
			timeOffset = int.Parse(dic["offset"].ToString()) * (int)Constant.HourMilliseconds;
	}

	public void evtSeed(string str)
	{
		resourceID = str;
	}

	public void evtSignup(IDictionary dic)
	{
		if (dic.Contains("uidUser"))
			privateData.uidUser = long.Parse(dic["uidUser"].ToString());

#if UNITY_EDITOR || HIDDEN_LOGIN_TEST
		SaveAccountConfig();
#endif	
		DeviceSaveManager.Inst.SetUID(privateData.uidUser, true);

		hive.Analytics.sendEvent("AccountCreation");
	}

	public void evtLogin(IDictionary dic)
	{
		if (dic.Contains("uidUser"))
			privateData.uidUser = long.Parse(dic["uidUser"].ToString());
		DeviceSaveManager.Inst.SetUID(privateData.uidUser, false);
	}

	public void evtCheckUserData(IDictionary dic)
	{
		HiveManager.Inst.SetConflictUserData(dic);
	}

	public void evtUser(IDictionary dic)
	{
		// OffLineTest
//		_evtUser(dic, true);
	}

	public void evtModifyUser(IDictionary dic)
	{
		// OffLineTest
//		_evtUser(dic, false);
	}

	public void evtSuggestGuild(IDictionary dic)
	{
		AccountDataStore.instance.suggestGuildInfoData.UpdateValues(dic, true);
	}

	public void evtSearchGuild(IDictionary dic)
	{
		AccountDataStore.instance.suggestGuildInfoData.UpdateValues(dic, true);
	}

	public void evtGuildMyMemberInfo(IDictionary dic)
	{
		GuildMemberData myMember = AccountDataStore.instance.guildData.GetGuildMyMemberData();
		if(myMember != null)
			myMember.UpdateValues(dic, true);
	}

	public void evtGuildMember(IDictionary dic)
	{
		AccountDataStore.instance.guildData.member.UpdateValues(dic, true);	
	}

	public void evtGuildInfo(IDictionary dic)
	{
		if (dic == null)
			AccountDataStore.instance.guildData.ClearData();	
		else
			AccountDataStore.instance.guildData.UpdateValues(dic, false);
		PlayerDataStore.PlayerData.Refresh(PlayerDataFieldType.GuildMembership, null);
		PlayerDataStore.PlayerData.Refresh(PlayerDataFieldType.GuildHallLv, null);
	}

	public void evtGuildBattleData(IDictionary dic)
	{
		AccountDataStore.instance.guildBattleData.ClearData();
		AccountDataStore.instance.guildBattleData.UpdateValues(dic, true);
	}

	public void evtGuildBattleTargetList(IDictionary dic)
	{
		AccountDataStore.instance.guildBattleTargetDic.UpdateValues(dic, true);
	}

	public void evtGuildBattleTargetInfo(IDictionary dic)
	{
		AccountDataStore.instance.targetGuildBattleData.ClearData();
		AccountDataStore.instance.targetGuildBattleData.UpdateValues(dic, true);	
	}

	public void evtGuildBattleAttackLog(IList list)
	{
		AccountDataStore.instance.UpdateGuildBattleLogList(list, true);
	}

	public void evtGuildBattleDefenseLog(IList list)
	{
		AccountDataStore.instance.UpdateGuildBattleLogList(list, false);
	}

	public void evtGuildBattleFullyLog(IList list)
	{
		UpdateList<GuildBattleFullyLogData>(list, AccountDataStore.instance.guildBattleFullyLogs);
	}

	private void UpdateList<T>(IList srcList, IList targetList) where T : IGiantNetElement
	{
		Type t = typeof(T);
		targetList.Clear();
		for (int i = 0; i < srcList.Count; ++i)
		{
			T data = (T)t.GetConstructor(new Type[] {}).Invoke(new object[] {});
			data.UpdateValues((IDictionary)srcList[i], true);
			targetList.Add(data);
		}
	}

	public void evtReviewGuild(IDictionary dic)
	{
		if (dic.Contains("guildInfo"))
			AccountDataStore.instance.reviewGuildData.UpdateValues((IDictionary)dic["guildInfo"], true);
		if (dic.Contains("guildMember"))
			AccountDataStore.instance.reviewGuildData.member.UpdateValues((IDictionary)dic["guildMember"], true);
	}

	public void evtGuildBattleRanking(IDictionary dic)
	{
		AccountDataStore.instance.guildBattleRankingData.UpdateValues(dic, true);
	}

	public void evtGuildBattleUserRanking(IDictionary dic)
	{
		AccountDataStore.instance.guildBattleMyRankingData.UpdateValues(dic, true);
	}

	public void evtArenaHostInfo(IDictionary dic)
	{
		GiantArenaHostInfoData data = new GiantArenaHostInfoData();
		data.UpdateValues(dic, true);
		AccountDataStore.instance.arena.seqRoom = data.seqRoom;

		if (!m_IsWsConnecting)
		{
			m_IsWsConnecting = true;
			Global.Inst.InstantiateThread(() =>
			{
				lock(m_LockObject)
				{
					ConnectArenaSocket(data.host, data.port);					
					m_IsWsConnecting = false;
				}
			});	
		}
	}

	public void evtGuildReqUser(IDictionary dic)
	{
		AccountDataStore.instance.guildData.reqUser.UpdateValues(dic, true);
		var enumerator = AccountDataStore.instance.guildData.reqUser.GetEnumerator();
		while (enumerator.MoveNext())
		{
			GuildReqUser recvData = enumerator.Current.Value;
			ChatData chatData		= new ChatData();
			chatData.chatFunc 		= CHAT_FUNC.SYSTEM_MSG;
			chatData.chatType 		= CHAT_TYPE.GUILD;
			chatData.chatSubType	= CHAT_SUB_TYPE.REQUEST_SIGNUP;

			ChatRecvDataSysMsg data = new ChatRecvDataSysMsg();
			data.reqId 				= recvData._id;
			data.msg 				= recvData.msg;
			data.timestamp		 	= recvData.actionTime;
			data.target 			= recvData.nickname;
			data.targetLv 			= recvData.level;
			data.uidUser 			= recvData.uidUser.ToString();

			if (!string.IsNullOrEmpty(recvData._id) && ChatManager.Inst.guildChatDataList.Find(item => item.GetReceiveData.GetReqID() == recvData._id) == null)
			{
				chatData.SetReceiveData(data);
				ChatManager.Inst.AddMessageItem(chatData);
			}
		}
	}

	private void _evtUser(IDictionary dic, bool isClear)
	{
		int updateFlag = 0;

		if (AccountDataStore.instance.user.UpdateValues(dic, isClear))
			updateFlag |= (int)UserDataFlag.User;

		// 업데이트 순서 중요
		// item -> hero
		// area -> subQuestLog -> subQuest
		updateFlag |= UpdateGiantNetElement(dic, "item", AccountDataStore.instance.items, isClear) & (int)UserDataFlag.Item;
		updateFlag |= UpdateGiantNetElement(dic, "hero", AccountDataStore.instance.heroes, isClear) & (int)UserDataFlag.Hero;

		updateFlag |= UpdateGiantNetElement(dic, "area", AccountDataStore.instance.areas, isClear) & (int)UserDataFlag.Area;

		updateFlag |= UpdateGiantNetElement(dic, "mainQuestLog", AccountDataStore.instance.mainQuestLogs, isClear) & (int)UserDataFlag.MainQuestLog;
		updateFlag |= UpdateGiantNetElement(dic, "subQuestLog", AccountDataStore.instance.subQuestLogs, isClear) & (int)UserDataFlag.SubQuestLog;
		updateFlag |= UpdateGiantNetElement(dic, "subQuest", AccountDataStore.instance.subQuests, isClear) & (int)UserDataFlag.SubQuest;

		updateFlag |= UpdateGiantNetElement(dic, "eventLog", AccountDataStore.instance.eventLogs, isClear) & (int)UserDataFlag.EventLog;
		updateFlag |= UpdateGiantNetElement(dic, "stageLog", AccountDataStore.instance.stageLogs, isClear) & (int)UserDataFlag.StageLog;

		updateFlag |= UpdateGiantNetElement(dic, "treasureChest", AccountDataStore.instance.treasureChest, isClear) & (int)UserDataFlag.TreasureChest;

		updateFlag |= UpdateGiantNetElement(dic, "goodsLog", AccountDataStore.instance.goodsLogs, isClear) & (int)UserDataFlag.PurchaseGoods;

		updateFlag |= UpdateGiantNetElement(dic, "clientData", AccountDataStore.instance.clientData, isClear) & (int)UserDataFlag.ClientData;
		updateFlag |= UpdateGiantNetElement(dic, "researchSkill", AccountDataStore.instance.researchSkill, isClear) & (int)UserDataFlag.ResearchSkill;
		updateFlag |= UpdateGiantNetElement(dic, "achievementLog", AccountDataStore.instance.achievement, isClear) & (int)UserDataFlag.AchievementLog;
		updateFlag |= UpdateGiantNetElement(dic, "dungeonMission", AccountDataStore.instance.dungeonMission, isClear) & (int)UserDataFlag.DungeonMission;
		updateFlag |= UpdateGiantNetElement(dic, "giantStorage", AccountDataStore.instance.giantStorage, isClear) & (int)UserDataFlag.BaseStorage;
		updateFlag |= UpdateGiantNetElement(dic, "resourceStorage", AccountDataStore.instance.resourceStorage, isClear) & (int)UserDataFlag.BaseStorage;
		updateFlag |= UpdateGiantNetElement(dic, "searchBuff", AccountDataStore.instance.searchBuff, isClear) & (int)UserDataFlag.SearchBuff;
		updateFlag |= UpdateGiantNetElement(dic, "core", AccountDataStore.instance.core, isClear) & (int)UserDataFlag.Core;
		updateFlag |= UpdateGiantNetElement(dic, "npc", AccountDataStore.instance.npc, isClear) & (int)UserDataFlag.Npc;
		updateFlag |= UpdateGiantNetElement(dic, "gather", AccountDataStore.instance.gather, isClear) & (int)UserDataFlag.Gather;
		updateFlag |= UpdateGiantNetElement(dic, "raceBattleChest", AccountDataStore.instance.raceBattleChest, isClear) & (int)UserDataFlag.RaceBattleChest;
		updateFlag |= UpdateGiantNetElement(dic, "dailyQuest", AccountDataStore.instance.dailyQuest, isClear) & (int)UserDataFlag.DailyQuest;

		if (updateUserDelegate != null)
			updateUserDelegate((UserDataFlag)updateFlag);
		
		AccountDataStore.instance.UpdateAccountInfo();
	}

	// OffLineTest
	public void OfflineEvt()
	{
		IDictionary userDic = (IDictionary)Util.JsonDecode("{\"nickname\":\"\",\"level\":95,\"exp\":3456,\"gold\":95490,\"rGold\":0,\"cash\":0,\"rCash\":5410,\"ether\":22460,\"ethereum\":0,\"dungeonToken\":{\"count\":10,\"time\":1538372954749,\"buyTime\":0,\"buyCount\":0},\"item\":{\"16\":5,\"14010\":2,\"22010\":6,\"41010\":3,\"51010\":5,\"61020\":3,\"2010201\":1,\"5030101\":1},\"core\":{\"level\":1,\"inventory\":{\"1\":{\"idDtCore\":1100,\"idChar\":50001,\"rank\":2,\"evo\":1,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{},\"useHero\":0,\"lockup\":0},\"2\":{\"idDtCore\":1100,\"idChar\":50001,\"rank\":2,\"evo\":1,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{},\"useHero\":0,\"lockup\":0},\"3\":{\"idDtCore\":10,\"idChar\":10410,\"rank\":2,\"evo\":3,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{\"0\":{\"id\":1081,\"lv\":25},\"1\":{\"id\":2027,\"lv\":13}},\"useHero\":0,\"lockup\":0,\"ver\":1},\"4\":{\"idDtCore\":43,\"idChar\":10360,\"rank\":1,\"evo\":2,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{\"0\":{\"id\":1021,\"lv\":16}},\"useHero\":0,\"lockup\":0,\"ver\":1},\"5\":{\"idDtCore\":9,\"idChar\":10170,\"rank\":2,\"evo\":2,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{\"0\":{\"id\":1001,\"lv\":17}},\"useHero\":0,\"lockup\":0,\"ver\":1},\"6\":{\"idDtCore\":56,\"idChar\":10700,\"rank\":2,\"evo\":2,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{\"0\":{\"id\":1021,\"lv\":14}},\"useHero\":0,\"lockup\":0,\"ver\":1},\"7\":{\"idDtCore\":17,\"idChar\":10080,\"rank\":3,\"evo\":2,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{\"0\":{\"id\":1061,\"lv\":17}},\"useHero\":0,\"lockup\":0,\"ver\":1},\"8\":{\"idDtCore\":29,\"idChar\":10340,\"rank\":2,\"evo\":2,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{\"0\":{\"id\":1001,\"lv\":17}},\"useHero\":0,\"lockup\":0,\"ver\":1},\"9\":{\"idDtCore\":6,\"idChar\":10430,\"rank\":3,\"evo\":3,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{\"0\":{\"id\":1001,\"lv\":25},\"1\":{\"id\":2006,\"lv\":14}},\"useHero\":0,\"lockup\":0,\"ver\":1},\"10\":{\"idDtCore\":39,\"idChar\":10480,\"rank\":1,\"evo\":2,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{\"0\":{\"id\":1061,\"lv\":16}},\"useHero\":0,\"lockup\":0,\"ver\":1},\"11\":{\"idDtCore\":28,\"idChar\":10470,\"rank\":2,\"evo\":2,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{\"0\":{\"id\":1041,\"lv\":14}},\"useHero\":0,\"lockup\":0,\"ver\":1},\"12\":{\"idDtCore\":25,\"idChar\":10290,\"rank\":2,\"evo\":2,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{\"0\":{\"id\":1001,\"lv\":15}},\"useHero\":0,\"lockup\":0,\"ver\":1},\"13\":{\"idDtCore\":8,\"idChar\":10020,\"rank\":1,\"evo\":4,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{\"0\":{\"id\":1041,\"lv\":30},\"1\":{\"id\":2011,\"lv\":20}},\"useHero\":0,\"lockup\":0,\"ver\":1}}},\"area\":{\"10\":{\"eps\":{},\"count\":0},\"20\":{\"eps\":{\"0\":{\"category\":5,\"value\":13,\"quantity\":210,\"clear\":true},\"1\":{\"category\":5,\"value\":41010,\"quantity\":2,\"clear\":true},\"2\":{\"category\":2,\"clear\":false,\"value\":21012,\"start\":false,\"succBattleCount\":0,\"failBattleCount\":1,\"log\":{\"1\":{\"result\":0}}},\"3\":{\"category\":2,\"clear\":false,\"value\":21014,\"start\":false,\"succBattleCount\":0,\"failBattleCount\":0,\"log\":{}}},\"count\":1,\"searchEPArray\":2012},\"30\":{\"eps\":{\"0\":{\"category\":2,\"clear\":false,\"value\":32011,\"start\":false,\"succBattleCount\":0,\"failBattleCount\":0,\"log\":{}},\"1\":{\"category\":9,\"clear\":true,\"value\":96873,\"pvp\":{\"uidUser\":289013,\"nickname\":\"\",\"level\":11,\"grade\":5,\"researchSkill\":{\"1\":1,\"2\":1},\"hero\":{\"20201\":{\"idHero\":20201,\"exp\":22758,\"level\":19,\"evo\":2,\"rank\":4,\"skill\":{\"100101\":1,\"100102\":1,\"100103\":4,\"100104\":4,\"100105\":0,\"1100101\":0},\"limitBreak\":1,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":13,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}},\"30301\":{\"idHero\":30301,\"exp\":22758,\"level\":19,\"evo\":3,\"rank\":4,\"skill\":{\"101801\":1,\"101802\":1,\"101803\":4,\"101804\":4,\"101805\":4,\"1101801\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":9,\"1\":11,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}},\"30501\":{\"idHero\":30501,\"exp\":16494,\"level\":18,\"evo\":2,\"rank\":4,\"skill\":{\"100501\":1,\"100502\":1,\"100503\":4,\"100504\":4,\"100505\":0,\"1100501\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":14,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}},\"40101\":{\"idHero\":40101,\"exp\":8906,\"level\":15,\"evo\":2,\"rank\":3,\"skill\":{\"102201\":1,\"102202\":1,\"102203\":3,\"102204\":3,\"102205\":0,\"1102201\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":11030,\"1\":0,\"2\":0,\"3\":0,\"4\":51030,\"5\":51030},\"core\":{\"0\":2,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}},\"80401\":{\"idHero\":80401,\"exp\":15358,\"level\":18,\"evo\":2,\"rank\":4,\"skill\":{\"105601\":1,\"105602\":1,\"105603\":4,\"105604\":4,\"105605\":0,\"1105601\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":5,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}}},\"core\":{\"2\":{\"idDtCore\":7,\"idChar\":10100,\"rank\":1,\"evo\":1,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{\"0\":{\"id\":1061,\"lv\":3}},\"useHero\":40101,\"lockup\":0},\"5\":{\"idDtCore\":23,\"idChar\":10250,\"rank\":2,\"evo\":1,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{\"0\":{\"id\":1071,\"lv\":1}},\"useHero\":80401,\"lockup\":0},\"9\":{\"idDtCore\":22,\"idChar\":10270,\"rank\":3,\"evo\":3,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{\"0\":{\"id\":1021,\"lv\":14},\"1\":{\"id\":2005,\"lv\":11}},\"useHero\":30301,\"lockup\":0},\"11\":{\"idDtCore\":23,\"idChar\":10250,\"rank\":2,\"evo\":1,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{\"0\":{\"id\":1031,\"lv\":5}},\"useHero\":30301,\"lockup\":0},\"13\":{\"idDtCore\":7,\"idChar\":10100,\"rank\":3,\"evo\":1,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{\"0\":{\"id\":1051,\"lv\":3}},\"useHero\":20201,\"lockup\":0},\"14\":{\"idDtCore\":31,\"idChar\":10500,\"rank\":2,\"evo\":2,\"enchant\":0,\"enchantBonusRate\":0,\"option\":{\"0\":{\"id\":1021,\"lv\":9}},\"useHero\":30501,\"lockup\":0}}},\"retry\":0,\"start\":true},\"2\":{\"category\":5,\"value\":61020,\"quantity\":2,\"clear\":true},\"3\":{\"category\":5,\"value\":13010,\"quantity\":4,\"clear\":false},\"4\":{\"category\":2,\"clear\":false,\"value\":32012,\"start\":false,\"succBattleCount\":0,\"failBattleCount\":0,\"log\":{}}},\"count\":230,\"fever\":{\"target\":0,\"targetTime\":0,\"targetClear\":0,\"readyGrade\":0,\"star\":285,\"count\":0,\"saveCount\":0},\"searchEPArray\":3102}},\"gather\":{},\"npc\":{},\"searchBuff\":{},\"hero\":{\"20101\":{\"idHero\":20101,\"exp\":0,\"level\":1,\"evo\":1,\"rank\":1,\"skill\":{\"101101\":1,\"101102\":1,\"101103\":1,\"101104\":0,\"101105\":0,\"1101101\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":0,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}},\"20201\":{\"idHero\":20201,\"exp\":0,\"level\":1,\"evo\":1,\"rank\":1,\"skill\":{\"100101\":1,\"100102\":1,\"100103\":1,\"100104\":0,\"100105\":0,\"1100101\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":0,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}},\"20203\":{\"idHero\":20203,\"exp\":0,\"level\":1,\"evo\":2,\"rank\":1,\"skill\":{\"106301\":1,\"106302\":1,\"106303\":1,\"106304\":0,\"106305\":0,\"1106301\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":0,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}},\"20401\":{\"idHero\":20401,\"exp\":0,\"level\":1,\"evo\":2,\"rank\":1,\"skill\":{\"100701\":1,\"100702\":1,\"100703\":1,\"100704\":0,\"100705\":0,\"1100701\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":0,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}},\"20403\":{\"idHero\":20403,\"exp\":0,\"level\":1,\"evo\":3,\"rank\":1,\"skill\":{\"106001\":1,\"106002\":1,\"106003\":1,\"106004\":0,\"106005\":0,\"1106001\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":0,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}},\"30201\":{\"idHero\":30201,\"exp\":0,\"level\":1,\"evo\":1,\"rank\":1,\"skill\":{\"101701\":1,\"101702\":1,\"101703\":1,\"101704\":0,\"101705\":0,\"1101701\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":0,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}},\"30203\":{\"idHero\":30203,\"exp\":0,\"level\":1,\"evo\":3,\"rank\":1,\"skill\":{\"107001\":1,\"107002\":1,\"107003\":1,\"107004\":0,\"1107001\":0,\"1107002\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":0,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}},\"30402\":{\"idHero\":30402,\"exp\":0,\"level\":1,\"evo\":3,\"rank\":1,\"skill\":{\"107101\":1,\"107102\":1,\"107103\":1,\"107104\":0,\"107105\":0,\"1107101\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":0,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}},\"40101\":{\"idHero\":40101,\"exp\":0,\"level\":1,\"evo\":2,\"rank\":1,\"skill\":{\"102201\":1,\"102202\":1,\"102203\":1,\"102204\":0,\"102205\":0,\"1102201\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":0,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}},\"40501\":{\"idHero\":40501,\"exp\":0,\"level\":1,\"evo\":2,\"rank\":1,\"skill\":{\"102401\":1,\"102402\":1,\"102403\":1,\"102404\":0,\"102405\":0,\"1102401\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":0,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}},\"50301\":{\"idHero\":50301,\"exp\":0,\"level\":1,\"evo\":3,\"rank\":1,\"skill\":{\"102901\":1,\"102902\":1,\"102903\":1,\"102904\":0,\"102905\":0,\"1102901\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":0,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0},\"idCostume\":5030101,\"index\":0},\"50401\":{\"idHero\":50401,\"exp\":0,\"level\":1,\"evo\":1,\"rank\":1,\"skill\":{\"102601\":1,\"102602\":1,\"102603\":1,\"102604\":0,\"102605\":0,\"1102601\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":0,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}},\"60203\":{\"idHero\":60203,\"exp\":0,\"level\":1,\"evo\":2,\"rank\":1,\"skill\":{\"105201\":1,\"105202\":1,\"105203\":1,\"1105201\":0,\"1105202\":0,\"1105203\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":0,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}},\"60401\":{\"idHero\":60401,\"exp\":0,\"level\":1,\"evo\":2,\"rank\":1,\"skill\":{\"103101\":1,\"103102\":1,\"103103\":1,\"103104\":0,\"1103101\":0,\"1103102\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":0,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}},\"80101\":{\"idHero\":80101,\"exp\":0,\"level\":1,\"evo\":3,\"rank\":1,\"skill\":{\"105301\":1,\"105302\":1,\"105303\":1,\"105304\":0,\"105305\":0,\"1105301\":0},\"limitBreak\":0,\"soul\":1,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":0,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}},\"80202\":{\"idHero\":80202,\"exp\":0,\"level\":1,\"evo\":2,\"rank\":1,\"skill\":{\"105701\":1,\"105702\":1,\"105703\":1,\"105704\":0,\"105705\":0,\"1105701\":0},\"limitBreak\":0,\"soul\":0,\"item\":{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0},\"core\":{\"0\":0,\"1\":0,\"2\":0},\"inscription\":{},\"assign\":{\"time\":0}}},\"researchSkill\":{},\"giantStorage\":{},\"resourceStorage\":{},\"mainQuest\":1001,\"mainQuestHard\":1199,\"mainQuestLog\":{},\"subQuestSlot\":1,\"subQuest\":{},\"subQuestLog\":{},\"eventLog\":{\"80\":1,\"90\":1,\"95\":1,\"101\":1,\"103\":1,\"105\":1,\"106\":1,\"111\":1,\"121\":1,\"123\":1,\"125\":1,\"130\":1,\"131\":1,\"133\":1,\"141\":1,\"143\":1,\"145\":1,\"147\":1,\"149\":1,\"151\":1,\"153\":1,\"155\":1,\"156\":1,\"157\":1,\"158\":1,\"159\":1,\"190\":1,\"191\":1,\"192\":1,\"193\":1,\"194\":1,\"195\":1,\"197\":1,\"198\":1,\"211\":1,\"221\":1,\"222\":1,\"224\":1,\"225\":1,\"228\":1,\"229\":1,\"231\":1,\"232\":1,\"233\":1,\"234\":1,\"241\":1,\"242\":1,\"243\":1,\"247\":1,\"261\":1,\"262\":1,\"263\":1,\"264\":1,\"265\":1,\"266\":1,\"268\":1,\"269\":1,\"270\":1,\"271\":1,\"272\":1,\"274\":1,\"275\":1,\"276\":1,\"278\":1,\"280\":1,\"281\":1,\"282\":1,\"284\":1,\"285\":1,\"287\":1,\"288\":1,\"292\":1,\"293\":1,\"294\":1,\"299\":1,\"311\":1,\"321\":1,\"323\":1,\"325\":1,\"327\":1,\"329\":1,\"331\":1,\"333\":1,\"335\":1,\"341\":1,\"343\":1,\"345\":1,\"347\":1,\"351\":1,\"352\":1,\"353\":1,\"361\":1,\"363\":1,\"365\":1,\"371\":1,\"373\":1,\"375\":1,\"377\":1,\"391\":1,\"393\":1,\"394\":1,\"395\":1,\"396\":1,\"397\":1,\"398\":1,\"399\":1,\"400\":1,\"401\":1,\"402\":1,\"403\":1,\"404\":1,\"411\":1,\"412\":1,\"421\":1,\"426\":1,\"429\":1,\"491\":1,\"495\":1,\"496\":1,\"498\":1,\"500\":1,\"581\":1,\"582\":1,\"583\":1,\"584\":1,\"585\":1},\"eventLastLog\":0,\"stageLog\":{\"10\":{\"stage\":{},\"count\":0},\"20\":{\"stage\":{\"21012\":{\"count\":1,\"stageStar\":0}},\"count\":0},\"30\":{\"stage\":{},\"count\":0}},\"achievementLog\":{},\"dungeonMission\":{},\"missionStartDate\":0,\"dailyQuestTime\":0,\"dailyQuest\":{},\"goodsLog\":{\"2\":{\"buy\":1538373012976,\"base\":1538373012976,\"time\":1538373012976,\"count\":1},\"1006\":{\"buy\":1538373725779,\"base\":1538373725779,\"time\":1538373725779,\"count\":2},\"2010\":{\"buy\":1538373643850,\"base\":1538373643850,\"time\":1538373643850,\"count\":2},\"2012\":{\"buy\":1538373733158,\"base\":1538352000000,\"time\":1538438400000,\"count\":1},\"3001\":{\"buy\":1543211766101,\"base\":1543211766101,\"time\":1543211766101,\"count\":15},\"4003\":{\"buy\":1543557979898,\"base\":1543557979898,\"time\":1543557979898,\"count\":1},\"10002\":{\"buy\":1543211755127,\"base\":1543211755127,\"time\":1543211755127,\"count\":1},\"10003\":{\"buy\":1543211881300,\"base\":1543211881300,\"time\":1543211881300,\"count\":1}},\"refillGoldTime\":1538373722452,\"totalGold\":0,\"totalEther\":210,\"treasureChest\":{},\"treasureBoxCount\":80,\"freeTreasureCount\":0,\"freeTreasureTime\":0,\"raceBattleChest\":{\"0\":{},\"1\":{},\"2\":{}},\"pvpDeck\":{\"0\":50301},\"pvpHeroMaxPoint\":1091,\"pvpHeroGrade\":7,\"pvpHeroTotalPoint\":0,\"pvpHeroMaxGrade\":7,\"pvpHeroMaxRanking\":0,\"pvpHeroBeforeRanking\":0,\"pvpHeroWinCount\":5,\"pvpHeroMatchCount\":0,\"arenaCurrency\":4,\"arenaMatchCount\":0,\"arenaPoint\":909,\"arenaGrade\":1,\"pvpBaseWinCount\":0,\"worldBossCount\":0,\"worldBossTotalDamage\":0,\"worldBossMaxDamage\":0,\"worldBossChest\":{\"1\":0,\"2\":0,\"3\":0},\"worldBossSeasonPlay\":340003,\"worldBossSeasonReward\":340003,\"guild\":{\"point\":0,\"totalPoint\":0,\"resetTime\":0},\"clientData\":{\"subQuestData\":{},\"arenaPlayedOnce\":true,\"fatalSkillPoint\":22,\"dailyQuestData\":{},\"guildChatBlockEndTime\":0,\"isOpenArena\":true,\"idLastClearStage\":0,\"areaClearFlags\":{},\"chatBlockEndTime\":0,\"guildChatBlockIndex\":0,\"chatBlockIndex\":0,\"isDoneFirst4CoreEvoTracker\":false},\"actionTime\":1543558013774,\"idSearchLast\":20}");
		_evtUser(userDic, true);

		IDictionary baseDic = (IDictionary)Util.JsonDecode("{\"uidUser\":289048,\"nickname\":\"\",\"level\":95,\"hall\":{\"level\":10,\"upgradeTime\":0},\"plant\":{\"2\":{\"idPlant\":2,\"productSlot\":{},\"storageSlot\":{},\"giantSocket\":0,\"towerSocket\":{\"0\":0,\"1\":0,\"2\":0},\"capacity\":1000},\"3\":{\"idPlant\":3,\"productSlot\":{},\"storageSlot\":{},\"giantSocket\":0,\"towerSocket\":{\"0\":0,\"1\":0,\"2\":0},\"capacity\":1000},\"4\":{\"idPlant\":4,\"productSlot\":{},\"storageSlot\":{},\"giantSocket\":0,\"towerSocket\":{\"0\":0,\"1\":0,\"2\":0},\"capacity\":1000}},\"giant\":{},\"tower\":{},\"plantUpNow\":0,\"plantUpMax\":1,\"lootingLog\":{},\"areaInfo\":30,\"researchUpgrade\":{},\"raidShieldTime\":0,\"actionTime\":1543557846178,\"createTime\":1543212142675,\"workRoom\":{\"1\":{\"status\":0,\"startTime\":0,\"endTime\":0,\"idItem\":0,\"idTransmute\":0,\"choiceIdItem\":0,\"createTime\":0},\"2\":{\"status\":0,\"startTime\":0,\"endTime\":0,\"idItem\":0,\"idTransmute\":0,\"choiceIdItem\":0,\"createTime\":0},\"3\":{\"status\":0,\"startTime\":0,\"endTime\":0,\"idItem\":0,\"idTransmute\":0,\"choiceIdItem\":0,\"createTime\":0},\"4\":{\"status\":0,\"startTime\":0,\"endTime\":0,\"idItem\":0,\"idTransmute\":0,\"choiceIdItem\":0,\"createTime\":0},\"5\":{\"status\":0,\"startTime\":0,\"endTime\":0,\"idItem\":0,\"idTransmute\":0,\"choiceIdItem\":0,\"createTime\":0}},\"darkether\":0,\"lumber\":0,\"cubic\":0}");
		_evtBase(baseDic, true);
	}

	public void evtBase(IDictionary dic)
	{
		// OffLineTest
//		_evtBase(dic, true);
	}

	public void evtModifyBase(IDictionary dic)
	{
		// OffLineTest
//		_evtBase(dic, false);
	}

	private void _evtBase(IDictionary dic, bool isClear)
	{
		int updateFlag = 0;

		if (AccountDataStore.instance.baseData.UpdateValues(dic, isClear))
			updateFlag |= (int)BaseDataFlag.Base;

		updateFlag |= UpdateGiantNetElement(dic, "hall", AccountDataStore.instance.hall, isClear) & (int)BaseDataFlag.Hall;
		updateFlag |= UpdateGiantNetElement(dic, "plant", AccountDataStore.instance.plants, isClear) & (int)BaseDataFlag.Plant;
		updateFlag |= UpdateGiantNetElement(dic, "giant", AccountDataStore.instance.giants, isClear) & (int)BaseDataFlag.Giant;
		updateFlag |= UpdateGiantNetElement(dic, "tower", AccountDataStore.instance.towers, isClear) & (int)BaseDataFlag.Tower;
		if (isClear)		// evtModifyBase 로 들어오는 lootingLog 는 무시한다. 베이스에 진입하면 서버에서 lootingLog 를 초기화하는데, 이 값이 evtModifyBase 로 들어오기 때문이다.
			updateFlag |= UpdateGiantNetElement(dic, "lootingLog", AccountDataStore.instance.lootingLogs, isClear) & (int)BaseDataFlag.LootingLog;

		if (dic.Contains("revengeRaid") && dic["revengeRaid"] != null)
			evtRevengeRaid((IDictionary)dic["revengeRaid"]);

		if (updateBaseDelegate != null)
			updateBaseDelegate((BaseDataFlag)updateFlag);
		
		AccountDataStore.instance.UpdateAccountInfo();
	}

	public void evtLootingLog(IDictionary dic)
	{
		AccountDataStore.instance.lootingLogs.UpdateValues(dic, true);
	}

	public void evtBaseRaidLog(IDictionary dic)
	{
		AccountDataStore.instance.plantLogs.UpdateValues(dic, true);
	}

	public void evtModifyBaseRaidLog(IDictionary dic)
	{
		AccountDataStore.instance.plantLogs.UpdateValues(dic, false);
	}

	public void evtRevengeRaid(IDictionary dic)
	{
		if (!dic.Contains("uidBaseRaid"))
			return;
		
		long uidBaseRaid = (long)dic["uidBaseRaid"];
		GiantPlantLogData logData;
		if (!AccountDataStore.instance.plantLogs.TryGetValue(uidBaseRaid, out logData) || logData == null)
			return;

		if (logData.epData.baseRaid == null)
			logData.epData.baseRaid = new GiantEPBaseRaidData();
		
		logData.epData.baseRaid.UpdateValues(dic, true);
		
		if (dic.Contains("requestRevengeRaidTime"))
			logData.epData.baseRaid.raidStartTime = (long)dic["requestRevengeRaidTime"];
	}

	public void evtPvpHeroPoint(long pvpHeroPoint)
	{
		AccountDataStore.instance.user.pvpHeroPoint = (int)pvpHeroPoint;
	}

	public void evtRevengePvpHero(IDictionary dic)
	{
		if (!dic.Contains("pvp") || !((IDictionary)dic["pvp"]).Contains("uidPvpHeroLog"))
			return;
		
		long uidPvpHeroLog = (long)((IDictionary)dic["pvp"])["uidPvpHeroLog"];
		PvpUserLog pvpUserLog = AccountDataStore.instance.GetPvpDefenseLog(uidPvpHeroLog);
		if (pvpUserLog == null)
			return;
		
		pvpUserLog.epData.UpdateValues(dic, false);
		pvpUserLog.epData.pvp.uidPvpHeroLog = uidPvpHeroLog;
		pvpUserLog.UpdateEpData();
	}

	public void evtAttendCheck(IDictionary dic)
	{
		AccountDataStore.instance.attendCheck.UpdateValues(dic, true);
	}

	public void evtCheckRefillGold(IDictionary dic)
	{
		Util.GetFields(AccountDataStore.instance.user, dic);
	}

	public void evtInscriptionEnchant(bool isSuccess)
	{
		if(onInscriptionEnchantEndDelegate != null)
			onInscriptionEnchantEndDelegate(isSuccess);
	}

	public void evtCoreEnchant(List<object> obj)
	{
		List<GiantCoreEnchantResult> results = new List<GiantCoreEnchantResult>();
		var enumerator = obj.GetEnumerator();
		while(enumerator.MoveNext())
		{
			GiantCoreEnchantResult result = new GiantCoreEnchantResult();
			Util.GetFields(result, enumerator.Current);
			results.Add(result);
		}

		if(onCoreEnchantEndDelegate != null)
			onCoreEnchantEndDelegate(results);
	}

	public void evtPostList(IDictionary dic)
	{
		AccountDataStore.instance.posts.UpdateValues(dic, true);
	}

	public void evtPostCount(long postCount)
	{
		AccountDataStore.instance.user.postCount = (int)postCount;

		if (updateUserDelegate != null)
			updateUserDelegate(UserDataFlag.User);
	}

	public void evtPvpHeroSeasonInfo(IDictionary dic)
	{
		AccountDataStore.instance.pvpSeasonInfo.UpdateValues(dic, true);
	}
		
	public void evtPvpHeroLog(IDictionary dic)
	{
		AccountDataStore.instance.pvpLog.battleLog.Clear();
		AccountDataStore.instance.pvpLog.defenseLog.Clear();
		AccountDataStore.instance.pvpLog.UpdateValues(dic, false);
		AccountDataStore.instance.pvpLog.InitData();
	}

	public void evtPvpHeroRanking(IList list)
	{
		AccountDataStore.instance.pvpHeroRanking.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			PvpHeroRanking item = new PvpHeroRanking();
			item.UpdateValues((IDictionary)list[i], true);
			AccountDataStore.instance.pvpHeroRanking.Add(item);
		}
		AccountDataStore.instance.pvpHeroRanking.Sort(PvpHeroRanking.SortByPoint);

		PvpHeroRanking prevRankData = new PvpHeroRanking();
		for (int i = 0; i < AccountDataStore.instance.pvpHeroRanking.Count; i++)
		{
			PvpHeroRanking rankData = AccountDataStore.instance.pvpHeroRanking[i];

			rankData.LastWeek = false;
			if (rankData.point.Equals(prevRankData.point))
			{
				rankData.Rank = prevRankData.Rank;	
			}
			else
			{
				rankData.Rank = i + 1;
			}
			prevRankData = rankData;
		}
		AccountDataStore.instance.pvpSeasonInfo.lastWeekRankRefresh = false;
	}

	public void evtBeforePvpHeroRanking(IList list)
	{
		if (list == null || list.Count == 0)
			return;
		
		AccountDataStore.instance.pvpHeroRankingLastWeek.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			PvpHeroRanking item = new PvpHeroRanking();
			item.UpdateValues((IDictionary)list[i], true);
			AccountDataStore.instance.pvpHeroRankingLastWeek.Add(item);
		}
		AccountDataStore.instance.pvpHeroRankingLastWeek.Sort(PvpHeroRanking.SortByPoint);

		PvpHeroRanking prevRankData = new PvpHeroRanking();
		for (int i = 0; i < AccountDataStore.instance.pvpHeroRankingLastWeek.Count; i++)
		{
			PvpHeroRanking rankData = AccountDataStore.instance.pvpHeroRankingLastWeek[i];

			rankData.LastWeek = true;
			if (rankData.point.Equals(prevRankData.point))
			{
				rankData.Rank = prevRankData.Rank;	
			}
			else
			{
				rankData.Rank = i + 1;
			}
			prevRankData = rankData;
		}
	}

	public void evtPvpHeroUserRanking(long heroRank)
	{
		AccountDataStore.instance.user.pvpHeroRanking = (int)heroRank;
	}

	// 추후 삭제 예정
	public void evtArenaSeasonInfo(IDictionary dic)
	{
	}

	// 추후 삭제 예정
	public void evtDuelSeasonInfo(IDictionary dic)
	{
	}

	public void evtArenaRanking(IList list)
	{
		AccountDataStore.instance.arenaRanking.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			GiantArenaRankingData item = new GiantArenaRankingData();
			item.UpdateValues((IDictionary)list[i], true);
			item.ranking = i + 1;
			AccountDataStore.instance.arenaRanking.Add(item);
		}
	}

	public void evtArenaUserRanking(long arenaRanking)
	{
		AccountDataStore.instance.user.arenaRanking = (int)arenaRanking;
	}

	public void evtArenaDeckInfo(IDictionary dic)
	{
		DeckInfo deckInfo = new DeckInfo();
		deckInfo.UpdateValues(dic, true);
		deckInfo.deckType = (int)TargetUserDeckType.Arena;

		var enumerator = deckInfo.hero.GetEnumerator();
		while (enumerator.MoveNext())
			enumerator.Current.Value.idHero = enumerator.Current.Key;
		deckInfoDelegate(deckInfo);
	}

	public void evtWorldBossDeckInfo(IDictionary dic)
	{
		_evtDeckInfo(dic, TargetUserDeckType.WorldBoss);
	}

	public void evtWorldBossSeasonInfo(IDictionary dic)
	{
		AccountDataStore.instance.worldBossSeasonInfo.UpdateValues(dic, true);
	}

	public void evtUserWorldBossTotalRanking(IDictionary dic)
	{
		AccountDataStore.instance.worldBossUserTotalRanking.UpdateValues(dic, true);
	}

	public void evtUserWorldBossMaxRanking(IDictionary dic)
	{
		AccountDataStore.instance.worldBossUserBestRanking.UpdateValues(dic, true);
	}

	public void evtWorldBossTotalRanking(IList list)
	{
		_evtRanking(list, AccountDataStore.instance.worldBossTotalRanking);
	}

	public void evtWorldBossMaxRanking(IList list)
	{
		_evtRanking(list, AccountDataStore.instance.worldBossBestRanking);
	}

	private void _evtRanking(IList list, List<GiantRankingData> datas)
	{
		datas.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			GiantRankingData item = new GiantRankingData();
			item.UpdateValues((IDictionary)list[i], true);
			item.ranking = i + 1;
			datas.Add(item);
		}
	}

	public void evtDuelDummy(IDictionary dic)
	{
		int id = int.Parse(dic["idDummy"].ToString());
		AccountDataStore.instance.duel.UpdateDummyData(id);
	}

	public void evtDuelUser(IDictionary dic)
	{
		AccountDataStore.instance.duel.UpdateUserData(dic);
	}

	public void evtDuelRanking(IList list)
	{
		_evtRanking(list, AccountDataStore.instance.duelRanking);
	}

	public void evtDuelUserRanking(IDictionary dic)
	{
		AccountDataStore.instance.duelUserRanking.UpdateValues(dic, true);
	}

	public void evtDuelDeckInfo(IDictionary dic)
	{
		_evtDeckInfo(dic, TargetUserDeckType.Duel);
	}

	private void _evtDeckInfo(IDictionary dic, TargetUserDeckType type)
	{
		DeckInfo deckInfo = new DeckInfo();
		deckInfo.UpdateValues(dic, true);
		deckInfo.deckType = (int)type;

		var enumerator = deckInfo.hero.GetEnumerator();
		while (enumerator.MoveNext())
			enumerator.Current.Value.idHero = enumerator.Current.Key;
		deckInfoDelegate(deckInfo);
	}

	public void evtNoticeEvent(List<object> obj)
	{
		ChatManager.Inst.noticeDataList.Clear();
		var enumerator = obj.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ChatManager.Inst.GenerateNoticeItem(enumerator.Current);	
		}
	}

	public void evtBuffEvent(List<object> obj)
	{
		var enumerator = obj.GetEnumerator();
		while (enumerator.MoveNext())
		{
			AccountDataStore.instance.GenerateBuffEvent(enumerator.Current);	
		}
	}

	public void evtRewards(IList list)
	{
		for (int i = 0; i < list.Count; ++i)
		{
			GiantRewardData reward = new GiantRewardData();
			reward.UpdateValues((IDictionary)list[i], true);

			AccountDataStore.instance.AddReward(reward);
		}
	}

	public void evtMaxOverReward(IDictionary dic)
	{
		GiantRewardData reward = new GiantRewardData();
		reward.UpdateValues((IDictionary)dic, true);

		AccountDataStore.instance.AddMaxOverReward(reward);
	}

	public void evtLimitOverReward(IDictionary dic)
	{
		GiantRewardData reward = new GiantRewardData();
		reward.UpdateValues((IDictionary)dic, true);

		AccountDataStore.instance.AddLimitOverReward(reward);
	}

	public void evtTime(long svrTime)
	{
		timeGap = svrTime - clientTime;
	}

	public void evtExtraBossResetTime(long extraBossResetTime)
	{
		AccountDataStore.instance.extraBossResetSvrTime = extraBossResetTime;
		if (extraBossResetTimeDelegate != null)
			extraBossResetTimeDelegate();
	}

	public void evtPvpHeroDeckInfo(IDictionary dic)
	{
		DeckInfo deckInfo = new DeckInfo();
		deckInfo.UpdateValues(dic, true);
		deckInfo.deckType = (int)TargetUserDeckType.Pvp;

		var enumerator = deckInfo.hero.GetEnumerator();
		while (enumerator.MoveNext())
			enumerator.Current.Value.idHero = enumerator.Current.Key;
		if (deckInfoDelegate != null)
			deckInfoDelegate(deckInfo);
	}

	private int UpdateGiantNetElement(IDictionary dic, string key, IGiantNetElement giantNetElement, bool isClear)
	{
		try
		{
			if (dic.Contains(key))
				if (giantNetElement.UpdateValues((IDictionary)dic[key], isClear))
					return int.MaxValue;
		}
		catch (Exception exception)
		{
			Debug.LogError(Util.LogFormat("Failed UpdateValues GiantNetElement ", key, isClear, exception));
		}

		return 0;
	}
	#endregion
	
	#region arena socket
	public Action<string, IDictionary> onReceiveSocketMessageDelegate;
	public Action<bool> onDisconnectedSocketDelegate;
	private Tween m_ConnectArenaSocketDelayTween;
	private bool isCalledByCloseArenaSocket;
	private long receiveSocketMessageTime;
	
	public bool IsConnectedArenaSocket()
	{
		return m_ArenaWSClient != null;
	}
	
	public bool IsCalledByCloseArenaSocket()
	{
		return isCalledByCloseArenaSocket;
	}
	
	private void CheckConnectArenaSocket()
	{
		if (m_ArenaWSClient == null)
			return;
		if (receiveSocketMessageTime + Constant.ArenaSocketTimeout > serverTime)
		{
			Global.Inst.PushTimeout(Constant.ArenaSocketTimeout, CheckConnectArenaSocket);
			return;
		}
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			CloseArenaSocket();
			isCalledByCloseArenaSocket = false;
		}
		else
			Global.Inst.PushTimeout(Constant.ArenaSocketTimeout, CheckConnectArenaSocket);
	}

	private void ConnectArenaSocket(string url, string port)
	{
		m_ArenaWSClient = new WSClientST(Util.StringFormat(StringFormat.HTTP_URL_PORT, url, port), this);
		m_ArenaWSClient.Connect(OnReceiveSocketMessage);
		
		Global.Inst.PushTask(() => {
			recvArenaResult = null;
			isArenaResultByDisconnected = false;
			isCalledByCloseArenaSocket = false;
			
			ArenaBattleData.Reset();
		});
	}

	public void CloseArenaSocket()
	{
		if (m_ConnectArenaSocketDelayTween != null)
		{
			m_ConnectArenaSocketDelayTween.Kill();
			LoadingManager.inst.Connecting(false, false);
		}
		if (m_ArenaWSClient == null)
			return;
		isCalledByCloseArenaSocket = true;
		Debug.Log(Logger.Write("arena socket close"));
		m_ArenaWSClient.Close();
	}

	private void ArenaRequest(string json)
	{
		Debug.Log(Logger.Write("arena socket send", json));
		if (m_ArenaWSClient == null)
			return;
		m_ArenaWSClient.Send(json);
	}
	
	struct CommandHeader
	{
		public string			cmd;
	}
	
	// header
	private string GetCommandHeaderString(string cmd)
	{
		CommandHeader header	= new CommandHeader();
		header.cmd				= cmd;
		return Util.JsonEncode2(header);
	}
	
	// header + param
	private string GetSocketPacketString(string header, string param)
	{
		if (string.IsNullOrEmpty(param))
			return header;
		StringBuilder stringBuilder = Giant.Util.GetStringBuilder();
		stringBuilder.Append(header, 0, header.Length - 1);
		stringBuilder.AppendFormat(",{0}", param.Substring(1));
		return stringBuilder.ToString();
	}

	struct ArenaJoinParam
	{
		public long uidUser;
		public int seqRoom;
	}

	public void doArenaJoin()
	{
		ArenaJoinParam param = new ArenaJoinParam();
		param.uidUser = privateData.uidUser;
		param.seqRoom = AccountDataStore.instance.arena.seqRoom;
		
		string json = GetSocketPacketString(GetCommandHeaderString(ArenaCmd.arenaJoin), Util.JsonEncode2(param));
		LoadingManager.inst.Connecting(true, false);
		m_ConnectArenaSocketDelayTween = DOVirtual.DelayedCall(1f, ()=>{
				ArenaRequest(json);
				m_ConnectArenaSocketDelayTween = null;
				LoadingManager.inst.Connecting(false, false);
			});
	}
	
	public void doArenaReady()
	{
		string json = GetSocketPacketString(GetCommandHeaderString(ArenaCmd.arenaReady), null);
		ArenaRequest(json);
	}

	struct ArenaBattlePlacementParam
	{
		public GiantArenaBattlePlacementReq info;
	}
	
	public void doArenaBattlePlacement(GiantArenaBattlePlacementReq req)
	{
		ArenaBattlePlacementParam param = new ArenaBattlePlacementParam();
		param.info = req;
		param.info.uidUser = privateData.uidUser;

		string json = GetSocketPacketString(GetCommandHeaderString(ArenaCmd.arenaBattleInfo), Util.JsonEncode2(param));
		ArenaRequest(json);
	}
	
	struct ArenaBattleMoveParam
	{
		public GiantArenaBattleMoveReq info;
	}
	
	public void doArenaBattleMove(GiantArenaBattleMoveReq req)
	{
		ArenaBattleMoveParam param = new ArenaBattleMoveParam();
		param.info = req;
		param.info.uidUser = privateData.uidUser;
		
		string json = GetSocketPacketString(GetCommandHeaderString(ArenaCmd.arenaBattleInfo), Util.JsonEncode2(param));
		ArenaRequest(json);
	}
	
	struct ArenaBattleSkillParam
	{
		public GiantArenaBattleSkillReq info;
	}

	public void doArenaBattleSkill(GiantArenaBattleSkillReq req)
	{
		ArenaBattleSkillParam param = new ArenaBattleSkillParam();
		param.info = req;
		param.info.uidUser = privateData.uidUser;
		
		string json = GetSocketPacketString(GetCommandHeaderString(ArenaCmd.arenaBattleInfo), Util.JsonEncode2(param));
		ArenaRequest(json);
	}
	
	struct ArenaBattleDieParam
	{
		public GiantArenaBattleDieReq info;
	}

	public void doArenaBattleDie(GiantArenaBattleDieReq req)
	{
		ArenaBattleDieParam param = new ArenaBattleDieParam();
		param.info = req;
		param.info.uidUser = privateData.uidUser;

		string json = GetSocketPacketString(GetCommandHeaderString(ArenaCmd.arenaBattleInfo), Util.JsonEncode2(param));
		ArenaRequest(json);
	}

	struct ArenaBattleStatParam
	{
		public GiantArenaBattleStatReq info;
	}

	public void doArenaBattleStat(GiantArenaBattleStatReq req)
	{
		ArenaBattleStatParam param = new ArenaBattleStatParam();
		param.info = req;
		param.info.uidUser = privateData.uidUser;

		string json = GetSocketPacketString(GetCommandHeaderString(ArenaCmd.arenaBattleInfo), Util.JsonEncode2(param));
		ArenaRequest(json);
	}

	struct ArenaFinishParam
	{
		public string result;	// win, draw, lose
	}

	public void doArenaFinish(string result)
	{
		ArenaFinishParam param = new ArenaFinishParam();
		param.result	= result;
		
		string json = GetSocketPacketString(GetCommandHeaderString(ArenaCmd.arenaFinish), Util.JsonEncode2(param));
		ArenaRequest(json);
	}

	private void OnReceiveSocketMessage(SocketMessage socketMessage)
	{
		try
		{
			switch(socketMessage.Type)
			{
			case WsMessageType.Ws_Open:
				Debug.Log(Logger.Write("arena socket connected"));
				doArenaJoin();
				break;
			case WsMessageType.Ws_Message:
				OnReceiveSocketMessage(socketMessage.Body);
				break;
			case WsMessageType.Ws_Error:
				Debug.LogError(Logger.Write("arena socket error"));
				OnReceiveSocketCloseOrError();
				break;
			case WsMessageType.Ws_Close:
				Debug.Log(Logger.Write("arena socket closed"));
				OnReceiveSocketCloseOrError();
				break;
			default:
				Debug.LogWarning(socketMessage.Type);
				Debug.LogWarning(socketMessage);
				break;
			}
		}
		catch (Exception e)
		{
			Debug.LogError(Logger.Write("error", "GiantHandler.OnReceiveSocketMessage", e.ToString()));
		}
	}
	
	private void OnReceiveSocketCloseOrError()
	{
		if (m_ArenaWSClient == null)
			return;

		m_ArenaWSClient = null;
		if (onDisconnectedSocketDelegate != null)
			onDisconnectedSocketDelegate(isCalledByCloseArenaSocket);
	}

	private void OnReceiveSocketMessage(string strBody)
	{
		receiveSocketMessageTime = serverTime;
		
		Global.Inst.PushTimeout(Constant.ArenaSocketTimeout, CheckConnectArenaSocket);
		
		IDictionary dic = (IDictionary)Util.JsonDecode(strBody);
		string cmd = null;
		if (dic.Contains("cmd") && dic["cmd"] != null)
			cmd = (string)dic["cmd"];

		if (string.IsNullOrEmpty(cmd))
			return;
		
		Debug.Log(Logger.Write("arena socket recv", strBody));

		switch (cmd)
		{
		case ArenaCmd.arenaStart:
			if (dic.Contains("time"))
				evtTime(long.Parse(dic["time"].ToString()));
			break;
		case ArenaCmd.arenaResult:
			if (dic.Contains("result"))
				recvArenaResult = dic["result"].ToString();
			if (dic.Contains("reason"))
				isArenaResultByDisconnected = (dic["reason"].ToString()).Equals("disconnect");
			break;
		case ArenaCmd.error:
			if (dic.Contains("msg"))
				DefaultRequestFailure(dic["msg"]);
			break;
		}
		
		if (onReceiveSocketMessageDelegate != null)
			onReceiveSocketMessageDelegate(cmd, dic);
	}
	#endregion

	#region public function
	private long clientTime { get { return Util.Millis; } }
	public long serverTime { get {
		// 동일한 프레임에서는 serverTime 으로 항상 같은 값을 반환한다.
		if (timeFrameCount != Time.frameCount)
		{
			timeFrameCount = Time.frameCount;
			curServerTime = clientTime + timeGap;
		}
		return curServerTime;
	} }
	public long serverOffsetTime { get {
		DateTime dt = new System.DateTime(1, 1, 1);
		dt = dt.AddMilliseconds(serverTime + timeOffset);
		return dt.Ticks / 10000;
	} }
	#endregion

	#region private function
	private string GetPlatform()
	{
#if UNITY_EDITOR
		return "U";
#elif UNITY_ANDROID
		return "A";
#elif UNITY_IOS
		return "I";
#else
		return "E";
#endif
	}
	
	private string GetMarket()
	{
#if UNITY_ANDROID
		return "GO";
#elif UNITY_IOS
		return "AP";
#else
		return "ETC";
#endif
	}
	#endregion
}
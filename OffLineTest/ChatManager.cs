using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SocketMessage
{
	public string Body {
		get; set;
	}

	public WsMessageType Type {
		get; set;
	}

	public string Summary {
		get; set;
	}

	public override string ToString ()
	{
		return String.Format ("{0}: {1}", Summary, Body);
	}
}


// header
public class ChatSendHeader			
{
	public string			cmd;
	public string			channel;
	public string			uidUser;
	public string			nick;
	public string			msg;
	public string			guild;
	public string			party;
	public string			desType;
	public string			desId;
	public int	 			doerLv;
	public int 				authority;
	public int 				isParsedMsg;
	public long 			timestamp;
}

// Message
class MessageParam
{
	public int 				type;
	public string 			message;
}

public class ChatData
{
	public CHAT_TYPE 		chatType;
	public CHAT_FUNC		chatFunc;
	public CHAT_SUB_TYPE	chatSubType = CHAT_SUB_TYPE.NONE;

	private ChatRecvData 	receiveData;	
	public ChatRecvData		GetReceiveData { get{ return receiveData;}}

	public static int SortMessage(ChatData data1, ChatData data2)
	{
		return	data1.GetReceiveData.timestamp.CompareTo(data2.GetReceiveData.timestamp);
	}

	public bool IsMyMessage()
	{
		if (GiantHandler.Inst.GetUidUser().ToString().CompareTo(receiveData.uidUser) == 0)
			return true;
		return false;
	}
		
	public string GetChatTimeString()
	{
		DateTime currTime = Giant.Util.GetDateTimeFromUnixTime(GiantHandler.Inst.serverTime);
		currTime += Giant.Util.GetLocalTimeOffset();

		DateTime dateTime = Giant.Util.GetDateTimeFromUnixTime(receiveData.timestamp);
		dateTime += Giant.Util.GetLocalTimeOffset();

		if (currTime.Day.Equals(dateTime.Day))
		{
			int hour = dateTime.Hour;
			if (DeviceSaveManager.Inst.GetLanguage().Equals(Language.DE))
				return Datatable.Inst.GetUIText(UITextEnum.GUILD_CHAT_TIME, hour, dateTime.Minute);
			else
			{
				if (hour >= 12)
				{
					if (hour > 12)
						hour -= 12;
					return Datatable.Inst.GetUIText(UITextEnum.GUILD_CHAT_TIME_PM, hour, dateTime.Minute);
				}
				else
				{
					return Datatable.Inst.GetUIText(UITextEnum.GUILD_CHAT_TIME_AM, hour, dateTime.Minute);	
				}	
			}
		}
		return dateTime.ToShortTimeString();
	}

	public void SetReceiveData(ChatRecvData data)
	{
		if (data != null)
		{
			data.ParseMessage();
			receiveData = data;
			if(!string.IsNullOrEmpty(receiveData.cmd))
				chatFunc = ChatManager.GetChatFuncTypeFromStr(receiveData.cmd);

			if(!string.IsNullOrEmpty(receiveData.desType))
				chatType = ChatManager.GetChatTypeFromStr(receiveData.desType);	

			if (chatFunc == CHAT_FUNC.SYSTEM_MSG)
			{
				if (!string.IsNullOrEmpty(data.GetSubtype()))
					chatSubType = ChatManager.GetChatSubTypeFromStr(data.GetSubtype());
			}
			else if (chatFunc == CHAT_FUNC.ERROR)
			{
				chatType = CHAT_TYPE.NORMAL;
			}
		}
	}
}

public enum NOTICE_STATUS
{
	SET_NOTICE,
	RESTART,
	STOP,
	DELETE,
	NONE,
}

public enum BUFF_EVENT_STATUS
{
	SET_BUFF,
	MODIFY,
	RESTART,
	STOP,
	DELETE,
	NONE,
}

public enum NOTICE_TYPE
{
	ALL,
	CHATTING,
	TICKER,
	FIXED,
}

public class _NoticeData
{
	public long 			_id;
	public int 				noticeType;		// 0:ALL, 1:채팅, 2:티커, 3:고정공지
	public int 				dateType;		// 1:서버시간, 2:KST 기준
	public long 			startDate;
	public long 			endDate;
	public int 				repeatMinute;
	public int 				repeatCount;
	public int 				idNoticeReason;
	public string 			messageKor;
	public string 			messageEng;
	public string 			messageChnsm;
	public string 			messageChntr;
	public string			messageDeu;
	public long 			stopActivation;
}

public class NoticeData : _NoticeData
{
	public List<long> 		noticeTimeList = new List<long>();

	public string 			GetMessage()
	{
		switch (DeviceSaveManager.Inst.GetLanguage())
		{
		case Language.KO:
			return messageKor;
		case Language.EN:
			return messageEng;
		case Language.ZHT:
			return messageChntr;
		case Language.ZHS:
			return messageChnsm;
		case Language.DE:
			return messageDeu;
		default:
			return messageEng;
		}
	}
}

public class _ChatRecvData
{
	public string 			cmd;
	public string 			desType;
	public string 			uidUser;
	public string 			desId;
	public string 			nick;
	public long 			timestamp;	
}

public class ChatRecvData : _ChatRecvData 
{
	public virtual string 	GetMessage()		{ return string.Empty;}
	public virtual string 	GetStateGuild()		{ return string.Empty;}
	public virtual string 	GetSubtype()		{ return string.Empty;}
	public virtual string 	GetReqID()			{ return string.Empty;}
	public virtual string	GetDoer()			{ return string.Empty;}
	public virtual string 	GetTarget()			{ return string.Empty;}
	public virtual string 	GetRating()			{ return string.Empty;}
	public virtual int		GetDoerLv()			{ return -1;}
	public virtual int 		GetTargetLv()		{ return -1;}
	public virtual int 		GetAutority()		{ return -1;}
	public virtual int	 	GetIdItem()			{ return -1;}
	public virtual int 		GetAmount()			{ return -1;}
	public virtual int	 	GetIdBuilding()		{ return -1;}
	public virtual int 		GetBuildingLv()		{ return -1;}
	public string 			GetNickname()		{ return Giant.Util.CheckNicknameEmpty(nick); }
	public virtual ChatMsgType GetMsgType()		{ return ChatMsgType.Normal;} 
	public virtual void 	ParseMessage()		{}
}

public class ChatRecvDataMessage : ChatRecvData
{
	public int 				isParsedMsg;
	public string 			msg;
	public ChatMsgType 		msgType;
	public int	 			doerLv;
	public int 				authority;
	public override string 	GetMessage()		{ return msg; }
	public override int		GetDoerLv()			{ return doerLv;}
	public override int 	GetAutority()		{ return authority;}
	public override ChatMsgType GetMsgType()		{ return msgType;} 
	public override void	ParseMessage()
	{
		msgType = ChatMsgType.Normal;
		if (isParsedMsg > 0)
		{
			MessageParam data = new MessageParam();
			object obj = Util.JsonDecode(msg);
			if ((IDictionary)obj != null)
			{
				Util.GetFields(data, obj);
				if (data != null && data.type == (int)ChatMsgType.HeroEvo)
				{
					msg = data.message;
					msgType = (ChatMsgType)data.type;
				}
			}	
		}
	}
}

public class ChatRecvDataLeave : ChatRecvData
{
	public string 			guild;
	public override string 	GetStateGuild()		{ return guild;}
}

public class ChatRecvDataSysMsg : ChatRecvData
{
	public string 			subType;
	public string 			msg;
	public string 			reqId;
	public string 			doer;
	public int	 			doerLv;
	public string 			target;
	public int 				targetLv;
	public int 				authority;
	public string 			rating;
	public int 				idItem;
	public int 				amount;
	public int 				idBuilding;
	public int 				buildingLv;

	public override string 	GetMessage()		{ return msg;}
	public override string 	GetSubtype()		{ return subType;}
	public override string 	GetReqID()			{ return reqId;}
	public override string	GetDoer()			{ return doer;}
	public override string 	GetTarget()			{ return target;}
	public override string 	GetRating()			{ return rating;}
	public override int		GetDoerLv()			{ return doerLv;}
	public override int 	GetTargetLv()		{ return targetLv;}
	public override int 	GetAutority()		{ return authority;}
	public override int 	GetIdItem()			{ return idItem;}
	public override int 	GetAmount()			{ return amount;}
	public override int 	GetIdBuilding()		{ return idBuilding;}
	public override int 	GetBuildingLv()		{ return buildingLv;}
}

public class ChatManager {

	private static ChatManager _inst = new ChatManager();
	public static ChatManager Inst { get { return _inst; } }

//	private const string 					_serverURL = "ws://192.168.1.204:10000";
	public const string _strChannel			= "channel";
	public const string _strGuild			= "guild";
	public const string _strAll				= "all";

	private const int						m_MaxChatCount = 30;
	public int 								MaxChatCount{ get { return m_MaxChatCount;}}

	private const int						m_MaxGuildChatCount = 100;
	public int 								MaxGuildChatCount{ get { return m_MaxGuildChatCount;}}

	private WSClientST 						m_wsClient = null;
	public List<ChatData> 					chatDataList = new List<ChatData>();
	public List<ChatData> 					guildChatDataList = new List<ChatData>();
	public List<NoticeData> 				noticeDataList = new List<NoticeData>();

	public event Action<ChatData>		 	responseMsgAction = null;
	public event Action 					guildNotiAction = null;			// 일반 채팅은 노티를 띄어주지 않는다

	private bool 							m_InitNotice = false;
	private bool 							m_InitGuildChat = false;
	private bool 							m_DirtyFlag  = false;
	private string 							m_StateChannel;
	public string 							CurrentChannel{ get { return m_StateChannel;}}
	public bool 							DirtyUpdateFlag{ get {return m_DirtyFlag;} set { m_DirtyFlag = value;}}
	public bool								InitNotice{ get { return m_InitNotice;} set{ m_InitNotice = value;}}

	private int 							m_RetryCount = 0;
	private const int 						MAX_RETRY_COUNT = 1;
	private object							m_LockObject = new object();
	private bool 							m_IsWsConnecting;

	public bool IsOnGuildChatNotice()
	{
		long savedNotiTimeStamp = DeviceSaveManager.Inst.LoadChatNotiTimeStamp();
		long lastChatNotiTimeStamp = GetLatestGuildChatTimeStamp();

		if (lastChatNotiTimeStamp > 0 && savedNotiTimeStamp < lastChatNotiTimeStamp)
			return true;
		else
			return false;
	}

	public long GetLatestGuildChatTimeStamp()
	{
		long guildTimeStamp = -1;

		if (guildChatDataList.Count > 0)
		{
			guildTimeStamp = guildChatDataList [guildChatDataList.Count - 1].GetReceiveData.timestamp;
		}

		return guildTimeStamp;
	}

	public bool IsConnected()
	{
		if (m_wsClient != null && m_wsClient.IsConnected())
			return true;
		return false;
	}

	public void ConnectWS()
	{
		// OffLineTest
		return;
		if (IsConnected())
		{
			Debug.Log (Logger.Write("Already Connected"));
			return;
		}

		Action action = () => { 
			lock(m_LockObject)
			{
				m_wsClient = new WSClientST(GiantHandler.Inst.chatServerURL, this);
				m_wsClient.Connect ((socketMsg) => { 
					ResponseData (socketMsg);
				});				
				m_IsWsConnecting = false;
			}
		};

		if (!m_IsWsConnecting)
		{
			m_IsWsConnecting = true;
			Global.Inst.InstantiateThread(action);
		}
	}

	public void CloseWS()
	{
		Debug.Log(Logger.Write("Chatmanager CloseWS"));
		if (m_wsClient != null)
		{
			m_wsClient.Close();
		}
	}

	private void OnDisconnectedWS() 
	{
		Debug.Log(Logger.Write("OnDisconnectedWS"));
		if (m_wsClient == null)
			return;
		m_wsClient = null;
		if (!LoginManager.Inst.isLogined || GameManager.Inst.PauseSatus)
			return;
	
		if (m_RetryCount++ <= MAX_RETRY_COUNT)
		{
			ConnectWS();	
		}
	}

	private void SendData(string json)
	{
		m_wsClient.Send (json);		
	}

	void InitChat()
	{
		ChatSendHeader header		= new ChatSendHeader();
		header.cmd					= "init";
		header.uidUser				= GiantHandler.Inst.GetUidUser().ToString();
		header.nick 				= AccountDataStore.instance.user.nickname;
		string json 				= Util.JsonEncode2(header);

		SendData(json);
		m_InitGuildChat	 			= false;
		m_RetryCount 				= 0;
	}

	public void JoinChat(CHAT_TYPE chatType, string idChannel)
	{
		ChatSendHeader header		= new ChatSendHeader();
		header.cmd					= "join";

		if (chatType == CHAT_TYPE.NORMAL)
		{
			header.desType 			= _strChannel;
			header.desId 			= idChannel;	
		}
		else
		{
			header.desType 			= _strGuild;
			header.desId 			= idChannel;
		}

		string json 				=  Util.JsonEncode2(header);
		SendData(json);
	}

	public void LeaveChat(CHAT_TYPE chatType)
	{
		ChatSendHeader header 		= new ChatSendHeader();
		header.cmd					= "leave";

		switch (chatType)
		{
		case CHAT_TYPE.ALL:
			header.desType 			= _strAll;
			break;
		case CHAT_TYPE.GUILD:
			header.desType 			= _strGuild;
			break;
		case CHAT_TYPE.NORMAL:
			header.desType 			= _strChannel;
			break;
		}

		string json 				= Util.JsonEncode2(header);
		SendData(json);
	}

	public void SendMessageChat(CHAT_TYPE chatType, string msg, ChatMsgType messageType = ChatMsgType.Normal)
	{
		ChatSendHeader header		= new ChatSendHeader();
		header.cmd					= "message";
		header.nick 				= AccountDataStore.instance.user.nickname;
		header.desId				= "";
		header.doerLv 				= AccountDataStore.instance.user.level;
		if (messageType != ChatMsgType.Normal)
			header.isParsedMsg = 1;
		switch (chatType)
		{
		case CHAT_TYPE.GUILD:
			if (AccountDataStore.instance.clientData.guildChatBlockEndTime > GiantHandler.Inst.serverTime)
			{
				long timeDiff = (long)((AccountDataStore.instance.clientData.guildChatBlockEndTime - GiantHandler.Inst.serverTime) * 0.001 + 1);
				string message = Datatable.Inst.GetUIText(UITextEnum.CHAT_BLOCK_MESSAGE_01, Giant.Util.GetFormattedTime((int)timeDiff, Constant.FormattedTimeSize));
				AddChatSystemMessage(message, chatType);
				return;
			}

			header.authority = AccountDataStore.instance.guildData.GetGuildMyMemberData().authority;
			header.desType = _strGuild;
			header.msg = msg;
			break;
		case CHAT_TYPE.NORMAL:
			if (AccountDataStore.instance.clientData.chatBlockEndTime > GiantHandler.Inst.serverTime)
			{
				long timeDiff = (long)((AccountDataStore.instance.clientData.chatBlockEndTime - GiantHandler.Inst.serverTime) * 0.001 + 1);
				string message = Datatable.Inst.GetUIText(UITextEnum.CHAT_BLOCK_MESSAGE_01, Giant.Util.GetFormattedTime((int)timeDiff, Constant.FormattedTimeSize));
				AddChatSystemMessage(message, chatType);
				return;
			}

			header.desType = _strChannel;
			header.msg = msg;
			break;
		case CHAT_TYPE.ALL:
			{
				MessageParam message = new MessageParam();
				message.message = msg;
				message.type = (int)messageType;
				header.msg = Util.JsonEncode2(message);
				header.desType = _strAll;
			}
			break;
		}

		if (CheckOverChat(chatType))
		{
			int limitSeconds = Datatable.Inst.dtChatInputLimit[Datatable.Inst.dtChatInputLimit.Count].LimitTime;
			if (chatType == CHAT_TYPE.NORMAL)
			{
				AccountDataStore.instance.clientData.chatBlockIndex++;
				if (Datatable.Inst.dtChatInputLimit.ContainsKey(AccountDataStore.instance.clientData.chatBlockIndex))
				{
					limitSeconds = Datatable.Inst.dtChatInputLimit[AccountDataStore.instance.clientData.chatBlockIndex].LimitTime;
				}
				AccountDataStore.instance.clientData.chatBlockEndTime = GiantHandler.Inst.serverTime + (limitSeconds * 1000);
			}
			else
			{
				AccountDataStore.instance.clientData.guildChatBlockIndex++;
				if (Datatable.Inst.dtChatInputLimit.ContainsKey(AccountDataStore.instance.clientData.guildChatBlockIndex))
				{
					limitSeconds = Datatable.Inst.dtChatInputLimit[AccountDataStore.instance.clientData.guildChatBlockIndex].LimitTime;
				}
				AccountDataStore.instance.clientData.guildChatBlockEndTime = GiantHandler.Inst.serverTime + (limitSeconds * 1000);
			}

			GiantHandler.Inst.doClientData(AccountDataStore.instance.clientData).SetSuccessAction(() =>
			{
				string message = Datatable.Inst.GetUIText(UITextEnum.CHAT_BLOCK_MESSAGE_02, Giant.Util.GetFormattedTime(limitSeconds, Constant.FormattedTimeSize));
				AddChatSystemMessage(message, chatType);
			});
		}
		else
		{
			string json 				=  Util.JsonEncode2(header);
			SendData(json);		
		}
	}

	public void RequestHistory(CHAT_TYPE chatType)
	{
		ChatSendHeader header		= new ChatSendHeader();
		header.cmd					= "history";
		header.timestamp 			= AccountDataStore.instance.guildData.GetGuildMyMemberData().joinDate;

		switch (chatType)
		{
		case CHAT_TYPE.ALL:
			header.desType 			= _strAll;
			break;
		case CHAT_TYPE.GUILD:
			header.desType 			= _strGuild;
			break;
		case CHAT_TYPE.NORMAL:
			header.desType 			= _strChannel;
			break;
		}

		string json 				=  Util.JsonEncode2(header);
		SendData(json);
	}

	public ChatData GetPrevChatData()
	{
		if (chatDataList.Count > 1)
		{
			return chatDataList[chatDataList.Count - 2];
		}
		return null;
	}

	void ResponseData(SocketMessage socketData)
	{
		try
		{
			Debug.Log(Logger.Write("ResponseData : ", socketData.ToString()));
			switch (socketData.Type) 
			{
			case WsMessageType.Ws_Open:
				InitChat();
				break;
			case WsMessageType.Ws_Message:
				ProcessMessage(socketData.Body);
				break;
			case WsMessageType.Ws_Error:
				OnDisconnectedWS();
				break;
	
			case WsMessageType.Ws_Close:
				OnDisconnectedWS();
				break;
			}
		}
		catch (Exception e)
		{
			Debug.LogError(Logger.Write("error", "ChatManager.ResponseData", e.ToString()));
		}
	}

	public void ClearMessageItems()
	{
		chatDataList.Clear();
		guildChatDataList.Clear();
	}

	public void AddMessageItem(ChatData chatData)
	{
		switch (chatData.chatType)
		{
		case CHAT_TYPE.ALL:
		case CHAT_TYPE.NORMAL:
			chatDataList.Add(chatData);		
			break;
		case CHAT_TYPE.GUILD:
			guildChatDataList.Add(chatData);
			break;
		}

		if(responseMsgAction != null)
			responseMsgAction(chatData);		
	}

	private bool CheckOverChat(CHAT_TYPE chatType)
	{
		if (chatType == CHAT_TYPE.ALL)
			return false;
		
		List<ChatData> chatList = chatDataList;
		if (chatType == CHAT_TYPE.GUILD)
			chatList = guildChatDataList;
		
		List<ChatData> listMyData = chatList.FindAll((item)=>{
			return (item.chatFunc == CHAT_FUNC.MESSAGE && item.IsMyMessage());
		});

		if (listMyData.Count + 1 >= Datatable.Inst.settingData.ChatCheckCount)
		{
			int count = 0;
			for (int i = listMyData.Count - 1; i >= 0; i--)
			{
				if ((GiantHandler.Inst.serverTime - listMyData[i].GetReceiveData.timestamp) > (Datatable.Inst.settingData.ChatCheckTime * 1000))
					break;
				count++;
			}
			if (count + 1 >= Datatable.Inst.settingData.ChatCheckCount)
				return true;
		}
		return false;
	}

	private void SortChatList()
	{
		chatDataList.Sort(ChatData.SortMessage);
		guildChatDataList.Sort(ChatData.SortMessage);
	}

	private void ProcessMessage(string strBody)
	{
		IDictionary dic = (IDictionary)Util.JsonDecode(strBody);

		ChatData chatData = new ChatData();
		chatData.chatFunc = CHAT_FUNC.NONE;

		if (dic.Contains("cmd") && dic["cmd"]!= null)
		{ 
			chatData.chatFunc = ChatManager.GetChatFuncTypeFromStr(dic["cmd"].ToString());
		}

		switch (chatData.chatFunc)
		{
		case CHAT_FUNC.NOTICE:
			{
				IDictionary dicNotice = (IDictionary)dic["notice"];

				if (dicNotice.Contains("param") && dicNotice["param"] != null)
				{
					object param = dicNotice["param"];
					NOTICE_STATUS status = NOTICE_STATUS.SET_NOTICE;
					if(dicNotice["status"] != null)
						status = ChatManager.GetNoticeTypeFromStr(dicNotice["status"].ToString());
					
					switch (status)
					{
					case NOTICE_STATUS.SET_NOTICE:
						GenerateNoticeItem(param);
						break;
					case NOTICE_STATUS.DELETE:
						RemoveNoticeItem(param);
						break;
					case NOTICE_STATUS.RESTART:
						RestartNoticeItem(param);
						break;
					case NOTICE_STATUS.STOP:
						StopNoticeItem(param);
						break;
					}					
				}
			}
			break;
		case CHAT_FUNC.BUFF_EVENT:
			{
				IDictionary dicBuffEvent = (IDictionary)dic["buff"];

				if (dicBuffEvent.Contains("param") && dicBuffEvent["param"] != null)
				{
					object param = dicBuffEvent["param"];
					BUFF_EVENT_STATUS status = BUFF_EVENT_STATUS.SET_BUFF;
					if(dicBuffEvent["status"] != null)
						status = ChatManager.GetBuffTypeFromStr(dicBuffEvent["status"].ToString());

					switch (status)
					{
					case BUFF_EVENT_STATUS.SET_BUFF:
						AccountDataStore.instance.GenerateBuffEvent(param);
						break;
					case BUFF_EVENT_STATUS.MODIFY:
						AccountDataStore.instance.MotifyBuffEvent(param);
						break;
					case BUFF_EVENT_STATUS.DELETE:
						AccountDataStore.instance.RemoveBuffEvent(param);
						break;
					case BUFF_EVENT_STATUS.RESTART:
						AccountDataStore.instance.RestartBuffEvent(param);
						break;
					case BUFF_EVENT_STATUS.STOP:
						AccountDataStore.instance.StopBuffEvent(param);
						break;
					}					
				}
			}
			break;
		case CHAT_FUNC.INIT:
			break;
		case CHAT_FUNC.JOIN:
			{
				ChatRecvData data = new ChatRecvData();
				Util.GetFields(data, Util.JsonDecode(strBody));
				chatData.SetReceiveData(data);

				if (chatData.chatType == CHAT_TYPE.NORMAL)
				{
					if (!m_InitGuildChat)
					{
						if (AccountDataStore.instance.IsOnGuildMembership())
							ChatManager.Inst.JoinChat(CHAT_TYPE.GUILD, AccountDataStore.instance.guildData._id);
						m_InitGuildChat = true;	
					}
					m_StateChannel = data.desId;
					AddMessageItem(chatData);
				}
				else if (chatData.chatType == CHAT_TYPE.GUILD)
				{
					if (chatData.GetReceiveData.uidUser == GiantHandler.Inst.GetUidUser().ToString())
					{
						ChatManager.Inst.RequestHistory(CHAT_TYPE.GUILD);
					}
				}
			}
			break;
		case CHAT_FUNC.LEAVE:
			{
				ChatRecvDataLeave data = new ChatRecvDataLeave();
				Util.GetFields(data, Util.JsonDecode(strBody));
				chatData.SetReceiveData(data);

				if (chatData.IsMyMessage())
				{
					switch (chatData.chatType)
					{
					case CHAT_TYPE.ALL:
					case CHAT_TYPE.NORMAL:
						chatDataList.Clear();
						break;
					case CHAT_TYPE.GUILD:
						guildChatDataList.Clear();
						break;
					}
				}
			}
			break;
		case CHAT_FUNC.MESSAGE:
			{
				ChatRecvDataMessage data = new ChatRecvDataMessage();
				Util.GetFields(data, Util.JsonDecode(strBody));
				chatData.SetReceiveData(data);

				AddMessageItem(chatData);
			}
			break;
		case CHAT_FUNC.ERROR:
			{
				ChatRecvDataSysMsg data = new ChatRecvDataSysMsg();
				Util.GetFields(data, Util.JsonDecode(strBody));
				data.msg = Datatable.Inst.GetServerErrorText(data.msg);
				chatData.SetReceiveData(data);
				if (chatData.chatType == CHAT_TYPE.NORMAL)
					AddMessageItem(chatData);
			}
			break;
		case CHAT_FUNC.SYSTEM_MSG:
			{
				ChatRecvDataSysMsg data = new ChatRecvDataSysMsg();
				Util.GetFields(data, Util.JsonDecode(strBody));
				chatData.SetReceiveData(data);

				if (chatData.chatSubType == CHAT_SUB_TYPE.ACCEPT_SIGNUP ||
					chatData.chatSubType == CHAT_SUB_TYPE.DENY_SIGNUP)
				{
					for (int i = guildChatDataList.Count - 1; i >= 0; i--)
					{
						if (guildChatDataList[i].chatSubType == CHAT_SUB_TYPE.REQUEST_SIGNUP && 
							guildChatDataList[i].GetReceiveData.GetReqID() == chatData.GetReceiveData.GetReqID())
						{
							guildChatDataList.RemoveAt(i);
						}						
					}
				}
				AddMessageItem(chatData);
			}
			break;
		case CHAT_FUNC.REQ_JOIN:
			{
				if (dic.Contains("reqId"))
				{
					string idReq = dic["reqId"].ToString();
					GiantHandler.Inst.doGetReqUser(idReq);
				}
			}
			break;
		case CHAT_FUNC.HISTORY:
			{
				if (dic.Contains("history"))
				{
					guildChatDataList.Clear();
					IList listHistory = (IList)dic["history"];
					var enumerator = listHistory.GetEnumerator();

					while (enumerator.MoveNext())
					{
						IDictionary dicHistory = (IDictionary)enumerator.Current;
						CHAT_FUNC funcHistory = CHAT_FUNC.NONE;

						if (dicHistory.Contains("cmd") && dicHistory["cmd"]!= null)
						{ 
							funcHistory = ChatManager.GetChatFuncTypeFromStr(dicHistory["cmd"].ToString());
						}

						switch (funcHistory)
						{
						case CHAT_FUNC.MESSAGE:
							{
								ChatData historyData = new ChatData();
								ChatRecvDataMessage data = new ChatRecvDataMessage();
								Util.GetFields(data, enumerator.Current);
								historyData.SetReceiveData(data);
								guildChatDataList.Add(historyData);	
							}
							break;

						case CHAT_FUNC.SYSTEM_MSG:
							{
								ChatData historyData = new ChatData();
								ChatRecvDataSysMsg data = new ChatRecvDataSysMsg();
								Util.GetFields(data, enumerator.Current);
								historyData.SetReceiveData(data);
								guildChatDataList.Add(historyData);	
							}
							break;	
						}
					}
				}
				SortChatList();

				ChatData history = new ChatData();
				history.chatType = CHAT_TYPE.GUILD;
				history.chatFunc = CHAT_FUNC.HISTORY;
				if(responseMsgAction != null)
					responseMsgAction(history);

			}
			break;
		case CHAT_FUNC.STATE:
			break;
		case CHAT_FUNC.NONE:
			break;
		}

		switch (chatData.chatFunc)
		{
		case CHAT_FUNC.LEAVE:
		case CHAT_FUNC.MESSAGE:
			if (chatData.GetReceiveData.uidUser != GiantHandler.Inst.GetUidUser().ToString())
			{
				if(guildNotiAction != null && chatData.chatType == CHAT_TYPE.GUILD)
					guildNotiAction();
			}
			break;
		case CHAT_FUNC.SYSTEM_MSG:
		case CHAT_FUNC.REQ_JOIN:
			{
				if(guildNotiAction != null && chatData.chatType == CHAT_TYPE.GUILD)
					guildNotiAction();	
			}
			break;
		case CHAT_FUNC.JOIN:
			break;
		default:
			break;
		}
	}

	public static CHAT_TYPE GetChatTypeFromStr(string str)
	{
		if (string.IsNullOrEmpty(str))
			return CHAT_TYPE.NONE;
		
		if (str.CompareTo("channel") == 0)
		{
			return CHAT_TYPE.NORMAL;
		}
		else if (str.CompareTo("guild") == 0)
		{
			return CHAT_TYPE.GUILD;
		}
		else if (str.CompareTo("all") == 0)
		{
			return CHAT_TYPE.ALL;
		}
		else
		{
			return CHAT_TYPE.NONE;
		}
	}

	public static CHAT_FUNC GetChatFuncTypeFromStr(string str)
	{
		if(string.IsNullOrEmpty(str))
			return CHAT_FUNC.NONE;

		if (str.CompareTo("init") == 0)
		{
			return CHAT_FUNC.INIT;
		}
		else if (str.CompareTo("state") == 0)
		{
			return CHAT_FUNC.STATE;
		}
		else if (str.CompareTo("message") == 0)
		{
			return CHAT_FUNC.MESSAGE;
		}
		else if (str.CompareTo("sysmsg") == 0)
		{
			return CHAT_FUNC.SYSTEM_MSG;
		}
		else if (str.CompareTo("sysnoti") == 0)
		{
			return CHAT_FUNC.REQ_JOIN;
		}
		else if (str.CompareTo("join") == 0)
		{
			return CHAT_FUNC.JOIN;
		}
		else if (str.CompareTo("leave") == 0)
		{
			return CHAT_FUNC.LEAVE;
		}
		else if (str.CompareTo("error") == 0)
		{
			return CHAT_FUNC.ERROR;
		}
		else if (str.CompareTo("history") == 0)
		{
			return CHAT_FUNC.HISTORY;
		}
		else if (str.CompareTo("notice") == 0)
		{
			return CHAT_FUNC.NOTICE;
		}
		else if (str.CompareTo("buff") == 0)
		{
			return CHAT_FUNC.BUFF_EVENT;
		}
		else
		{
			return CHAT_FUNC.NONE;
		}
	}

	public static CHAT_SUB_TYPE GetChatSubTypeFromStr(string str)
	{
		if (str.CompareTo("changeSettingMsg") == 0)
		{
			return CHAT_SUB_TYPE.CHANGE_SETTING;
		}
		else if (str.CompareTo("signUpMsg") == 0)
		{
			return CHAT_SUB_TYPE.SIGNUP;
		}
		else if (str.CompareTo("denySignUpMsg") == 0)
		{
			return CHAT_SUB_TYPE.DENY_SIGNUP;
		}
		else if (str.CompareTo("acceptSignUpMsg") == 0)
		{
			return CHAT_SUB_TYPE.ACCEPT_SIGNUP;
		}
		else if (str.CompareTo("changeAuthorityMsg") == 0)
		{
			return CHAT_SUB_TYPE.CHANGE_AUTH;
		}
		else if (str.CompareTo("exileMsg") == 0)
		{
			return CHAT_SUB_TYPE.EXILE_SIGNUP;
		}
		else if (str.CompareTo("secedeMsg") == 0)
		{
			return CHAT_SUB_TYPE.SECEDE_SIGNUP;
		}
		else if (str.CompareTo("donateStockForGuildMsg") == 0)
		{
			return CHAT_SUB_TYPE.DONATE_STOCK;
		}
		else if (str.CompareTo("notifyFullStockGuildResMsg") == 0)
		{
			return CHAT_SUB_TYPE.FULL_STOCK_NOTI;
		}
		else if (str.CompareTo("upgradeGuildBuildingMsg") == 0)
		{
			return CHAT_SUB_TYPE.UPGRADE_BUILDING;
		}
		else if (str.CompareTo("targetGuildShareMsg") == 0)
		{
			return CHAT_SUB_TYPE.TARGET_GUILD_SHARE;
		}
		else if (str.CompareTo("joinGuildBattleReward") == 0)
		{
			return CHAT_SUB_TYPE.JOIN_GUILD_BATTLE_REWARD;
		}
		else if (str.CompareTo("reJoinGuildBattle") == 0)
		{
			return CHAT_SUB_TYPE.REJOIN_GUILD_BATTLE;
		}
		else if (str.CompareTo("getCoinDefenseBonus") == 0)
		{
			return CHAT_SUB_TYPE.GET_COIN_DEFENSE_BONUS;
		}
		else if (str.CompareTo("getCoinBrokenGuild") == 0)
		{
			return CHAT_SUB_TYPE.GET_COIN_BROKEN_GUILD;
		}
		else
		{
			return CHAT_SUB_TYPE.NONE;
		}
	}

	public static BUFF_EVENT_STATUS GetBuffTypeFromStr(string str)
	{
		if (str.CompareTo("setBuff") == 0)
		{
			return BUFF_EVENT_STATUS.SET_BUFF;
		}
		else if (str.CompareTo("modifyBuffEvent") == 0)
		{
			return BUFF_EVENT_STATUS.MODIFY;
		}
		else if (str.CompareTo("stopBuff") == 0)
		{
			return BUFF_EVENT_STATUS.STOP;
		}
		else if (str.CompareTo("restartBuff") == 0)
		{
			return BUFF_EVENT_STATUS.RESTART;
		}
		else if (str.CompareTo("deleteBuff") == 0)
		{
			return BUFF_EVENT_STATUS.DELETE;
		}
		else
		{
			return BUFF_EVENT_STATUS.NONE;
		}
	}

	public static NOTICE_STATUS GetNoticeTypeFromStr(string str)
	{
		if (str.CompareTo("setNotice") == 0)
		{
			return NOTICE_STATUS.SET_NOTICE;
		}
		else if (str.CompareTo("stopNotice") == 0)
		{
			return NOTICE_STATUS.STOP;
		}
		else if (str.CompareTo("restartNotice") == 0)
		{
			return NOTICE_STATUS.RESTART;
		}
		else if (str.CompareTo("deleteNotice") == 0)
		{
			return NOTICE_STATUS.DELETE;
		}
		else
		{
			return NOTICE_STATUS.NONE;
		}
	}

#region Notice
	public void GenerateNoticeItem(object obj)
	{
		NoticeData data = new NoticeData();
		Util.GetFields(data, obj);

		if (data.endDate <= GiantHandler.Inst.serverTime)
			return;
		
		long startTime = data.startDate;
		if (startTime < GiantHandler.Inst.serverTime)
			startTime = GiantHandler.Inst.serverTime;
		
		if (data != null)
		{
			if (noticeDataList.FindIndex(x => x._id == data._id) >= 0)
			{
				Debug.Log("Duplucate Notice ID");
				return;
			}

			noticeDataList.Add(data);		
			for (int i = 0; i < data.repeatCount; i++)
			{
				data.noticeTimeList.Add(startTime + (i * data.repeatMinute * 60 * 1000)); 
			}
		}
	}

	public void RemoveNoticeItem(object obj)
	{
		long idNotice = (long)obj;
		int index = noticeDataList.FindIndex(x => x._id == idNotice);
		if (index >= 0)
			noticeDataList.RemoveAt(index);
	}

	public void RestartNoticeItem(object obj)
	{
		long idNotice = (long)obj;
		NoticeData data = noticeDataList.Find(x => x._id == idNotice);
		if (data != null)
		{
			data.noticeTimeList.Clear();
			data.stopActivation = 0;

			long startTime = data.startDate;
			if (startTime < GiantHandler.Inst.serverTime)
				startTime = GiantHandler.Inst.serverTime;

			for (int i = 0; i < data.repeatCount; i++)
			{
				data.noticeTimeList.Add(startTime + (i * data.repeatMinute * 60 * 1000)); 
			}
		}
	}

	public void StopNoticeItem(object obj)
	{
		long idNotice = (long)obj;
		NoticeData data = noticeDataList.Find(x => x._id == idNotice);
		if (data != null)
		{
			data.stopActivation = GiantHandler.Inst.serverTime;

			List<PopupUI> popupList = SystemOverlayPopupManager.Inst.GetShowPopups();
			var enumerator = popupList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TickerInfoPopup ticker = enumerator.Current as TickerInfoPopup;
				if (ticker != null && ticker.IDNotice == idNotice)
				{	
					ticker.Hide();	
				}
			}
		}
	}

	public void AddChatSystemMessage(string message, CHAT_TYPE chatType)
	{
		ChatData chatData		= new ChatData();
		chatData.chatFunc 		= CHAT_FUNC.SYSTEM_MSG;
		chatData.chatType 		= chatType;

		ChatRecvDataSysMsg data = new ChatRecvDataSysMsg();
		data.msg 				= message;

		chatData.SetReceiveData(data);
		AddMessageItem(chatData);
	}
#endregion
}


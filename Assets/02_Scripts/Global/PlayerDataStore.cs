using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerDataFieldType
{
	PlayerName = 0,
	PlayerLv = 1,
	PlayerXP = 2,
	Ether = 4,
	Gold = 6,
	Ethereum = 7,
	DungeonToken = 9,
	Cash = 11,
	ArenaCurrency = 12,
	DarkEther = 18,
	Lumber = 19,
	Cubic = 20,
	HallLv = 21,
	GuildPoint = 22,
	GuildMembership = 23,
	GuildCoin = 24,
	GuildHallLv = 25,
	GuildDarkEther = 26,
	GuildLumber = 27,
	GuildCubic = 28,

	None = 9999,
	MAX,
}

public interface IPlayerDataField
{
	void UpdateData(PlayerDataFieldType type, string data);
}

public class PlayerDataField
{
	private Dictionary<PlayerDataFieldType, List<IPlayerDataField>> PlayerDataFields = new Dictionary<PlayerDataFieldType, List<IPlayerDataField>>();

	public string PlayerName
	{
		get { return m_PlayerName; }
		set { CheckAndSetValue(ref m_PlayerName, value, PlayerDataFieldType.PlayerName); }
	}

	public bool IsNicknameInited
	{
		get { return PlayerName.CompareTo(string.Empty) != 0;}
	}

	public int PlayerLv
	{
		get { return m_PlayerLv; }
		set { CheckAndSetValue(ref m_PlayerLv, value, PlayerDataFieldType.PlayerLv); }
	}
		
	public int PlayerXP
	{
		get { return m_PlayerXP; }
		set { CheckAndSetValue(ref m_PlayerXP, value, PlayerDataFieldType.PlayerXP); }
	}

	public int Ether
	{
		get { return m_Ether; }
		set { CheckAndSetValue(ref m_Ether, value, PlayerDataFieldType.Ether); }
	}

	public int MaxEther
	{
		get { return m_MaxEther; }
		set { CheckAndSetValue(ref m_MaxEther, value, PlayerDataFieldType.Ether); }
	}

	public int Ethereum
	{
		get { return m_Ethereum; }
		set { CheckAndSetValue(ref m_Ethereum, value, PlayerDataFieldType.Ethereum); }
	}

	public int DungeonToken
	{
		get { return m_DungeonToken; }
		set { CheckAndSetValue(ref m_DungeonToken, value, PlayerDataFieldType.DungeonToken); }
	}

	public int MaxDungeonToken
	{
		get { return m_MaxDungeonToken; }
		set { CheckAndSetValue(ref m_MaxDungeonToken, value, PlayerDataFieldType.DungeonToken); }
	}

	public int Gold
	{
		get { return m_Gold; }
		set { CheckAndSetValue(ref m_Gold, value, PlayerDataFieldType.Gold); }
	}

	public int Cash
	{
		get { return m_Cash; }
		set { CheckAndSetValue(ref m_Cash, value, PlayerDataFieldType.Cash); }
	}

	public int ArenaCurrency
	{
		get { return m_ArenaCurrency; }
		set { CheckAndSetValue(ref m_ArenaCurrency, value, PlayerDataFieldType.ArenaCurrency); }
	}

	public int DarkEther
	{
		get { return m_DarkEther; }
		set { CheckAndSetValue(ref m_DarkEther, value, PlayerDataFieldType.DarkEther); }
	}

	public int MaxDarkEther
	{
		get { return m_MaxDarkEther; }
		set { CheckAndSetValue(ref m_MaxDarkEther, value, PlayerDataFieldType.DarkEther); }
	}

	public int Lumber
	{
		get { return m_Lumber; }
		set { CheckAndSetValue(ref m_Lumber, value, PlayerDataFieldType.Lumber); }
	}

	public int MaxLumber
	{
		get { return m_MaxLumber; }
		set { CheckAndSetValue(ref m_MaxLumber, value, PlayerDataFieldType.Lumber); }
	}

	public int Cubic
	{
		get { return m_Cubic; }
		set { CheckAndSetValue(ref m_Cubic, value, PlayerDataFieldType.Cubic); }
	}

	public int MaxCubic
	{
		get { return m_MaxCubic; }
		set { CheckAndSetValue(ref m_MaxCubic, value, PlayerDataFieldType.Cubic); }
	}

	public int HallLv
	{
		get { return m_HallLv; }
		set { CheckAndSetValue(ref m_HallLv, value, PlayerDataFieldType.HallLv); }
	}

	public int GuildPoint
	{
		get { return m_GuildPoint; }
		set { CheckAndSetValue(ref m_GuildPoint, value, PlayerDataFieldType.GuildPoint); }
	}

	public int GuildCoin
	{
		get { return m_GuildCoin; }
		set { CheckAndSetValue(ref m_GuildCoin, value, PlayerDataFieldType.GuildCoin); }
	}

	public int GuildDarkEther
	{
		get { return m_GuildDarkEther; }
		set { CheckAndSetValue(ref m_GuildDarkEther, value, PlayerDataFieldType.GuildDarkEther); }
	}

	public int MaxGuildDarkEther
	{
		get { return m_MaxGuildDarkEther; }
		set { CheckAndSetValue(ref m_MaxGuildDarkEther, value, PlayerDataFieldType.GuildDarkEther); }
	}

	public int GuildLumber
	{
		get { return m_GuildLumber; }
		set { CheckAndSetValue(ref m_GuildLumber, value, PlayerDataFieldType.GuildLumber); }
	}

	public int MaxGuildLumber
	{
		get { return m_MaxGuildLumber; }
		set { CheckAndSetValue(ref m_MaxGuildLumber, value, PlayerDataFieldType.GuildLumber); }
	}

	public int GuildCubic
	{
		get { return m_GuildCubic; }
		set { CheckAndSetValue(ref m_GuildCubic, value, PlayerDataFieldType.GuildCubic); }
	}

	public int MaxGuildCubic
	{
		get { return m_MaxGuildCubic; }
		set { CheckAndSetValue(ref m_MaxGuildCubic, value, PlayerDataFieldType.GuildCubic); }
	}

	private string m_PlayerName;
	private int m_PlayerLv;
	private int m_PlayerXP;
	private int m_Ether;
	private int m_MaxEther;
	private int m_Ethereum;
	private int m_DungeonToken;
	private int m_MaxDungeonToken;
	private int m_Gold;
	private int m_Cash;
	private int m_ArenaCurrency;
	private int m_DarkEther;
	private int m_MaxDarkEther;
	private int m_Lumber;
	private int m_MaxLumber;
	private int m_Cubic;
	private int m_MaxCubic;
	private int m_HallLv;
	private int m_GuildPoint;
	private int m_GuildCoin;
	private int m_GuildDarkEther;
	private int m_MaxGuildDarkEther;
	private int m_GuildLumber;
	private int m_MaxGuildLumber;
	private int m_GuildCubic;
	private int m_MaxGuildCubic;

	public void Refresh(PlayerDataFieldType type, IPlayerDataField playerDataField)
	{
		Callback(type, playerDataField);
	}

	public void ClearDataField()
	{
		var enumerator = PlayerDataFields.GetEnumerator();
		while (enumerator.MoveNext())
			enumerator.Current.Value.Clear();
	}

	public void RegistCallback(PlayerDataFieldType type, IPlayerDataField iPlayerDataField)
	{
		if (!PlayerDataFields.ContainsKey(type) || PlayerDataFields[type] == null)
			PlayerDataFields[type] = new List<IPlayerDataField>();

		PlayerDataFields[type].Add(iPlayerDataField);
	}

	public void UnregistCallback(PlayerDataFieldType type, IPlayerDataField iPlayerDataField)
	{
		if (!PlayerDataFields.ContainsKey(type))
			return;

		PlayerDataFields[type].Remove(iPlayerDataField);
	}

	private void CheckAndSetValue(ref int value, int newValue, PlayerDataFieldType dataFieldType)
	{
		if (value == newValue)
			return;
		SetValue(ref value, newValue, dataFieldType);
	}

	private void CheckAndSetValue(ref float value, float newValue, PlayerDataFieldType dataFieldType)
	{
		if (value == newValue)
			return;
		SetValue(ref value, newValue, dataFieldType);
	}

	private void CheckAndSetValue<ValueType>(ref ValueType value, ValueType newValue, PlayerDataFieldType dataFieldType) where ValueType : class
	{
		if (value == newValue)
			return;
		SetValue(ref value, newValue, dataFieldType);
	}

	private void SetValue<ValueType>(ref ValueType value, ValueType newValue, PlayerDataFieldType dataFieldType)
	{
		value = newValue;
		Callback(dataFieldType, null);
	}

	private void Callback(PlayerDataFieldType type, IPlayerDataField playerDataField)
	{
//		if (PlayerDataFields.Count <= 0 || !PlayerDataFields.ContainsKey(type) || PlayerDataFields[type].Count <= 0)
//			return;
//
//		string content = string.Empty;
//
//		switch(type)
//		{
//		case PlayerDataFieldType.PlayerName:
//			content = PlayerName;
//			break;
//
//		case PlayerDataFieldType.PlayerLv:
//			content = PlayerLv.ToString();
//			break;
//
//		case PlayerDataFieldType.PlayerXP:
//			content = PlayerXP.ToString();
//			break;
//
//		case PlayerDataFieldType.Ether:
//			content = Giant.Util.ConvertToPriceMark(Ether.ToString());
//			break;
//
//		case PlayerDataFieldType.Ethereum:
//			content = Giant.Util.ConvertToPriceMark(Ethereum.ToString());
//			break;
//
//		case PlayerDataFieldType.DungeonToken:
//			content = DungeonToken.ToString();
//			break;
//
//		case PlayerDataFieldType.Gold:
//			content = Giant.Util.ConvertToPriceMark(Gold.ToString());
//			break;
//
//		case PlayerDataFieldType.Cash:
//			content = Giant.Util.ConvertToPriceMark(Cash.ToString());
//			break;
//
//		case PlayerDataFieldType.DarkEther:
//			content = Giant.Util.ConvertToPriceMark(DarkEther.ToString());
//			break;
//
//		case PlayerDataFieldType.Lumber:
//			content = Giant.Util.ConvertToPriceMark(Lumber.ToString());
//			break;
//
//		case PlayerDataFieldType.Cubic:
//			content = Giant.Util.ConvertToPriceMark(Cubic.ToString());
//			break;
//
//		case PlayerDataFieldType.ArenaCurrency:
//			content = Giant.Util.ConvertToPriceMark(ArenaCurrency.ToString());
//			break;
//
//		case PlayerDataFieldType.GuildPoint:
//			content = Giant.Util.ConvertToPriceMark(GuildPoint.ToString());
//			break;
//
//		case PlayerDataFieldType.GuildCoin:
//			content = Giant.Util.ConvertToPriceMark(GuildCoin.ToString());
//			break;
//
//		case PlayerDataFieldType.GuildDarkEther:
//			content = Giant.Util.ConvertToPriceMark(GuildDarkEther.ToString());
//			break;
//
//		case PlayerDataFieldType.GuildLumber:
//			content = Giant.Util.ConvertToPriceMark(GuildLumber.ToString());
//			break;
//
//		case PlayerDataFieldType.GuildCubic:
//			content = Giant.Util.ConvertToPriceMark(GuildCubic.ToString());
//			break;
//		}
//
//		if (playerDataField != null)
//			playerDataField.UpdateData(type, content);
//		else
//			for (int i = 0; i < PlayerDataFields[type].Count; ++i)
//				if (PlayerDataFields[type][i] != null)
//					PlayerDataFields[type][i].UpdateData(type, content);
	}
}

public static class PlayerDataStore 
{
	public static PlayerDataField PlayerData = new PlayerDataField();

	public static void RegistCallback(PlayerDataFieldType type, IPlayerDataField playerDataField)
	{
		PlayerData.RegistCallback(type, playerDataField);
		PlayerData.Refresh(type, playerDataField);
	}

	public static void UnregistCallback(PlayerDataFieldType type, IPlayerDataField playerDataField)
	{
		PlayerData.UnregistCallback(type, playerDataField);
	}
}

using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using System.Collections;
using System.Collections.Generic;

public interface INeedCheckItem
{
	// Return : idItem
	bool OnNeedCheckItem(out int idItem, out int count);
}

public interface INeedCheckItems
{
	// Return : IdItem
	bool OnNeedCheckItems(out int[] idItems, out int[] count);
}

public class MaxResourceObserver : MonoBehaviour
{
	[System.Flags]
	public enum CheckType
	{
		None = 0,
		Gold,
		Cash,
		Ether,
		Ethereum,
		DungeonToken,
		DarkEther,
		Lumber,
		Cubic,
		Core,
	}
		
	// isAllow, idItem
	public static System.Func<bool, int, bool> onInvokeAction;

	private const int Pass = -1;

	[SerializeField] private MonoBehaviour m_Starter;
	[SerializeField] private CheckType[] m_CheckType;
	[SerializeField] private bool m_IsAllow = true;

	private Button m_Button;
	private Button.ButtonClickedEvent m_ButtonClickedEvent;
	private List<int> m_OverResourceIDItems = new List<int>();

//	private void Awake()
//	{
//		FindButton();
//	}

//	private void FindButton()
//	{
//		m_Button = GetComponent<Button>();
//
//		if (m_Button == null)
//			return;
//
//		m_ButtonClickedEvent = m_Button.onClick;
//		m_Button.onClick = new Button.ButtonClickedEvent();
//		m_Button.onClick.AddListener(OnClickButton);
//	}

	private void CallButtonClickEvent()
	{
		if (m_ButtonClickedEvent != null)
			m_ButtonClickedEvent.Invoke();
	}

	private bool CallOnInvoke(bool isAllow, int idItem)
	{
		if (onInvokeAction == null)
			return false;

		return onInvokeAction(isAllow, idItem);
	}

	private void ShowPopup(List<int> idItems)
	{
//		if (idItems.Count <= 0)
//		{
//			if (m_IsAllow)
//				CallButtonClickEvent();
//			return;
//		}
//
//		int tempIDItem = idItems[0];
//		idItems.RemoveAt(0);
//
//		if (CallOnInvoke(m_IsAllow, tempIDItem))
//			return;
//
//		string name = Datatable.Inst.GetItemName(tempIDItem);
//
//		if (onInvokeAction == null && m_IsAllow)
//		{
//			if (tempIDItem == SpecialItemID.CoreInventory)
//				SystemPopupManager.Inst.ShowCheckSystemPopup(Datatable.Inst.GetUIText(UITextEnum.COMMON_RESOURCE_MAX_CONTINUE, name), Datatable.Inst.GetUIText(UITextEnum.COMMON_NEVER_SEE_AGAIN), Datatable.Inst.GetUIText(UITextEnum.LOBBY_MAIN_MOVE_TO_CORE)).SetOnClickOKButton((isCheck) => {
//					if (isCheck)
//						AccountDataStore.instance.maxOverNotiOffIDItems.Add(tempIDItem);
//					ShowPopup(idItems);
//				}).SetOnClickETCButton(() => {
//					PanelManager.inst.PushPanel(PanelType.Core);
//				}).SetETCButtonGrayScale(BaseOperatorUnit.instance.sceneType == PlaySceneType.PreGame);
//			else
//				SystemPopupManager.Inst.ShowCheckSystemPopup(Datatable.Inst.GetUIText(UITextEnum.COMMON_RESOURCE_MAX_CONTINUE, name), Datatable.Inst.GetUIText(UITextEnum.COMMON_NEVER_SEE_AGAIN)).SetOnClickOKButton((isCheck) => {
//					if (isCheck)
//						AccountDataStore.instance.maxOverNotiOffIDItems.Add(tempIDItem);
//					ShowPopup(idItems);
//				});
//		}
//		else
//		{
//			GlobalSystemRewardMsgHandler.instance.ShowMessage(Datatable.Inst.GetUIText(UITextEnum.COMMON_RESOURCE_MAX, name));
//			ShowPopup(idItems);
//		}
	}

//	private bool CheckStarter(MonoBehaviour mono)
//	{
//		if (mono == null)
//			return false;
//
//		if (CheckNeedCheckItems(mono))
//			return true;
//
//		if (CheckNeedCheckItem(mono))
//			return true;
//
//		return false;
//	}

//	private bool CheckNeedCheckItems(MonoBehaviour mono)
//	{
//		if (!(mono is INeedCheckItems))
//			return false;
//
//		int[] idItems, count;
//		if (!(mono as INeedCheckItems).OnNeedCheckItems(out idItems, out count))
//			return false;
//		
//		if (idItems == null || idItems.Length <= 0)
//			return false;
//
//		m_OverResourceIDItems.Clear();
//		for (int i = 0; i < idItems.Length; ++i)
//		{
//			if (IsOverItem(idItems[i], count[i]))
//				m_OverResourceIDItems.Add(idItems[i]);
//		}
//		return CheckShowPopup();
//	}
//
//	private bool CheckNeedCheckItem(MonoBehaviour mono)
//	{
//		if (!(mono is INeedCheckItem))
//			return false;
//
//		int idItem, count;
//		if (!(mono as INeedCheckItem).OnNeedCheckItem(out idItem, out count))
//			return false;
//
//		m_OverResourceIDItems.Clear();
//		if (IsOverItem(idItem, count))
//			m_OverResourceIDItems.Add(idItem);
//		return CheckShowPopup();
//	}

//	private bool CheckTypes(CheckType[] checkTypes)
//	{
//		if (checkTypes == null || checkTypes.Length <= 0)
//			return false;
//
//		m_OverResourceIDItems.Clear();
//		for (int i = 0; i < checkTypes.Length; i++)
//		{
//			int idItem = CheckTypeToIDItem(checkTypes[i]);
//			if (IsOverItem(idItem, 0))
//				m_OverResourceIDItems.Add(idItem);
//		}
//		return CheckShowPopup();
//	}

//	private bool IsOverItem(int idItem, int count)
//	{
//		if (m_IsAllow && AccountDataStore.instance.maxOverNotiOffIDItems.Contains(idItem))
//			return false;
//
//		return !AccountDataStore.instance.IsEnableReceiveItemMaxValue(idItem, 0, count, true);
//	}
//
//	private bool CheckShowPopup()
//	{
//		bool isOver = m_OverResourceIDItems.Count > 0;
//		if (isOver)
//			ShowPopup(m_OverResourceIDItems);
//		return isOver;
//	}
//
//	private int CheckTypeToIDItem(CheckType checkType)
//	{
//		switch (checkType)
//		{
//		case CheckType.Gold:			return SpecialItemID.Gold;
//		case CheckType.Cash:			return SpecialItemID.Cash;
//		case CheckType.Ether:			return SpecialItemID.Ether;
//		case CheckType.Ethereum:		return SpecialItemID.Ethereum;
//		case CheckType.DungeonToken:	return SpecialItemID.DungeonToken;
//		case CheckType.DarkEther:		return SpecialItemID.DarkEther;
//		case CheckType.Lumber:			return SpecialItemID.Lumber;
//		case CheckType.Cubic:			return SpecialItemID.Cubic;
//		case CheckType.Core:			return SpecialItemID.CoreInventory;
//			
//		default:
//			return 0;
//		}
//	}
//
//	private void OnClickButton()
//	{
//		if (CheckStarter(m_Starter))
//			return;
//
//		if (CheckTypes(m_CheckType))
//			return;
//
//		CallButtonClickEvent();
//	}
}
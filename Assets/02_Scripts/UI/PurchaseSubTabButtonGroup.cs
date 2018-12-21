using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseSubTabButtonGroup : TabButtonGroup {

	protected override void Awake(){}

	void OnDisable()
	{
		m_TabItems.Clear();
	}

	public void AddTabItem(PurchaseSubTabItem item)
	{
		m_TabItems.Add(item);
		item.SetData(OnTouchTabButton, m_TabItems.Count - 1, m_NormalColorType, m_SelectColorType, m_DisableColorType);
	}

	public void Clear()
	{
		m_TabItems.Clear();
	}

	public override void UpdateSelectTabButtonUI(int index)
	{
		m_SelectIndex = index;
		for(int i = 0; i < m_TabItems.Count; i++)
		{
			if(i == index)
				SetIndexTabState(m_SelectIndex, TabButtonState.Select);
			else
				m_TabItems[i].SetState(TabButtonState.Normal, GetStateSprite(TabButtonState.Normal));
		}
	}

	public void UpdateTapNoti()
	{
		for(int i = 0; i < m_TabItems.Count; ++i)
		{
			PurchaseSubTabItem item = m_TabItems[i] as PurchaseSubTabItem;
			if (item != null)
				item.UpdateNoti();
		}
	}
}

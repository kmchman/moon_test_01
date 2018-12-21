using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using STExtensions;

public class STChatScrollRect : STScrollRect 
{
	private System.Func<int, STScrollRectItem> 		m_OnItemMakeChat;
	private List<float> 							m_ItemHeightList = new List<float>();
	private int 									m_TotalCount;
	private float									m_Spacing;
	private float									m_Bottom;

	protected override void Awake()
	{
		base.Awake();
		VerticalLayoutGroup verticalLayoutGroup = content.GetComponent<VerticalLayoutGroup>();
		m_Spacing = verticalLayoutGroup.spacing;
		m_Bottom = verticalLayoutGroup.padding.bottom;
	}

	public override void Clear()
	{
		m_ItemHeightList.Clear();
		m_OnItemMakeChat = null;
		m_TotalCount = 0;
		base.Clear();
	}
		
	protected override void SetContentOffsetAnchoredPosition()
	{
		if(content.GetHeight() <= viewport.GetHeight())
			content.SetAnchoredPositionY(-viewport.GetHeight());
		else
			SetContentAnchoredPosition(new Vector2(content.anchoredPosition.x, content.anchoredPosition.y - viewport.GetHeight()));
	}

	public void InitChatList(int count, System.Func<int, STScrollRectItem> itemMakeDelegate)
	{
		Clear();

		m_IsAddRange = true;
		m_OnItemMakeChat = itemMakeDelegate;

		float sumHeight = m_Bottom;
		m_TotalCount = count;

		for(int i = m_TotalCount - 1; i >= 0; --i)
		{
			if(m_OnItemMakeChat == null)
				break;
			m_TempItem = m_OnItemMakeChat(i);
			float height = m_TempItem.height; 
			sumHeight += height;
			m_ItemHeightList.Add(height);
			m_ScrollRectItemList.Add(m_TempItem);
			if(sumHeight > viewport.GetHeight() * 1.3f)
			{
				break;
			}
			sumHeight += m_Spacing;
		}

		for(int i = m_ScrollRectItemList.Count - 1; i >= 0; --i)
		{
			AddItem(m_ScrollRectItemList[i], true);
		}

		CalculateContentSize();
		CheckVisibleItem();

		m_IsAddRange = false;
	}
		
	protected override void SetContentAnchoredPosition(Vector2 position)
	{
		if(content.GetHeight() <= viewport.GetHeight())
			return;
		SetContentAnchoredPosition(position, false);
	}
		
	protected override void CheckVisibleItem()
	{
		int startIndex1 = -1;
		int endIndex1 = -1;
		float sumFloat = 0;

		if(!m_IsAddRange && m_TotalCount > m_ItemHeightList.Count && m_ItemHeightList.Count > 0 && content.GetHeight() + content.anchoredPosition.y < viewport.GetHeight() * 0.3f)
		{
			m_IsAddRange = true;
			m_TempItem = m_OnItemMakeChat(m_TotalCount - m_ItemHeightList.Count - 1);
			content.SetHeight(content.GetHeight() + m_TempItem.height);
			m_ItemHeightList.Add(m_TempItem.height);
			m_ScrollRectItemList.Add(m_TempItem);
			m_ShowScrollRectItemList.Add(m_TempItem);

			m_TempItem = STScrollRectItem.Packing(m_TempItem);
			m_TempItem.SetParentContent(this, content, null, true);

			m_TempItem.SetActive(true);
			m_TempItem.gameObject.SetActive(true);
			m_IsAddRange = false;
			return;
		}

		for(int i = 0; i < m_ItemHeightList.Count; ++i)
		{
			sumFloat += m_ItemHeightList[i];
			if(startIndex1 == -1)
			{
				if(- content.anchoredPosition.y - viewport.GetHeight() < sumFloat)
					startIndex1 = i;
			}

			if(- content.anchoredPosition.y < sumFloat)
				endIndex1 = i;
			
			sumFloat += m_Spacing;
		}
		startIndex1 = startIndex1 == -1 ? 0 : startIndex1;
		endIndex1 = endIndex1 == -1 ? m_ItemHeightList.Count - 1 : endIndex1;

		for(int i = 0; i < m_ScrollRectItemList.Count; ++i)
		{
			m_ScrollRectItemList[i].gameObject.SetActive(startIndex1 <= i && i <= endIndex1);
		}
	}
}

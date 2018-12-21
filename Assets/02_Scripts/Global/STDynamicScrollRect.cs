using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using STExtensions;

public class STDynamicScrollRect : STScrollRect
{
	[HideInInspector] public List<STScrollRectItem> activeItemList = new List<STScrollRectItem>();

	private List<RectTransform> 									m_LayoutElementList = new List<RectTransform>();
	private System.Func<int, STScrollRectItem, STScrollRectItem> 	m_OnItemMake;
	private System.Action<STScrollRectItem> 						m_OnMoveEnd;
	private GameObject 												m_TempGameObject;
	private int 													m_MoveTargetIndex;
	private Vector2													m_OriginalCotnentPosition;

	public void InitList(int count, System.Func<int, STScrollRectItem, STScrollRectItem> itemMakeDelegate)
	{
		m_OriginalCotnentPosition = content.anchoredPosition;
		
		Clear();

		m_IsAddRange = true;
		m_OnItemMake = itemMakeDelegate;

		for(int i = 0; i < count; ++i)
		{
			if(m_LayoutElementList.Count > i)
			{
				m_LayoutElementList[i].gameObject.SetActive(true);
				continue;
			}
			else
			{
				m_TempGameObject = new GameObject("Item", typeof(RectTransform));
				m_TempGameObject.transform.SetParent(content);
				m_TempGameObject.transform.localScale = Vector3.one;
				m_TempGameObject.transform.localPosition = new Vector3(m_TempGameObject.transform.localPosition.x, m_TempGameObject.transform.localPosition.y, 0);
				m_ScrollRectItemList.Add(null);
				m_ShowScrollRectItemList.Add(null);
				m_LayoutElementList.Add(m_TempGameObject.transform as RectTransform);
			}
		}
			
		for(int i = m_LayoutElementList.Count - 1; count <= i; --i)
		{
			RectTransform rectTransform;
			rectTransform = m_LayoutElementList[i];
			rectTransform.gameObject.SetActive(false);
			m_LayoutElementList.RemoveAt(i);
			Destroy(rectTransform.gameObject);
		}

		m_IsAddRange = false;

		content.anchoredPosition = m_OriginalCotnentPosition;

		CalculateContentSize();
		CheckVisibleItem();
	}

	public override void Clear()
	{
		for(int i = 0; i < m_ShowScrollRectItemList.Count; ++i)
		{
			m_TempItem = m_ShowScrollRectItemList[i];
			if(m_TempItem == null)
				continue;
			m_TempItem.idItemInitData = 0;
			m_TempItem.gameObject.SetActive(false);
			PushItem(m_TempItem);
			m_ScrollRectItemList[i] = m_ShowScrollRectItemList[i] = null;
		}
		content.anchoredPosition = Vector2.zero;
	}
		
	public override void ScrollToFirst()
	{
		if(m_LayoutElementList.Count <= 0)
			return;

		SetContentOffset(0);
	}

	public override void ScrollToLast()
	{
		if(m_LayoutElementList.Count <= 0)
			return;

		SetContentOffset(m_LayoutElementList.Count - 1);
	}

	public void SetContentOffset(int index, System.Action<STScrollRectItem> onMoveEnd)
	{
		m_OnMoveEnd = onMoveEnd;
		m_MoveTargetIndex = index;
		SetContentOffset(index, false, false);
	}

	public override void SetContentOffset(int index, bool isFixedHorizontal, bool isFixedVertical, float marginRatio = 0f)
	{
		if(m_LayoutElementList.Count <= index || index < 0)
			return;

		SetContentOffset(m_LayoutElementList[index], marginRatio);
	}

	public override void CalculateContentSize()
	{
		Rect viewPortRect = viewport.rect;

		Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
		Vector2 max = new Vector2(float.MinValue, float.MinValue);

		CalculateLayout();

		for(int i = 0; i < m_LayoutElementList.Count; ++i)
		{
			min = Vector2.Min(min, m_LayoutElementList[i].offsetMin);
			max = Vector2.Max(max, m_LayoutElementList[i].offsetMax);
		}

		Vector2 calculateSize = new Vector2(max.x - min.x + layoutGroup.padding.left + layoutGroup.padding.right,
			max.y - min.y + layoutGroup.padding.top + layoutGroup.padding.bottom);

		content.SetSize(Mathf.Max(viewPortRect.width, calculateSize.x), Mathf.Max(viewPortRect.height, calculateSize.y));

		CheckViewPortRectMovableScroll();

		if(!vertical)
			content.SetAnchoredPositionY(0);
		if(!horizontal)
			content.SetAnchoredPositionX(0);
	}

	protected override void AddItem(STScrollRectItem item, bool isChatInit = false)
	{
		base.AddItem(item, isChatInit);
		m_LayoutElementList.Add(item.layoutElementRectTransformRef);
	}
		
	protected override void CheckVisibleItem()
	{
		Rect contentWorldRect = viewport.GetWorldRect();
		bool tempBool = false;
		int startIndex = -1;
		int endIndex = -1;
		for (int i = 0; i < m_LayoutElementList.Count; ++i)
		{
			tempBool = CheckIsVisible(contentWorldRect, m_LayoutElementList[i].GetWorldRect());
			if(startIndex == -1 && tempBool)
				startIndex = i;
			if(startIndex != -1 && !tempBool)
			{
				endIndex = i;
				break;
			}
		}
		if(endIndex == -1)
		{
			endIndex = m_ShowScrollRectItemList.Count - 1;
		}
			
		SetItems(startIndex, endIndex);
	}

	private bool CheckIsVisible(Rect contentWorldRect, Rect worldRect)
	{
		Vector2 marignRectSize = new Vector2(worldRect.width * 0.1f, worldRect.height * 0.1f);

		bool isContains = !(worldRect.yMax + marignRectSize.y < contentWorldRect.yMin ||
			worldRect.xMax + marignRectSize.x < contentWorldRect.xMin ||
			contentWorldRect.yMax < worldRect.yMin - marignRectSize.y ||
			contentWorldRect.xMax < worldRect.xMin - marignRectSize.x);

		return isContains;
	}
		
	private int m_OldStartIndex = 0;
	private void SetItems(int startIndex, int endIndex)
	{
		m_IsAddRange = true;
		activeItemList.Clear();
		bool isUp = startIndex < m_OldStartIndex;
		m_OldStartIndex = startIndex;
		int count = m_LayoutElementList.Count;
		for(int i = 0; i < count; ++i)
		{
			int index = isUp ? count - 1 - i : i;
			STScrollRectItem tempItem = m_ScrollRectItemList[index];
			if(startIndex <= index && index <= endIndex)
			{
				if(tempItem == null && m_OnItemMake != null)
				{
					tempItem = m_OnItemMake(index, tempItem);
					tempItem.gameObject.SetActive(true);
					m_ScrollRectItemList[index] = tempItem;
					m_ShowScrollRectItemList[index] = tempItem;
					tempItem.transform.SetParent(m_LayoutElementList[index]);
					tempItem.transform.localPosition = Vector3.zero;
					tempItem.transform.localScale = Vector3.one;
				}
				else if(tempItem != null && m_OnItemMake != null)
				{
					m_OnItemMake(index, tempItem);
				}

				activeItemList.Add(tempItem);
				tempItem.gameObject.SetActive(true);
			}
			else
			{
				if(tempItem != null)
				{
					tempItem.gameObject.SetActive(false);
					PushItem(tempItem);
					m_ShowScrollRectItemList[index] = null;
					m_ScrollRectItemList[index] = null;
				}
			}
		}
			
		if(m_OnMoveEnd != null)
		{
			if(m_ScrollRectItemList.Count <= m_MoveTargetIndex)
				return;
			
			m_OnMoveEnd(m_ScrollRectItemList[m_MoveTargetIndex]);
			m_OnMoveEnd = null;
		}
		m_IsAddRange = false;
	}
}
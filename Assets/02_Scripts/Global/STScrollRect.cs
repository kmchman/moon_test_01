using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using STExtensions;

using DG;
using DG.Tweening;
using STDOTweenExtensions;

public class STScrollRect : STScrollRectBase
{
	private const float HIDE_ANIMATION_TIME = 0.15f;
	private const string COVER_IMAGE_NAME = "CoverImage";
	private const float ACCEPTABLE_FRACTIONS = 1f;

	public enum ItemAnimationType
	{
		None,
		VerticalCrush,
		HorizontalCrush,
	}

	public bool isPlaying { get { 
		return rectTransform.IsDOAnimation(); 
	} }

	public bool isEnableScroll { get {
		return vertical || horizontal;
	} }

	public int count { get { 
		return m_ShowScrollRectItemList.Count; 
	} }

	public STScrollRectItem this[int index] { get {
		if (!CheckIndex(index))
			return null;

		return m_ShowScrollRectItemList[index];
	} }

	public RectTransform rectTransform { get {
		if (m_RectTransformRef == null)
			m_RectTransformRef = transform as RectTransform;
		return m_RectTransformRef;
	} }
	
	public LayoutGroup layoutGroup { get {
		if (m_LayoutGroup == null)
			m_LayoutGroup = transform.GetComponentInChildren<LayoutGroup>(true);
		return m_LayoutGroup;
	} }

	private RectTransform m_RectTransformRef;
	private LayoutGroup m_LayoutGroup;

	protected List<STScrollRectItem> m_ScrollRectItemList = new List<STScrollRectItem>();
	protected List<STScrollRectItem> m_ShowScrollRectItemList = new List<STScrollRectItem>();
	protected Dictionary<string, Stack<STScrollRectItem>> m_ScrollRectPools = new Dictionary<string, Stack<STScrollRectItem>>();
	protected STScrollRectItem m_TempItem;

	private object m_HideAnimationTarget = new object();

	protected bool m_IsAddRange = false;
	private GridLayoutGroup m_GridLayoutGroup;

	public STScrollRectItem GetItemByIndex(int index)
	{
		if(!CheckIndex(index))
			return null;

		return m_ShowScrollRectItemList[index];
	}

	public void RemoveByIndex(int index)
	{
		if(!CheckIndex(index))
			return;

		STScrollRectItem item = m_ShowScrollRectItemList[index];

		if(item == null)
			Remove(item);
		else
			RemoveItemByIndex(index);
	}

	private void RemoveItemByIndex(int index)
	{
		m_ShowScrollRectItemList[index].layoutElementRectTransformRef.gameObject.SetActive(false);
		m_ShowScrollRectItemList.RemoveAt(index);
		m_ScrollRectItemList.RemoveAt(index);

		// Check Content View
		CalculateContentSize();
		SetContentAnchoredPosition(ClampScrollAblePosition(content.anchoredPosition), true);
	}

	public void SetOneContentHeight(float height)
	{
		content.SetHeight(height);
		vertical = viewport.GetHeight() < height;
		content.SetAnchoredPositionY(0);
	}
		
	public ItemType GetReuseItem<ItemType>(ItemType prefab, System.Action<ItemType> initDelegate) where ItemType : STScrollRectItem
	{
		if(!CheckActive())
			return null;

		m_TempItem = PopItem(prefab.reuseIdentifierName);

		if(m_TempItem == null)
		{
			m_TempItem = GameObject.Instantiate<ItemType>(prefab);
			if(initDelegate != null)
				initDelegate((ItemType)m_TempItem);
		}

		if(!m_IsAddRange)
			Add(m_TempItem);

		return (ItemType)m_TempItem;
	}

	public int GetItemIndex(STScrollRectItem item)
	{
		return m_ShowScrollRectItemList.FindIndex((STScrollRectItem x)=>{ return x == item; });
	}

	public void Add(STScrollRectItem item)
	{
		if(!CheckActive())
			return;

		StopHideItemAnimation();

		AddItem(item);

		// Check Content View
		CalculateContentSize();
		CheckVisibleItem();
	}

	public void AddRange(int count, System.Func<int, STScrollRectItem> makeItemFunc)
	{
		if(count <= 0 || makeItemFunc == null)
			return;

		if(!CheckActive())
			return;

		StopHideItemAnimation();

		m_IsAddRange = true;

		for(int i = 0; i < count; ++i)
		{
			STScrollRectItem item = makeItemFunc(i);
			if(item == null)
				continue;

			AddItem(item);
		}

		m_IsAddRange = false;

		// Check Content View
		CalculateContentSize();
		CheckVisibleItem();
	}

	public void Remove(STScrollRectItem item)
	{
		StopHideItemAnimation();

		RemoveItem(item);

		// Check Content View
		CalculateContentSize();
		SetContentAnchoredPosition(ClampScrollAblePosition(content.anchoredPosition), true);
	}

	public virtual void Clear()
	{
		for(int i = 0; i < m_ScrollRectItemList.Count; ++i)
		{
			m_TempItem = m_ScrollRectItemList[i];
			m_TempItem.layoutElementRectTransformRef.gameObject.SetActive(false);
			PushItem(m_TempItem);
		}
		m_ShowScrollRectItemList.Clear();
		m_ScrollRectItemList.Clear();

		CalculateContentSize();
		SetContentAnchoredPosition(ClampScrollAblePosition(content.anchoredPosition), true);
	}

	public void Sort(Comparison<STScrollRectItem> comparer)
	{
		m_ScrollRectItemList.Sort(comparer);
		m_ShowScrollRectItemList.Sort(comparer);

		for(int i = 0; i < m_ScrollRectItemList.Count; ++i)
		{
			m_ScrollRectItemList[i].layoutElementRectTransformRef.SetAsLastSibling();
		}

		CalculateLayout();
		CheckVisibleItem();
	}

	public void Filter(System.Func<STScrollRectItem, bool> filter)
	{
		m_ShowScrollRectItemList.Clear();

		for (int i = 0; i < m_ScrollRectItemList.Count; ++i)
		{
			if (filter == null || filter(m_ScrollRectItemList[i]))
			{
				m_ScrollRectItemList[i].layoutElementRectTransformRef.gameObject.SetActive(true);
				m_ShowScrollRectItemList.Add(m_ScrollRectItemList[i]);
			}
			else
			{
				m_ScrollRectItemList[i].layoutElementRectTransformRef.gameObject.SetActive(false);
			}
		}

		CalculateContentSize();
		SetContentAnchoredPosition(ClampScrollAblePosition(content.anchoredPosition));
	}

	public virtual void ScrollToFirst()
	{
		if(m_ShowScrollRectItemList.Count <= 0)
			return;

		SetContentOffset(0);
	}

	public virtual void ScrollToLast()
	{
		if(m_ShowScrollRectItemList.Count <= 0)
			return;

		SetContentOffset(m_ShowScrollRectItemList.Count - 1);
	}
		
	public void SetContentOffset(int index) { SetContentOffset(index, false, false); }
	public virtual void SetContentOffset(int index, bool isFixedHorizontal, bool isFixedVertical, float marginRatio = 0f)
	{
		if(m_ShowScrollRectItemList.Count <= index)
			return;

		SetContentOffset(m_ShowScrollRectItemList[index].rectTransformRef, marginRatio);
	}

	public void SetContentOffset(RectTransform item, float marginRatio = 0f) { SetContentOffset(item, false, false, marginRatio); }
	public virtual void SetContentOffset(RectTransform item, bool isFixedHorizontal, bool isFixedVertical, float marginRatio)
	{
		StopMovement();

		Rect itemWorldRect = item.GetWorldRect();
		Vector2 leftTop = new Vector2(itemWorldRect.xMin, itemWorldRect.yMax) + new Vector2(0, item.GetHeight() * marginRatio);

		Vector2 contentPosition = content.localPosition;
		Vector2 newContentPosition = -content.worldToLocalMatrix.MultiplyPoint3x4(leftTop);

		newContentPosition += new Vector2(layoutGroup.padding.left, -layoutGroup.padding.top);
		newContentPosition = new Vector2((isFixedHorizontal ? contentPosition.x : newContentPosition.x),
			(isFixedVertical ? contentPosition.y : newContentPosition.y));

		newContentPosition = ClampScrollAblePosition(newContentPosition);

		content.anchoredPosition = newContentPosition;
		SetContentOffsetAnchoredPosition();
	}

	protected virtual void SetContentOffsetAnchoredPosition()
	{
		SetContentAnchoredPosition(content.anchoredPosition);
	}

	public virtual void CalculateContentSize()
	{
		Rect viewPortRect = viewport.rect;

		Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
		Vector2 max = new Vector2(float.MinValue, float.MinValue);

		CalculateLayout();

		for(int i = 0; i < m_ShowScrollRectItemList.Count; ++i)
		{
			min = Vector2.Min(min, m_ShowScrollRectItemList[i].offsetMin);
			max = Vector2.Max(max, m_ShowScrollRectItemList[i].offsetMax);
		}

		Vector2 calculateSize = new Vector2(max.x - min.x + layoutGroup.padding.left + layoutGroup.padding.right,
			max.y - min.y + layoutGroup.padding.top + layoutGroup.padding.bottom);

		content.SetSize(Mathf.Max(viewPortRect.width, calculateSize.x), Mathf.Max(viewPortRect.height, calculateSize.y));
		CheckViewPortRectMovableScroll();
	}

	protected override void SetContentAnchoredPosition(Vector2 position)
	{
		SetContentAnchoredPosition(position, false);
	}

	public void SetContentPosition(Vector2 position)
	{
		SetContentAnchoredPosition(position, false);
	}

	protected void SetContentAnchoredPosition(Vector2 position, bool isForce)
	{
		if(isForce)
		{
			bool tmpVertical = vertical;
			bool tmpHorizontal = horizontal;

			vertical = true;
			horizontal = true;

			base.SetContentAnchoredPosition(position);

			vertical = tmpVertical;
			horizontal = tmpHorizontal;
		}
		else
		{
			base.SetContentAnchoredPosition(position);
		}
		CalculateLayout();
		CheckVisibleItem();
	}

	private bool CheckActive()
	{
		if(gameObject.activeInHierarchy)
			return true;

		Debug.LogErrorFormat("ScrollRec[{0}] : Inactive", name);
		return false;
	}

	private bool CheckAnimationPlaying()
	{
		if(!rectTransform.IsDOAnimation())
			return true;

		Debug.LogError("ScrollRect Playing Animation");
		return false;
	}

	private bool CheckIndex(int index)
	{
		bool isResult = !(m_ShowScrollRectItemList.Count <= 0 || m_ShowScrollRectItemList.Count <= index);

		if(!isResult)
			Debug.LogError(Util.LogFormat("Out of bounds", index));

		return isResult;
	}

	protected virtual void AddItem(STScrollRectItem item, bool isAddScrollRectItemList = false)
	{
		item = STScrollRectItem.Packing(item);
		item.SetParentContent(this, content, OnChangeItemRectTransform);

		if(!isAddScrollRectItemList)
			m_ScrollRectItemList.Add(item);
		m_ShowScrollRectItemList.Add(item);
		item.SetActive(true);
	}

	protected void RemoveItem(STScrollRectItem item)
	{
		m_ScrollRectItemList.Remove(item);
		m_ShowScrollRectItemList.Remove(item);
		item.SetActive(false);
		PushItem(item);
	}

	protected void PushItem(STScrollRectItem item)
	{
		if(item == null || string.IsNullOrEmpty(item.reuseIdentifierName))
			return;

		if(!m_ScrollRectPools.ContainsKey(item.reuseIdentifierName))
			m_ScrollRectPools[item.reuseIdentifierName] = new Stack<STScrollRectItem>();

		m_ScrollRectPools[item.reuseIdentifierName].Push(item);
	}

	protected STScrollRectItem PopItem(string reuseIdentifier)
	{
		if(!string.IsNullOrEmpty(reuseIdentifier) && m_ScrollRectPools.ContainsKey(reuseIdentifier) && m_ScrollRectPools[reuseIdentifier].Count > 0)
			return m_ScrollRectPools[reuseIdentifier].Pop();

		return null;
	}

	protected Vector2 ClampScrollAblePosition(Vector2 position)
	{
		float scrollAbleContentWidth = content.GetWidth() - viewport.GetWidth();
		float scrollAbleContentHeight = content.GetHeight() - viewport.GetHeight();

		return new Vector2(Mathf.Clamp(position.x, -scrollAbleContentWidth, 0),
			Mathf.Clamp(position.y, 0, scrollAbleContentHeight));
	}

	protected void CalculateLayout()
	{
		if(layoutGroup == null)
			return;
		
		layoutGroup.CalculateLayoutInputHorizontal();
		layoutGroup.CalculateLayoutInputVertical();

		layoutGroup.SetLayoutHorizontal();
		layoutGroup.SetLayoutVertical();
	}

	protected virtual void CheckVisibleItem()
	{
		Rect contentWorldRect = viewport.GetWorldRect();
		for(int i = 0; i < m_ShowScrollRectItemList.Count; ++i)
		{
			m_ShowScrollRectItemList[i].OnCheckVisible(contentWorldRect);
		}
	}

	protected override void SetNormalizedPosition(float value, int axis)
	{
		base.SetNormalizedPosition (value, axis);
		CheckVisibleItem();
	}
	
	protected void CheckViewPortRectMovableScroll()
	{
		horizontal = viewport.rect.width < content.rect.width - ACCEPTABLE_FRACTIONS;
		vertical = viewport.rect.height < content.rect.height - ACCEPTABLE_FRACTIONS;
	}

	private void StopHideItemAnimation()
	{
		DOTween.Kill(m_HideAnimationTarget, true);
	}

	private void OnChangeItemRectTransform(STScrollRectItem item)
	{
		CalculateContentSize();
	}
}
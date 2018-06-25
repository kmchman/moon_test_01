using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System;
using System.Collections;
using System.Collections.Generic;

using STExtensions;

using DG;
using DG.Tweening;
using STDOTweenExtensions;

public class STScrollRectItem : UIBehaviour
{
	public Vector2 offsetMax { get { return m_layoutElementRectTransformRef.offsetMax; } }
	public Vector2 offsetMin { get { return m_layoutElementRectTransformRef.offsetMin; } }

	public RectTransform rectTransformRef
	{
		get
		{
			if (m_rectTransformRef == null)
				m_rectTransformRef = transform as RectTransform;
			return m_rectTransformRef;
		}
	}

	public LayoutElement layoutElement { get { return m_layoutElement; } }
	public RectTransform layoutElementRectTransformRef { get { return m_layoutElementRectTransformRef; } }

	public float width
	{
		get
		{
			if (m_layoutElement != null)
				return m_layoutElement.minWidth;
			return rectTransformRef.GetLocalScaledWidth();
		}
		set
		{
			rectTransformRef.sizeDelta = new Vector2(value / rectTransformRef.localScale.x, rectTransformRef.sizeDelta.y);
			UpdateWidth();
		}
	}

	public float height
	{
		get
		{
			if (m_layoutElement != null)
				return m_layoutElement.minHeight;
			return rectTransformRef.GetLocalScaledHeight();
		}
		set
		{
			rectTransformRef.sizeDelta = new Vector2(rectTransformRef.sizeDelta.x, value / rectTransformRef.localScale.y);
			UpdateHeight();
		}
	}

	public string reuseIdentifierName;
	[HideInInspector] public int idItemInitData;

	protected STScrollRect m_parentScrollRect;

	protected RectTransform m_rectTransformRef;

	protected LayoutElement m_layoutElement;
	protected RectTransform m_layoutElementRectTransformRef;

	protected Action<STScrollRectItem> m_onChangeRectTransformDelegate;

	private HashSet<IEnumerator> m_RunningCoroutineSet = new HashSet<IEnumerator>();

	public static STScrollRectItem Packing(STScrollRectItem item)
	{
		if (item.m_layoutElement != null)
			return item;

		return MakeItemObject(item);
	}

	private static STScrollRectItem MakeItemObject(STScrollRectItem item)
	{
		GameObject gameObject = new GameObject("Item", typeof(RectTransform));

		item.rectTransformRef.SetParent(gameObject.transform, false);

		item.m_layoutElement = gameObject.AddComponent<LayoutElement>();
		item.UpdateWidth();
		item.UpdateHeight();

		item.m_layoutElementRectTransformRef = (gameObject.transform as RectTransform);

		return item;
	}

	public void UpdateSize()
	{
		UpdateWidth();
		UpdateHeight();
	}

	public void SetParentContent(STScrollRect parentScrollRect, RectTransform parent, Action<STScrollRectItem> onChangeRectTransformDelegate, bool isToFirst = false)
	{
		if (m_layoutElement == null)
		{
			Debug.LogError("Did not Packing");
			return;
		}

		m_parentScrollRect = parentScrollRect;

		m_layoutElementRectTransformRef.SetParent(parent, false);
		if(isToFirst)
			m_layoutElementRectTransformRef.SetAsFirstSibling();
		else
			m_layoutElementRectTransformRef.SetAsLastSibling();
		m_onChangeRectTransformDelegate = onChangeRectTransformDelegate;
	}

	public void SetActive(bool isActive)
	{
		if(m_layoutElement != null)
			m_layoutElement.gameObject.SetActive(isActive);
	}

	public void OnCheckVisible(Rect contentWorldRect)
	{
		Rect worldRect = rectTransformRef.GetWorldRect();
		Vector2 marignRectSize = new Vector2(worldRect.width * 0.1f, worldRect.height * 0.1f);

		bool isContains = !(worldRect.yMax + marignRectSize.y < contentWorldRect.yMin ||
		                    worldRect.xMax + marignRectSize.x < contentWorldRect.xMin ||
		                    contentWorldRect.yMax < worldRect.yMin - marignRectSize.y ||
		                    contentWorldRect.xMax < worldRect.xMin - marignRectSize.x);

		gameObject.SetActive(isContains);
	}

	protected IEnumerator StartItemCoroutine(IEnumerator routine)
	{
		if (!CheckParentScrollRect())
			return null;
			
		m_parentScrollRect.StartCoroutine(MakeCoroutine(routine));
		return routine;
	}

	protected void StopItemCoroutine(IEnumerator routine)
	{
		if (!CheckParentScrollRect())
			return;

		m_parentScrollRect.StopCoroutine(routine);

		m_RunningCoroutineSet.Remove(routine);
	}

	protected void StopAllItemCoroutine()
	{
		if (!CheckParentScrollRect())
			return;

		if (m_RunningCoroutineSet.Count <= 0)
			return;

		var enumerator = m_RunningCoroutineSet.GetEnumerator();
		while (enumerator.MoveNext())
			m_parentScrollRect.StopCoroutine(enumerator.Current);

		m_RunningCoroutineSet.Clear();
	}

	private void UpdateWidth()
	{
		if (m_layoutElement == null)
			return;
		
		m_layoutElement.minWidth = rectTransformRef.GetLocalScaledWidth();

		if (m_onChangeRectTransformDelegate != null)
			m_onChangeRectTransformDelegate(this);
	}

	private void UpdateHeight()
	{
		if (m_layoutElement == null)
			return;

		m_layoutElement.minHeight = rectTransformRef.GetLocalScaledHeight();

		if (m_onChangeRectTransformDelegate != null)
			m_onChangeRectTransformDelegate(this);
	}

	private bool CheckParentScrollRect()
	{
		if (m_parentScrollRect != null)
			return true;
		
		Debug.LogError("Not Found Parent Scroll Rect");
		return false;
	}

	private IEnumerator MakeCoroutine(IEnumerator routine)
	{
		m_RunningCoroutineSet.Add(routine);
		yield return m_parentScrollRect.StartCoroutine(routine);
		m_RunningCoroutineSet.Remove(routine);
	}
}
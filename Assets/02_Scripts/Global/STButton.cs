using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

public class STButton : Button
{
	[SerializeField] private UIAnimationEffectorType m_NormalStateAnimatonEffectorType;
	[SerializeField] private UIAnimationEffectorType m_PressedStateAnimatonEffectorType;
	[SerializeField] private RectTransform m_AnimationRectTransform;
	[SerializeField] private SoundEffect m_OnClickSEType = SoundEffect.ButtonB;

	[SerializeField] private List<int> m_Markers = new List<int>();

	public RectTransform rectTransformRef { get { return m_RectTransformRef; } }

	public System.Func<bool> isCheckRunEnableFunc;

	private RectTransform m_RectTransformRef;

	private HashSet<int> m_MarkerSet = new HashSet<int>();
	private bool m_IsPressedState = false;
	private bool m_IsChainTouch = false;

	protected override void Awake()
	{
		m_RectTransformRef = transform as RectTransform;

		if(m_AnimationRectTransform == null)
			m_AnimationRectTransform = m_RectTransformRef;

		base.Awake();

//#if UNITY_EDITOR
//		STGraphicManager.MakeSTGraphicManager();
//#endif

		Navigation tempNavigation = navigation;
		tempNavigation.mode = Navigation.Mode.None;
		navigation = tempNavigation;

//		InitMarker();
//		AddButton();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

//		RemoveButton();
	}

	protected override void DoStateTransition(SelectionState state, bool instant)
	{
		base.DoStateTransition(state, instant);

		switch (state)
		{
		case SelectionState.Normal:
		case SelectionState.Highlighted:
			if (m_IsPressedState && m_NormalStateAnimatonEffectorType != UIAnimationEffectorType.None)
			{
				m_AnimationRectTransform.DOKill(true);
				Tween tween = UIAnimationEffector.Invoke(m_NormalStateAnimatonEffectorType, m_AnimationRectTransform);
				if (instant)
					tween.Kill(true);

				m_IsPressedState = false;
			}
			break;

		case SelectionState.Pressed:
			if (m_PressedStateAnimatonEffectorType != UIAnimationEffectorType.None)
			{
				m_AnimationRectTransform.DOKill(true);
				Tween tween = UIAnimationEffector.Invoke(m_PressedStateAnimatonEffectorType, m_AnimationRectTransform);
				if (instant)
					tween.Kill(true);

				m_IsPressedState = true;
			}
			break;
		}
	}

//	public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
//	{
//		if (eventData.used || !STGraphicManager.inst.OnWillClickButton(this))
//			return;
//		
//		if(isCheckRunEnableFunc != null && !isCheckRunEnableFunc())
//			return;
//		
//		ReorderClickEvent();
//		SoundManager.instance.PlayEffect(m_OnClickSEType);
//		if (!m_IsChainTouch && ChainTouchManager.inst.isChainTouch)
//		{
//			ChainTouchManager.inst.SetOnOtherTouch(()=>{
//					base.OnPointerClick(eventData);
//				});
//			return;
//		}
//		base.OnPointerClick(eventData);
//	}
//
//	public override void OnSubmit(UnityEngine.EventSystems.BaseEventData eventData)
//	{
//		if (!STGraphicManager.inst.OnWillClickButton(this))
//			return;
//
//		if(isCheckRunEnableFunc != null && !isCheckRunEnableFunc())
//			return;
//
//		ReorderClickEvent();
//		base.OnSubmit(eventData);
//	}

	public bool IsEqualsMarker(int marker)
	{
		for (int i = 0; i < m_Markers.Count; ++i)
			if (m_Markers[i] == marker)
				return true;

		return false;
	}

//	public void SetMarker(int marker, params int[] addMarkers)
//	{
//		m_Markers.Clear();
//
//		m_Markers.Add(marker);
//
//		if (addMarkers != null && addMarkers.Length > 0)
//			for (int i = 0; i < addMarkers.Length; ++i)
//				m_Markers.Add(addMarkers[i]);
//
//		RemoveButton();
//
//		InitMarker();
//		AddButton();
//	}
//
//	public void AddMarker(int marker)
//	{
//		if (m_MarkerSet.Contains(marker))
//			return;
//
//		RemoveButton();
//
//		m_Markers.Add(marker);
//		m_MarkerSet.Add(marker);
//		AddButton();
//	}
//
//	public void RemoveMarker(int marker)
//	{
//		if (!m_MarkerSet.Contains(marker))
//			return;
//
//		RemoveButton();
//
//		m_Markers.Remove(marker);
//		m_MarkerSet.Remove(marker);
//		AddButton();
//	}
//
//	public void RemoveAllMarker()
//	{
//		m_Markers.Clear();
//		RemoveButton();
//		InitMarker();
//		AddButton();
//	}
//
//	public void SetIsChainTouch(bool isChainTouch)
//	{
//		m_IsChainTouch = isChainTouch;
//	}
//
//	private void ReorderClickEvent()
//	{
//		onClick.RemoveListener(CallOnClickButton);
//		onClick.AddListener(CallOnClickButton);
//	}

//	private void CallOnClickButton()
//	{
//		STGraphicManager.inst.OnClickButton(this);
//	}
//
//	private void InitMarker()
//	{
//		m_MarkerSet.Clear();
//
//		for (int i = 0; i < m_Markers.Count; ++i)
//			m_MarkerSet.Add(m_Markers[i]);
//	}
//
//	private void AddButton()
//	{
//		if (STGraphicManager.inst == null)
//			return;
//
//		STGraphicManager.inst.AddButton(this, m_MarkerSet);
//	}
//
//	private void RemoveButton()
//	{
//		if (STGraphicManager.inst == null)
//			return;
//
//		STGraphicManager.inst.RemoveButton(this, m_MarkerSet);
//	}
}
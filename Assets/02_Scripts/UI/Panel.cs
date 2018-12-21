using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Panel : MonoBehaviour
{
	public enum Direction
	{
		None,
		Forward,
		Backward
	}

	[SerializeField] private bool m_IsDestroyAtHide = false;

	public bool isDestroyAtHide { get { return m_IsDestroyAtHide; } }
	public Direction direction { get { return m_Direction; } }

	protected PanelManager m_PanelManager;
	protected Direction m_Direction;

	protected HudController m_HudController;

	protected RectTransform m_RectTransformRef;

	private Action<Panel> m_CompleteShowAnimationDelegate;
	private Action<Panel> m_CompleteHideAnimationDelegate;

	protected virtual void Awake()
	{
		m_RectTransformRef = transform as RectTransform;
		m_HudController = GetComponent<HudController>();
	}

	public void SetPanelManager(PanelManager panelManager)
	{
		m_PanelManager = panelManager;
	}

	public void SetDirection(Direction direction)
	{
		m_Direction = direction;
	}

	public void ShowPanelByManager(Action<Panel> completeShowAnimation, params object[] values)
	{
		m_CompleteShowAnimationDelegate = completeShowAnimation;

		if (m_HudController != null)
			m_HudController.UpdateHud();

		Show(values);
	}

	public void HidePanelByManager(Action<Panel> completeHideAnimation, params object[] values)
	{
		m_CompleteHideAnimationDelegate = completeHideAnimation;

		Hide(values);
	}

	public virtual bool OnCallByBackButtonManager()
	{
		return false;
	}

	protected abstract void Show(params object[] values);
	protected abstract void Hide(params object[] values);

	// 각 패널은 Show 애니메이션 후 이 함수를 호출한다
	protected void CompleteShowAnimation()
	{
		if (m_CompleteShowAnimationDelegate != null)
		{
			m_CompleteShowAnimationDelegate(this);
			m_CompleteShowAnimationDelegate = null;
		}
	}

	// 각 패널은 Hide 애니메이션 후 이 함수를 호출한다
	protected void CompleteHideAnimation()
	{
		if (m_CompleteHideAnimationDelegate != null)
		{
			m_CompleteHideAnimationDelegate(this);
			m_CompleteHideAnimationDelegate = null;
		}
	}

	protected void SavePanelData(params object[] values)
	{
		m_PanelManager.SavePanelData(values);
	}
}
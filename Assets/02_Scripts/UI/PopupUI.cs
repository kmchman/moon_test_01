using UnityEngine;

using System;
using System.Collections;

using STExtensions;

using DG.Tweening;
using STDOTweenExtensions;

public class PopupUI : MonoBehaviour
{
	[SerializeField] private bool m_IsUseDefaultBackground = true;
	[SerializeField] private bool m_IsAutoHideClickDefaultBackground = false;
	[SerializeField] private bool m_IsDestroyAtHide = false;
	[SerializeField] private bool m_IsPauseEventAction = false;
	[SerializeField] private bool m_IsIgnoreHidePopupByBackButtonManager = false;

	[SerializeField] protected STAnimationItem m_ShowAnimationItem = null;

	public bool isUseDefaultBackground { get { return m_IsUseDefaultBackground; } }
	public bool isAutoHideClickDefaultBackground { get { return m_IsAutoHideClickDefaultBackground; } }
	public bool isDestroyAtHide { get { return m_IsDestroyAtHide; } }
	public bool isPauseEventAction { get { return m_IsPauseEventAction; } }
	public bool isIgnoreHidePopupByBackButtonManager { get { return m_IsIgnoreHidePopupByBackButtonManager; } }

	public virtual bool isShow { get { return gameObject.activeSelf; } }
	public RectTransform rectTransformRef { get { return m_RectTransformRef; } }

	public string alias { get { return m_Alias; } }

	protected GameObject m_GameObjectRef;
	protected RectTransform m_RectTransformRef;

	protected string m_Alias;

	protected bool m_IsInit;

	protected UIAnimationSetter m_ShowAnimationSetter;
	protected UIAnimationSetter m_HideAnimationSetter;

	protected Action m_ShowPopupAction;
	protected Action m_HidePopupAction;

	private FlagSynchronizer m_FlagSynchronizer = new FlagSynchronizer();

	private Coroutine WaitForHideCoroutine = null;

	private Action<PopupUI> m_DefaultHidePopupAction = null;

	protected virtual void Awake()
	{
		m_GameObjectRef = gameObject;
		m_RectTransformRef = transform as RectTransform;

		FindAnimationSetter();
	}

	protected virtual void Start() {}

	protected virtual void OnEnable() {}
	protected virtual void OnDisable() {}

	public void ShowPopupByManager(Action<PopupUI> hidePopupAction, params object[] values)
	{
		m_DefaultHidePopupAction = hidePopupAction;

		Show(values);
	}

	public void HidePopupByManager(params object[] values)
	{
		Hide(values);
	}

	public virtual bool WillHidePopupByBackButtonManager()
	{
		return true;
	}

	public virtual void HidePopupByBackButtonManager()
	{
		Hide();
	}

	public virtual void Show(params object[] values)
	{
		InputBlocker.inst.SetBlockByPopup(this);

		m_FlagSynchronizer.Init(0, 1);
		m_FlagSynchronizer.AddWaitEvent(OnCompleteShowAnimation);

		if (!PlayAnimationSetter(m_ShowAnimationSetter))
			gameObject.SetActive(true);
		
		if (m_ShowAnimationItem != null)
			m_ShowAnimationItem.SetOnCompleteAction(null).SetOnCompleteAction(OnCompleteShowAnimationItem);
	}

	public virtual void Hide(params object[] values)
	{
		InputBlocker.inst.SetBlockByPopup(this);

		m_FlagSynchronizer.Init(0, 1);
		m_FlagSynchronizer.AddWaitEvent(OnCompleteHideAnimation);

		PlayAnimationSetter(m_HideAnimationSetter);
	}

	public PopupUI SetAlias(string alias)
	{
		m_Alias = alias;
		return this;
	}

	public PopupUI SetInitOnceAction(Action<PopupUI> initAction)
	{
		if (!m_IsInit && initAction != null)
			initAction(this);
		
		m_IsInit = true;
		return this;
	}

	public PopupUI SetShowPopupAction(Action showPopupAction)
	{
		m_ShowPopupAction = m_ShowPopupAction.AppendOrClear(showPopupAction);
		return this;
	}

	public PopupUI SetHidePopupAction(Action hidePopupAction)
	{
		m_HidePopupAction = m_HidePopupAction.AppendOrClear(hidePopupAction);
		return this;
	}

	public YieldInstruction WaitForHide()
	{
		if (!isShow)
			return null;
		if (WaitForHideCoroutine == null)
			WaitForHideCoroutine = GlobalManager.Inst.StartCoroutine(CoWaitForHide());
		return WaitForHideCoroutine;
	}
		
	protected void CompleteShowAnimation()
	{
		m_FlagSynchronizer.ResetFlag(1);
	}

	protected void CompleteHideAnimation()
	{
		m_FlagSynchronizer.ResetFlag(1);
	}

	protected virtual void OnCompleteShowAnimation()
	{
		if (m_ShowPopupAction != null)
		{
			m_ShowPopupAction();
			m_ShowPopupAction = null;
		}

		InputBlocker.inst.ResetBlockByPopup(this);
	}

	protected virtual void OnCompleteHideAnimation()
	{
		gameObject.SetActive(false);

		if (m_DefaultHidePopupAction != null)
			m_DefaultHidePopupAction(this);

		if (m_HidePopupAction != null)
		{
			m_HidePopupAction();
			m_HidePopupAction = null;
		}

		InputBlocker.inst.ResetBlockByPopup(this);
	}

	protected virtual void OnCompleteShowAnimationItem(STEffectItem item)
	{
		CompleteShowAnimation();
	}

	public virtual void OnClickDefaultBackgroundImage()
	{
		if (m_IsAutoHideClickDefaultBackground)
			Hide();
	}

	public virtual void OnClickDefaultHideAction()
	{
		Hide();
	}

	private IEnumerator CoWaitForHide()
	{
		while (m_GameObjectRef != null && isShow)
			yield return null;

		WaitForHideCoroutine = null;
	}

	private bool PlayAnimationSetter(UIAnimationSetter animationSetter)
	{
		if (animationSetter == null)
		{
			m_FlagSynchronizer.ResetFlag(0);
			return false;
		}

		animationSetter.Invoke(m_RectTransformRef).OnCompleteAppend(OnCompleteAnimationSetter);
		return true;
	}

	private void OnCompleteAnimationSetter()
	{
		m_FlagSynchronizer.ResetFlag(0);
	}

	private void FindAnimationSetter()
	{
		UIAnimationSetter[] animationSetters = GetComponents<UIAnimationSetter>();

		for (int i = 0; i < animationSetters.Length; ++i)
		{
			UIAnimationEffectorType type = animationSetters[i].animationEffectorType;

			if (UIAnimationEffectorType.PopupShowStart < type && type < UIAnimationEffectorType.PopupShowMax)
				m_ShowAnimationSetter = animationSetters[i];
			else if (UIAnimationEffectorType.PopupHideStart < type && type < UIAnimationEffectorType.PopupHideMax)
				m_HideAnimationSetter = animationSetters[i];
		}
	}
}
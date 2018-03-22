using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public class MoonPopupUI : MonoBehaviour {

	public RectTransform 				rectTransformRef { get { return m_RectTransformRef; } }
	public MoonPopupUI 					popupPrefab { get { return m_UIPrefab;} set { m_UIPrefab = value;}}
	protected RectTransform 			m_RectTransformRef;

	private Action<MoonPopupUI> 		m_HideAction;
	private	MoonPopupUI 				m_UIPrefab;

	private UIAnimationSetter	 		m_ShowAnimationSetter;
	private UIAnimationSetter 			m_HideAnimationSetter;

	private Synchronizer m_animationSynchronizer = new Synchronizer();

	protected virtual void Awake()
	{
		m_RectTransformRef	 = transform as RectTransform;
		m_UIPrefab			 = null;

		UIAnimationSetter [] uiAnimationSetter = GetComponents<UIAnimationSetter>();

		for (int i = 0; i < uiAnimationSetter.Length; i++) 
		{
			switch (uiAnimationSetter[i].UIAnimationType) 
			{
			case UIAnimationEffectorType.PopupShow:
				m_ShowAnimationSetter = GetComponent<UIAnimationSetter>();
				break;
			case UIAnimationEffectorType.PopupHide:
				m_HideAnimationSetter = GetComponent<UIAnimationSetter>();
				break;
			case UIAnimationEffectorType.ButtonScaleUp:
				break;
			case UIAnimationEffectorType.ButtonScaleNomal:
				break;
			}
		}
	}

	public void ShowPopupByManager(Action<MoonPopupUI> hidePopupAction, params object[] values)
	{
		m_HideAction = hidePopupAction;
		Show(values);
	}

	public virtual void Show(params object[] values)
	{
		gameObject.SetActive(true);

		m_animationSynchronizer.Init();
		m_animationSynchronizer.IncreaseCount();
		m_animationSynchronizer.IncreaseCount();
		m_animationSynchronizer.AddWaitEvent(CompleteShowAnimation);

		if (m_ShowAnimationSetter) 
		{
			UIAnimationEffector.Invoke(m_ShowAnimationSetter.UIAnimationType, rectTransformRef);	
		}
	}

	public virtual void Hide(params object[] values)
	{
		if (m_HideAnimationSetter) 
		{
//			UIAnimationEffector.Invoke(m_HideAnimationSetter.UIAnimationType, rectTransformRef).OnComplete(CompleteHideAnimation);
			UIAnimationEffector.Invoke(m_HideAnimationSetter.UIAnimationType, rectTransformRef).OnCompleteAppend(CompleteHideAnimation);
		} 
		else 
		{
			CompleteHideAnimation();	
		}
	}

	protected void CompleteShowAnimation()
	{
		
	}

	protected void CompleteHideAnimation()
	{
		gameObject.SetActive(false);
		if(m_HideAction != null)
			m_HideAction(this);		
	}

}

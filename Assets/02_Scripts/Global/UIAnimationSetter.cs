using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using STDOTweenExtensions;

public class UIAnimationSetter : MonoBehaviour
{
	[SerializeField] private UIAnimationEffectorType m_AnimationEffectorType = UIAnimationEffectorType.None;

	public UIAnimationEffectorType animationEffectorType { get { return m_AnimationEffectorType; } }

	public Tween Invoke(RectTransform rectTransform)
	{
		return UIAnimationEffector.Invoke(m_AnimationEffectorType, rectTransform);
	}
}
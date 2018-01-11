using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum UIAnimationEffectorType
{
	None = 0,

	ButtonScaleUp,
	ButtonScaleNomal,

	PopupShow = 90010,
	PopupHide = 90020,
}

public class UIAnimationEffector : MonoBehaviour {

	public static Tween Invoke(UIAnimationEffectorType type, RectTransform rectTransform)
	{
		switch (type) 
		{
		case UIAnimationEffectorType.ButtonScaleUp:
			return	PlayPopUpShowAnimation(rectTransform);

		case UIAnimationEffectorType.ButtonScaleNomal:
			return PlayPopUpHideAnimation(rectTransform);

		case UIAnimationEffectorType.PopupShow:
			return PlayPopUpShowAnimation(rectTransform);

		case UIAnimationEffectorType.PopupHide:
			return PlayPopUpHideAnimation(rectTransform);
		
		default:
			return rectTransform.DoEmpty(); 
		}
	}

	public static Tween PlayPopUpShowAnimation(RectTransform rectTransform)
	{
		Sequence sequence = DOTween.Sequence();
		Vector3 originScale = rectTransform.localScale;
		float bigScale = 1.5f;
		float smallScale = 0.5f;

		sequence.Append(rectTransform.DOScale(smallScale, 0.2f).SetEase(Ease.Flash));
		sequence.Append(rectTransform.DOScale(bigScale, 0.2f).SetEase(Ease.Flash));

		return sequence;
	}

	public static Tween PlayPopUpHideAnimation(RectTransform rectTransform)
	{
		Sequence sequence = DOTween.Sequence();
		Vector3 originScale = rectTransform.localScale;
		float bigScale = 1.5f;
		float smallScale = 0.5f;

		sequence.Append(rectTransform.DOScale(smallScale, 0.2f).SetEase(Ease.Flash));
		sequence.Append(rectTransform.DOScale(bigScale, 0.2f).SetEase(Ease.Flash));
		sequence.AppendCallback(() =>
		{
			rectTransform.localScale = originScale;
//			rectTransform.gameObject.SetActive(false);
		});
		return sequence;
	}
}

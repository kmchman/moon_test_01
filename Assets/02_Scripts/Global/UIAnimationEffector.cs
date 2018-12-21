using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using STExtensions;

using DG.Tweening;
using STDOTweenExtensions;

public enum UIAnimationEffectorType
{
	None = 0,

	ButtonScaleNomal,
	ButtonScaleUp,
	ButtonScaleDown,
	ButtonBounceUpNormal,
	ButtonMixBounce,

	PopupShowStart = 90000,
	PopupShow = 90010,
	PopupShowFromLeft,
	PopupShowFromTop,
	PopupShowFromRight,
	PopupShowFromBottom,
	PopupShowMax,

	PopupHideStart = 90100,
	PopupHide = 90110,
	PopupHideToLeft,
	PopupHideToTop,
	PopupHideToRight,
	PopupHideToBottom,
	PopupHideMax,
}

public class UIAnimationEffector : MonoBehaviour
{
	private enum Direction
	{
		None,
		Left,
		Top,
		Right,
		Bottom,
	}

	public static Tween Invoke(UIAnimationEffectorType type, RectTransform rectTransform)
	{
		switch (type)
		{
		case UIAnimationEffectorType.ButtonScaleNomal:		return PlayButtonScaleNormalAnimation(rectTransform);
		case UIAnimationEffectorType.ButtonScaleUp:			return PlayButtonScaleUpAnimation(rectTransform);
		case UIAnimationEffectorType.ButtonScaleDown:		return PlayButtonScaleDownAnimation(rectTransform);
		case UIAnimationEffectorType.ButtonBounceUpNormal:	return PlayButtonBounceUpNormalAnimation(rectTransform);
		case UIAnimationEffectorType.ButtonMixBounce:		return PlayButtonMixBounceAnimation(rectTransform);
			
		case UIAnimationEffectorType.PopupShow:				return PlayPopupShowAnimation(rectTransform);
		case UIAnimationEffectorType.PopupShowFromLeft:		return PlayPopupDirectionShowAnimation(rectTransform, Direction.Left);
		case UIAnimationEffectorType.PopupShowFromTop:		return PlayPopupDirectionShowAnimation(rectTransform, Direction.Top);
		case UIAnimationEffectorType.PopupShowFromRight:	return PlayPopupDirectionShowAnimation(rectTransform, Direction.Right);
		case UIAnimationEffectorType.PopupShowFromBottom:	return PlayPopupDirectionShowAnimation(rectTransform, Direction.Bottom);
			
		case UIAnimationEffectorType.PopupHide:				return PlayPopupHideAnimation(rectTransform);
		case UIAnimationEffectorType.PopupHideToLeft:		return PlayPopupDirectionHideAnimation(rectTransform, Direction.Left);
		case UIAnimationEffectorType.PopupHideToTop:		return PlayPopupDirectionHideAnimation(rectTransform, Direction.Top);
		case UIAnimationEffectorType.PopupHideToRight:		return PlayPopupDirectionHideAnimation(rectTransform, Direction.Right);
		case UIAnimationEffectorType.PopupHideToBottom:		return PlayPopupDirectionHideAnimation(rectTransform, Direction.Bottom);

		default:
			return rectTransform.DOEmpty();
		}
	}

	private static float GetDurationByDistance3000Per1(float distance) { return GetDurationByDistance(distance, 3000f, 1f); }
	private static float GetDurationByDistance(float distance, float distanceUnit, float durationUnit)
	{
		return Mathf.Abs(distance / distanceUnit * durationUnit);
	}

	private static Tween PlayPopupShowAnimation(RectTransform rectTransform)
	{
		float initScale = 0.8f;
		float bigScale = 1.1f;

		rectTransform.gameObject.SetActive(true);

		Vector3 originalScale = rectTransform.localScale;

		rectTransform.localScale *= initScale;

		Sequence sequence = DOTween.Sequence();
		sequence.Append(rectTransform.DOScale(originalScale * bigScale, 0.15f)).SetEase(Ease.OutExpo);
		sequence.Append(rectTransform.DOScale(originalScale, 0.1f)).SetEase(Ease.InSine);

		return sequence;
	}

	private static Tween PlayPopupHideAnimation(RectTransform rectTransform)
	{
		float bigScale = 1.1f;
		float goalScale = 0.4f;

		rectTransform.gameObject.SetActive(true);

		Vector3 originalScale = rectTransform.localScale;

		Sequence sequence = DOTween.Sequence();
		sequence.Append(rectTransform.DOScale(originalScale * bigScale, 0.1f)).SetEase(Ease.OutExpo);
		sequence.Append(rectTransform.DOScale(originalScale * goalScale, 0.2f)).SetEase(Ease.InSine);
		sequence.AppendCallback(() => 
		{
			rectTransform.gameObject.SetActive(false);
			rectTransform.localScale = originalScale;
		});

		return sequence;
	}

	private static Tween PlayPopupDirectionShowAnimation(RectTransform rectTransform, Direction direction)
	{
		Vector2 originalPosition = rectTransform.anchoredPosition;
		Vector2 size = new Vector2(rectTransform.GetWidth(), rectTransform.GetHeight());

		rectTransform.gameObject.SetActive(true);
		float distance = 0;

//		Vector2 halfCanvasSize = SceneRoot.inst.canvasRectTransformRef.sizeDelta * 0.5f;
//
//		switch (direction)
//		{
//		case Direction.Left:
//			rectTransform.localPosition = new Vector2(-(halfCanvasSize.x + size.x), rectTransform.localPosition.y);
//			distance = rectTransform.anchoredPosition.x - originalPosition.x;
//			break;
//		case Direction.Top:
//			rectTransform.localPosition = new Vector2(rectTransform.localPosition.x, (halfCanvasSize.y + size.y));
//			distance = rectTransform.anchoredPosition.y - originalPosition.y;
//			break;
//		case Direction.Right:
//			rectTransform.localPosition = new Vector2((halfCanvasSize.x + size.x), rectTransform.localPosition.y);
//			distance = rectTransform.anchoredPosition.x - originalPosition.x;
//			break;
//		case Direction.Bottom:
//			rectTransform.localPosition = new Vector2(rectTransform.localPosition.x, -(halfCanvasSize.y + size.y));
//			distance = rectTransform.anchoredPosition.y - originalPosition.y;
//			break;
//		}

		Tweener tweener = rectTransform.DOAnchorPos(originalPosition, GetDurationByDistance3000Per1(distance)).SetEase(Ease.OutQuad);
		tweener.OnCompleteAppend(() =>
		{
			rectTransform.anchoredPosition = originalPosition;
		});

		return tweener;
	}

	private static Tween PlayPopupDirectionHideAnimation(RectTransform rectTransform, Direction direction)
	{
		Vector3 originalPosition = rectTransform.localPosition;
		Vector2 goalPosition = Vector2.zero;
		Vector2 size = new Vector2(rectTransform.GetWidth(), rectTransform.GetHeight());

		rectTransform.gameObject.SetActive(true);
		float distance = 0;

//		Vector2 halfCanvasSize = SceneRoot.inst.canvasRectTransformRef.sizeDelta * 0.5f;
//
//		switch (direction)
//		{
//		case Direction.Left:
//			goalPosition = new Vector2(-(halfCanvasSize.x + size.x), rectTransform.localPosition.y);
//			distance = goalPosition.x - originalPosition.x;
//
//			break;
//		case Direction.Top:
//			goalPosition = new Vector2(rectTransform.localPosition.x, (halfCanvasSize.y + size.y));
//			distance = goalPosition.y - originalPosition.y;
//			break;
//		case Direction.Right:
//			goalPosition = new Vector2((halfCanvasSize.x + size.x), rectTransform.localPosition.y);
//			distance = goalPosition.x - originalPosition.x;
//			break;
//		case Direction.Bottom:
//			goalPosition = new Vector2(rectTransform.localPosition.x, -(halfCanvasSize.y + size.y));
//			distance = goalPosition.y - originalPosition.y;
//			break;
//		}

		Tweener tweener = rectTransform.DOLocalMoveXY(goalPosition, GetDurationByDistance3000Per1(distance)).SetEase(Ease.InQuad);
		tweener.OnCompleteAppend(() =>
		{
			rectTransform.gameObject.SetActive(false);
			rectTransform.localPosition = originalPosition;
		});

		return tweener;
	}

	private static Tween PlayButtonScaleNormalAnimation(RectTransform rectTransform)
	{
		float normalScale = 1f;
		float duration = 0.25f;

		return rectTransform.DoScaleFromCenter(normalScale, duration);
	}

	private static Tween PlayButtonScaleUpAnimation(RectTransform rectTransform)
	{
		float upScale = 1.2f;
		float duration = 0.25f;

		return rectTransform.DoScaleFromCenter(upScale, duration);
	}

	private static Tween PlayButtonScaleDownAnimation(RectTransform rectTransform)
	{
		float downScale = 0.9f;
		float duration = 0.25f;

		return rectTransform.DoScaleFromCenter(downScale, duration);
	}

	private static Tween PlayButtonBounceUpNormalAnimation(RectTransform rectTrasnform)
	{
		Sequence sequence = DOTween.Sequence();

		sequence.Append(rectTrasnform.DoScaleFromCenter(1.06f, 0.1f).SetEase(Ease.OutExpo));
		sequence.Append(rectTrasnform.DoScaleFromCenter(1f, 0.06f).SetEase(Ease.InExpo));

		return sequence;
	}

	private static Tween PlayButtonMixBounceAnimation(RectTransform rectTrasnform)
	{
		Sequence sequence = DOTween.Sequence();

		sequence.Append(PlayButtonScaleDownAnimation(rectTrasnform));
		sequence.Append(PlayButtonBounceUpNormalAnimation(rectTrasnform));

		return sequence;
	}
}
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using STExtensions;

using DG.Tweening;

namespace STDOTweenExtensions
{
	public static class TweenExtensions
	{
		private static FieldInfo onCompleteFieldInfo = null;
		private static FieldInfo onUpdateFieldInfo = null;
		private static FieldInfo onKillFieldInfo = null;

		private static HashSet<Tween> PauseEventActionTweens = new HashSet<Tween>();

		public static bool IsPauseEventAction()
		{
			return PauseEventActionTweens.Count > 0;
		}

		public static TypeName OnCheckComplete<TypeName>(this TypeName tween, System.Action onComplete) where TypeName : Tween
		{
			if (onComplete == null)
				return tween;

			return tween.OnCompleteAppend(new TweenCallback(onComplete));
		}

		public static TypeName OnCompleteAppend<TypeName>(this TypeName tween, TweenCallback appendOnComplete) where TypeName : Tween
		{
			if (appendOnComplete == null)
				return tween;

			if (onCompleteFieldInfo == null)
				onCompleteFieldInfo = typeof(Tween).GetField("onComplete", BindingFlags.Instance | BindingFlags.NonPublic);

			TweenCallback onComplete = (TweenCallback)onCompleteFieldInfo.GetValue(tween);
			onComplete += appendOnComplete;

			onCompleteFieldInfo.SetValue(tween, onComplete);

			return tween;
		}

		public static TypeName OnUpdateAppend<TypeName>(this TypeName tween, System.Action<TypeName> appendOnUpdate) where TypeName : Tween
		{
			if (appendOnUpdate == null)
				return tween;

			if(onUpdateFieldInfo == null)
				onUpdateFieldInfo = typeof(Tween).GetField("onUpdate", BindingFlags.Instance | BindingFlags.NonPublic);

			TweenCallback onUpdate = (TweenCallback)onUpdateFieldInfo.GetValue(tween);
			onUpdate += () => { appendOnUpdate(tween); };

			onUpdateFieldInfo.SetValue(tween, onUpdate);

			return tween;
		}

		public static TypeName OnKillAppend<TypeName>(this TypeName tween, TweenCallback appendOnKill) where TypeName : Tween
		{
			if (appendOnKill == null)
				return tween;

			if (onKillFieldInfo == null)
				onKillFieldInfo = typeof(Tween).GetField("onKill", BindingFlags.Instance | BindingFlags.NonPublic);

			TweenCallback onKill = (TweenCallback)onKillFieldInfo.GetValue(tween);
			onKill += appendOnKill;

			onKillFieldInfo.SetValue(tween, onKill);

			return tween;
		}

		public static TypeName SetIsPauseEvent<TypeName>(this TypeName tween, bool isPause) where TypeName : Tween
		{
			if (isPause)
			{
				PauseEventActionTweens.Add(tween);
				tween.OnKillAppend(() => { RemovePauseEventTween(tween); });
			}
			else
			{
				PauseEventActionTweens.Remove(tween);
			}
			
			return tween;
		}

		public static TypeName SetIsBlockInputEvent<TypeName>(this TypeName tween, bool isBlock) where TypeName : Tween
		{
			if (isBlock)
			{
				InputBlocker.inst.SetBlockByDOTween(tween);
				tween.OnKillAppend(() => { InputBlocker.inst.ResetBlockByDOTween(tween); });
			}
			else
			{
				InputBlocker.inst.ResetBlockByDOTween(tween);
			}

			return tween;
		}

		private static void RemovePauseEventTween(Tween tween)
		{
			PauseEventActionTweens.Remove(tween);
		}
	}

	public static class GraphicExtensions
	{
		public static Tween DOGrayScale(this Graphic graphic, float duraion)
		{
			graphic.material = GlobalDataStore.Inst.GetCloneUIMaskGrayScaleMaterial();

			return DOTween.To((percent) =>
			{
				graphic.material.SetFloat("_EffectAmount", percent);
			}, 0, 1, duraion).SetEase(Ease.OutCirc).SetTarget(graphic);
		}

		public static Tween DOCanvasRenderColor(this Graphic graphic, Color endColor, float duration)
		{
			return DOTween.To(graphic.canvasRenderer.GetColor, (color) => { graphic.canvasRenderer.SetColor(color); }, endColor, duration);
		}
	}

	public static class CanvasGroupExtensions
	{
		public static bool IsDOAnimation(this CanvasGroup canvasGroup)
		{
			return DOTween.IsTweening(canvasGroup);
		}

		public static Tweener DOAlpha(this CanvasGroup canvasGroup, float alpha, float duraion)
		{
			return DOTween.To((percent) =>
			{
				canvasGroup.alpha = percent;
			}, canvasGroup.alpha, alpha, duraion).SetTarget(canvasGroup);
		}
	}

	public static class TransformExtensions
	{
		public static bool IsDOAnimation(this Transform transform)
		{
			return DOTween.IsTweening(transform);
		}

		public static int DOKill(this Transform transform, bool complete)
		{
			return DOTween.Kill(transform, complete);
		}

		public static Tween DOEmpty(this Transform transform)
		{
			return DOVirtual.DelayedCall(0, EmptyCallBack);
		}

		public static Tween DOInvoke(this Transform transform, Action invoke, float delay)
		{
			return DOVirtual.DelayedCall(delay, new TweenCallback(invoke)).SetTarget(transform);
		}

		// Move
		public static Tweener DOLocalMoveXY(this Transform transform, Vector2 position, float duraion) { return DOLocalMoveXY(transform, position.x, position.y, duraion); }
		public static Tweener DOLocalMoveXY(this Transform transform, float x, float y, float duraion)
		{
			return transform.DOLocalMove(new Vector3(x, y, 0), duraion);
		}

		public static Tweener DOMoveXY(this Transform transform, Vector2 position, float duration) { return DOMoveXY(transform, position.x, position.y, duration); }
		public static Tweener DOMoveXY(this Transform transform, float x, float y, float duration)
		{
			return transform.DOMove(new Vector3(x, y, transform.position.z), duration);
		}
			
		// Roration
		public static Tweener DOLocalRotateX(this Transform transform, float x, float duraion)
		{
			return transform.DOLocalRotate(new Vector3(x, 0, 0), duraion);
		}

		public static Tweener DOLocalRotateY(this Transform transform, float y, float duraion)
		{
			return transform.DOLocalRotate(new Vector3(0, y, 0), duraion);
		}

		public static Tweener DOLocalRotateZ(this Transform transform, float z, float duraion)
		{
			return transform.DOLocalRotate(new Vector3(0, 0, z), duraion);
		}

		public static Tweener DOSTPunchRotation(this Transform transform, Vector3 punch, float duration)
		{
			Vector3 prevAngle = Vector3.zero;

			return DOTween.To((percent) =>
			{
				Vector3 newAngle = PunchRotation(punch, percent);
				transform.Rotate(newAngle - prevAngle, Space.Self);
				prevAngle = newAngle;
			}, 0, 1, duration).SetEase(Ease.Linear).SetTarget(transform);
		}

		// Scale
		public static Tweener DOScaleXY(this Transform transform, float xy, float duraion) { return DOScaleXY(transform, xy, xy, duraion); }
		public static Tweener DOScaleXY(this Transform transform, Vector2 scale, float duraion) { return DOScaleXY(transform, scale.x, scale.y, duraion); }
		public static Tweener DOScaleXY(this Transform transform, float x, float y, float duraion)
		{
			return transform.DOScale(new Vector3(x, y, transform.localScale.z), duraion);
		}

		public static Tweener DOSTPunchScaleXY(this Transform transform, float x, float y, float duration) { return transform.DOSTPunchScale(new Vector3(x, y, transform.localScale.z), duration); }
		public static Tweener DOSTPunchScale(this Transform transform, Vector3 punch, float duration)
		{
			Vector3 originalScale = transform.localScale;

			return DOTween.To((percent) =>
			{
				transform.localScale = originalScale + PunchScale(punch, percent);
			}, 0, 1, duration).SetEase(Ease.Linear).SetTarget(transform);
		}

		public static void DoBoxOpen(this Transform transform, System.Action endCB)
		{
			Sequence jumpSequencer = DOTween.Sequence();
			Sequence rotationSequencers = DOTween.Sequence();

			jumpSequencer.Append(transform.DOLocalMoveY(110, 0.1f).SetEase(Ease.OutCirc));
			jumpSequencer.Append(transform.DOLocalMoveY(0, 0.1f).SetEase(Ease.OutCirc));
			jumpSequencer.SetTarget(transform);

			rotationSequencers.Append(transform.DOLocalRotateZ(10, 0.05f).SetEase(Ease.InQuad));
			rotationSequencers.Append(transform.DOLocalRotateZ(-10, 0.05f).SetEase(Ease.InQuad));
			rotationSequencers.Append(transform.DOLocalRotateZ(7, 0.05f).SetEase(Ease.InQuad));
			rotationSequencers.OnCompleteAppend(() =>
			{
				transform.localEulerAngles = Vector3.zero;
				if(endCB != null)
					endCB();
			});
			rotationSequencers.SetTarget(transform);
		}

		private static void EmptyCallBack()
		{
			
		}

		private static Vector3 PunchRotation(Vector3 punch, float percent)
		{
			float x, y, z;
			x = y = z = 0;
			
			if (punch.x > 0)
				x = Punch(punch.x, percent);
			else if (punch.x < 0)
				x = -Punch(Mathf.Abs(punch.x), percent);

			if (punch.y > 0)
				y = Punch(punch.y, percent);
			else if (punch.y < 0)
				y = -Punch(Mathf.Abs(punch.y), percent);

			if (punch.z > 0)
				z = Punch(punch.z, percent);
			else if (punch.z < 0)
				z = -Punch(Mathf.Abs(punch.z), percent);

			return new Vector3(x, y, z);
		}

		private static Vector3 PunchScale(Vector3 punch, float percent)
		{
			float x, y, z;
			x = y = z = 0;

			if (punch.x > 0)
				x = Punch(punch.x, percent);
			else if(punch.x < 0)
				x = -Punch(Mathf.Abs(punch.x), percent);

			if (punch.y > 0)
				y = Punch(punch.y, percent);
			else if(punch.y < 0)
				y = -Punch(Mathf.Abs(punch.y), percent);
			
			if (punch.z > 0)
				z = Punch(punch.z, percent);
			else if(punch.z < 0)
				z = -Punch(Mathf.Abs(punch.z), percent);

			return new Vector3(x, y, z);
		}

		private static float Punch(float amplitude, float value)
		{
			float s = 9;
			if (value == 0)
				return 0;
			else if (value == 1)
				return 0;
			
			float period = 1 * 0.3f;
			s = period / (2 * Mathf.PI) * Mathf.Asin(0);
			return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
		}
	}

	public static class RectTransformExtensions
	{
		public static Tween DoScaleFromCenter(this RectTransform rectTransform, float scale, float duration)
		{
			Vector3 goal = GetScaleDeltaPosition(rectTransform, scale);
			goal.z = scale;

			return DOTween.To(() => { return new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, rectTransform.localScale.x); },
			(value) =>
			{
				rectTransform.localPosition = new Vector3(value.x, value.y, rectTransform.localPosition.z);
				rectTransform.localScale = new Vector3(value.z, value.z, rectTransform.localScale.z);
			}, goal, duration).SetTarget(rectTransform);
		}

		internal static Vector2 GetScaleDeltaPosition(this RectTransform rectTransform, float scale)
		{
			bool isUpScale = (scale > rectTransform.localScale.x);
			Vector3 goal = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, scale);
			Vector2 pivot = rectTransform.GetSignedPivot();

			float dScale = (isUpScale ? scale - rectTransform.localScale.x : rectTransform.localScale.x - scale);

			if (!pivot.x.IsZero())
			{
				float offset = rectTransform.GetWidth() * dScale * pivot.x;
				goal.x += offset * (isUpScale ? 1 : -1);
			}

			if (!pivot.y.IsZero())
			{
				float offset = rectTransform.GetHeight() * dScale * pivot.y;
				goal.y += offset * (isUpScale ? 1 : -1);
			}

			return goal;
		}
	}
}
using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Reflection;

public static class DoTweenExtension {

	private static FieldInfo onCompleteFieldInfo = null;

	public static T OnCompleteAppend<T> (this T tween, TweenCallback appendOnComplete) where T : Tween
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
//	public static TypeName OnCompleteAppend<TypeName>(this TypeName tween, TweenCallback appendOnComplete) where TypeName : Tween
//	{
//		if (appendOnComplete == null)
//			return tween;
//
//		if (onCompleteFieldInfo == null)
//			onCompleteFieldInfo = typeof(Tween).GetField("onComplete", BindingFlags.Instance | BindingFlags.NonPublic);
//
//		TweenCallback onComplete = (TweenCallback)onCompleteFieldInfo.GetValue(tween);
//		onComplete += appendOnComplete;
//
//		onCompleteFieldInfo.SetValue(tween, onComplete);
//
//		return tween;
//	}
}

public static class TransFormExtension
{
	public static Tween DoEmpty(this Transform transform)
	{
		return	DOVirtual.DelayedCall(0, () =>
		{
		});
	}
}
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace STExtensions
{
	public struct TransformDataSet
	{
		public Vector3 position;
		public Vector3 scale;
		public Vector3 eulerAngles;
	}

	public static class FloatExtensions
	{
		public static bool IsZero(this float value)
		{
			return Math.Abs(value) <= float.Epsilon;
		}

		public static bool IsEqual(this float value, float value2)
		{
			return Math.Abs(value - value2) <= float.Epsilon;
		}
	}

	public static class CameraExtensions
	{
		public static Vector3 WorldToWorldPoint(this Camera camera, Camera anotherCamera, Vector3 position)
		{
			return anotherCamera.ScreenToWorldPoint(camera.WorldToScreenPoint(position));
		}
	}

	public static class TransformExtensions
	{
		public static void Reset(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
			transform.localEulerAngles = Vector3.zero;
			transform.localScale = Vector3.one;
		}

		public static void SetPositionXY(this Transform transform, Vector3 position)
		{
			transform.position = position;
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
		}

		public static void SetLocalScaleXY(this Transform transform, float scale)
		{
			transform.localScale = new Vector3(scale, scale, transform.localScale.z);
		}

		public static void SetLocalScaleX(this Transform transform, float x)
		{
			transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
		}

		public static void SetLocalScaleY(this Transform transform, float y)
		{
			transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
		}

		public static Vector3 GetScreenPoint(this Transform transform, ref Canvas canvas)
		{
			return GetScreenPoint(transform, transform.position, ref canvas);
		}

		public static void SetScreenPoint(this Transform transform, Vector3 position, ref Canvas canvas)
		{
			if (canvas == null)
				canvas = transform.GetComponentInParent<Canvas>();

			if (canvas == null)
			{
				Debug.LogError(Util.LogFormat("Not Found Parent Canvas", transform), transform);
				return;
			}

			SetPositionXY(transform, canvas.worldCamera.ScreenToWorldPoint(position));
		}

		public static TransformDataSet SaveLocal(this Transform transform)
		{
			TransformDataSet dataSet = new TransformDataSet();
			dataSet.position = transform.localPosition;
			dataSet.eulerAngles = transform.localEulerAngles;
			dataSet.scale = transform.localScale;

			return dataSet;
		}

		public static void LoadLocal(this Transform transform, TransformDataSet dataSet)
		{
			transform.localPosition = dataSet.position;
			transform.localEulerAngles = dataSet.eulerAngles;
			transform.localScale = dataSet.scale;
		}

		internal static Vector3 GetScreenPoint(Transform transform, Vector3 position, ref Canvas canvas)
		{
			if (canvas == null)
				canvas = transform.GetComponentInParent<Canvas>();

			if (canvas == null)
			{
				Debug.LogError(Util.LogFormat("Not Found Parent Canvas", transform), transform);
				return Vector3.zero;
			}

			return canvas.worldCamera.WorldToScreenPoint(position);
		}

	}

	public static class RectTransformExtensions
	{
		private static Vector3[] tempCorners = new Vector3[4];

		public static Rect GetWorldRect(this RectTransform rectTransform)
		{
			rectTransform.GetWorldCorners(tempCorners);

			Vector3 topLeft = tempCorners[0];
			Vector2 scaledSize = new Vector2(rectTransform.lossyScale.x * rectTransform.rect.size.x, rectTransform.lossyScale.y * rectTransform.rect.size.y);

			return new Rect(topLeft, scaledSize);
		}

		public static Vector3 GetCenterScreenPoint(this RectTransform rectTransform)
		{
			Vector2 center = rectTransform.TransformVector(rectTransform.rect.center);
			Vector3 position = rectTransform.position + new Vector3(center.x, center.y, 0);

			Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
			return TransformExtensions.GetScreenPoint(rectTransform, position, ref canvas);
		}

		public static Vector2 GetSignedPivot(this RectTransform rectTransform)
		{
			return rectTransform.pivot - new Vector2(0.5f, 0.5f);
		}

		public static float GetWidth(this RectTransform rectTransform)
		{
			return rectTransform.rect.width;
		}

		public static float GetLocalScaledWidth(this RectTransform rectTransform)
		{
			return rectTransform.GetWidth() * rectTransform.localScale.x;
		}

		public static float GetLossyScaledWidth(this RectTransform rectTransform)
		{
			return rectTransform.GetWidth() * rectTransform.lossyScale.x;
		}

		public static float GetHeight(this RectTransform rectTransform)
		{
			return rectTransform.rect.height;
		}

		public static float GetLocalScaledHeight(this RectTransform rectTransform)
		{
			return rectTransform.GetHeight() * rectTransform.localScale.y;
		}

		public static float GetLossyScaledHeight(this RectTransform rectTransform)
		{
			return rectTransform.GetHeight() * rectTransform.lossyScale.y;
		}

		public static void SetSize(this RectTransform rectTransform, Vector2 size) { SetSize(rectTransform, size.x, size.y); }
		public static void SetSize(this RectTransform rectTransform, float width, float height)
		{
			Vector2 anchorMin = rectTransform.anchorMin;
			Vector2 anchorMax = rectTransform.anchorMax;

			// Stretch
			if (anchorMin.x == 0 && anchorMax.x == 1)
				SetStretchWidth(rectTransform, width);
			else
				SetNonStretchWidth(rectTransform, width);

			// Stretch
			if (anchorMin.y == 0 && anchorMax.y == 1)
				SetStretchHeight(rectTransform, height);
			else
				SetNonStretchHeight(rectTransform, height);
		}

		private static void SetStretchWidth(this RectTransform rectTransform, float width)
		{
			width = width - (rectTransform.parent as RectTransform).rect.width;
			SetNonStretchWidth(rectTransform, width);
		}

		private static void SetStretchHeight(this RectTransform rectTransform, float height)
		{
			height = height - (rectTransform.parent as RectTransform).rect.height;
			SetNonStretchHeight(rectTransform, height);
		}

		private static void SetNonStretchWidth(this RectTransform rectTransform, float width)
		{
			rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
		}

		private static void SetNonStretchHeight(this RectTransform rectTransform, float height)
		{
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
		}

		public static void SetWidth(this RectTransform rectTransform, float width)
		{
			Vector2 anchorMin = rectTransform.anchorMin;
			Vector2 anchorMax = rectTransform.anchorMax;

			// Stretch
			if (anchorMin.x == 0 && anchorMax.x == 1)
				SetStretchWidth(rectTransform, width);
			else
				SetNonStretchWidth(rectTransform, width);
		}

		public static void SetHeight(this RectTransform rectTransform, float height)
		{
			Vector2 anchorMin = rectTransform.anchorMin;
			Vector2 anchorMax = rectTransform.anchorMax;

			// Stretch
			if (anchorMin.y == 0 && anchorMax.y == 1)
				SetStretchHeight(rectTransform, height);
			else
				SetNonStretchHeight(rectTransform, height);
		}

		public static void SetAnchoredPositionX(this RectTransform rectTransform, float x)
		{
			rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
		}

		public static void SetAnchoredPositionY(this RectTransform rectTransform, float y)
		{
			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);
		}
	}

	public static class GraphicExtensions
	{

		public static void SetAlpha(this Graphic graphic, float alpha)
		{
			Color color = graphic.color;
			color.a = alpha;
			graphic.color = color;
		}

		public static void SetGrayScaleAlpha(this Graphic graphic, bool isGrayScale)
		{
			graphic.SetAlpha(isGrayScale ? 0.784f : 1f);
		}
	}

	public static class TextExtensions
	{
		public static float CalcPreferredWidth(this Text text, string str)
		{
			TextGenerationSettings generationSettings = text.GetGenerationSettings(Vector2.zero);
			return text.cachedTextGeneratorForLayout.GetPreferredWidth(str, generationSettings) / text.pixelsPerUnit;
		}

		public static float CalcPreferredHeight(this Text text, string str)
		{
			TextGenerationSettings generationSettings = text.GetGenerationSettings(new Vector2(text.rectTransform.rect.size.x, 0));
			return text.cachedTextGeneratorForLayout.GetPreferredHeight(str, generationSettings) / text.pixelsPerUnit;
		}

		public static Rect GetTextRect(this Text text)
		{	                                  
			IList<UIVertex> verts = text.cachedTextGenerator.verts;

			float minX = float.MaxValue;
			float minY = float.MaxValue;
			float maxX = float.MinValue;
			float maxY = float.MinValue;
			float positionX, positionY;

			for (int i = 0; i < verts.Count; ++i)
			{
				positionX = verts[i].position.x;
				positionY = verts[i].position.y;

				if (positionX < minX) minX = positionX;
				if (positionY < minY) minY = positionY;

				if (maxX < positionX) maxX = positionX;
				if (maxY < positionY) maxY = positionY;
			}
				
			float pixelsPerUnit = 1.0f / text.canvas.scaleFactor;
			minX *= pixelsPerUnit;
			minY *= pixelsPerUnit;
			maxX *= pixelsPerUnit;
			maxY *= pixelsPerUnit;

			return new Rect(minX, minY, (maxX - minX), (maxY - minY));
		}
	}

	public static class StringExtensions
	{
		public static System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

		public static string RemoveWhiteSpace(this string str)
		{
			if (str == null)
				return str;
			
			stringBuilder.Remove(0, stringBuilder.Length);

			for (int i = 0; i < str.Length; ++i)
			{
				char c = str[i];
				if (char.IsWhiteSpace(c))
					continue;

				stringBuilder.Append(c);
			}

			return stringBuilder.ToString();
		}
	} 

	public static class DictionaryExtensions
	{
		public static void AddValue<KeyType>(this Dictionary<KeyType, int> dictionary, KeyType key, int value)
		{
			if (!dictionary.ContainsKey(key))
				dictionary[key] = 0;
			dictionary[key] += value;
		}
	}

	public static class HashSetExtensions
	{
		public static void AddRange<Type>(this HashSet<Type> hashSet, IEnumerable<Type> collection)
		{
			if (collection == null)
				return;

			var enumerator = collection.GetEnumerator();
			while (enumerator.MoveNext())
				hashSet.Add(enumerator.Current);
		}
	}

	public static class ActionExtensions
	{
		public static Action AppendOrClear(this Action action, Action anotherAction)
		{
			if (anotherAction == null)
				action = null;
			else
				action += anotherAction;
			return action;
		}

		public static Action<T> AppendOrClear<T>(this Action<T> action, Action<T> anotherAction)
		{
			if (anotherAction == null)
				action = null;
			else
				action += anotherAction;
			return action;
		}

		public static Action<T1, T2> AppendOrClear<T1, T2>(this Action<T1, T2> action, Action<T1, T2> anotherAction)
		{
			if (anotherAction == null)
				action = null;
			else
				action += anotherAction;
			return action;
		}

		public static Action<T1, T2, T3> AppendOrClear<T1, T2, T3>(this Action<T1, T2, T3> action, Action<T1, T2, T3> anotherAction)
		{
			if (anotherAction == null)
				action = null;
			else
				action += anotherAction;
			return action;
		}

		public static Action<T1, T2, T3, T4> AppendOrClear<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, Action<T1, T2, T3, T4> anotherAction)
		{
			if (anotherAction == null)
				action = null;
			else
				action += anotherAction;
			return action;
		}
	}
}
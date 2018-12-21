using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class SafeAreaHelper : MonoBehaviour
{
	private void Start()
	{
		ApplySafeArea();
	}

	private void ApplySafeArea()
	{
		float div_width = 1f / Screen.width, div_height = 1f / Screen.height;
		Rect safeArea = GetSafeArea();
		Vector2 anchorMin = safeArea.position;
		Vector2 anchorMax = safeArea.position + safeArea.size;

		anchorMin.x *= div_width;
		anchorMin.y *= div_height;
		anchorMax.x *= div_width;
		anchorMax.y *= div_height;

		RectTransform rectTransform = GetComponent<RectTransform>();
		rectTransform.anchorMin = anchorMin;
		rectTransform.anchorMax = anchorMax;
	}

	private Rect GetSafeArea()
	{
#if UNITY_EDITOR && IPHONE_X_TEST
		return new Rect(132f, 63f, 2172f, 1062f);
#else
		return Screen.safeArea;
#endif
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneRoot : MonoBehaviour {

	private static SceneRoot m_Inst;
	public static SceneRoot inst { get { return m_Inst; } }

	private RectTransform m_CanvasRectTransformRef;
	public RectTransform canvasRectTrasformRef { get { return m_CanvasRectTransformRef; } }

	[SerializeField] private Camera m_camera;
	[SerializeField] private Canvas m_canvas;

	private void Awake()
	{
		m_Inst = this;	
		m_CanvasRectTransformRef = m_canvas.transform as RectTransform;
	}

	private void Destroy()
	{
		m_Inst = null;
		CleanUpCanvas();
	}

	private void CleanUpCanvas()
	{
		if (m_canvas == null)
			return;
		
		Image[] images = m_canvas.GetComponentsInChildren<Image>(true);
		for (int i = 0; i < images.Length; i++) 
		{
			images[i].overrideSprite = null;
			images[i].sprite = null;
		}
		
	}
}

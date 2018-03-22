using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MoonPopupManager : MonoBehaviour {

	// static
	private static Dictionary<int, MoonPopupManager> 		m_PopupManager = new Dictionary<int, MoonPopupManager>();

	// Key : PopupUI Prefab, value : instance
	private Dictionary<MoonPopupUI, MoonPopupUI> 			m_AllPopups = new Dictionary<MoonPopupUI, MoonPopupUI>();
	private List<MoonPopupUI>								m_OpenedPopupLists = new List<MoonPopupUI>();

	// Key : Instance
	private Dictionary<MoonPopupUI, Image> 					m_CurrentBackGroundImage = new Dictionary<MoonPopupUI, Image>();

	public RectTransform 									rectTransformRef { get { return m_RectTransformRef; } }
	private RectTransform 									m_RectTransformRef;


	[SerializeField] private int 							m_depth = 0;
	[SerializeField] private List<Image> 					m_BackgroundImages = new List<Image>();


	protected virtual void Awake()
	{
		m_RectTransformRef = transform as RectTransform;

		var enumerator = m_BackgroundImages.GetEnumerator();
		while (enumerator.MoveNext()) 
		{
			enumerator.Current.gameObject.SetActive(false);
		}

		if(!m_PopupManager.ContainsKey(m_depth))
		{
			m_PopupManager[m_depth] = this;			
		}
	}

	public T ShowPopup<T>(T popupPrefab, params object[] args) where T : MoonPopupUI
	{
		T popup = MakePopup(popupPrefab);
		m_OpenedPopupLists.Add(popup);

		Image backGroundImage = MakeBackgroundImage();
		m_CurrentBackGroundImage[popup] = backGroundImage;

		backGroundImage.rectTransform.SetAsLastSibling();
		popup.rectTransformRef.SetAsLastSibling();

		popup.ShowPopupByManager(OnHidePopup, args);
		return popup;
	}

	private T MakePopup<T> (T Prefab) where T : MoonPopupUI
	{
		if (m_AllPopups.ContainsKey(Prefab))
			return m_AllPopups[Prefab] as T;
		
		m_AllPopups[Prefab] = Moon.Util.MakeItem(Prefab, transform, false);

		return m_AllPopups[Prefab] as T;
	}

	public static MoonPopupManager GetPopupManager(int depth)
	{
		if (m_PopupManager.ContainsKey(depth)) 
		{
			return m_PopupManager[depth];
		}
			
		return null;
	}

	protected virtual void OnHidePopup(MoonPopupUI popup)
	{
		m_OpenedPopupLists.Remove(popup);

		if (m_CurrentBackGroundImage.ContainsKey(popup)) 
		{
			m_CurrentBackGroundImage[popup].gameObject.SetActive(false);
			m_CurrentBackGroundImage.Remove(popup);
		}
	}

	private Image MakeBackgroundImage()
	{
		if (m_BackgroundImages.Count <= 0)
		{
			Debug.LogError(Util.LogFormat("Not found background image object"));
			return null;
		}

		var enumerator = m_BackgroundImages.GetEnumerator();
		while (enumerator.MoveNext()) 
		{
			if (enumerator.Current.gameObject.activeSelf == false) 
			{
				enumerator.Current.gameObject.SetActive(true);

				return enumerator.Current;
			}
		}

		Image backGroundImage = Moon.Util.MakeItem(m_BackgroundImages[0], transform, false);
		backGroundImage.gameObject.SetActive(true);
		m_BackgroundImages.Add(backGroundImage);

		return backGroundImage;
	}

	public void HideLastPopup()
	{
		if (m_OpenedPopupLists.Count <= 0)
			return;

		MoonPopupUI popUp = m_OpenedPopupLists[m_OpenedPopupLists.Count - 1];
		popUp.Hide();
	}
}

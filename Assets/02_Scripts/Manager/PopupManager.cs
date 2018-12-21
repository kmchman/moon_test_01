using UnityEngine;

using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
#region Static
	public static int count { get { return DepthList.Count; } }
	public static bool isPauseEventAction { get { return PauseEventActionPopups.Count > 0; } }

	// Key : Depth
	private static Dictionary<int, PopupManager> Managers = new Dictionary<int, PopupManager>();
	private static List<int> DepthList = new List<int>();

	private static HashSet<PopupUI> PauseEventActionPopups = new HashSet<PopupUI>();
#endregion
	[SerializeField] private Color m_BackgroundColor = new Color(0f, 0f, 0f, 0.5f);
	[SerializeField] private List<STImage> m_BackgroundImages = new List<STImage>();
	[SerializeField] protected int m_Depth;

	public RectTransform rectTransformRef { get { return m_RectTransformRef; } }

	// Key : PopupUI Prefab
	private Dictionary<PopupUI, PopupUI> m_AllPopups = new Dictionary<PopupUI, PopupUI>();
	private List<PopupUI> m_CurrentPopupList = new List<PopupUI>();

	// Key : PopupUI Instance
	private Dictionary<PopupUI, STImage> m_CurrentBackgroundImages = new Dictionary<PopupUI, STImage>();

	private RectTransform m_RectTransformRef;

	protected virtual void Awake()
	{
		if(Managers.ContainsKey(m_Depth))
			Debug.LogError(Util.LogFormat("Already exist depth", m_Depth, name));

		Managers[m_Depth] = this;

		DepthList.Clear();

		var enumerator = Managers.GetEnumerator();
		while (enumerator.MoveNext())
			DepthList.Add(enumerator.Current.Key);
		
		DepthList.Sort();

		m_RectTransformRef = transform as RectTransform;
	}

	protected virtual void OnDestroy()
	{
		DepthList.Remove(m_Depth);
		Managers.Remove(m_Depth);

		m_AllPopups.Clear();
		m_CurrentPopupList.Clear();
	}

	public static PopupManager ManagerAt(int index)
	{
		if (index >= DepthList.Count)
		{
			Debug.LogError(Util.LogFormat("Out of bounds", index));
			return null;
		}

		int depth = DepthList[index];
		if (!Managers.ContainsKey(depth))
		{
			Debug.LogError(Util.LogFormat("Not found manager", depth));
			return null;
		}

		return Managers[depth];
	}

	public static List<PopupUI> GetShowPopupsByAll()
	{
		List<PopupUI> popupList = new List<PopupUI>();
		GetShowPopupsByAll(popupList);
		return popupList;
	}

	private static List<PopupUI> TempPopupUIList = new List<PopupUI>();
	public static void GetShowPopupsByAll(List<PopupUI> popupList)
	{
		if (popupList == null)
			popupList = new List<PopupUI>();

		popupList.Clear();

		var enumerator = Managers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Value.GetShowPopups(TempPopupUIList);
			popupList.AddRange(TempPopupUIList);
		}
	}

	public static PopupUI GetPopupByAll(string alias)
	{
		PopupUI popup = null;

		var enumerator = Managers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			popup = enumerator.Current.Value.GetPopup(alias);
			if (popup != null)
				break;
		}

		return popup;
	}

	public List<PopupUI> GetShowPopups()
	{
		List<PopupUI> popupList = new List<PopupUI>();
		GetShowPopups(popupList);
		return popupList;
	}

	public void GetShowPopups(List<PopupUI> popupList)
	{
		if (popupList == null)
			popupList = new List<PopupUI>();
		
		popupList.Clear();
		popupList.AddRange(m_CurrentPopupList);
	}

	public PopupUI GetPopup(string alias)
	{
		for (int i = 0; i < m_CurrentPopupList.Count; ++i)
			if (m_CurrentPopupList[i].alias == alias)
				return m_CurrentPopupList[i];
		return null;
	}

	public PopupType GetPopup<PopupType>(PopupType popupPrefab) where PopupType : PopupUI
	{
		if(!m_AllPopups.ContainsKey(popupPrefab) || !m_AllPopups[popupPrefab].isShow)
			return null;

		return (PopupType)m_AllPopups[popupPrefab];
	}

	public PopupType ReadyPopup<PopupType>(PopupType popupPrefab) where PopupType : PopupUI
	{
		PopupType popup = MakePopup(popupPrefab);
		popup.gameObject.SetActive(false);

		return popup;
	}

	public PopupType ShowPopup<PopupType>(PopupType popupPrefab, params object[] args) where PopupType : PopupUI
	{
		PopupType popup = MakePopup(popupPrefab);
		m_CurrentPopupList.Add(popup);

		if (popup.isUseDefaultBackground || popup.isAutoHideClickDefaultBackground)
		{
			STImage backgroundImage = MakeBackgroundImage();
			backgroundImage.rectTransform.SetAsLastSibling();

			backgroundImage.color = m_BackgroundColor;
			if (!popup.isUseDefaultBackground)
				backgroundImage.color = new Color(0, 0, 0, 0);

			m_CurrentBackgroundImages[popup] = backgroundImage;
		}

		popup.rectTransformRef.SetAsLastSibling();
		popup.ShowPopupByManager(OnHidePopup, args);

		if (popup.isPauseEventAction)
			PauseEventActionPopups.Add(popup);
		
		return popup;
	}

	public void HidePopup<PopupType>(PopupType popupPrefab, params object[] args) where PopupType : PopupUI
	{
		if(!m_AllPopups.ContainsKey(popupPrefab) || !m_AllPopups[popupPrefab].isShow)
			return;
		
		m_AllPopups[popupPrefab].HidePopupByManager(args);
	}
	
	public void HidePopup(string alias, params object[] args)
	{
		PopupUI popup = GetPopup(alias);
		if (popup == null)
			return;
		
		popup.Hide(args);
	}

	public void HideAllPopup()
	{
		for(int i = m_CurrentPopupList.Count - 1; i >= 0; i--)
			m_CurrentPopupList[i].HidePopupByManager();
	}

	public bool HideLastPopupByBackButtonManager()
	{
		if (m_CurrentPopupList.Count <= 0)
			return false;		

		for (int i = m_CurrentPopupList.Count - 1; i >= 0; i--)
		{
			PopupUI popup = m_CurrentPopupList[i];

			if (popup.isIgnoreHidePopupByBackButtonManager)
				continue;
			
			if (!popup.WillHidePopupByBackButtonManager())
				return true;

			popup.HidePopupByBackButtonManager();
			return true;
		}

		return false;
	}
		
	public bool IsShowPopupByAliasName(string popupAlias)
	{
		PopupUI tempPopup = GetPopup(popupAlias);
		if(tempPopup == null)
			return false;
		return tempPopup.isShow;
	}

	protected virtual void OnHidePopup(PopupUI popup)
	{
		m_CurrentPopupList.Remove(popup);

		if (m_CurrentBackgroundImages.ContainsKey(popup))
		{
			m_CurrentBackgroundImages[popup].gameObject.SetActive(false);
			m_CurrentBackgroundImages.Remove(popup);
		}

		if (popup.isDestroyAtHide)
		{
			PopupUI key = null;

			var enumerator =  m_AllPopups.GetEnumerator();
			while(enumerator.MoveNext())
				if(enumerator.Current.Value == popup)
					key = enumerator.Current.Key;
			
			if(key != null)
				m_AllPopups.Remove(key);

			GameObject.Destroy(popup.gameObject);
		}

		if (PauseEventActionPopups.Contains(popup))
			PauseEventActionPopups.Remove(popup);
	}

	public virtual void OnClickDefaultBackground(STImage backgroundImage)
	{
		PopupUI popup = null;
		var enumerator = m_CurrentBackgroundImages.GetEnumerator();

		while (enumerator.MoveNext())
		{
			if (enumerator.Current.Value == backgroundImage)
			{
				popup = enumerator.Current.Key;
				break;
			}
		}

		if (popup != null)
			popup.OnClickDefaultBackgroundImage();
	}

	protected virtual PopupType MakePopup<PopupType>(PopupType prefab) where PopupType : PopupUI
	{
		if (m_AllPopups.ContainsKey(prefab))
			return m_AllPopups[prefab] as PopupType;

		PopupType popup = Giant.Util.MakeItem(prefab, m_RectTransformRef, false);

		if (!popup.isDestroyAtHide)
			m_AllPopups[prefab] = popup;

		return popup;
	}

	protected virtual STImage MakeBackgroundImage()
	{
		if (m_BackgroundImages.Count <= 0)
		{
			Debug.LogError(Util.LogFormat("Not found background image object"));
			return null;
		}

		for (int i = 0; i < m_BackgroundImages.Count; ++i)
		{
			if (!m_BackgroundImages[i].gameObject.activeSelf)
			{
				m_BackgroundImages[i].gameObject.SetActive(true);
				return m_BackgroundImages[i];
			}
		}

		STImage backgroundImage = Giant.Util.MakeItem(m_BackgroundImages[0], m_RectTransformRef, false);
		backgroundImage.gameObject.SetActive(true);
		m_BackgroundImages.Add(backgroundImage);

		return backgroundImage;
	}
}
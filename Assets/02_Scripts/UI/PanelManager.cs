using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using STExtensions;

public class PanelManager : MonoBehaviour
{
	[Serializable]
	public class PanelData
	{
		public PanelType type;
		public GameObject panelPrefab;
		public Panel panel;

		public object[] panelValues;

		public override string ToString()
		{
			return Util.StringFormat("Type:{0}, Prefab:{1}, Object:{2}", type, panelPrefab, panel);
		}
	}

	public static PanelManager inst { get { return m_inst; } }
	private static PanelManager m_inst = null;

	public static bool IsShowCurrentPanel(PanelType type)
	{
		if(m_inst == null)
			return false;

		return m_inst.currentPanelData.type == type;
	}

	[SerializeField] private PopupManager m_PopupManager;
	[SerializeField] private List<PanelData> m_PanelDataList = new List<PanelData>();

	public PanelData currentPanelData { get {
		if (m_PanelStack.Count <= 0)
			return null;
		return m_PanelStack.Peek();
	} }

	public bool isRunAnimation { get { return m_PlayAnimationList.Count > 0; } }
	public bool isShowingPanel { get { return m_ShowingPanelData != null; } }
	public bool isHidingPanel { get { return m_HidingPanelData != null; } }

	public Action<PanelData> willShowPanelAction;
	public Action<PanelData> didShowPanelAction;
	public Action<PanelData> willHidePanelAction;
	public Action<PanelData> didHidePanelAction;

	private Dictionary<PanelType, PanelData> m_PanelDataDictinary = new Dictionary<PanelType, PanelData>();

	private Stack<PanelData> m_PanelStack = new Stack<PanelData>();

	private List<GameObject> m_PlayAnimationList = new List<GameObject>();

	private PanelData m_ShowingPanelData;
	private PanelData m_HidingPanelData;

	private Transform m_transformRef;

	private void Awake()
	{
		m_inst = this;

		m_transformRef = transform;

		MakePanelPrefabData();
	}

	private void OnDestroy()
	{
		m_inst = null;
	}

	public void PushPanel(PanelType panelType, bool isClear = false, params object[] values)
	{
		PanelData tempCurrentPanelData = currentPanelData;
		if (tempCurrentPanelData != null && tempCurrentPanelData.type == panelType)
			return;

		PanelData panelData = MakePanel(panelType);
		if (panelData == null || panelData.panel == null)
			return;

		panelData.panelValues = values;

		if (m_PanelStack.Count > 0)
			HidePanel(m_PanelStack.Peek(), Panel.Direction.Forward, () => { if (isClear) m_PanelStack.Clear(); ShowPanel(panelData, Panel.Direction.Forward, null); });
		else
			ShowPanel(panelData, Panel.Direction.Forward, null);
	}

	public void PopPanel()
	{
		PopPanel(1);
	}

	public void PopPanel(int count)
	{
		if (m_PanelStack.Count <= 1)
			return;

//		TooltipManager.Inst.HideAllToolTip();

		// HidePanel에서 현재 패널의 정보를 사용하는 경우를 대비해서 미리 Pop하지 않고 Peek로 처리한다.
		// Ex) HidePanel에서 SavePanelData 함수 사용
		HidePanel(m_PanelStack.Peek(), Panel.Direction.Backward, () =>
		{
			while (count-- > 0 && m_PanelStack.Count > 1)
				m_PanelStack.Pop();
			
			ShowPanel(m_PanelStack.Pop(), Panel.Direction.Backward, null);
		});
	}

	public void ClearPanel()
	{
		PopPanel(m_PanelStack.Count);
	}

	public PopupType GetPopup<PopupType>(PopupType popupPrefab) where PopupType : PopupUI
	{
//		return m_PopupManager.GetPopup(popupPrefab);
		return null;
	}

	public PopupType ReadyPopup<PopupType>(PopupType popupPrefab) where PopupType : PopupUI
	{
//		return m_PopupManager.ReadyPopup(popupPrefab);
		return null;
	}

	public PopupType ShowPopup<PopupType>(PopupType popupPrefab, params object[] values) where PopupType : PopupUI
	{
//		m_PopupManager.rectTransformRef.SetAsLastSibling();
//		return m_PopupManager.ShowPopup(popupPrefab, values);
		return null;
	}

	public void HidePopup<PopupType>(PopupType popupPrefab, params object[] values) where PopupType : PopupUI
	{
//		m_PopupManager.HidePopup(popupPrefab, values);
	}

	public void SavePanelData(params object[] values)
	{
		if (m_PanelStack.Count <= 0)
			return;

		m_PanelStack.Peek().panelValues = values;
	}

	public bool OnCallByBackButtonManager()
	{
		PanelData panelData = currentPanelData;
		if (panelData == null)
			return false;

		if (panelData.panel.OnCallByBackButtonManager())
			return true;

		PopPanel();
		return true;
	}

	public void SetPlayAnimationObject(GameObject animationObject)
	{
		m_PlayAnimationList.Add(animationObject);
	}

	public void ResetPlayAnimationObject(GameObject animationObject)
	{
		if(m_PlayAnimationList.Contains(animationObject))
			m_PlayAnimationList.Remove(animationObject);
	}

	private void ShowPanel(PanelData panelData, Panel.Direction direction, Action completeShowPanel)
	{
#if UNITY_EDITOR
		CheckCompleteShowAnimationError();
#endif

		m_PanelStack.Push(panelData);

		if (m_ShowingPanelData != null)
		{
			Debug.LogError(Util.LogFormat("Already Showing Panel", m_ShowingPanelData));
			return;
		}

		m_ShowingPanelData = panelData;

		InputBlocker.inst.SetBlockByPanel();
		m_ShowingPanelData.panel.gameObject.SetActive(true);
		m_ShowingPanelData.panel.SetDirection(direction);

		CallWillShowPanelAction(m_ShowingPanelData);

		m_ShowingPanelData.panel.ShowPanelByManager((showPanel) =>
		{
			InputBlocker.inst.ResetBlockByPanel();

			OnCompleteShowPanelAnimation(showPanel);

			if (completeShowPanel != null)
				completeShowPanel();
		}, m_ShowingPanelData.panelValues);
	}

	private void HidePanel(PanelData panelData, Panel.Direction direction, Action completeHidePanel)
	{
#if UNITY_EDITOR
		CheckCompleteHideAnimationError();
#endif

		if (m_HidingPanelData != null)
		{
			Debug.LogError(Util.LogFormat("Already Hiding Panel", m_HidingPanelData));
			return;
		}

		m_HidingPanelData = panelData;

		InputBlocker.inst.SetBlockByPanel();
		m_HidingPanelData.panel.SetDirection(direction);

		CallWillHidePanelAction(m_HidingPanelData);

//		m_PopupManager.HideAllPopup();

		m_HidingPanelData.panel.HidePanelByManager((hidePanel) =>
		{
			InputBlocker.inst.ResetBlockByPanel();

			OnCompleteHidePanelAnimation(hidePanel);

			if (completeHidePanel != null)
				completeHidePanel();
		}, m_HidingPanelData.panelValues);
	}

	private void OnCompleteShowPanelAnimation(Panel panel)
	{
		if (m_ShowingPanelData.panel != panel)
			Debug.LogError(Util.LogFormat("Diffrent Panel", m_ShowingPanelData, panel));

		CallDidShowPanelAction(m_ShowingPanelData);

		m_ShowingPanelData = null;

#if UNITY_EDITOR
		CheckCompleteShowAnimationError();
#endif
	}
		
	private void OnCompleteHidePanelAnimation(Panel panel)
	{
		if (m_HidingPanelData.panel != panel)
			Debug.LogError(Util.LogFormat("Diffrent Panel", m_HidingPanelData, panel));

		CallDidHidePanelAction(m_HidingPanelData);

		if (m_HidingPanelData.panel.isDestroyAtHide)
			DestroyPanel(m_HidingPanelData);
		else
			m_HidingPanelData.panel.gameObject.SetActive(false);
		m_HidingPanelData = null;

#if UNITY_EDITOR
		CheckCompleteHideAnimationError();
#endif
	}

	private void CallWillShowPanelAction(PanelData panelData)
	{
		if (willShowPanelAction != null)
			willShowPanelAction(panelData);
	}

	private void CallDidShowPanelAction(PanelData panelData)
	{
		if (didShowPanelAction != null)
			didShowPanelAction(panelData);
	}

	private void CallWillHidePanelAction(PanelData panelData)
	{
		if (willHidePanelAction != null)
			willHidePanelAction(panelData);
	}

	private void CallDidHidePanelAction(PanelData panelData)
	{
		if (didHidePanelAction != null)
			didHidePanelAction(panelData);
	}

	private void MakePanelPrefabData()
	{
		m_PanelDataDictinary.Clear();
		for (int i = 0; i < m_PanelDataList.Count; ++i)
		{
			if (m_PanelDataList[i].panelPrefab == null && m_PanelDataList[i].panel == null)
			{
				Debug.LogError(Util.LogFormat("Panel Data is NULL", m_PanelDataList[i].type));
				continue;
			}
				
			m_PanelDataDictinary[m_PanelDataList[i].type] = m_PanelDataList[i];
		}
	}

	private PanelData MakePanel(PanelType panelType)
	{
		if (!m_PanelDataDictinary.ContainsKey(panelType))
		{
			Debug.LogError(Util.LogFormat("Not Found Panel Data", panelType));
			return null;
		}

		PanelData panelData = m_PanelDataDictinary[panelType];

//		if (panelData.panel == null)
//		{
//			Panel panel = Giant.Util.MakeItem<Panel>(panelData.panelPrefab, m_transformRef, false);
//			panel.SetPanelManager(this);
//
//			panelData.panel = panel;
//		}

		return panelData;
	}

	private void DestroyPanel(PanelData panelData)
	{
		if (panelData == null)
		{
			Debug.LogError(Util.LogFormat("PanelData is null"));
			return;
		}

		if (panelData.panel != null)
		{
			GameObject.Destroy(panelData.panel.gameObject);
			panelData.panel = null;
		}
	}

#if UNITY_EDITOR
	private bool m_CheckCompleteShowAnimationError = false;
	private bool m_CheckCompleteHideAnimationError = false;

	private void CheckCompleteShowAnimationError()
	{
		if (m_CheckCompleteShowAnimationError)
		{
			CancelInvoke("PrintCompleteShowAnimationError");
			m_CheckCompleteShowAnimationError = false;
		}
		else
		{
			Invoke("PrintCompleteShowAnimationError", 3);
			m_CheckCompleteShowAnimationError = true;
		}
	}

	private void CheckCompleteHideAnimationError()
	{
		if (m_CheckCompleteHideAnimationError)
		{
			CancelInvoke("PrintCompleteHideAnimationError");
			m_CheckCompleteHideAnimationError = false;
		}
		else
		{
			Invoke("PrintCompleteHideAnimationError", 3);
			m_CheckCompleteHideAnimationError = true;
		}
	}

	private void PrintCompleteShowAnimationError()
	{
		Debug.LogError("CompleteShowAnimation 함수가 호출되지 않음");
	}

	private void PrintCompleteHideAnimationError()
	{
		Debug.LogError("CompleteHideAnimation 함수가 호출되지 않음");
	}
#endif
}
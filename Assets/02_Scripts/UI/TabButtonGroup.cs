using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TabButtonGroup : MonoBehaviour {
	 
	[SerializeField] protected List<TabItem> 	m_TabItems; 
	[SerializeField] private SpriteSet 			m_StateSpriteSet;

	[SerializeField] protected ColorType 		m_NormalColorType 	= ColorType.Disable;
	[SerializeField] protected ColorType 		m_SelectColorType	= ColorType.Normal;
	[SerializeField] protected ColorType 		m_DisableColorType 	= ColorType.Disable;

	public int buttonCount {
		get {
			if(m_TabItems == null)
				return 0;
			return m_TabItems.Count;
		}
	}

	private System.Action<int> m_OnTouchCB = null;
	protected int m_SelectIndex = -1;
	public int SelectedIndex		 { get { return m_SelectIndex;}}

	protected virtual void Awake()
	{
		for(int i = 0; i < m_TabItems.Count; i++)
		{
			m_TabItems[i].SetData(OnTouchTabButton, i, m_NormalColorType, m_SelectColorType, m_DisableColorType);
		}
	}
		
	public TabButtonGroup Init(System.Action<int> onTouchCB)
	{
		m_OnTouchCB = onTouchCB;
		InitTabButtonIndex();
		return this;
	}

	public void OnTouchTabButton(int index)
	{
		if(index == m_SelectIndex)
			return;
		UpdateSelectTabButtonUI(index);
		if(m_OnTouchCB != null)
			m_OnTouchCB(m_SelectIndex);
	}

	public virtual void UpdateSelectTabButtonUI(int index)
	{
		SetIndexTabState(m_SelectIndex, TabButtonState.Normal);
		m_SelectIndex = index;
		SetIndexTabState(m_SelectIndex, TabButtonState.Select);
	}

	public void SetDisable(int index)
	{
		SetIndexTabState(index, TabButtonState.Disable);
	}

	private bool IsEnableIndex(int index)
	{
		return -1 < index && m_TabItems.Count > index;
	}

	private void InitTabButtonIndex()
	{
		for(int i = 0; i < m_TabItems.Count; i++)
		{
			m_TabItems[i].SetState(TabButtonState.Normal, GetStateSprite(TabButtonState.Normal));
		}
		m_SelectIndex = -1;
	}

	protected Sprite GetStateSprite(TabButtonState state)
	{
		if(m_StateSpriteSet == null)
			return null;
		
		switch(state)
		{
		case TabButtonState.Normal:
			return m_StateSpriteSet.GetSprite(SpriteType.Tab_Unselect);
		case TabButtonState.Select:
			return m_StateSpriteSet.GetSprite(SpriteType.Tab_Select);
		case TabButtonState.Disable:
			return m_StateSpriteSet.GetSprite(SpriteType.Tab_Disable);
		}
		return null;
	}

	protected void SetIndexTabState(int index, TabButtonState state)
	{
		if (IsEnableIndex(index)) 
		{
			if(state == TabButtonState.Disable)
				m_TabItems[index].SetState(state, null);
			else
				m_TabItems[index].SetState(state, GetStateSprite(state));
		}
	}
}

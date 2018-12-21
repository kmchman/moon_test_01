using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using STExtensions;

public enum TabButtonState
{
	Normal = 0,
	Select = 1,
	Disable = 2,
}

public class TabItem : STScrollRectItem {
	[SerializeField] protected STText			m_TitleText;
	[SerializeField] private STImage			m_SelectImage;
	[SerializeField] private STButton 			m_ThisButton;
	[SerializeField] private STGrayScaleGroup	m_GrayScaleGroup;

	[SerializeField] private GameObject			m_NormalObject;
	[SerializeField] private GameObject			m_SelectObject;

	private int									m_Index;
	public int 									Index{ get{ return m_Index;}}
	private System.Action<int> 					m_OnTouchCB = null;

	private ColorType 							m_NormalColorType;
	private ColorType 							m_SelectColorType;
	private ColorType 							m_DisableColorType;
	private Color								m_OrgTextColor;

	private UIBaseLvUnlockContentsLinker		m_BaseLvUnlockContentsLinker;

	protected override void Awake()
	{
		base.Awake();
		m_BaseLvUnlockContentsLinker = GetComponent<UIBaseLvUnlockContentsLinker>();

		m_ThisButton.onClick.AddListener(()=>
		{
			if(m_OnTouchCB != null)
				m_OnTouchCB(m_Index);
		});
		if(m_TitleText)
			m_OrgTextColor = m_TitleText.color;
	}

	public void SetData(System.Action<int> onTouchCB, int index, ColorType normarl, ColorType select, ColorType disable)
	{
		m_Index = index;
		m_OnTouchCB = onTouchCB;

		m_NormalColorType = normarl;
		m_SelectColorType = select;
		m_DisableColorType = disable;
	}

	public void SetState(TabButtonState state, Sprite stateSprite)
	{
		// TODO : 임시처리. 특정 TabButton을 정지모드로 만들어서 풀기전까지 상태변환이 안되는 상태를 만들것.
		if (m_BaseLvUnlockContentsLinker != null && !m_BaseLvUnlockContentsLinker.isUnlock)
			return;

		if(m_GrayScaleGroup)
			m_GrayScaleGroup.SetActive(false);
		
		if(m_ThisButton)
			m_ThisButton.enabled = true;
		
		if(m_SelectObject)
			m_SelectObject.SetActive(false);
		
		if(m_NormalObject)
			m_NormalObject.SetActive(false);

		switch (state) 
		{
		case TabButtonState.Normal:
			{
				if(m_NormalObject)
					m_NormalObject.SetActive(true);

				if (m_TitleText)
				{
					if (m_NormalColorType == ColorType.None)
					{
						m_TitleText.color = m_OrgTextColor;
					}
					else
					{
						m_TitleText.color = GlobalDataStore.Inst.GetGameColor(m_NormalColorType);					
					}
				}
			}
			break;
		case TabButtonState.Select:
			{
				if(m_SelectObject)
					m_SelectObject.SetActive(true);	
				if (m_TitleText && m_SelectColorType != ColorType.None)
					m_TitleText.color = GlobalDataStore.Inst.GetGameColor(m_SelectColorType);
			}
			break;
		case TabButtonState.Disable:
			{
				if(m_GrayScaleGroup)
					m_GrayScaleGroup.SetActive(true);
				if(m_ThisButton)
					m_ThisButton.enabled = false;
				if (m_TitleText && m_DisableColorType != ColorType.None)
					m_TitleText.color = GlobalDataStore.Inst.GetGameColor(m_DisableColorType);
			}
			break;
		}

		if(m_SelectImage == null)
			return;

		if (state != TabButtonState.Disable) 
		{
			if(stateSprite != null)
			{
				m_SelectImage.gameObject.SetActive(true);
				m_SelectImage.sprite = stateSprite;
			}
			else
			{
				m_SelectImage.gameObject.SetActive(false);
			}
		}
	}
}
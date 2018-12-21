using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using STExtensions;

public class STSlider : Slider
{	
	[SerializeField] private ColorType 		m_ColorDisabled = ColorType.Disable;
	[SerializeField] private Sprite 		m_ImageHandleDisable;

	private Color 							m_ColorOrgFill;
	private Color 							m_ColorOrgHandle;
	private Image 							m_ImageFill;
	private Image 							m_ImageHandle;
	private Sprite 							m_SpriteHandleEnable;

	protected override void Awake()
	{
		base.Awake();
		m_ImageFill 				= fillRect.GetComponent<Image>();
		m_ImageHandle 				= handleRect.GetComponent<Image>();

		m_ColorOrgFill		 		= m_ImageFill.color;
		m_ColorOrgHandle 			= m_ImageHandle.color;
		m_SpriteHandleEnable 		= m_ImageHandle.sprite;
	}

	public void SetEnable(bool value)
	{
		if (value) 
		{
			m_ImageFill.color		= m_ColorOrgFill;
			m_ImageHandle.color 	= m_ColorOrgHandle;
			m_ImageHandle.sprite	= m_SpriteHandleEnable;
//			m_ImageHandle.SetGrayScale(false);
		} 
		else 
		{
			m_ImageFill.color		= GlobalDataStore.Inst.GetGameColor(m_ColorDisabled);
			m_ImageHandle.color		= GlobalDataStore.Inst.GetGameColor(m_ColorDisabled);
			m_ImageHandle.sprite	= m_ImageHandleDisable;
//			m_ImageHandle.SetGrayScale(true);

		}			
	}
}

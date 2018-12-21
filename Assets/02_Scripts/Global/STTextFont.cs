using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using System.Reflection;

using STExtensions;

public class STTextFont : MonoBehaviour
{
	[SerializeField] private Font m_DefaultFont;
	[SerializeField] private STFontSetting m_DefaultFontSetting;

	private Font m_Font = null;

	private float m_FontSizeFactor = 1;
	private float m_LineSpaceFactor = 1;

	public Font font
	{
		get
		{
			if (m_DefaultFont != null)
				return m_DefaultFont;
			return m_Font;
		}
	}

	public float fontSizeFactor
	{
		get
		{
			if (m_DefaultFontSetting != null)
				return m_DefaultFontSetting.fontSizeFactor;
			return m_FontSizeFactor;
		}
	}

	public float lineSpaceFactor
	{
		get
		{
			if (m_DefaultFontSetting != null)
				return m_DefaultFontSetting.lineSpaceFactor;
			return m_LineSpaceFactor;
		}
	}

	public void Clear()
	{
		m_Font = null;
		m_FontSizeFactor = m_LineSpaceFactor = 1;
	}

	public void SetFont(Font newFont)
	{
		if (newFont == null)
			return;

		m_Font = newFont;
	}

	public void SetFactor(float fontSizeFactor, float lineSpaceFactor)
	{
		m_FontSizeFactor = fontSizeFactor;
		m_LineSpaceFactor = lineSpaceFactor;
	}
}
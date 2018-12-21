using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class STFontSetting : ScriptableObject
{
	[SerializeField] private float m_FontSizeFactor = 1;
	[SerializeField] private float m_LineSpaceFactor = 1;

	public float fontSizeFactor { get { return m_FontSizeFactor; } }
	public float lineSpaceFactor { get { return m_LineSpaceFactor; } }
}

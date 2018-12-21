using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public class STTextStyler : MonoBehaviour
{
	[SerializeField] private STTextFont m_Font;
	[SerializeField] private int m_FontSize = 14;
	[SerializeField] private float m_LineSpacing = 1;

	[SerializeField] private STMeshEffectData m_Shadow = new STMeshEffectData();
	[SerializeField] private STMeshEffectData m_Outline = new STMeshEffectData();

	public Font font { get { return m_Font.font; } }
	public int fontSize { get { float value = m_FontSize * m_Font.fontSizeFactor; return (int)value; } }
	public float lineSpacing { get { float value = m_LineSpacing * m_Font.lineSpaceFactor; return value; } }
	public STMeshEffectData shadow { get { return m_Shadow; } }
	public STMeshEffectData outline { get { return m_Outline; } }
}
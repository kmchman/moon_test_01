using UnityEngine;

using System;
using System.Collections;

[Serializable]
public class STMeshEffectData
{
	[SerializeField] private bool m_isUse = false;
	[SerializeField] private Color m_color = new Color(0f, 0f, 0f, 1f);
	[SerializeField] private Vector2 m_distance = new Vector2(2f, -2f);
	[SerializeField] private bool m_isUseGraphicAlpha = true;

	public bool isUse
	{
		get { return m_isUse; }
		set { m_isUse = value; }
	}

	public Color color
	{
		get { return m_color; }
		set { m_color = value; }
	}

	public Vector2 distance
	{
		get { return m_distance; }
		set { m_distance = value; }
	}

	public bool isUseGraphicAlpha
	{
		get { return m_isUseGraphicAlpha; }
		set { m_isUseGraphicAlpha = value; }
	}
}

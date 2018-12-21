using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public class STImage : Image
{
	[SerializeField] private ColorType m_ColorType = ColorType.None;

	public ColorType colorType
	{
		get { return m_ColorType; }
		set { SetColorType(value); }
	}

	protected override void Awake()
	{
		base.Awake();

		ApplyColorType(m_ColorType);
	}

	public override void SetVerticesDirty()
	{
		base.SetVerticesDirty();
		CheckColor();
	}

	private void CheckColor()
	{
		canvasRenderer.SetAlpha(color.a <= 0 ? 0 : 1);
	}

	private void ApplyColorType(ColorType colorType)
	{
		if (GlobalDataStore.Inst == null)
			return;

		if (colorType == ColorType.None)
			return;

		color = GlobalDataStore.Inst.GetGameColor(colorType);
	}

	private void SetColorType(ColorType colorType)
	{
		if (m_ColorType == colorType)
			return;

		m_ColorType = colorType;
		ApplyColorType(m_ColorType);
	}
}
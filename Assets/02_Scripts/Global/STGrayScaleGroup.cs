using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using STExtensions;

public class STGrayScaleGroup : MonoBehaviour
{
	[SerializeField] private bool m_IsActive;

	[SerializeField] private bool m_IncludeChildren;

	[UnityEngine.Serialization.FormerlySerializedAs("m_GrayScaleImageList")]
	[SerializeField] private List<Graphic> m_GrayScaleList;

	[SerializeField] private List<STButton> m_ButtonList;

	public bool active { get { return m_IsActive; } }

	private void Awake()
	{
		if (m_IncludeChildren)
		{
			m_GrayScaleList.AddRange(transform.GetComponentsInChildren<Image>(true));
			m_GrayScaleList.AddRange(transform.GetComponentsInChildren<RawImage>(true));
			m_GrayScaleList.AddRange(transform.GetComponentsInChildren<STText>(true));
		}

		UpdateActive();
	}

	public void SetActive(bool isActive)
	{
		if (m_IsActive == isActive)
			return;

		m_IsActive = isActive;

		UpdateActive();
	}

	private void UpdateActive()
	{
		for (int i = 0; i < m_GrayScaleList.Count; ++i)
		{
			if (m_GrayScaleList[i] is Image)
				UpdateActiveToImage(m_GrayScaleList[i] as Image);
			else if (m_GrayScaleList[i] is RawImage)
				UpdateActiveToImage(m_GrayScaleList[i] as RawImage);
			else if (m_GrayScaleList[i] is STText)
				UpdateActiveToSTText(m_GrayScaleList[i] as STText);
		}
		for (int i = 0; i < m_ButtonList.Count; ++i)
			UpdateActiveToButton(m_ButtonList[i]);
	}

	private void UpdateActiveToImage(Image image)
	{
//		image.SetGrayScale(m_IsActive);
	}

	private void UpdateActiveToImage(RawImage rawImage)
	{
//		rawImage.SetGrayScale(m_IsActive);
	}
	
	private void UpdateActiveToSTText(STText text)
	{
		text.SetGrayScale(m_IsActive);
	}

	private void UpdateActiveToButton(STButton button)
	{
		button.image.SetGrayScaleAlpha(m_IsActive);
	}
}

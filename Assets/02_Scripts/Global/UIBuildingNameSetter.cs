using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIBuildingNameSetter : MonoBehaviour
{
	[SerializeField] private BUILDING m_IDBuilding;

	private void Start()
	{
		SetText();
	}

	private void SetText()
	{
//		Text uiText = GetComponent<Text>();
//		if (uiText != null)
//			uiText.text = Datatable.Inst.GetBuildingName((int)m_IDBuilding);
	}
}

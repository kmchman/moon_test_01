using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI : MonoBehaviour {

	[SerializeField] private HerosPopup 				m_HerosPopupPrefab;

	public void OnClickBtnHero()
	{
		GlobalPopupManager.Inst.ShowPopup(m_HerosPopupPrefab);	
	}
}

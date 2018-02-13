using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerosPopup : MoonPopupUI {
	[SerializeField] private HeroListItem		m_HeroItemPrefab;
	[SerializeField] private Transform 			m_HeroListParent;

	public override void Show(params object[] values)
	{
		for (int i = 0; i < 5; i++) 
		{
			
		}
		CompleteShowAnimation();
	}

	public override void Hide(params object[] values)
	{
		for (int i = 0; i < 5; i++) 
		{

		}

		CompleteHideAnimation();
	}

}

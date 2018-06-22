using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerosPopup : MoonPopupUI {
	[SerializeField] private HeroListItem		m_HeroItemPrefab;
	[SerializeField] private Transform 			m_HeroListParent;

	public override void Show(params object[] values)
	{
		Debug.Log("public override void Show(params object[] values)");
		base.Show(values);
		var enumerator = Datatable.Inst.dtHeroData.GetEnumerator();
		while (enumerator.MoveNext()) {
			Debug.Log("hero : " + enumerator.Current.Value.Name);
		}
		CompleteShowAnimation();
	}

	public override void Hide(params object[] values)
	{
		for (int i = 0; i < 5; i++) 
		{

		}
		base.Hide(values);
		CompleteHideAnimation();
	}

}

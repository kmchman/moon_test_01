using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HerosPopup : PopupUI {

	[SerializeField] private HeroListItem		m_HeroItemPrefab;
	[SerializeField] private Transform 			m_HeroListParent;
	[SerializeField] private STScrollRect		m_ScrollRect;

	public override void Show(params object[] values)
	{
		m_ScrollRect.Clear();
		Debug.Log("public override void Show(params object[] values)");
		base.Show(values);
		var enumerator = Datatable.Inst.dtHeroData.GetEnumerator();
		while (enumerator.MoveNext()) {
			Debug.Log("hero : " + enumerator.Current.Value.Name);
			HeroListItem item = m_ScrollRect.GetReuseItem(m_HeroItemPrefab, null);
			item.SetData(enumerator.Current.Value.HeroID);
		}
		CompleteShowAnimation();
	}

	public override void Hide(params object[] values)
	{
		m_ScrollRect.Clear();
		for (int i = 0; i < 5; i++) 
		{

		}
		base.Hide(values);
		CompleteHideAnimation();
	}
}

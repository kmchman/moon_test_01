using UnityEngine;
using System.Collections;

public class TestPopup01 : MoonPopupUI {

	public override void Show(params object[] values)
	{
		base.Show(values);

	}

	public override void Hide(params object[] values)
	{
		base.Hide(values);

	}

	public void OnClickBtnClose()
	{
		Hide();
	}

}

using UnityEngine;
using System.Collections;

public class TestPopup02 : MoonPopupUI {
	
	protected override void Awake()
	{
		base.Awake();

		gameObject.SetActive (false);

	}

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

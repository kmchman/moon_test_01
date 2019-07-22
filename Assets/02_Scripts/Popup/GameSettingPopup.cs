using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameSettingPopup : MoonPopupUI {

	[SerializeField] private TestPopup01	m_TestPopupPrefab;

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

	public void OnClickBtnOpenTest01()
	{
		MoonGlobalPopupManager.Inst.ShowPopup(m_TestPopupPrefab);	
	}

}

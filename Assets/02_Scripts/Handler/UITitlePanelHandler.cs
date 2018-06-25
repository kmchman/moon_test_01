using UnityEngine;
using System.Collections;

public class UITitlePanelHandler : MonoBehaviour {
	
	public void OnClickBtn_TouchScreen()
	{
		Datatable.Inst.LoadDt();
		BaseOperatorUnit.instance.LoadLevel_Async(SceneName.Lobby);
	}
}

using UnityEngine;
using System.Collections;

public class UITitlePanelHandler : MonoBehaviour {


	public void OnClickBtn_TouchScreen()
	{
		BaseOperatorUnit.instance.LoadLevel_Async(SceneName.Lobby);
	}

}

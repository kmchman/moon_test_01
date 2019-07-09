using UnityEngine;
using System.Collections;

public class BackButtonManager : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        
		if (Input.GetKeyDown(KeyCode.Escape)) 
		{
			TouchBackButton();
		}
	}

	private void TouchBackButton()
	{
		if (InputBlocker.inst.isBlock)
			return;

		for (int i = 0; i < PopupManager.count; ++i)
		{
			PopupManager popupManager = PopupManager.ManagerAt(i);
			if (popupManager == null)
				continue;

			if (popupManager.HideLastPopupByBackButtonManager())
				return;
		}

		if (PanelManager.inst != null)
			PanelManager.inst.OnCallByBackButtonManager();
	}
}

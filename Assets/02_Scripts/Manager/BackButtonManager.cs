using UnityEngine;
using System.Collections;

public class BackButtonManager : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.Escape)) 
		{
			BackButtonTouch();
		}
	}

	private void BackButtonTouch()
	{
		MoonPopupManager popupManager = MoonPopupManager.GetPopupManager(0);
		popupManager.HideLastPopup();
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class STInputField : InputField {

	void Update()
	{
		if (m_Keyboard == null) 
			return;

		switch (m_Keyboard.status) 
		{
		case TouchScreenKeyboard.Status.Canceled:
			text = string.Empty;	
			break;
		case TouchScreenKeyboard.Status.LostFocus:
			text = string.Empty;
			break;
		case TouchScreenKeyboard.Status.Done:
			text = m_Keyboard.text;
			break;
		case TouchScreenKeyboard.Status.Visible:
			break;
		default:
			break;
		}
	}
}

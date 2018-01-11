using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LobbyMainUIItem : MonoBehaviour {

	[SerializeField] private GameSettingPopup  			m_GameSettingPrefab;				
	[SerializeField] private TestPopup01 				m_TestPopup01Prefab;
	[SerializeField] private TestPopup02 				m_TestPopup02Prefab;
	[SerializeField] private InputField 				m_TestInputField;

	private TouchScreenKeyboard 						m_ChatKeyboard;


	void Start()
	{
//		TouchScreenKeyboard.vi
	}

	public void OnClickBtn_GameSetting()
	{
		MoonGlobalPopupManager.Inst.ShowPopup(m_GameSettingPrefab);
	}

	public void OnClickBtn_Test01()
	{
		MoonGlobalPopupManager.Inst.ShowPopup(m_TestPopup01Prefab);
	}

	public void OnClickBtn_Test02()
	{
//		MoonGlobalPopupManager.Inst.ShowPopup(m_TestPopup02Prefab);
		TouchScreenKeyboard.hideInput = true;
		m_ChatKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.ASCIICapable, false, false, false, false, "xyz");
		m_ChatKeyboard.active = true;
	}
}

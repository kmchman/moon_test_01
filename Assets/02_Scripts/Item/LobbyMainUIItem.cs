using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net.NetworkInformation;

using System;
//using System.IO;

public class LobbyMainUIItem : MonoBehaviour {

	[SerializeField] private GameSettingPopup  			m_GameSettingPrefab;				
	[SerializeField] private TestPopup01 				m_TestPopup01Prefab;
	[SerializeField] private TestPopup02 				m_TestPopup02Prefab;
	[SerializeField] private HerosPopup 				m_HerosPopupPrefab;
	[SerializeField] private InputField 				m_TestInputField;

	private TouchScreenKeyboard 						m_ChatKeyboard;


	void Start()
	{
//		TouchScreenKeyboard.vi
	}

	public void OnClickBtn_GameSetting()
	{
		
	}

	public void OnClickBtn_Test01()
	{
		var enumerator = Datatable.Inst.dtCharData.GetEnumerator();
		while (enumerator.MoveNext()) {
			Debug.Log("ID : " + enumerator.Current.Key.ToString());
			Debug.Log(enumerator.Current.ToString());
		}
//		MoonGlobalPopupManager.Inst.ShowPopup(m_TestPopup01Prefab);
	}

	public void OnClickBtn_Test02()
	{
		MoonGlobalPopupManager.Inst.ShowPopup(m_HerosPopupPrefab);
	}

	public static void ShowNetworkInterfaces()
	{
		IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
		NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
		Console.WriteLine("Interface information for {0}.{1}     ",
			computerProperties.HostName, computerProperties.DomainName);
		if (nics == null || nics.Length < 1)
		{
			Console.WriteLine("  No network interfaces found.");
			return;
		}

		Console.WriteLine("  Number of interfaces .................... : {0}", nics.Length);
		foreach (NetworkInterface adapter in nics)
		{
			IPInterfaceProperties properties = adapter.GetIPProperties(); //  .GetIPInterfaceProperties();
			Console.WriteLine();
			Console.WriteLine(adapter.Description);
			Console.WriteLine(String.Empty.PadLeft(adapter.Description.Length,'='));
			Console.WriteLine("  Interface type .......................... : {0}", adapter.NetworkInterfaceType);
			Console.Write("  Physical address ........................ : ");
			PhysicalAddress address = adapter.GetPhysicalAddress();
			byte[] bytes = address.GetAddressBytes();
			for(int i = 0; i< bytes.Length; i++)
			{
				// Display the physical address in hexadecimal.
				Console.Write("{0}", bytes[i].ToString("X2"));
				// Insert a hyphen after each byte, unless we are at the end of the 
				// address.
				if (i != bytes.Length -1)
				{
					Console.Write("-");
				}
			}
			Console.WriteLine();
		}
	}
}

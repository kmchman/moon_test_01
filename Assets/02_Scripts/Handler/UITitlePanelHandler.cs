using UnityEngine;
using System.Collections;

public class UITitlePanelHandler : MonoBehaviour {
	
	public void OnClickBtn_TouchScreen()
	{
		foreach(var value in Global.dtList)
		{
			string data = (Resources.Load("DT/" + value) as TextAsset).text;
			IList dataList = (IList)Util.JsonDecode(data);

			for (int i = 1; i < dataList.Count; i++) {
				IDictionary dic = (IDictionary)dataList[i];	
				Datatable.Inst.LoadDt(dic);
			}	
		}

		BaseOperatorUnit.instance.LoadLevel_Async(SceneName.Lobby);
	}
}

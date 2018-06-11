using UnityEngine;
using System.Collections;

public class UITitlePanelHandler : MonoBehaviour {
	
	public void OnClickBtn_TouchScreen()
	{
		string data = (Resources.Load("DT/CharData") as TextAsset).text;
		IList dataList = (IList)Util.JsonDecode(data);

		for (int i = 1; i < dataList.Count; i++) {
			IDictionary dic = (IDictionary)dataList[i];	
			Datatable.Inst.LoadDt(dic);
		}

		Debug.Log("Datatable.Inst.dtCharData.Count" + Datatable.Inst.dtCharData.Count);
		BaseOperatorUnit.instance.LoadLevel_Async(SceneName.Lobby);
	}
}

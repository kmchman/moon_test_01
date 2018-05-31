using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MMenu {
	
	[MenuItem("MProject/Generate Data/Table", false, 0)]
	static public void GenerateTable()
	{
		string test = (Resources.Load("DT/CharData") as TextAsset).text;
		IList dic = (IList)Util.JsonDecode(test);
		var enumerator = dic.GetEnumerator();
		while (enumerator.MoveNext()) {
			Debug.Log(enumerator.Current.ToString());
		}

	}

}

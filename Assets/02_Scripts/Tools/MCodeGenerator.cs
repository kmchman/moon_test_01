using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;

public class MCodeGenerator {

	private static string 											m_JsonfilePath = "/98_Table/HeroData";

	public static void GenDatatable()
	{
		// file list
		string filePath = Application.dataPath + m_JsonfilePath + ".json";
		//TextAsset loadedJsonFile = Resources.Load<TextAsset>(jsonFilePath);    
		string contents = System.IO.File.ReadAllText (filePath);

		IList list = (IList)Util.JsonDecode (contents);
		Debug.Log (list);
		var enumerator = list.GetEnumerator ();
		while (enumerator.MoveNext ()) {
			Debug.Log (enumerator.Current);	
		}

		/*
		IDictionary dict = (IDictionary)Util.JsonDecode (contents);
		Debug.Log ("GenDatatable");
		Debug.Log (dict);
*/
	}

	static void GenDatatableCode(StringBuilder sb, IDictionary data) {

		List<string> keys = data.Keys.Cast<string> ().ToList ();

		keys.Sort ();
	}

	static void GenTableCode(StringBuilder sb, string dtName, List<string> names, IDictionary types, string keyName) {

		string keyType = (string)types [keyName];
	}
}

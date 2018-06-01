using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class MCodeGenerator {
	
	static string DATATABLE_DEF_PATH = "Assets/02_Scripts/Data/_Datatable.cs";

	public static void GenerateDatatable()
	{
		string data = (Resources.Load("DT/CharData") as TextAsset).text;
		IList dataList = (IList)Util.JsonDecode(data);
		IDictionary dic = (IDictionary)dataList[0];
		GenDatatableCode(dic);
	}

	private static void CreateDtFile(string filePath, string contents)
	{
		using (StreamWriter sw = new StreamWriter(filePath, false)) {
			try{
				sw.WriteLine(contents);		
			}
			catch {
				Debug.Log("exception");
			}
		}
	}

	private static void GenDatatableCode(IDictionary dic)
	{
		StringBuilder sb = new StringBuilder();

		sb.Append("#pragma warning disable 114\n");
		sb.Append("using UnityEngine;\n");
		sb.Append("using System.Collections;\n");
		sb.Append("using System.Collections.Generic;\n");
		sb.Append("public class _Datatable {\n");
		sb.Append("}");

		CreateDtFile(DATATABLE_DEF_PATH, sb.ToString());
		Debug.Log("Complete GenDatatableCode");
	}
}

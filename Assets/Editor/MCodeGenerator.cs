using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;

public class MCodeGenerator {

	static string DATATABLE_DEF_PATH = "Assets/02_Scripts/Data/_Datatable.cs";

	public static void GenerateDatatable()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append("#pragma warning disable 114\n");
		sb.Append("using UnityEngine;\n");
		sb.Append("using System.Collections;\n");
		sb.Append("using System.Collections.Generic;\n");
		sb.Append("public class _Datatable {\n");

		foreach (var value in Global.dtList) {
			string data = (Resources.Load("DT/" + value) as TextAsset).text;
			IList dataList = (IList)Util.JsonDecode(data);
			IDictionary dic = (IDictionary)dataList[0];
			GenTableCode(sb, value, dic);	
		}
		GenLoadCode(sb);
		sb.Append("}");
		CreateDtFile(DATATABLE_DEF_PATH, sb.ToString());

		Debug.Log("Complete GenDatatableCode");
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

	static void GenLoadCode(StringBuilder sb)
	{
		sb.Append("\tpublic void LoadDatatable()\n");
		sb.Append("\t{\n");
		foreach (string value in Global.dtList) {
			Debug.Log("LoadCharData(null, dic);" + value);
//			sb.AppendFormat("\tLoad{0}\n", value);
			sb.AppendFormat("\tLoad{0}\n", value);
		}
		sb.Append("\t}\n");
	}

	static void GenTableCode(StringBuilder sb, string dtName, IDictionary dic) {
		
		string dtKeyType = GetKeyType(dic);	
		string dtKeyName = GetKeyName(dic);

		Debug.Log("dtName : " + dtName);
		Debug.Log("dtKeyType : " + dtKeyType);
		Debug.Log("dic[dtKeyType] : " + dic[dtKeyType]);
		sb.AppendFormat("\tpublic class {0} {{\n", dtName);
		var enumerator = dic.GetEnumerator();
		while (enumerator.MoveNext()) 
		{
			GenVariableCode(sb, (string)enumerator.Key, (string)enumerator.Value); 
		}
		sb.AppendFormat("\t}};\n");

		sb.AppendFormat("\tpublic Dictionary<{0}, {1}> dt{1} = new Dictionary<{0}, {1}>();\n", dtKeyType, dtName);

		sb.AppendFormat("\tpublic void Load{0}s(IDictionary dic) {{\n", dtName);
		sb.Append("\t\tvar enumerator = dic.GetEnumerator();\n");
		sb.Append("\t\twhile (enumerator.MoveNext()) {\n");
		sb.AppendFormat("\t\t\tLoad{0}(enumerator.Key, enumerator.Value);\n", dtName);
		sb.Append("\t\t}\n");
		sb.Append("\t}\n");

		sb.AppendFormat("\tpublic void Load{0}(object key, object value) {{\n", dtName);
		sb.AppendFormat("\t\tIDictionary v = (IDictionary)value;\n\n");
		sb.AppendFormat("\t\t{0} i = new {0}();\n", dtName);

		enumerator = dic.GetEnumerator();
		while (enumerator.MoveNext()) 
		{
			GenVariableCode2(sb, (string)enumerator.Key, (string)enumerator.Value);
		}

		sb.AppendFormat("\t\tdt{0}.Add(i.{1}, i);\n", dtName, dtKeyName);
		sb.Append("\t}\n");
	}

	static void GenVariableCode(StringBuilder sb, string name, string type) 
	{
		type = type.Replace("*", string.Empty).ToLower();
		name = name.Replace(".", "_");

		if (type.Equals("mtext")) 
			type = "string";
		sb.AppendFormat("\t\tpublic {0} {1};\n", type, name);
	}

	static void GenVariableCode2(StringBuilder sb, string name, string type) 
	{
		type = type.Replace("*", string.Empty).ToLower();
		name = name.Replace(".", "_");

		if (type.Equals("mtext")) 
			type = "string";
		
		if (type.Equals("string")) {
			sb.AppendFormat("\t\ti.{0} = (string)v[\"{0}\"];\n", name);
		} 
		else { 
			sb.AppendFormat("\t\ti.{0} = {1}.Parse((string)v[\"{0}\"]);\n", name, type);
		}
	}

	static string GetKeyType(IDictionary dic)
	{
		var enumerator = dic.GetEnumerator();
		while(enumerator.MoveNext())
		{
			if (enumerator.Value.ToString().Contains("*")) 
			{
				return ((string)enumerator.Value).Replace("*", string.Empty);
			}
		}
		Debug.LogError("key not found");
		return string.Empty;
	}

	static string GetKeyName(IDictionary dic)
	{
		var enumerator = dic.GetEnumerator();
		while(enumerator.MoveNext())
		{
			if (enumerator.Value.ToString().Contains("*"))
				return (string)enumerator.Key;
		}
		Debug.LogError("key not found");
		return string.Empty;
	}
}

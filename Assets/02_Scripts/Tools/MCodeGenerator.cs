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

		GenTableCode(sb, "CharData", dic);
		sb.Append("}");
		CreateDtFile(DATATABLE_DEF_PATH, sb.ToString());

		Debug.Log("Complete GenDatatableCode");
	}


	static void GenTableCode(StringBuilder sb, string dtName, IDictionary dic) {

		string keyType = GetKeyType(dic);	

//		string keyType = (string)types[keyName];
		sb.AppendFormat("\tpublic class {0} {{\n", dtName);
		var enumerator = dic.GetEnumerator();
		while (enumerator.MoveNext()) 
		{
			GenVariableCode(sb, (string)enumerator.Key, (string)enumerator.Value); 
		}
		sb.AppendFormat("\t}};\n");

		sb.AppendFormat("\tpublic Dictionary<{1}, {0}> dt{0} = new Dictionary<{1}, {0}>();\n", dtName, keyType);

		sb.AppendFormat("\tpublic void Load{0}s(IDictionary dic) {{\n", dtName);
		sb.Append("\t\tvar enumerator = dic.GetEnumerator();\n");
		sb.Append("\t\twhile (enumerator.MoveNext()) {\n");
		sb.AppendFormat("\t\t\tLoad{0}(enumerator.Key, enumerator.Value);\n", dtName);
		sb.Append("\t\t}\n");
		sb.Append("\t}\n");

		sb.AppendFormat("\tpublic void Load{0}(object key, object value) {{\n", dtName);
//		sb.AppendFormat("\t\t{0} k;\n", keyType);
		sb.AppendFormat("\t\tIDictionary v = (IDictionary)value;\n\n");
		sb.AppendFormat("\t\t{0} i = new {0}();\n", dtName);
//		sb.AppendFormat("\t\tdt{0}[k] = i;\n", dtName);
		sb.Append("\t}\n");
	}

	static void GenVariableCode(StringBuilder sb, string name, string type) 
	{
		if (type.Contains("*")) {
			type = type.Replace("*", string.Empty);
		}

		if (type.Equals("mtext")) {
//			string mlName = name.Replace('_', '.');
//			sb.AppendFormat("\t\tpublic virtual string {0} {{ get {{ return Datatable.Inst.GetML(GetType(), \"{1}\", {2}.ToString()); }} }}\n", name, mlName, key);
			sb.AppendFormat("\t\tpublic string {0};\n", name);
		} 
		else if(type.Equals("String"))
		{
			sb.AppendFormat("\t\tpublic string {0};\n", name);
		}
		else { 
			sb.AppendFormat("\t\tpublic {0} {1};\n", type, name);
		}
	}

	static string GetKeyType(IDictionary dic)
	{
		var enumerator = dic.GetEnumerator();
		while(enumerator.MoveNext())
		{
			if (enumerator.Value.ToString().Contains("*"))
				return enumerator.Value.ToString().Replace("*", string.Empty);
		}
		Debug.LogError("key not found");
		return string.Empty;
	}
}

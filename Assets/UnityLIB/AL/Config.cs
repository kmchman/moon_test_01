using UnityEngine;

using System.Collections;

namespace AL {

public class Config {

	bool isDebug = true;
	string name;
	IDictionary dict = new Hashtable();

	public Config(string name, bool debugMode = true) {
		this.name = name;
		this.isDebug = debugMode;
	}

	public string ToJson() {
		return Util.JsonEncode(dict);
	}

	private Config LoadJson(string json) {
#if UNITY_EDITOR
		if(isDebug) 
			Debug.Log(Logger.Write("load config", name, json));
#endif
		if (null == json) json = "{}";
		Extend((IDictionary)Util.JsonDecode(json));
		return this;
	}
	public static Config LoadJson(string name, string json, bool isDebug = true) {
		return new Config(name, isDebug).LoadJson(json);
	}

	private Config LoadFile(string path) {
#if UNITY_EDITOR
		if(isDebug)
			Debug.Log(Logger.Write("load config", name, path));
#endif
		return LoadJson(Util.ReadText(path));
	}
	public static Config LoadFile(string name, string path, bool isDebug = true) {
		return new Config(name, isDebug).LoadFile(path);
	}

	private string LongToJson(long value) {
		return value.ToString() + ",\n";
	}

	private string DoubleToJson(double value) {
		return value.ToString() + ",\n";
	}

	private string StringToJson(string value) {
		return "\"" + value.ToString().Replace("\n", "\\n") + "\",\n";
	}

	private string IDictionaryToJson(IDictionary value, int depth) {
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		sb.Append("{\n");
		SortedList sl = new SortedList(value);
		foreach(DictionaryEntry de in sl) {
			if(de.Value == null)
				continue;
			
			for(int i = 0; i < depth; i++)
				sb.Append("\t");

			sb.Append("\t\"" + de.Key + "\": ");
			if(de.Value is long)
				sb.Append(this.LongToJson((long)de.Value));
			else if(de.Value is double)
				sb.Append(this.DoubleToJson((double)de.Value));
			else if(de.Value is string)
				sb.Append(this.StringToJson((string)de.Value));
			else if(de.Value is IDictionary)
				sb.Append(this.IDictionaryToJson((IDictionary)de.Value, depth + 1));
			else
				sb.Append("\n");
		}
		
		sb.Remove(sb.Length - 2, 2);
		if(sb.Length != 0) {
			sb.Append("\n");

			for(int i = 0; i < depth; i++)
				sb.Append("\t");

			sb.Append("},\n");
			return sb.ToString();
		}

		return "{}\n";
	}

	public bool SaveJson(string path)
	{
#if UNITY_EDITOR
		if(isDebug)
			Debug.Log(Logger.Write("svae config", name, path));
#endif
		SortedList sl = new SortedList(dict);
		string text = this.IDictionaryToJson(sl, 0);

		if(text.Length != 0) {
			text = text.Remove(text.Length - 2, 2);
			return Util.WriteFile(path, System.Text.Encoding.UTF8.GetBytes(text));
		}
		else { return false; }
	}

	private Config LoadResource(string path) {
#if UNITY_EDITOR
		if(isDebug)
			Debug.Log(Logger.Write("load config", name, path));
#endif
		return LoadJson((Resources.Load(path) as TextAsset).text);
	}
	public static Config LoadResource(string name, string path, bool isDebug = true) {
		return new Config(name, isDebug).LoadResource(path);
	}

	public Config Extend(IDictionary d) {
		foreach (DictionaryEntry e in d) {
#if UNITY_EDITOR
			if(isDebug)
				Debug.Log(Logger.Write("extend config", name, e.Key, e.Value));
#endif
			dict[e.Key] = e.Value;
		}
		return this;
	}
	public Config Extend(Config cfg) {
		return Extend(cfg.dict);
	}

	public void Set(object key, object val) {
#if UNITY_EDITOR
		if(isDebug)
			Debug.Log(Logger.Write("config.set", name, key, val));
#endif
		dict[key] = val;
	}

	public object Get(object key, object def) {
#if UNITY_EDITOR
		if(isDebug)
			Debug.Log(Logger.Write("config.get", name, key, def));
#endif
		bool exist = dict.Contains(key);
		object val = exist ? dict[key] : def;
#if UNITY_EDITOR
		if(isDebug)
			Debug.Log(Logger.Write("config.get", name, exist, val));
#endif
		return val;
	}

	public IDictionary GetDictionary(object key, IDictionary def) {
		return (IDictionary)Get(key, def);
	}
	public string GetString(object key, string def) {
		return (string)Get(key, def);
	}
	public double GetNumber(object key, double def) {
		return double.Parse(Get(key, def).ToString());
	}

};

}

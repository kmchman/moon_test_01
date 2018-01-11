using UnityEngine;
#if !UNITY_EDITOR && UNITY_IPHONE && !UNITY_4_6
using UnityEngine.iOS;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Reflection;

namespace AL {

public class MemberToString {
	public override string ToString() {
		return Util.JsonEncode2(this);
	}
	public object ToDict() {
		return Util.JsonDecode(this.ToString());
	}
};

public class Util
{
	static private Type TypeOfShort = typeof(short);
	static private Type TypeOfUShort = typeof(ushort);
	static private Type TypeOfInt = typeof(int);
	static private Type TypeOfUInt = typeof(uint);
	static private Type TypeOfLong = typeof(long);
	static private Type TypeOfULong = typeof(ulong);
	static private Type TypeOfFloat = typeof(float);
	static private Type TypeOfBool = typeof(bool);
	static private Type TypeOfDouble = typeof(double);
	static private Type TypeOfString = typeof(string);
	static private Type TypeOfIList = typeof(IList);
	static private Type TypeOfIDictionary = typeof(IDictionary);
	static private Type TypeOfHashtable = typeof(Hashtable);

	static private StringBuilder sb = new StringBuilder();

	static private Dictionary<string, FieldInfo[]> fd = new Dictionary<string, FieldInfo[]>();

	static public bool EqualFloat(float value1, float value2) {
		return Math.Abs(value1 - value2) <= float.Epsilon;
	}

	static public bool EqualDouble(double value1, double value2) {
		return Math.Abs(value1 - value2) <= double.Epsilon;
	}

	static public string StringFormat(string format, params object[] args) {
		sb.Remove(0, sb.Length);
		return sb.AppendFormat(format, args).ToString();
	}

	static public void CheckException(Action action) {
		try {
			action();
		}
		catch(Exception e) {
			Debug.LogError(Logger.Write(e));
		}
	}

	static public long SwapBit(long n, int b1, int b2)
	{
		if(b1 == b2) { return n; }
		if(b1 > b2) { Swap<int>(ref b1, ref b2); }
		
		n ^= (n & (1 << b1)) << (b2 - b1);
		n ^= (n & (1 << b2)) >> (b2 - b1);
		n ^= (n & (1 << (b1))) << (b2 - b1);
			
		return n;
	}

	static public void Swap<T>(ref T lhs, ref T rhs)
	{
		T temp;
		temp = lhs;
		lhs = rhs;
		rhs = temp;
	}

	static public void ForEach<K, V>(IDictionary d, Action<K, V> cb) {
		foreach (DictionaryEntry e in d) cb((K)e.Key, (V)e.Value);
	}
	static public void ForEach(IDictionary d, Action<object, object> cb) {
		ForEach<object, object>(d, cb);
	}
	static public void ForEach<V>(IList l, Action<V> cb) {
		foreach (V e in l) cb(e);
	}
	static public void ForEach(IList l, Action<object> cb) {
		ForEach<object>(l, cb);
	}

	public class StopWatch {
		long st = Millis;
		public void Check(string name) {
			long dt = Millis - st;
			Logger.Write("sw", name, dt);
			st = Millis;
		}
	}
		
	static public bool WriteFile(string path, byte[] data, bool isiCloudAndiTunesBackup = false) {
		try {
			using (FileStream fs = File.Open(path, FileMode.Create)) {
				fs.Write(data, 0, data.Length);
				fs.Close();
#if !UNITY_EDITOR && UNITY_IPHONE
#if UNITY_4_6
				if(isiCloudAndiTunesBackup)
					iPhone.ResetNoBackupFlag(path);
				else
					iPhone.SetNoBackupFlag(path);
#else
				if(isiCloudAndiTunesBackup)
					Device.ResetNoBackupFlag(path);
				else
					Device.SetNoBackupFlag(path);
#endif
#endif
				return true;
			}
		} catch (Exception e) {
			Debug.LogError(Logger.Write("WriteFile", "error!", path, e.Message, e.StackTrace));
			return false;
		}
	}
	static public byte[] ReadFile(string path) {
		Debug.Log(Logger.Write("ReadFile", path));
		try {
			using (FileStream fs = File.OpenRead(path)) {
				byte[] data = new byte[fs.Length];
				fs.Read (data, 0, data.Length);
				fs.Close();
				return data;
			}
		} catch (Exception e) {
			Debug.LogError(Logger.Write("ReadFileObject", "error!", e.Message, e.StackTrace));
			return null;
		}
	}

	static public string ReadText(string path, Encoding encoding = null)
	{
		if (null == encoding)
			encoding = Encoding.UTF8;
		string text;
		try {
			using (FileStream fs = File.Open(path, FileMode.Open)) {
				byte[] b = new byte[fs.Length];
				fs.Read(b, 0, b.Length);
				text = encoding.GetString(b);
				fs.Close();
			}
		} catch (Exception e) {
			Debug.Log(Logger.Write("Util.ReadText", "error", e.Message, e.StackTrace));
			return null;
		}
		return text;
	}
	
	static public string LogFormat(params object[] args)
	{
		string o = null;
		foreach (object a in args) {
			string sa = "\"" + a.ToString() + "\"";
			o = (null != o ? o + "," : "") + sa;
		}
		return o;
	}
	
	static public long Millis {
		get { return DateTime.Now.Ticks / 10000; }
	}
	
	static public object CastValue(Type t, object v) {
		if (t.IsGenericType) {
			Type gt = t.GetGenericTypeDefinition();
			if (TypeOfIList.IsAssignableFrom(gt)) {
				IList nv = (IList)t.GetConstructor(new Type[] {}).Invoke(new object[] {});
				GetListFields(nv, v);
				return nv;
			} else if (TypeOfIDictionary.IsAssignableFrom(gt)) {
				IDictionary nv = (IDictionary)t.GetConstructor(new Type[] {}).Invoke(new object[] {});
				GetDictFields(nv, v);
				return nv;
			}
		} else if (t.IsValueType) {
			if (t.Equals(TypeOfShort)) {
				return short.Parse(v.ToString());
			} else if (t.Equals(TypeOfUShort)) {
				return ushort.Parse(v.ToString());
			} else if (t.Equals(TypeOfInt)) {
				return int.Parse(v.ToString());
			} else if (t.Equals(TypeOfUInt)) {
				return uint.Parse(v.ToString());
			} else if (t.Equals(TypeOfLong)) {
				return long.Parse(v.ToString());
			} else if (t.Equals(TypeOfULong)) {
				return ulong.Parse(v.ToString());
			} else if (t.Equals(TypeOfFloat)) {
				return float.Parse(v.ToString());
			} else if (t.Equals(TypeOfBool)) {
				return bool.Parse(v.ToString());
			} else if (t.Equals(TypeOfDouble)) {
				return double.Parse(v.ToString());
			} else {
				object nv = Activator.CreateInstance(t);
				return GetFields(nv, v);
			}
		} else if (t.IsArray) {
			IList l = v as IList;
			Array nv = Array.CreateInstance(t.GetElementType(), l.Count);
			GetArrayFields(nv, v);
			return nv;
		} else if (t.Equals(TypeOfString)) {
			return v.ToString();
		} else if (t.Equals(TypeOfHashtable)) {
			Hashtable nv = new Hashtable();
			GetHashFields(nv, v);
			return nv;
		} else if (v == null) {
			return null;
		} else {
			object nv = t.GetConstructor(new Type[] {}).Invoke(new object[] {});
			return GetFields(nv, v);
		}
		return v;
	}
	static public void GetArrayFields(Array o, object d) {
		Type t = o.GetType();
		Type et = t.GetElementType();
		int c = 0;
		foreach (object i in (IList)d) {
			object v = CastValue(et, i);
			o.SetValue(v, c++);
		}
	}
	static public void GetListFields(IList o, object d)
	{
		Type t = o.GetType();
		Type at = t.GetGenericArguments()[0];
		foreach (object i in (IList)d) {
			object v = CastValue(at, i);
			o.Add(v);
		}
	}
	static public void GetDictFields(IDictionary o, object d)
	{
		Type t = o.GetType();
		Type[] ga = t.GetGenericArguments();
		Type ak = ga[0];
		Type av = ga[1];
		foreach (DictionaryEntry e in (IDictionary)d) {
			object k = CastValue(ak, e.Key);
			object v = CastValue(av, e.Value);
			o.Add(k, v);
		}
	}
	static public void GetHashFields(Hashtable o, object d)
	{
		Debug.Log(Logger.Write(o, d.GetType()));
		foreach (DictionaryEntry e in (IDictionary)d) {
			object k = e.Key;
			object v = e.Value;
			o.Add(k, v);
		}
	}
	static public object GetFields(object o, object _dic)
	{
		IDictionary dic = (IDictionary)_dic;
		Type t = o.GetType();
		FieldInfo[] fields = null;
		if(fd.ContainsKey(t.FullName)) {
			fields = fd[t.FullName];
		}
		else {
			fields = t.GetFields();
			fd[t.FullName] = fields;
		}
		foreach( FieldInfo f in fields ) {
			if( !dic.Contains(f.Name) ) {
				//Debug.LogWarning(Util.LogFormat("GetField", "!dic.Contains(key)", o, f.Name, f.FieldType));
			} else {
				object v = dic[f.Name];
				if( v == null ) {
					Debug.LogWarning(Util.LogFormat("GetField", "v == null", f.Name, f.FieldType, o));
				} else {
					try {
						Type ft = f.FieldType;
						object fv = CastValue(ft, v);
						f.SetValue(o, fv);
					} catch (Exception e) {
						Debug.LogError(Util.LogFormat("GetField", f.Name, f.FieldType, v, v.GetType(), e, o));
//						throw e;
					}
				}
			}
		}
		return o;
	}
	
	static public byte[] Sub(byte[] bin, int offset, int length) {
		byte[] nbin = new byte[length];
		Array.Copy(bin, offset, nbin, 0, length);
		return nbin;
	}
	static public byte[] Join(IList bufs) {
		int len = 0;
		foreach(byte[] b in bufs) {
			len += b.Length;
		}
		byte[] nbin = new byte[len];
		int offset = 0;
		foreach(byte[] b in bufs) {
			Array.Copy(b, 0, nbin, offset, b.Length);
			offset += b.Length;
		}
		return nbin;
	}
	
	static public void PushBack(IList list, params object[] args) {
		foreach(object o in args) {
			list.Add(o);
		}
	}
	static public void PushFront(IList list, params object[] args) {
		int i = 0;
		foreach(object o in args) {
			list.Insert(i++, o);
		}
	}
	static public object Pop(IList list, int i) {
		object o = list[i];
		list.RemoveAt(i);
		return o;
	}
	static public object PopBack(IList list) {
		return Pop(list, list.Count-1);
	}
	static public object PopFront(IList list) {
		return Pop(list, 0);
	}
	static public object[] ToArray(IList list) {
		return new ArrayList(list).ToArray();
	}
	
	static public object[] mkargs(params object[] args) {
		return args;
	}
	
	static public byte[] Compress(byte[] d) {
		MemoryStream ms = new MemoryStream();
		ms.WriteByte(1);
		ComponentAce.Compression.Libs.zlib.ZOutputStream s
			= new ComponentAce.Compression.Libs.zlib.ZOutputStream(ms, ComponentAce.Compression.Libs.zlib.zlibConst.Z_BEST_COMPRESSION);
		s.Write(d, 0, d.Length);
		s.finish();
		byte[] c = ms.ToArray();
		if (c.Length <= d.Length) {
			return c;
		}
		ms = new MemoryStream();
		ms.WriteByte(0);
		ms.Write(d, 0, d.Length);
		return ms.ToArray();
	}
	
	static public byte[] Decompress(byte[] d) {
		if (0 == d[0]) {
			byte[] d2 = new byte[d.Length-1];
			Array.Copy(d, 1, d2, 0, d2.Length);
			return d2;
		}
		MemoryStream os = new MemoryStream();
		ComponentAce.Compression.Libs.zlib.ZOutputStream s
				= new ComponentAce.Compression.Libs.zlib.ZOutputStream(os);
		s.Write(d, 1, d.Length - 1);
		s.finish();
		return os.ToArray();
	}
	
	static int asyncCounter = 0;
	static public event Action<int> evAsync;
	static public event Action evAsyncWait;
	static public event Action evAsyncWait2;
	static public int AsyncCounter {
		get { return asyncCounter; }
	}
	static public void IncAsync() {
		++asyncCounter;
		Debug.Log(Logger.Write("IncAsync", asyncCounter));
		if (null != evAsync) evAsync(asyncCounter);
	}
	static public void DecAsync() {
		--asyncCounter;
		Debug.Log(Logger.Write("DecAsync", asyncCounter));
		if (null != evAsync) evAsync(asyncCounter);
		if (null != evAsyncWait && 0 == asyncCounter) {
			evAsyncWait2 = evAsyncWait;
			evAsyncWait = null;
			evAsyncWait2();
		}
	}
	static public void WaitAsync(Action cb) {
		if (0 == asyncCounter)
			cb();
		else
			evAsyncWait += () => cb();
	}
	
	static public IEnumerator _AsyncWWW(string url, Action<WWW> cb) {
		IncAsync();
		WWW www = new WWW(url);
		yield return www;
		if (null != www.error) {
			Debug.LogError(Logger.Write("Util._AsyncWWW", "error!", url, www.error));
		}
		cb(www);
		DecAsync();
	}
	static public void AsyncWWW(string url, Action<WWW> cb) {
		GlobalManager.Inst.StartCoroutine(_AsyncWWW(url, cb));
	}
	
	static public void SyncWWW(string url, Action<WWW> cb) {
		WWW www = new WWW(url);
		while (!www.isDone) { System.Threading.Thread.Sleep(1); }
		if (null != www.error) {
			Debug.LogError(Logger.Write("Util._AsyncWWW", "error!", url, www.error));
		}
		cb(www);
	}

	static public void HttpGet(string url, Action<byte[]> cb) {
		Debug.Log(Logger.Write("HttpGet", url));
		AsyncWWW(url, (www) => cb(null != www.error ? null : www.bytes));
	}
	static public void HttpGetALE(string url, Action<byte[]> cb) {
		HttpGet(url, (data) => cb(null == data ? null : ALEDecode(data)));
	}
	static public void HttpGetUtf8(string url, Action<string> cb) {
		HttpGet(url, (data) => cb(null == data ? null : Utf8Decode(data)));
	}
	static public void HttpGetJson(string url, Action<object> cb) {
		HttpGetUtf8(url, (json) => cb(null == json ? null : JsonDecode(json)));
	}
	
	static public string HexEncode(byte[] data) {
		return BitConverter.ToString(data).Replace("-", string.Empty);
	}
	
	static public string ToGitHash(byte[] data) {
		System.Security.Cryptography.SHA1 sha
			= System.Security.Cryptography.SHA1.Create();
		byte[] hdr = Util.Utf8Encode("blob " + data.Length + " ");
		hdr[hdr.Length-1] = 0;
		sha.TransformBlock(hdr, 0, hdr.Length, hdr, 0);
		sha.TransformFinalBlock(data, 0, data.Length);
		return Util.HexEncode(sha.Hash).ToLower();
	}

	static public string ToSHA1(byte[] data) {
		System.Security.Cryptography.SHA1 sha
			= System.Security.Cryptography.SHA1.Create();
		sha.TransformFinalBlock(data, 0, data.Length);
		return HexEncode(sha.Hash).ToLower();
	}
	static public string ToSHA1(string txt) {
		return ToSHA1(Utf8Encode(txt));
	}
	
	static public string Utf8Decode(byte[] data) {
		return Encoding.UTF8.GetString(data);
	}
	static public byte[] Utf8Encode(string str) {
		return Encoding.UTF8.GetBytes(str);
	}

	static public byte[] Base64Decode(string s) {
		return Convert.FromBase64String(s);
	}
	static public string Base64Encode(byte[] s) {
		return Convert.ToBase64String(s);
	}

	static public object JsonDecode(string json) {
		return MiniJSON.Json.Deserialize(json);
	}
	static public string JsonEncode(object obj) {
		return MiniJSON.Json.Serialize(obj);
	}
	static public string JsonEncode2(object obj) {
		return JSON.Serialize(obj);
	}

	static public byte[] ALEDecode(byte[] data) {
		uint s = (uint)data.Length ^ 0x55555555;
		return Decompress(LCGMask.NumericalRecipes(s).mask(data));
	}
	static public byte[] ALEEncode(byte[] data) {
		data = Compress(data);
		uint s = (uint)data.Length ^ 0x55555555;
		return LCGMask.NumericalRecipes(s).mask(data);
	}
	
	static public List<T> GetSpecificFieldsToList<T>(string fieldName, IDictionary iDic)
	{
		List<T> list = new List<T>();
		
		IDictionaryEnumerator en = iDic.GetEnumerator();
		while( en.MoveNext() )
		{
			foreach( FieldInfo f in en.Value.GetType().GetFields() )
			{
				if( f.Name == fieldName ) 
				{
					try {
						list.Add((T)f.GetValue(en.Value));
					}
					catch(Exception e) {
						Debug.LogError("fieldName = " + fieldName + "\n ## cast error : \n" + e + "\n ## trace : \n" + e.StackTrace);	
					}
				}
			}
		}
		
		if( list.Count == 0 ) {
			Debug.LogError("GetSpecificFieldsInDic Error : fieldName = " + fieldName);
		}
		
		return list;
	}
	
	static public SortedList GetSpecificFieldsToSList(string fieldName, IDictionary iDic)
	{
		SortedList slist = new SortedList();
		
		IDictionaryEnumerator en = iDic.GetEnumerator();
		while( en.MoveNext() )
		{
			foreach( FieldInfo f in en.Value.GetType().GetFields() )
			{
				if( f.Name == fieldName ) 
				{
					try {
						slist.Add(en.Key, f.GetValue(en.Value));
					}
					catch(Exception e) {
						Debug.LogError("fieldName = " + fieldName + "\n ## cast error : \n" + e + "\n ## trace : \n" + e.StackTrace);	
					}
				}
			}
		}
		
		if( slist.Count == 0 ) {
			Debug.LogError("GetSpecificFieldsInDic Error : fieldName = " + fieldName);
		}
		
		return slist;
	}
	
	static public SortedList GetSpecificDistinctFieldsToSList(string fieldName, IDictionary iDic)
	{
		IDictionary id = GetSpecificFieldsToSList(fieldName, iDic);
		IDictionaryEnumerator ide = id.GetEnumerator();
		
		SortedList slist = new SortedList();
		ArrayList tempList = new ArrayList();
		
		while( ide.MoveNext() )
		{
			if( tempList.Contains(ide.Value) )
			{
				continue;
			}
			
			tempList.Add(ide.Value);
			slist.Add(ide.Key, ide.Value);
		}
		
		return slist;
	}
	
	static public SortedList GetSortedListByField(string fieldName, IDictionary iDic)
	{
		SortedList slist = GetSpecificDistinctFieldsToSList(fieldName, iDic);
		SortedList retSList = new SortedList();
		
		foreach( DictionaryEntry e in slist )
		{
			retSList.Add(e.Key, iDic[e.Key]);
		}
		
		return retSList;
	}
	
	static public object GetObjectByFieldNameAndValue(string fieldName, object fieldValue, IDictionary iDic)
	{
		IDictionaryEnumerator en = iDic.GetEnumerator();
		while( en.MoveNext() )
		{
			foreach( FieldInfo f in en.Value.GetType().GetFields() )
			{
				if( f.Name == fieldName ) 
				{
					if( (f.GetValue(en.Value)).Equals(fieldValue) )
					{
						return en.Value;
					}
				}
			}
		}
		
		return null;
	}

	static private System.Random _rand = new System.Random((int)Millis);
	static public int RandInt() { return _rand.Next(); }
	static public double RandDouble() { return _rand.NextDouble(); }
	
	public class DelayDone {
		private event Action<bool> ev;
		private int refcnt;
		private bool failure = false;
		public void SetFailure(bool state) {
			failure = state;
		}
		public DelayDone() {
			refcnt = 0;
		}
		public void Done(Action<bool> cb) {
			if (refcnt == 0)
				cb(failure);
			else
				ev += cb;
		}
//		public void Done(Action cb) {
//			if (refcnt == 0)
//				cb();
//			else
//				ev += (state) => { cb(); };
//		}
		public void Capture() {
			refcnt++;
			Debug.Log(Logger.Write("Capture", refcnt));
		}
		public void Release() {
			if (--refcnt == 0 && null != ev) { ev(failure); ev = null; }
			Debug.Log(Logger.Write("Release", refcnt));
		}
		public Action Wrap() {
			Capture();
			return () => Release();
		}
		public Action Wrap(Action cb) {
			Capture();
			return () => { cb(); Release(); };
		}
		public Action<T> Wrap<T>(Action<T> cb) {
			Capture();
			return (a0) => { cb(a0); Release(); };
		}
		public Action<T1, T2> Wrap<T1, T2>(Action<T1, T2> cb) {
			Capture();
			return (a0, a1) => { cb(a0, a1); Release(); };
		}
	};

	public class JSON {
        public static string Serialize(object obj) {
            return Serializer.Serialize(obj);
        }
        sealed class Serializer {
            StringBuilder builder;

            Serializer() {
                builder = new StringBuilder();
            }

            public static string Serialize(object obj) {
                var instance = new Serializer();

                instance.SerializeValue(obj);

                return instance.builder.ToString();
            }

            void SerializeValue(object value) {
                IList asList;
                IDictionary asDict;
                string asStr;

                if (value == null) {
                    builder.Append("null");
                }
                else if ((asStr = value as string) != null) {
                    SerializeString(asStr);
                }
                else if (value is bool) {
                    builder.Append(value.ToString().ToLower());
                }
                else if ((asList = value as IList) != null) {
                    SerializeArray(asList);
                }
                else if ((asDict = value as IDictionary) != null) {
                    SerializeDictionary(asDict);
                }
                else if (value is char) {
                    SerializeString(value.ToString());
                }
                else {
                    SerializeOther(value);
                }
            }

            void SerializeDictionary(IDictionary obj) {
                bool first = true;

                builder.Append('{');

                foreach (object e in obj.Keys) {
                    if (!first) {
                        builder.Append(',');
                    }

                    SerializeString(e.ToString());
                    builder.Append(':');

                    SerializeValue(obj[e]);

                    first = false;
                }

                builder.Append('}');
            }

            void SerializeArray(IList anArray) {
                builder.Append('[');

                bool first = true;

                foreach (object obj in anArray) {
                    if (!first) {
                        builder.Append(',');
                    }

                    SerializeValue(obj);

                    first = false;
                }

                builder.Append(']');
            }

            void SerializeString(string str) {
                builder.Append('\"');

                char[] charArray = str.ToCharArray();
                foreach (var c in charArray) {
                    switch (c) {
                    case '"':
                        builder.Append("\\\"");
                        break;
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '\b':
                        builder.Append("\\b");
                        break;
                    case '\f':
                        builder.Append("\\f");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    default:
                        int codepoint = Convert.ToInt32(c);
                        if ((codepoint >= 32) && (codepoint <= 126)) {
                            builder.Append(c);
                        }
                        else {
                            builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
                        }
                        break;
                    }
                }

                builder.Append('\"');
            }

            void SerializeOther(object value) {
                if (value is float
                    || value is int
                    || value is uint
                    || value is long
                    || value is double
                    || value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is ulong
                    || value is decimal) {
                    builder.Append(value.ToString());
                }
                else {
                    SerializeObject(value);
                }
            }

			void SerializeObject(object obj) {
				Hashtable h = new Hashtable();
				FieldInfo[] fields = obj.GetType().GetFields();
				foreach (FieldInfo f in fields) {
					h[f.Name] = f.GetValue(obj);
				}
				SerializeDictionary(h);
			}
        }
	};

};

public class Logger
{
	private static bool _init = false;
	private static string logFilePath;
	public static StreamWriter _LogFile;
	public static StreamWriter LogFile() {
		if (!_init) {
			_init = !_init;
			ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
		}
		if (null == _LogFile) {
			if(null == logFilePath)
				logFilePath = Application.persistentDataPath+"/log.txt";
#if UNITY_EDITOR
			Debug.Log("log file path: " + logFilePath);
			FileStream f = new FileStream(logFilePath, FileMode.Create);
			_LogFile = new StreamWriter(f);
#else
			try	{
				File.Delete(logFilePath);
			}
			catch(Exception) {}
#endif
		}
		return _LogFile;
	}
	public static string tos(object o) {
		return null == o ? "<nil>" : o.ToString();
	}
	public static string tos(params object[] args) {
		string s = "";
		if (args.Length > 0) {
			s += "0: " + tos(args[0]);
		}
		for (int i = 1; i < args.Length; ++i) {
			s += "\t" + i + ": " + tos(args[i]);
		}
		return s;
	}
	private static object wclock = new object();
	private static List<object> writeCache;
	private static void __SendLog(IList wc) {
		Hashtable  d = new Hashtable();
		d["deviceId"] = SystemInfo.deviceUniqueIdentifier;
		d["logs"] = wc;
		string s = Util.JsonEncode(d);
		Hashtable hdr = new Hashtable();
		hdr["Authorization"]
			= "Basic " + Util.Base64Encode(Util.Utf8Encode("elslog:alal22"));
		HTTP.Post("https://aldev.altwave.com/log", hdr, Util.Utf8Encode(s), (status, data) => {
		});
	}
	private static void __WriteServerFlush() {
		//List<object> wc;
		lock (wclock) {
			if (null == writeCache || writeCache.Count <= 0)
				return;
			//wc = writeCache;
			writeCache = new List<object>();
		}
		//__SendLog(wc);
	}
	private static void _WriteServerFlush() {
		__WriteServerFlush();
		Global.Inst.PushTimeout(500, _WriteServerFlush);
	}
	private static void WriteServer(object l) {
		lock (wclock) {
			if (null == writeCache) {
				writeCache = new List<object>();
				Global.Inst.PushTimeout(500, _WriteServerFlush);
			}
			writeCache.Add(l);
		}
	}
	public static string WriteObjects(params object[] args) {
		List<object> l = new List<object>(args);
		l.Insert(0, Util.Millis);
		string s = Util.JsonEncode(l);
		if (LogFile() != null) {
			LogFile().Write(s);
			OperatingSystem os = Environment.OSVersion;
			if (os.Platform != PlatformID.Unix && os.Platform != PlatformID.MacOSX)
				LogFile().Write("\r\n");
			else
				LogFile().Write("\n");
			LogFile().Flush();
			WriteServer(l);
		}
		return "Logger> " + s;
	}
	public static string Write(params object[] args) {
		return WriteObjects(args);
	}
	/*
	public static void Log(params object[] args) {
		string s = tos (args);
		LogFile().Write(s);
		LogFile().Write("\n");
		LogFile().Flush();
		Debug.Log(s);
	}
	public static void Warning(params object[] args) {
		string s = tos (args);
		LogFile().Write(s);
		LogFile().Write("\n");
		LogFile().Flush();
		Debug.LogWarning(s);
	}
	public static void Error(params object[] args) {
		string s = tos (args);
		LogFile().Write(s);
		LogFile().Write("\n");
		LogFile().Flush();
		Debug.LogError(s);
	}
	*/
};

}

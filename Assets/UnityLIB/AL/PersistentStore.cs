using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace AL {

public class PersistentStore {

	static private PersistentStore _inst = new PersistentStore();
	static public PersistentStore Inst { get { return _inst; } }
	
	static public string BaseDir { get { return baseDir + "/"; } }
	static private string baseDir;
	static public string ObjDir { get { return objDir; } }
	static private string objDir = BaseDir + "object/";

	static public string resourceURL;

	public PersistentStore()
	{
		PersistentStore.baseDir = Application.persistentDataPath;
	}
	
	static public void Init(string resurl) {
		resourceURL = resurl;
		try {
			string file = objDir.Substring(0, objDir.Length - 1);
			FileAttributes attr = File.GetAttributes(file);
			if (attr != FileAttributes.Directory)
				File.Delete(file);
		} catch (Exception) {
		}
		try {
			if (!Directory.Exists(objDir))
				Directory.CreateDirectory(objDir);
		} catch (Exception) {
		}
	}

	static public string GetPath(string path) {
		return BaseDir + path;
	}
	
	const bool enableCheckHash = true;
	static public bool _CheckHash(string id, byte[] data) {
		Util.StopWatch sw = new Util.StopWatch();
		System.Security.Cryptography.SHA1 sha
			= System.Security.Cryptography.SHA1.Create();
		byte[] hdr = Util.Utf8Encode("blob " + data.Length + " ");
		hdr[hdr.Length-1] = 0;
		sha.TransformBlock(hdr, 0, hdr.Length, hdr, 0);
		sha.TransformFinalBlock(data, 0, data.Length);
		string hid = Util.HexEncode(sha.Hash).ToLower();
		sw.Check("CalcHash");
		return id == hid;
	}
	static public bool CheckHash(string id, byte[] data) {
		return !enableCheckHash || _CheckHash(id, data);
	}
	
	public delegate void cbBytes(byte[] data);
	public delegate void cbAssetBundle(AssetBundle assetBundle);
	public delegate void cbWWW(WWW www);

	public bool IsExistFile(string id) {
		if(null == id) 
			return false;
		return File.Exists(objDir + id);
	}
	
	public void ReadFileObject(string id, cbBytes cb) {
		string path = objDir + id;
		if (!File.Exists(path)) {
			cb(null);
			return;
		}
		Debug.Log(Logger.Write("ReadFileObject", path));
		Util.StopWatch sw = new Util.StopWatch();			
		byte[] data = Util.ReadFile(path);
		sw.Check("ReadFileObject");
		cb(data);
	}
	public void AsyncReadFileObject(string id, cbBytes cb) {
		AsyncReadFile(id, (www) => {
			if (null == www || null != www.error) {
				cb(null);
				return;
			}
			cb(www.bytes);
		});
	}
	public void AsyncReadFileAssetBundle(string id, cbAssetBundle cb) {
		AsyncReadFile(id, (www) => {
			if (null == www || null != www.error) {
				cb(null);
				return;
			}
			cb(www.assetBundle);
		});
	}
	private void AsyncReadFile(string id, cbWWW cb) {
		string path = objDir + id;
		if (!File.Exists(path)) {
			cb(null);
			return;
		}
		Debug.Log(Logger.Write("AsyncReadFile", path));
		Util.StopWatch sw = new Util.StopWatch();
		string prefix = null;
		OperatingSystem os = Environment.OSVersion;
		if (os.Platform != PlatformID.Unix && os.Platform != PlatformID.MacOSX)
			prefix = "file:///";
		else
			prefix = "file://";
		Util.AsyncWWW(prefix + path, (www) => {
			sw.Check("AsyncReadFile");
			cb(www);
		});
	}
	public void ReadHttpObject(string id, cbBytes cb) {
		if (null == id) {
			cb(null);
			return;
		}
		string url = resourceURL+"object/" + id;
		Debug.Log(Logger.Write("ReadHttpObject", url));
		Util.StopWatch sw = new Util.StopWatch();
		SimpleHTTP.SyncGET(url, (www) => {
			if (null == www || null != www.error) {
				cb(null);
				return;
			}
			sw.Check("ReadHttpObject");
			cb(www.bytes);
		});
	}
	public void AsyncReadHttpObject(string id, cbBytes cb) {
		if (null == id) {
			cb(null);
			return;
		}
		string url = resourceURL+"object/" + id;
		Debug.Log(Logger.Write("AsyncReadHttpObject", url));
		Util.StopWatch sw = new Util.StopWatch();
		SimpleHTTP.AsyncGET(url, (www) => {
			if (null == www || null != www.error) {
				cb(null);
				return;
			}
			sw.Check("AsyncReadHttpObject");
			cb(www.bytes);
		});
	}

	public void LoadObject(string id, cbBytes cb) {
		ReadFileObject(id, (data1) => {
			if (null != data1) {
				if (CheckHash(id, data1)) {
					cb(data1);
					return;
				}
				Debug.LogWarning(Logger.Write("PersistentStore.LoadObject", "invalid hash", id));
			}
			ReadHttpObject(id, (data2) => {
				if (null == data2) {
					cb(null);
					return;
				}
				if (!CheckHash(id, data2)) {
					Debug.LogError(Logger.Write("PersistentStore.LoadObject", "invalid hash error!", id));
					cb(null);
					return;
				}
				string path = objDir + id;
				if (!Util.WriteFile(path, data2)) {
					Debug.LogError(Logger.Write("PersistentStore.LoadObject", "file write error!", id, path));
					cb(null);
					return;
				}
				cb(data2);
			});
		});
	}
		
	public void AsyncLoadObject(string id, cbBytes cb) {
		cbBytes cb2 = cb;
		Util.IncAsync();
		cb = (data) => {
			Global.Inst.PushTask(() => {
				cb2(data);
				Util.DecAsync();
			});
		};
		AsyncReadFileObject(id, (data1) => {
			if (null != data1) {
				if (CheckHash(id, data1)) {
					cb(data1);
					return;
				}
				Debug.LogWarning(Logger.Write("PersistentStore.AsyncLoadObject", "invalid hash", id));
			}
			AsyncReadHttpObject(id, (data2) => {
				if (null == data2) {
					cb(null);
					return;
				}
				if (!CheckHash(id, data2)) {
					Debug.LogError(Logger.Write("PersistentStore.AsyncLoadObject", "invalid hash error!", id));
					cb(null);
					return;
				}
				string path = objDir + id;
				if (!Util.WriteFile(path, data2)) {
					Debug.LogError(Logger.Write("PersistentStore.AsyncLoadObject", "file write error!", id, path));
					cb(null);
					return;
				}
				cb(data2);
			});
		});
	}

	private object downloadLock = new object();
	private class DownloadItem {
		public string id;
		public Action<byte[]> cb;
		public void InvokeCB(byte[] bytes) {
			cb(bytes);
		}
	}
	private Dictionary<string, DownloadItem> downloadList = new Dictionary<string, DownloadItem>();
	private Queue<string> firstDownloadQueue = new Queue<string>();
	private Queue<string> downloadQueue = new Queue<string>();

	private int downloadCount = 0;
	public void DoDownload() {
		lock (downloadLock) {
			if (downloadCount >= 1)
				return;
			++downloadCount;
		}
		string id;
		lock (downloadLock) {
			if (firstDownloadQueue.Count <= 0 && downloadQueue.Count <= 0)
				return;

			if (firstDownloadQueue.Count > 0)
				id = firstDownloadQueue.Dequeue();
			else
				id = downloadQueue.Dequeue();
		}
		
		Global.Inst.PushThreadTask(() => {
			if(IsExistFile(id)) {
				ReadFileObject(id, (data) => {
					DownloadItem i;
					lock (downloadLock) {
						--downloadCount;
						i = downloadList[id];
						downloadList.Remove(id);
					}
					if (firstDownloadQueue.Count > 0 || downloadQueue.Count > 0)
						DoDownload();
					Global.Inst.PushTask(() => {
						i.InvokeCB(data);
					});
				});
			}
			else {
				AsyncReadHttpObject(id, (data) => {
					DownloadItem i;
					lock (downloadLock) {
						--downloadCount;
						i = downloadList[id];
						downloadList.Remove(id);
					}
					if (firstDownloadQueue.Count > 0 || downloadQueue.Count > 0)
						DoDownload();
					Global.Inst.PushTask(() => {
						i.InvokeCB(data);
					});
					Global.Inst.PushThreadTask(() => {
						if (null != data) {
							string path = objDir + id;
							if (!Util.WriteFile(path, data)) {
								Debug.LogError(Logger.Write("PersistentStore.LoadObject", "file write error!", id, path));
								data = null;
							}
						}
					});
				});
			}
		});
	}
	public void DownloadObject(string id, cbBytes cb, bool isHighPriority = false) {
		DownloadItem i;
		lock (downloadLock) {
			if (!downloadList.ContainsKey(id)) {
				i = downloadList[id] = new DownloadItem();
				i.id = id;
				if(isHighPriority)
					firstDownloadQueue.Enqueue(id);
				else
					downloadQueue.Enqueue(id);
			} else {
				i = downloadList[id];
			}
		}
		i.cb = (data) => Global.Inst.PushTask(() => cb(data));
		DoDownload();
	}

	public void CancelDownload(HashSet<string> idSet)
	{
		if(idSet == null)
			return;

		Queue<string> oldDownloadQueue = downloadQueue;
		Queue<string> oldFirstDownloadQueue = firstDownloadQueue;

		downloadQueue = new Queue<string>();
		firstDownloadQueue = new Queue<string>();

		string id;
		while(oldDownloadQueue.Count > 0) {
			id = oldDownloadQueue.Dequeue();
			if(!idSet.Contains(id))
				downloadQueue.Enqueue(id);
			else
				downloadList.Remove(id);
		}

		while(oldFirstDownloadQueue.Count > 0) {
			id = oldFirstDownloadQueue.Dequeue();
			if(!idSet.Contains(id))
				firstDownloadQueue.Enqueue(id);
			else
				downloadList.Remove(id);
		}
	}
	
	public void LoadJsonFromID(string id, Action<object> cb) {
		LoadObject(id, (data) => {
			if (null == data) {
				cb(null);
				return;
			}
			data = Util.ALEDecode(data);
			string json = Util.Utf8Decode(data);
			object obj = Util.JsonDecode(json);
			cb(obj);
		});
	}
		
	public void AsyncLoadJsonFromID(string id, Action<object> cb) {
		AsyncLoadObject(id, (data) => {
			if (null == data) {
				cb(null);
				return;
			}
			data = Util.ALEDecode(data);
			string json = Util.Utf8Decode(data);
			object obj = Util.JsonDecode(json);
			cb(obj);
		});
	}
	
	public void LoadTexture(string id, Action<Texture> cb) {
		LoadObject(id, (data) => {
			if (null == data) {
				cb(null);
				return;
			}
			Util.StopWatch sw = new Util.StopWatch();
			Texture2D tex = new Texture2D(0, 0, TextureFormat.RGB565, false);
			tex.LoadImage(data);
			tex.wrapMode = TextureWrapMode.Clamp;
			tex.Compress(false);
			sw.Check("LoadImage");
			//Debug.Log(Logger.Write(tex.width, tex.height, tex.format));
			//tex.Resize(512, 512, TextureFormat.RGB24, false);
			//sw.Check("Resize");
			//Debug.Log(Logger.Write(tex.width, tex.height, tex.format));
			//tex.Resize(512, 512, TextureFormat.PVRTC_RGB2, false);
			//sw.Check("Resize");
			//Debug.Log(Logger.Write(tex.width, tex.height, tex.format));
			//tex.Apply();
			//sw.Check("Apply");
			//Debug.Log(Logger.Write(tex.width, tex.height, tex.format));
			//tex.Compress(true);
			//sw.Check("Compress");
			//Debug.Log(Logger.Write(tex.width, tex.height, tex.format));
			cb(tex);
			//GC.Collect();
			//GC.WaitForPendingFinalizers();
		});
	}

	public void LoadTextureTherad(string id, Action<Texture> cb) {
		Global.Inst.PushThreadTask(() => {
			LoadObject(id, (data) => {
				if (null == data) {
					cb(null);
					return;
				}

				Global.Inst.PushTask(() => {
					Util.StopWatch sw = new Util.StopWatch();
					Texture2D tex = new Texture2D(0, 0, TextureFormat.RGB565, false);
					tex.LoadImage(data);
					tex.wrapMode = TextureWrapMode.Clamp;
					tex.Compress(false);
					sw.Check("LoadImage");
					cb(tex);
				});
			});	
		});
	}
		
	public void AsyncLoadTexture(string id, Action<Texture> cb) {
		AsyncLoadObject(id, (data) => {
			if (null == data) {
				cb(null);
				return;
			}
			Util.StopWatch sw = new Util.StopWatch();
			Texture2D tex = new Texture2D(0, 0, TextureFormat.RGB565, false);
			tex.LoadImage(data);
			tex.wrapMode = TextureWrapMode.Clamp;
			tex.Compress(false);
			sw.Check("LoadImage");
			cb(tex);
		});
	}
	
	public void LoadAssetBundle(string id, cbAssetBundle cb)
	{
		string path = objDir + id;
#if (UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
		if(File.Exists(path)) { cb(AssetBundle.CreateFromFile(path)); }
#else
		if(File.Exists(path)) { cb(AssetBundle.LoadFromFile(path)); }
#endif
		else
		{
			ReadHttpObject(id, (data) => 
			{
				if (null == data) {
					cb(null);
					return;
				}
				if (!CheckHash(id, data)) {
					Debug.LogError(Logger.Write("PersistentStore.LoadObject", "invalid hash error!", id));
					cb(null); return;
				}
				if (!Util.WriteFile(path, data)) 
				{
					Debug.LogError(Logger.Write("PersistentStore.LoadObject", "file write error!", id, path));
					cb(null); return;
				}
#if (UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
				cb(AssetBundle.CreateFromFile(path));
#else
				cb(AssetBundle.LoadFromFile(path));
#endif
			});
		}
	}

	public void LoadAssetBundleFromMemory(string id, cbAssetBundle cb)
	{
		LoadObject(id, (data) => {
			if (null == data) {
				cb(null);
				return;
			}
			Util.StopWatch sw = new Util.StopWatch();
#if (UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
			AssetBundle assetBundle = AssetBundle.CreateFromMemoryImmediate(data);
#else
			AssetBundle assetBundle = AssetBundle.LoadFromMemory(data);
#endif
			sw.Check("LoadAssetBundle");
			cb(assetBundle);
		});
	}
	
	public void AsyncLoadAssetBundleFromMemory(string id, cbAssetBundle cb)
	{
		string path = objDir + id;
		if (File.Exists(path))
		{
			AsyncReadFileAssetBundle(id, cb);
		}
		else
		{
			AsyncLoadObject(id, (data) => {
				if (null == data) {
					cb(null);
					return;
				}
				Util.StopWatch sw = new Util.StopWatch();
#if (UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
				AssetBundle assetBundle = AssetBundle.CreateFromMemoryImmediate(data);
#else
				AssetBundle assetBundle = AssetBundle.LoadFromMemory(data);
#endif
				sw.Check("LoadAssetBundle");
				cb(assetBundle);
			});
		}
	}
}

}

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace AL {

public class ResourceManager {

	static public string imagePath = "img/";
	static public string fontPath = "font/";
	static public string voicePath = "voice/";

	static public string rawImagePath = "raw/" + ResourceManager.imagePath;
	static public string rawFontPath = "raw/" + ResourceManager.fontPath;
	static public string rawVoicePath = "raw/" + ResourceManager.voicePath;
	
	public string resID = "";
	public Hashtable resfiles = new Hashtable();
	public string GetID(string fn) {
		return (string)resfiles[fn];
	}
	public Dictionary<string, long> ressizes = new Dictionary<string, long>();

	public void ResetResID() {
		resID = "";
	}

	public void LoadJsonFileByThread(string fn, Action<object> cb) {
		Debug.Log(Logger.Write("ResourceManager.LoadJsonFileByThread",fn));
		string id = GetID(fn);
		if (null == id ) {
			cb(null);
			return;
		}

		Global.Inst.PushThreadTask(() => { PersistentStore.Inst.LoadJsonFromID(id, cb); });
	}

	public void LoadJsonFile(string fn, Action<object> cb) {
		Debug.Log(Logger.Write("ResourceManager.LoadJsonFile",fn));
		string id = GetID(fn);
		if (null == id ) {
			cb(null);
			return;
		}
		PersistentStore.Inst.LoadJsonFromID(id, cb);
	}
		
	public void AsyncLoadJsonFile(string fn, Action<object> cb) {
		Debug.Log(Logger.Write("ResourceManager.AsyncLoadJsonFile",fn));
		string id = GetID(fn);
		if (null == id ) {
			cb(null);
			return;
		}
		PersistentStore.Inst.AsyncLoadJsonFromID(id, cb);
	}
	
	public RuntimePlatform currentPlatform = Application.platform;
	
	public bool enableAssetBundle = false;
	public bool enableTexCache = true;
	
	Dictionary<Texture, string> texCacheReverse = new Dictionary<Texture, string>();
	Dictionary<string, Texture> texCache = new Dictionary<string, Texture>();
	Dictionary<string, AssetBundle> texAssetBundleCache = new Dictionary<string, AssetBundle>();

	public bool IsExist(string fn) {
		return IsExistByID(GetID(fn));
	}

	public bool IsExistTexture(string fn) {
		return IsExistByID(GetID(rawImagePath+fn));
	}

	public bool IsExistFont(string fn) {
		return IsExistByID(GetID(rawFontPath+fn));
	}

	public bool IsExistVoice(string fn) {
		string id = GetID(rawVoicePath+fn);
		if(null != id && voiceAssetBundleCache.ContainsKey(id))
			return true;

		return IsExistByID(id);
	}

	public bool IsExistByID(string id) {
		if(null == id)
			return false;
		
		return PersistentStore.Inst.IsExistFile(id);
	}

	public void LoadTextureInFileByThread(string fn, Action<Texture> cb) {
		Util.StopWatch sw = new Util.StopWatch();

		string id = GetID(rawImagePath+fn);
		
		if (null == id) {
			cb(null);
			return;
		}

		if(enableTexCache && texCache.ContainsKey(id)) {
			sw.Check("LoadTextureInFileByThread.Cached");
			cb(texCache[id]);
			return;
		}

		PersistentStore.Inst.LoadTextureTherad(id, (tex) => {
			if (enableTexCache && null != tex) {
				texCache[id] = tex;
				texCacheReverse[tex] = id;
			}
			sw.Check("LoadTextureInFileByThread.PersistentStore");
			cb(tex);
			return;
		});
	}
	
	public void LoadTexture(string fn, Action<Texture> cb) {
//		Debug.Log(Logger.Write("ResourceManager.LoadTexture",fn));

		Util.StopWatch sw = new Util.StopWatch();
		
		string id = GetID(rawImagePath+fn);
		if(enableAssetBundle) {
			switch(currentPlatform) {
				case RuntimePlatform.Android: id = GetID("android/img/" + fn); break;
				case RuntimePlatform.IPhonePlayer: id = GetID("ios/img/" + fn); break;
			}
		}

		if (null == id) {
			Texture tex = Resources.Load(imagePath+fn) as Texture;
			if (null == tex)
				tex = Resources.Load(fn) as Texture;
			sw.Check("LoadTexture.Resources.Load");
			cb(tex);
			//			Debug.Log(Logger.Write("ResourceManager.LoadTexture","local", fn));
			return;
		}
		
		LoadTextureByID(id, cb);
	}
	
	public Texture2D LoadTexture(string fn)
	{
		Texture2D texture2d = null;
		this.LoadTexture(fn, (texture) => 
		{
			texture2d = (Texture2D)texture;
		});
		
		return texture2d;
	}

	public void LoadTextureByID(string id, Action<Texture> cb) {
		Util.StopWatch sw = new Util.StopWatch();

		if(null == id) {
			cb(null);
			return;
		}

		if (enableTexCache && texCache.ContainsKey(id)) {
			sw.Check("LoadTexture.Cached");
			cb(texCache[id]);
			return;
		}
		
		if(enableAssetBundle && (currentPlatform == RuntimePlatform.Android || currentPlatform == RuntimePlatform.IPhonePlayer)) {
			PersistentStore.Inst.LoadAssetBundle(id, (assetBundle) => {
				if(assetBundle != null)	{
					Texture tex = GetMainAsset(assetBundle) as Texture;
					if(tex != null) {
						texAssetBundleCache[id] = assetBundle;
						texCache[id] = tex;
						texCacheReverse[tex] = id;
						cb(tex);
						return;
					}
					else {
						assetBundle.Unload(true);
					}
				}		
			});
		}
		else {
			PersistentStore.Inst.LoadTexture(id, (tex) => {			
				if (enableTexCache && null != tex) {
					texCache[id] = tex;
					texCacheReverse[tex] = id;
				}
				sw.Check("LoadTexture.PersistentStore");
				cb(tex);
				return;
			});		
		}
	}

	public Dictionary<string, Action<Texture>> asyncLoadTextureCBCache = new Dictionary<string, Action<Texture>>();
	public void AsyncLoadTextureInFile(string fn, Action<Texture> cb) {
		Util.StopWatch sw = new Util.StopWatch();
		string id = GetID(rawImagePath+fn);
			
		if (null == id) {
			Texture tex = Resources.Load(imagePath+fn) as Texture;
			if (null == tex)
				tex = Resources.Load(fn) as Texture;
			sw.Check("LoadTexture.Resources.Load");
			cb(tex);
//			Debug.Log(Logger.Write("ResourceManager.LoadTexture","local", fn));
			return;
		}
			
		if (enableTexCache && texCache.ContainsKey(id)) {
			sw.Check("LoadTexture.Cached");
			cb(texCache[id]);
			return;
		}

		if (asyncLoadTextureCBCache.ContainsKey(id)) {
			asyncLoadTextureCBCache[id] += cb;
			return;
		}

		asyncLoadTextureCBCache[id] = null;
		PersistentStore.Inst.AsyncLoadTexture(id, (tex) => {	
			if (enableTexCache && null != tex) {
				texCache[id] = tex;
				texCacheReverse[tex] = id;
			}
			sw.Check("LoadTexture.PersistentStore");
			cb(tex);

			if (asyncLoadTextureCBCache.ContainsKey(id)) {
				if (asyncLoadTextureCBCache[id] != null)
					asyncLoadTextureCBCache[id](tex);
				asyncLoadTextureCBCache.Remove(id);
			}
			return;
		});	
	}

	Dictionary<string, AssetBundle> fontAssetBundleCache = new Dictionary<string, AssetBundle>();
	public void LoadFont(string fn, Action<Font> cb) {
		string id = GetID(rawFontPath + fn);
		LoadFontByID(id, cb);
	}

	public void LoadFontByID(string id, Action<Font> cb) {
		Util.StopWatch sw = new Util.StopWatch();

		if(null == id) {
			cb(null);
			return;
		}

		if(fontAssetBundleCache.ContainsKey(id)) {
			sw.Check("LoadFont.Cached");
			cb(GetMainAsset(fontAssetBundleCache[id]) as Font);
			return;
		}

		PersistentStore.Inst.LoadAssetBundle(id, (assetBundle) => {
			UnityEngine.Object mainAsset = GetMainAsset(assetBundle);
			if(null != mainAsset && mainAsset.GetType() == typeof(Font)) {
				fontAssetBundleCache[id] = assetBundle;
				sw.Check("LoadFont.PersistentStore");
				cb(mainAsset as Font);
			}
			else
				cb(null);

			return;
		});
	}

	Dictionary<string, AssetBundle> voiceAssetBundleCache = new Dictionary<string, AssetBundle>();
	public void LoadVoice(string fn, Action<AudioClip> cb) {
		LoadVoice(fn, null, cb);
	}

	public void LoadVoice(string fn, string assetName, Action<AudioClip> cb) {
		string id = GetID(rawVoicePath + fn);
		LoadVoiceByID(id, assetName, cb);
	}

	public void LoadVoiceByID(string id, string assetName, Action<AudioClip> cb) {
		Util.StopWatch sw = new Util.StopWatch();
		
		if(null == id) {
			cb(null);
			return;
		}
		
		if(voiceAssetBundleCache.ContainsKey(id)) {
			sw.Check("LoadVoice.Cached");
			AudioClip audioClip = null;
			if(string.IsNullOrEmpty(assetName))
				audioClip = GetMainAsset(voiceAssetBundleCache[id]) as AudioClip;
			else
				audioClip = voiceAssetBundleCache[id].LoadAsset<AudioClip>(assetName);

			if(null != audioClip)
				cb(audioClip);
			else
				cb(null);

			return;
		}
		
		PersistentStore.Inst.LoadAssetBundle(id, (assetBundle) => {
			AudioClip audioClip = null;
			if(string.IsNullOrEmpty(assetName))
				audioClip = GetMainAsset(assetBundle) as AudioClip;
			else
				audioClip = assetBundle.LoadAsset<AudioClip>(assetName);

			if(null != audioClip) {
				voiceAssetBundleCache[id] = assetBundle;
				sw.Check("LoadVoice.PersistentStore");
				cb(audioClip);
			}
			else
				cb(null);
			
			return;
		});
	}
	
	public void UnloadUnusedTexture(List<Texture> usedTextureList)
	{
		usedTextureList = usedTextureList.Distinct().ToList();
		List <Texture> unusedTextureList = texCacheReverse.Keys.Except(usedTextureList).ToList();
		foreach(Texture texture in unusedTextureList)
		{
			UnloadTexture(texture);
		}
	}

	public void UnloadTexture(Texture texture)
	{
		if(null == texture)
			return;

		if(!texCacheReverse.ContainsKey(texture))
			return;
		
		string id = texCacheReverse[texture];
		if(texCache.ContainsKey(id)) 
			texCache.Remove(id);
		
		if(texAssetBundleCache.ContainsKey(id)) {
			texAssetBundleCache[id].Unload(true);
			texAssetBundleCache.Remove(id);
		}
		
		texCacheReverse.Remove(texture);
	}
	
	public void ClearTextureCache() {
		texCache = new Dictionary<string, Texture>();
		GC.Collect();
		GC.WaitForPendingFinalizers();
	}

	public void UnloadAllFont() {
		UnloadUnusedFont(null);
	}

	public void UnloadUnusedFont(Font font) {
		List<string> keys = fontAssetBundleCache.Keys.ToList();
		for(int i = 0; i < keys.Count; i++) {
			if(GetMainAsset(fontAssetBundleCache[keys[i]]) == font)
				continue;

			fontAssetBundleCache[keys[i]].Unload(true);
			fontAssetBundleCache.Remove(keys[i]);
		}
	}

	public void UnloadAllVoice() {
		List<string> keys = voiceAssetBundleCache.Keys.ToList();

		for(int i = 0; i < keys.Count; i++) {
			voiceAssetBundleCache[keys[i]].Unload(true);
			voiceAssetBundleCache.Remove(keys[i]);
		}
	}

	public UnityEngine.Object GetMainAsset(AssetBundle asb)
	{
		if(asb == null) { return null; }
		if(asb.mainAsset != null) { return asb.mainAsset; }
		if(asb.GetAllAssetNames().Length <= 0) { return null; }

		return asb.LoadAsset(asb.GetAllAssetNames()[0]);
	}
	
	public bool isCache(string fn)
	{
		string id = GetID(rawImagePath+fn);
		if(enableAssetBundle) {
			switch(currentPlatform) {
				case RuntimePlatform.Android: id = GetID("android/img/" + fn); break;
				case RuntimePlatform.IPhonePlayer: id = GetID("ios/img/" + fn); break;
			}
		}
		
		return (id != null && enableTexCache && texCache.ContainsKey(id));
	}
		
	// cb param : isFailure, isNewData
	public void Init(string resourceURL, string id, Action<bool, bool> cb) {
		if(resID == id) { cb(false, false); return; }
			
		Debug.Log(Logger.Write("resource seed", id));
		PersistentStore.Inst.LoadJsonFromID(id, (obj) => {
			if(null == obj) { Debug.LogError("Check ReloadDt!"); cb(true, false); }
			else {
				foreach (IList fi in (IList)obj) {
					resfiles[fi[1]] = fi[0];
					ressizes[fi[0].ToString()] = long.Parse(fi[2].ToString());
				}
				resID = id;
				cb(false, true);
			}
		});
	}
	
};

}

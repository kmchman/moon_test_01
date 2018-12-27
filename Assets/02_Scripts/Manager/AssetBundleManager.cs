using UnityEngine;
using System.Collections;
using System;

public class AssetBundleManager : MonoBehaviour {

	private static AssetBundleManager _inst = null;
	public static AssetBundleManager Inst { get { return _inst; } }

	public void AsyncGetCharacterAsset(int idCharData, Action<UnityEngine.Object> cb)
	{
		if (!Datatable.Inst.dtCharData.ContainsKey(idCharData))
		{
			cb(null);
			return;
		}

		AsyncGetCharacterAsset(Datatable.Inst.dtCharData[idCharData].PrefabName, cb);
	}

	public void AsyncGetCharacterAsset(string prefabName, Action<UnityEngine.Object> cb)
	{
		if (GameObjectPool.ContainsKey(prefabName))
		{
			cb(GameObjectPool.GetPrefab(prefabName));
		}
		else
		{
			AsyncLoadAssetBundle(prefabName, prefabName, (asset) => {
				cb(asset);
			});
		}
	}

	public void AsyncLoadAssetBundle(string assetBundleName, string assetName, Action<UnityEngine.Object> cb)
	{
		string  assetBundleLowerName = assetBundleName.ToLower();
#if UNITY_EDITOR && USE_ASSETDATABASE
//		string [] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleLowerName, assetName);
//		if (assetPaths.Length != 1)
//		{
//		Debug.LogError(Util.StringFormat("Check AssetBundle Name : {0}({1})", assetBundleLowerName, assetName));
//		cb(null);
//		return;
//		}
//
//		cb(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPaths[0]));
#else
		AddWaitingAssetBundle(assetBundleLowerName, cb);

		AsyncLoadDependencies(assetBundleLowerName, (isResult) => {
			if (!ExistsWaitingAssetBundle(assetBundleLowerName, cb))
			{
				if (isResult)
					UnloadDependencies(assetBundleLowerName);
				return;
			}
			if (!isResult)
			{
				RemoveWaitingAssetBundle(assetBundleLowerName, cb);
				cb(null);
				return;
			}

			AsyncLoadAssetBundleInternal(assetBundleLowerName, (assetBundle) => {
				if (assetBundle == null)
					UnloadDependencies(assetBundleLowerName);
				if (!ExistsWaitingAssetBundle(assetBundleLowerName, cb))
				{
					if (assetBundle != null)
						UnloadAssetBundle(assetBundleLowerName);
					return;
				}
				if (assetBundle == null || string.IsNullOrEmpty(assetName))
				{
					RemoveWaitingAssetBundle(assetBundleLowerName, cb);
					cb(null);
					return;
				}
				StartCoroutine(AsyncLoadAsset(assetBundle, assetName, (obj) => {
					if (!ExistsWaitingAssetBundle(assetBundleLowerName, cb))
						UnloadAssetBundle(assetBundleLowerName);
					else
					{
						RemoveWaitingAssetBundle(assetBundleLowerName, cb);
						cb(obj);
					}
				}));
			});
		});
#endif
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAssetBundles : MonoBehaviour
{
    AssetBundle myLoadedAssetbundles;
    public string path;
    public string testAssetName;

    void Start()
    {
        LoadAssetBundle(path);
        InstantiateObjectFromBundle(testAssetName);
    }

    void LoadAssetBundle(string bundleUrl)
    {
        myLoadedAssetbundles = AssetBundle.LoadFromFile(bundleUrl);
        Debug.Log(myLoadedAssetbundles == null ? "failed to load assetBundles" : "AssetBundle succesfully loaded");

    }

    void InstantiateObjectFromBundle(string assetName)
    {
        var prefab = myLoadedAssetbundles.LoadAsset(assetName);
        Instantiate(prefab);
    }
}

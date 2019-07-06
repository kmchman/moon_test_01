using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAssetBundles : MonoBehaviour
{
    AssetBundle myLoadedAssetbundles;
    public string path;
    public string testAssetName;

    public static LoadAssetBundles Inst;
    
    void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        //LoadAssetBundle(path);
        //InstantiateObjectFromBundle(testAssetName);
    }

    public void LoadTest(string assetBundle, string asset)
    {
        LoadAssetBundle(Constant.TestAssetRoot + assetBundle);
        InstantiateObjectFromBundle(asset);
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

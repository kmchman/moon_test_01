using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;


public class LoadAssetBundles : MonoBehaviour
{
    public class AssetbundleInfo
    {
        public string bundle { get; private set; }
        public Hash128 hash128 { get; private set; }

        public AssetbundleInfo(string bundle, Hash128 hash128)
        {
            this.bundle = bundle;
            this.hash128 = hash128;
        }
    }

    AssetBundle myLoadedAssetbundles;
    public string testAssetName;

    public static LoadAssetBundles Inst;
    private int version = 8;
    private List<AssetbundleInfo> oldAsset = new List<AssetbundleInfo>();
    private List<AssetbundleInfo> newAsset = new List<AssetbundleInfo>();


    public string[] strAssetBundle = new string[] { "assetBundle", "assettest" };

    
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

        System.Action action = () =>
        {
            StartCoroutine(LoadFromCacheOrDownload(Constant.TestAssetRoot + strAssetBundle[0], (manifest) =>
            {
                
                int i = 0;
                foreach (var old in this.oldAsset)
                {

                    Debug.LogFormat("<color=blue>{0}, {1}\t{2}, {3}</color>", old.bundle, old.hash128, this.newAsset[i].bundle, this.newAsset[i].hash128);

                    if (old.hash128 != this.newAsset[i].hash128)
                    {
                        Debug.LogFormat("<color=red>{0}, {1}</color>", this.newAsset[i].bundle, this.newAsset[i].hash128);

                        StartCoroutine(this.SaveAndDownload(Constant.TestAssetRoot + this.newAsset[i].bundle, Application.persistentDataPath + "/AssetBundles/", this.newAsset[i].bundle));

                        StartCoroutine(this.SaveAndDownload(Constant.TestAssetRoot + strAssetBundle[0], Application.persistentDataPath + "/AssetBundles/", strAssetBundle[0]));

                    }

                    i++;
                }
            }));
        };
        StartCoroutine(LoadFromLocal(strAssetBundle[0], action)); 

    }

    //public void LoadTest(string assetBundle, string asset)
    //{

    //    StartCoroutine(LoadFromLocal(strAssetBundle[0]));

    //    StartCoroutine(LoadFromCacheOrDownload(Constant.TestAssetRoot + strAssetBundle[0], (manifest) =>
    //    {

    //        int i = 0;
    //        foreach (var old in this.oldAsset)
    //        {

    //            Debug.LogFormat("<color=red>{0}, {1}\t{2}, {3}</color>", old.bundle, old.hash128, this.newAsset[i].bundle, this.newAsset[i].hash128);

    //            if (old.hash128 != this.newAsset[i].hash128)
    //            {
    //                Debug.LogFormat("<color=red>{0}, {1}</color>", this.newAsset[i].bundle, this.newAsset[i].hash128);

    //                StartCoroutine(this.SaveAndDownload(Constant.TestAssetRoot + this.newAsset[i].bundle, Application.persistentDataPath + "/AssetBundles/", this.newAsset[i].bundle));

    //                StartCoroutine(this.SaveAndDownload(Constant.TestAssetRoot + strAssetBundle[0], Application.persistentDataPath + "/AssetBundles/", strAssetBundle[0]));

    //            }

    //            i++;
    //        }
    //    }));
    //}

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

    IEnumerator SaveAndDownload(string url, string localPath, string fileName)
    {
        var uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();

        //WWW www = new WWW(url);

        //Debug.Log(url);

        //yield return www;

        ////yield return www;
        //byte[] bytes = www.bytes;
        byte[] bytes = uwr.downloadHandler.data;

        Debug.Log("<color=red>" + bytes.Length + "</color>");

        // Create the directory if it doesn't already exist
        if (!Directory.Exists(localPath))
        {
            Directory.CreateDirectory(localPath);
        }

        Debug.Log(localPath);

        File.WriteAllBytes(localPath + fileName, bytes);
    }

    public IEnumerator LoadFromCacheOrDownload(string uri, System.Action<AssetBundleManifest> onComplete)
    {
        var uwr = UnityWebRequestAssetBundle.GetAssetBundle(uri);
        yield return uwr.SendWebRequest();
        AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(uwr);
        
        Debug.Log("assetBundle: " + assetBundle);

        AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        Debug.Log("manifest: " + manifest);

        foreach (var bundle in manifest.GetAllAssetBundles())
        {
            this.newAsset.Add(new AssetbundleInfo(bundle, manifest.GetAssetBundleHash(bundle)));
            Debug.Log("new : " + manifest.GetAssetBundleHash(bundle));
        }
        
        assetBundle.Unload(false);

        onComplete(manifest);

    }

    IEnumerator LoadFromLocal(string assetName, System.Action callBack)
    {
        var path = Path.Combine(Application.persistentDataPath + "/AssetBundles/", assetName);
        Debug.Log(path);

        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(path);
        yield return req;

        var assetBundle = req.assetBundle;

        if (assetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            yield return StartCoroutine(SaveAndDownload(Constant.TestAssetRoot + strAssetBundle[0], Application.persistentDataPath + "/AssetBundles/", strAssetBundle[0]));
            yield return StartCoroutine(SaveAndDownload(Constant.TestAssetRoot + strAssetBundle[1], Application.persistentDataPath + "/AssetBundles/", strAssetBundle[1]));

            req = AssetBundle.LoadFromFileAsync(path);
            yield return req;
            assetBundle = req.assetBundle;
        }

        Debug.Log(assetBundle);

        AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        Debug.Log("manifest: " + manifest);

        foreach (var bundle in manifest.GetAllAssetBundles())
        {
            this.oldAsset.Add(new AssetbundleInfo(bundle, manifest.GetAssetBundleHash(bundle)));

            Debug.LogFormat("Old : {0}, {1}", bundle, manifest.GetAssetBundleHash(bundle));
        }

        assetBundle.Unload(true);
        //var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("ch_02_01");
        //Instantiate(prefab);
        callBack();
    }
}

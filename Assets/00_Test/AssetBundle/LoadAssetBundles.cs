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

    private Dictionary<string, AssetbundleInfo> oldAssetDic = new Dictionary<string, AssetbundleInfo>();
    private Dictionary<string, AssetbundleInfo> newAssetDic = new Dictionary<string, AssetbundleInfo>();

    //private List<AssetbundleInfo> oldAssetList = new List<AssetbundleInfo>();
    //private List<AssetbundleInfo> newAssetList = new List<AssetbundleInfo>();

    Dictionary<string, Object> ObjectMap = new Dictionary<string, Object>();
    List<AssetBundle> assetBundleList = new List<AssetBundle>();


    public string[] strAssetBundle = new string[] { "assetBundle", "assettest" };
    public string rootAssetBundleName = "assetBundle";

    void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        //LoadAssetBundle(path);
        //InstantiateObjectFromBundle(testAssetName);
    }

    public IEnumerator AssetLoad()
    {
        string root = Application.persistentDataPath + "/AssetBundles/";
        //AssetBundle bundle = AssetBundle.LoadFromFile(Constant.TestAssetRoot + strAssetBundle[0]);
        //AssetBundle bundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + strAssetBundle[0]);

        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(Application.streamingAssetsPath + "/" + strAssetBundle[0], 0);
        yield return request.SendWebRequest();
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

        AssetBundleManifest manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        Debug.Log(manifest);
        string[] bundleList = manifest.GetAllAssetBundles();
        foreach (string strAssetBundle in bundleList)
        {
            //AssetBundle _assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + strAssetBundle);
            //AssetBundle _assetBundle = Resources.Load<AssetBundle>(strAssetBundle);
            UnityWebRequest request2 = UnityWebRequestAssetBundle.GetAssetBundle(Application.streamingAssetsPath + "/" + strAssetBundle, 0);
            yield return request2.SendWebRequest();
            AssetBundle _assetBundle = DownloadHandlerAssetBundle.GetContent(request2);


            if (_assetBundle == null)
            {
                AssetBundle _bundle = AssetBundle.LoadFromFile(root + strAssetBundle);
                Debug.Log(_bundle);
                assetBundleList.Add(_bundle);
                GameObject[] objs = _bundle.LoadAllAssets<GameObject>();
                foreach (GameObject obj in objs)
                {
                    Debug.Log("Down obj : " + obj);
                    Instantiate(obj);
                }
            }
            else
            {
                GameObject[] objs = _assetBundle.LoadAllAssets<GameObject>();
                foreach (GameObject obj in objs)
                {
                    Debug.Log("Resource obj : " + obj);
                    Instantiate(obj);
                }
            }
            
        }
    }

    public IEnumerator DownLoadTest()
    {
        CheckOldAsset();

        StartCoroutine(LoadFromCacheOrDownload(Constant.TestAssetRoot + strAssetBundle[0], (manifest) =>
        {
            var enumertor = newAssetDic.GetEnumerator();
            while (enumertor.MoveNext())
            {
                AssetbundleInfo _newAssetInfo = enumertor.Current.Value;
                if (!oldAssetDic.ContainsKey(_newAssetInfo.bundle) || oldAssetDic[_newAssetInfo.bundle].hash128 != _newAssetInfo.hash128)
                {
                    Debug.Log("download : " + _newAssetInfo.bundle);
                    StartCoroutine(this.SaveAndDownload(Constant.TestAssetRoot + _newAssetInfo.bundle, Application.persistentDataPath + "/AssetBundles/", _newAssetInfo.bundle));
                }
            }
        }));

        yield return null;
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
        //var uwr = UnityWebRequestAssetBundle.GetAssetBundle(uri);
        //yield return uwr.SendWebRequest();
        var uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        string localPath = Application.persistentDataPath + "/AssetBundles/";
        if (!Directory.Exists(localPath))
        {
            Directory.CreateDirectory(localPath);
        }

        Debug.Log(localPath);
        byte[] bytes = uwr.downloadHandler.data;
        File.WriteAllBytes(localPath + rootAssetBundleName, bytes);

        //AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(uwr);
        AssetBundle assetBundle = AssetBundle.LoadFromFile(localPath + rootAssetBundleName);
        
        Debug.Log("assetBundle: " + assetBundle);

        AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        Debug.Log("manifest: " + manifest);

        foreach (var bundle in manifest.GetAllAssetBundles())
        {
            newAssetDic.Add(bundle, new AssetbundleInfo(bundle, manifest.GetAssetBundleHash(bundle)));
            Debug.Log("new : " + manifest.GetAssetBundleHash(bundle));
        }

        assetBundle.Unload(false);

        onComplete(manifest);

    }

    private void CheckOldAsset()
    {
        if (File.Exists(Application.persistentDataPath + "/AssetBundles/" + rootAssetBundleName))
        {
            AssetBundle oldAssetBundle = AssetBundle.LoadFromFile(Application.persistentDataPath + "/AssetBundles/" + rootAssetBundleName);

            AssetBundleManifest manifest = oldAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            foreach (var bundle in manifest.GetAllAssetBundles())
            {
                oldAssetDic.Add(bundle, new AssetbundleInfo(bundle, manifest.GetAssetBundleHash(bundle)));
                Debug.LogFormat("Old : {0}, {1}", bundle, manifest.GetAssetBundleHash(bundle));
            }
            oldAssetBundle.Unload(false);
        }
    }

    IEnumerator LoadFromLocal(string assetName, System.Action callBack)
    {
        var path = Path.Combine(Application.persistentDataPath + "/AssetBundles/", assetName);
        Debug.Log(path);
        
        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(path);
        yield return req;

        var assetBundle = req.assetBundle;
        Debug.Log(assetBundle);

        AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        Debug.Log("manifest: " + manifest);

        foreach (var bundle in manifest.GetAllAssetBundles())
        {
            //this.oldAssetList.Add(new AssetbundleInfo(bundle, manifest.GetAssetBundleHash(bundle)));
            Debug.LogFormat("Old : {0}, {1}", bundle, manifest.GetAssetBundleHash(bundle));
        }

        assetBundle.Unload(true);
        //var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("ch_02_01");
        //Instantiate(prefab);
        callBack();
    }
}

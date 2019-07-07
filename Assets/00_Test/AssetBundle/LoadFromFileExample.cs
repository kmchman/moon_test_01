using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Linq;


public class LoadFromFileExample : MonoBehaviour
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
    private string assetBundleBaseUrl = "ftp://121.161.123.7:2122/";
    private int version = 2;
    private List<AssetbundleInfo> oldAsset = new List<AssetbundleInfo>();
    private List<AssetbundleInfo> newAsset = new List<AssetbundleInfo>();

    public static void CleanAssetBundleCache()
    {
        Debug.Log(Caching.ClearCache() ? "Successfully removed Cache" : "Cache in use");
    }

    void Start()
    {

        //CleanAssetBundleCache(); return;

        //신규유저일경우 로드 후 로컬 저장 
        //StartCoroutine(this.SaveAndDownload(assetBundleBaseUrl + "StandaloneWindows", Application.persistentDataPath + "/AssetBundles/", "StandaloneWindows.unity3d"));
        //StartCoroutine(this.SaveAndDownload(assetBundleBaseUrl + "character", Application.persistentDataPath + "/AssetBundles/", "character.unity3d"));
        //StartCoroutine(this.SaveAndDownload(assetBundleBaseUrl + "hat", Application.persistentDataPath + "/AssetBundles/", "hat.unity3d"));
        //StartCoroutine(this.SaveAndDownload(assetBundleBaseUrl + "weapon", Application.persistentDataPath + "/AssetBundles/", "weapon.unity3d"));




        //로컬
        StartCoroutine(this.LoadFromLocal("StandaloneWindows.unity3d"));

        //서버 

        StartCoroutine(LoadFromCacheOrDownload(assetBundleBaseUrl + "StandaloneWindows", (manifest) => {

            int i = 0;
            foreach (var old in this.oldAsset)
            {

                //Debug.LogFormat("<color=red>{0}, {1}\t{2}, {3}</color>", old.bundle, old.hash128, this.newAsset[i].bundle, this.newAsset[i].hash128);

                if (old.hash128 != this.newAsset[i].hash128)
                {
                    Debug.LogFormat("<color=red>{0}, {1}</color>", this.newAsset[i].bundle, this.newAsset[i].hash128);

                    StartCoroutine(this.SaveAndDownload(assetBundleBaseUrl + this.newAsset[i].bundle, Application.persistentDataPath + "/AssetBundles/", this.newAsset[i].bundle + ".unity3d"));

                    StartCoroutine(this.SaveAndDownload(assetBundleBaseUrl + "StandaloneWindows", Application.persistentDataPath + "/AssetBundles/", "StandaloneWindows.unity3d"));

                }

                i++;
            }
        }));




    }


    IEnumerator LoadFromLocal(string assetName)
    {
        var path = Path.Combine(Application.persistentDataPath + "/AssetBundles/", assetName);
        Debug.Log(path);

        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(path);
        yield return req;

        var assetBundle = req.assetBundle;

        if (assetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            yield break;
        }

        Debug.Log(assetBundle);

        AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        Debug.Log("manifest: " + manifest);

        foreach (var bundle in manifest.GetAllAssetBundles())
        {
            this.oldAsset.Add(new AssetbundleInfo(bundle, manifest.GetAssetBundleHash(bundle)));

            Debug.LogFormat("{0}, {1}", bundle, manifest.GetAssetBundleHash(bundle));
        }

        assetBundle.Unload(true);
        //var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("ch_02_01");
        //Instantiate(prefab);
    }

    IEnumerator LoadAssetBundle(string uri, System.Action onComplete)
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri, 0);
        //var request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();
        
        //Debug.LogFormat("{0}, {1}", request.isHttpError, request.isNetworkError);

        AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(request);

        Debug.Log(assetBundle);

        Debug.Log("assetBundle: " + assetBundle);

        AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");


        foreach (var bundle in manifest.GetAllAssetBundles())
        {
            Debug.LogFormat("{0}", manifest.GetAssetBundleHash(bundle));


            this.newAsset.Add(new AssetbundleInfo(bundle, manifest.GetAssetBundleHash(bundle)));
        }



        assetBundle.Unload(false);

        onComplete();
    }

    public IEnumerator LoadFromCacheOrDownload(string uri, System.Action<AssetBundleManifest> onComplete)
    {
        using (WWW www = WWW.LoadFromCacheOrDownload(uri, version))
        {
            yield return www;

            AssetBundle assetBundle = www.assetBundle;

            Debug.Log("assetBundle: " + assetBundle);

            AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");


            Debug.Log("manifest: " + manifest);

            foreach (var bundle in manifest.GetAllAssetBundles())
            {
                this.newAsset.Add(new AssetbundleInfo(bundle, manifest.GetAssetBundleHash(bundle)));
                Debug.Log(manifest.GetAssetBundleHash(bundle));
            }


            assetBundle.Unload(false);

            onComplete(manifest);
        }
    }
    IEnumerator SaveAndDownload(string url, string localPath, string fileName)
    {
        WWW www = new WWW(url);

        Debug.Log(url);

        yield return www;

        //yield return www;
        byte[] bytes = www.bytes;

        Debug.Log("<color=red>" + bytes.Length + "</color>");

        // Create the directory if it doesn't already exist
        if (!Directory.Exists(localPath))
        {
            Directory.CreateDirectory(localPath);
        }

        Debug.Log(localPath);

        File.WriteAllBytes(localPath + fileName, bytes);
    }


}

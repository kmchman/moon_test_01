using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class AssetBundleTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AssetLoad());
    }

    public IEnumerator AssetLoad()
    {
        //string root = Application.persistentDataPath + "/AssetBundles/";
        //AssetBundle bundle = AssetBundle.LoadFromFile(Constant.TestAssetRoot + strAssetBundle[0]);
        //AssetBundle bundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + strAssetBundle[0]);
        string assetFolderPath = string.Format("{0}/{1}", Application.persistentDataPath, "StandaloneWindows");
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(string.Format("{0}/{1}", assetFolderPath, "StandaloneWindows"), 0);
        yield return request.SendWebRequest();
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

        AssetBundleManifest manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        
        string[] bundleList = manifest.GetAllAssetBundles();
        foreach (string strAssetBundle in bundleList)
        {
            //AssetBundle _assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + strAssetBundle);
            //AssetBundle _assetBundle = Resources.Load<AssetBundle>(strAssetBundle);
            UnityWebRequest request2 = UnityWebRequestAssetBundle.GetAssetBundle(string.Format("{0}/{1}", assetFolderPath, strAssetBundle), 0);
            yield return request2.SendWebRequest();
            AssetBundle _assetBundle = DownloadHandlerAssetBundle.GetContent(request2);

            if (_assetBundle == null)
            {
                //AssetBundle _bundle = AssetBundle.LoadFromFile(root + strAssetBundle);
                //Debug.Log(_bundle);
                //assetBundleList.Add(_bundle);
                //GameObject[] objs = _bundle.LoadAllAssets<GameObject>();
                //foreach (GameObject obj in objs)
                //{
                //    Debug.Log("Down obj : " + obj);
                //    Instantiate(obj);
                //}
            }
            else
            {
                GameObject[] objs = _assetBundle.LoadAllAssets<GameObject>();
                foreach (GameObject obj in objs)
                {
                    Debug.Log("Resource obj : " + obj);
                    Instantiate(obj);
                    Instantiate(obj);
                    Instantiate(obj);
                }
            }

        }
    }
}

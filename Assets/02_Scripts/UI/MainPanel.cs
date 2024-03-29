﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System;
using UnityEngine.Networking;
using Table;

public class MainPanel : MonoBehaviour
{
    [SerializeField] private TestPopup02 testPopup02Prefab;
    [SerializeField] private Text titleMessage;

    private string url;

    class BuildingData
    {
        public int x;
        public int y;
        public int z;
        public BuildingData(int _x, int _y, int _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
    }
    [SerializeField] private GameObject titleImage;

    private void OnEnable()
    {
        titleImage.SetActive(false);
    }

    BuildingData[] buildingData = new BuildingData[] {
        new BuildingData(1, 3, 5),
        new BuildingData(2, 3, 6),
        new BuildingData(7, 3, 5),
        new BuildingData(2, 3, 8),
        new BuildingData(4, 3, 1),
        new BuildingData(3, 3, 6),
    };
    private List<GameObject> buildingList = new List<GameObject>();

    public void CreateObject(int x, int y, int z)
    {
        GameObject buildingObj = Util_GO.MakeGameObjectFromPrefab("Model/BuildingHouse_C", false);
        buildingObj.transform.position = new Vector3(x, y, z);
        buildingList.Add(buildingObj);
    }

    public void CreateBuildingRand()
    {
        
    }

    public void RemoveObject()
    {
        Debug.Log("LoadAssetBundles.Inst.AssetLoad()");

        LoadAssetBundles.Inst.StartCoroutine("AssetLoad");
    }

    private void LoadTable()
    {
        string tableFolerPath = string.Format("{0}/{1}", Application.dataPath, Constant.TablePath);
        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(tableFolerPath);

        foreach (System.IO.FileInfo file in di.GetFiles())
        {
            string onlyFileName = file.Name.Substring(0, file.Name.Length - 5);

            if (file.Extension.ToLower().CompareTo(".json") == 0)
            {
                Debug.Log("fileName : " + file.Name);

                Type tableClass = Type.GetType("Table." + onlyFileName);
                MethodInfo loadMethod = tableClass.GetMethod("LoadFromJsonFile");
                loadMethod.Invoke(null, new object[] { string.Format("{0}/{1}/{2}", Application.dataPath, Constant.TablePath, file.Name) });
                //Table.tb_Building_Base.LoadFromJsonFile(string.Format("{0}/{1}/{2}", Application.dataPath, Constant.TablePath, "tb_Building_Base.json"));
            }
        }
    }

    private IEnumerator StartDownload()
    {
        
        var uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();
        Debug.Log("StartDownload : " + url);
        if (!uwr.isNetworkError && !uwr.isHttpError)
        {
            titleMessage.text = "sccess";
        }
        else
        {
            titleMessage.text = "fail url : " + url;
            Debug.Log(uwr.isNetworkError);
        }

    }

    public void OnClickBtnCreate()
    {
        url = string.Format("http://abyssworld-cdn.flerogamessvc.com/Internal/Internal{0}", "_AOS.json");
        
        StopCoroutine("StartDownload");
        StartCoroutine("StartDownload");
        //LoadTable();
        //Table.tb_Building_Base.LoadFromJsonFile(string.Format("{0}/{1}/{2}", Application.dataPath, Constant.TablePath,"tb_Building_Base.json"));
        //Debug.Log("Table.tb_Building_Base.map.Count" + Table.tb_Building_Base.map.Count);

        //string csvPath = Application.streamingAssetsPath + "/csvTest1.csv";
        //string jsonPath = Application.streamingAssetsPath + "/csvTest1.json";
        //string json = Giant.Util.ConvertCsvFileToJsonObject(csvPath);
        //Debug.Log("ConvertCsvFileToJsonObject" + json);
        //System.IO.File.WriteAllText(jsonPath, json);

        //MoonGlobalPopupManager.Inst.ShowPopup(testPopup02Prefab, null);
        //Debug.Log("LoadAssetBundles.Inst.LoadTest();");
        //MainController.Inst.CreateBaseMap();

        //LoadAssetBundles.Inst.StopCoroutine("DownLoadTest");
        //LoadAssetBundles.Inst.StartCoroutine("DownLoadTest");
    }
    //public void CreateBuildingRand()
    //{
    //    int x = UnityEngine.Random.Range(0, 5);
    //    int y = UnityEngine.Random.Range(3, 4);
    //    int z = UnityEngine.Random.Range(0, 5);

    //    GameObject buildingObj = Util_GO.MakeGameObjectFromPrefab("Model/BuildingDeco03", false);
    //    buildingObj.transform.position = new Vector3(x, y, z);
    //    buildingList.Add(buildingObj);
    //}

    //public void RemoveObject()
    //{
    //    for(int i = 0; i < buildingList.Count; i++)
    //        GameObject.Destroy(buildingList[i].gameObject);
    //    buildingList.Clear();
    //}

    //public void OnClickBtnCreate()
    //{
    //    for (int i = 0; i < buildingData.Length; i++)
    //    {
    //        CreateObject(buildingData[i].x, buildingData[i].y, buildingData[i].z);
    //    }
    //}

    public void OnClickBtnRemove()
    {
        url = string.Format("https://abyssworld-cdn.flerogamessvc.com/Internal/Internal{0}", "_AOS.json");

        StopCoroutine("StartDownload");
        StartCoroutine("StartDownload");
        //RemoveObject();
        //MoonGlobalPopupManager.Inst.ShowPopup("Popup_Test02");
    }

    public void OnClickBtnChange()
    {
        url = string.Format("http://abyssworld-cdn.flerogamessvc.com/Internal/Internal{0}", "_AOS.json");

        StopCoroutine("StartDownload");
        StartCoroutine("StartDownload");
        //RemoveObject();
        //for (int i = 0; i < 10; i++)
        //{
        //    CreateBuildingRand();
        //}
    }
}

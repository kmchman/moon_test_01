using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanel : MonoBehaviour
{
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

    public void OnClickBtnCreate()
    {
        Debug.Log("LoadAssetBundles.Inst.LoadTest();");
        MainController.Inst.CreateBaseMap();

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
        RemoveObject();
    }

    public void OnClickBtnChange()
    {
        RemoveObject();
        for (int i = 0; i < 10; i++)
        {
            CreateBuildingRand();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public Transform areaGroup;
    public static MainController Inst;

    void Awake()
    {
        Inst = this;
    }

    public void CreateBaseMap()
    {
#if UNITY_EDITOR
        string path = "Assets/00_Test/MapTest/Abyss_Main_01_new_map.prefab";

        GameObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

        if (obj != null)
        {
            GameObject go = Instantiate(obj) as GameObject;

            Vector3 originPos = go.transform.position;
            go.transform.SetParent(areaGroup);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = originPos;
        }
#endif
    }
}

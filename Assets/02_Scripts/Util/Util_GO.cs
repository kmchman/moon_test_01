using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util_GO : MonoBehaviour
{

    public static T MakeGameObjectFromPrefab<T>(string relativePath, bool resetTrans = true) where T : MonoBehaviour
    {
        GameObject go = MakeGameObjectFromPrefab(relativePath, resetTrans);
        return go.GetComponent<T>();
    }

    //=========================================================================
    //: desc    :   
    //=========================================================================
    public static GameObject MakeGameObjectFromPrefab(string relativePath, bool resetTrans = true)
    {
        Object prefab = Resources.Load(relativePath);
        if (prefab == null)
            return null;

        GameObject newObject = Object.Instantiate(prefab) as GameObject;

        if (resetTrans == true)
            InitTransform(newObject);

        return newObject;
    }

    public static GameObject MakeGameObject(string objectName, GameObject parent, bool dontDestroyOnLoad)
    {
        GameObject TempObject = new GameObject(objectName);

        if (dontDestroyOnLoad == true)
        {
            GameObject.DontDestroyOnLoad(TempObject);
        }

        if (parent != null)
        {
            SetParent(TempObject, parent, false);
        }

        return TempObject;
    }

    //=========================================================================
    //: desc    :   
    //=========================================================================
    public static void SetParent(GameObject obj, GameObject parent)
    {
        SetParent(obj, parent, false);
    }

    //=========================================================================
    //: desc    :   
    //=========================================================================
    public static void SetParent(GameObject obj, GameObject parent, bool resetTrans)
    {
        SetParent(obj, parent, resetTrans, false);
    }

    //=========================================================================
    //: desc    :   
    //=========================================================================
    public static void SetParent(GameObject obj, GameObject parent, bool resetTrans, bool applyParentLayer)
    {
        if (obj == null || parent == null)
            return;

        Transform t = obj.transform;

        if (applyParentLayer == true)
        {
            SetLayer(obj, parent.layer);
        }


        if (resetTrans == false)
        {
            Vector3 p = t.localPosition;
            Quaternion r = t.localRotation;
            Vector3 s = t.localScale;
            t.SetParent(parent.transform);
            //t.parent = parent.transform;
            t.localPosition = p;
            t.localRotation = r;
            t.localScale = s;
        }
        else
        {
            t.parent = parent.transform;
            InitTransform(obj);
        }
    }

    static public void SetLayer(GameObject go, int layer)
    {
        go.layer = layer;

        Transform t = go.transform;

        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            SetLayer(child.gameObject, layer);
        }
    }

    //=========================================================================
    //: desc    :   Instantiate an object and add it to the specified parent.
    //=========================================================================
    static public GameObject MakeAndAddChild(GameObject parent, GameObject prefab, bool resetTrans, bool applyParentLayer)
    {
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        SetParent(go, parent, resetTrans, applyParentLayer);
        return go;
    }

    //=========================================================================
    //: desc    :   Init GameObject Transform
    //=========================================================================
    public static void InitTransform(GameObject _obj)
    {
        if (_obj == null) return;

        Transform t = _obj.transform;

        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
    }
}

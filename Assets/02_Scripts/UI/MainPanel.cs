using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanel : MonoBehaviour
{
    public void CreateObject()
    {
        GameObject rootGO = Util_GO.MakeGameObjectFromPrefab("Model/Building_Factory_04", false);
        rootGO.transform.position = new Vector3(0, 0, 0);
    }

    public void RemoveObject()
    {

    }

    public void OnClickBtnCreate()
    {
        CreateObject();
    }

    public void OnClickBtnRemove()
    {

    }

    public void OnClickBtnChange()
    {

    }
}

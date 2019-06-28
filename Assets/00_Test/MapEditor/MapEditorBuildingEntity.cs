using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    public class MapEditorBuildingEntity : MonoBehaviour
    {
        public BuildingData data;

        public GameObject meshObj;

        public void Startup(GameObject _obj)
        {
            meshObj = _obj;
        }

        public void Rotate()
        {
            data.rotY++;
            if (data.rotY == 4)
                data.rotY = 0;
            meshObj.transform.localEulerAngles = Vector3.up * 90 * data.rotY;
        }

        public void SetTilePos(int x, int y)
        {
            data.building_TilePos[0] = x;
            data.building_TilePos[1] = y;
        }

        public void Initialize(int uid , int tid, int x, int y , int roty, int itemID)
        {
            data = new BuildingData();
            data.building_UID = uid;
            data.building_TID = tid;
            data.building_TilePos = new int[2];
            data.building_TilePos[0] = x;   
            data.building_TilePos[1] = y;
            data.rotY = roty;
            data.cropID = itemID;
        }
    }
}
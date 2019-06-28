using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using NN;
namespace MapEditor
{
    public class MapEditorObjectFactory : MonoBehaviour
    {
        private const string stonePath = "Entity/Stone/";
        private const string buildingPath = "Entity/Structure/";
        private const string areaPath = "Entity/Area/Area_02";

        public Transform buildingGroup;
        public Transform areaGroup;
        private int generatedUID = 0;

        public int GenerateUID()
        {
            generatedUID++;
            return generatedUID;
        }

        public void ResetWorld(NPCData data)
        {
            StartCoroutine(ResetRoutine(data));
        }

        private IEnumerator ResetRoutine(NPCData data)
        {
            int areaCount = areaGroup.childCount;
            int buildingCount = buildingGroup.childCount;

            for(int i= areaCount - 1; i>=0; i--)
            {
                Destroy(areaGroup.GetChild(i).gameObject);
            }

            for (int i = buildingCount - 1; i >= 0; i--)
            {
                Destroy(buildingGroup.GetChild(i).gameObject);
            }

            yield return null;
            LoadAbyssworld(data);
        }
        public void LoadAbyssworld(NPCData data)
        {
            LoadMapBase();
            LoadArea();
            LoadBuilding(data.buildingArray);
            LoadFish(data.fishArray);
        }

        private void LoadMapBase()
        {
            string path = "Assets/Game_Asset/Model/New_buildings/P_Terrain/Abyss_Main_01_new_map.prefab";

            GameObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

            if (obj != null)
            {
                GameObject go = Instantiate(obj) as GameObject;

                Vector3 originPos = go.transform.position;
                go.transform.SetParent(areaGroup);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = originPos;
            }
        }

        private void LoadArea()
        {
            var expandTable = MapEditorManager.Instance.tableDataManager.GetTable<ExpandmapTable>();

            var expandEnumerator = expandTable.GetExpandContainer().GetEnumerator();

            while (expandEnumerator.MoveNext())
            {
                var expandData = expandEnumerator.Current.Value;
                GameObject go = NN.Util.GO.MakeGameObjectFromPrefab(areaPath);

                go.transform.SetParent(areaGroup);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = new Vector3(expandData.posX, 0, expandData.posY);
            }
        }

        private void LoadBuilding(BuildingData[] buildingList)
        {
            if (buildingList == null)
                return;
            for(int i=0; i <buildingList.Length; i++)
            {
                BuildingData data = buildingList[i];
                CreateBuilding(data.building_UID, data.building_TID, data.building_TilePos[0], data.building_TilePos[1], data.rotY, data.cropID);
            }
        }

        private void LoadFish(FishData[] fishArray )
        {
            if (fishArray == null)
                return;
            for (int i = 0; i < fishArray.Length; i++)
            {
                FishData data = fishArray[i];
                CreateFish(data.fish_TID, data.fish_Count, data.fish_Zone);
            }
        }

        public void CreateFish(int id, int count , int zone)
        {

        }

        public MapEditorBuildingEntity CreateBuilding(int id, int x, int y, int roty, int itemID)
        {
            return CreateBuilding(GenerateUID(), id, x, y, roty, itemID);
        }

        public MapEditorBuildingEntity CreateBuilding(int uid , int id, int x, int y, int roty, int itemID)
        {
            var buildingData = MapEditorManager.Instance.buildingTable.GetBaseBuildingData(id);

            if (buildingData == null)
                return null;

            string fullPath = "";
            switch (buildingData.buildingType)
            {
                case SQLite.Data.eBuildingType.Stone:
                    fullPath = stonePath + buildingData.resName;
                    break;
                default:
                    fullPath = buildingPath + buildingData.resName;
                    break;
            }

            GameObject parent = new GameObject(buildingData.resName);
            parent.transform.SetParent(buildingGroup);
            parent.transform.localPosition = new Vector3(x, 0, y);

            GameObject go = NN.Util.GO.MakeGameObjectFromPrefab(fullPath);
            go.transform.SetParent(parent.transform);
            float _sizeX = buildingData.sizeX;
            float _sizeY = buildingData.sizeY;
            Vector3 localPos = Vector3.zero;
            localPos.x = ((_sizeX - 1) / 2) - 0.5f;
            localPos.z = ((_sizeY - 1) / 2) - 0.5f;

            go.transform.localPosition = localPos;
            go.transform.localEulerAngles = Vector3.up * 90 * roty;
            NN.Util.GO.SetLayer(parent, LayerMask.NameToLayer("Building"));

            MapEditorBuildingEntity entity = parent.AddComponent<MapEditorBuildingEntity>();
            entity.Startup(go);
            entity.Initialize(GenerateUID(), id, x, y, roty, itemID);
            
            return entity;
        }
    }
}
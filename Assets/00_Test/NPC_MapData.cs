using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace MapEditor
{
    [System.Serializable]
    public class BuildingData
    {
        public int building_UID;
        public int building_TID;
        public int[] building_TilePos;
        public int rotY;
        public int cropID;
    }

    [System.Serializable]
    public class AreaData
    {
        public int[] area_TIlePos;
        public int open;
    }

    [System.Serializable]
    public class FishData
    {
        public int fish_TID;
        public int fish_Count;
        public int fish_Zone;
    }

    public class NPCData
    {
        public int npcID;
        public string npcName;
        public BuildingData[] buildingArray;
        public AreaData[] areaArray;
        public FishData[] fishArray;
    }

    [System.Serializable]
    public class NPCDataManager
    {
        public List<BuildingData> buildingDatas;
        public List<AreaData> areaDatas;
        public List<FishData> fishDatas;

        public NPCData npcData;

        public NPCDataManager()
        {
            npcData = new NPCData();
            buildingDatas = new List<BuildingData>();
            areaDatas = new List<AreaData>();
            fishDatas = new List<FishData>();
        }

        public void AddBuildingData(BuildingData data)
        {
            buildingDatas.Add(data);
        }
        public void RemoveBuildingData(BuildingData data)
        {
            for(int i=0; i< buildingDatas.Count; i++)
            {
                if(buildingDatas[i].building_UID == data.building_UID)
                    buildingDatas.Remove(data);
            }
        }

        public void LoadFromJson(string path)
        {
            string json = File.ReadAllText(path);
            npcData = JsonUtility.FromJson<NPCData>(json);
        }

        public string ToJson()
        {

            npcData.buildingArray = buildingDatas.ToArray();
            npcData.areaArray = areaDatas.ToArray();
            npcData.fishArray = fishDatas.ToArray();
            
            string jsonData = JsonUtility.ToJson(npcData, true);

            return jsonData;
            //File.WriteAllText(Application.dataPath + "/NPC_DATA.json", jsonData);
        }
    }
}
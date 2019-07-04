//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using Game;
//using NN;

//namespace MapEditor
//{
    
//    public class MapEditorManager : MonoBehaviour
//    {

        
//        public static MapEditorManager Instance
//        {
//            get { return _instance; }
//        }

//        private static MapEditorManager _instance;

//        public TableDataManager tableDataManager;
//        public SystemManager systemManager;
//        public GameSystemManager gameSystemManager;
//        public SQLiteSystem sqliteSystem;

//        public NPCDataManager npcDataManager;

//        public MapEditorEditController editController;
//        public MapEditorObjectFactory objectFactory;

//        public MapEditorUIBuildingList buildingListUI;

//        public Building_Table buildingTable;
//        public FishTable fishTable;
//        public GUITable guiTable;

//        private void Init_System()
//        {
//            systemManager = gameObject.AddComponent<SystemManager>();
//            systemManager.Initialize();

//            gameSystemManager = gameObject.AddChild<GameSystemManager>();
//            gameSystemManager.Initialize();

//            sqliteSystem = systemManager.MakeMonoSystem<SQLiteSystem>();

//            sqliteSystem.Startup(true);
//            sqliteSystem.Initialize();
//        }

//        private void Init_Table()
//        {
//            tableDataManager = new TableDataManager();
//            tableDataManager.Initialize();

//            buildingTable = tableDataManager.MakeTable<Building_Table>();
//            buildingTable.Initialize();

//            fishTable = tableDataManager.MakeTable<FishTable>();
//            fishTable.Initialize();

//            guiTable = tableDataManager.MakeTable<GUITable>();
//            guiTable.Initialize();

//            var expandTable = tableDataManager.MakeTable<ExpandmapTable>();
//            expandTable.Initialize();
//        }

//        private void Initialize()
//        {
//            _instance = this;
//            Init_System();
//            Init_Table();
//            StartCoroutine(Prepare_DB());
//        }
        
//        void Start()
//        {
//            Initialize();
//        }

//        private IEnumerator Prepare_DB()
//        {
//            var sql = systemManager.GetSystem<SQLiteSystem>();

//            yield return StartCoroutine(sql.PrepareProcess());
//            sql.Open();
//            tableDataManager.Build();

//            npcDataManager = new NPCDataManager();

//            buildingListUI.Initialize();

//            editController.Initialize();
//            objectFactory.LoadAbyssworld(npcDataManager.npcData);
//        }

//        public void CreateFish()
//        {

//        }

//        public void CreateBuilding(int id)
//        {
//            var obj = objectFactory.CreateBuilding(id, 0, 0, 0, 0);
//            if(obj != null)
//            {
//                npcDataManager.AddBuildingData(obj.data);
//            }
//        }

//        public void DeleteBuilding(GameObject go, MapEditorBuildingEntity entity)
//        {
//            npcDataManager.RemoveBuildingData(entity.data);
//            Destroy(go);
//        }

//        public void OnClickSave()
//        {
//            string json = npcDataManager.ToJson();

//            string path = EditorUtility.SaveFilePanel("Json 파일 저장하기", "","NPC_DATA.json", "json");
//            if(path.Length != 0)
//            {
//                System.IO.File.WriteAllText(path, json);
//            }
//        }

//        public void OnClickLoad()
//        {
//            string path = UnityEditor.EditorUtility.OpenFilePanel("Json 파일 불러오기", "", "json");
//            if (path.Length != 0)
//            {
//                npcDataManager.LoadFromJson(path);

//                objectFactory.ResetWorld(npcDataManager.npcData);
//            }
//        }

//        public void OnClickShowBuildingList()
//        {
//            buildingListUI.Open();
//        }

//        public void OnClickShowFishList()
//        {

//        }
//    }
//}
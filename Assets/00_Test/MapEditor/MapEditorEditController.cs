using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace MapEditor
{
    public class MapEditorEditController : MonoBehaviour
    {
        public UIEditPanelForEditorModeWindow editPanel;
        public MapEditorController controller;
        public Camera gameCamera { private set; get; }
        public TouchInputController touchInputController;
        public MobileTouchCamera touchCamera;

        public LayerMask layerMask;

        private RaycastHit hit;

        private const string resourceFullPath = "Camera/GameCamera";
        private GameObject selectObj;
        private MapEditorBuildingEntity selectedEntity;

        public float val_Snap = 1;
        float yy = 0.01f;

        private Vector2 startMousePos;
        private Vector3 pos_Grid;
        private Vector3 pos_result;

        protected int[] m_Grid_Result = new int[2];

        public int[] Grid_Result
        {
            get { return m_Grid_Result; }
        }
        private float Grid_val_Snap = 1;

        public event SelectObject EventSelectObject;
        public delegate void SelectObject(GameObject go);

        public bool canDrag = false;
        public void Initialize()
        {
            GameObject camObj = NN.Util.GO.MakeGameObjectFromPrefab(resourceFullPath);
            gameCamera = Camera.main;

            touchInputController = gameCamera.GetComponent<TouchInputController>();
            touchCamera = gameCamera.GetComponent<MobileTouchCamera>();
            
            editPanel.Initialize();

            AddEventListener();
        }

        private void AddEventListener()
        {
            controller.EventInputDown += OnEventInputDown;
            controller.EventInputClick += OnEventInputClick;
            controller.EventInputDrag += OnEventInputDrag;
        }

        private void OnEventInputDown(Vector3 pos)
        {
            Ray ray = gameCamera.ScreenPointToRay(pos);
            canDrag = false;
            if (Physics.Raycast(ray, out hit, 500f, layerMask))
            {
                if(hit.collider.transform.parent.gameObject == selectObj)
                {
                    canDrag = true;
                }
                else
                {
                    selectObj = hit.collider.transform.parent.gameObject;
                    selectedEntity = selectObj.GetComponent<MapEditorBuildingEntity>();
                    
                }
                
                touchCamera.OnTouchBlock(true);
            }
        }

        private void OnEventInputClick(Vector3 pos)
        {
            Ray ray = gameCamera.ScreenPointToRay(pos);

            if (Physics.Raycast(ray, out hit, 500f, layerMask))
            {
                if (selectObj != null)
                {
                    if (EventSelectObject != null)
                        EventSelectObject(selectObj);
                }
            }
            touchCamera.OnTouchBlock(false);
        }

        private void OnEventInputDrag(Vector3 pos)
        {
            if (!canDrag)
                return;
            Ray ray = gameCamera.ScreenPointToRay(pos);

            if (Physics.Raycast(ray, out hit, 500f, LayerMask.NameToLayer("Terrain")))
            {
                if (selectObj != null)
                {
                    Vector3 _pos = Get_TouchGridPos(hit.point);
                    

                    selectObj.transform.position = _pos;

                    selectedEntity.SetTilePos((int)_pos.x, (int)_pos.z);
                }
            }
        }

        public Vector3 Get_TouchGridPos(Vector3 position)
        {
            pos_Grid.x = RoundSnap(position.x);
            pos_Grid.y = RoundSnap(position.y);
            pos_Grid.z = RoundSnap(position.z);

            Vector3 gridpos = pos_Grid;
            pos_result.x = gridpos.x;
            pos_result.y = yy;
            pos_result.z = gridpos.z;
            m_Grid_Result[0] = (int)(pos_Grid.x * Grid_val_Snap);
            m_Grid_Result[1] = (int)(pos_Grid.z * Grid_val_Snap);
            return pos_result;
        }

        private float RoundSnap(float _input)
        {
            float snap_Val = val_Snap * Mathf.Round((_input / val_Snap));
            int DataGrd = (int)Mathf.Round(snap_Val);
            return DataGrd;
        }

        public void ReleaseObj()
        {
            selectObj = null;
        }
    }
}
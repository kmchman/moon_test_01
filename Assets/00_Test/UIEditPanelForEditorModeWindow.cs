//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace MapEditor
//{
//    public class UIEditPanelForEditorModeWindow : MonoBehaviour
//    {
//        private MapEditorBuildingEntity selectObj;

//        public void Initialize()
//        {
//            MapEditorManager.Instance.editController.EventSelectObject += OnEventSelect;
//        }

//        public void OnEventSelect(GameObject go)
//        {
//            selectObj = go.GetComponent<MapEditorBuildingEntity>();
//            Open();
//        }

//        public void Open()
//        {
//            gameObject.SetActive(true);
//        }

//        public void OnClickConfirm()
//        {

//        }

//        public void OnClickRotate()
//        {
//            if (selectObj != null)
//                selectObj.Rotate();
//        }

//        public void OnClickDelete()
//        {
//            MapEditorManager.Instance.DeleteBuilding(selectObj.gameObject, selectObj);
//            OnClickClose();
//        }

//        public void OnClickClose()
//        {
//            selectObj = null;
//            gameObject.SetActive(false);
//            MapEditorManager.Instance.editController.ReleaseObj();
//        }
//    }
//}
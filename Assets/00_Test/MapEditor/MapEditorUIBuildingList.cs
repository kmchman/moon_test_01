using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    public class MapEditorUIBuildingList : MonoBehaviour
    {
        public MapEditorUIBuildingObject buildingObject;
        public RectTransform content;

        public void Initialize()
        {
            var enumertor = MapEditorManager.Instance.buildingTable.GetBaseBuildingContainer().GetEnumerator();

            while(enumertor.MoveNext())
            {
                var data = enumertor.Current.Value;

                GameObject go = Instantiate(buildingObject.gameObject) as GameObject;
                go.transform.SetParent(content);
                go.transform.localScale = Vector3.one;
                MapEditorUIBuildingObject uiObject = go.GetComponent<MapEditorUIBuildingObject>();
                uiObject.Initialize(data.id, data.iconID);
            }
            buildingObject.gameObject.SetActive(false);
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void OnClickClose()
        {
            gameObject.SetActive(false);
        }
    }
}
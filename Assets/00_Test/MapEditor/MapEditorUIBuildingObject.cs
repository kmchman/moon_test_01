using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor
{
    public class MapEditorUIBuildingObject : MonoBehaviour
    {
        public int TID;
        public Image image_Icon;

        public void Initialize(int tid , int iconID)
        {
            TID = tid;
            var data = MapEditorManager.Instance.guiTable.GetIconData(iconID);

            if (data == null)
            {
                gameObject.SetActive(false);
                return;
            }
            Sprite sprite = NN.Util.GUI.LoadSprite(data.path + data.resName);
            if(sprite != null)
            {
                image_Icon.sprite = sprite;
            }
        }

        public void OnClick()
        {
            MapEditorManager.Instance.CreateBuilding(TID);
        }
    }
}
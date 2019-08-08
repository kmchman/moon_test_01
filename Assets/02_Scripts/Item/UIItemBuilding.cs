using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemBuilding : STScrollRectItem
{
    public int TID;
    public Image image_Icon;

    public void SetData(int itemID)
    {

        //var sp = Resources.Load(_path) as Sprite;
    }

    public void SetData(int tid, int iconID)
    {
        //TID = tid;
        //var data = MapEditorManager.Instance.guiTable.GetIconData(iconID);

        //if (data == null)
        //{
        //    gameObject.SetActive(false);
        //    return;
        //}
        //Sprite sprite = Util_GO.LoadSprite(data.path + data.resName);
        //if (sprite != null)
        //{
        //    image_Icon.sprite = sprite;
        //}
    }
}
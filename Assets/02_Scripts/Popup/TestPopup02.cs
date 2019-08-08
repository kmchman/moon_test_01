using UnityEngine;
using System.Collections;

public class TestPopup02 : MoonPopupUI {

    [SerializeField] private STScrollRect m_ScrollRect;
    [SerializeField] private UIItemBuilding uiBuildingItemPrefab;
    protected override void Open(params object[] values)
    {
        var enumertor = Table.tb_Building_Base.map.GetEnumerator();
        while (enumertor.MoveNext())
        {
            Debug.Log("enumertor.Current.Value.Name : " + enumertor.Current.Value.Name);
            UIItemBuilding item = m_ScrollRect.GetReuseItem(uiBuildingItemPrefab, null);
            
        }
    }
}

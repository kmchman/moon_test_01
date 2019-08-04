using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MoonPopupManager : MonoBehaviour {

    [SerializeField] private GameObject bgImage;
    // Static
    private static Dictionary<int, MoonPopupManager> popupManager = new Dictionary<int, MoonPopupManager>();

    // Key : PopupUI Prefab, value : instance
    private Dictionary<string, MoonPopupUI> allPopup = new Dictionary<string, MoonPopupUI>();
    private List<MoonPopupUI> openedPopupLists = new List<MoonPopupUI>();

    public int depth = 0;
    public int defaultOrder = 0;

    protected virtual void Awake()
    {
        if (!popupManager.ContainsKey(depth))
        {
            popupManager[depth] = this;
        }
        bgImage.SetActive(false);
    }

    public T ShowPopup<T>(T popupPrefab, params object[] args) where T : MoonPopupUI
    {
        T popup = MakePopup<T>(popupPrefab);
        //if(!openedPopupLists.Contains(popup))
        //    openedPopupLists.Add(popup);

        //popup.ShowPopupByManager(OnHidePopup, defaultOrder + openedPopupLists.Count, args);
        
        return _ShowPopup(popup, args) as T;
    }

    private T MakePopup<T>(T popupPrefab) where T : MoonPopupUI
    { 
        if (allPopup.ContainsKey(popupPrefab.name))
            return allPopup[popupPrefab.name] as T;
        allPopup[popupPrefab.name] = Giant.Util.MakeItem<T>(popupPrefab, transform, false);

        return allPopup[popupPrefab.name] as T;
    }

    public MoonPopupUI ShowPopup(string popupPrefab, params object[] args)
    {
        return _ShowPopup(MakePopup(popupPrefab), args);
    }

    private MoonPopupUI _ShowPopup(MoonPopupUI popup, params object[] args)
    {
        if(!openedPopupLists.Contains(popup))
            openedPopupLists.Add(popup);

        popup.ShowPopupByManager(OnHidePopup, defaultOrder, openedPopupLists.Count, args);

        TouchBlock(popup.touchBackground);

        return popup;
    }

    private void TouchBlock(bool value)
    {
        //GameUserInputSystem gameUserInputSystem = Framework.GetGameSystem<GameUserInputSystem>();
        //gameUserInputSystem.myEvent.Send_TouchBlock(value);
        bgImage.SetActive(value);
    }

    public MoonPopupUI GetPopup(string popupPrefab)
    {
        MoonPopupUI popup = null;
        if (allPopup.ContainsKey(popupPrefab))
            popup = openedPopupLists.Find(item => item == allPopup[popupPrefab]);
        return popup;
    }

    public void HidePopup(string popupPrefab)
    {
        if (allPopup.ContainsKey(popupPrefab))
        {
            MoonPopupUI popupUI = allPopup[popupPrefab];
            popupUI.Hide();
        }
    }

    private MoonPopupUI MakePopup(string popupPrefab)
    {
        if (allPopup.ContainsKey(popupPrefab))
            return allPopup[popupPrefab] as MoonPopupUI;
        
        MoonPopupUI popupUI = Giant.Util.MakeItem<MoonPopupUI>(Constant.PopupRootPath + popupPrefab, transform, false);
        RectTransform rect = popupUI.GetComponent<RectTransform>();
        popupUI.transform.localPosition = Vector3.zero;
        popupUI.transform.localScale = Vector3.one;

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        allPopup[popupPrefab] = popupUI;

        return allPopup[popupPrefab];
    }

    protected virtual void OnHidePopup(MoonPopupUI popup)
    {
        openedPopupLists.Remove(popup);

        foreach (MoonPopupUI _popup in openedPopupLists)
        {
            if (_popup.touchBackground == true)
            {
                TouchBlock(true);
                return;
            }
        }
        TouchBlock(false);

    }
}

using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public class MoonPopupUI : MonoBehaviour {

    public Canvas canvas;
    public Canvas overCanvas;
    public int overCanver_Zorder = -500;
    public bool touchBackground = true;

    private Action<MoonPopupUI> hideAction;
    private Action hidePopupAction;
    protected System.Action refreshAction;
    private System.Action completeAction;
    private Animator popupAnimator;

    public virtual void Show(params object[] values)
    {
        gameObject.SetActive(true);
        _OnInitialize(values);
        AddEventListener();
        StopAllCoroutines();
        if (popupAnimator != null)
            popupAnimator.SetTrigger("Show");
        Open(values);
    }

    public virtual void Hide(params object[] values)
    {
        completeAction = () =>
        {
            gameObject.SetActive(false);
            RemoveEventListener();
            if (hideAction != null)
                hideAction(this);
            if (hidePopupAction != null)
                hidePopupAction();
        };

        if (popupAnimator != null && popupAnimator.enabled)
        {
            popupAnimator.SetTrigger("Hide");
            popupAnimator.Update(0);
            StopCoroutine("OnCompleteAnimation");
            StartCoroutine("OnCompleteAnimation");
        }
        else
            completeAction();
    }

    protected virtual void AddEventListener() { }
    protected virtual void RemoveEventListener() { }
    protected virtual void Open(params object[] values) { }
    protected virtual void _OnInitialize(params object[] values)
    {
        popupAnimator = GetComponent<Animator>();
    }

    public MoonPopupUI SetHidePopupAction(Action _hidePopupAction)
    {
        hidePopupAction = _hidePopupAction;
        return this;
    }

    public MoonPopupUI SetOnRefreshAction(System.Action _action)
    {
        refreshAction = _action;
        return this;
    }

    public virtual void OnClickBtnClose()
    {
        Hide();
    }

    public void ShowPopupByManager(Action<MoonPopupUI> _hidePopupAction, int defaultOrder, int order, params object[] values)
    {
        canvas.overrideSorting = true;
        canvas.sortingOrder = defaultOrder + (order - 1) * 10;
        canvas.transform.localPosition = new Vector3(0, 0, (order - 1) * -500);

        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        if (overCanvas != null)
        {
            overCanvas.sortingOrder = canvas.sortingOrder + 1;
            overCanvas.transform.localPosition = new Vector3(0, 0, canvas.transform.localPosition.z - 500);
            //overCanvas.transform.localPosition = new Vector3(0, 0, overCanver_Zorder);

            rectTransform = overCanvas.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
            }
        }

        InitParticleOrder();
        hideAction = _hidePopupAction;
        Show(values);
    }

    private void InitParticleOrder()
    {
        ParticleSystem[] particles = transform.GetComponentsInChildren<ParticleSystem>();
        foreach (var iter in particles)
        {
            Renderer renderer = iter.GetComponent<Renderer>();
            renderer.sortingOrder = canvas.sortingOrder;
        }
    }

    IEnumerator OnCompleteAnimation()
    {
        while (popupAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            yield return null;
        if (completeAction != null)
            completeAction();
    }

}

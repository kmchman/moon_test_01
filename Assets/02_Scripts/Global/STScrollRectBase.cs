using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class STScrollRectBase : ScrollRect
{
	public override void OnBeginDrag(PointerEventData eventData)
	{
//		if (!STGraphicManager.inst.OnWillDragScrollRect())
//			return;
		base.OnBeginDrag(eventData);
	}
	
	public override void OnDrag(PointerEventData eventData)
	{
//		if (!STGraphicManager.inst.OnWillDragScrollRect())
//			return;
		base.OnDrag(eventData);
	}
}

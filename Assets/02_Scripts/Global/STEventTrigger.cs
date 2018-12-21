using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;


public class STEventTrigger : EventTrigger
{
	[SerializeField] private UnityEvent			m_OnPointerLongDown;
	[SerializeField] private UnityEvent			m_OnPointerLongUp;
	[SerializeField] private bool				m_isInherit;

	private bool								m_isDown;
	private bool								m_isUsed;

	public override void OnPointerDown(PointerEventData eventData)
	{
		m_isDown = true;
		m_isUsed = false;

		base.OnPointerDown(eventData);

//		Invoke("InvokePointerLongDown", Constant.LongDownEventTime);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		m_isDown = false;

		if (m_isUsed)
			m_OnPointerLongUp.Invoke();
		
		base.OnPointerUp(eventData);

		CancelInvoke("InvokePointerLongDown");
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);

		if (m_isDown)
			CancelInvoke("InvokePointerLongDown");
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (m_isUsed)
		{
			eventData.Use();
			return;
		}

		Execute<IPointerClickHandler>(EventTriggerType.PointerClick, eventData, ExecuteEvents.pointerClickHandler);
	}

	public override void OnInitializePotentialDrag(PointerEventData eventData)
	{
		eventData.pointerDrag = Execute<IInitializePotentialDragHandler>(EventTriggerType.InitializePotentialDrag, eventData, ExecuteEvents.initializePotentialDrag);
	}

	private void InvokePointerLongDown()
	{
		if (m_OnPointerLongDown.GetPersistentEventCount() <= 0)
			return;

		m_isUsed = true;

		m_OnPointerLongDown.Invoke();
	}

	// 반환값 : 이벤트를 처리한 게임 오브젝트
	private GameObject Execute<T>(EventTriggerType eventTriggerType, BaseEventData eventData, ExecuteEvents.EventFunction<T> functor) where T : IEventSystemHandler
	{
		bool isExistCallback = false;
		for (int i = 0, count = triggers.Count; i < count; ++i)
		{
			var trigger = triggers[i];
			if (trigger.eventID == eventTriggerType && trigger.callback != null)
			{
				isExistCallback = true;
				trigger.callback.Invoke(eventData);
			}
		}
		if(isExistCallback)
			return gameObject;
		
		if (m_isInherit)
		{
			GameObject obj = ExecuteEvents.GetEventHandler<T>(GetParentObject());
			if (obj != null)
			{
				ExecuteEvents.Execute(obj, eventData, functor);
				return obj;
			}

			T[] components = gameObject.GetComponents<T>();
			bool isHaveOtherComponent = false;
			for (int i = 0; i < components.Length; ++i)
			{
				if ((object)components[i] == (object)this)
					continue;
				isHaveOtherComponent = true;
				break;
			}

			if(isHaveOtherComponent)
				return obj;

			if (eventTriggerType == EventTriggerType.PointerClick)
				InvokeOnPointerLongDown();
		}
		return gameObject;
	}

	private void InvokeOnPointerLongDown()
	{
		if(m_OnPointerLongDown.GetPersistentEventCount() > 0)
			m_OnPointerLongDown.Invoke();
	}

	private GameObject GetParentObject()
	{
		Transform parent = gameObject.transform.parent;
		if (parent != null)
			return parent.gameObject;
		return null;
	}
}

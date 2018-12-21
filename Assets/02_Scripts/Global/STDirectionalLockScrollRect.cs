using UnityEngine;
using System.Collections;

public class STDirectionalLockScrollRect : STScrollRect
{
	private enum DirectionalState
	{
		None,
		DirectionX,
		DirectionY,
	}

	[SerializeField] private bool m_UseDirectionalLock = true;

	private DirectionalState m_DirectionalState = DirectionalState.None;

	public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
	{
		Vector2 delta = eventData.delta;

		if (!m_UseDirectionalLock)
			m_DirectionalState = DirectionalState.None;
		else if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
			m_DirectionalState = DirectionalState.DirectionX;
		else
			m_DirectionalState = DirectionalState.DirectionY;

		if (m_UseDirectionalLock)
			eventData.position = GetDirectionPosition(eventData.position, eventData.pressPosition, m_DirectionalState);
		
		base.OnBeginDrag(eventData);
	}

	public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
	{
		if (m_UseDirectionalLock)
			eventData.position = GetDirectionPosition(eventData.position, eventData.pressPosition, m_DirectionalState);

		base.OnDrag(eventData);
	}

	private Vector3 GetDirectionPosition(Vector3 position, Vector3 pressPosition, DirectionalState state)
	{
		switch (state)
		{
		case DirectionalState.DirectionX:
			position.y = pressPosition.y;
			break;

		case DirectionalState.DirectionY:
			position.x = pressPosition.x;
			break;
		}

		return position;
	}
}

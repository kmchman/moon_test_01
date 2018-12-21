using UnityEngine;

using System.Collections;

using STExtensions;

public class STAnimationEventer : MonoBehaviour
{
	private System.Action<string> m_OnAnimationEventAction;

	public void SetOnAnimationEventAction(System.Action<string> onAnimationEventAction)
	{
		m_OnAnimationEventAction = m_OnAnimationEventAction.AppendOrClear(onAnimationEventAction);
	}

	public void OnAnimationEvent(string eventName)
	{
		if (m_OnAnimationEventAction != null)
			m_OnAnimationEventAction(eventName);
	}
}

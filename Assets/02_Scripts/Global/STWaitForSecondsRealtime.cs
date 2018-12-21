using UnityEngine;
using System.Collections;

public class STWaitForSecondsRealtime : CustomYieldInstruction
{
	public override bool keepWaiting { get { return Time.realtimeSinceStartup < m_WaitTime; } }

	private float m_WaitTime;

	public STWaitForSecondsRealtime SetWaitTime(float time)
	{
		m_WaitTime = Time.realtimeSinceStartup + time;
		return this;
	}
}
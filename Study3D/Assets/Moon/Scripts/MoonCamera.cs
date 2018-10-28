using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonCamera : MonoBehaviour {

	[SerializeField] Transform 		m_TargetObject;
	[SerializeField] int 			m_SmoothValue;

	private Vector3 				m_Offset;
	// Use this for initialization
	void Start () {
		m_Offset = this.transform.position - m_TargetObject.position;
	}

	void FixedUpdate()
	{
		Vector3 targetPos = m_TargetObject.position + m_Offset;
		transform.position= Vector3.Lerp (transform.position, targetPos, Time.deltaTime * m_SmoothValue);
	}
}

using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class STPrefabGenerator : MonoBehaviour
{
	[SerializeField] private GameObject m_PrefabObject;
	[SerializeField] private bool m_IsUseGameObjectPool = false;
	[SerializeField] private bool m_IsMakeOnAwake = true;

	public RectTransform rectTransformRef
	{
		get
		{
			if (m_RectTrasnformRef == null)
				m_RectTrasnformRef = transform as RectTransform;
			return m_RectTrasnformRef;
		}
	}

	private RectTransform m_RectTrasnformRef;

	private GameObject m_Object;

	private void Awake()
	{
		if (m_IsMakeOnAwake)
			MakePrefabObject();
	}

	public ComponentType Get<ComponentType>() where ComponentType : Component
	{
		if (m_Object == null && !MakePrefabObject())
			return null;
		return m_Object.GetComponent<ComponentType>();
	}

	public void Release()
	{
		if (m_IsUseGameObjectPool)
			GameObjectPool.Push(m_Object);
		else
			GameObject.Destroy(m_Object);
		
		m_Object = null;
	}

	private bool MakePrefabObject()
	{
//		if (m_PrefabObject == null || m_Object != null)
//			return false;
//
//		if (m_IsUseGameObjectPool)
//			m_Object = GameObjectPool.Pop(m_PrefabObject, false, rectTransformRef, false);
//		else
//			m_Object = Giant.Util.MakeItem(m_PrefabObject, rectTransformRef, false);
		
		return true;
	}
}

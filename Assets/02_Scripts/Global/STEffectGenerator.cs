using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using STExtensions;

public class STEffectGenerator : MonoBehaviour
{
	[SerializeField] private bool m_PlayOnAwake;
	[SerializeField] private bool m_IsOneShot;
	[SerializeField] private STEffectItem[] m_EffectPrefab;

	private STEffectItem[] m_EffectObject;

	private FlagSynchronizer m_CompleteSynchronizer = new FlagSynchronizer();

	private System.Action m_CompleteEffectAction;

	private void OnEnable()
	{
		if (m_PlayOnAwake)
			Play();
	}

	private void OnDisable() { }

	public void Play() { Play(0); }
	public void Play(float normalTime)
	{
		if (m_EffectPrefab == null || m_EffectPrefab.Length <= 0)
			return;
		
		MakeEffectObject();

		m_CompleteSynchronizer.Init();

		for (int i = 0; i < m_EffectObject.Length; ++i)
		{
			m_CompleteSynchronizer.SetFlag(m_EffectObject[i]);
			m_EffectObject[i].Play(normalTime).SetOnCompleteAction(OnCompleteItem);
		}
			
		m_CompleteSynchronizer.AddWaitEvent(OnCompleteEffect);
	}

	public void Pause()
	{
		if (m_EffectObject == null || m_EffectObject.Length <= 0)
			return;

		for (int i = 0; i < m_EffectObject.Length; ++i)
			m_EffectObject[i].Pause();
	}

	public void Stop()
	{
		if (m_EffectObject == null || m_EffectObject.Length <= 0)
			return;

		for (int i = 0; i < m_EffectObject.Length; ++i)
			m_EffectObject[i].Stop();
	}

	public void Clear()
	{
		DestroyEffectObject();
	}

	public void SetOnCompleteAction(System.Action completeEffectAction)
	{
		m_CompleteEffectAction = m_CompleteEffectAction.AppendOrClear(completeEffectAction);
	}

	private void MakeEffectObject()
	{
//		if (m_EffectObject != null)
//			return;
//
//		Transform transformRef = transform;
//		m_EffectObject = new STEffectItem[m_EffectPrefab.Length];
//
//		for (int i = 0; i < m_EffectPrefab.Length; ++i)
//		{
//			m_EffectObject[i] = Giant.Util.MakeItem(m_EffectPrefab[i], transformRef, false);
//			m_EffectObject[i].Stop();
//		}
	}

	private void DestroyEffectObject()
	{
		if (m_EffectObject == null || m_EffectObject.Length <= 0)
			return;

		for (int i = 0; i < m_EffectObject.Length; ++i)
			GameObject.Destroy(m_EffectObject[i].gameObject);

		m_EffectObject = null;
	}

	private void OnCompleteItem(STEffectItem item)
	{
		m_CompleteSynchronizer.ResetFlag(item);
	}

	private void OnCompleteEffect()
	{
		if (m_CompleteEffectAction != null)
			m_CompleteEffectAction();

		if (m_IsOneShot)
			DestroyEffectObject();
	}
}
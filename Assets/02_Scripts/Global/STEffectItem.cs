using UnityEngine;

using System.Collections;

using STExtensions;

public abstract class STEffectItem : MonoBehaviour
{
	public enum State
	{
		None,
		Complete,
		LoopComplete,
	}

	[SerializeField] private string m_Alias;
	[UnityEngine.Serialization.FormerlySerializedAs("m_IsUseInputEvent")]
	[SerializeField] private bool m_IsBlockInputEvent = false;
	[UnityEngine.Serialization.FormerlySerializedAs("m_IsUseEventAction")]
	[SerializeField] private bool m_IsPauseEventAction = false;
	[SerializeField] private SoundEffect m_SEType = SoundEffect.None;
	[SerializeField] private float m_SEDelayTime = 0f;
	[SerializeField] private bool m_IsPlaySEOnEnable = false;

	public string alias { get { return m_Alias; } }
	public bool isBlockInputEvent { get { return m_IsBlockInputEvent; } }
	public bool isPauseEventAction { get { return m_IsPauseEventAction; } }
	public virtual bool isPause { get { return false; } }

	private System.Action<STEffectItem> m_OnCompleteAction;
	private SoundManagerAudioSource m_PlaySEAudioSource;

	protected virtual void Awake() {}
	protected virtual void Start() {}

	protected virtual void OnEnable()
	{
		if (m_IsPlaySEOnEnable)
			PlaySE();
		RegisterEffectItem();
	}

	protected virtual void OnDisable()
	{
		StopSE();
		UnRegisterEffectItem();
		m_PlaySEAudioSource = null;
	}

	public STEffectItem SetOnCompleteAction(System.Action<STEffectItem> onCompleteAction)
	{
		m_OnCompleteAction = m_OnCompleteAction.AppendOrClear(onCompleteAction);
		return this;
	}

	public STEffectItem Play()
	{
		Play(0);
		return this;
	}

	public virtual STEffectItem Play(float normalTime)
	{
		PlaySE();
		gameObject.SetActive(false);
		gameObject.SetActive(true);
		return this;
	}

	public virtual STEffectItem Pause()
	{
		PauseSE();
		return this;
	}

	public virtual STEffectItem Stop()
	{
		StopSE();
		gameObject.SetActive(false);
		return this;
	}

	public virtual void OnCompleteByManager(State state)
	{
//		if (m_OnCompleteAction != null)
//		{
//			m_OnCompleteAction(this);
//			m_OnCompleteAction = null;
//			m_PlaySEAudioSource = null;
//		}
//
//		switch (state)
//		{
//		case State.Complete:
//			STEffectItemManager.inst.UnregisterEffectItem(this);
//			break;
//		}
	}

	public abstract State CheckCompleteItem();

	protected void RegisterEffectItem()
	{
//		STEffectItemManager.inst.RegisterEffectItem(this);
	}

	protected void UnRegisterEffectItem()
	{
//		STEffectItemManager.inst.UnregisterEffectItem(this);
		m_OnCompleteAction = null;
	}

	protected void PlaySE()
	{
		if (m_SEType == SoundEffect.None)
			return;
		if (m_PlaySEAudioSource != null)
		{
			m_PlaySEAudioSource.Continue();
			return;
		}
//		m_PlaySEAudioSource = SoundManager.instance.PlayEffect(m_SEType, m_SEDelayTime);
	}

	protected void PauseSE()
	{
		if (m_SEType == SoundEffect.None)
			return;

		if (m_PlaySEAudioSource != null)
		{
			m_PlaySEAudioSource.Pause();
			return;
		}
	}

	protected void StopSE()
	{
		if (m_SEType == SoundEffect.None)
			return;
		
		if (m_PlaySEAudioSource != null)
		{
			m_PlaySEAudioSource.Stop();
			m_PlaySEAudioSource = null;
		}
	}
}
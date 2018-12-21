using UnityEngine;
using System.Collections;

public class STParticleSystemItem : STEffectItem
{
	[SerializeField] private ParticleSystem m_MainParticleSystem;

	public override bool isPause { get { return m_MainParticleSystem.isPaused; } }

	public ParticleSystem mainParticleSystem { get { return m_MainParticleSystem; } }

	private bool m_IsLoop;
	private float m_Duration;
	private float m_LastTime;

	protected override void Awake()
	{
		base.Awake();

		m_IsLoop = m_MainParticleSystem.main.loop;
		m_Duration = m_MainParticleSystem.main.duration;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_LastTime = 0;
	}

	public override STEffectItem Play(float normalTime)
	{
		base.Play(normalTime);

		m_LastTime = 0;
		m_MainParticleSystem.time = m_Duration * normalTime;

		return this;
	}

	public override STEffectItem Pause()
	{
		m_MainParticleSystem.Pause();
		return base.Pause();
	}

	public override State CheckCompleteItem()
	{
		float lastTime = m_LastTime;
		float time = mainParticleSystem.time;
		m_LastTime = time;

		if (m_IsLoop && lastTime > time)
			return State.LoopComplete;
		if (!m_IsLoop && time >= m_Duration)
			return State.Complete;
				
		return State.None;
	}
}
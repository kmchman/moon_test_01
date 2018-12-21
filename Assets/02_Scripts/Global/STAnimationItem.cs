using UnityEngine;

using System;
using System.Collections;

using STExtensions;

public class STAnimationItem : STEffectItem
{
	private const string UIResetStateName = "UIReset";
	private static int UIResetHash = Animator.StringToHash(UIResetStateName);

	[SerializeField] private Animator m_Animator;

	[SerializeField] private bool m_PlayOnAwake = true;

	[SerializeField] private STAnimationEventer[] m_AnimationEventers;

	public override bool isPause { get { return m_Animator.speed.IsEqual(0); } }

	public Animator animator { get { return m_Animator; } }

	private Action<string> m_OnAnimationEventAction;

	private bool m_DontRepeat = false;
	private bool m_InitStart = false;

	private int m_LastNameHash = -1;

	private int m_LastNormalTime;

	protected override void Awake()
	{
		base.Awake();

		for (int i = 0; i < m_AnimationEventers.Length; ++i)
			m_AnimationEventers[i].SetOnAnimationEventAction(OnAnimationEvent);
	}

	protected override void Start()
	{
		base.Start();

		m_InitStart = true;

		if (!m_PlayOnAwake)
			Reset();
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_LastNormalTime = 0;

		// Start가 한번도 호출 되지 않았다면, 최초 1회는 Start에서 Reset을 한다. (m_DidAwake 라는 에러가 출력되는 문제 방지)
		if (m_InitStart && !m_PlayOnAwake)
			Reset();
	}

	public override STEffectItem Play(float normalTime)
	{
		return Play(0, normalTime);
	}

	public STAnimationItem Play(string name, float normalTime)
	{
		return Play(Animator.StringToHash(name), normalTime);
	}

	private STAnimationItem Play(int nameHash, float normalTime)
	{
		if (isPause)
		{
//			PlaySE();
			m_Animator.speed = 1;
			return this;
		}

		if (m_DontRepeat && m_LastNameHash == nameHash)
			return this;

		Reset();

		if (nameHash != 0 && !m_Animator.HasState(0, nameHash))
		{
			Debug.LogError(Util.LogFormat("Not found Animator State", name, nameHash), gameObject);
		    return this;
		}

		base.Play(normalTime);

		if (!m_Animator.gameObject.activeInHierarchy)
			return this;

		m_Animator.Rebind();
		m_Animator.speed = 1;
		m_Animator.Play(nameHash, 0, normalTime);

		m_LastNameHash = nameHash;
		m_LastNormalTime = 0;

//		RegisterEffectItem();

		return this;
	}

	public override STEffectItem Pause()
	{
		m_Animator.speed = 0;
		return base.Pause();
	}

//	public STAnimationItem SetIsPauseEvent(bool isPause)
//	{
//		STEffectItemManager.inst.SetIsPauseEventAction(this, isPause);
//		return this;
//	}
//
//	public STAnimationItem SetIsBlockInputEvent(bool isBlock)
//	{
//		STEffectItemManager.inst.SetIsBlockInputEvent(this, isBlock);
//		return this;
//	}

	public void Reset()
	{
		if (!m_Animator.gameObject.activeInHierarchy)
			return;

		if (!m_Animator.HasState(0, UIResetHash))
			return;

		m_Animator.Play(UIResetStateName, 0, 10f);
		m_Animator.Update(0);
		m_Animator.StopPlayback();

//		UnRegisterEffectItem();
	}

	public void TakeSnapshot()
	{
		gameObject.SetActive(false);
		gameObject.SetActive(true);
	}

	public void SetDontRepeat(bool dontRepeat)
	{
		m_DontRepeat = dontRepeat;
	}

	public override State CheckCompleteItem()
	{
		if (!m_Animator.gameObject.activeInHierarchy)
			return State.Complete;

		AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
		int lastNormalTime = m_LastNormalTime;
		m_LastNormalTime = (int)stateInfo.normalizedTime;

		if (stateInfo.loop && (lastNormalTime < m_LastNormalTime))
			return State.LoopComplete;
		if (!stateInfo.loop && stateInfo.normalizedTime > 1)
			return State.Complete;

		return State.None;
	}

	public override void OnCompleteByManager(State state)
	{
		base.OnCompleteByManager(state);

		if (state == State.Complete && m_DontRepeat)
			TakeSnapshot();
	}

	public STAnimationItem SetOnAnimationEventAction(Action<string> onAnimationEventAction)
	{
		m_OnAnimationEventAction = m_OnAnimationEventAction.AppendOrClear(onAnimationEventAction);
		return this;
	}

	private void OnAnimationEvent(string eventName)
	{
		if (m_OnAnimationEventAction != null)
			m_OnAnimationEventAction(eventName);
	}
}
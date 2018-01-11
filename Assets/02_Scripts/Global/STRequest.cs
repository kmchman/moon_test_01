using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class STRequest
{
	private const int MAX_RETRY_COUNT = 2;

	private static List<STRequest> AllRequestList = new List<STRequest>();
	private static List<STRequest> ReadyRequestList = new List<STRequest>();

	private enum State
	{
		Wait		= 10,
		Cancel,
		Hold,					// 유저의 액션을 기다리는 상태

		Success		= 100,

		Failure		= 200,
	}

	public bool isFinish { get { return m_State == State.Success || m_State == State.Failure; } }
	public bool isPolling { get { return m_PollingIntervalSecond >= 0; } }
	public bool isMaxRetry { get { return m_RetryCount >= MAX_RETRY_COUNT; } }

	private string m_RequestJson;

	private Action m_SuccessAction;
	private Action<object> m_FailureAction;
	private Action<object> m_LoginFailAction;

	private State m_State;
	private bool m_IsUseDefaultFailureAction;
	private bool m_IsUseRetryRequest;
	private bool m_IsSilent;
	private float m_PollingIntervalSecond;
	private object m_Error;
	private int m_RetryCount;

	public static bool IsWait()
	{
		for (int i = 0; i < AllRequestList.Count; ++i)
			if (AllRequestList[i].m_State == State.Wait && !AllRequestList[i].m_IsSilent)
				return true;
		return false;
	}

	public static STRequest Request(string requestJson)
	{
		if (ReadyRequestList.Count <= 0)
		{
			STRequest newRequest = new STRequest();
			newRequest.Clear();

			AllRequestList.Add(newRequest);
			ReadyRequestList.Add(newRequest);
		}

		STRequest request = ReadyRequestList[0];
		ReadyRequestList.RemoveAt(0);

		request.Init(requestJson);

		return request;
	}

	public static void StopAllPollingRequest()
	{
		for (int i = 0; i < AllRequestList.Count; ++i)
			if (AllRequestList[i].isPolling)
				AllRequestList[i].Cancel();
	}

	private static void Return(STRequest request)
	{
		request.Clear();

		ReadyRequestList.Add(request);
	}

	public STRequest Init(string requestJson)
	{
		m_State = State.Wait;
		m_RequestJson = requestJson;

		return this;
	}

	public void Cancel()
	{
		m_State = State.Cancel;
	}

	public void Hold()
	{
		m_State = State.Hold;
		++m_RetryCount;
	}

	public void ReleaseHold()
	{
		m_State = State.Wait;
	}

	public void Return()
	{
		Return(this);
	}

	public STRequest SetSuccessAction(Action successAction)
	{
		m_SuccessAction = successAction;
		return this;
	}

	public STRequest SetFailureAction(Action<object> failureAction)
	{
		m_FailureAction = failureAction;
		return this;
	}

	public STRequest SetActiveDefaultFailureAction(bool isActive)
	{
		m_IsUseDefaultFailureAction = isActive;
		return this;
	}

	public STRequest SetRetryRequest(bool isActive)
	{
		m_IsUseRetryRequest = isActive;
		return this;
	}

	public STRequest SetSilentMode(bool isActive)
	{
		m_IsSilent = isActive;
		return this;
	}

	public STRequest SetPollingInterval(float intervalSecond)
	{
		m_PollingIntervalSecond = intervalSecond;
		return this;
	}

	public YieldInstruction WaitForResponse()
	{
		if (m_State == State.Success || m_State == State.Failure)
			return null;
		return GlobalManager.Inst.StartCoroutine(CoWaitForResponse());
	}

	public YieldInstruction WaitForSuccessResponse()
	{
		if (m_State == State.Success || m_State == State.Failure)
			return null;
		return GlobalManager.Inst.StartCoroutine(CoWaitForSuccessResponse());
	}

	public void OnResponse(object result)
	{
		m_Error = result;
		Global.Inst.PushFastTask(CallResponse);
	}

	private void Clear()
	{
		m_RequestJson = null;

		m_SuccessAction = null;
		m_FailureAction = null;

		m_IsUseDefaultFailureAction = true;
		m_IsUseRetryRequest = false;
		m_IsSilent = false;
		m_PollingIntervalSecond = -1;
		m_Error = null;
		m_RetryCount = 0;
	}

	private bool CheckCancel()
	{
		if (m_State != State.Cancel)
			return false;

		Return();
		return true;
	}

	private void CallResponse()
	{
		if (CheckCancel())
			return;

		try
		{
			if (m_Error == null)
				CallSuccessAction();
			else
				CallFailureAction(m_Error);
		}
		catch (Exception e)
		{
			Debug.LogError(Logger.Write("error", "STRequest.CallResponse", e.ToString()));
		}

		if (isPolling)
			GlobalManager.Inst.StartCoroutine(CoPolling());
		else
			Return();
	}

	private void CallSuccessAction()
	{
		m_State = State.Success;

		if (m_SuccessAction != null)
			m_SuccessAction();
	}

	private void CallFailureAction(object error)
	{
		m_State = State.Failure;

		Debug.LogError(Util.LogFormat(error, m_RequestJson));

		if (m_FailureAction != null)
			m_FailureAction(error);
		else if (m_IsUseDefaultFailureAction)
			CallDefaultFailureAction(error);	
	}

	private void CallDefaultFailureAction(object error)
	{
		if (m_IsSilent)
			return;

//		SystemOneButtonPopup popup = SystemPopupManager.Inst.ShowOneButtonPopup(Datatable.Inst.GetServerErrorText(error.ToString()));
//
//		if (m_IsUseRetryRequest)
//		{
//			string json = m_RequestJson;
//			Action successAction = m_SuccessAction;
//
//			popup.SetClickButtonAction(() =>
//			{
//				RetryRequest(json).SetSuccessAction(successAction).SetRetryRequest(true);
//			});
//		}
	}

	private STRequest RetryRequest(string json)
	{
		if (CheckCancel())
			return this;

//		if (m_IsSilent)
//			return GiantHandler.Inst.SilentRequest(json, this);
//		else
//			return GiantHandler.Inst.Request(json, this);
		return this;
	}

	private IEnumerator CoWaitForResponse()
	{
		while (m_State != State.Success && m_State != State.Failure)
			yield return null;
	}

	private IEnumerator CoWaitForSuccessResponse()
	{
		while (m_State != State.Success)
			yield return null;
	}

	private IEnumerator CoPolling()
	{
		float currentTime = Time.realtimeSinceStartup;

		while (Time.realtimeSinceStartup < currentTime + m_PollingIntervalSecond)
			yield return null;

		RetryRequest(m_RequestJson);
	}
}
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class Synchronizer
{
	private event Action _syncEvent;
	private int _syncCount = 0;
	
	public void Init()				{ _syncCount = 0; _syncEvent = null; }
	public void IncreaseCount()		{ _syncCount++; }
	public void DecreaseCount()
	{
		--_syncCount;
		
		if (_syncCount == 0 && _syncEvent != null)
		{
			Action syncEvent = _syncEvent;
			_syncEvent = null;
			syncEvent();
		}
	}
	
	public void AddWaitEvent(Action syncEvent)
	{
		if (_syncCount == 0)
		{
			syncEvent();
		}
		else
		{
			_syncEvent += syncEvent;
		}
	}
	
	public new string ToString()
	{
		return _syncCount.ToString();
	}
}

public class FlagSynchronizer
{
	private HashSet<object> _syncHash = new HashSet<object>();
	private event Action _syncEvent;

	public void Init(params object[] keys)
	{
		_syncHash.Clear();

		if (keys == null || keys.Length <= 0)
			return;

		for (int i = 0; i < keys.Length; ++i)
			SetFlag(keys[i]);
	}

	public void SetFlag(object key)
	{
		_syncHash.Add(key);
	}

	public void ResetFlag(object key)
	{
		if (!_syncHash.Remove(key))
			return;

		if (_syncHash.Count <= 0 && _syncEvent != null)
		{
			Action syncEvent = _syncEvent;
			_syncEvent = null;
			syncEvent();
		}	
	}

	public void AddWaitEvent(Action syncEvent)
	{
		if (_syncHash.Count <= 0)
			syncEvent();
		else
			_syncEvent += syncEvent;
	}
}
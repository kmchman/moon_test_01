using UnityEngine;
using System.Collections;
using System;

public class Synchronizer {

	private event Action _syncEvent;
	private int _syncCount = 0;

	public void Init()		
	{
		_syncCount = 0;
		_syncEvent = null;
	}

	public void IncreaseCount()
	{
		_syncCount++;
	}

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
}

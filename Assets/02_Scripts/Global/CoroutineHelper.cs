using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class CoroutineHelper
{
	private static int _index = 0;
	HashSet<int> _flag = new HashSet<int>();

	public Coroutine Wait(MonoBehaviour coroutineObjet, Action<Action<object>> method, Action<object> cb)
	{
		int index = _index++;
		Action<object> cb2 = this.Wait(coroutineObjet, method, cb, index);
		method(cb2);

		return coroutineObjet.StartCoroutine(Start(index));
	}

	public Coroutine Wait<TypeName1>(MonoBehaviour coroutineObjet, Action<TypeName1, Action<object>> method, TypeName1 arg1, Action<object> cb)
	{
		int index = _index++;
		Action<object> cb2 = this.Wait(coroutineObjet, method, cb, index);
		method(arg1, cb2);

		return coroutineObjet.StartCoroutine(Start(index));
	}

	public Coroutine Wait<TypeName1, TypeName2>(MonoBehaviour coroutineObjet, Action<TypeName1, TypeName2, Action<object>> method, TypeName1 arg1, TypeName2 arg2, Action<object> cb)
	{
		int index = _index++;
		Action<object> cb2 = this.Wait(coroutineObjet, method, cb, index);
		method(arg1, arg2, cb2);

		return coroutineObjet.StartCoroutine(Start(index));
	}

	public Coroutine Wait<TypeName1, TypeName2, TypeName3>(MonoBehaviour coroutineObjet, Action<TypeName1, TypeName2, TypeName3, Action<object>> method, TypeName1 arg1, TypeName2 arg2, TypeName3 arg3, Action<object> cb)
	{
		int index = _index++;
		Action<object> cb2 = this.Wait(coroutineObjet, method, cb, index);
		method(arg1, arg2, arg3, cb2);

		return coroutineObjet.StartCoroutine(Start(index));
	}

	private Action<object> Wait(MonoBehaviour CoroutineObjet, object method, Action<object> cb, int index)
	{
		if(CoroutineObjet == null || method == null) { Debug.LogError(Logger.Write("error", "coroutineHelper", "wait")); throw new System.Exception("coroutineHelper"); }

		_flag.Add(index);
		return (result) => { cb(result); _flag.Remove(index); };
	}

	private IEnumerator Start(int index)
	{
		while(_flag.Contains(index)) { yield return 0; }
	}
}
using UnityEngine;

using System;
using System.Collections;

public class MsgRequest {
	private int lastRequestID = 0;
	private Hashtable requestCallbacks = new Hashtable();
		
	public int GetRequestID(Action<object> cb) {
		for (;;) {
			int id = lastRequestID = ++lastRequestID % 0xFFFF;
			if (!requestCallbacks.Contains(id)) {
				requestCallbacks[id] = cb;
				return id;
			}
		}
	}
	
	public Action<object> ReleaseRequestID(int id) {
		if (!requestCallbacks.Contains(id))
			return null;
		Action<object> cb = (Action<object>)requestCallbacks[id];
		requestCallbacks.Remove(id);
		return cb;
	}
	
	public void CancelRequest(object err) {
		Util.ForEach<int, Action<object>>(requestCallbacks, (k, v) => {
			v(err);
		});
	}
}

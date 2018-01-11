using UnityEngine;

using System;
using System.Collections;
using System.Reflection;

namespace AL {

public class MsgContext {
	private object handler;
	private MsgRequest msgRequest = new MsgRequest();
	public MsgContext(object handler) {
		this.handler = handler;
	}
	public void CancelRequest(object err) {
		msgRequest.CancelRequest(err);
	}
	public virtual void Send(object[] args, Action<object> cb = null) {
	}
	public virtual void Close() {
	}
	public void Notify(object[] args) {
		ArrayList oargs = new ArrayList(args);
		oargs.Insert(0, "ntf");
		Send(oargs.ToArray());
	}
	public void Event(object[] args) {
		ArrayList oargs = new ArrayList(args);
		oargs.Insert(0, "evt");
		Send(oargs.ToArray());
	}
	public void Request(object[] args, Action<object> cb) {
		ArrayList oargs = new ArrayList(args);
		int id = msgRequest.GetRequestID(cb);
		oargs.Insert(0, "req");
		oargs.Insert(1, id);
		Send(oargs.ToArray(), cb);
	}
	private void Invoke(string member, object[] args) {
		Type type = handler.GetType();
		try {
			type.InvokeMember(
				member,
				BindingFlags.Default | BindingFlags.InvokeMethod,
				null,
				handler,
				args);
		} catch (Exception e) {
			Debug.LogError(Logger.Write(e));
			Debug.LogError(Logger.Write("invoke: ", member, args));
			MemberInfo[] mis = type.GetMember(member);
			Util.ForEach(mis, Debug.LogError);
		}
	}
	public void ProcMsg(IList msg) {
		Debug.Log(Logger.Write("recv", msg));
		try {
			Queue q = new Queue(msg);
			string proc = (string)q.Dequeue();
			string fn;
			switch (proc) {
			case "ntf":
				fn = (string)q.Dequeue();
				fn = "ntf" + fn.Substring(0, 1).ToUpper() + fn.Substring(1);
				Invoke(fn, q.ToArray());
				break;
			case "evt":
				fn = (string)q.Dequeue();
				fn = "evt" + fn.Substring(0, 1).ToUpper() + fn.Substring(1);
				Invoke(fn, q.ToArray());
				break;
			case "ans":
				int id = (int)(long)q.Dequeue();
				object err = q.Dequeue();
				Action<object> cb = msgRequest.ReleaseRequestID(id);
				if(null != cb)
					cb(err);
				break;
			default:
				throw new Exception("INVALID_PROC");
			}
		} catch (Exception e) {
			Debug.LogError("ProcMsg Exception: " + e);
		}
	}
};

}

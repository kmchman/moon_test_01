using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace AL {

public class NetElement : MemberToString {
	public virtual void OnUpdated() {}
}

public class NetDictionary<KT, VT> : Dictionary<KT, VT> {
	public event Action<KT, VT> OnUpdated;
	public event Action<KT, VT> OnNew;
	public void InvokeUpdated(KT k, VT v) {
		if (null != OnUpdated) OnUpdated(k, v);
	}
	public void InvokeNew(KT k, VT v) {
		if(null != OnNew) OnNew(k, v);
	}
	public void AddMonitor(Action<KT, VT> cb) {
		Util.ForEach(this, cb);
		OnUpdated += cb;
	}
	public void RemoveMonitor(Action<KT, VT> cb) {
		OnUpdated -= cb;
	}
	public void UpdateValue(object key, object d, Action<VT> cb = null) {
		Type kt = typeof(KT);
		KT k;
		if (kt != typeof(string)) {
			k = (KT)kt.InvokeMember(
				"Parse",
				BindingFlags.Default | BindingFlags.InvokeMethod,
				null,
				null,
				new object[] { key.ToString() });
			if (null == d) {
				Remove(k);
				InvokeUpdated(k, default(VT));
				return;
			}
		} else {
			k = (KT)key;
		}
		VT val;
		Type vt = typeof(VT);
		bool bNew = !ContainsKey(k);

		if (vt.IsValueType) {
			val = (VT)Util.CastValue(vt, d);
		}
		else {
			if (ContainsKey(k)) {
				val = this[k];
			}
			else {
				MethodInfo info = vt.GetMethod("Create");
				if (null != info) {
					val = this[k] = (VT)info.Invoke(null, new object[] { d });
				} else {
					val = this[k] = (VT)vt.GetConstructor(new Type[] {}).Invoke(null);
				}
			}
			Util.GetFields(val, d);
		}

		this[k] = val;
	
		MethodInfo mi = typeof(VT).GetMethod("OnUpdated");
		if (null != mi)
			mi.Invoke(val, null);
			
		if(bNew)
			InvokeNew(k, val);
		InvokeUpdated(k, val);	

		if (null != cb) cb(val);
		
	}
	public void UpdateValues(IDictionary d, Action<VT> cb = null) {
		Hashtable h = new Hashtable();
		Util.ForEach(d, (k, v) => h.Add(k.ToString(), v));
		Util.ForEach(this, (k, v) => {
			if (!h.Contains(k.ToString()))
				h[k] = null;
		});
		Util.ForEach(h, (k,v) => UpdateValue(k, v, cb));
	}
	public void UpdateValues(IDictionary d, bool clear) {
		Hashtable h = new Hashtable();

		var e = d.GetEnumerator();
		while (e.MoveNext())
			h.Add(e.Key.ToString(), e.Value);

		if (clear) 
		{
			e = this.GetEnumerator();
			while (e.MoveNext())
				if(!h.Contains(e.Key.ToString()))
				   h[e.Key] = null;
		}

		e = h.GetEnumerator();
		while (e.MoveNext())
			UpdateValue(e.Key, e.Value);
	}
	public new void Clear() {
		List<KT> l = new List<KT>();
		Util.ForEach<KT, VT>(this, (k, v) => {
			l.Add(k);
		});
		Util.ForEach<KT>(l, (k) => {
			UpdateValue(k, null);
		});
		base.Clear();
	}
	public VT Get(KT k) {
		if (!ContainsKey(k))
			return default(VT);
		return this[k];
	}
}

}

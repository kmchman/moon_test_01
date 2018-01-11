using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using WebSocketSharp;

namespace AL {

public class WSClient : MsgContext {

	private string url;
	private WebSocket ws;
	private bool opened;
	private bool connected;
	private uint rkey;
	private LCGMask rmask;
	private uint skey;
	private LCGMask smask;
	
	public bool IsConnected {
		get { return connected; }
	}
	
	public WSClient(string url, object handler) : base(handler) {
		this.url = url;
		this.opened = false;
		this.connected = false;
	}

	public void SetURL(string url) {
		this.url = url;
	}
	
	private List<MessageEventArgs> premsg = new List<MessageEventArgs>();
	public void Connect(Action<object> cb) {
		if (connected) {
			cb(null);
			return;
		}
		Debug.Log(Logger.Write("ws.Connect", url));
		rkey = (uint)Util.RandInt();
		Debug.Log(Logger.Write("ws.rkey", rkey.ToString("X")));
		rmask = LCGMask.NumericalRecipes(rkey);
		ws = new WebSocket(url);
		ws.OnError += (sender, e) => {
			Global.Inst.PushTask(delegate () {
				Debug.LogError(Logger.Write("ws.OnError", e.Message));
				ProcClose();
				if (null != cb) {
					cb("NETWORK_ERROR");
					cb = null;
				}
			});
		};
		ws.OnOpen += (sender, e) => {;
			Global.Inst.PushTask(delegate () {
				Debug.Log(Logger.Write("ws.OnOpen"));
				opened = true;
				if (null != premsg) {
					premsg.ForEach((e2) => ProcMsg(e2, ref cb));
					premsg = null;
				}
			});
		};
		ws.OnClose += (sender, e) => {
			Global.Inst.PushTask(delegate () {
				Debug.Log(Logger.Write("ws.OnClose", e.Code, e.Reason));
				ProcClose();
				if (null != cb) {
					cb("CONNECT_FAIL");
					cb = null;
				}
			});
		};
		ws.OnMessage += (sender, e) => {
			Global.Inst.PushTask(delegate () {
				if (!opened) {
					premsg.Add(e);
					return;
				}
				if (null != premsg) {
					premsg.ForEach((e2) => ProcMsg(e2, ref cb));
					premsg = null;
				}
				ProcMsg(e, ref cb);
			});
		};
		Global.Inst.PushTask(delegate () {
			try {
				ws.Connect();
			} catch (Exception e) {
				Debug.LogError(Logger.Write("ws.Connect", "error", e.Message, e.StackTrace));
				ProcClose();
				cb("NETWORK_ERROR");
				cb = null;
			}
		});
	}
	private void ProcMsg(MessageEventArgs e, ref Action<object> cb) {
		Debug.Log(Logger.Write("ws.OnMessage", e.Type, e.RawData.Length));
		if (!connected) {
			ws.Send(BitConverter.GetBytes(rkey));
			skey = BitConverter.ToUInt32(e.RawData, 0);
			smask = LCGMask.NumericalRecipes(skey);
			Debug.Log(Logger.Write("ws.handsake", rkey, skey));
			connected = true;
			if (null != cb) {
				cb(null);
				cb = null;
			}
			return;
		}
		byte[] d = e.RawData;
		d = rmask.mask(d);
		d = Util.Decompress(d);
		string json = Util.Utf8Decode(d);
		OnMsg(json);
	}
	
	public void ProcClose() {
		connected = false;
		ws = null;
		CancelRequest("NETWORK_CLOSE");
		OnClose();
	}
	
	public void Send(string json) {
		byte[] d = Util.Utf8Encode(json);
		d = Util.Compress(d);
		d = smask.mask(d);
		Debug.Log(Logger.Write("ws.Send", json.Length, d.Length));
		ws.Send(d);
	}

	public override void Send(object[] args, Action<object> cb = null) {
		Debug.Log(Logger.Write("send", args));
		string json = Util.JsonEncode(args);
		Send(json);
	}
	
	public override void Close() {
		if (null != ws) ws.Close();
	}
	public event Action OnClose = () => {};
	public virtual void OnMsg(string json) {
		IList l = (IList)Util.JsonDecode(json);
		Util.ForEach<IList>(l, ProcMsg);
	}
	
};

}

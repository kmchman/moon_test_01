using UnityEngine;

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Reflection;

namespace AL {

public class HTTPClient {
	private object handler;
	private string url;
	private MsgRequest msgRequest = new MsgRequest();

	public HTTPClient(object handler, string url) {
		this.handler = handler;
		this.url = url;
	}

	public void CancelRequest(object err) {
		msgRequest.CancelRequest(err);
	}

	private void GetRequestStreamCallback(HttpWebRequest req, IAsyncResult ar, byte[] data, Action<int, object> cb) { 
		try {
			using (Stream rs = req.EndGetRequestStream(ar)) {
				rs.Write(data, 0, data.Length);
				rs.Close();
			}
			req.BeginGetResponse(delegate (IAsyncResult ar2) {
				Response(req, ar2, cb);
			}, null);
		} catch (Exception e) {
			Global.Inst.PushTask(delegate () {
				Debug.LogError(Logger.Write("error", "HTTP.GetRequestStreamCallback", req.RequestUri, e.ToString()));
				cb(0, "NETWORK_ERROR");
			});	
		}
	}

	private void Response(HttpWebRequest req, IAsyncResult ar, Action<int, object> cb) {
		try {			
			HttpWebResponse res = (HttpWebResponse)req.EndGetResponse(ar);
			using (Stream rs = res.GetResponseStream()) {
				byte[] rdata = new byte[8192];
				using (MemoryStream ms = new MemoryStream()) {
					while (rs.CanRead) {
						int rlen = rs.Read(rdata, 0, rdata.Length);
						if (0 == rlen) break;
						ms.Write(rdata, 0, rlen);
					}
					Global.Inst.PushTask(delegate () {
						byte[] data = ms.ToArray();
						Debug.Log("HTTP.Response: " + req.RequestUri + " " + data.Length);
						cb((int)res.StatusCode, data);
					});
					ms.Close();
				}
				rs.Close();
			}
		} catch (Exception e) {
			Global.Inst.PushTask(delegate () {
				Debug.LogError(Logger.Write("error", "HTTP.Response", req.RequestUri, e.ToString()));
				cb(0, "NETWORK_ERROR");
			});
		}
	}

	private void Post(byte[] data, Action<int, object> cb) {
		try {
			Debug.Log(Logger.Write("HTTP.POST: " + url));
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
			req.Timeout = 5000;
			req.Method = "POST";
			req.ContentType = "application/json; charset=UTF-8";
			req.ContentLength = data.Length;
			req.BeginGetRequestStream(delegate (IAsyncResult ar) {
				GetRequestStreamCallback(req, ar, data, cb);
			}, null);
		} catch (Exception e) {
			Global.Inst.PushTask(delegate () {
				Debug.LogError(Logger.Write("error", "HTTP.POST", url, e.ToString()));
				cb(0, "NETWORK_ERROR");
			});
		}
	}

	public void Request(string json, Action<object> cb) {
		Send(json, cb);
	}

	private void Send(string json, Action<object> cb = null) {
		string json2 = '[' + json.Insert(1, string.Format("\"seq\":{0},", msgRequest.GetRequestID(cb))) + ']';
		Debug.Log(Logger.Write("send", json2));
		byte[] utf8 = Util.Utf8Encode(json2);
		Post(utf8, (status, res) => {
			Debug.Log(Logger.Write("recv", status, res));
			if (200 != status) {
				if (null != cb) cb(res.ToString());
				return;
			}
			utf8 = (byte[])res;
			json = Util.Utf8Decode(utf8);
			IList msgs = (IList)Util.JsonDecode(json);
			foreach (IDictionary dic in msgs)
				ProcMsg(dic, cb);
		});
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
#if UNITY_EDITOR || DONT_MUTE_LOG
				MemberInfo[] mis = type.GetMember(member);
				Util.ForEach(mis, Debug.LogError);
#endif
		}
	}

	public void ProcMsg(IDictionary msg, Action<object> cb) {
		Debug.Log(Logger.Write("recv", msg));
		try {
			if (msg.Contains("type"))
			{
				string msgType = msg["type"].ToString();
				switch (msgType)
				{
				case "ntf":
					if (msg.Contains("cmd"))
					{
						string msgCmd = msg["cmd"].ToString();
						string fuctionName = "ntf" + msgCmd.Substring(0, 1).ToUpper() + msgCmd.Substring(1);
						if (msg.Contains("param"))
						{
							Invoke(fuctionName, new object[] { msg["param"] });
						}
					}
					else
						throw new Exception("INVALID_PROC");
					break;
				case "evt":
					if (msg.Contains("cmd"))
					{
						string msgCmd = msg["cmd"].ToString();
						string fuctionName = "evt" + msgCmd.Substring(0, 1).ToUpper() + msgCmd.Substring(1);
						if (msg.Contains("param"))
						{
							Invoke(fuctionName, new object[] { msg["param"] });
						}
					}
					else
						throw new Exception("INVALID_PROC");
					break;
				case "ans":
					if (cb != null)
					{
						if (msg.Contains("err"))
						{
							cb(msg["err"]);
						}
						else
							throw new Exception("INVALID_PROC");
					}
					break;
				default:
					throw new Exception("INVALID_PROC");
				}
			}
			else
				throw new Exception("INVALID_PROC");
		} catch (Exception e) {
			Debug.LogError("ProcMsg Exception: " + e);
		}
	}
}

}

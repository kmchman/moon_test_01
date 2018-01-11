using UnityEngine;

using System;
using System.IO;
using System.Net;
using System.Collections;

namespace AL {

public class WebClient : MsgContext {
	private string url;
	public WebClient(string url, object handler) : base(handler) {
		this.url = url;
	}
	public void SetURL(string url) {
		this.url = url;
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
	public override void Send(object[] args, Action<object> cb = null) {
		Debug.Log(Logger.Write("send", args));
		string json = '['+Util.JsonEncode(args)+']';
		byte[] utf8 = Util.Utf8Encode(json);
		Post(utf8, (status, res) => {
			Debug.Log(Logger.Write("recv", status, res));
			if (200 != status) {
				if (null != cb) cb(res.ToString());
				return;
			}
			utf8 = (byte[])res;
			json = Util.Utf8Decode(utf8);
			IList msgs = (IList)Util.JsonDecode(json);
			Util.ForEach<IList>(msgs, ProcMsg);
		});
	}
};

}

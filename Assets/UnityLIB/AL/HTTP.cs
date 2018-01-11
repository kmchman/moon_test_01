using UnityEngine;

using System;
using System.IO;
using System.Net;
using System.Collections;

namespace AL {

public class HTTP {
	static private void GetRequestStreamCallback(HttpWebRequest req, IAsyncResult ar, byte[] data, Action<int, object> cb) { 
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
	static private void Response(HttpWebRequest req, IAsyncResult ar, Action<int, object> cb) {
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
					Global.Inst.PushTask(() => {
						byte[] data = ms.ToArray();
//						Debug.Log("HTTP.Response: " + req.RequestUri + " " + data.Length);
						cb((int)res.StatusCode, data);
					});
					ms.Close();
				}
				rs.Close();
			}
		} catch (Exception e) {
			Global.Inst.PushTask(() => {
				Debug.LogError(Logger.Write("error", "HTTP.Response", req.RequestUri, e.ToString()));
				cb(0, "NETWORK_ERROR");
			});
		}
	}
	static public void Post(string url, IDictionary hdr, byte[] data, Action<int, object> cb) {
		try {
			Debug.Log("HTTP.POST: " + url);
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
			req.Method = "POST";
			req.ContentType = "application/json; charset=UTF-8";
			req.ContentLength = data.Length;
			if (hdr != null) {
				Util.ForEach<string, string>(hdr, req.Headers.Add);
			}
			req.BeginGetRequestStream(delegate (IAsyncResult ar) {
				GetRequestStreamCallback(req, ar, data, cb);
			}, null);
		} catch (Exception e) {
			Global.Inst.PushTask(() => {
				Debug.LogError(Logger.Write("error", "HTTP.POST", url, e.ToString()));
				cb(0, "NETWORK_ERROR");
			});
		}
	}
};

}

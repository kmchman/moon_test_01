using UnityEngine;

using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Threading;

namespace AL {

public class SimpleHTTP {
	public bool isDone = false;
	public int status = 0;
	public byte[] bytes = null;
	public string error = null;
	public string stack = null;
	public SimpleHTTP() {
	}
	private void GetRequestStreamCallback(HttpWebRequest req, IAsyncResult ar, byte[] data, Action cb) { 
		try {
			using (Stream rs = req.EndGetRequestStream(ar)) {
				rs.Write(data, 0, data.Length);
				rs.Close();
			}
			req.BeginGetResponse(delegate (IAsyncResult ar2) {
				Response(req, ar2, cb);
			}, null);
		} catch (Exception e) {
			error = e.Message;
			stack = e.StackTrace;
			isDone = true;
			if (null != cb) cb();
		}
	}
	private void Response(HttpWebRequest req, IAsyncResult ar, Action cb) {
		try {			
			HttpWebResponse res = (HttpWebResponse)req.EndGetResponse(ar);
			status = (int)res.StatusCode;
			using (Stream rs = res.GetResponseStream()) {
				byte[] rbytes = new byte[8192];
				using (MemoryStream ms = new MemoryStream()) {
					while (rs.CanRead) {
						int rlen = rs.Read(rbytes, 0, rbytes.Length);
						if (0 == rlen) break;
						ms.Write(rbytes, 0, rlen);
					}
					bytes = ms.ToArray();
					ms.Close();
				}
				rs.Close();
			}
		} catch (Exception e) {
			error = e.Message;
			stack = e.StackTrace;
		} finally {
			if (!isDone) {
				isDone = true;
				if (null != cb) cb();
			}
		}
	}
	public void GET(string url, IDictionary hdr, Action cb) {
		try {
			Debug.Log("HTTP.GET: " + url);
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
			req.Method = "GET";
			if (hdr != null) {
				foreach (DictionaryEntry e in hdr) {
					req.Headers.Add(e.Key.ToString(), e.Value.ToString());
				}
			}
			req.BeginGetResponse((IAsyncResult ar) => {
				Response(req, ar, cb);
			}, null);
		} catch (Exception e) {
			error = e.Message;
			stack = e.StackTrace;
			isDone = true;
			if (null != cb) cb();
		}
	}
	public void POST(string url, IDictionary hdr, byte[] bytes, Action cb) {
		try {
			Debug.Log("HTTP.POST: " + url);
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
			req.Method = "POST";
			req.ContentType = "application/json; charset=UTF-8";
			req.ContentLength = bytes.Length;
			if (hdr != null) {
				foreach (DictionaryEntry e in hdr) {
					req.Headers.Add(e.Key.ToString(), e.Value.ToString());
				}
			}
			req.BeginGetRequestStream(delegate (IAsyncResult ar) {
				GetRequestStreamCallback(req, ar, bytes, cb);
			}, null);
		} catch (Exception e) {
			error = e.Message;
			stack = e.StackTrace;
			isDone = true;
			if (null != cb) cb();
		}
	}
	static public SimpleHTTP AsyncGET(string url, Action<SimpleHTTP> cb, IDictionary hdr = null) {
		SimpleHTTP http = new SimpleHTTP();
		http.GET(url, hdr, () => {
			Logger.Write("done", url, http.status, http.bytes != null ? http.bytes.Length : 0, http.error, http.stack);
			if (null != cb ) cb(http);
		});
		return http;
	}
	static public SimpleHTTP TaskGET(string url, Action<SimpleHTTP> cb, IDictionary hdr = null) {
		SimpleHTTP http = AsyncGET(url, (http2) => {
			Global.Inst.PushTask(() => {
				if (null != cb) cb(http2);
			});
		}, hdr);
		return http;
	}
	static public SimpleHTTP SyncGET(string url, Action<SimpleHTTP> cb = null, IDictionary hdr = null) {
		EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.ManualReset);
		SimpleHTTP http = AsyncGET(url, (http2) => {
			ewh.Set();
		}, hdr);
		ewh.WaitOne();
		if (null != cb ) cb(http);
		return http;
	}
};

}

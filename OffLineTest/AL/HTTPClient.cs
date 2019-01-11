using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;

namespace AL {

public class HTTPClient {
	private const int DefaultTimeout = 10000;	// 10초 (ms 단위)
	
	public class RequestData
	{
		public HttpWebRequest request;
	}

	private object handler;
	private string url;
	private MsgRequest msgRequest = new MsgRequest();
	
	private bool isUseCustomTimeout = false;
	private int timeout = DefaultTimeout;
	
	private bool isUseSeqToken = false;
	private long seqToken;
	
	private Dictionary<int, RequestData> requestDatas = new Dictionary<int, RequestData>();

	public HTTPClient(object handler, string url) {
		this.handler = handler;
		this.url = url;
		CreateSeqToken();
	}

	public void CreateSeqToken()
	{
		seqToken = DateTime.UtcNow.Ticks / 10000;
	}

	public void CancelRequest(object err) {
		msgRequest.CancelRequest(err);
	}
	
	// 비동기 방식에서는 기본적인 타임아웃 처리가 되지 않기 때문에 개별 처리를 해야 한다.
	public void SetUseCustomTimeout(bool isUseCustomTimeout)
	{
		this.isUseCustomTimeout = isUseCustomTimeout;
	}
	
	public void SetTimeout(int timeout)
	{
		this.timeout = timeout;
	}

	public void SetUseSeqToken(bool isUseSeqToken)
	{
		this.isUseSeqToken = isUseSeqToken;
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

	private void Post(int id, byte[] data, bool isALE, Action<object> cb, int timeout, Action<Action> cbRetry) {
		RequestData requestData = isUseCustomTimeout ? new RequestData() : null;
		
		// error callback
		Action<object> error = (res) => {
			if (null != cbRetry) { cbRetry(() => { Post(id, data, isALE, cb, timeout, cbRetry); }); }
			else if (null != cb) cb(res.ToString());
		};
		
		// callback
		Action<int, object> cb2 = (status, res) => {
			// 콜백을 받은 상황에서는 이미 PushTask 로 들어온 것이기 때문에 PushTask 로 다시 처리할 필요가 없다.
			Debug.Log(Logger.Write("recv", status, res));
			
			if (requestDatas.ContainsKey(id) && requestDatas[id].Equals(requestData))
			{
				requestData.request = null;
				requestDatas.Remove(id);
			}
			
			if (HttpStatusCode.OK != (HttpStatusCode)status) {
				error(res);
				return;
			}
			
			try {
				byte[] utf8 = isALE ? Util.ALEDecode((byte[])res) : (byte[])res;
				string json = Util.Utf8Decode(utf8);
				IList msgs = (IList)Util.JsonDecode(json);
				if (null != msgs) {
					foreach (IDictionary dic in msgs)
						ProcMsg(dic, cb);
				}
				else {
					Debug.LogError(Logger.Write("error", "HTTP.Post", "Util.JsonDecode"));
					error("NETWORK_ERROR");
				}
			}
			catch (Exception e)
			{
				Debug.LogError(Logger.Write("error", "HTTP.Post", e.ToString()));
				error("NETWORK_ERROR");
			}
		};
		
		try {
			Debug.Log(Logger.Write("HTTP.POST: " + url));
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
			req.ServicePoint.Expect100Continue = false;
			req.Method = "POST";
			if (isALE)
				req.ContentType = "application/octet-stream";
			else
				req.ContentType = "application/json; charset=UTF-8";
			req.ContentLength = data.Length;
			
			if (isUseCustomTimeout)
			{
				requestData.request = req;
				requestDatas.Add(id, requestData);

				Global.Inst.PushTimeout(timeout < 0 ? this.timeout : timeout, () => {
					if (requestDatas.ContainsKey(id) && requestDatas[id].Equals(requestData))
						requestData.request.Abort();
				});
			}
			
			req.BeginGetRequestStream(delegate (IAsyncResult ar) {
				GetRequestStreamCallback(req, ar, data, cb2);
			}, null);
		} catch (Exception e) {
			Global.Inst.PushTask(delegate () {
				Debug.LogError(Logger.Write("error", "HTTP.POST", url, e.ToString()));
				cb2(0, "NETWORK_ERROR");
			});
		}
	}
		
	public void Request(string json, Action<object> cb, int timeout = -1, Action<Action> cbRetry = null) {
		Send(json, cb, false, timeout, cbRetry);
	}

	public void ALERequest(string json, Action<object> cb, int timeout = -1, Action<Action> cbRetry = null) {
		Send(json, cb, true, timeout, cbRetry);
	}

	private void Send(string json, Action<object> cb, bool isALE, int timeout, Action<Action> cbRetry) {
			// OffLineTest
			return;
		int id = msgRequest.GetRequestID(cb);
		string json2;
		if (isUseSeqToken)
			json2 = '[' + json.Insert(1, string.Format("\"seq\":{0},\"seqToken\":{1},", id, seqToken)) + ']';
		else
			json2 = '[' + json.Insert(1, string.Format("\"seq\":{0},", id)) + ']';
		Debug.Log(Logger.Write("send", json2));
		byte[] utf8 = Util.Utf8Encode(json2);
		if (isALE)
			utf8 = Util.ALEEncode(utf8);
		
		Post(id, utf8, isALE, cb, timeout, cbRetry);
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
						Invoke(fuctionName, msg.Contains("param") ? new object[] { msg["param"] } : null);
					}
					else
						throw new Exception("INVALID_PROC");
					break;
				case "evt":
					if (msg.Contains("cmd"))
					{
						string msgCmd = msg["cmd"].ToString();
						string fuctionName = "evt" + msgCmd.Substring(0, 1).ToUpper() + msgCmd.Substring(1);
						Invoke(fuctionName, msg.Contains("param") ? new object[] { msg["param"] } : null);
					}
					else
						throw new Exception("INVALID_PROC");
					break;
				case "ans":
					if (msg.Contains("seq"))
					{
						int id = int.Parse(msg["seq"].ToString());
						msgRequest.ReleaseRequestID(id);
					}
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

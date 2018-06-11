using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace AL
{
	public class SingRouterHandler
	{
		public static SingRouterHandler Inst { get { return _inst; } }
		private static SingRouterHandler _inst = new SingRouterHandler();

		public string serverURL { get { return _serverURL; } }

		private HTTPClient _httpClient = null;

		private string _serverURL = "";

		private SingRouterHandler()
		{
//			this.setURL(Global.cfg.GetString("singrouterURL", ""));
		}

		public void setURL(string url)
		{
			_httpClient = new HTTPClient(this, url);
		}

		// Do
		public void doSingServerInfo(string projectCode, string game, string version, Action<object> cb)
		{
			_httpClient.Request(requestJsonString("singServerInfo", "prjCode", projectCode, "game", game, "version", version), cb);
		}

		// Evt
		public void evtSingServerInfo(IDictionary d)
		{
			if(d.Contains("serverURL"))
			{
				_serverURL = (string)d["serverURL"];
			}
		}

		// Private
		private string requestJsonString(string cmd, params object[] param)
		{
			if(param == null || param.Length % 2 != 0) 
			{
				Debug.LogError(Logger.Write("error", cmd));
				return "{}"; 
			}

			Hashtable hs = new Hashtable();
			hs["type"] = "req";
			hs["cmd"] = cmd;

			if(param.Length > 0)
			{
				Hashtable hs2 = new Hashtable();
				for(int i = 0; i < param.Length; i += 2)
				{
					hs2[param[i]] = param[i + 1];
				}
				hs["param"] = hs2;
			}


			return Util.JsonEncode2(hs);
		}
	}
}
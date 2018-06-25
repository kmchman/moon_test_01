using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Datatable : _Datatable
{
	private const string DATATABLE_NAME = "giantdatas";

	private static Datatable _inst = new Datatable();
	public static Datatable Inst { get { return _inst; } }

	public void LoadDt()
	{
		foreach(var value in Global.dtList)
		{
			string data = (Resources.Load("DT/" + value) as TextAsset).text;
			IList dataList = (IList)Util.JsonDecode(data);

			for (int i = 1; i < dataList.Count; i++) {
				IDictionary dic = (IDictionary)dataList[i];	

				switch (value) {
				case "HeroData":
					LoadHeroData(dic);
					break;
				case "CharData":
					LoadCharData(dic);
					break;
				}
			}	
		}
	}
}
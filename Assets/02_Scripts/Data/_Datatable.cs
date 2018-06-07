#pragma warning disable 114
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class _Datatable {
	public class CharData {
		public int ID;
		public int JobTitle;
		public string Race;
		public string Role;
		public string Attack_RangeType;
		public int MoveSpeed;
		public int ReservedPos;
		public int Protect;
		public int Scale;
		public int EffectScale;
		public int UIScale;
		public int DialogScale;
		public float DialogOffsetX;
		public int DialogOffsetY;
		public int PrefabName;
		public string FaceImage;
	};
	public Dictionary<int, CharData> dtCharData = new Dictionary<int, CharData>();
	public void LoadCharDatas(IDictionary dic) {
		var enumerator = dic.GetEnumerator();
		while (enumerator.MoveNext()) {
			LoadCharData(enumerator.Key, enumerator.Value);
		}
	}
	public void LoadCharData(object key, object value) {
//		IDictionary v = (IDictionary)value;
//
//		CharData i = new CharData();
//	i.ID = *int.Parse((string)v["ID"]);
//	i.JobTitle = int.Parse((string)v["JobTitle"]);
//	i.Race = String.Parse((string)v["Race"]);
//	i.Role = mtext.Parse((string)v["Role"]);
//	i.Attack_RangeType = String.Parse((string)v["Attack_RangeType"]);
//	i.MoveSpeed = int.Parse((string)v["MoveSpeed"]);
//	i.ReservedPos = int.Parse((string)v["ReservedPos"]);
//	i.Protect = int.Parse((string)v["Protect"]);
//	i.Scale = int.Parse((string)v["Scale"]);
//	i.EffectScale = int.Parse((string)v["EffectScale"]);
//	i.UIScale = int.Parse((string)v["UIScale"]);
//	i.DialogScale = int.Parse((string)v["DialogScale"]);
//	i.DialogOffsetX = float.Parse((string)v["DialogOffsetX"]);
//	i.DialogOffsetY = int.Parse((string)v["DialogOffsetY"]);
//	i.PrefabName = int.Parse((string)v["PrefabName"]);
//	i.FaceImage = String.Parse((string)v["FaceImage"]);
//	dtCharData.Add(i.int, i);
	}
}

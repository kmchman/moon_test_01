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
		IDictionary v = (IDictionary)value;

		CharData i = new CharData();
	}
}

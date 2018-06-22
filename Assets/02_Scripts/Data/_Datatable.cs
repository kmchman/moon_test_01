#pragma warning disable 114
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class _Datatable {
	public class CharData {
		public int ID;
		public string JobTitle;
		public string Race;
		public string Role;
		public bool Attack_RangeType;
		public float MoveSpeed;
		public int ReservedPos;
		public int Protect;
		public float Scale;
		public float EffectScale;
		public float UIScale;
		public float DialogScale;
		public float DialogOffsetX;
		public float DialogOffsetY;
		public string PrefabName;
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
		i.ID = int.Parse((string)v["ID"]);
		i.JobTitle = (string)v["JobTitle"];
		i.Race = (string)v["Race"];
		i.Role = (string)v["Role"];
		i.Attack_RangeType = bool.Parse((string)v["Attack_RangeType"]);
		i.MoveSpeed = float.Parse((string)v["MoveSpeed"]);
		i.ReservedPos = int.Parse((string)v["ReservedPos"]);
		i.Protect = int.Parse((string)v["Protect"]);
		i.Scale = float.Parse((string)v["Scale"]);
		i.EffectScale = float.Parse((string)v["EffectScale"]);
		i.UIScale = float.Parse((string)v["UIScale"]);
		i.DialogScale = float.Parse((string)v["DialogScale"]);
		i.DialogOffsetX = float.Parse((string)v["DialogOffsetX"]);
		i.DialogOffsetY = float.Parse((string)v["DialogOffsetY"]);
		i.PrefabName = (string)v["PrefabName"];
		i.FaceImage = (string)v["FaceImage"];
		dtCharData.Add(i.ID, i);
	}
	public class HeroData {
		public int HeroID;
		public int oldheroid;
		public int CharID;
		public string OriginalName;
		public string Name;
		public string Role;
		public int ItemTreeID;
		public int InitEvoGrade;
		public int StatDataID;
		public int BonusStatGroupID;
		public string ListState;
		public string PvpPrefabName;
		public string PvpFaceImage;
		public int InscriptionID_0;
		public int InscriptionID_1;
		public int InscriptionID_2;
	};
	public Dictionary<int, HeroData> dtHeroData = new Dictionary<int, HeroData>();
	public void LoadHeroDatas(IDictionary dic) {
		var enumerator = dic.GetEnumerator();
		while (enumerator.MoveNext()) {
			LoadHeroData(enumerator.Key, enumerator.Value);
		}
	}
	public void LoadHeroData(object key, object value) {
		IDictionary v = (IDictionary)value;

		HeroData i = new HeroData();
		i.HeroID = int.Parse((string)v["HeroID"]);
		i.oldheroid = int.Parse((string)v["oldheroid"]);
		i.CharID = int.Parse((string)v["CharID"]);
		i.OriginalName = (string)v["OriginalName"];
		i.Name = (string)v["Name"];
		i.Role = (string)v["Role"];
		i.ItemTreeID = int.Parse((string)v["ItemTreeID"]);
		i.InitEvoGrade = int.Parse((string)v["InitEvoGrade"]);
		i.StatDataID = int.Parse((string)v["StatDataID"]);
		i.BonusStatGroupID = int.Parse((string)v["BonusStatGroupID"]);
		i.ListState = (string)v["ListState"];
		i.PvpPrefabName = (string)v["PvpPrefabName"];
		i.PvpFaceImage = (string)v["PvpFaceImage"];
		i.InscriptionID_0 = int.Parse((string)v["InscriptionID_0"]);
		i.InscriptionID_1 = int.Parse((string)v["InscriptionID_1"]);
		i.InscriptionID_2 = int.Parse((string)v["InscriptionID_2"]);
		dtHeroData.Add(i.HeroID, i);
	}
	public void LoadDatatable()
	{
//	CharData
//	HeroData
	}
}

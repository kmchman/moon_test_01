using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Datatable : _Datatable
{
	private const string DATATABLE_NAME = "giantdatas";

	private static Datatable _inst = new Datatable();
	public static Datatable Inst { get { return _inst; } }

	public void LoadDt(IDictionary dic)
	{
		LoadCharDatas(dic);
	}

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MMenu {
	
	[MenuItem("MProject/Generate Data/Table", false, 0)]
	static public void GenerateTable()
	{
		MCodeGenerator.GenerateDatatable();
//		MCodeGenerator.GenDatatable ();
	}

}

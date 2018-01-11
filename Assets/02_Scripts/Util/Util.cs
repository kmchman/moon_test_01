using UnityEngine;
using System.Collections;

namespace Moon {

	public class Util {
		
		public static T MakeItem<T>(T prefab, Transform parent, bool isWorldPosition) where T : UnityEngine.Object
		{
			return UnityEngine.GameObject.Instantiate(prefab, parent, isWorldPosition) as T;
		}
	}
}



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameObjectPool
{
	private static Dictionary<string, Object> dicPrefab = new Dictionary<string, Object>();								// prefab name, Prefab Object
	private static Dictionary<string, Stack<GameObject>> dicGameObject = new Dictionary<string, Stack<GameObject>>();	// prefab name, GameObject
	private static List<string> dontClearList = new List<string>();

	public static bool HasObjectInPool(Object prefab)
	{
		return HasObjectInPool(prefab.name);
	}

	public static bool HasObjectInPool(string prefabName)
	{
		return dicGameObject.ContainsKey(prefabName) && dicGameObject[prefabName].Count > 0;
	}

	public static bool ContainsKey(Object prefab)
	{
		return ContainsKey(prefab.name);
	}

	public static bool ContainsKey(string prefabName)
	{
		return dicGameObject.ContainsKey(prefabName);
	}

	public static Object GetPrefab(string prefabName)
	{
		if (dicPrefab.ContainsKey(prefabName))
			return dicPrefab[prefabName];
		return null;
	}

	public static void RegisterPrefab(string prefabName, Object prefab, bool isGameObject)
	{
		if (prefab == null)
			return;
		dicPrefab.Add(prefabName, prefab);
		if (isGameObject)
			dicGameObject.Add(prefabName, new Stack<GameObject>());
	}

	private static GameObject PopInternal(Object prefab, bool isAssetBundle)
	{
		string prefabName = prefab.name;
		bool isExistKey = false;
		GameObject obj = null;
		if (dicGameObject.ContainsKey(prefabName))
		{
			Stack<GameObject> gameObjectStack = dicGameObject[prefabName];

			while (gameObjectStack.Count > 0 && obj == null)
				obj = dicGameObject[prefabName].Pop();

			if (obj != null)
			{
				obj.SetActive(true);
				return obj;
			}

			isExistKey = true;
		}

		obj = Object.Instantiate(prefab) as GameObject;
		if (obj == null)
		{
			Debug.LogError("GameObjectPool.Instantiate Failed.");
			return null;
		}

		// 이름에서 "(Clone)" 제거
		obj.name = prefab.name;

		obj.SetActive(true);

//#if UNITY_EDITOR && !USE_ASSETDATABASE
//		if (isAssetBundle)
//			Giant.Util.RemapShader(obj);
//#endif

		if (!isExistKey)
			RegisterPrefab(prefabName, prefab, true);
		return obj;
	}

	public static GameObject Pop(Object prefab, bool isAssetBundle)
	{
		return PopInternal(prefab, isAssetBundle);
	}

	public static GameObject Pop(Object prefab, bool isAssetBundle, Transform parent, bool instantiateInWorldSpace)
	{
		GameObject obj = PopInternal(prefab, isAssetBundle);

		if (obj != null)
		{
			Transform prefabTransform = (prefab as GameObject).transform;
			Transform objTransform = obj.transform;

			objTransform.SetParent(parent, false);
			objTransform.SetAsLastSibling();
			objTransform.localScale = Vector3.one;

			if (instantiateInWorldSpace)
			{
				objTransform.SetPositionAndRotation(prefabTransform.position, prefabTransform.rotation);
			}
			else
			{
				objTransform.localRotation = prefabTransform.localRotation;

				if (prefabTransform is RectTransform)
					(objTransform as RectTransform).anchoredPosition3D = (prefabTransform as RectTransform).anchoredPosition3D;
				else
					objTransform.localPosition = prefabTransform.localPosition;
			}
		}

		return obj;
	}

	public static GameObject Pop(Object prefab, bool isAssetBundle, Vector3 pos, Quaternion rot, bool isLocal, Transform parent)
	{
		GameObject obj = PopInternal(prefab, isAssetBundle);

		if (obj != null)
		{
			Transform objTransform = obj.transform;
			objTransform.SetParent(parent);
			objTransform.localScale = Vector3.one;
			if (isLocal)
			{
				objTransform.localPosition = pos;
				objTransform.localRotation = rot;
			}
			else
			{
				objTransform.SetPositionAndRotation(pos, rot);
			}
		}

		return obj;
	}

	public static GameObject Pop(string prefabName, bool isAssetBundle)
	{
		if (!ContainsKey(prefabName))
			return null;
		return Pop(dicPrefab[prefabName], isAssetBundle);
	}

	public static GameObject Pop(string prefabName, bool isAssetBundle, Vector3 pos, Quaternion rot, bool isLocal, Transform parent)
	{
		if (!ContainsKey(prefabName))
			return null;
		return Pop(dicPrefab[prefabName], isAssetBundle, pos, rot, isLocal, parent);
	}

	public static void Push(GameObject obj)
	{
		string prefabName = obj.name;
		if (dicGameObject.ContainsKey(prefabName))
		{
			obj.SetActive(false);
			dicGameObject[prefabName].Push(obj);
		}
	}

	public static void ClearGameObject()
	{
		List<string> tempKeyList = null;
		tempKeyList = dicPrefab.Keys.ToList();
		for(int i = 0; i < tempKeyList.Count; i++)
		{
			string tempKey = tempKeyList[i];
			if(!dontClearList.Contains(tempKey))
				dicPrefab.Remove(tempKey);
		}

		tempKeyList = dicGameObject.Keys.ToList();
		for(int i = 0; i < tempKeyList.Count; i++)
		{
			string tempKey = tempKeyList[i];
			if(!dontClearList.Contains(tempKey))
				dicGameObject.Remove(tempKey);
		}
	}
}

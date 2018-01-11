using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BaseOperatorUnit : MonoBehaviour {

	static private BaseOperatorUnit 	s_Instance;
	static public BaseOperatorUnit 		instance { get { return s_Instance;}}

	void Awake()
	{
		s_Instance = this;
	}

	void Start()
	{
		DontDestroyOnLoad(this);
	}


	public void LoadLevel_Async(string _sceneName)
	{
		SceneManager.LoadSceneAsync(_sceneName);
	}

//	private int a = 0;
//	// Update is called once per frame
//	void Update () {
//
//		Debug.Log("Update Start"); 
//
//		StartCoroutine(test1()); 
//
//		Debug.Log("Update End"); 	
//	}
//
//	IEnumerator test1() 
//	{ 
//		Debug.Log("test1-1-" + a); 
//		yield return null; 
//		a++; 
//		Debug.Log("test1-2-" + a); 
//	} 
}

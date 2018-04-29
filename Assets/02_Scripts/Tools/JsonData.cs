using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonData : MonoBehaviour {

	string fileName = "data.json";
	string path;

	GameData gameData = new GameData();

	// Use this for initialization
	void Start () {
		path = Application.persistentDataPath + "/" + fileName;
		Debug.Log (path);
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.S)) {
			
			gameData.time = System.DateTime.Now.ToShortTimeString ();
			gameData.date = System.DateTime.Now.ToShortDateString ();
			Quest q1 = new Quest ();
			q1.name = "q1 name";
			q1.desc = "q1 desc";
			gameData.quests.Add (q1);

			Quest q2 = new Quest ();
			q2.name = "q2 name";
			q2.desc = "q2 desc";
			gameData.quests.Add (q2);

			SaveData ();
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			ReadData ();
		}
	}

	void SaveData()
	{
		JsonWrapper wrapper = new JsonWrapper ();
		wrapper.gameData = gameData;
		string contents = JsonUtility.ToJson (wrapper, true);
		System.IO.File.WriteAllText(path, contents);

		foreach (Quest q in wrapper.gameData.quests) {
			Debug.Log ("quest data : " + q.name);
		}
	}
	void ReadData()
	{
		string contents = System.IO.File.ReadAllText (path);
		IDictionary dict = (IDictionary)Util.JsonDecode (contents);
		Debug.Log (dict );

		//IDictionary dic = JsonUtility.FromJson<IDictionary> (contents);
		//Debug.Log (dic);
		/*
		wrapper = JsonUtility.FromJson<JsonWrapper> (contents);
		Debug.Log ("gameData date : " + wrapper.gameData.date + "gameData date : " + wrapper.gameData.time);
		*/
	}
}

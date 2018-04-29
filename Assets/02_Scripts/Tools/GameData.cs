using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData {
	public string date;
	public string time;
	public List<Quest> quests = new List<Quest>();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUIHandler : MonoBehaviour {

	[SerializeField] private Transform editPanelParent;
	[SerializeField] private Object editPanelPrefab;

	private GameFormEditPanel editPanel = null;

	// handler
	[SerializeField] private BattleHandler handler;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowGameFormEdit(GiantEPData epData)
	{
		if (editPanel == null)
		{
			editPanel = Giant.Util.MakeItem<GameFormEditPanel>(editPanelPrefab, editPanelParent, false);
			editPanel.onClickStartAction += OnClickStartBattle;
		}
		else
			editPanel.gameObject.SetActive(true);

		editPanel.ShowPanel(epData, Vector3.zero);
	}

	public void OnClickStartBattle()
	{
		editPanel.gameObject.SetActive(false);

//		handler.StartGameAfterEdit();
	}
}

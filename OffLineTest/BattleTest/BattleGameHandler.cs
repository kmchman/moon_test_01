using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGameHandler : MonoBehaviour {

	[SerializeField] private Transform m_CharacterParent;
	[SerializeField] private StageHelperHandler m_StageHelperHandler;

	private BattleHandler m_Handler;
	private BattleUIHandler m_UIHandler;

	public Transform characterParent { get; private set; }
	public StageHelperHandler stageHelperHandler { get; private set; }

	private void Awake()
	{
		characterParent = m_CharacterParent;
		stageHelperHandler = m_StageHelperHandler;

//		m_SkillAutoInfos = new SkillAutoInfo[2];
//		for (int i = 0; i < m_SkillAutoInfos.Length; ++i)
//			m_SkillAutoInfos[i] = new SkillAutoInfo();

		if (PreGameHandler.Inst != null)
			PreGameHandler.Inst.transform.SetParent(transform.parent, true);
	}

	public void SetHandler(BattleHandler handler, BattleUIHandler uiHandler)
	{
		m_Handler = handler;
		m_UIHandler = uiHandler;
	}
}

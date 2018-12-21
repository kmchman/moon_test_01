using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using STExtensions;

public class UIBaseLvUnlockContentsLinker : MonoBehaviour, IPlayerDataField
{
	[SerializeField] private UNLOCK_CONTENTS			m_UnlockType;

	[SerializeField] private STButton					m_Btn;
	[SerializeField] private STText						m_Text;
	[SerializeField] private GameObject[]				m_Objs;

	public bool isUnlock { get; private set; }

	private CanvasGroup									m_CanvasGroup;
	private Color										m_TextColor;

	private void Awake()
	{
		// TODO : 임시처리. 가능하다면 BaseLvUnlockContents가 아닌 다른 방법을 사용 할 것.
		// 가령 레벨업 변화를 감지해서 바꾸는게 아니라, 레벨업을 하는곳에서 감지해서 바뀜을 당하는쪽으로 수정할 것
//		isUnlock = Datatable.Inst.IsUnlockContents(m_UnlockType);
	}

	private void Start()
	{
		m_CanvasGroup = gameObject.GetComponent<CanvasGroup>();
		if (m_CanvasGroup == null)
			m_CanvasGroup = gameObject.AddComponent<CanvasGroup>();

		if(m_Text != null)
			m_TextColor = m_Text.color;

		if(!isUnlock)
			m_Btn.isCheckRunEnableFunc += OnCheckButtonRunEnable;
		PlayerDataStore.RegistCallback(PlayerDataFieldType.PlayerLv, this);
	}

	private void OnDestroy()
	{
		m_Btn.isCheckRunEnableFunc -= OnCheckButtonRunEnable;
		PlayerDataStore.UnregistCallback(PlayerDataFieldType.PlayerLv, this);
	}

	public void UpdateData(PlayerDataFieldType type, string data)
	{
		UpdateControls();
	}

	private bool OnCheckButtonRunEnable()
	{
//		if(!isUnlock)
//			GlobalSystemRewardMsgHandler.instance.ShowMessage(Datatable.Inst.GetUnlockContentsLevelString(m_UnlockType));
//		return isUnlock;
		return false;
	}

	private void UpdateControls()
	{
//		isUnlock = Datatable.Inst.IsUnlockContents(m_UnlockType);
//
//		m_Btn.image.SetGrayScale(!isUnlock);
//
//		for (int i = 0; i < m_Objs.Length; ++i)
//			m_Objs[i].SetActive(isUnlock);
//
//		if (isUnlock)
//		{
//			if(m_Text != null)
//				m_Text.color = m_TextColor;
//			m_CanvasGroup.alpha = 1f;
//		}
//		else
//		{
//			if(m_Text != null)
//				m_Text.color = GlobalDataStore.Inst.GetGameColor(ColorType.Disable);
//			m_CanvasGroup.alpha = 0.5f;
//		}
	}
}

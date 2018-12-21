using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using System.Linq;

using DG.Tweening;
using STDOTweenExtensions;

public enum HudType
{
	BackButton,
	NickName,
	Ether,
	GoldAndCash,
	BaseResource,
	ChatButton,
	MailBoxButton,
	GameSettingButton,
	NoticeButton,
	WorldMapButton,
	InventoryButton,
	FoldGroupButton,
	PVPButton,
	Ethereum,
	HotTimeButton,
	ArenaCurrency,
	WebEvent,
	WebEventSpecial,
	DailyQuest,
	GuildPoint,
	GuildCoin,
	WorldBossButton,
	ArenaButton,
	GuildButton,
	GuildResource,
	AchievementButton,
	HeroButton,
	CoreButton,
	AssignButton,
	BaseButton,
	ShopButton,
	WorldBossButtonEx,
	GameFormSettingButton,
	DuelButton,
}

public class HudManager : MonoBehaviour
{
	[System.Serializable]
	public struct HudData
	{
		public HudType hudType;
		public GameObject hudObject;

		public RectTransform hudRectTransform
		{
			get
			{
				if (m_HudRectTransform == null)
					m_HudRectTransform = hudObject.transform as RectTransform;
				return m_HudRectTransform;
			}
		}

		private RectTransform m_HudRectTransform;
	}

	public static HudManager inst { get { return m_Inst; } }
	private static HudManager m_Inst;

	[SerializeField] private STText m_TitleText;
	[SerializeField] private HudData[] m_HudDatas;
	[SerializeField] private RectTransform 	m_RefreshLayoutGroup;

	private HashSet<HudType> m_TempHudTypes = new HashSet<HudType>();

	private STAnimationItem m_NickNameAnimationItem;
	private GameObject m_EthereumObject;

	private void Awake()
	{
		m_Inst = this;

		Init();
	}

	private void Init()
	{
		if (m_HudDatas == null || m_HudDatas.Length <= 0)
			return;
		
		for (int i = 0; i < m_HudDatas.Length; ++i)
		{
			switch (m_HudDatas[i].hudType)
			{
			case HudType.BackButton:
				SetBackButtonListener(m_HudDatas[i].hudObject);
				break;

			case HudType.NickName:
				FindNickNameAnimationItem(m_HudDatas[i].hudObject);
				break;
				
			case HudType.Ethereum:
				m_EthereumObject = m_HudDatas[i].hudObject;
				break;
			}
		}
	}

	public bool IsActiveHudObject(HudType hudType)
	{
		for (int i = 0; i < m_HudDatas.Length; ++i)
		{
			if (hudType.Equals(m_HudDatas[i].hudType))
				return true;
		}
		return false;
	}

	private void SetBackButtonListener(GameObject hudObject)
	{
		if (hudObject == null)
			return;

		Button button = hudObject.GetComponent<Button>();
		if (button == null)
			return;

		button.onClick.RemoveListener(OnClickBackButton);
		button.onClick.AddListener(OnClickBackButton);
	}

	private void FindNickNameAnimationItem(GameObject hudObject)
	{
		if (hudObject == null)
			return;

		m_NickNameAnimationItem = hudObject.GetComponent<STAnimationItem>();
		m_NickNameAnimationItem.SetDontRepeat(true);
	}

	public void SetHud(HudType[] activeHuds)
	{
//		if (m_HudDatas == null)
//			return;
//
//		m_TempHudTypes.Clear();
//
//		for (int i = 0; i < activeHuds.Length; ++i)
//			m_TempHudTypes.Add(activeHuds[i]);
//
//		bool isActive;
//		for (int i = 0; i < m_HudDatas.Length; ++i)
//		{
//			isActive = m_TempHudTypes.Contains(m_HudDatas[i].hudType);
//
//			if (m_HudDatas[i].hudObject.activeSelf != isActive)
//				m_HudDatas[i].hudObject.SetActive(isActive);
//			
//			switch (m_HudDatas[i].hudType)
//			{
//			case HudType.GuildButton:
//				m_HudDatas[i].hudObject.SetActive(isActive && Datatable.Inst.GetCurrentGBSeason() > 0 && AccountDataStore.instance.IsOnGuildMembership());
//				break;
//			case HudType.WorldBossButtonEx:
//				m_HudDatas[i].hudObject.SetActive(isActive && AccountDataStore.instance.worldBossSeasonInfo.isOnSeason);
//				break;
//			}
//		}
//		
//		if (!Datatable.Inst.IsDungeonOpen() && m_EthereumObject != null)
//			m_EthereumObject.SetActive(false);
//
//		if(m_RefreshLayoutGroup!= null)
//			LayoutRebuilder.ForceRebuildLayoutImmediate(m_RefreshLayoutGroup);
//		StartCoroutine(CheckNickNameAnimation(m_TempHudTypes));
	}

	public void SetTitle(UITextEnum textEnum)
	{
		string title = string.Empty;

//		if (textEnum != UITextEnum.NONE)
//			title = Datatable.Inst.GetUIText(textEnum);
		
		SetTitle(title);
	}

	public void SetTitle(string title)
	{
		if (m_TitleText == null)
			return;

		m_TitleText.text = title;
	}

	private IEnumerator CheckNickNameAnimation(HashSet<HudType> hudTypes)
	{
		if (!hudTypes.Contains(HudType.NickName))
			yield break;
		
		yield return null;
		
		if (hudTypes.Contains(HudType.BackButton))
			PlayNickNameAnimation("WithBackButton");
		else
			PlayNickNameAnimation("WithoutBackButton");
	}

	private void PlayNickNameAnimation(string animationName)
	{
		if (m_NickNameAnimationItem == null || !m_NickNameAnimationItem.gameObject.activeInHierarchy)
			return;

		m_NickNameAnimationItem.Play(animationName, 0);
	}

	private void OnCompleteNickNameAnimation(STEffectItem item)
	{
		(item as STAnimationItem).TakeSnapshot();
	}

	private void OnClickBackButton()
	{
//		if (BackButtonManager.Inst != null)
//			BackButtonManager.Inst.InvokeBackButton();
	}
}
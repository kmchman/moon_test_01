using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

public class InputBlocker : MonoBehaviour
{
	public static InputBlocker inst { get { return m_Inst; } }
	private static InputBlocker m_Inst;

	public bool isBlock { get { return m_InputBlockObject.activeSelf; } }
	public bool isHiveBlock{ get { return m_BlockByHive; } }

	[SerializeField] private GameObject m_InputBlockObject;

	private Dictionary<object, int> m_BlockCustom = new Dictionary<object, int>();

	private HashSet<STRequest> m_BlockRequests = new HashSet<STRequest>();

	private HashSet<STEffectItem> m_BlockEffectItems = new HashSet<STEffectItem>();
	private HashSet<Tween> m_BlockDOTweens = new HashSet<Tween>();
	private HashSet<UITweenEffect> m_BlockUITweenEffects = new HashSet<UITweenEffect>();
	private HashSet<PopupUI> m_BlockByPopup = new HashSet<PopupUI>();

	private bool m_BlockByHive = false;
	private bool m_BlockByScene = false;
	private bool m_BlockByPanel = false;

	private void Awake()
	{
		m_Inst = this;

#if INPUT_BLOCKER_TEST
		m_InputBlockObject.GetComponent<STImage>().color = new Color(1, 0, 0, 0.3f);
#endif
	}

#region Custom
	// 해당 함수만 카운트를 체크하고 나머지는 체크하지 않음에 유의하길 바란다.
	public void SetBlock(object obj)
	{
		if (m_BlockCustom.ContainsKey(obj))
			m_BlockCustom[obj]++;
		else
			m_BlockCustom.Add(obj, 1);
		UpdateInputBlockObject();
	}

	public void ResetBlock(object obj)
	{
		if (m_BlockCustom.ContainsKey(obj))
		{
			if (--m_BlockCustom[obj] <= 0)
			{
				m_BlockCustom.Remove(obj);
				UpdateInputBlockObject();
			}
		}
	}
#endregion

#region STRequest
	public void SetBlockByRequest(STRequest request)
	{
		SetBlock(m_BlockRequests, request);
	}

	public void ResetBlockByRequest(STRequest request)
	{
		ResetBlock(m_BlockRequests, request);
	}
#endregion

#region Animation
	public void SetBlockByEffectItem(STEffectItem item)
	{
		SetBlock(m_BlockEffectItems, item);
	}

	public void ResetBlockByEffectItem(STEffectItem item)
	{
		ResetBlock(m_BlockEffectItems, item);
	}

	public void SetBlockByDOTween(Tween tween)
	{
		SetBlock(m_BlockDOTweens, tween);
	}

	public void ResetBlockByDOTween(Tween tween)
	{
		ResetBlock(m_BlockDOTweens, tween);
	}

	public void SetBlockByUITweenEffect(UITweenEffect tweenEffect)
	{
		SetBlock(m_BlockUITweenEffects, tweenEffect);
	}

	public void ResetBlockByUITweenEffect(UITweenEffect tweenEffect)
	{
		ResetBlock(m_BlockUITweenEffects, tweenEffect);
	}
#endregion

#region Popup
	public void SetBlockByPopup(PopupUI popup)
	{
		SetBlock(m_BlockByPopup, popup);
	}

	public void ResetBlockByPopup(PopupUI popup)
	{
		ResetBlock(m_BlockByPopup, popup);
	}
#endregion

#region Hive
	public void SetBlockByHive()
	{
		m_BlockByHive = true;
		UpdateInputBlockObject();
	}

	public void ResetBlockByHive()
	{
		m_BlockByHive = false;
		UpdateInputBlockObject();
	}
#endregion

#region Scene
	public void SetBlockByScene()
	{
		m_BlockByScene = true;
		UpdateInputBlockObject();
	}

	public void ResetBlockByScene()
	{
		m_BlockByScene = false;
		UpdateInputBlockObject();
	}
#endregion

#region Panel
	public void SetBlockByPanel()
	{
		m_BlockByPanel = true;
		UpdateInputBlockObject();
	}

	public void ResetBlockByPanel()
	{
		m_BlockByPanel = false;
		UpdateInputBlockObject();
	}
#endregion

	private void SetBlock<T>(HashSet<T> hashSet, T item)
	{
		if (hashSet.Add(item))
	  		UpdateInputBlockObject();
	}

	private void ResetBlock<T>(HashSet<T> hashSet, T item)
	{
		if (hashSet.Remove(item))
			UpdateInputBlockObject();
	}

	private void UpdateInputBlockObject()
	{
		bool isBlock = false;

		isBlock = isBlock || (m_BlockCustom.Count > 0);
		isBlock = isBlock || (m_BlockRequests.Count > 0);
		isBlock = isBlock || (m_BlockEffectItems.Count > 0);
		isBlock = isBlock || (m_BlockDOTweens.Count > 0);
		isBlock = isBlock || (m_BlockUITweenEffects.Count > 0);
		isBlock = isBlock || (m_BlockByPopup.Count > 0);
		isBlock = isBlock || m_BlockByHive;
		isBlock = isBlock || m_BlockByScene;
		isBlock = isBlock || m_BlockByPanel;

		SetActiveInputBlockObject(isBlock);
	}

	private void SetActiveInputBlockObject(bool isActive)
	{
		m_InputBlockObject.SetActive(isActive);
	}
}
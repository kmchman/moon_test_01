using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalPopupManager : PopupManager
{
	public static GlobalPopupManager Inst { get { return m_Inst; } }
	private static GlobalPopupManager m_Inst = null;

	protected override void Awake()
	{
		base.Awake();

		m_Inst = this;
	}
}

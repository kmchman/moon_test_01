using UnityEngine;
using System.Collections;

public class MoonGlobalPopupManager : MoonPopupManager {

	public static MoonGlobalPopupManager Inst { get { return m_Inst; } }
	private static MoonGlobalPopupManager m_Inst = null;

	protected override void Awake()
	{
		base.Awake();

		m_Inst = this;
	}
}

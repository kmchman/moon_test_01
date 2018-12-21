using UnityEngine;

using System.Collections;

public class HudController : MonoBehaviour
{
	[SerializeField] private HudType[] m_ActiveHudTypes;
	[SerializeField] private UITextEnum m_TitleTextEnum = UITextEnum.NONE;
	[SerializeField] private BUILDING m_IDBuilding = BUILDING.NONE;
	[SerializeField] private GUILD_BUILDING m_IDGuildBuilding = GUILD_BUILDING.NONE;

	public void UpdateHud()
	{
		if (HudManager.inst == null)
			return;

//		HudManager.inst.SetHud(m_ActiveHudTypes);
//		HudManager.inst.SetTitle(m_TitleTextEnum);
//		if (m_IDBuilding != BUILDING.NONE)
//			HudManager.inst.SetTitle(Datatable.Inst.GetBuildingName((int)m_IDBuilding));
//		if (m_IDGuildBuilding != GUILD_BUILDING.NONE)
//		{
//			if (AccountDataStore.instance.GetGuildBuildingLevel((int)m_IDGuildBuilding) == 0)
//				HudManager.inst.SetTitle(Datatable.Inst.GetGuildBuildingName((int)m_IDGuildBuilding));
//		}
	}
}

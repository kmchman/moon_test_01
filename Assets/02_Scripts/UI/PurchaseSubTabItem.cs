using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseSubTabItem : TabItem {

	[SerializeField] private STImage			m_ImageNoti;
	[SerializeField] private SpriteSet			m_IconNotiSpriteSet;

	private int m_IdPurchaseSubCategory;
	public int IdPurchaseSubCategory{ get {return m_IdPurchaseSubCategory;}}

	public void SetDataEx(int idSubCategory)
	{
//		m_IdPurchaseSubCategory = idSubCategory;
//		Datatable.PurchaseSubCategory item = Datatable.Inst.dtPurchaseSubCategory[idSubCategory];
//		m_TitleText.text = item.Name;
//		UpdateNoti();
	}

	public void UpdateNoti()
	{
//		NotiType notiType = Datatable.Inst.GetPurchaseSubCategoryNoti(m_IdPurchaseSubCategory);
//
//		switch (notiType)
//		{
//		case NotiType.Red:
//			m_ImageNoti.gameObject.SetActive(true);
//			m_ImageNoti.sprite = m_IconNotiSpriteSet.GetSprite(SpriteType.IconNoti_Red);
//			break;
//		case NotiType.Purple:
//			m_ImageNoti.gameObject.SetActive(true);
//			m_ImageNoti.sprite = m_IconNotiSpriteSet.GetSprite(SpriteType.IconNoti_Purple);
//			break;
//		default:
//			m_ImageNoti.gameObject.SetActive(false);
//			break;
//		}
	}
}

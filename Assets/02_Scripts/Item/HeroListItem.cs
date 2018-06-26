using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class HeroListItem : STScrollRectItem {
	
	[SerializeField] private SpriteAtlas 		m_AtlasHeroFace;
	[SerializeField] private Image 				m_ImageFace;
	public void SetData(int idItem)
	{
		Datatable.HeroData heroData = Datatable.Inst.dtHeroData[idItem];
		m_ImageFace.sprite = m_AtlasHeroFace.GetSprite(heroData.PvpFaceImage);
	}
}

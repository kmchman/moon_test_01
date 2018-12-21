using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SpriteSet : MonoBehaviour 
{
	[Serializable]
	public class SpriteInfo
	{
		public SpriteType spriteType = SpriteType.None;
		public Sprite sprite = null;
	}

	public List<SpriteSet> spriteSetList = new List<SpriteSet>();
	public List<SpriteInfo> spriteInfoList = new List<SpriteInfo>();

	private Dictionary<SpriteType, Sprite> m_spriteData = new Dictionary<SpriteType, Sprite>();
	private List<Sprite> m_spriteList = new List<Sprite>();

	public Sprite GetSprite(SpriteType type)
	{
		MakeSpriteData();

		Sprite sprite;
		if (m_spriteData.TryGetValue(type, out sprite))
			return sprite;

		Debug.LogError(Util.LogFormat("Error Not Found SpriteSet", name, type));
		return null;
	}

	public Sprite GetSprite(string spriteName)
	{
		MakeSpriteData();

		for (int i = 0; i < m_spriteList.Count; ++i)
		{
			if (m_spriteList[i].name == spriteName)
				return m_spriteList[i];
		}
		
		var enumerator = m_spriteData.GetEnumerator();
		while (enumerator.MoveNext())
			if (enumerator.Current.Value.name == spriteName)
				return enumerator.Current.Value;

		Debug.LogError(Util.LogFormat("Error Not Found SpriteSet", name, spriteName));
		return null;
	}

	private void MakeSpriteData()
	{
		if (m_spriteData.Count > 0)
			return;

		for (int i = 0; i < spriteSetList.Count; ++i)
		{
			var enumerator = spriteSetList[i].GetAllSpriteData().GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Key == SpriteType.None)
					m_spriteList.Add(enumerator.Current.Value);
				else
					m_spriteData[enumerator.Current.Key] = enumerator.Current.Value;
			}
		}

		for (int i = 0; i < spriteInfoList.Count; ++i)
		{
			if (spriteInfoList[i].spriteType == SpriteType.None)
				m_spriteList.Add(spriteInfoList[i].sprite);
			else
				m_spriteData[spriteInfoList[i].spriteType] = spriteInfoList[i].sprite;
		}
	}

	private Dictionary<SpriteType, Sprite> GetAllSpriteData()
	{
		MakeSpriteData();

		return m_spriteData;
	}
}
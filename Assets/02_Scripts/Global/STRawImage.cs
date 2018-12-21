using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class STRawImage : RawImage
{
	[SerializeField] private GameObject m_LoadingPrefab;
	[SerializeField] private GameObject m_ErrorPrefab;

	private GameObject m_LoadingObject = null;
	private GameObject m_ErrorObject = null;

	private string m_LastFileName;

	public void AsyncLoadTexture(string fileName, Action<Texture> cb = null)
	{
		SetActiveLoadingObject(true);
		SetActiveErrorObject(false);

		canvasRenderer.SetAlpha(0f);

		texture = null;

		m_LastFileName = fileName;

//		ResourceManager.Inst.AsyncLoadTextureInFile(fileName, (tex) => { AsyncLoadedTexture(fileName, tex, cb); });
	}

	private void AsyncLoadedTexture(string fileName, Texture tex, Action<Texture> cb)
	{
		if (!m_LastFileName.Equals(fileName))
		{
			if (cb != null)
				cb(null);
			return;
		}
		
		SetActiveLoadingObject(false);

		texture = tex;

		if (tex == null)
		{
			Debug.LogError(Util.LogFormat("STRawImage.AsyncLoadedTexture", fileName));
			
			SetActiveErrorObject(true);

			if (m_ErrorObject == null)
				canvasRenderer.SetAlpha(1f);
		}
		else
		{
			canvasRenderer.SetAlpha(1f);
		}
		
		if (cb != null)
			cb(tex);
	}

	private void SetActiveLoadingObject(bool value)
	{
		SetActiveObject(value, ref m_LoadingObject, m_LoadingPrefab);
	}

	private void SetActiveErrorObject(bool value)
	{
		SetActiveObject(value, ref m_ErrorObject, m_ErrorPrefab);
	}

	private void SetActiveObject(bool value, ref GameObject obj, GameObject prefab)
	{
//		if (value && obj == null && prefab != null)
//			obj = Giant.Util.MakeItem(prefab, transform, false);
//
//		if (obj != null)
//			obj.SetActive(value);
	}
}

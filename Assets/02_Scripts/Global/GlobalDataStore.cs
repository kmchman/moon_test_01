using UnityEngine;
using System.Collections;

public class GlobalDataStore : MonoBehaviour
{
	private static GlobalDataStore _inst = null;
	public static GlobalDataStore Inst { get { return _inst; } }

	[SerializeField] private Material uiMaskGrayscaleMat;

	[SerializeField] private Color [] gameColors;

	private string [] gameColorStrings;

	void Awake()
	{
		useGUILayout = false;
		_inst = this;

		// 초기화
		uiMaskGrayscaleMat.SetFloat("_EffectAmount", 1f);

		gameColorStrings = new string[gameColors.Length];
		for (int i = 0; i < gameColors.Length; ++i)
		{
			gameColorStrings[i] = ColorUtility.ToHtmlStringRGBA(gameColors[i]);
		}
	}

	public Material GetUIMaskGrayScaleMaterial()
	{
		return uiMaskGrayscaleMat;
	}

	public Material GetCloneUIMaskGrayScaleMaterial()
	{
		return Instantiate<Material>(uiMaskGrayscaleMat);
	}

	public Color GetGameColor(ColorType type)
	{
		int index = (int)type;
		if (index < 0 || index >= gameColors.Length)
			return new Color();
		return gameColors[index];
	}

	public string GetGameColorString(ColorType type)
	{
		int index = (int)type;
		if (index < 0 || index >= gameColorStrings.Length)
			return string.Empty;
		return gameColorStrings[index];
	}
}

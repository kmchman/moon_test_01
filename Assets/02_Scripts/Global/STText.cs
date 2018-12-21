using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using STDOTweenExtensions;
using STExtensions;

public class STText : Text
{
	[UnityEngine.Serialization.FormerlySerializedAs("m_textStyler")]
	[SerializeField] private STTextStyler m_TextStyler;

	[SerializeField] private bool m_IsGrayScale = false;
	[UnityEngine.Serialization.FormerlySerializedAs("m_colorType")]
	[SerializeField] private ColorType m_ColorType = ColorType.None;

	[UnityEngine.Serialization.FormerlySerializedAs("m_shadow")]
	[SerializeField] private STMeshEffectData m_Shadow = new STMeshEffectData();
	[UnityEngine.Serialization.FormerlySerializedAs("m_outline")]
	[SerializeField] private STMeshEffectData m_Outline = new STMeshEffectData();

	[SerializeField] private bool m_IsUnchangeable;		// 입력된 문자열을 바꾸지 않는 용도

	public STTextStyler textStyler { get { return m_TextStyler; } }

	public bool isUnchangeable { get { return m_IsUnchangeable; } }

	private string m_FormatString = "{0}";
	private Tweener m_NumberChangerTween; 
	private double m_TargetValue = 0f;
	private double m_CurrentValue = -1f;
	private bool m_IsInt = true;
	private System.Action m_ChangeNumberAnimationEndCB = null;
	private System.Action m_ChangeNumberAnimationUpdateCB = null;
	private System.Action<float> m_ChangeNumberAnimationUpdateRateCB = null;

	private readonly UIVertex[] m_TempVerts = new UIVertex[4];

	public ColorType colorType
	{
		get { return m_ColorType; }
		set { SetColorType(value); }
	}

	public bool isUseShadow
	{
		get { return m_Shadow.isUse; }
		set { SetIsUseData(m_Shadow, value); }
	}

	public Color shadowColor
	{
		get { return m_Shadow.color; }
		set { SetColorData(m_Shadow, value); }
	}

	public Vector2 shadowDistance
	{
		get { return m_Shadow.distance; }
		set { SetDistanceData(m_Shadow, value); }
	}

	public bool isUseShadowGraphicAlpha
	{
		get { return m_Shadow.isUseGraphicAlpha; }
		set { SetIsUseGraphicAlphaData(m_Shadow, value); }
	}

	public bool isUseOutline
	{
		get { return m_Outline.isUse; }
		set { SetIsUseData(m_Outline, value); }
	}

	public Color outlineColor
	{
		get { return m_Outline.color; }
		set { SetColorData(m_Outline, value); }
	}

	public Vector2 outlineDistance
	{
		get { return m_Outline.distance; }
		set { SetDistanceData(m_Outline, value); }
	}

	public bool isUseOutlineGraphicAlpha
	{
		get { return m_Outline.isUseGraphicAlpha; }
		set { SetIsUseGraphicAlphaData(m_Outline, value); }
	}

	public void SetGrayScale(bool isGrayScale)
	{
		if (m_IsGrayScale == isGrayScale)
			return;

		m_IsGrayScale = isGrayScale;
		SetVerticesDirty();
	}
		
	public void ChangeNumberInit(double value)
	{
		ChangeNumberInit(value, m_FormatString);
	}

	public void ChangeNumberInit(double value, string formatString)
	{
		if(!string.IsNullOrEmpty(formatString))
			m_FormatString = formatString;
		m_CurrentValue = m_TargetValue = value;
		SetImmediateText(m_TargetValue);
	}
		
	public void ChangeNumberAnimation(double targetNumber, float animationTime = 0.4f, System.Action endCB = null)
	{
		ChangeNumberAnimation(m_CurrentValue, targetNumber, animationTime, m_FormatString, endCB);
	}

	public void ChangeNumberAnimation(double targetNumber, string formatString, float animationTime = 0.4f, System.Action endCB = null)
	{
		ChangeNumberAnimation(m_CurrentValue, targetNumber, animationTime, formatString, endCB);
	}

	public void ChangeNumberAnimation(double startNumber, double targetNumber, float animationTime, string formatString, System.Action endCB = null)
	{
		m_ChangeNumberAnimationEndCB = endCB;
		m_CurrentValue = startNumber;
		if(!string.IsNullOrEmpty(formatString))
			m_FormatString = formatString;
		SetImmediateText(m_CurrentValue);
		RunAnimation(targetNumber, animationTime);
	}

	public void StopAnimation()
	{
		if(m_NumberChangerTween != null)
			m_NumberChangerTween.Kill(true);

		SetImmediateText(m_TargetValue);
	}

	public void SetChangeNumberAnimationUpdateCB(System.Action cb)
	{
		m_ChangeNumberAnimationUpdateCB = cb;
	}

	public void SetChangeNumberAnimationUpdateRateCB(System.Action<float> cb)
	{
		m_ChangeNumberAnimationUpdateRateCB = cb;
	}

	public void SetUseFloat(bool isUse)
	{
		m_IsInt = !isUse;
	}
		
	protected override void Awake()
	{
		base.Awake();

//#if UNITY_EDITOR
//		STGraphicManager.MakeSTGraphicManager();
//#endif

		if (Application.isPlaying)
			StyleInit();

		AddText();
	}

	private void AddText()
	{
//		if (STGraphicManager.inst != null)
//			STGraphicManager.inst.AddText(this);
	}

	private void RemoveText()
	{
//		if (STGraphicManager.inst != null)
//			STGraphicManager.inst.RemoveText(this);
	}
		
	protected override void OnDisable()
	{
		base.OnDisable();

		if(m_NumberChangerTween != null)
			m_NumberChangerTween.Kill(true);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		RemoveText();
	}

	private void MoveCharVertex(UIVertex[] vertexs, float fontRate, int lastCharIndex, int preLastCharIndex)
	{
		int vertexsLength = vertexs.Length;

		char lastChar = text [lastCharIndex];
		char preLastChar = text [preLastCharIndex];

		while (preLastChar == '\n') {
			--preLastCharIndex;
			preLastChar = text [preLastCharIndex];
		}

		font.RequestCharactersInTexture(Util.StringFormat("{0}{1}", lastChar, preLastChar));

		CharacterInfo lastCharacterInfo;
		font.GetCharacterInfo(lastChar, out lastCharacterInfo);

		CharacterInfo preLastCharacterInfo2;
		font.GetCharacterInfo(preLastChar, out preLastCharacterInfo2);

		Vector3 vertex3Position = vertexs[lastCharIndex * 4 + 1].position;
		Vector3 vertex1Position = vertexs[lastCharIndex * 4 + 3].position;

		float tempWidth = Mathf.Abs(vertex3Position.x - vertex1Position.x);
		float tempHeight = Mathf.Abs(vertex3Position.y - vertex1Position.y);


		Vector3 startPosition = vertexs[preLastCharIndex * 4 + 3].position - new Vector3(preLastCharacterInfo2.minX, preLastCharacterInfo2.minY, 0) * fontRate;
		startPosition += new Vector3(preLastCharacterInfo2.advance * fontRate, 0, 0);

		float minX = lastCharacterInfo.minX * fontRate;
		float minY = lastCharacterInfo.minY * fontRate;

		vertexs[lastCharIndex * 4 + 3].position = startPosition + new Vector3(minX, 				minY);
		vertexs[lastCharIndex * 4 + 2].position = startPosition + new Vector3(minX + tempWidth, 	minY);
		vertexs[lastCharIndex * 4 + 1].position = startPosition + new Vector3(minX + tempWidth, 	minY + tempHeight);
		vertexs[lastCharIndex * 4	 ].position = startPosition + new Vector3(minX, 				minY + tempHeight);
	}

	protected override void OnPopulateMesh(VertexHelper toFill)
	{
		base.OnPopulateMesh(toFill);
//
//		UIVertex[] vertexs = new UIVertex[toFill.currentVertCount];
//		
//		for (int i = 0; i < vertexs.Length; ++i)
//			toFill.PopulateUIVertex(ref vertexs[i], i);
//
//		// start 
//		string notCharString = Datatable.Inst.settingData.NoLastOneChar;
//		if (DeviceSaveManager.Inst.IsCurrentLanguageEqual(SystemLanguage.Japanese) && !string.IsNullOrEmpty(notCharString))
//		{
//			int currentFontSize = 0;
//			if (resizeTextForBestFit)
//			{
//				if (resizeTextMinSize > cachedTextGenerator.fontSizeUsedForBestFit)
//					currentFontSize = resizeTextMinSize;
//				else if (resizeTextMaxSize < cachedTextGenerator.fontSizeUsedForBestFit)
//					currentFontSize = resizeTextMaxSize;
//				else
//					currentFontSize = cachedTextGenerator.fontSizeUsedForBestFit;
//			}
//			else
//			{
//				currentFontSize = fontSize;
//			}
//
//			float fontRate = currentFontSize / (float)font.fontSize;
//
//			UILineInfo[] lineInfos = cachedTextGenerator.GetLinesArray();
//			for (int i = 1; i < lineInfos.Length; ++i)
//			{
//				int firstIndex = lineInfos[i].startCharIdx;
//				if (text.Length <= firstIndex)
//					break;
//				char firstChar = text[firstIndex];
//
//				while(notCharString.IndexOf(firstChar) != -1)
//				{
//					MoveCharVertex (vertexs, fontRate, firstIndex, firstIndex - 1);
//					int lineLastIndex = i < lineInfos.Length - 1 ? lineInfos[i + 1].startCharIdx - 1 : text.Length;
//
//					CharacterInfo lastCharacterInfo;
//					font.GetCharacterInfo(firstChar, out lastCharacterInfo);
//					float deltaXValue = lastCharacterInfo.advance * fontRate;
//					for (int j = firstIndex + 1; j <= lineLastIndex; ++j) {
//						if (j * 4 >= vertexs.Length)
//							break;
//						vertexs [j*4].position.x -= deltaXValue;
//						vertexs [j*4+1].position.x -= deltaXValue;
//						vertexs [j*4+2].position.x -= deltaXValue;
//						vertexs [j*4+3].position.x -= deltaXValue;
//					}
////					blank line roll up not used
////					float deltaYValue = Mathf.Abs(lineInfos[i].leading) * fontRate * 2 + lineInfos[i].height * fontRate;
////					bool isEnableRollUp = lineLastIndex * 4 < vertexs.Length && vertexs[lineLastIndex * 4 - 1].position.y > vertexs[lineLastIndex * 4].position.y + deltaYValue;
////					if (firstIndex + 1 < text.Length && text[firstIndex + 1] == '\n' && isEnableRollUp) 
////					{
////						for (int j = lineLastIndex * 4; j < vertexs.Length; ++j) 
////						{
////							vertexs[j].position.y += deltaYValue;
////						}
////					}
//					if (++firstIndex >= lineLastIndex)
//						break;
//					firstChar = text[firstIndex];
//				}
//			}
//		}
//		//end
//		toFill.Clear();
//
//		if (m_IsGrayScale)
//		{
//			Color color = (GlobalDataStore.Inst != null) ? GlobalDataStore.Inst.GetGameColor(ColorType.Disable) : Color.gray;
//			for (int i = 0; i < vertexs.Length; ++i)
//				vertexs[i].color = color;
//		}
//
//		if (m_Shadow.isUse)
//		{
//			AddUIVertex(toFill, ref vertexs, m_Shadow.distance.x, m_Shadow.distance.y, m_Shadow.isUseGraphicAlpha, m_Shadow.color);
//			AddUIVertex(toFill, ref vertexs, m_Shadow.distance.x + m_Outline.distance.x, m_Shadow.distance.y, m_Shadow.isUseGraphicAlpha, m_Shadow.color);
//			AddUIVertex(toFill, ref vertexs, m_Shadow.distance.x - m_Outline.distance.x, m_Shadow.distance.y, m_Shadow.isUseGraphicAlpha, m_Shadow.color);
//		}
//
//		if (m_Outline.isUse)
//		{
//			AddUIVertex(toFill, ref vertexs, m_Outline.distance.x, m_Outline.distance.y, m_Outline.isUseGraphicAlpha, m_Outline.color);
//			AddUIVertex(toFill, ref vertexs, m_Outline.distance.x, -m_Outline.distance.y, m_Outline.isUseGraphicAlpha, m_Outline.color);
//			AddUIVertex(toFill, ref vertexs, -m_Outline.distance.x, m_Outline.distance.y, m_Outline.isUseGraphicAlpha, m_Outline.color);
//			AddUIVertex(toFill, ref vertexs, -m_Outline.distance.x, -m_Outline.distance.y, m_Outline.isUseGraphicAlpha, m_Outline.color);
//			AddUIVertex(toFill, ref vertexs, m_Outline.distance.x, 0, m_Outline.isUseGraphicAlpha, m_Outline.color);
//			AddUIVertex(toFill, ref vertexs, -m_Outline.distance.x, 0, m_Outline.isUseGraphicAlpha, m_Outline.color);
//			AddUIVertex(toFill, ref vertexs, 0, m_Outline.distance.y, m_Outline.isUseGraphicAlpha, m_Outline.color);
//			AddUIVertex(toFill, ref vertexs, 0, -m_Outline.distance.y, m_Outline.isUseGraphicAlpha, m_Outline.color);
//		}
//
//		for (int i = 0; i < vertexs.Length; ++i)
//		{
//			int tempVertsIndex = i & 3;
//			m_TempVerts[tempVertsIndex] = vertexs[i];
//
//			if (tempVertsIndex == 3)
//				toFill.AddUIVertexQuad(m_TempVerts);
//		}
	}

	public void StyleInit()
	{
		ApplyTextStyler();
		ApplyColorType();
	}

	private void ApplyTextStyler()
	{
		if (m_TextStyler == null)
			return;

		font = m_TextStyler.font;
		fontSize = m_TextStyler.fontSize;
		lineSpacing = m_TextStyler.lineSpacing;

		m_Shadow = m_TextStyler.shadow;
		m_Outline = m_TextStyler.outline;
	}

	private void ApplyColorType()
	{
		ApplyColor(m_ColorType);
	}

	private void ApplyColor(ColorType colorType)
	{
		if (GlobalDataStore.Inst == null)
			return;

		if (colorType == ColorType.None)
			return;
		
		color = GlobalDataStore.Inst.GetGameColor(colorType);
	}

	private void SetColorType(ColorType colorType)
	{
		if (m_ColorType == colorType)
			return;

		m_ColorType = colorType;
		ApplyColorType();
	}

	private void SetIsUseData(STMeshEffectData data, bool isUse)
	{
		if (data.isUse == isUse)
			return;

		data.isUse = isUse;
		SetVerticesDirty();
	}

	private void SetColorData(STMeshEffectData data, Color color)
	{
		if (data.color == color)
			return;

		data.color = color;
		SetVerticesDirty();
	}

	private void SetDistanceData(STMeshEffectData data, Vector2 distance)
	{
		distance = CheckDistance(distance);
		if (data.distance == distance)
			return;

		data.distance = distance;
		SetVerticesDirty();
	}

	private void SetIsUseGraphicAlphaData(STMeshEffectData data, bool isUseGraphicAlpha)
	{
		if (data.isUseGraphicAlpha == isUseGraphicAlpha)
			return;

		data.isUseGraphicAlpha = isUseGraphicAlpha;
		SetVerticesDirty();
	}

	private Vector2 CheckDistance(Vector2 distance)
	{
		return new Vector2(Mathf.Clamp(distance.x, -600.0f, 600.0f), Mathf.Clamp(distance.y, -600.0f, 600.0f));
	}

	private void AddUIVertex(VertexHelper toFill, ref UIVertex[] vertexs, float x, float y, bool isUseGraphicAlpha, Color color)
	{
		Vector3 distance = new Vector3(x, y, 0);

		for (int i = 0; i < vertexs.Length; ++i)
		{
			int tempVertsIndex = i & 3;
			m_TempVerts[tempVertsIndex] = vertexs[i];
			m_TempVerts[tempVertsIndex].position += distance;

			if (isUseGraphicAlpha)
			{
				Color32 tempColor = color;
				tempColor.a = (byte)(tempColor.a * vertexs[i].color.a / 255);
				m_TempVerts[tempVertsIndex].color = tempColor;
			}
			else
			{
				m_TempVerts[tempVertsIndex].color = color;
			}

			if (tempVertsIndex == 3)
				toFill.AddUIVertexQuad(m_TempVerts);
		}
	}

	private void RunAnimation(double targetNumber, float animationTime)
	{
		if(m_NumberChangerTween != null)
			m_NumberChangerTween.Kill(true);
		
		if(m_CurrentValue == -1)
		{
			SetImmediateText(targetNumber);
		}
		else
		{
			m_TargetValue = targetNumber;
			double deltaValue = m_TargetValue - m_CurrentValue;
			m_NumberChangerTween = DOVirtual.Float(0f, 1f, animationTime, (float value)=>{
				text = m_IsInt ? Util.StringFormat(m_FormatString, (long)(m_CurrentValue + (deltaValue * value))) : Util.StringFormat(m_FormatString, (m_CurrentValue + (deltaValue * value)));
				if(m_ChangeNumberAnimationUpdateCB != null)
					m_ChangeNumberAnimationUpdateCB();
				if(m_ChangeNumberAnimationUpdateRateCB != null)
					m_ChangeNumberAnimationUpdateRateCB(value);
			}).OnCompleteAppend(()=>{
				m_CurrentValue = m_TargetValue;
				if(m_ChangeNumberAnimationEndCB != null)
					m_ChangeNumberAnimationEndCB();
			});
		}
	}

	private void SetImmediateText(double targetNumber)
	{
		m_CurrentValue = targetNumber;
		text = m_IsInt ? Util.StringFormat(m_FormatString, (int)targetNumber) : Util.StringFormat(m_FormatString, targetNumber);
	}
}
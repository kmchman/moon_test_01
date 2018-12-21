using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

using STExtensions;
using STDOTweenExtensions;

public enum UITweenEffectType
{
	Transfer = 0,
	Scale,
	PunchScale,
	PunchRotation,
	Rotation,
	Nothing,
}

public enum UIImageEffectType
{
	None = 0,
	GrayScale,
	Dissolve,
	Blur,
	ReplaceSprite,
	SpriteAnimation,
	ChangeColor,
}

public enum UIEaseTweenType
{
	easeInQuad = 0,
	easeOutQuad,
	easeInOutQuad,
	easeInCubic,
	easeOutCubic,
	easeInOutCubic,
	easeInQuart,
	easeOutQuart,
	easeInOutQuart,
	easeInQuint,
	easeOutQuint,
	easeInOutQuint,
	easeInSine,
	easeOutSine,
	easeInOutSine,
	easeInExpo,
	easeOutExpo,
	easeInOutExpo,
	easeInCirc,
	easeOutCirc,
	easeInOutCirc,
	linear,
	spring,
	easeInBounce,
	easeOutBounce,
	easeInOutBounce,
	easeInBack,
	easeOutBack,
	easeInOutBack,
	easeInElastic,
	easeOutElastic,
	easeInOutElastic,
}

[System.Serializable]
public class SingtaTweenEffectData
{
	public UITweenEffectType			m_TweenType;
	public UIImageEffectType			m_ImageEffectType;

	public bool							IsLocal;
	public bool							Once;
	public bool							Loop;
	public bool							FadeInOut;
	public bool							UseAnimationCurve;
	public bool							IsDisableInCompleted;
	
	public float						Duration;
	public float						Delay;
	public float						Sleep;
	public AnimationCurve				FadeCurve;
	public AnimationCurve				ScaleXCurve;
	public float						PunchScalePower;
	public float						SpriteAnimationTime;
	public Color						BeforeColor;
	public Color						AfterColor;
	
	public UIEaseTweenType				EaseType;
	
	public Vector3						StartPos;
	public Vector3						EndPos;
	public Vector3						StartScale;
	public Vector3						EndScale;
	public Vector3						Angle;
	public Vector3						PunchRotationAxis;

	public Image						ImageItem;
	public Sprite						ReplaceSprite;
	public Sprite[]						SpriteAnimation;
	public GameObject[]					ActivationObjects;

	public Ease Ease
	{
		get
		{
			switch (EaseType)
			{
			case UIEaseTweenType.easeInQuad:		return Ease.InQuad;
			case UIEaseTweenType.easeOutQuad:		return Ease.OutQuad;
			case UIEaseTweenType.easeInOutQuad:		return Ease.InOutQuad;
			case UIEaseTweenType.easeInCubic:		return Ease.InCubic;
			case UIEaseTweenType.easeOutCubic:		return Ease.OutCubic;
			case UIEaseTweenType.easeInOutCubic:	return Ease.InOutCubic;
			case UIEaseTweenType.easeInQuart:		return Ease.InQuart;
			case UIEaseTweenType.easeOutQuart:		return Ease.OutQuart;
			case UIEaseTweenType.easeInOutQuart:	return Ease.InOutQuart;
			case UIEaseTweenType.easeInQuint:		return Ease.InQuint;
			case UIEaseTweenType.easeOutQuint:		return Ease.OutQuint;
			case UIEaseTweenType.easeInOutQuint:	return Ease.InOutQuint;
			case UIEaseTweenType.easeInSine:		return Ease.InSine;
			case UIEaseTweenType.easeOutSine:		return Ease.OutSine;
			case UIEaseTweenType.easeInOutSine:		return Ease.InOutSine;
			case UIEaseTweenType.easeInExpo:		return Ease.InExpo;
			case UIEaseTweenType.easeOutExpo:		return Ease.OutExpo;
			case UIEaseTweenType.easeInOutExpo:		return Ease.InOutExpo;
			case UIEaseTweenType.easeInCirc:		return Ease.InCirc;
			case UIEaseTweenType.easeOutCirc:		return Ease.OutCirc;
			case UIEaseTweenType.easeInOutCirc:		return Ease.InOutCirc;
			case UIEaseTweenType.linear:			return Ease.Linear;
			case UIEaseTweenType.easeInBounce:		return Ease.InBounce;
			case UIEaseTweenType.easeOutBounce:		return Ease.OutBounce;
			case UIEaseTweenType.easeInOutBounce:	return Ease.InOutBounce;
			case UIEaseTweenType.easeInBack:		return Ease.InBack;
			case UIEaseTweenType.easeOutBack:		return Ease.OutBack;
			case UIEaseTweenType.easeInOutBack:		return Ease.InOutBack;
			case UIEaseTweenType.easeInElastic:		return Ease.InElastic;
			case UIEaseTweenType.easeOutElastic:	return Ease.OutElastic;
			case UIEaseTweenType.easeInOutElastic:	return Ease.InOutElastic;
			}

			return Ease.Linear;
		}
	}
}

public class UITweenEffect : MonoBehaviour 
{
	public bool							m_IsBlockInputEvent				= false;
	[UnityEngine.Serialization.FormerlySerializedAs("m_IsUseEventAction")]
	public bool							m_IsPauseEventAction			= false;
	public bool							PlayAwake						= true;
	public bool							UseImageComponent				= false;
	public SingtaTweenEffectData[]		m_TweenEffectItems;

	private bool						m_bIsInited;
	private bool						m_bIsReCalcLocalPos;
	private bool						m_bIsFinished;

	private float						m_fCustomAlpha;

	private Image						m_Img_Icon;
	private Sprite						m_Sprite_Orignal;

	private RectTransform				m_RectTransform;
	private Transform					m_Tr_MyTransform;
	private CanvasGroup					m_CanvasGroup;
	private List<SingtaTweenEffectData>	Lt_SingtaTweenEffectList;

	private System.Action				m_EndCB;

	void Awake()
	{
		m_bIsReCalcLocalPos 		= false;
		m_bIsFinished				= false;
		Lt_SingtaTweenEffectList 	= new List<SingtaTweenEffectData>();

		m_Tr_MyTransform			= transform;
		m_RectTransform				= gameObject.GetComponent<RectTransform>();

		if( !UseImageComponent )
		{
			if( gameObject.GetComponent<CanvasGroup>() == null )
			{
				m_CanvasGroup 	= gameObject.AddComponent<CanvasGroup>();
			}
			else
			{
				m_CanvasGroup	= gameObject.GetComponent<CanvasGroup>();
			}
		}
		else
		{
			if( m_Img_Icon == null )
			{
				m_Img_Icon 		= gameObject.GetComponent<Image>();
			}
		}

		if( !UseImageComponent )
		{
			if( m_TweenEffectItems.Length > 0 && enabled && m_TweenEffectItems[0].FadeInOut )
			{
				m_CanvasGroup.alpha = m_fCustomAlpha+m_TweenEffectItems[0].FadeCurve.Evaluate(0.0f);
				m_fCustomAlpha		= 0f;
			}
		}
		else
		{
			if( m_TweenEffectItems.Length > 0 && enabled && m_TweenEffectItems[0].FadeInOut )
			{
				m_Img_Icon.color = new Color(m_Img_Icon.color.r, m_Img_Icon.color.g, m_Img_Icon.color.b, 0.0f);
			}
		}

		if( m_TweenEffectItems.Length > 0 )
		{
			SingtaTweenEffectData item;
			for( int i = 0; i < m_TweenEffectItems.Length; i++ )
			{
				item = m_TweenEffectItems[i];
				if( item.ImageItem != null )
				{
					if( item.m_ImageEffectType == UIImageEffectType.ReplaceSprite )
					{
						m_Sprite_Orignal = item.ImageItem.sprite;
					}
				}
			}
		}

		m_bIsInited = true;
	}

	void OnEnable()
	{
		if( m_TweenEffectItems == null ) return;
		if( !m_bIsInited ) Awake();

		if( !UseImageComponent )
		{
			if( m_CanvasGroup == null )
			{
				m_RectTransform	= gameObject.GetComponent<RectTransform>();
				m_CanvasGroup 	= gameObject.AddComponent<CanvasGroup>();
			}
		}
		else
		{
			if( m_Img_Icon == null )
			{
				m_RectTransform	= gameObject.GetComponent<RectTransform>();
				m_Img_Icon 		= gameObject.GetComponent<Image>();
			}
		}

		if( m_TweenEffectItems.Length > 0 )
		{
//			SingtaTweenEffectData item;
//			for( int i = 0; i < m_TweenEffectItems.Length; i++ )
//			{
//				item = m_TweenEffectItems[i];
//				if( item.ImageItem != null )
//				{
//					if( item.m_ImageEffectType == UIImageEffectType.ReplaceSprite )
//					{
//						item.ImageItem.sprite = m_Sprite_Orignal;
//						item.ImageItem.SetNativeSize();
//					}
//
//					item.ImageItem.SetGrayScale(false);
//				}
//
//				if( item.ActivationObjects != null )
//				{
//					for( int k = 0; k < item.ActivationObjects.Length; k++ )
//					{
//						item.ActivationObjects[k].SetActive(false);
//					}
//				}
//			}
//
//			if( !PlayAwake )
//			{
//				for( int i = 0; i < m_TweenEffectItems.Length; i++ )
//				{
//					item = m_TweenEffectItems[i];
//					if( item.m_TweenType == UITweenEffectType.Transfer )
//					{
//						m_Tr_MyTransform.localPosition = item.StartPos;
//						break;
//					}
//				}
//			}
		}

		if( !PlayAwake ) return;
		if( !m_bIsReCalcLocalPos ) CalcLocalPosition(ref m_TweenEffectItems);

		Lt_SingtaTweenEffectList.Clear();
		Lt_SingtaTweenEffectList.AddRange(m_TweenEffectItems);

		m_Tr_MyTransform.DOKill();
		
		StopCoroutine("IE_Update_");
		StartCoroutine("IE_Update_");

//		STEffectItemManager.inst.RegisterUITweenEffect(this);
	}

	void OnDisable()
	{
//		STEffectItemManager.inst.UnRegisterUITweenEffect(this);

		if( !m_bIsInited ) Awake();
		if( !PlayAwake && m_bIsFinished ) return;

		if( m_TweenEffectItems.Length > 0 )
		{
			SingtaTweenEffectData item = m_TweenEffectItems[0];
			
			if( item.FadeInOut )
			{
				if( item.Delay > 0 )
				{
					if( !UseImageComponent )
					{
						m_CanvasGroup.alpha = 0f;
					}
					else
					{
						m_Img_Icon.color = new Color(m_Img_Icon.color.r, m_Img_Icon.color.g, m_Img_Icon.color.b, 0.0f);
					}
				}
				else
				{
					if( !UseImageComponent )
					{
						m_CanvasGroup.alpha = item.FadeCurve.Evaluate(0.0f);
					}
					else
					{
						m_Img_Icon.color = new Color(m_Img_Icon.color.r, m_Img_Icon.color.g, m_Img_Icon.color.b, item.FadeCurve.Evaluate(0.0f));
					}
				}
			}
			else
			{
				if( !UseImageComponent )
				{
					m_CanvasGroup.alpha = 1.0f;
				}
				else
				{
					m_Img_Icon.color = new Color(m_Img_Icon.color.r, m_Img_Icon.color.g, m_Img_Icon.color.b, 1.0f);
				}
			}

			m_Tr_MyTransform.DOKill();
			switch(item.m_TweenType)
			{
			case UITweenEffectType.Transfer:
				
				m_Tr_MyTransform.localPosition = item.StartPos;
				
				break;
				
			case UITweenEffectType.Scale:
				
				m_Tr_MyTransform.localScale = item.StartScale;
				
				break;
				
			case UITweenEffectType.PunchScale:

				m_Tr_MyTransform.localScale = item.StartScale;

				break;

			case UITweenEffectType.PunchRotation:

				m_Tr_MyTransform.localRotation = Quaternion.identity;

				break;

			case UITweenEffectType.Rotation:

				m_Tr_MyTransform.localRotation = Quaternion.identity;

				break;

			case UITweenEffectType.Nothing:

				m_Tr_MyTransform.localScale = item.StartScale;

				break;
			}

			switch(item.m_ImageEffectType)
			{
			case UIImageEffectType.ChangeColor:

				item.ImageItem.color = item.BeforeColor;

				break;
			}

			for( int i = 0; i < m_TweenEffectItems.Length; i++ )
			{
				item = m_TweenEffectItems[i];
				if( item.ActivationObjects != null )
				{
					for( int k = 0; k < item.ActivationObjects.Length; k++ )
					{
						item.ActivationObjects[k].SetActive(false);
					}
				}

				switch(item.m_ImageEffectType)
				{
				case UIImageEffectType.SpriteAnimation:
					item.ImageItem.sprite = item.SpriteAnimation[0];
					item.ImageItem.SetNativeSize();
					break;
				}
			}
		}
	}

	public void SetEndCB(System.Action endCB)
	{
		m_EndCB = endCB;
	}

	public void OnClickBtn_Enable()
	{
		if( m_TweenEffectItems == null ) return;
		if( !m_bIsInited ) Awake();

		m_bIsFinished = true;

		if( !UseImageComponent )
		{
			if( m_CanvasGroup == null )
			{
				m_RectTransform	= gameObject.GetComponent<RectTransform>();
				m_CanvasGroup 	= gameObject.AddComponent<CanvasGroup>();
			}
		}
		else
		{
			if( m_Img_Icon == null )
			{
				m_RectTransform	= gameObject.GetComponent<RectTransform>();
				m_Img_Icon	 	= gameObject.GetComponent<Image>();
			}
		}
		
		if( !m_bIsReCalcLocalPos ) CalcLocalPosition(ref m_TweenEffectItems);
		
		Lt_SingtaTweenEffectList.Clear();
		Lt_SingtaTweenEffectList.AddRange(m_TweenEffectItems);
		
//		StopAllCoroutines();

		m_Tr_MyTransform.DOKill();
		
		StopCoroutine("IE_Update_");
		StartCoroutine("IE_Update_");
	}

	public void OnClickBtn_Disable()
	{
//		if( m_TweenEffectItems == null ) return;
//
//		m_Tr_MyTransform.DOKill();
//
//		StopCoroutine("IE_Update_");
//		StopCoroutine("IE_SpriteAnimation_");
//
//		if( m_TweenEffectItems.Length > 0 )
//		{
//			SingtaTweenEffectData item;
//			for( int i = 0; i < m_TweenEffectItems.Length; i++ )
//			{
//				item = m_TweenEffectItems[i];
//				if( item.ImageItem != null )
//				{
//					item.ImageItem.SetGrayScale(false);
//				}
//
//				if( item.ActivationObjects != null )
//				{
//					for( int k = 0; k < item.ActivationObjects.Length; k++ )
//					{
//						item.ActivationObjects[k].SetActive(false);
//					}
//				}
//
//				switch(item.m_ImageEffectType)
//				{
//				case UIImageEffectType.SpriteAnimation:
//					item.ImageItem.sprite = item.SpriteAnimation[0];
//					break;
//				}
//			}
//		}
	}

	public void OnClickBtn_Disable(float _alpha)
	{
		if( m_TweenEffectItems == null ) return;

		m_Tr_MyTransform.DOKill();

		StopCoroutine("IE_Update_");

		if( !UseImageComponent )
		{
			m_CanvasGroup.alpha = _alpha;
		}
		else
		{
			m_Img_Icon.color = new Color(m_Img_Icon.color.r, m_Img_Icon.color.g, m_Img_Icon.color.b, _alpha);
		}
	}

	public void FakeFinished()
	{
		if( m_TweenEffectItems == null ) return;

		m_Tr_MyTransform.DOKill();

		StopCoroutine("IE_Update_");

		if( m_TweenEffectItems.Length > 0 )
		{
			SingtaTweenEffectData item;
			for( int i = m_TweenEffectItems.Length-1; i >= 0; i-- )
			{
				item = m_TweenEffectItems[i];
				if( item.m_TweenType == UITweenEffectType.Transfer )
				{
					m_Tr_MyTransform.localPosition = item.EndPos;
					break;
				}

				if( item.m_ImageEffectType == UIImageEffectType.ReplaceSprite )
				{
					item.ImageItem.sprite = item.ReplaceSprite;
					item.ImageItem.SetNativeSize();
				}
			}
		}
	}

	public void ForcedAlpha(float _val)
	{
		if( m_CanvasGroup == null )
		{
			if( gameObject.GetComponent<CanvasGroup>() == null )
			{
				m_CanvasGroup 	= gameObject.AddComponent<CanvasGroup>();
			}
			else
			{
				m_CanvasGroup	= gameObject.GetComponent<CanvasGroup>();
			}
		}

		m_fCustomAlpha = _val;
		m_CanvasGroup.alpha = m_fCustomAlpha;
	}

	void OnFinished(SingtaTweenEffectData _data)
	{
		m_Tr_MyTransform.DOKill();
		switch(_data.m_TweenType)
		{
		case UITweenEffectType.Transfer:
			m_Tr_MyTransform.localPosition = _data.EndPos;
			break;

		case UITweenEffectType.Scale:
			m_Tr_MyTransform.localScale = _data.EndScale;
			break;

		case UITweenEffectType.PunchScale:
			m_Tr_MyTransform.localScale = _data.EndScale;
			break;

		case UITweenEffectType.PunchRotation:
			m_Tr_MyTransform.localRotation = Quaternion.identity;
			break;
		}
	}
	
	void OnCompleted()
	{
		if( Lt_SingtaTweenEffectList.Count > 0 )
		{
			m_Tr_MyTransform.DOKill();
			
			StopCoroutine("IE_Update_");
			StartCoroutine("IE_Update_");
		}
		else
		{
			if( m_TweenEffectItems[m_TweenEffectItems.Length-1].IsDisableInCompleted )
			{
				OnClickBtn_Disable();
				gameObject.SetActive(false);
			}
			else if( !UseImageComponent )
			{
				if( m_CanvasGroup != null )
				{
					if( m_TweenEffectItems[m_TweenEffectItems.Length-1].FadeInOut )
					{
						m_CanvasGroup.alpha = m_TweenEffectItems[m_TweenEffectItems.Length-1].FadeCurve.Evaluate(m_TweenEffectItems[m_TweenEffectItems.Length-1].Duration);
					}
				}
			}
			else
			{
				if( m_Img_Icon != null )
				{
					if( m_TweenEffectItems[m_TweenEffectItems.Length-1].FadeInOut )
					{
						m_Img_Icon.color = new Color(m_Img_Icon.color.r, m_Img_Icon.color.g, m_Img_Icon.color.b, 
							m_TweenEffectItems[m_TweenEffectItems.Length-1].FadeCurve.Evaluate(m_TweenEffectItems[m_TweenEffectItems.Length-1].Duration));
					}
				}
			}
		}
		if(m_EndCB != null)
			m_EndCB();
	}

	void CalcLocalPosition(ref SingtaTweenEffectData[] _data)
	{
		m_bIsReCalcLocalPos		= true;

		float fIntervalX 		= 0.0f;
		float fIntervalY 		= 0.0f;
		Vector2 vec2LocalPos 	= Vector2.zero;

		for( int i = 0; i < _data.Length; i++ )
		{
			fIntervalX = _data[i].StartPos.x - m_RectTransform.anchoredPosition.x;
			fIntervalY = _data[i].StartPos.y - m_RectTransform.anchoredPosition.y;

			vec2LocalPos = new Vector2(m_Tr_MyTransform.localPosition.x, m_Tr_MyTransform.localPosition.y);

			_data[i].StartPos = new Vector3(vec2LocalPos.x + fIntervalX, vec2LocalPos.y + fIntervalY, 0.0f);

			fIntervalX = _data[i].EndPos.x - m_RectTransform.anchoredPosition.x;
			fIntervalY = _data[i].EndPos.y - m_RectTransform.anchoredPosition.y;
			
			_data[i].EndPos = new Vector3(vec2LocalPos.x + fIntervalX, vec2LocalPos.y + fIntervalY, 0.0f);
		}
	}

	IEnumerator IE_Update_()
	{
		yield return new WaitForEndOfFrame();
		
		List<SingtaTweenEffectData> tempList_deleteIdx = new List<SingtaTweenEffectData>();
		for( int i = 0; i < Lt_SingtaTweenEffectList.Count; i++ )
		{
			SingtaTweenEffectData item = Lt_SingtaTweenEffectList[i];
			
			if( !item.Loop || item.Once ) 
			{ 
				tempList_deleteIdx.Add(item); 
			}
			
			if( item.FadeInOut )
			{
				if( !UseImageComponent )
				{
					if( Lt_SingtaTweenEffectList.Count.Equals(1) && (item.Delay > 0) )
					{
						m_CanvasGroup.alpha = 0f;
					}
					else
					{
						m_CanvasGroup.alpha = item.FadeCurve.Evaluate(0.0f);
					}
				}
				else
				{
					if( Lt_SingtaTweenEffectList.Count.Equals(1) && (item.Delay > 0) )
					{
						m_Img_Icon.color = new Color(m_Img_Icon.color.r, m_Img_Icon.color.g, m_Img_Icon.color.b, 0.0f);
					}
					else
					{
						m_Img_Icon.color = new Color(m_Img_Icon.color.r, m_Img_Icon.color.g, m_Img_Icon.color.b, item.FadeCurve.Evaluate(0.0f));
					}
				}
			}
			else
			{
				if( !UseImageComponent )
				{
					m_CanvasGroup.alpha = 1.0f;
				}
				else
				{
					m_Img_Icon.color = new Color(m_Img_Icon.color.r, m_Img_Icon.color.g, m_Img_Icon.color.b, 1.0f);
				}
			}

			transform.DOKill();
			switch(item.m_TweenType)
			{
			case UITweenEffectType.Transfer:
				
				m_Tr_MyTransform.localPosition = item.StartPos;

				if (item.IsLocal)
					m_Tr_MyTransform.DOLocalMove(item.EndPos, item.Duration).SetEase(item.Ease).SetDelay(item.Delay);
				else
					m_Tr_MyTransform.DOMove(item.EndPos, item.Duration).SetEase(item.Ease).SetDelay(item.Delay);

				break;
				
			case UITweenEffectType.Scale:
				
				m_Tr_MyTransform.localScale = item.StartScale;

				if( !item.UseAnimationCurve )
				{
					m_Tr_MyTransform.DOScale(item.EndScale, item.Duration).SetEase(item.Ease).SetDelay(item.Delay);
				}
				else
				{
					StopCoroutine("IE_CurveScaleX_");
					StartCoroutine(IE_CurveScaleX_(item.ScaleXCurve, item.Duration));
				}

				break;
				
			case UITweenEffectType.PunchScale:

				m_Tr_MyTransform.localScale = item.StartScale;

				m_Tr_MyTransform.DOSTPunchScaleXY(item.PunchScalePower, item.PunchScalePower, item.Duration);

				break;

			case UITweenEffectType.PunchRotation:

				m_Tr_MyTransform.localRotation = Quaternion.identity;

				m_Tr_MyTransform.DOSTPunchRotation(item.PunchRotationAxis, item.Duration).SetDelay(item.Delay * 0.6f);

				break;

			case UITweenEffectType.Rotation:

				if (item.IsLocal)
					m_Tr_MyTransform.DOLocalRotate(item.Angle, item.Duration).SetEase(item.Ease).SetDelay(item.Delay);
				else
					m_Tr_MyTransform.DORotate(item.Angle, item.Duration).SetEase(item.Ease).SetDelay(item.Delay);
				
				break;

			case UITweenEffectType.Nothing:

				m_Tr_MyTransform.localRotation = Quaternion.identity;

				break;
			}

			if( item.Delay > 0f )
			{
				yield return new WaitForSeconds(item.Delay);
			}

			switch(item.m_ImageEffectType)
			{
			case UIImageEffectType.GrayScale:
//				item.ImageItem.SetGrayScale(true);
				break;

			case UIImageEffectType.Dissolve:
				break;

			case UIImageEffectType.Blur:
				break;

			case UIImageEffectType.ReplaceSprite:
				item.ImageItem.sprite = item.ReplaceSprite;
				item.ImageItem.SetNativeSize();
				break;

			case UIImageEffectType.SpriteAnimation:
				StopCoroutine("IE_SpriteAnimation_");
				StartCoroutine(IE_SpriteAnimation_(item.SpriteAnimationTime, item.SpriteAnimation, item.ImageItem));
				break;

			case UIImageEffectType.ChangeColor:
				StopCoroutine("IE_ChangeColor_");
				StartCoroutine(IE_ChangeColor_(item.ImageItem, item.BeforeColor, item.AfterColor, item.Duration));
				break;
			}

			if( item.ActivationObjects != null )
			{
				for( int k = 0; k < item.ActivationObjects.Length; k++ )
				{
					item.ActivationObjects[k].SetActive(true);
				}
			}

			if( item.FadeInOut )
			{
				if( !UseImageComponent )
				{
					float fStart = UnityEngine.Time.time;
					while(fStart + (item.Duration) > UnityEngine.Time.time)
					{
						m_CanvasGroup.alpha = item.FadeCurve.Evaluate(UnityEngine.Time.time-fStart);

						yield return null;
					}
				}
				else
				{
					m_Img_Icon.CrossFadeAlpha(item.FadeCurve.Evaluate(item.Duration), item.Duration, false);
				}
			}
			else
			{
				yield return new WaitForSeconds(item.Duration);
			}

			OnFinished(item);

			if( item.Sleep > 0f )
			{
				yield return new WaitForSeconds(item.Sleep);
			}
		}
		
		for( int i = 0; i < tempList_deleteIdx.Count; i++ )
		{
			if( Lt_SingtaTweenEffectList.Contains(tempList_deleteIdx[i]) )
			{
				Lt_SingtaTweenEffectList.Remove(tempList_deleteIdx[i]);
			}
		}
		
		OnCompleted();

//		STEffectItemManager.inst.UnRegisterUITweenEffect(this);
		
		yield break;
	}

	IEnumerator IE_CurveScaleX_(AnimationCurve _curve, float _duration)
	{
		float fStart = Time.time;
		while(fStart+_duration > Time.time)
		{
			m_Tr_MyTransform.localScale = new Vector3(_curve.Evaluate(Time.time-fStart), 1f, 1f);

			yield return null;
		}

		yield break;
	}

	IEnumerator IE_SpriteAnimation_(float _time, Sprite[] _sprites, Image _img)
	{
		float fAveTime = _time / _sprites.Length;

		for( int i = 0; i < _sprites.Length; i++ )
		{
			_img.sprite = _sprites[i];
			_img.SetNativeSize();

			yield return new WaitForSeconds(fAveTime);
		}
		
		yield break;
	}

	IEnumerator IE_ChangeColor_(Image _img, Color _before_col, Color _after_col, float _duration)
	{
		_img.color = _before_col;

		AnimationCurve curve_	= new AnimationCurve();
		curve_.AddKey(0f, 0f);
		curve_.AddKey(_duration, 1f);

		Vector4 vec4ChangeVal_ 	= new Vector4(_before_col.r - _after_col.r, _before_col.g- _after_col.g, _before_col.b - _after_col.b, _before_col.a - _after_col.a);

		float fVal_		= 0f;
		float fStart 	= Time.time;
		while(fStart+_duration > Time.time)
		{
			fVal_ 	= curve_.Evaluate(Time.time-fStart);

			_img.color = new Color(_before_col.r-(fVal_*vec4ChangeVal_.x), _before_col.g-(fVal_*vec4ChangeVal_.y), _before_col.g-(fVal_*vec4ChangeVal_.z), 
				_before_col.a-(fVal_*vec4ChangeVal_.w));

			yield return null;
		}

		_img.color = _after_col;

		yield break;
	}
}

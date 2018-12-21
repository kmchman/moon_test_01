using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class RenderTextureCamera : MonoBehaviour
{
	private static RenderTextureCamera s_Instance = null;
	public static RenderTextureCamera instance { get{ return s_Instance; } }

	[SerializeField] private Transform m_DefaultRootTransform;
	[SerializeField] private Transform m_GiantRootTransform;
	[SerializeField] private Transform m_TowerRootTransform;
	[SerializeField] private Transform m_NPCRootTransform;

	[SerializeField] private Vector3 m_DefaultStartRotate;
	[SerializeField] private Vector3 m_GiantStartRotate;
	[SerializeField] private Vector3 m_TowerStartRotate;
	[SerializeField] private Vector3 m_NPCStartRotate;

	public int idCharData { get { return m_IDCharData; } }

	private Camera m_Camera;
	private RenderTexture m_RenderTexture;

	private System.Action<GameObject> m_CompleteShowCharacterDelegate;

	private int m_IDSequence;
	private int m_IDCharData;
	private GameObject m_CharacterObject;
	private Animator m_CharacterAnimator;

	private void Awake()
	{
		useGUILayout = false;

		if (s_Instance != null)
		{
			GameObject.Destroy(gameObject);
			return;
		}

		s_Instance = this;
		m_Camera = GetComponent<Camera>();

		gameObject.SetActive(false);
	}

	public RenderTexture PlayCamera()
	{
		gameObject.SetActive(true);

		if (m_RenderTexture == null)
		{
			RenderTextureFormat format = RenderTextureFormat.ARGB4444;
			if (!SystemInfo.SupportsRenderTextureFormat(format))
				format = RenderTextureFormat.ARGB32;
			
			m_RenderTexture = new RenderTexture(1024, 1024, 16, format);
			m_Camera.targetTexture = m_RenderTexture;
		}

		return m_RenderTexture;
	}

	public void StopCamera()
	{		
		m_Camera.targetTexture = null;

		if (m_RenderTexture != null)
		{
			m_RenderTexture.Release();
			m_RenderTexture = null;
		}

		gameObject.SetActive(false);
	}

	public void ShowCharacter(int idCharData, bool isShowDialog, System.Action<GameObject> completeShowCharacterDelegate)
	{
		if (m_IDCharData == idCharData)
			return;

		ReleaseCharacterAsset();

		int idSequence = ++m_IDSequence;
		m_IDCharData = idCharData;
		m_CompleteShowCharacterDelegate = completeShowCharacterDelegate;
//		AssetBundleManager.Inst.AsyncGetCharacterAsset(m_IDCharData, (prefab) =>
//		{
//			GetCharacterAsset(idSequence, idCharData, isShowDialog, prefab);
//		});
	}
		
	public void ReleaseCharacterAsset()
	{
		m_IDCharData = 0;

		if (m_CharacterObject != null)
		{
			GameObject.Destroy(m_CharacterObject);
			m_CharacterObject = null;
		}
	}

	public void PlayCharacterWinAnimation()
	{
		PlayCharacterAnimation("WIN");
	}

	private void PlayCharacterAnimation(string animationName)
	{
		if (m_CharacterAnimator != null)
		{
			m_CharacterAnimator.Play(animationName);
		}
	}

	private void InvokeCompleteShowCharacter(GameObject characterObject)
	{
		if (m_CompleteShowCharacterDelegate != null)
			m_CompleteShowCharacterDelegate(characterObject);
	}

//	private void GetCharacterAsset(int idSequence, int idCharData, bool isShowDialog, Object prefab)
//	{
//		if (m_IDSequence != idSequence || prefab == null)
//		{
//			InvokeCompleteShowCharacter(null);
//			return;
//		}
//
//		m_CharacterObject = GameObjectPool.Pop(prefab, true);
//		m_CharacterAnimator = m_CharacterObject.GetComponent<Animator>();
//
//		Transform characterTransform = m_CharacterObject.transform;
//		SetCharacterTransform(idCharData, isShowDialog, characterTransform);
//
//		GameObject.Destroy(m_CharacterObject.GetComponent<CharacterBase>());
//		GameObject.Destroy(m_CharacterObject.GetComponent<CapsuleCollider>());
//		GameObject.Destroy(m_CharacterObject.GetComponent<NavMeshAgent>());
//
//		InvokeCompleteShowCharacter(m_CharacterObject);
//
//		AssetBundleManager.Inst.UnloadCharacter(m_IDCharData, null);
//	}
//
//	private void SetCharacterTransform(int idCharData, bool isShowDialog, Transform characterTransform)
//	{
//		ROLE role = (ROLE)Datatable.Inst.GetCharRole(idCharData);
//
//		Transform parentTransform = m_DefaultRootTransform;
//		Vector3 rotate = m_DefaultStartRotate;
//
//		switch(role)
//		{
//		case ROLE.GIANT:
//			parentTransform = m_GiantRootTransform;
//			rotate = m_GiantStartRotate;
//			break;
//
//		case ROLE.TOWER:
//			parentTransform = m_TowerRootTransform;
//			rotate = m_TowerStartRotate;
//			break;
//
//		case ROLE.NPC:
//			parentTransform = m_NPCRootTransform;
//			rotate = m_NPCStartRotate;
//			break;
//		}
//
//		characterTransform.parent = parentTransform;
//
//		characterTransform.localPosition = Vector3.zero;
//		characterTransform.localEulerAngles = rotate;
//
//		if(isShowDialog)
//			characterTransform.localScale = Vector3.one * Datatable.Inst.GetDialogCharUIScale(idCharData);
//		else
//			characterTransform.localScale = Vector3.one * Datatable.Inst.GetCharUIScale(idCharData);
//	}
}
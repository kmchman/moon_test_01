using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

public class CharacterDetailViewItem : MonoBehaviour
{
	[SerializeField] private RawImage m_image;

	private GameObject m_CharacterObject;
	private Transform m_CharacterTransform;

	private Vector2 m_LastDragPosition;
	private bool m_isReserveWinAnimation;

	public void Show()
	{
		gameObject.SetActive(true);

		m_image.texture = RenderTextureCamera.instance.PlayCamera();
	}

	public void Show(int idCharData, bool isShowDialog = false)
	{
		if (RenderTextureCamera.instance.idCharData == idCharData)
			return;

		Reset();

		Hide();

		m_image.texture = RenderTextureCamera.instance.PlayCamera();
		RenderTextureCamera.instance.ShowCharacter(idCharData, isShowDialog, (characterObject) =>
		{
			if (characterObject == null)
				return;
			
			m_CharacterObject = characterObject;
			m_CharacterTransform = m_CharacterObject.transform;

			gameObject.SetActive(true);
				
			if (m_isReserveWinAnimation)
				PlayWinAnimation();
		});
	}

	public void Hide()
	{
		m_image.texture = null;

		gameObject.SetActive(false);
	}

	public void HideAndRelease()
	{
		Reset();

		m_image.texture = null;

		RenderTextureCamera.instance.ReleaseCharacterAsset();
		RenderTextureCamera.instance.StopCamera();

		gameObject.SetActive(false);
	}

	public void PlayWinAnimation()
	{
		if (m_CharacterObject == null)
			m_isReserveWinAnimation = true;
		else
			RenderTextureCamera.instance.PlayCharacterWinAnimation();
	}

	private void Reset()
	{
		m_CharacterObject = null;
		m_CharacterTransform = null;
		m_isReserveWinAnimation = false;
	}

	public void OnBeginDrag(BaseEventData data)
	{
		if (m_CharacterObject == null)
			return;

		PointerEventData eventData = data as PointerEventData;
		m_LastDragPosition = eventData.pointerCurrentRaycast.screenPosition;
	}

	public void OnDrag(BaseEventData data)
	{
		if (m_CharacterObject == null)
			return;

		PointerEventData eventData = data as PointerEventData;

		if (eventData.pointerPress != eventData.pointerCurrentRaycast.gameObject)
			return;

		float angleY = m_CharacterTransform.localEulerAngles.y;

		angleY += m_LastDragPosition.x - eventData.pointerCurrentRaycast.screenPosition.x;
		m_CharacterTransform.localEulerAngles = new Vector3(m_CharacterTransform.localEulerAngles.x, angleY, m_CharacterTransform.localEulerAngles.z);

		m_LastDragPosition = eventData.pointerCurrentRaycast.screenPosition;
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIGameColorSetter : MonoBehaviour
{
	[SerializeField] private ColorType colorType;

	public ColorType currentColorType
	{
		get { return colorType; }
	}

	void Start()
	{
		if (GlobalDataStore.Inst == null)
			return;

		Graphic graphic = GetComponent<Graphic>();
		if (graphic != null)
			graphic.color = GlobalDataStore.Inst.GetGameColor(colorType);
	}
}

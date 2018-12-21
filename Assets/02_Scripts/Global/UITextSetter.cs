using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITextSetter : MonoBehaviour
{
	[SerializeField] private UITextEnum textEnum;
	[SerializeField] private string [] textParams;

#if UNITY_EDITOR
	public int textEnumInt {
		get { return (int)textEnum; }
		set { textEnum = (UITextEnum)value; }
	}
#endif
	
	private bool isInit = false;

	void OnEnable()
	{
		if (!isInit)
			StartCoroutine("SetText");
	}

	IEnumerator SetText()
	{
		yield return null;

//		while (!Datatable.Inst.isLoaded)
//			yield return null;
//
//		isInit = true;
//
//		Text uiText = GetComponent<Text>();
//		if (uiText != null)
//			uiText.text = Datatable.Inst.GetUIText(textEnum, textParams);
	}
}

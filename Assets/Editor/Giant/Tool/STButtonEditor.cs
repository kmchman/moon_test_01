using UnityEngine;
using UnityEngine.UI;

using UnityEditor;

using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[CanEditMultipleObjects, CustomEditor(typeof(STButton), true)]
public class STButtonEditor : UnityEditor.UI.ButtonEditor
{
	private SerializedProperty m_NormalStateAnimatonEffectorType;
	private SerializedProperty m_PressedStateAnimatonEffectorType;

	private SerializedProperty m_AnimationRectTransform;

	private SerializedProperty m_Markers;

	protected override void OnEnable()
	{
		base.OnEnable();

		m_NormalStateAnimatonEffectorType = serializedObject.FindProperty("m_NormalStateAnimatonEffectorType");;
		m_PressedStateAnimatonEffectorType = serializedObject.FindProperty("m_PressedStateAnimatonEffectorType");

		m_AnimationRectTransform = serializedObject.FindProperty("m_AnimationRectTransform");

		m_Markers = serializedObject.FindProperty("m_Markers");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		DrawSTButtonProperty();
	}

	private void DrawSTButtonProperty()
	{
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Addition Options", EditorStyles.boldLabel);

		++EditorGUI.indentLevel;

		serializedObject.Update();

		DrawStateAnimationType();

		EditorGUILayout.PropertyField(m_AnimationRectTransform, true);

		EditorGUILayout.PropertyField(m_Markers, true);

		serializedObject.ApplyModifiedProperties();

		--EditorGUI.indentLevel;
	}

	private void DrawStateAnimationType()
	{
		EditorGUILayout.LabelField("State Animation Type");

		++EditorGUI.indentLevel;

		EditorGUILayout.PropertyField(m_NormalStateAnimatonEffectorType, new GUIContent("Normal"), true);
		EditorGUILayout.PropertyField(m_PressedStateAnimatonEffectorType, new GUIContent("Pressed"), true);

		--EditorGUI.indentLevel;
	}
}
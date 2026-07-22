using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(email))]
public class EmailEditor : Editor
{
    public override void OnInspectorGUI()
	{
		serializedObject.Update();
		SerializedProperty structProp = serializedObject.FindProperty("Email");
		EditorGUILayout.PropertyField(structProp, new GUIContent("Email"), includeChildren: true);
		serializedObject.ApplyModifiedProperties();
	}
}
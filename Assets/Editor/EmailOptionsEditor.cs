using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(emailOptions))]
public class EmailOptionsEditor : Editor
{
    public override void OnInspectorGUI()
	{
		serializedObject.Update();
		SerializedProperty structProp = serializedObject.FindProperty("Email Options");
		EditorGUILayout.PropertyField(structProp, new GUIContent("Email Options"), includeChildren: true);
		serializedObject.ApplyModifiedProperties();
	}
}

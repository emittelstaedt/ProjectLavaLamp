using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(entry))]
public class EntryEditor : Editor
{
    public override void OnInspectorGUI()
	{
		serializedObject.Update();
		SerializedProperty structProp = serializedObject.FindProperty("Entry");
		EditorGUILayout.PropertyField(structProp, new GUIContent("Entry"), includeChildren: true);
		serializedObject.ApplyModifiedProperties();
	}
}
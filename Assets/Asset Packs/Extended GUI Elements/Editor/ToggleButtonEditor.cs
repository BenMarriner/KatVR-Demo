using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;

[CustomEditor(typeof(ToggleButton))]
public class ToggleButtonEditor : ToggleSelectableEditor
{
    SerializedObject sobject;
    protected override void OnEnable()
    {
        base.OnEnable();
        sobject = new SerializedObject(target);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.PropertyField(sobject.FindProperty("m_OnClick"), true);
        sobject.ApplyModifiedProperties();
    }
}

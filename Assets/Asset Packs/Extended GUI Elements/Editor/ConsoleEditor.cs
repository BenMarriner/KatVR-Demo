using UnityEngine.UI;
using UnityEditor;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(Console))]
    public class ConsoleEditor : Editor
    {

        SerializedObject sobject;

        protected virtual void OnEnable()
        {
            sobject = new SerializedObject(target);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(sobject.FindProperty("_cursor"));
            EditorGUILayout.PropertyField(sobject.FindProperty("_consoleText"));
            EditorGUILayout.PropertyField(sobject.FindProperty("_singleton"));
            EditorGUILayout.PropertyField(sobject.FindProperty("_allowInput"));

            EditorGUILayout.PropertyField(sobject.FindProperty("_blinkRate"));


            EditorGUILayout.LabelField("Escape Character Options", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(sobject.FindProperty("_useEscapeChar"));
            if (sobject.FindProperty("_useEscapeChar").boolValue)
            {
                EditorGUILayout.PropertyField(sobject.FindProperty("_escapeChar"));
            }


            EditorGUILayout.LabelField("Password Options", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(sobject.FindProperty("_usePasswordChar"));
            if (sobject.FindProperty("_usePasswordChar").boolValue)
            {
                EditorGUILayout.PropertyField(sobject.FindProperty("_passwordChar"));
            }

            sobject.ApplyModifiedProperties();
        }
    }
}
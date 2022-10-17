using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(FPSLabel))]
    public class FPSLabelEditor : UnityEditor.UI.TextEditor
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
            EditorGUILayout.LabelField("FPS Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(sobject.FindProperty("_updateInterval"));
            EditorGUILayout.PropertyField(sobject.FindProperty("_formatString"));
        }
    }
}
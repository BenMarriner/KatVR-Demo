using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(SelectionPanel))]
    public class SelectionPanelEditor : SelectableEditor
    {
        //SelectionPanel selectionpanel;
        SerializedObject sobject;
        BooleanTypes _booltypes;

        bool _useBooleanOnly = false;

        BooleanTypes boolTypes
        {
            get
            {
                return _booltypes;
            }
            set
            {
                if (sobject.FindProperty("_selectionOptions").arraySize == 0)
                {
                    sobject.FindProperty("_selectionOptions").InsertArrayElementAtIndex(0);
                }
                if (sobject.FindProperty("_selectionOptions").arraySize == 1)
                {
                    sobject.FindProperty("_selectionOptions").InsertArrayElementAtIndex(0);
                }
                switch (value)
                {
                    case BooleanTypes.OnOff:
                        sobject.FindProperty("_selectionOptions").GetArrayElementAtIndex(0).stringValue = "Off";
                        sobject.FindProperty("_selectionOptions").GetArrayElementAtIndex(1).stringValue = "On";
                        break;
                    case BooleanTypes.YesNo:
                        sobject.FindProperty("_selectionOptions").GetArrayElementAtIndex(0).stringValue = "No";
                        sobject.FindProperty("_selectionOptions").GetArrayElementAtIndex(1).stringValue = "Yes";
                        break;
                    case BooleanTypes.TrueFalse:
                        sobject.FindProperty("_selectionOptions").GetArrayElementAtIndex(0).stringValue = "False";
                        sobject.FindProperty("_selectionOptions").GetArrayElementAtIndex(1).stringValue = "True";
                        break;
                    default:
                        sobject.FindProperty("_selectionOptions").GetArrayElementAtIndex(0).stringValue = "Off";
                        sobject.FindProperty("_selectionOptions").GetArrayElementAtIndex(1).stringValue = "On";
                        break;
                }

                _booltypes = value;
            }
        }

        bool useBooleanOnly
        {
            get
            {
                return _useBooleanOnly;
            }
            set
            {
                if (value)
                {
                    sobject.FindProperty("_selectionOptions").ClearArray();
                    sobject.FindProperty("_selectionOptions").InsertArrayElementAtIndex(0);
                    sobject.FindProperty("_selectionOptions").InsertArrayElementAtIndex(1);
                    boolTypes = _booltypes;
                }
                _useBooleanOnly = value;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            sobject = new SerializedObject(target);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //selectionpanel = (SelectionPanel)target;

            sobject.Update();

            EditorGUILayout.PropertyField(sobject.FindProperty("leftButton"), true);
            EditorGUILayout.PropertyField(sobject.FindProperty("rightButton"), true);
            EditorGUILayout.PropertyField(sobject.FindProperty("selectionText"), true);


            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(sobject.FindProperty("_allowOverflow"));
            EditorGUILayout.PropertyField(sobject.FindProperty("_changeTimerMin"));
            EditorGUILayout.PropertyField(sobject.FindProperty("_changeTimerMax"));



            EditorGUILayout.Separator();

            if (useBooleanOnly)
            {
                useBooleanOnly = EditorGUILayout.Toggle("Use Boolean Values", useBooleanOnly);

                boolTypes = (BooleanTypes)EditorGUILayout.EnumPopup(boolTypes);
            }
            else if (sobject.FindProperty("_useNumbersOnly").boolValue)
            {
                EditorGUILayout.PropertyField(sobject.FindProperty("_useNumbersOnly"));
                EditorGUILayout.Separator();
                EditorGUILayout.PropertyField(sobject.FindProperty("_minValue"));
                EditorGUILayout.PropertyField(sobject.FindProperty("_maxValue"));
            }
            else
            {
                useBooleanOnly = EditorGUILayout.Toggle("Use Boolean Values", useBooleanOnly);
                EditorGUILayout.PropertyField(sobject.FindProperty("_useNumbersOnly"));
                EditorGUILayout.Separator();

                EditorGUILayout.PropertyField(sobject.FindProperty("_selectionOptions"), true);

            }

            EditorGUILayout.PropertyField(sobject.FindProperty("_index"), true);

            ((SelectionPanel)target).Index = sobject.FindProperty("_index").intValue;

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(sobject.FindProperty("_onTextChanged"), true);


            sobject.ApplyModifiedProperties();

            if ((!sobject.FindProperty("_useNumbersOnly").boolValue || useBooleanOnly) && sobject.FindProperty("_index").intValue >= sobject.FindProperty("_selectionOptions").arraySize)
            {
                sobject.FindProperty("_index").intValue = sobject.FindProperty("_selectionOptions").arraySize - 1;
            }
            else if (sobject.FindProperty("_index").intValue >= sobject.FindProperty("_maxValue").intValue - sobject.FindProperty("_minValue").intValue)
            {
                sobject.FindProperty("_index").intValue = (sobject.FindProperty("_maxValue").intValue - sobject.FindProperty("_minValue").intValue) - 1;
            }

            if (sobject.FindProperty("_minValue").intValue >= sobject.FindProperty("_maxValue").intValue)
            {
                sobject.FindProperty("_maxValue").intValue = sobject.FindProperty("_minValue").intValue + 1;

            }

            if (sobject.FindProperty("_index").intValue <= -1)
            {
                sobject.FindProperty("_index").intValue = 0;
            }
            sobject.ApplyModifiedProperties();
        }

        enum BooleanTypes
        {
            OnOff,
            YesNo,
            TrueFalse,
        }
    }
}
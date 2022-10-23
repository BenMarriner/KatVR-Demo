using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(Window), true)]
    public class WindowEditor : Editor
    {
        protected SerializedProperty _dragable;
        protected SerializedProperty _resizable;
        protected SerializedProperty _windowInteractable;
        protected SerializedProperty _targetGraphics;
        protected SerializedProperty _borderSize;
        protected SerializedProperty _minSize;
        protected SerializedProperty _maxSize;
        protected SerializedProperty _windowComponents;
        protected SerializedProperty _dragAreaHeight;
        protected SerializedProperty _transitionState;
        protected SerializedProperty _colors;
        protected SerializedProperty _sprites;
        protected SerializedProperty _onWindowOpened;
        protected SerializedProperty _onWindowClosed;
        protected SerializedProperty _onWindowFocused;
        protected SerializedProperty _onWindowUnfocused;
        protected SerializedProperty _onWindowDragged;
        protected SerializedProperty _onWindowResized;
        protected SerializedProperty _onWindowEnabled;
        protected SerializedProperty _onWindowDisabled;

        public void OnEnable()
        {
            _dragable = serializedObject.FindProperty("_dragable");
            _resizable = serializedObject.FindProperty("_resizable");
            _windowInteractable = serializedObject.FindProperty("_windowInteractable");
            _targetGraphics = serializedObject.FindProperty("_targetGraphics");
            _borderSize = serializedObject.FindProperty("_borderSize");
            _minSize = serializedObject.FindProperty("_minSize");
            _maxSize = serializedObject.FindProperty("_maxSize");
            _windowComponents = serializedObject.FindProperty("_windowComponents");
            _dragAreaHeight = serializedObject.FindProperty("_dragAreaHeight");
            _transitionState = serializedObject.FindProperty("_transitionState");
            _colors = serializedObject.FindProperty("_colors");
            _sprites = serializedObject.FindProperty("_sprites");
            _onWindowOpened = serializedObject.FindProperty("_onWindowOpened");
            _onWindowClosed = serializedObject.FindProperty("_onWindowClosed");
            _onWindowFocused = serializedObject.FindProperty("_onWindowFocused");
            _onWindowUnfocused = serializedObject.FindProperty("_onWindowUnfocused");
            _onWindowDragged = serializedObject.FindProperty("_onWindowDragged");
            _onWindowResized = serializedObject.FindProperty("_onWindowResized");
            _onWindowEnabled = serializedObject.FindProperty("_onWindowEnabled");
            _onWindowDisabled = serializedObject.FindProperty("_onWindowDisabled");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_targetGraphics, new GUIContent("Target Graphics"));
            EditorGUILayout.PropertyField(_dragAreaHeight, new GUIContent("Drag Area Height"));
            EditorGUILayout.PropertyField(_windowComponents, new GUIContent("Window Components", "Objects which should not be disabled when the window is not interactable, eg. Close buttons."), true);

            EditorGUILayout.PropertyField(_dragable, new GUIContent("Dragable"));
            EditorGUILayout.PropertyField(_resizable, new GUIContent("Resizable"));
            EditorGUILayout.PropertyField(_windowInteractable, new GUIContent("Interactable"));

            EditorGUILayout.PropertyField(_minSize, new GUIContent("Min Size"));
            EditorGUILayout.PropertyField(_maxSize, new GUIContent("Max Size"));
            EditorGUILayout.PropertyField(_borderSize, new GUIContent("Border Size"));

            EditorGUILayout.PropertyField(_transitionState, new GUIContent("Transition"));

            if ((TransitionStates)_transitionState.enumValueIndex == TransitionStates.Colors)
            {
                EditorGUILayout.PropertyField(_colors, true);
            }
            else if ((TransitionStates)_transitionState.enumValueIndex == TransitionStates.Sprites)
            {
                EditorGUILayout.PropertyField(_sprites, true);
            }
            else
            {
                EditorGUILayout.PropertyField(_colors, true);
                EditorGUILayout.PropertyField(_sprites, true);
            }

            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_onWindowOpened, new GUIContent("On Open"));
            EditorGUILayout.PropertyField(_onWindowClosed, new GUIContent("On Close"));
            EditorGUILayout.PropertyField(_onWindowFocused, new GUIContent("On Focus"));
            EditorGUILayout.PropertyField(_onWindowUnfocused, new GUIContent("On UnFocus"));
            EditorGUILayout.PropertyField(_onWindowDragged, new GUIContent("On Drag"));
            EditorGUILayout.PropertyField(_onWindowResized, new GUIContent("On Resize"));
            EditorGUILayout.PropertyField(_onWindowEnabled, new GUIContent("On Enabled"));
            EditorGUILayout.PropertyField(_onWindowDisabled, new GUIContent("On Disabled"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
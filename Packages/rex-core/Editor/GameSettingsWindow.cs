using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace REXTools.REXCore
{
    public class GameSettingsWindow : EditorWindow
    {
        private Vector2 scroll;

        //private GameSettings instance;
        private Editor gameSettings;
        private Editor gameEditorSettings;

        public void retrieveProperties()
        {
            // Debug.Log(_ETERNAL.I);
            gameSettings = Editor.CreateEditor(_ETERNAL.I.gameSettings, typeof(E));
            gameEditorSettings = Editor.CreateEditor(_ETERNAL.I.gameEditorSettings, typeof(GameEditorSettings));
        }

        [MenuItem("Window/Game Settings")]
        public static void OpenWindow()
        {
            GetWindow<GameSettingsWindow>("Game Settings");
        }

        private void OnEnable()
        {
            _ETERNAL.I = ((GameObject)Resources.Load("_ETERNAL")).GetComponent<_ETERNAL>();

            retrieveProperties();
        }

        private void OnFocus()
        {
            retrieveProperties();
        }

        private void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Width(position.width), GUILayout.Height(position.height));
            EditorGUI.BeginChangeCheck();

            if (gameSettings != null)
            {
                EditorGUILayout.LabelField("Game Settings", EditorStyles.boldLabel);

                gameSettings.OnInspectorGUI();
            }

            if (gameEditorSettings != null)
            {
                EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);

                gameEditorSettings.OnInspectorGUI();
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(new Object[] { gameSettings, gameEditorSettings }, "Changed Game Settings");
            }
            EditorGUILayout.EndScrollView();
        }
    }
}

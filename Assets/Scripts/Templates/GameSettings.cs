using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TransformTools;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "GameSettings", menuName = "Project/Game Settings")]
[System.Serializable]
public class GameSettings : ScriptableObject
{
    public static GameSettings I
    {
        get
        {
            return _ETERNAL.I.gameSettings;
        }
    }

    public float tileSize = 0.5f;

    public List<Tag> gameTags;

    public static Dictionary<Axis, Color> axisColours = new Dictionary<Axis, Color>
    {
        { Axis.X, new Color(219f / 255f, 62f / 255f, 29f / 255f) },
        { Axis.Y, new Color(154 / 255f, 243 / 255f, 72f / 255f) },
        { Axis.Z, new Color(58f / 255f, 122f / 255f, 237f / 255f) }
    };

#if UNITY_EDITOR
    [CustomEditor(typeof(GameSettings))]
    public class E : Editor
    {
        private new GameSettings target;

        private void OnEnable()
        {
            target = (GameSettings)base.target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (target != I)
            {
                if (GUILayout.Button("Set as Main"))
                {
                    _ETERNAL.I.gameSettings = target;
                }
            }
        }
    }

    public class W : EditorWindow
    {
        private Vector2 scroll;

        //private GameSettings instance;
        private Editor gameSettings;
        private Editor gameEditorSettings;

        public void retrieveProperties()
        {
            // Debug.Log(_ETERNAL.I);
            gameSettings = Editor.CreateEditor(_ETERNAL.I.gameSettings, typeof(E));
            gameEditorSettings = Editor.CreateEditor(_ETERNAL.I.gameEditorSettings, typeof(GameEditorSettings.E));
        }

        [MenuItem("Window/Game Settings")]
        public static void OpenWindow ()
        {
            GetWindow<W>("Game Settings");
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
            
            if (gameSettings != null) {
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
#endif
}

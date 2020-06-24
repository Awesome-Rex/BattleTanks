using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "GameSettings", menuName = "Project/GameSettings")] [System.Serializable]
public class GameSettings : ScriptableObject
{
    public static GameSettings current;

    public float tileSize = 0.5f;

    public List<Tag> gameTags;

    public static Dictionary<Axis, Color> axisColours = new Dictionary<Axis, Color>
    {
        { Axis.X, new Color(219f / 255f, 62f / 255f, 29f / 255f) },
        { Axis.Y, new Color(154 / 255f, 243 / 255f, 72f / 255f) },
        { Axis.Z, new Color(58f / 255f, 122f / 255f, 237f / 255f) }
    };

    public void SetAsCurrent ()
    {
        current = this;
    }

    private void OnEnable()
    {
        
    }

#if UNITY_EDITOR
    public class W : EditorWindow
    {
        //private GameSettings instance;
        private Editor instanceEditor;

        [MenuItem("Window/Game Settings")]
        public static void OpenWindow ()
        {
            GetWindow<W>("Game Settings");
        }

        private void OnEnable()
        {
            instanceEditor = Editor.CreateEditor(GameSettings.current, typeof(E));
        }

        private void OnFocus()
        {
            instanceEditor = Editor.CreateEditor(GameSettings.current, typeof(E));
        }

        private void OnGUI()
        {
            //instance = (GameSettings)EditorGUILayout.ObjectField("Instance", null, typeof(GameSettings), true);
            
            if (instanceEditor != null) {
                instanceEditor.OnInspectorGUI();
            }
        }
    }


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

            if (target != current) {
                if (GUILayout.Button("Set as Current"))
                {
                    (target).SetAsCurrent();
                }
            }
        }
    }
#endif
}

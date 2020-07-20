using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CreateAssetMenu(fileName = "GameEditorSettings", menuName = "Project/Game Editor Settings")]
[System.Serializable]
public class GameEditorSettings : ScriptableObject
{
    public static GameEditorSettings I
    {
        get
        {
            return _ETERNAL.I.gameEditorSettings;
        }
    }

    public List<GUIStyle> styles;

    [CustomEditor(typeof(GameEditorSettings))]
    public class E : Editor
    {
        public new GameEditorSettings target;


        private void OnEnable()
        {
            target = (GameEditorSettings)base.target;
            EditorUtility.SetDirty(target);
        }

        public override void OnInspectorGUI()
        {
            /*foreach (KeyValuePair<string, GUIStyle> i in target.styles)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                var newKey = EditorGUILayout.TextField(i.Key);
                var newValue = i.Value.
                if (EditorGUI.EndChangeCheck())
                {

                }
                EditorGUILayout.EndHorizontal();
            }*/
            base.OnInspectorGUI();

            if (target != I)
            {
                if (GUILayout.Button("Set as Main"))
                {
                    _ETERNAL.I.gameEditorSettings = target;
                }
            }
        }
    }
#endif
}

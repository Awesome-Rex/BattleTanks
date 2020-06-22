#if false
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameSettings))]
public class GameSettingsCustomEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        if (GUILayout.Button("Open Editor"))
        {
            GameSettingsEditorWindow.Open((GameSettings)target);
        }
    }
}
#endif

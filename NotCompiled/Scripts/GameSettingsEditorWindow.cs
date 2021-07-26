#if false
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameSettingsEditorWindow : ExtendedEditorWindow
{
    public static void Open (GameSettings instance)
    {
        GameSettingsEditorWindow window = GetWindow<GameSettingsEditorWindow>("Game Settings");
        window.serializedObject = new SerializedObject(instance);
    }

    private void OnGUI()
    {
        currentProperty = serializedObject.FindProperty("GameSettings");
        DrawProperties(currentProperty, true);
    }
}
#endif

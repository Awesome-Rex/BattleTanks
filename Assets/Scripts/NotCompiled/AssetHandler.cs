#if false
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class AssetHandler
{
    [OnOpenAsset()]
    public static bool OpenEditor (int instanceID, int line)
    {
        GameSettings obj = EditorUtility.InstanceIDToObject(instanceID) as GameSettings;

        if (obj != null)
        {
            GameSettingsEditorWindow.Open(obj);
            return true;
        }

        return false;
    }
}
#endif
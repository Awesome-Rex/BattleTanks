using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace REXTools.REXCore
{
    [CreateAssetMenu(fileName = "GameEditorSettings", menuName = "REX/Game Editor Settings")]
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
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace REXTools.REXCore
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "REX/Game Settings")]
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
    }
}
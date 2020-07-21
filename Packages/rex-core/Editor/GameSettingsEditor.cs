using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace REXTools.REXCore
{
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

            //if (target != I)
            //{
            //    if (GUILayout.Button("Set as Main"))
            //    {
            //        _ETERNAL.I.gameSettings = target;
            //    }
            //}
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace REXTools.REXCore
{
    [CustomEditor(typeof(GameEditorSettings))]
    public class GameEditorSettingsEditor : Editor
    {
        public new GameEditorSettings target;

        //delegates
        //public static System.Action SetCheckedEnumMenuItems;

        //methods
        //[MenuItem("File/Reload Menu Item Enums")]
        //private static void SetCheckedEnumMenuItemsEvent ()
        //{
        //    if (SetCheckedEnumMenuItems != null) {
        //        SetCheckedEnumMenuItems();
        //    }
        //}


        //events
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

            //if (target != I)
            //{
            //    if (GUILayout.Button("Set as Main"))
            //    {
            //        _ETERNAL.I.gameEditorSettings = target;
            //    }
            //}
        }
    }
}
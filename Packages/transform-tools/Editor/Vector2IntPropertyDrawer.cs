using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using REXTools.REXCore;

namespace REXTools.TransformTools
{
    [CustomPropertyDrawer(typeof(Vector2Int))]
    public class Vector2IntPropertyDrawer : PropertyDrawerPRO
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIPRO(position, property, label, () =>
            {
                UnityEngine.Vector2Int value = EditorGUI.Vector2IntField(position, label, new UnityEngine.Vector2Int(
                    property.FindPropertyRelative("x").intValue,
                    property.FindPropertyRelative("y").intValue
                ));

                property.FindPropertyRelative("x").intValue = value.x;
                property.FindPropertyRelative("y").intValue = value.y;
            });
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + (!EditorGUIUtility.wideMode ? lineHeight : 0f);
        }
    }
}
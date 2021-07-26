using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using REXTools.REXCore;

namespace REXTools.TransformTools
{
    [CustomPropertyDrawer(typeof(Vector3Int))]
    public class Vector3IntPropertyDrawer : PropertyDrawerPRO
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIPRO(position, property, label, () =>
            {
                UnityEngine.Vector3Int value = EditorGUI.Vector3IntField(position, label, new UnityEngine.Vector3Int(
                    property.FindPropertyRelative("x").intValue,
                    property.FindPropertyRelative("y").intValue,
                    property.FindPropertyRelative("z").intValue
                ));

                property.FindPropertyRelative("x").intValue = value.x;
                property.FindPropertyRelative("y").intValue = value.y;
                property.FindPropertyRelative("z").intValue = value.z;
            });
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + (!EditorGUIUtility.wideMode ? lineHeight : 0f);
        }
    }
}
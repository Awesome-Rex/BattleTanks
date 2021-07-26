using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using REXTools.REXCore;
using REXTools.EditorTools;

namespace REXTools.TransformTools
{
    [CustomPropertyDrawer(typeof(Vector3Float))]
    public class Vector3FloatPropertyDrawer : PropertyDrawerPRO
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIPRO(position, property, label, () =>
            {
                Vector3 value = EditorGUI.Vector3Field(position, label, new Vector3(
                    property.FindPropertyRelative("x").floatValue,
                    property.FindPropertyRelative("y").floatValue,
                    property.FindPropertyRelative("z").floatValue
                ));

                property.FindPropertyRelative("x").floatValue = value.x;
                property.FindPropertyRelative("y").floatValue = value.y;
                property.FindPropertyRelative("z").floatValue = value.z;
            });
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + (!EditorGUIUtility.wideMode ? lineHeight : 0f);
        }
    }
}
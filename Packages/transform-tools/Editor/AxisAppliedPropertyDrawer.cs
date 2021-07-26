using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using REXTools.REXCore;
using REXTools.EditorTools;

namespace REXTools.TransformTools
{
    [CustomPropertyDrawer(typeof(AxisApplied))]
    public class AxisAppliedPropertyDrawer : PropertyDrawerPRO
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIPRO(position, property, label, () =>
            {
                EditorGUI.PropertyField(newPosition, property.FindPropertyRelative("axis"), label);

                if (EditorGUIUtility.wideMode)
                {
                    newPosition.x = position.x + EditorGUIUtility.labelWidth - lineHeight * 3f;
                    newPosition.width = lineHeight * 2f;//EditorGUIUtility.labelWidth - lineHeight;
                }
                else 
                {
                    newPosition.x = position.x + EditorGUIUtility.labelWidth;
                    newPosition.width = position.width - EditorGUIUtility.labelWidth;
                }
                EditorGUI.PropertyField(newPosition, property.FindPropertyRelative("units"), GUIContent.none);

                newPosition.position = position.position; //newPosition reset
                newPosition.width = lineHeight;
                if (EditorGUIUtility.wideMode)
                {
                    newPosition.x += EditorGUIUtility.labelWidth - lineHeight/* - 2f*/;
                }
                else
                {
                    newPosition.y += lineHeight;
                    //newPosition.x = position.x + position.width - lineHeight;
                }
                EditorGUI.PropertyField(newPosition, property.FindPropertyRelative("space"));
            });
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + (!EditorGUIUtility.wideMode ? lineHeight : 0f);
        }
    }
}
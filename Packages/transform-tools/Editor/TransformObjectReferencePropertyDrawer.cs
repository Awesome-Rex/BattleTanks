using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using REXTools.EditorTools;
using REXTools.REXCore;

namespace REXTools.TransformTools
{
    [CustomPropertyDrawer(typeof(TransformObjectReference))]
    public class TransformObjectReferencePropertyDrawer : PropertyDrawerPRO
    {
        private bool maximized = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIPRO(position, property, label, () =>
            {
                lines = 1f;

                maximized = EditorGUI.Foldout(newPosition, maximized, property.displayName);

                if (maximized)
                {
                    lines += 3f;

                    EditorGUI.indentLevel += 1;

                    bool originalEnabled = GUI.enabled;
                    GUI.enabled = false;

                    //Debug.Log((property.FindPropertyRelative("_position").objectReferenceValue) != null);
                    
                    newPosition.y += lineHeight;
                    EditorGUI.LabelField(newPosition, "Position");
                    //property.FindPropertyRelative("_position").vector3Value = EditorGUI.Vector3Field(newPosition, "Position", property.FindPropertyRelative("_position").vector3Value);
                    //if (!EditorGUIUtility.wideMode)
                    //{
                    //    lines += 1f;
                    //    newPosition.y += lineHeight;
                    //}

                    newPosition.y += lineHeight;
                    EditorGUI.LabelField(newPosition, "Rotation");
                    //property.FindPropertyRelative("_rotation").quaternionValue = Quaternion.Euler(EditorGUI.Vector3Field(newPosition, "Rotation", property.FindPropertyRelative("_rotation").quaternionValue.eulerAngles));
                    //if (!EditorGUIUtility.wideMode)
                    //{
                    //    lines += 1f;
                    //    newPosition.y += lineHeight;
                    //}

                    newPosition.y += lineHeight;
                    EditorGUI.LabelField(newPosition, "Scale");
                    //property.FindPropertyRelative("_scale").vector3Value = EditorGUI.Vector3Field(newPosition, "Scale", property.FindPropertyRelative("_scale").vector3Value);
                    //if (!EditorGUIUtility.wideMode)
                    //{
                    //    lines += 1f;
                    //    newPosition.y += lineHeight;
                    //}

                    GUI.enabled = originalEnabled;
                }
            });
        }
    }
}
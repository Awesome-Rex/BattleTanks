using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using REXTools.EditorTools;
using REXTools.REXCore;

namespace REXTools.TransformTools
{
    [CustomPropertyDrawer(typeof(TransformObjectComponent))]
    public class TransformObjectComponentPropertyDrawer : PropertyDrawerPRO
    {
        private bool maximized = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIPRO(position, property, label, () =>
            {
                lines = 1f;

                if (property.FindPropertyRelative("transform").objectReferenceValue != null)
                {
                    maximized = EditorGUI.Foldout(newPosition, maximized, GUIContent.none /*property.displayName*/);
                }
                property.FindPropertyRelative("transform").objectReferenceValue = EditorGUI.ObjectField(newPosition, property.displayName, property.FindPropertyRelative("transform").objectReferenceValue, typeof(Transform), true);

                if (maximized)
                {
                    //lines += 1f;

                    EditorGUI.indentLevel += 1;

                    //newPosition.y += lineHeight;

                    if (property.FindPropertyRelative("transform").objectReferenceValue != null) {
                        lines += 3f;

                        Undo.RecordObject((Transform)property.FindPropertyRelative("transform").objectReferenceValue, "Undo Inspector");
                        newPosition.y += lineHeight;
                        ((Transform)property.FindPropertyRelative("transform").objectReferenceValue).position = EditorGUI.Vector3Field(newPosition, "Position", ((Transform)property.FindPropertyRelative("transform").objectReferenceValue).position);
                        if (!EditorGUIUtility.wideMode)
                        {
                            lines += 1f;
                            newPosition.y += lineHeight;
                        }

                        newPosition.y += lineHeight;
                        ((Transform)property.FindPropertyRelative("transform").objectReferenceValue).rotation = Quaternion.Euler(EditorGUI.Vector3Field(newPosition, "Position", ((Transform)property.FindPropertyRelative("transform").objectReferenceValue).rotation.eulerAngles));
                        if (!EditorGUIUtility.wideMode)
                        {
                            lines += 1f;
                            newPosition.y += lineHeight;
                        }

                        newPosition.y += lineHeight;
                        ((Transform)property.FindPropertyRelative("transform").objectReferenceValue).localScale = EditorGUI.Vector3Field(newPosition, "Scale", ((Transform)property.FindPropertyRelative("transform").objectReferenceValue).localScale);
                        if (!EditorGUIUtility.wideMode)
                        {
                            lines += 1f;
                            newPosition.y += lineHeight;
                        }
                    }
                }
            });
        }
    }
}
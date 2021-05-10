using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using REXTools.REXCore;

namespace REXTools.TransformTools
{
    [CustomPropertyDrawer(typeof(TransformObject), true)]
    public class TransformObjectPropertyDrawer : PropertyDrawerPRO
    {
        private bool maximized = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIPRO(position, property, label, () =>
            {
                //++++++++++Add context menu to switch ReferenceType
                //Context menu
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && position.Contains(Event.current.mousePosition))
                {
                    GenericMenu context = new GenericMenu();

                    context.AddItem(new GUIContent("Transform"), property.FindPropertyRelative("referenceType").enumValueIndex == (int)TransformObject.ReferenceType.Transform, () => ApplyModifiedProperties(() => property.FindPropertyRelative("referenceType").enumValueIndex = (int)TransformObject.ReferenceType.Transform, property));
                    context.AddItem(new GUIContent("Constant"), property.FindPropertyRelative("referenceType").enumValueIndex == (int)TransformObject.ReferenceType.Constant, () => ApplyModifiedProperties(() => property.FindPropertyRelative("referenceType").enumValueIndex = (int)TransformObject.ReferenceType.Constant, property));
                    context.AddItem(new GUIContent("Delegate Function"), property.FindPropertyRelative("referenceType").enumValueIndex == (int)TransformObject.ReferenceType.Function, () => ApplyModifiedProperties(() => property.FindPropertyRelative("referenceType").enumValueIndex = (int)TransformObject.ReferenceType.Function, property));

                    context.ShowAsContext();
                }

                if (property.FindPropertyRelative("referenceType").enumValueIndex == (int)TransformObject.ReferenceType.Transform)
                {
                    lines = 1f;

                    //if (property.FindPropertyRelative("transform").objectReferenceValue != null)
                    //{
                    //    maximized = EditorGUI.Foldout(newPosition, maximized, GUIContent.none /*property.displayName*/);
                    //}
                    property.FindPropertyRelative("transform").objectReferenceValue = EditorGUI.ObjectField(newPosition, property.displayName, property.FindPropertyRelative("transform").objectReferenceValue, typeof(Transform), true);

                    //if (maximized)
                    //{
                    //    //lines += 1f;

                    //    EditorGUI.indentLevel += 1;

                    //    //newPosition.y += lineHeight;

                    //    if (property.FindPropertyRelative("transform").objectReferenceValue != null)
                    //    {
                    //        lines += 3f;

                    //        Undo.RecordObject((Transform)property.FindPropertyRelative("transform").objectReferenceValue, "Undo Inspector");
                    //        newPosition.y += lineHeight;
                    //        ((Transform)property.FindPropertyRelative("transform").objectReferenceValue).position = EditorGUI.Vector3Field(newPosition, "Position", ((Transform)property.FindPropertyRelative("transform").objectReferenceValue).position);
                    //        if (!EditorGUIUtility.wideMode)
                    //        {
                    //            lines += 1f;
                    //            newPosition.y += lineHeight;
                    //        }

                    //        newPosition.y += lineHeight;
                    //        ((Transform)property.FindPropertyRelative("transform").objectReferenceValue).rotation = Quaternion.Euler(EditorGUI.Vector3Field(newPosition, "Position", ((Transform)property.FindPropertyRelative("transform").objectReferenceValue).rotation.eulerAngles));
                    //        if (!EditorGUIUtility.wideMode)
                    //        {
                    //            lines += 1f;
                    //            newPosition.y += lineHeight;
                    //        }

                    //        newPosition.y += lineHeight;
                    //        ((Transform)property.FindPropertyRelative("transform").objectReferenceValue).localScale = EditorGUI.Vector3Field(newPosition, "Scale", ((Transform)property.FindPropertyRelative("transform").objectReferenceValue).localScale);
                    //        if (!EditorGUIUtility.wideMode)
                    //        {
                    //            lines += 1f;
                    //            newPosition.y += lineHeight;
                    //        }
                    //    }
                    //}
                }
                else if (property.FindPropertyRelative("referenceType").enumValueIndex == (int)TransformObject.ReferenceType.Constant) {
                    lines = 1f;

                    maximized = EditorGUI.Foldout(newPosition, maximized, property.displayName);

                    if (maximized)
                    {
                        lines += 3f;

                        EditorGUI.indentLevel += 1;

                        newPosition.y += lineHeight;
                        property.FindPropertyRelative("_positionConstant").vector3Value = EditorGUI.Vector3Field(newPosition, "Position", property.FindPropertyRelative("_positionConstant").vector3Value);
                        if (!EditorGUIUtility.wideMode)
                        {
                            lines += 1f;
                            newPosition.y += lineHeight;
                        }

                        newPosition.y += lineHeight;
                        property.FindPropertyRelative("_rotationConstant").quaternionValue = Quaternion.Euler(EditorGUI.Vector3Field(newPosition, "Rotation", property.FindPropertyRelative("_rotationConstant").quaternionValue.eulerAngles));
                        if (!EditorGUIUtility.wideMode)
                        {
                            lines += 1f;
                            newPosition.y += lineHeight;
                        }

                        newPosition.y += lineHeight;
                        property.FindPropertyRelative("_scaleConstant").vector3Value = EditorGUI.Vector3Field(newPosition, "Scale", property.FindPropertyRelative("_scaleConstant").vector3Value);
                        if (!EditorGUIUtility.wideMode)
                        {
                            lines += 1f;
                            newPosition.y += lineHeight;
                        }
                    }
                }
                else if (property.FindPropertyRelative("referenceType").enumValueIndex == (int)TransformObject.ReferenceType.Function)
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
                }
            });
        }
    }
}
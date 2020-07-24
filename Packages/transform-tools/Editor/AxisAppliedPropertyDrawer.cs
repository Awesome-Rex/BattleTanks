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
        private static bool showSpace = false;

        private GUIStyle axisButtonStyle;
        private GUIStyle selectedAxisButtonStyle;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (axisButtonStyle == null || selectedAxisButtonStyle == null)
            {
                axisButtonStyle = EditorStyles.miniButton.clone().richText();
                selectedAxisButtonStyle = EditorStyles.miniButton.clone().richText().fontSize(lineHeight * 0.9f);
            }

            if (!property.serializedObject.isEditingMultipleObjects)
            {
                OnGUIPRO(position, property, label, () =>
                {
                //CONTENT
                EditorGUI.indentLevel = 0;

                #region "COLOURED BUTTONS"

                newPosition.width /= 4;

                    Renter.rentUse(GUI.backgroundColor, (i) => GUI.backgroundColor = i, () =>
                    {
                        GUI.backgroundColor = new Color(219f / 255f, 62f / 255f, 29f / 255f);

                        if (property.FindPropertyRelative("axis").enumValueIndex == (int)Axis.X)
                        {
                            if (GUI.Button(newPosition, "X".colour(Color.white).bold(), EditorStyles.miniButton.clone().richText().fontSize(lineHeight * 0.9f)))
                            {
                                property.FindPropertyRelative("axis").enumValueIndex = (int)Axis.X;
                            }
                        }
                        else
                        {
                        //GUI.backgroundColor *= 1.5f;

                        if (GUI.Button(newPosition, "X".colour(GUI.backgroundColor * 1.5f), axisButtonStyle))
                            {
                                property.FindPropertyRelative("axis").enumValueIndex = (int)Axis.X;
                            }
                        }


                    });


                    newPosition.x += (indentedPosition.width / 4);
                    Renter.rentUse(GUI.backgroundColor, (i) => GUI.backgroundColor = i, () =>
                    {
                        GUI.backgroundColor = new Color(154 / 255f, 243 / 255f, 72f / 255f);

                        if (property.FindPropertyRelative("axis").enumValueIndex == (int)Axis.Y)
                        {
                            if (GUI.Button(newPosition, "Y".colour(Color.white).bold(), selectedAxisButtonStyle))
                            {
                                property.FindPropertyRelative("axis").enumValueIndex = (int)Axis.Y;
                            }
                        }
                        else
                        {
                        //GUI.backgroundColor *= 1.5f;

                        if (GUI.Button(newPosition, "Y".colour(GUI.backgroundColor * 1.5f), axisButtonStyle))
                            {
                                property.FindPropertyRelative("axis").enumValueIndex = (int)Axis.Y;
                            }
                        }
                    });

                    newPosition.x += (indentedPosition.width / 4);
                    Renter.rentUse(GUI.backgroundColor, (i) => GUI.backgroundColor = i, () =>
                    {
                        GUI.backgroundColor = new Color(58f / 255f, 122f / 255f, 237f / 255f);

                        if (property.FindPropertyRelative("axis").enumValueIndex == (int)Axis.Z)
                        {
                            if (GUI.Button(newPosition, "Z".colour(Color.white).bold(), selectedAxisButtonStyle))
                            {
                                property.FindPropertyRelative("axis").enumValueIndex = (int)Axis.Z;
                            }
                        }
                        else
                        {
                        //GUI.backgroundColor *= 1.5f;

                        if (GUI.Button(newPosition, "Z".colour(GUI.backgroundColor * 1.5f), axisButtonStyle))
                            {
                                property.FindPropertyRelative("axis").enumValueIndex = (int)Axis.Z;
                            }
                        }
                    });
                #endregion

                //units
                newPosition.x += (indentedPosition.width / 4);
                    newPosition.width -= lineHeight;
                    property.FindPropertyRelative("units").floatValue = EditorGUI.FloatField(newPosition, GUIContent.none, property.FindPropertyRelative("units").floatValue);

                //show/hide button
                newPosition = indentedPosition;
                    newPosition.size = Vector2.one * lineHeight;
                    newPosition.x = position.width;
                    if (showSpace)
                    {
                        newPosition.y += lineHeight;
                    }
                    if (GUI.Button(newPosition, GUIContent.none))
                    {
                        showSpace = !showSpace;
                    }

                //enum
                if (showSpace)
                    {
                        newPosition = indentedPosition;
                        newPosition.y += lineHeight;
                        newPosition.width -= lineHeight;
                        newPosition.height = lineHeight;

                        if (property.FindPropertyRelative("space").enumValueIndex == (int)Space.Self)
                        {
                            if (GUI.Button(newPosition, "Self", EditorStyles.miniButton))
                            {
                                property.FindPropertyRelative("space").enumValueIndex = (int)Space.World;
                            }
                        }
                        else
                        {
                            if (GUI.Button(newPosition, "World", EditorStyles.miniButton))
                            {
                                property.FindPropertyRelative("space").enumValueIndex = (int)Space.Self;
                            }
                        }

                        lines = 2f;
                    }
                    else
                    {
                        lines = 1f;
                    }
                });
            }
            else
            {
                OnGUIPRO(position, property, label, () =>
                {
                    lines = 1f;

                    GUI.Label(indentedPosition, "Multi-Object Editing is not Supported".colour(Color.red), EditorStyles.boldLabel.clone().richText());
                });
            }
        }
    }
}
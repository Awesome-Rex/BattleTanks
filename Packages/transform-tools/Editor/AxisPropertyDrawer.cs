using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using REXTools.REXCore;
using REXTools.EditorTools;

namespace REXTools.TransformTools
{
    [CustomPropertyDrawer(typeof(Axis))]
    public class AxisPropertyDrawer : PropertyDrawerPRO
    {
        private GUIStyle axisButtonStyle;
        private GUIStyle selectedAxisButtonStyle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIPRO(position, property, label, () =>
            {
                if (axisButtonStyle == null || selectedAxisButtonStyle == null)
                {
                    axisButtonStyle = EditorStyles.miniButton.clone().richText();
                    selectedAxisButtonStyle = EditorStyles.miniButton.clone().richText().fontSize(lineHeight * 0.9f);
                }

                Renter.rentUse(GUI.backgroundColor, (i) => GUI.backgroundColor = i, () =>
                {
                    this.Vector3TField(position,
                        (position) =>
                        {
                            GUI.backgroundColor = new Color(219f / 255f, 62f / 255f, 29f / 255f);

                            if (property.enumValueIndex == (int)Axis.X)
                            {
                                if (GUI.Button(position, "X".colour(Color.white).bold(), EditorStyles.miniButton.clone().richText().fontSize(lineHeight * 0.9f)))
                                {
                                    property.enumValueIndex = (int)Axis.X;
                                }
                            }
                            else
                            {
                                //GUI.backgroundColor *= 1.5f;

                                if (GUI.Button(position, "X".colour(GUI.backgroundColor * 1.5f), axisButtonStyle))
                                {
                                    property.enumValueIndex = (int)Axis.X;
                                }
                            }
                        },
                        (position) =>
                        {
                            GUI.backgroundColor = new Color(154 / 255f, 243 / 255f, 72f / 255f);

                            if (property.enumValueIndex == (int)Axis.Y)
                            {
                                if (GUI.Button(position, "Y".colour(Color.white).bold(), selectedAxisButtonStyle))
                                {
                                    property.enumValueIndex = (int)Axis.Y;
                                }
                            }
                            else
                            {
                                //GUI.backgroundColor *= 1.5f;

                                if (GUI.Button(position, "Y".colour(GUI.backgroundColor * 1.5f), axisButtonStyle))
                                {
                                    property.enumValueIndex = (int)Axis.Y;
                                }
                            }
                        },
                        (position) =>
                        {
                            GUI.backgroundColor = new Color(58f / 255f, 122f / 255f, 237f / 255f);

                            if (property.enumValueIndex == (int)Axis.Z)
                            {
                                if (GUI.Button(position, "Z".colour(Color.white).bold(), selectedAxisButtonStyle))
                                {
                                    property.enumValueIndex = (int)Axis.Z;
                                }
                            }
                            else
                            {
                                //GUI.backgroundColor *= 1.5f;

                                if (GUI.Button(position, "Z".colour(GUI.backgroundColor * 1.5f), axisButtonStyle))
                                {
                                    property.enumValueIndex = (int)Axis.Z;
                                }
                            }
                        },
                        (position) =>
                        {
                            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                        }
                    );
                });
            });
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + (!EditorGUIUtility.wideMode ? lineHeight : 0f);
        }
    }
}
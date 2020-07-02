using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

using TransformTools;
using EditorTools;

[System.Serializable]
public struct AxisApplied
{
    public Axis axis;
    public float units;

    public Space space;

    /*[HideInInspector]
    public SpaceVariety variety;*/

    public AxisApplied(Axis axis, float units,/* SpaceVariety variety = SpaceVariety.OneSided, */Space space = Space.Self)
    {
        this.axis = axis;
        this.units = units;

        this.space = space;
        //this.variety = variety;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(AxisApplied))]
    public class P : PropertyDrawerPRO
    {
        private static bool showSpace = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIPRO(position, property, label, () => {
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
                    } else
                    {
                        //GUI.backgroundColor *= 1.5f;

                        if (GUI.Button(newPosition, "X".colour(GUI.backgroundColor * 1.5f), EditorStyles.miniButton.clone().richText()))
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
                        if (GUI.Button(newPosition, "Y".colour(Color.white).bold(), EditorStyles.miniButton.clone().richText().fontSize(lineHeight * 0.9f)))
                        {
                            property.FindPropertyRelative("axis").enumValueIndex = (int)Axis.Y;
                        }
                    }
                    else
                    {
                        //GUI.backgroundColor *= 1.5f;

                        if (GUI.Button(newPosition, "Y".colour(GUI.backgroundColor * 1.5f), EditorStyles.miniButton.clone().richText()))
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
                        if (GUI.Button(newPosition, "Z".colour(Color.white).bold(), EditorStyles.miniButton.clone().richText().fontSize(lineHeight * 0.9f)))
                        {
                            property.FindPropertyRelative("axis").enumValueIndex = (int)Axis.Z;
                        }
                    }
                    else
                    {
                        //GUI.backgroundColor *= 1.5f;

                        if (GUI.Button(newPosition, "Z".colour(GUI.backgroundColor * 1.5f), EditorStyles.miniButton.clone().richText()))
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
                    
                    if (property.FindPropertyRelative("space").enumValueIndex == (int)Space.Self) {
                        if (GUI.Button(newPosition, "Self", EditorStyles.miniButton)) {
                            property.FindPropertyRelative("space").enumValueIndex = (int)Space.World;
                        }
                    } else
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
    }
#endif
}

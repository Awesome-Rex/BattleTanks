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
                
                
                newPosition.width /= 2;
                EditorGUI.PropertyField(newPosition, property.FindPropertyRelative("axis"), GUIContent.none);

                newPosition.x += indentedPosition.width / 2f;
                newPosition.width -= lineHeight;
                EditorGUI.PropertyField(newPosition, property.FindPropertyRelative("units"), GUIContent.none);
                


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

                if (showSpace)
                {
                    newPosition = indentedPosition;
                    newPosition.y += lineHeight;
                    newPosition.width -= lineHeight;
                    newPosition.height = lineHeight;
                    EditorGUI.PropertyField(newPosition, property.FindPropertyRelative("space"), GUIContent.none);
                    lines = 2f;
                }
                else
                {
                    lines = 1f;
                }

                //END
            });
        }
    }
#endif
}

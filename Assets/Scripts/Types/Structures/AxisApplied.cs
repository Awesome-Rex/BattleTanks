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
    public class P : PropertyDrawer
    {
        private static bool showSpace = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //SETUP
            Rect indentedPosition = EditorGUI.IndentedRect(position);
            
            int indentLevelOG = EditorGUI.indentLevel;

            float lineHeight = base.GetPropertyHeight(property, label);

            //START
            EditorGUI.BeginProperty(indentedPosition, label, property);
            
            Rect newPosition = indentedPosition;

            //CONTENT
            EditorGUI.indentLevel = 0;

            newPosition.height = lineHeight;

            newPosition.width /= 2;
            EditorGUI.PropertyField(newPosition, property.FindPropertyRelative("axis"), GUIContent.none);

            newPosition.x += indentedPosition.width / 2f;
            EditorGUI.PropertyField(newPosition, property.FindPropertyRelative("units"), GUIContent.none);


            newPosition = indentedPosition;
            newPosition.size = Vector2.one * lineHeight;
            newPosition.x -= lineHeight;
            if (GUI.Button(newPosition, GUIContent.none))
            {
                showSpace = !showSpace;
            }

            if (showSpace)
            {
                newPosition = indentedPosition;
                newPosition.height = lineHeight;
                newPosition.y += lineHeight;
                EditorGUI.PropertyField(newPosition, property.FindPropertyRelative("space"));
            }
            
            //END
            EditorGUI.indentLevel = indentLevelOG;
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * (showSpace ? 2f : 1f);
        }
    }
#endif
}

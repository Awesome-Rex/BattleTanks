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
	   
	   /*Rect indentedPosition = EditorGUI.IndentedRect(position);

            EditorGUI.BeginProperty(indentedPosition, label, property);


            Color backgroundColorOG = GUI.backgroundColor;
            int indentLevelOG = EditorGUI.indentLevel;

            Rect newPosition = indentedPosition;
            newPosition.width /= 4;
            newPosition.x = indentedPosition.x;


            GUI.backgroundColor = new Color(219f / 255f, 62f / 255f, 29f / 255f);
            //if (((AxisApplied)(fieldInfo.GetValue(property.serializedObject.targetObject) as AxisApplied?)).axis == Axis.X)
            //{
                if (GUI.Button(newPosition, "X", EditorStyles.miniButton))
                {

                }
            //}

            newPosition.x += (indentedPosition.width / 4);
            //if (((AxisApplied)(fieldInfo.GetValue(property.serializedObject.targetObject) as AxisApplied?)).axis == Axis.Y)
            //{
                GUI.backgroundColor = new Color(154 / 255f, 243 / 255f, 72f / 255f);
                if (GUI.Button(newPosition, "Y", EditorStyles.miniButton))
                {

                }
            //}

            newPosition.x += (indentedPosition.width / 4);
            //if (((AxisApplied)(fieldInfo.GetValue(property.serializedObject.targetObject) as AxisApplied?)).axis == Axis.Z)
            //{
                GUI.backgroundColor = new Color(58f / 255f, 122f / 255f, 237f / 255f);
                if (GUI.Button(newPosition, "Z", EditorStyles.miniButton))
                {
                    //fieldInfo.SetValue(property.serializedObject.targetObject, )
                }
            //}

            //var field = property.serializedObject.targetObject.GetType().GetField(property.propertyPath);
            //Debug.Log(field);
            //if (field != null)
            //{
            //    var value = field.GetValue(property.serializedObject.targetObject);
            //    Debug.Log(((AxisApplied)(value as AxisApplied?)).axis);
            //}



            GUI.backgroundColor = backgroundColorOG;

            EditorGUI.indentLevel = 0;
            newPosition.x += (indentedPosition.width / 4);
            EditorGUI.PropertyField(newPosition, property.FindPropertyRelative("units"), GUIContent.none);
            EditorGUI.indentLevel = indentLevelOG;

            EditorGUI.EndProperty();*/
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using REXTools.REXCore;

namespace REXTools.TransformTools
{
    [CustomPropertyDrawer(typeof(Transition))]
    public class TransitionPropertyDrawer : PropertyDrawerPRO
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIPRO(position, property, label, () =>
            {
                newPosition.width /= 2f;

                if (property.FindPropertyRelative("type").enumValueIndex == (int)Curve.Linear)
                {
                    if (GUI.Button(newPosition, "Linear", EditorStyles.miniButton))
                    {
                        property.FindPropertyRelative("type").enumValueIndex = (int)Curve.Interpolate;
                    }

                    newPosition.x += indentedPosition.width / 2f;

                    EditorGUI.PropertyField(newPosition, property.FindPropertyRelative("speed"), GUIContent.none);

                    lines = 1f;
                }
                else
                {
                    if (GUI.Button(newPosition, "Interpolate", EditorStyles.miniButton))
                    {
                        property.FindPropertyRelative("type").enumValueIndex = (int)Curve.Linear;
                    }

                    newPosition.x += indentedPosition.width / 2f;
                    newPosition.width /= 2f;
                    EditorGUI.PropertyField(newPosition, property.FindPropertyRelative("percent"), GUIContent.none);
                    newPosition.x += indentedPosition.width / 4f;
                    property.FindPropertyRelative("percent").floatValue = EditorGUI.FloatField(newPosition, property.FindPropertyRelative("percent").floatValue / (1f / Time.fixedDeltaTime)) * (1f / Time.fixedDeltaTime);

                    newPosition = indentedPosition;
                    newPosition.y += lineHeight;

                    EditorGUI.ProgressBar(newPosition, property.FindPropertyRelative("percent").floatValue * Time.fixedDeltaTime, string.Empty);

                    lines = 2f;
                }
            });
        }
    }
}

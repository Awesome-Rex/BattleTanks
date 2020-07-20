using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TransformTools
{
    [Serializable]
    public struct Transition
    {
        public Curve type;

        public float speed;

        public float percent;

        public float MoveTowards(float a, float b)
        {
            if (type == Curve.Linear)
            {
                return Mathf.MoveTowards(a, b, speed * Time.fixedDeltaTime);
            }
            else if (type == Curve.Interpolate)
            {
                return Mathf.Lerp(a, b, percent * Time.fixedDeltaTime);
            }

            return a;
        }

        public Vector3 MoveTowards(Vector3 a, Vector3 b)
        {
            if (type == Curve.Linear)
            {
                return Vector3.MoveTowards(a, b, speed * Time.fixedDeltaTime);
            }
            else if (type == Curve.Interpolate)
            {
                return Vector3.Lerp(a, b, percent * Time.fixedDeltaTime);
            }

            return a;
        }

        public Quaternion MoveTowards(Quaternion a, Quaternion b)
        {
            if (type == Curve.Linear)
            {
                return Quaternion.RotateTowards(a, b, speed * Time.fixedDeltaTime);
            }
            else if (type == Curve.Interpolate)
            {
                return Quaternion.Lerp(a, b, percent * Time.fixedDeltaTime);
            }

            return a;
        }

#if UNITY_EDITOR

        [CustomPropertyDrawer(typeof(Transition))]
        public class E : PropertyDrawerPRO
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
#endif
    }
}
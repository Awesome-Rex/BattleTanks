using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct Transition
{
    public Curve type;

    public float speed;

    public float percent;

    public AnimationCurve curve;
    
    public float MoveTowards (float a, float b)
    {
        if (type == Curve.Linear)
        {
            return Mathf.MoveTowards(a, b, speed * Time.deltaTime);
        }
        else if (type == Curve.Interpolate)
        {
            return Mathf.Lerp(a, b, percent * Time.deltaTime);
        } else if (type == Curve.Custom) {
            
        }

        return a;
    }

    public Vector3 MoveTowards(Vector3 a, Vector3 b)
    {
        if (type == Curve.Linear)
        {
            return Vector3.MoveTowards(a, b, speed * Time.deltaTime);
        }
        else if (type == Curve.Interpolate)
        {
            return Vector3.Lerp(a, b, percent * Time.deltaTime);
        }
        else if (type == Curve.Custom)
        {

        }

        return a;
    }

    public Quaternion MoveTowards (Quaternion a, Quaternion b)
    {
        if (type == Curve.Linear)
        {
            return Quaternion.RotateTowards(a, b, speed * Time.deltaTime);
        }
        else if (type == Curve.Interpolate)
        {
            return Quaternion.Lerp(a, b, percent * Time.deltaTime);
        }
        else if (type == Curve.Custom)
        {

        }

        return a;
    }

#if UNITY_EDITOR

    //[CustomPropertyDrawer(typeof(Transition))]
    //public class E : PropertyDrawer
    //{
    //    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //    {
    //        //base.OnGUI(position, property, label);

    //        EditorGUI.BeginProperty(position, label, property);
    //        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Keyboard), label);
    //        EditorGUI.PropertyField(position, property, GUIContent.none);

    //        EditorGUI.EndProperty();
    //    }

    //    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //    {
    //        return base.GetPropertyHeight(property, label);
    //    }
    //}
#endif
}

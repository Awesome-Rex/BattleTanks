using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace REXTools.TransformTools
{
    public enum Curve { Linear, Interpolate }

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
    }
}
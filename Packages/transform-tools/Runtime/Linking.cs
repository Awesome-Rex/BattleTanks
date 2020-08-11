using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.TransformTools
{
    public static class Linking
    {
        //local > world
        public static Vector3 TransformPoint(Vector3 point, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            return Matrix4x4.TRS(position, rotation, scale).MultiplyPoint3x4(point);
        }
        public static Vector3 TransformPoint(Vector3 point, Vector3 position, Quaternion rotation)
        {
            return rotation * point + position;
        }

        //world > local
        public static Vector3 InverseTransformPoint(Vector3 point, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            // *ALTERNATE* return Matrix4x4.TRS(position, rotation, scale).inverse.MultiplyPoint3x4(point);
            return Vector3.Scale(
                Vector3.one.Divide(scale),
                (Quaternion.Inverse(rotation) * (point - position))
            );
        }
        public static Vector3 InverseTransformPoint(Vector3 point, Vector3 position, Quaternion rotation)
        {
            return Vector3.Scale(
                Vector3.one.Divide(Vector3.one/**/),
                (Quaternion.Inverse(rotation) * (point - position))
            );
        }

        //local > world
        public static Quaternion TransformEuler(Quaternion eulers, Quaternion rotation)
        {
            return rotation * eulers;
        }
        //world > local
        public static Quaternion InverseTransformEuler(Quaternion eulers, Quaternion rotation)
        {
            return Quaternion.Inverse(rotation) * eulers;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TransformControl
{
    public enum Axis { X, Y, Z }
    public enum SpaceVariety { OneSided, Mixed }

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
                Vectors.DivideVector3(Vector3.one, scale),
                (Quaternion.Inverse(rotation) * (point - position))
            );
        }
        public static Vector3 InverseTransformPoint(Vector3 point, Vector3 position, Quaternion rotation)
        {
            return Vector3.Scale(
                Vectors.DivideVector3(Vector3.one, Vector3.one/**/),
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

    public static class Vectors
    {
        // THE AXIS BIBLE
        public static Axis[] axisIterate = new Axis[]
        {
        Axis.X, Axis.Y, Axis.Z
        };
        public static Axis[] axisDefaultOrder = new Axis[]
        {
        Axis.X, Axis.Y, Axis.Z
        };

        public static Dictionary<Axis, string> axisNames = new Dictionary<Axis, string>
    {
        { Axis.X, "X" },
        { Axis.Y, "Y" },
        { Axis.Z, "Z" }
    };
        public static Dictionary<Axis, Vector3> axisDirections = new Dictionary<Axis, Vector3>
    {
        { Axis.X, new Vector3(1f, 0f, 0f) },
        { Axis.Y, new Vector3(0f, 1f, 0f) },
        { Axis.Z, new Vector3(0f, 0f, 1f) }
    };

        public static float GetAxis(Axis axis, Vector3 from)
        {
            if (axis == Axis.X)
            {
                return from.x;
            }
            else if (axis == Axis.Y)
            {
                return from.y;
            }
            else if (axis == Axis.Z)
            {
                return from.z;
            }

            return 0f;
        }

        public static Vector3 MultiplyVector3(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        public static Vector3 DivideVector3(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }
        //END
    }
}

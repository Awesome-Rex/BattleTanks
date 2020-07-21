using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.TransformTools
{
    public enum Axis { X, Y, Z }
    public enum SpaceVariety { OneSided, Mixed }

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

        public static Vector3 Rad2Deg(Vector3 rad)
        {
            return new Vector3(rad.x * Mathf.Rad2Deg, rad.y * Mathf.Rad2Deg, rad.z * Mathf.Rad2Deg);
        }
        public static Vector3 Deg2Rad(Vector3 deg)
        {
            return new Vector3(deg.x * Mathf.Deg2Rad, deg.y * Mathf.Deg2Rad, deg.z * Mathf.Deg2Rad);
        }
        //END
    }
}
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

            return default;
        }
        public static Vector3 SetAxis (Axis axis, float value, Vector3 from)
        {
            if (axis == Axis.X)
            {
                return new Vector3(value, from.y, from.z);
            }
            else if (axis == Axis.Y)
            {
                return new Vector3(from.x, value, from.z);
            }
            else if (axis == Axis.Z)
            {
                return new Vector3(from.x, from.y, value);
            }

            return default;
        }

        public static Vector3 OperateVector3(Vector3 a, System.Func<float, float> operation)
        {
            return new Vector3(
                operation(a.x),
                operation(a.y),
                operation(a.z)
            );
        }
        public static Vector2 OperateVector2(Vector2 a, System.Func<float, float> operation)
        {
            return new Vector2(
                operation(a.x),
                operation(a.y)
            );
        }
        public static Vector3 OperateVector3 (Vector3 a, Vector3 b, System.Func<float, float, float> operation)
        {
            return new Vector3(
                operation(a.x, b.x),
                operation(a.y, b.y),
                operation(a.z, b.z)
            );
        }
        public static Vector2 OperateVector2(Vector2 a, Vector2 b, System.Func<float, float, float> operation)
        {
            return new Vector2(
                operation(a.x, b.x),
                operation(a.y, b.y)
            );
        }

        public static Vector3Int OperateVector3Int(Vector3Int a, System.Func<int, int> operation)
        {
            return new Vector3Int(
                operation(a.x),
                operation(a.y),
                operation(a.z)
            );
        }
        public static Vector2Int OperateVector2Int(Vector2Int a, System.Func<int, int> operation)
        {
            return new Vector2Int(
                operation(a.x),
                operation(a.y)
            );
        }
        public static Vector3Int OperateVector3(Vector3Int a, Vector3Int b, System.Func<int, int, int> operation)
        {
            return new Vector3Int(
                operation(a.x, b.x),
                operation(a.y, b.y),
                operation(a.z, b.z)
            );
        }
        public static Vector2Int OperateVector2(Vector2Int a, Vector2Int b, System.Func<int, int, int> operation)
        {
            return new Vector2Int(
                operation(a.x, b.x),
                operation(a.y, b.y)
            );
        }


        public static Vector3 MultiplyVector3(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        public static Vector3 DivideVector3(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }
	    public static Vector2 MultiplyVector2(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }
        public static Vector2 DivideVector2(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x / b.x, a.y / b.y);
        }
	   
        public static Vector3 ReciprocolVector3 (Vector3 a)
        {
            return new Vector3(1f / a.x, 1f / a.y, 1f / a.z);
        }
        public static Vector2 ReciprocolVector2(Vector2 a)
        {
            return new Vector2(1f / a.x, 1f / a.y);
        }

        public static Vector3 Rad2Deg(Vector3 rad)
        {
            return new Vector3(rad.x * Mathf.Rad2Deg, rad.y * Mathf.Rad2Deg, rad.z * Mathf.Rad2Deg);
        }
        public static Vector3 Deg2Rad(Vector3 deg)
        {
            return new Vector3(deg.x * Mathf.Deg2Rad, deg.y * Mathf.Deg2Rad, deg.z * Mathf.Deg2Rad);
        }

        public static bool SignGreater (float a, float b)
        {
            if (a >= 0)
            {
                return a > b;
            } else
            {
                return a < b;
            }
        }
        public static bool SignLess(float a, float b)
        {
            if (a >= 0)
            {
                return a < b;
            }
            else
            {
                return a > b;
            }
        }

        public static float SignZeroed(float f)
        {
            if (f > 0f)
            {
                return 1f;
            }
            else if (f < 0f)
            {
                return -1f;
            }
            else
            {
                return 0f;
            }
        }

        public static float SignCeil (float f, bool includeZero = false)
        {
            if (f < 0)
            {
                return Mathf.Floor(f);
            }
            else if (f > 0)
            {
                return Mathf.Ceil(f);
            }
            else
            {
                if (includeZero)
                {
                    return 0f;
                }
                else
                {
                    return 1f;
                }
            }
        }
        public static float SignFloor(float f/*, bool includeZero = false*/)
        {
            if (f < 0)
            {
                return Mathf.Ceil(f);
            }
            else if (f > 0)
            {
                return Mathf.Floor(f);
            }
            else
            {
                return 0f;

                //if (includeZero)
                //{
                //    return 0f;
                //}
                //else
                //{

                //}
            }
        }
        //END
    }
}
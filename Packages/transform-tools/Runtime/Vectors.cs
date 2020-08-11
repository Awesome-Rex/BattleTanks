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

        //methods
        public static float GetAxis(this Vector3 from, Axis axis)
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
        public static Vector3 SetAxis(this Vector3 from, Axis axis, float value)
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
        public static float GetAxis(this Vector2 from, Axis axis)
        {
            if (axis == Axis.X)
            {
                return from.x;
            }
            else if (axis == Axis.Y)
            {
                return from.y;
            }

            return default;
        }
        public static Vector2 SetAxis(this Vector2 from, Axis axis, float value)
        {
            if (axis == Axis.X)
            {
                return new Vector2(value, from.y);
            }
            else if (axis == Axis.Y)
            {
                return new Vector2(from.x, value);
            }

            return default;
        }
        public static int GetAxis(this Vector3Int from, Axis axis)
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
        public static Vector3Int SetAxis(this Vector3Int from, Axis axis, int value)
        {
            if (axis == Axis.X)
            {
                return new Vector3Int(value, from.y, from.z);
            }
            else if (axis == Axis.Y)
            {
                return new Vector3Int(from.x, value, from.z);
            }
            else if (axis == Axis.Z)
            {
                return new Vector3Int(from.x, from.y, value);
            }

            return default;
        }
        public static int GetAxis(this Vector2Int from, Axis axis)
        {
            if (axis == Axis.X)
            {
                return from.x;
            }
            else if (axis == Axis.Y)
            {
                return from.y;
            }

            return default;
        }
        public static Vector2Int SetAxis(this Vector2Int from, Axis axis, int value)
        {
            if (axis == Axis.X)
            {
                return new Vector2Int(value, from.y);
            }
            else if (axis == Axis.Y)
            {
                return new Vector2Int(from.x, value);
            }

            return default;
        }
        
        public static Vector3 Operate(this Vector3 a, System.Func<Axis, float, float> operation)
        {
            return new Vector3(
                operation(Axis.X, a.x),
                operation(Axis.Y, a.y),
                operation(Axis.Z, a.z)
            );
        }
        public static Vector2 Operate(this Vector2 a, System.Func<Axis, float, float> operation)
        {
            return new Vector2(
                operation(Axis.X, a.x),
                operation(Axis.Y, a.y)
            );
        }
        public static Vector3 Operate(this Vector3 a, Vector3 b, System.Func<Axis, float, float, float> operation)
        {
            return new Vector3(
                operation(Axis.X, a.x, b.x),
                operation(Axis.Y, a.y, b.y),
                operation(Axis.Z, a.z, b.z)
            );
        }
        public static Vector2 Operate(this Vector2 a, Vector2 b, System.Func<Axis, float, float, float> operation)
        {
            return new Vector2(
                operation(Axis.X, a.x, b.x),
                operation(Axis.Y, a.y, b.y)
            );
        }

        public static Vector3Int Operate(this Vector3Int a, System.Func<Axis, int, int> operation)
        {
            return new Vector3Int(
                operation(Axis.X, a.x),
                operation(Axis.Y, a.y),
                operation(Axis.Z, a.z)
            );
        }
        public static Vector2Int Operate(this Vector2Int a, System.Func<Axis, int, int> operation)
        {
            return new Vector2Int(
                operation(Axis.X, a.x),
                operation(Axis.Y, a.y)
            );
        }
        public static Vector3Int Operate(this Vector3Int a, Vector3Int b, System.Func<Axis, int, int, int> operation)
        {
            return new Vector3Int(
                operation(Axis.X, a.x, b.x),
                operation(Axis.Y, a.y, b.y),
                operation(Axis.Z, a.z, b.z)
            );
        }
        public static Vector2Int Operate(this Vector2Int a, Vector2Int b, System.Func<Axis, int, int, int> operation)
        {
            return new Vector2Int(
                operation(Axis.X, a.x, b.x),
                operation(Axis.Y, a.y, b.y)
            );
        }
        
        public static Vector3 Multiply(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        public static Vector2 Multiply(this Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }
        public static Vector3 Divide(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }
        public static Vector2 Divide(this Vector2 a, Vector2 b)
        {
            return new Vector2(a.x / b.x, a.y / b.y);
        }

        public static Vector3Int Multiply(this Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        public static Vector2Int Multiply(this Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x * b.x, a.y * b.y);
        }
        public static Vector3Int Divide(this Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(a.x / b.x, a.y / b.y, a.z / b.z);
        }
        public static Vector2Int Divide(this Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x / b.x, a.y / b.y);
        }

        public static Vector3 Reciprocol(this Vector3 a)
        {
            return new Vector3(1f / a.x, 1f / a.y, 1f / a.z);
        }
        public static Vector2 Reciprocol(this Vector2 a)
        {
            return new Vector2(1f / a.x, 1f / a.y);
        }

        public static Vector3 Rad2Deg(this Vector3 rad)
        {
            return new Vector3(rad.x * Mathf.Rad2Deg, rad.y * Mathf.Rad2Deg, rad.z * Mathf.Rad2Deg);
        }
        public static Vector3 Deg2Rad(this Vector3 deg)
        {
            return new Vector3(deg.x * Mathf.Deg2Rad, deg.y * Mathf.Deg2Rad, deg.z * Mathf.Deg2Rad);
        }

        public static Vector3 CustomRoundVector3(Vector3 f, Vector3 increment, Vector3 offset)
        {
            return f.Operate(offset, (s, a, b) => a - b).Operate(increment, (s, a, b) => Mathf.Round(a / b) * b).Operate(offset, (s, a, b) => a + b); ;
        }
        public static Vector3 CustomRoundVector3(Vector3 f, Vector3 increment)
        {
            return CustomRoundVector3(f, increment, Vector3.zero);
        }
        public static Vector3 CustomRound(this Vector3 f, Vector3 increment, Vector3 offset)
        {
            return CustomRoundVector3(f, increment, offset);
        }
        public static Vector3 CustomRound(this Vector3 f, Vector3 increment)
        {
            return CustomRoundVector3(f, increment, Vector3.zero);
        }
        //END
    }
}
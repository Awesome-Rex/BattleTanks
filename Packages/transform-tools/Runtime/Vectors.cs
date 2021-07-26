using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace REXTools.TransformTools
{
    public enum Axis { X, Y, Z }
    public enum SpaceVariety { OneSided, Mixed }

    public enum PlaneProjection { Reorient, View, Plane }

    public static class Vectors
    {
        // THE AXIS BIBLE
        public readonly static Axis[] axisIterate = new Axis[]
        {
            Axis.X, Axis.Y, Axis.Z
        };
        public readonly static Axis[] axisDefaultOrder = new Axis[]
        {
            Axis.X, Axis.Y, Axis.Z
        };

        public readonly static Dictionary<Axis, string> axisNames = new Dictionary<Axis, string>
        {
            { Axis.X, "X" },
            { Axis.Y, "Y" },
            { Axis.Z, "Z" }
        };
        public readonly static Dictionary<Axis, Vector3> axisDirections = new Dictionary<Axis, Vector3>
        {
            { Axis.X, new Vector3(1f, 0f, 0f) },
            { Axis.Y, new Vector3(0f, 1f, 0f) },
            { Axis.Z, new Vector3(0f, 0f, 1f) }
        };
        public readonly static Dictionary<Axis, Vector2T<Axis>> axisPlanes = new Dictionary<Axis, Vector2T<Axis>>
        {
            { Axis.X, new Vector2T<Axis>(Axis.Z, Axis.Y) },
            { Axis.Y, new Vector2T<Axis>(Axis.X, Axis.Z) },
            { Axis.Z, new Vector2T<Axis>(Axis.X, Axis.Y) }
        };
        public readonly static Dictionary<Vector3, Vector2T<Vector3>> axisPlaneDirections = new Dictionary<Vector3, Vector2T<Vector3>> {
            { axisDirections[Axis.X], new Vector2T<Vector3>(axisDirections[Axis.Z], axisDirections[Axis.Y]) },
            { axisDirections[Axis.Y], new Vector2T<Vector3>(axisDirections[Axis.X], axisDirections[Axis.Z]) },
            { axisDirections[Axis.Z], new Vector2T<Vector3>(axisDirections[Axis.X], axisDirections[Axis.Y]) }
        };

        public static void AxisIterate(System.Action<Axis> operation)
        {
            operation(Axis.X);
            operation(Axis.Y);
            operation(Axis.Z);
        }
        public static void AxisIterate2D(System.Action<Axis> operation)
        {
            operation(Axis.X);
            operation(Axis.Y);
        }

        //METHODS

        //get axis/set axis
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
        public static int GetAxis(this UnityEngine.Vector3Int from, Axis axis)
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
        public static UnityEngine.Vector3Int SetAxis(this UnityEngine.Vector3Int from, Axis axis, int value)
        {
            if (axis == Axis.X)
            {
                return new UnityEngine.Vector3Int(value, from.y, from.z);
            }
            else if (axis == Axis.Y)
            {
                return new UnityEngine.Vector3Int(from.x, value, from.z);
            }
            else if (axis == Axis.Z)
            {
                return new UnityEngine.Vector3Int(from.x, from.y, value);
            }

            return default;
        }
        public static int GetAxis(this UnityEngine.Vector2Int from, Axis axis)
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
        public static UnityEngine.Vector2Int SetAxis(this UnityEngine.Vector2Int from, Axis axis, int value)
        {
            if (axis == Axis.X)
            {
                return new UnityEngine.Vector2Int(value, from.y);
            }
            else if (axis == Axis.Y)
            {
                return new UnityEngine.Vector2Int(from.x, value);
            }

            return default;
        }

        //operation (with same type)
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

        public static UnityEngine.Vector3Int Operate(this Vector3Int a, System.Func<Axis, int, int> operation)
        {
            return new UnityEngine.Vector3Int(
                operation(Axis.X, a.x),
                operation(Axis.Y, a.y),
                operation(Axis.Z, a.z)
            );
        }
        public static UnityEngine.Vector2Int Operate(this Vector2Int a, System.Func<Axis, int, int> operation)
        {
            return new UnityEngine.Vector2Int(
                operation(Axis.X, a.x),
                operation(Axis.Y, a.y)
            );
        }
        public static UnityEngine.Vector3Int Operate(this Vector3Int a, Vector3Int b, System.Func<Axis, int, int, int> operation)
        {
            return new UnityEngine.Vector3Int(
                operation(Axis.X, a.x, b.x),
                operation(Axis.Y, a.y, b.y),
                operation(Axis.Z, a.z, b.z)
            );
        }
        public static UnityEngine.Vector2Int Operate(this Vector2Int a, Vector2Int b, System.Func<Axis, int, int, int> operation)
        {
            return new UnityEngine.Vector2Int(
                operation(Axis.X, a.x, b.x),
                operation(Axis.Y, a.y, b.y)
            );
        }


        //vector3 custom operate
        public static Vector3T<TR> Operate<TR>(this Vector3 a, System.Func<Axis, float, TR> operation)
        {
            return new Vector3T<TR>(
                operation(Axis.X, a.x),
                operation(Axis.Y, a.y),
                operation(Axis.Z, a.z)
            );
        }
        public static Vector3T<TR> Operate<TR, T1>(this Vector3 a, Vector3T<T1> b, System.Func<Axis, float, T1, TR> operation)
        {
            return new Vector3T<TR>(
                operation(Axis.X, a.x, b.x),
                operation(Axis.Y, a.y, b.y),
                operation(Axis.Z, a.z, b.z)
            );
        }
        public static Vector3T<TR> Operate<TR, T1, T2>(this Vector3 a, Vector3T<T1> b, Vector3T<T2> c, System.Func<Axis, float, T1, T2, TR> operation)
        {
            return new Vector3T<TR>(
                operation(Axis.X, a.x, b.x, c.x),
                operation(Axis.Y, a.y, b.y, c.y),
                operation(Axis.Z, a.z, b.z, c.z)
            );
        }

        //vector2 custom operate
        public static Vector2T<TR> Operate<TR>(this Vector2 a, System.Func<Axis, float, TR> operation)
        {
            return new Vector2T<TR>(
                operation(Axis.X, a.x),
                operation(Axis.Y, a.y)
            );
        }
        public static Vector2T<TR> Operate<TR, T1>(this Vector2 a, Vector3T<T1> b, System.Func<Axis, float, T1, TR> operation)
        {
            return new Vector2T<TR>(
                operation(Axis.X, a.x, b.x),
                operation(Axis.Y, a.y, b.y)
            );
        }
        public static Vector2T<TR> Operate<TR, T1, T2>(this Vector2 a, Vector3T<T1> b, Vector3T<T2> c, System.Func<Axis, float, T1, T2, TR> operation)
        {
            return new Vector2T<TR>(
                operation(Axis.X, a.x, b.x, c.x),
                operation(Axis.Y, a.y, b.y, c.y)
            );
        }

        //vector3int custom operate
        public static Vector3T<TR> Operate<TR>(this UnityEngine.Vector3Int a, System.Func<Axis, int, TR> operation)
        {
            return new Vector3T<TR>(
                operation(Axis.X, a.x),
                operation(Axis.Y, a.y),
                operation(Axis.Z, a.z)
            );
        }
        public static Vector3T<TR> Operate<TR, T1>(this UnityEngine.Vector3Int a, Vector3T<T1> b, System.Func<Axis, int, T1, TR> operation)
        {
            return new Vector3T<TR>(
                operation(Axis.X, a.x, b.x),
                operation(Axis.Y, a.y, b.y),
                operation(Axis.Z, a.z, b.z)
            );
        }
        public static Vector3T<TR> Operate<TR, T1, T2>(this UnityEngine.Vector3Int a, Vector3T<T1> b, Vector3T<T2> c, System.Func<Axis, int, T1, T2, TR> operation)
        {
            return new Vector3T<TR>(
                operation(Axis.X, a.x, b.x, c.x),
                operation(Axis.Y, a.y, b.y, c.y),
                operation(Axis.Z, a.z, b.z, c.z)
            );
        }

        //vector2int custom operate
        public static Vector2T<TR> Operate<TR>(this UnityEngine.Vector2Int a, System.Func<Axis, int, TR> operation)
        {
            return new Vector2T<TR>(
                operation(Axis.X, a.x),
                operation(Axis.Y, a.y)
            );
        }
        public static Vector2T<TR> Operate<TR, T1>(this UnityEngine.Vector2Int a, Vector3T<T1> b, System.Func<Axis, int, T1, TR> operation)
        {
            return new Vector2T<TR>(
                operation(Axis.X, a.x, b.x),
                operation(Axis.Y, a.y, b.y)
            );
        }
        public static Vector2T<TR> Operate<TR, T1, T2>(this UnityEngine.Vector2Int a, Vector3T<T1> b, Vector3T<T2> c, System.Func<Axis, int, T1, T2, TR> operation)
        {
            return new Vector2T<TR>(
                operation(Axis.X, a.x, b.x, c.x),
                operation(Axis.Y, a.y, b.y, c.y)
            );
        }


        //operate bool
        public static Vector3Bool OperateBool(this Vector3 a, System.Func<Axis, float, bool> operation)
        {
            return new Vector3Bool(
                operation(Axis.X, a.x),
                operation(Axis.Y, a.y),
                operation(Axis.Z, a.z)
            );
        }
        public static Vector2Bool OperateBool(this Vector2 a, System.Func<Axis, float, bool> operation)
        {
            return new Vector2Bool(
                operation(Axis.X, a.x),
                operation(Axis.Y, a.y)
            );
        }
        public static Vector3Bool OperateBool(this Vector3 a, Vector3 b, System.Func<Axis, float, float, bool> operation)
        {
            return new Vector3Bool(
                operation(Axis.X, a.x, b.x),
                operation(Axis.Y, a.y, b.y),
                operation(Axis.Z, a.z, b.z)
            );
        }
        public static Vector2Bool OperateBool(this Vector2 a, Vector2 b, System.Func<Axis, float, float, bool> operation)
        {
            return new Vector2Bool(
                operation(Axis.X, a.x, b.x),
                operation(Axis.Y, a.y, b.y)
            );
        }

        public static Vector3Bool OperateBool(this Vector3Int a, System.Func<Axis, int, bool> operation)
        {
            return new Vector3Bool(
                operation(Axis.X, a.x),
                operation(Axis.Y, a.y),
                operation(Axis.Z, a.z)
            );
        }
        public static Vector2Bool OperateBool(this Vector2Int a, System.Func<Axis, int, bool> operation)
        {
            return new Vector2Bool(
                operation(Axis.X, a.x),
                operation(Axis.Y, a.y)
            );
        }
        public static Vector3Bool OperateBool(this Vector3Int a, Vector3Int b, System.Func<Axis, int, int, bool> operation)
        {
            return new Vector3Bool(
                operation(Axis.X, a.x, b.x),
                operation(Axis.Y, a.y, b.y),
                operation(Axis.Z, a.z, b.z)
            );
        }
        public static Vector2Bool OperateBool(this Vector2Int a, Vector2Int b, System.Func<Axis, int, int, bool> operation)
        {
            return new Vector2Bool(
                operation(Axis.X, a.x, b.x),
                operation(Axis.Y, a.y, b.y)
            );
        }

        //math
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

        public static UnityEngine.Vector3Int Multiply(this Vector3Int a, Vector3Int b)
        {
            return new UnityEngine.Vector3Int(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        public static UnityEngine.Vector2Int Multiply(this Vector2Int a, Vector2Int b)
        {
            return new UnityEngine.Vector2Int(a.x * b.x, a.y * b.y);
        }
        public static UnityEngine.Vector3Int Divide(this Vector3Int a, Vector3Int b)
        {
            return new UnityEngine.Vector3Int(a.x / b.x, a.y / b.y, a.z / b.z);
        }
        public static UnityEngine.Vector2Int Divide(this Vector2Int a, Vector2Int b)
        {
            return new UnityEngine.Vector2Int(a.x / b.x, a.y / b.y);
        }

        public static Vector3 Abs(this Vector3 a)
        {
            return new Vector3(Mathf.Abs(a.x), Mathf.Abs(a.y), Mathf.Abs(a.z));
        }
        public static Vector2 Abs(this Vector2 a)
        {
            return new Vector2(Mathf.Abs(a.x), Mathf.Abs(a.y));
        }
        public static UnityEngine.Vector3Int Abs(this UnityEngine.Vector3Int a)
        {
            return new UnityEngine.Vector3Int(Mathf.Abs(a.x), Mathf.Abs(a.y), Mathf.Abs(a.z));
        }
        public static UnityEngine.Vector2Int Abs(this UnityEngine.Vector2Int a)
        {
            return new UnityEngine.Vector2Int(Mathf.Abs(a.x), Mathf.Abs(a.y));
        }

        public static Vector3 Round(this Vector3 a)
        {
            return new Vector3(Mathf.Round(a.x), Mathf.Round(a.y), Mathf.Round(a.z));
        }
        public static Vector2 Round(this Vector2 a)
        {
            return new Vector2(Mathf.Round(a.x), Mathf.Round(a.y));
        }
        public static Vector3 Ceil(this Vector3 a)
        {
            return new Vector3(Mathf.Ceil(a.x), Mathf.Ceil(a.y), Mathf.Ceil(a.z));
        }
        public static Vector2 Ceil(this Vector2 a)
        {
            return new Vector2(Mathf.Ceil(a.x), Mathf.Ceil(a.y));
        }
        public static Vector3 Floor(this Vector3 a)
        {
            return new Vector3(Mathf.Floor(a.x), Mathf.Floor(a.y), Mathf.Floor(a.z));
        }
        public static Vector2 Floor(this Vector2 a)
        {
            return new Vector2(Mathf.Floor(a.x), Mathf.Floor(a.y));
        }


        public static UnityEngine.Vector3Int RoundToInt(this Vector3 a)
        {
            return new UnityEngine.Vector3Int(Mathf.RoundToInt(a.x), Mathf.RoundToInt(a.y), Mathf.RoundToInt(a.z));
        }
        public static UnityEngine.Vector2Int RoundToInt(this Vector2 a)
        {
            return new UnityEngine.Vector2Int(Mathf.RoundToInt(a.x), Mathf.RoundToInt(a.y));
        }
        public static UnityEngine.Vector3Int CeilToInt(this Vector3 a)
        {
            return new UnityEngine.Vector3Int(Mathf.CeilToInt(a.x), Mathf.CeilToInt(a.y), Mathf.CeilToInt(a.z));
        }
        public static UnityEngine.Vector2Int CeilToInt(this Vector2 a)
        {
            return new UnityEngine.Vector2Int(Mathf.CeilToInt(a.x), Mathf.CeilToInt(a.y));
        }
        public static UnityEngine.Vector3Int FloorToInt(this Vector3 a)
        {
            return new UnityEngine.Vector3Int(Mathf.FloorToInt(a.x), Mathf.FloorToInt(a.y), Mathf.FloorToInt(a.z));
        }
        public static UnityEngine.Vector2Int FloorToInt(this Vector2 a)
        {
            return new UnityEngine.Vector2Int(Mathf.FloorToInt(a.x), Mathf.FloorToInt(a.y));
        }
        //PROPERTIES
        public static Vector3 Sign(this Vector3 a)
        {
            return new Vector3(Mathf.Sign(a.x), Mathf.Sign(a.y), Mathf.Sign(a.z));
        }
        public static Vector2 Sign(this Vector2 a)
        {
            return new Vector2(Mathf.Sign(a.x), Mathf.Sign(a.y));
        }
        public static UnityEngine.Vector3Int Sign(this UnityEngine.Vector3Int a)
        {
            return new UnityEngine.Vector3Int((int)Mathf.Sign(a.x), (int)Mathf.Sign(a.y), (int)Mathf.Sign(a.z));
        }
        public static UnityEngine.Vector2Int Sign(this UnityEngine.Vector2Int a)
        {
            return new UnityEngine.Vector2Int((int)Mathf.Sign(a.x), (int)Mathf.Sign(a.y));
        }

        public static Vector3 SignZeroed(this Vector3 a)
        {
            return new Vector3(a.x.SignZeroed(), a.y.SignZeroed(), a.z.SignZeroed());
        }
        public static Vector2 SignZeroed(this Vector2 a)
        {
            return new Vector2(a.x.SignZeroed(), a.y.SignZeroed());
        }
        public static UnityEngine.Vector3Int SignZeroed(this UnityEngine.Vector3Int a)
        {
            return new UnityEngine.Vector3Int(((float)a.x).SignZeroedToInt(), ((float)a.y).SignZeroedToInt(), ((float)a.z).SignZeroedToInt());
        }
        public static UnityEngine.Vector2Int SignZeroed(this UnityEngine.Vector2Int a)
        {
            return new UnityEngine.Vector2Int(((float)a.x).SignZeroedToInt(), ((float)a.y).SignZeroedToInt());
        }

        public static Vector3 SignCeil(this Vector3 a)
        {
            return new Vector3(a.x.SignCeil(), a.y.SignCeil(), a.z.SignCeil());
        }
        public static Vector2 SignCeil(this Vector2 a)
        {
            return new Vector2(a.x.SignCeil(), a.y.SignCeil());
        }
        public static UnityEngine.Vector3Int SignCeil(this UnityEngine.Vector3Int a)
        {
            return new UnityEngine.Vector3Int(((float)a.x).SignCeilToInt(), ((float)a.y).SignCeilToInt(), ((float)a.z).SignCeilToInt());
        }
        public static UnityEngine.Vector2Int SignCeil(this UnityEngine.Vector2Int a)
        {
            return new UnityEngine.Vector2Int(((float)a.x).SignCeilToInt(), ((float)a.y).SignCeilToInt());
        }

        public static Vector3 SignFloor(this Vector3 a)
        {
            return new Vector3(a.x.SignFloor(), a.y.SignFloor(), a.z.SignFloor());
        }
        public static Vector2 SignFloor(this Vector2 a)
        {
            return new Vector2(a.x.SignFloor(), a.y.SignFloor());
        }
        public static UnityEngine.Vector3Int SignFloor(this UnityEngine.Vector3Int a)
        {
            return new UnityEngine.Vector3Int(((float)a.x).SignFloorToInt(), ((float)a.y).SignFloorToInt(), ((float)a.z).SignFloorToInt());
        }
        public static UnityEngine.Vector2Int SignFloor(this UnityEngine.Vector2Int a)
        {
            return new UnityEngine.Vector2Int(((float)a.x).SignFloorToInt(), ((float)a.y).SignFloorToInt());
        }

        public static Vector3Sign ToSign(this Vector3 a) 
        {
            return new Vector3Sign(a.x.ToSign(), a.y.ToSign(), a.z.ToSign());
        }
        public static Vector2Sign ToSign(this Vector2 a)
        {
            return new Vector2Sign(a.x.ToSign(), a.y.ToSign());
        }
        public static Vector3Sign ToSign(this UnityEngine.Vector3Int a)
        {
            return new Vector3Sign(a.x.ToSign(), a.y.ToSign(), a.z.ToSign());
        }
        public static Vector2Sign ToSign(this UnityEngine.Vector2Int a)
        {
            return new Vector2Sign(a.x.ToSign(), a.y.ToSign());
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
        public static Vector3 Rad2Deg(this Vector2 rad)
        {
            return new Vector3(rad.x * Mathf.Rad2Deg, rad.y * Mathf.Rad2Deg);
        }
        public static Vector3 Deg2Rad(this Vector3 deg)
        {
            return new Vector3(deg.x * Mathf.Deg2Rad, deg.y * Mathf.Deg2Rad, deg.z * Mathf.Deg2Rad);
        }
        public static Vector3 Deg2Rad(this Vector2 deg)
        {
            return new Vector3(deg.x * Mathf.Deg2Rad, deg.y * Mathf.Deg2Rad);
        }

        public static float[] ToArray(this Vector3 f)
        {
            return new float[]{
                f.x,
                f.y,
                f.z
            };
        }
        public static float[] ToArray(this Vector2 f)
        {
            return new float[]{
                f.x,
                f.y
            };
        }
        public static int[] ToArray(this Vector3Int f)
        {
            return new int[]{
                f.x,
                f.y,
                f.z
            };
        }
        public static int[] ToArray(this Vector2Int f)
        {
            return new int[]{
                f.x,
                f.y
            };
        }

        public static List<float> ToList(this Vector3 f)
        {
            return new List<float>{
                f.x,
                f.y,
                f.z
            };
        }
        public static List<float> ToList(this Vector2 f)
        {
            return new List<float>{
                f.x,
                f.y
            };
        }
        public static List<int> ToList(this Vector3Int f)
        {
            return new List<int>{
                f.x,
                f.y,
                f.z
            };
        }
        public static List<int> ToList(this Vector2Int f)
        {
            return new List<int>{
                f.x,
                f.y
            };
        }



        //QUATERNION EXTENSION METHODS
        public static Quaternion Add(this Quaternion a, Quaternion b, Space space = Space.Self)
        {
            if (space == Space.Self)
            {
                return a * b;
            }
            else if (space == Space.World)
            {
                return b * a;
            }

            return default;
        }
        public static Quaternion Subtract(this Quaternion a, Quaternion b, Space space = Space.Self)
        {
            if (space == Space.Self)
            {
                return a * Quaternion.Inverse(b);
            }
            else if (space == Space.World)
            {
                return Quaternion.Inverse(b) * a;
                //return (Quaternion.identity * Quaternion.Inverse(b)) * a;
                //return b * Quaternion.Inverse(a);
                //return Quaternion.Inverse(b) * Quaternion.Inverse(a);
                //return Quaternion.Inverse(Quaternion.Inverse(b) * Quaternion.Inverse(a));
                //return Quaternion.Inverse(b * a);
                //return Quaternion.Inverse(Quaternion.Inverse(b) * a);
                //return Quaternion.Inverse(b * Quaternion.Inverse(a));
            }

            return default;
        }


        //AXIS EXTENSION METHODS
        public static Axis Rotate(this Axis axis, UnityEngine.Vector3Int eulers, Space space = Space.Self)
        {
            Axis newAxis = axis;

            new Vector3Sign(Vector3Sign.zero.SetAxis(axis, TransformTools.Sign.Positive)).Rotate(eulers, space).Operate((s, a) =>
            {
                if (a != TransformTools.Sign.Neutral)
                {
                    newAxis = s;
                }

                return default;
            });

            return newAxis;
        }



        //public static UnityEngine.Vector3Int ToInt(this Quaternion euler, int divisions)
        //{
        //    return ((Vector3Int)euler.eulerAngles.Operate((s, a) =>
        //    {
        //        return Mathf.RoundToInt(a / 90f);
        //    })).UValue;
        //}
        //public static Quaternion ToQuaternion(this UnityEngine.Vector3Int euler, int divisions)
        //{

        //}
        //public static UnityEngine.Vector3Int ToInt(this Vector3 euler, int divisions)
        //{

        //}
        //public static Vector3 ToQuaternion(this UnityEngine.Vector3Int euler, int divisions)
        //{

        //}

        ///

        //SPECIAL

        //Get point on plane from raycast
        public static Vector3? RaycastPoint(this Plane plane, Ray ray, bool registerStart = false)
        {
            float distance;
            bool result = plane.Raycast(ray, out distance);

            if (registerStart && Vector3.Distance(ray.origin, plane.ClosestPointOnPlane(ray.origin)) <= Geometry.zeroRound)
            { //ray starts on plane
                return ray.origin;
            }
            else
            { //ray starts outside of plane
                if
                (
                    (distance < 0f) ||
                    (!result && distance == 0)
                )
                { // ray points away / perpendicular to plane
                    return null;
                }
                else
                { //ray points towards plane
                    return ray.GetPoint(distance);
                }
            }
        }
        public static bool RaycastPoint(this Plane plane, Ray ray, out Vector3 set, bool registerStart = false)
        {
            float distance;
            bool result = plane.Raycast(ray, out distance);

            if (registerStart && Vector3.Distance(ray.origin, plane.ClosestPointOnPlane(ray.origin)) <= Geometry.zeroRound)
            { //ray starts on plane
                set = ray.origin;
                return true;
            }
            else
            { //ray starts outside of plane
                if
                (
                    (distance < 0f) ||
                    (!result && distance == 0)
                )
                { // ray points away / perpendicular to plane
                    set = ray.GetPoint(distance);
                    return false;
                }
                else 
                { //ray points towards plane
                    set = ray.GetPoint(distance);
                    return true;
                }
            }
        }
        public static Vector3? LinecastPoint(this Plane plane, Ray ray, bool registerStart = true) {
            Vector3 pointA;
            Vector3 pointB;
            if (plane.RaycastPoint(ray, out pointA, registerStart))
            {
                return pointA;
            }
            else if (plane.RaycastPoint(new Ray(ray.origin, -ray.direction), out pointB, registerStart))
            {
                return pointB;
            }
            else 
            {
                return null;
            }
        }
        public static bool LinecastPoint(this Plane plane, Ray ray, out Vector3 set, bool registerStart = true)
        {
            Vector3 pointA;
            Vector3 pointB;
            if (plane.RaycastPoint(ray, out pointA, registerStart))
            {
                set = pointA;
                return true;
            }
            else if (plane.RaycastPoint(new Ray(ray.origin, -ray.direction), out pointB, registerStart))
            {
                set = pointB;
                return true;
            }

            set = Vector3.zero;
            return false;
        }


        //public static Vector3? OnPlane(Ray ray, Vector3 planePosition, Vector3 planeNormal)
        //{
        //    planeNormal = planeNormal.normalized;

        //    Plane plane = new Plane(planeNormal, planePosition);

        //    float distance;

        //    if (!(!plane.Raycast(ray, out distance) && distance == 0f))
        //    {
        //        return ray.GetPoint(distance);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        //public static Vector3? OnPlane(Vector3 point, Vector3 direction, Vector3 planePosition, Vector3 planeNormal)
        //{
        //    return OnPlane(new Ray(point, direction), planePosition, planeNormal);
        //}

        ///

        //returns a DIRECTION, 
        //used for top down movement with dynamic camera (only Y, Z rotation influence)
        //(X is always the same)
        public static Vector3 ProjectVector2(this Plane plane, Vector2 direction, Quaternion view, PlaneProjection projection = PlaneProjection.Reorient, bool backwardsFlip = false, Vector3 viewOrigin = default)
        {
            if (direction != Vector2.zero)
            {
                plane = new Plane(plane.normal, Vector3.zero);

                Vector3 planeDirection = default;
                if (projection == PlaneProjection.Reorient)
                { //reorients view flattely onto plane
                    Vector3 up = plane.ClosestPointOnPlane(view * Vector3.up).normalized;
                    if (up == Vector3.zero)
                    {
                        up = plane.ClosestPointOnPlane(view * Quaternion.Euler(Vector3.right) * Vector3.up).normalized;
                    }

                    planeDirection = (Quaternion.LookRotation(-plane.normal, up) * direction).normalized;
                }
                else if (projection == PlaneProjection.View)
                { //projects rays onto plane from view
                    //planeDirection = ((Vector3)plane.LinecastPoint(new Ray(Vector3.zero + view * direction, view * Vector3.forward)))/*.normalized*/;
                    plane.LinecastPoint(new Ray(Vector3.zero + view * direction, view * Vector3.forward), out planeDirection);
                }
                else if (projection == PlaneProjection.Plane)
                { //gets closest points on plane from view
                    planeDirection = plane.ClosestPointOnPlane(Vector3.zero + view * direction)/*.normalized*/;
                }

                if (backwardsFlip)
                {
                    Vector3 point = default;
                    if (!plane.RaycastPoint(new Ray(viewOrigin, view * Vector3.forward), out point, true))
                    {
                        planeDirection = -planeDirection;
                    }
                }

                return planeDirection;
            }
            else
            {
                return Vector3.zero;
            }
        }
        
        //Flattens ray direction onto plane
        public static Vector3 ProjectRay(this Plane plane, Ray ray)
        {
            float dot = RMath.CustomRound(Vector3.Dot(plane.normal.normalized, ray.direction.normalized), Geometry.zeroRound);
            if (dot > 0f || dot < 0f)
            {
                if (Mathf.Abs(dot) == 1f) 
                { //pointing in same / opposite direction
                    return Vector3.zero;
                } 
                else 
                { //point in different direction (STANDARD)
                    Vector3 raycastPoint;
                    plane.LinecastPoint(ray, out raycastPoint);
                    return (raycastPoint - plane.ClosestPointOnPlane(ray.origin));
                }
            }
            else if (dot == 0f) //perpendicular
            {
                return ray.GetPoint(1f) - ray.origin;
            }

            return default;
        }

        //Snaps direction on plane to intervals
        public static Vector3 SnapDirection(this Plane plane, Vector3 direction, Vector3 snapDirection, int count = 4)
        {
            plane = new Plane(plane.normal, Vector3.zero);

            count = (int)Mathf.Clamp(count, 1, Mathf.Infinity);
            snapDirection = plane.ClosestPointOnPlane(snapDirection).normalized;

            List<Vector3> snapDirections = new List<Vector3>();
            for (int i = 0; i < count; i++)
            {
                Quaternion newDirection = Quaternion.LookRotation(plane.normal, snapDirection);
                newDirection.eulerAngles = newDirection.eulerAngles.SetAxis(Axis.Z, newDirection.eulerAngles.z + ((360f / count) * i));

                snapDirections.Add(newDirection * Vector3.up);
            }

            direction = snapDirections.Aggregate((dir, newDir) => {
                if (Vector3.Angle(direction, newDir) < Vector3.Angle(direction, dir))
                {
                    return newDir;
                }
                else
                {
                    return dir;
                }
            });

            return direction;
        }
        public static Vector3 SnapDirection(this Plane plane, Vector3 direction, Vector3[] snapDirections)
        {
            plane = new Plane(plane.normal, Vector3.zero);

            for (int i = 0; i < snapDirections.Length; i++)
            {
                snapDirections[i] = plane.ClosestPointOnPlane(snapDirections[i]).normalized;
            }

            direction = snapDirections.Aggregate((dir, newDir) =>
            {
                if (Vector3.Angle(direction, newDir) < Vector3.Angle(direction, dir))
                {
                    return newDir;
                }
                else
                {
                    return dir;
                }
            });

            return direction;
        }

        //Checks if specified point(s) are seen by a camera
        public static bool InView(this Camera camera, Vector3 point)
        {
            Vector3 viewportPoint = camera.WorldToViewportPoint(point);

            if (
                viewportPoint.x >= 0f && viewportPoint.x <= 1f &&
                viewportPoint.y >= 0f && viewportPoint.y <= 1f &&
                viewportPoint.z >= 0f
                )
            {
                return true;
            } else
            {
                return false;
            }
        }
        public static bool InView(this Camera camera, Vector3[] points, bool fullyInView = false)
        {
            if (!fullyInView)
            {
                //If atleast one point is in view, set to true
                bool oneInView = false;

                foreach (Vector3 point in points)
                {
                    if (camera.InView(point))
                    {
                        oneInView = true;
                    }
                }

                return oneInView;
            } else if (fullyInView)
            {
                foreach (Vector3 point in points)
                {
                    if (!camera.InView(point))
                    {
                        return false;
                    }
                }
                return true;
            }

            return default;
        }
        public static bool InView(this Camera camera, List<Vector3> points, bool fullyInView = false)
        {
            return camera.InView(points.ToArray(), fullyInView);
        }

        //END
    }
}
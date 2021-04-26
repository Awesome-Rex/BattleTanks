using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        public static Dictionary<Axis, Vector2T<Axis>> axisPlanes = new Dictionary<Axis, Vector2T<Axis>>
        {
            { Axis.X, new Vector2T<Axis>(Axis.Z, Axis.Y) },
            { Axis.Y, new Vector2T<Axis>(Axis.X, Axis.Z) },
            { Axis.Z, new Vector2T<Axis>(Axis.X, Axis.Y) }
        };
        public static Dictionary<Vector3, Vector2T<Vector3>> axisPlaneDirections = new Dictionary<Vector3, Vector2T<Vector3>> {
            { axisDirections[Axis.X], new Vector2T<Vector3>(axisDirections[Axis.Z], axisDirections[Axis.Y]) },
            { axisDirections[Axis.Y], new Vector2T<Vector3>(axisDirections[Axis.X], axisDirections[Axis.Z]) },
            { axisDirections[Axis.Z], new Vector2T<Vector3>(axisDirections[Axis.X], axisDirections[Axis.Y]) }
        };

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

        
        public static Vector3T<Sign> Rotate(this Vector3T<Sign> f, UnityEngine.Vector3Int eulers)
        {
            return SignAdjacency<dynamic>.signPositions.First(x => x.Value == (Quaternion.Euler(((Vector3)eulers) * 90f) * Quaternion.LookRotation(SignAdjacency<dynamic>.signPositions[f])) * Vector3.forward * SignAdjacency<dynamic>.signPositions[f].magnitude).Key;
        }
        public static Vector3T<Sign> Mirror(this Vector3T<Sign> f, Axis axis)
        {
            return f.SetAxis(axis, f.GetAxis(axis).Negative());
        }


        //SPECIAL

        //plane raycasting
        public static Vector3? OnPlane(Ray ray, Vector3 planePosition, Vector3 planeNormal)
        {
            planeNormal = planeNormal.normalized;

            Plane plane = new Plane(planeNormal, planePosition);

            float distance;

            if (!(!plane.Raycast(ray, out distance) && distance == 0f))
            {
                return ray.GetPoint(distance);
            }
            else
            {
                return null;
            }
        }
        public static Vector3? OnPlane(Vector3 point, Vector3 direction, Vector3 planePosition, Vector3 planeNormal)
        {
            return OnPlane(new Ray(point, direction), planePosition, planeNormal);
        }

        public static Vector3? RaycastPoint (this Plane plane, Ray ray)
        {
            float distance;

            if (!(!plane.Raycast(ray, out distance) && distance == 0f))
            {
                return ray.GetPoint(distance);
            }
            else
            {
                return null;
            }
        }

        public static bool RaycastPoint(this Plane plane, Ray ray, out Vector3 set)
        {
            float distance;

            if (!(!plane.Raycast(ray, out distance) && distance == 0f))
            {
                set = ray.GetPoint(distance);
                return true;
            }
            else
            {
                set = Vector3.zero;
                return false;
            }
        }

        //returns a DIRECTION, 
        //used for top down movement with dynamic camera (only Y, Z rotation influence)
        //(X is always the same)
        public static Vector3 Vector2OntoPlane (Vector2 direction, Vector3 planeNormal, Quaternion view)
        {
            Plane plane = new Plane(planeNormal, Vector3.zero);
            
            Vector3 up = plane.ClosestPointOnPlane(view * Vector3.up).normalized;
            if (up == Vector3.zero)
            {
                up = plane.ClosestPointOnPlane(view * Quaternion.Euler(Vector3.right) * Vector3.up).normalized;
            }

            return Quaternion.LookRotation(-planeNormal, up) * direction;
        }
        public static Vector3 Vector2OntoPlane(Vector2 direction, Vector3 planeNormal, Quaternion view, Vector3 snapDirection, int count = 4)
        {
            Plane plane = new Plane(planeNormal, Vector3.zero);
            
            count = (int)Mathf.Clamp(count, 1, Mathf.Infinity);
            snapDirection = plane.ClosestPointOnPlane(snapDirection).normalized;
            
            Vector3 up = plane.ClosestPointOnPlane(view * Vector3.up).normalized;
            if (up == Vector3.zero)
            {
                up = plane.ClosestPointOnPlane(view * Quaternion.Euler(Vector3.right) * Vector3.up).normalized;
            }

            List<Vector3> snapDirections = new List<Vector3>();
            for (int i = 0; i < count; i++)
            {
                Quaternion newDirection = Quaternion.LookRotation(planeNormal, snapDirection);
                newDirection.eulerAngles = newDirection.eulerAngles.SetAxis(Axis.Z, newDirection.eulerAngles.z + ((360f / count) * i));

                snapDirections.Add(newDirection * Vector3.up);
            }

            up = snapDirections.Aggregate((dir, newDir) => {
                if (Vector3.Angle(up, newDir) < Vector3.Angle(up, dir))
                {
                    return newDir;
                } else
                {
                    return dir;
                }
            });
            
            return Quaternion.LookRotation(-planeNormal, up) * direction;
        }
        public static Vector3 Vector2OntoPlane(Vector2 direction, Vector3 planeNormal, Quaternion view, Vector3[] snapDirections)
        {
            Plane plane = new Plane(planeNormal, Vector3.zero);

            for (int i = 0; i < snapDirections.Length; i++)
            {
                snapDirections[i] = plane.ClosestPointOnPlane(snapDirections[i]).normalized;
            }

            Vector3 up = plane.ClosestPointOnPlane(view * Vector3.up).normalized;
            if (up == Vector3.zero)
            {
                up = plane.ClosestPointOnPlane(view * Quaternion.Euler(Vector3.right) * Vector3.up).normalized;
            }

            up = snapDirections.Aggregate((dir, newDir) => {
                if (Vector3.Angle(up, newDir) < Vector3.Angle(up, dir))
                {
                    return newDir;
                }
                else
                {
                    return dir;
                }
            });

            return Quaternion.LookRotation(-planeNormal, up) * direction;
        }

        //checks if specified point(s) are seen by a camera
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
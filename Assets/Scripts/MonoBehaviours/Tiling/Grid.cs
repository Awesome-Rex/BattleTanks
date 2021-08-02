using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

namespace REXTools.Tiling
{
    public enum TileRepeatMethod { Point, Outline }
    public enum TileAppearance { Border, Cross }

    [CreateAssetMenu(fileName = "New REX Grid", menuName = "REX/Tiling/Grid", order = 0)]
    public class Grid : ScriptableObject
    {
        public Vector3 size = Vector3.one;
        public float scale = 1f;

        //properties
        public Dictionary<Axis, TileOrigin> center = new Dictionary<Axis, TileOrigin>
        {
            {Axis.X, TileOrigin.Inside},
            {Axis.Y, TileOrigin.Inside},
            {Axis.Z, TileOrigin.Inside}
        };



        //properties
        public bool equal
        {
            get => size.x == size.y && size.x == size.z;
        }

        public Vector3Bool canPivot
        {
            get
            {
                return new Vector3Bool(
                    size.y == size.z,
                    size.x == size.z,
                    size.x == size.y
                );
            }
        }



        public Vector3 subdividedTileSize(int subdivisions)
        {
            //return size - (spacing * (subdivisions - 1));
            return size / subdivisions;
        }
        public Vector3 subdividedTileSizeRaw(float subdivisions)
        {
            return size / subdivisions;
        }

        //SPACE CONVERSION

        public static Vector3 GridToOne(Vector3 position, int subdivisions = 1)
        { //multiple subdivisions to one subdivision
            subdivisions = RMath.ClampMin(subdivisions, 1);

            if (RMath.Odd(subdivisions))
            {
                return position / subdivisions;
            }
            else
            {
                return (position / subdivisions) + (Vector3.one * (-0.25f / subdivisions * 2f));
            }
        }
        public static Vector3 OneToGrid(Vector3 position, int subdivisions = 1)
        { //one subdivision to multiple subdivisions
            subdivisions = RMath.ClampMin(subdivisions, 1);

            if (RMath.Odd(subdivisions))
            {
                return position * subdivisions;
            }
            else
            {
                return (position + (Vector3.one * (0.25f / subdivisions * 2f))) * subdivisions;
            }
        }
        public static Vector3 GridToGrid(Vector3 position, int subdivisions = 1, int newSubdivisions = 1)
        {
            //converts 
            //Position => One
            //One => Position

            subdivisions = RMath.ClampMin(subdivisions, 1);
            newSubdivisions = RMath.ClampMin(newSubdivisions, 1);

            return OneToGrid(GridToOne(position, subdivisions), newSubdivisions);
        }


        //DISTANCE CONVERSION
        public static float GridToOneDistance(float distance, int subdivisions = 1)
        {
            return distance / subdivisions;
        }
        public static float OneToGridDistance(float distance, int subdivisions = 1)
        {
            return distance * subdivisions;
        }
        public static float GridToGridDistance(float distance, int subdivisions = 1, int newSubdivisions = 1)
        {
            return OneToGridDistance(GridToOneDistance(distance, subdivisions), newSubdivisions);
        }


        public static Ray GridToOneRay(Ray ray, int subdivisions = 1)
        {
            return new Ray(GridToOne(ray.origin, subdivisions), ray.direction);
        }
        public static Ray OneToGridRay(Ray ray, int subdivisions = 1)
        {
            return new Ray(OneToGrid(ray.origin, subdivisions), ray.direction);
        }
        public static Ray GridToGridRay(Ray ray, int subdivisions = 1, int newSubdivisions = 1)
        {
            return OneToGridRay(GridToOneRay(ray, subdivisions), subdivisions);
        }




        //GRID RAYCASTING
        //*distance in one's unit
        //*ray in subdivision's unit
        public static GridCastHit TileCast(Ray ray, Axis axis, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            float pointDirection = Mathf.Sign(ray.direction.GetAxis(axis));
            Vector3 planeOrigin = Vectors.axisDirections[axis] * (pointDirection == 1 ?
                RMath.CustomCeil(ray.origin.GetAxis(axis), 1f, 0f) :
                RMath.CustomFloor(ray.origin.GetAxis(axis), 1f, 0f));

            //implements rounding to tile centers
            Plane plane = new Plane(Vectors.axisDirections[axis] * pointDirection, planeOrigin);

            //++++++++++++++++++++GridCastHit uses ONE's unit (not in context of subdivisions)
            float rayDistance;
            if (plane.Raycast(ray, out rayDistance) && GridToOneDistance(rayDistance, subdivisions) <= maxDistance)
            {
                rayDistance = Mathf.Abs(rayDistance);

                GridCastHit hit = new GridCastHit();

                hit.point = GridToOne(ray.GetPoint(rayDistance), subdivisions);
                hit.distance = GridToOneDistance(rayDistance, subdivisions); //modified

                return hit;
            }

            return null;
        }
        public static List<GridCastHit> TileCastAll(Ray ray, Axis axis, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            List<GridCastHit> hitInfo = new List<GridCastHit>();


            //Creates range between 2 points along axis where planes can be made
            Vector3 pointA = ray.origin;
            //distance in one's unit
            Vector3 pointB = OneToGrid(OneToGrid(GridToOneRay(ray, subdivisions).GetPoint(maxDistance), subdivisions), subdivisions);
            float pointDirection = Mathf.Sign(ray.direction.GetAxis(axis));


            //implements rounding to tile centers
            for (
                float i = pointDirection == 1 ? RMath.CustomCeil(pointA.GetAxis(axis), 1f, 0f) : RMath.CustomFloor(pointA.GetAxis(axis), 1f, 0f);
                pointDirection == 1 ? i < RMath.CustomFloor(pointB.GetAxis(axis), 1f, 0f) : i > RMath.CustomCeil(pointB.GetAxis(axis), 1f, 0f);
                i += pointDirection
            )
            {
                Plane plane = new Plane(Vectors.axisDirections[axis] * pointDirection, Vectors.axisDirections[axis] * i);

                //++++++++++++++++++++GridCastHit uses ONE's unit (not in context of subdivisions)
                float rayDistance;
                if (plane.Raycast(ray, out rayDistance))
                {
                    rayDistance = Mathf.Abs(rayDistance);

                    GridCastHit hit = new GridCastHit();

                    hit.point = GridToOne(ray.GetPoint(rayDistance), subdivisions);
                    hit.distance = GridToOneDistance(rayDistance, subdivisions);

                    hitInfo.Add(hit);
                }
            }

            if (hitInfo.Count >= 1)
            {
                return hitInfo;
            }
            else
            {
                return null;
            }
        }

        public static Vector3T<GridCastHit> TileCastAxis(Ray ray, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            Vector3T<GridCastHit> hitInfo = null;

            foreach (Axis axis in Vectors.axisIterate)
            {
                hitInfo = hitInfo.SetAxis(axis, TileCast(ray, axis, maxDistance, subdivisions/*, AxisMask*/));
            }

            return hitInfo;
        }
        public static Vector3T<List<GridCastHit>> TileCastAxisAll(Ray ray, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            Vector3T<List<GridCastHit>> hitInfo = null;

            foreach (Axis axis in Vectors.axisIterate)
            {
                hitInfo = hitInfo.SetAxis(axis, TileCastAll(ray, axis, maxDistance, subdivisions/*, AxisMask*/));
            }

            return hitInfo;
        }


        public static GridCastHit EdgeCast(Ray ray, Axis axis, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            float pointDirection = Mathf.Sign(ray.direction.GetAxis(axis));
            Vector3 planeOrigin = Vectors.axisDirections[axis] * (pointDirection == 1 ?
                RMath.CustomCeil(ray.origin.GetAxis(axis), 1f, 0.5f) :
                RMath.CustomFloor(ray.origin.GetAxis(axis), 1f, 0.5f));

            //implements rounding to tile centers
            Plane plane = new Plane(Vectors.axisDirections[axis] * pointDirection, planeOrigin);

            //++++++++++++++++++++GridCastHit uses ONE's unit (not in context of subdivisions)
            float rayDistance;
            if (plane.Raycast(ray, out rayDistance) && GridToOneDistance(rayDistance, subdivisions) <= maxDistance)
            {
                rayDistance = Mathf.Abs(rayDistance);

                GridCastHit hit = new GridCastHit();

                hit.point = GridToOne(ray.GetPoint(rayDistance), subdivisions);
                hit.distance = GridToOneDistance(rayDistance, subdivisions); //modified

                return hit;
            }

            return null;
        }
        public static List<GridCastHit> EdgeCastAll(Ray ray, Axis axis, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            List<GridCastHit> hitInfo = new List<GridCastHit>();


            //Creates range between 2 points along axis where planes can be made
            Vector3 pointA = ray.origin;
            //distance in one's unit
            Vector3 pointB = OneToGrid(OneToGrid(GridToOneRay(ray, subdivisions).GetPoint(maxDistance), subdivisions), subdivisions);
            float pointDirection = Mathf.Sign(ray.direction.GetAxis(axis));


            //implements rounding to tile centers
            for (
                float i = pointDirection == 1 ? RMath.CustomCeil(pointA.GetAxis(axis), 1f, 0.5f) : RMath.CustomFloor(pointA.GetAxis(axis), 1f, 0.5f);
                pointDirection == 1 ? i < RMath.CustomFloor(pointB.GetAxis(axis), 1f, 0.5f) : i > RMath.CustomCeil(pointB.GetAxis(axis), 1f, 0.5f);
                i += pointDirection
            )
            {
                Plane plane = new Plane(Vectors.axisDirections[axis] * pointDirection, Vectors.axisDirections[axis] * i);

                //++++++++++++++++++++GridCastHit uses ONE's unit (not in context of subdivisions)
                float rayDistance;
                if (plane.Raycast(ray, out rayDistance))
                {
                    rayDistance = Mathf.Abs(rayDistance);

                    GridCastHit hit = new GridCastHit();

                    hit.point = GridToOne(ray.GetPoint(rayDistance), subdivisions);
                    hit.distance = GridToOneDistance(rayDistance, subdivisions);

                    hitInfo.Add(hit);
                }
            }

            if (hitInfo.Count >= 1)
            {
                return hitInfo;
            }
            else
            {
                return null;
            }
        }

        public static Vector3T<GridCastHit> EdgeCastAxis(Ray ray, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            Vector3T<GridCastHit> hitInfo = null;

            foreach (Axis axis in Vectors.axisIterate)
            {
                hitInfo = hitInfo.SetAxis(axis, TileCast(ray, axis, maxDistance, subdivisions/*, AxisMask*/));
            }

            return hitInfo;
        }
        public static Vector3T<List<GridCastHit>> EdgeCastAxisAll(Ray ray, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            Vector3T<List<GridCastHit>> hitInfo = null;

            foreach (Axis axis in Vectors.axisIterate)
            {
                hitInfo = hitInfo.SetAxis(axis, TileCastAll(ray, axis, maxDistance, subdivisions/*, AxisMask*/));
            }

            return hitInfo;
        }


        //GRID TILE INTERSECTIONS (collisions)
        #region "Intersection"
        public bool Intersects(
            UnityEngine.Vector3Int a, int subdivisionsA,
            UnityEngine.Vector3Int b, int subdivisionsB,

            bool trim = false
        )
        {
            Bounds tileA = new Bounds(GridToOne(a, subdivisionsA), (1f / subdivisionsA) * Vector3.one);
            Bounds tileB = new Bounds(GridToOne(b, subdivisionsB), (1f / subdivisionsB) * Vector3.one);

            return tileA.Intersects(tileB, trim);
        }
        public bool IntersectsRaw(
            Vector3 a, int subdivisionsA, 
            Vector3 b, int subdivisionsB, 
            
            bool trim = false, bool allowance = false
        )
        {
            Bounds tileA = new Bounds(GridToOne(a, subdivisionsA), (1f / subdivisionsA) * Vector3.one);
            Bounds tileB = new Bounds(GridToOne(b, subdivisionsB), (1f / subdivisionsB) * Vector3.one);

            return tileA.Intersects(tileB, trim, allowance);
        }
        public bool IntersectsRaw(
            Vector3 positionA, Vector3 sizeA, int subdivisionsA,
            Vector3 positionB, Vector3 sizeB, int subdivisionsB,

            bool trim = false, bool allowance = false
        )
        {
            Bounds tileA = new Bounds(GridToOne(positionA, subdivisionsA), (1f / subdivisionsA) * sizeA);
            Bounds tileB = new Bounds(GridToOne(positionB, subdivisionsB), (1f / subdivisionsB) * sizeB);

            return tileA.Intersects(tileB, trim, allowance);
        }
        public bool IntersectsRaw(
            Vector3 positionA, Vector3 sizeA, Quaternion rotationA, int subdivisionsA,
            Vector3 positionB, Vector3 sizeB, Quaternion rotationB, int subdivisionsB,

            bool trim = false, bool allowance = true
        )
        {
            Box tileA = new Box(GridToOne(positionA, subdivisionsA), rotationA, (1f / subdivisionsA) * sizeA);
            Box tileB = new Box(GridToOne(positionB, subdivisionsB), rotationB, (1f / subdivisionsB) * sizeB);

            return tileA.Intersects(tileB, trim, allowance);
        }



        public bool Intersects(
            UnityEngine.Vector3Int a, int subdivisionsA,
            UnityEngine.Vector3Int b, int subdivisionsB,

            out BoxIntersectHit hit,

            bool trim = false
        )
        {
            Bounds tileA = new Bounds(GridToOne(a, subdivisionsA), (1f / subdivisionsA) * Vector3.one);
            Bounds tileB = new Bounds(GridToOne(b, subdivisionsB), (1f / subdivisionsB) * Vector3.one);

            return tileA.Intersects(tileB, out hit, trim);
        }
        public bool IntersectsRaw(
            Vector3 a, int subdivisionsA,
            Vector3 b, int subdivisionsB,

            out BoxIntersectHit hit,

            bool trim = false, bool allowance = false
        )
        {
            Bounds tileA = new Bounds(GridToOne(a, subdivisionsA), (1f / subdivisionsA) * Vector3.one);
            Bounds tileB = new Bounds(GridToOne(b, subdivisionsB), (1f / subdivisionsB) * Vector3.one);

            return tileA.Intersects(tileB, out hit, trim, allowance);
        }
        public bool IntersectsRaw(
            Vector3 positionA, Vector3 sizeA, int subdivisionsA,
            Vector3 positionB, Vector3 sizeB, int subdivisionsB,

            out BoxIntersectHit hit,

            bool trim = false, bool allowance = false
        )
        {
            Bounds tileA = new Bounds(GridToOne(positionA, subdivisionsA), (1f / subdivisionsA) * sizeA);
            Bounds tileB = new Bounds(GridToOne(positionB, subdivisionsB), (1f / subdivisionsB) * sizeB);

            return tileA.Intersects(tileB, out hit, trim, allowance);
        }
        public bool IntersectsRaw(
            Vector3 positionA, Vector3 sizeA, Quaternion rotationA, int subdivisionsA,
            Vector3 positionB, Vector3 sizeB, Quaternion rotationB, int subdivisionsB,

            out BoxIntersectHit hit,

            bool trim = false, bool allowance = true
        )
        {
            Box tileA = new Box(GridToOne(positionA, subdivisionsA), rotationA, (1f / subdivisionsA) * sizeA);
            Box tileB = new Box(GridToOne(positionB, subdivisionsB), rotationB, (1f / subdivisionsB) * sizeB);

            return tileA.Intersects(tileB, out hit, trim, allowance);
        }
        #endregion
        #region "Adjacency"
        public bool Adjacent(
            UnityEngine.Vector3Int a, int subdivisionsA,
            UnityEngine.Vector3Int b, int subdivisionsB
        )
        {
            Bounds tileA = new Bounds(GridToOne(a, subdivisionsA), (1f / subdivisionsA) * Vector3.one);
            Bounds tileB = new Bounds(GridToOne(b, subdivisionsB), (1f / subdivisionsB) * Vector3.one);

            return tileA.Adjacent(tileB);
        }
        public bool AdjacentRaw(
            Vector3 a, int subdivisionsA,
            Vector3 b, int subdivisionsB,

            bool allowance = false
        )
        {
            Bounds tileA = new Bounds(GridToOne(a, subdivisionsA), (1f / subdivisionsA) * Vector3.one);
            Bounds tileB = new Bounds(GridToOne(b, subdivisionsB), (1f / subdivisionsB) * Vector3.one);

            return tileA.Adjacent(tileB, allowance);
        }
        public bool AdjacentRaw(
            Vector3 positionA, Vector3 sizeA, int subdivisionsA,
            Vector3 positionB, Vector3 sizeB, int subdivisionsB,

            bool allowance = false
        )
        {
            Bounds tileA = new Bounds(GridToOne(positionA, subdivisionsA), (1f / subdivisionsA) * sizeA);
            Bounds tileB = new Bounds(GridToOne(positionB, subdivisionsB), (1f / subdivisionsB) * sizeB);

            return tileA.Adjacent(tileB, allowance);
        }
        public bool AdjacentRaw(
            Vector3 positionA, Vector3 sizeA, Quaternion rotationA, int subdivisionsA,
            Vector3 positionB, Vector3 sizeB, Quaternion rotationB, int subdivisionsB,

            bool allowance = true
        )
        {
            Box tileA = new Box(GridToOne(positionA, subdivisionsA), rotationA, (1f / subdivisionsA) * sizeA);
            Box tileB = new Box(GridToOne(positionB, subdivisionsB), rotationB, (1f / subdivisionsB) * sizeB);

            return tileA.Adjacent(tileB, allowance);
        }



        public bool Adjacent(
            UnityEngine.Vector3Int a, int subdivisionsA,
            UnityEngine.Vector3Int b, int subdivisionsB,

            out BoxAdjacentHit hit
        )
        {
            Bounds tileA = new Bounds(GridToOne(a, subdivisionsA), (1f / subdivisionsA) * Vector3.one);
            Bounds tileB = new Bounds(GridToOne(b, subdivisionsB), (1f / subdivisionsB) * Vector3.one);

            return tileA.Adjacent(tileB, out hit);
        }
        public bool AdjacentRaw(
            Vector3 a, int subdivisionsA,
            Vector3 b, int subdivisionsB,

            out BoxAdjacentHit hit,

            bool allowance = false
        )
        {
            Bounds tileA = new Bounds(GridToOne(a, subdivisionsA), (1f / subdivisionsA) * Vector3.one);
            Bounds tileB = new Bounds(GridToOne(b, subdivisionsB), (1f / subdivisionsB) * Vector3.one);

            return tileA.Adjacent(tileB, out hit, allowance);
        }
        public bool AdjacentRaw(
            Vector3 positionA, Vector3 sizeA, int subdivisionsA,
            Vector3 positionB, Vector3 sizeB, int subdivisionsB,

            out BoxAdjacentHit hit,

            bool allowance = false
        )
        {
            Bounds tileA = new Bounds(GridToOne(positionA, subdivisionsA), (1f / subdivisionsA) * sizeA);
            Bounds tileB = new Bounds(GridToOne(positionB, subdivisionsB), (1f / subdivisionsB) * sizeB);

            return tileA.Adjacent(tileB, out hit, allowance);
        }
        public bool AdjacentRaw(
            Vector3 positionA, Vector3 sizeA, Quaternion rotationA, int subdivisionsA,
            Vector3 positionB, Vector3 sizeB, Quaternion rotationB, int subdivisionsB,

            out BoxAdjacentHit hit,

            bool allowance = true
        )
        {
            Box tileA = new Box(GridToOne(positionA, subdivisionsA), rotationA, (1f / subdivisionsA) * sizeA);
            Box tileB = new Box(GridToOne(positionB, subdivisionsB), rotationB, (1f / subdivisionsB) * sizeB);

            return tileA.Adjacent(tileB, out hit, allowance);
        }
        #endregion
        #region "Containment"
        public bool Contains(
            UnityEngine.Vector3Int a, int subdivisionsA,
            UnityEngine.Vector3Int b, int subdivisionsB,

            bool trim = false
        )
        {
            Bounds tileA = new Bounds(GridToOne(a, subdivisionsA), (1f / subdivisionsA) * Vector3.one);
            Bounds tileB = new Bounds(GridToOne(b, subdivisionsB), (1f / subdivisionsB) * Vector3.one);

            return tileA.Contains(tileB, trim);
        }
        public bool ContainsRaw(
            Vector3 a, int subdivisionsA,
            Vector3 b, int subdivisionsB,

            bool trim = false, bool allowance = false
        )
        {
            Bounds tileA = new Bounds(GridToOne(a, subdivisionsA), (1f / subdivisionsA) * Vector3.one);
            Bounds tileB = new Bounds(GridToOne(b, subdivisionsB), (1f / subdivisionsB) * Vector3.one);

            return tileA.Contains(tileB, trim, allowance);
        }
        public bool ContainsRaw(
            Vector3 positionA, Vector3 sizeA, int subdivisionsA,
            Vector3 positionB, Vector3 sizeB, int subdivisionsB,

            bool trim = false, bool allowance = false
        )
        {
            Bounds tileA = new Bounds(GridToOne(positionA, subdivisionsA), (1f / subdivisionsA) * sizeA);
            Bounds tileB = new Bounds(GridToOne(positionB, subdivisionsB), (1f / subdivisionsB) * sizeB);

            return tileA.Contains(tileB, trim, allowance);
        }
        public bool ContainsRaw(
            Vector3 positionA, Vector3 sizeA, Quaternion rotationA, int subdivisionsA,
            Vector3 positionB, Vector3 sizeB, Quaternion rotationB, int subdivisionsB,

            bool trim = false, bool allowance = true
        )
        {
            Box tileA = new Box(GridToOne(positionA, subdivisionsA), rotationA, (1f / subdivisionsA) * sizeA);
            Box tileB = new Box(GridToOne(positionB, subdivisionsB), rotationB, (1f / subdivisionsB) * sizeB);

            return tileA.Contains(tileB, trim, allowance);
        }



        public bool Contains(
            UnityEngine.Vector3Int a, int subdivisionsA,
            UnityEngine.Vector3Int b, int subdivisionsB,

            out BoxContainHit hit,

            bool trim = false
        )
        {
            Bounds tileA = new Bounds(GridToOne(a, subdivisionsA), (1f / subdivisionsA) * Vector3.one);
            Bounds tileB = new Bounds(GridToOne(b, subdivisionsB), (1f / subdivisionsB) * Vector3.one);

            return tileA.Contains(tileB, out hit, trim);
        }
        public bool ContainsRaw(
            Vector3 a, int subdivisionsA,
            Vector3 b, int subdivisionsB,

            out BoxContainHit hit,

            bool trim = false, bool allowance = false
        )
        {
            Bounds tileA = new Bounds(GridToOne(a, subdivisionsA), (1f / subdivisionsA) * Vector3.one);
            Bounds tileB = new Bounds(GridToOne(b, subdivisionsB), (1f / subdivisionsB) * Vector3.one);

            return tileA.Contains(tileB, out hit, trim, allowance);
        }
        public bool ContainsRaw(
            Vector3 positionA, Vector3 sizeA, int subdivisionsA,
            Vector3 positionB, Vector3 sizeB, int subdivisionsB,

            out BoxContainHit hit,

            bool trim = false, bool allowance = false
        )
        {
            Bounds tileA = new Bounds(GridToOne(positionA, subdivisionsA), (1f / subdivisionsA) * sizeA);
            Bounds tileB = new Bounds(GridToOne(positionB, subdivisionsB), (1f / subdivisionsB) * sizeB);

            return tileA.Contains(tileB, out hit, trim, allowance);
        }
        public bool ContainsRaw(
            Vector3 positionA, Vector3 sizeA, Quaternion rotationA, int subdivisionsA,
            Vector3 positionB, Vector3 sizeB, Quaternion rotationB, int subdivisionsB,

            out BoxContainHit hit,

            bool trim = false, bool allowance = true
        )
        {
            Box tileA = new Box(GridToOne(positionA, subdivisionsA), rotationA, (1f / subdivisionsA) * sizeA);
            Box tileB = new Box(GridToOne(positionB, subdivisionsB), rotationB, (1f / subdivisionsB) * sizeB);

            return tileA.Contains(tileB, out hit, trim, allowance);
        }
        #endregion



        //Static methods
        public static Vector3 SnapPosition(Vector3 position, int subdivisions = 1, int snapSubdivisions = 1)
        { //returns subdivided position
            return GridToGrid(position, subdivisions, snapSubdivisions).Round();
        }
        public static UnityEngine.Vector3Int SnapPositionToInt(Vector3 position, int subdivisions = 1, int snapSubdivisions = 1)
        { //returns subdivided position
            return GridToGrid(position, subdivisions, snapSubdivisions).RoundToInt();
        }

        public static Vector3 SnapPositionSubdivided(Vector3 position, int subdivisions = 1, int snapSubdivisions = 1)
        { //returns tile position without subdivisions
            return (SnapPosition((position * (1f / subdivisions)) * snapSubdivisions) * (1f / snapSubdivisions)) * subdivisions;
        }

        public static Quaternion SnapRotation(Quaternion rotation, Vector3Bool canPivot) //takes grid rotation, returns grid rotation
        {
            return Quaternion.Euler((new Vector3Float(rotation.eulerAngles.Divide(Vector3.one * 90f).Round().Operate(canPivot, (s, a, b) =>
            {
                if (b)
                {
                    return a.CustomRound(2);
                }
                else
                {
                    return a;
                }
            })).UValue.Round().Multiply(Vector3.one * 90)));
        }
        public static Quaternion SnapRotation(Quaternion rotation) //takes grid rotation, returns grid rotation
        {
            return SnapRotation(rotation, new Vector3Bool(true, true, true));
        }
        public static UnityEngine.Vector3Int SnapRotationToInt(Quaternion rotation, Vector3Bool canPivot) //takes grid rotation, returns grid rotation
        {
            return (new TransformTools.Vector3Int(rotation.eulerAngles.Divide(Vector3.one * 90f).RoundToInt().Operate(canPivot, (s, a, b) =>
            {
                if (b)
                {
                    return a.CustomRound(2);
                }
                else
                {
                    return a;
                }
            }))).UValue;
        }
        public static UnityEngine.Vector3Int SnapRotationToInt(Quaternion rotation) //takes grid rotation, returns grid rotation
        {
            return SnapRotationToInt(rotation, new Vector3Bool(true, true, true));
        }

        public static int SnapSubdivisions(float subdivisions) //takes grid subdivisions, returns grid subdivisions
        {
            return Mathf.RoundToInt(subdivisions);
        }


        public static UnityEngine.Vector3Int PointArea(Vector3 point, int subdivisions = 1, int areaSubdivisions = 1)
        { //returns subdivided tile position
            return SnapPositionToInt(GridToGrid(point, subdivisions, areaSubdivisions));
        }
        public static BoundsInt BoundsArea(Vector3 min, Vector3 max, int subdivisions = 1, int areaSubdivisions = 1)
        { //returns subdivided bounds 
            BoundsInt newBounds = new BoundsInt();
            newBounds.SetMinMax(SnapPositionToInt(GridToGrid(min, subdivisions, areaSubdivisions)), SnapPositionToInt(GridToGrid(max, subdivisions, areaSubdivisions)));
            newBounds.size = newBounds.size.Abs();

            return newBounds;
        }
    }
}
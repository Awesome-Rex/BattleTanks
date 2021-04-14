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


        
        public Vector3 subdividedTileSize (int subdivisions)
        {
            //return size - (spacing * (subdivisions - 1));
            return size / subdivisions;
        }

        //SPACE CONVERSION
        
        public Vector3 GridToOne(Vector3 position, int subdivisions = 1)
        { //multiple subdivisions to one subdivision
            subdivisions = RMath.ClampMin(subdivisions, 1);

            if (RMath.Odd(subdivisions))
            {
                return position / subdivisions;
            } 
            else 
            {
                return (position / subdivisions) + (Vector3.one * (-0.25f) );
            }
        }
        public Vector3 OneToGrid(Vector3 position, int subdivisions = 1)
        { //one subdivision to multiple subdivisions
            subdivisions = RMath.ClampMin(subdivisions, 1);

            if (RMath.Odd(subdivisions)) {
                return position * subdivisions;
            } 
            else
            {
                return (position + (Vector3.one * 0.25f)) * subdivisions;
            }
        }
        public Vector3 GridToGrid(Vector3 position, int subdivisions = 1, int newSubdivisions = 1)
        {
            //converts 
            //Position => One
            //One => Position

            subdivisions = RMath.ClampMin(subdivisions, 1);
            newSubdivisions = RMath.ClampMin(newSubdivisions, 1);

            return OneToGrid(GridToOne(position, subdivisions), newSubdivisions);
        }


        //DISTANCE CONVERSION
        public float GridToOneDistance(float distance, int subdivisions = 1)
        {
            return distance / subdivisions;
        }
        public float OneToGridDistance(float distance, int subdivisions = 1)
        {
            return distance * subdivisions;
        }
        public float GridToGridDistance(float distance, int subdivisions = 1, int newSubdivisions = 1)
        {
            return OneToGridDistance(GridToOneDistance(distance, subdivisions), newSubdivisions);
        }


        public Ray GridToOneRay(Ray ray, int subdivisions = 1)
        {
            return new Ray(GridToOne(ray.origin, subdivisions), ray.direction);
        }
        public Ray OneToGridRay(Ray ray, int subdivisions = 1)
        {
            return new Ray(OneToGrid(ray.origin, subdivisions), ray.direction);
        }
        public Ray GridToGridRay(Ray ray, int subdivisions = 1, int newSubdivisions = 1)
        {
            return OneToGridRay(GridToOneRay(ray, subdivisions), subdivisions);
        }




        //GRID RAYCASTING
        //*distance in one's unit
        //*ray in subdivision's unit
        public GridCastHit TileCast(Ray ray, Axis axis, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
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
        public List<GridCastHit> TileCastAll(Ray ray, Axis axis, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
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

        public Vector3T<GridCastHit> TileCastAxis(Ray ray, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            Vector3T<GridCastHit> hitInfo = null;

            foreach (Axis axis in Vectors.axisIterate)
            {
                hitInfo = hitInfo.SetAxis(axis, TileCast(ray, axis, maxDistance, subdivisions/*, AxisMask*/));
            }

            return hitInfo;
        }
        public Vector3T<List<GridCastHit>> TileCastAxisAll(Ray ray, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            Vector3T<List<GridCastHit>> hitInfo = null;

            foreach (Axis axis in Vectors.axisIterate)
            {
                hitInfo = hitInfo.SetAxis(axis, TileCastAll(ray, axis, maxDistance, subdivisions/*, AxisMask*/));
            }

            return hitInfo;
        }


        public GridCastHit EdgeCast(Ray ray, Axis axis, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
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
        public List<GridCastHit> EdgeCastAll(Ray ray, Axis axis, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
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

        public Vector3T<GridCastHit> EdgeCastAxis(Ray ray, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            Vector3T<GridCastHit> hitInfo = null;

            foreach (Axis axis in Vectors.axisIterate)
            {
                hitInfo = hitInfo.SetAxis(axis, TileCast(ray, axis, maxDistance, subdivisions/*, AxisMask*/));
            }

            return hitInfo;
        }
        public Vector3T<List<GridCastHit>> EdgeCastAxisAll(Ray ray, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            Vector3T<List<GridCastHit>> hitInfo = null;

            foreach (Axis axis in Vectors.axisIterate)
            {
                hitInfo = hitInfo.SetAxis(axis, TileCastAll(ray, axis, maxDistance, subdivisions/*, AxisMask*/));
            }

            return hitInfo;
        }
    }
}
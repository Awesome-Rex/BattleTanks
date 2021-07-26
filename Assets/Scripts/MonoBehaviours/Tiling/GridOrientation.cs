using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

using REXTools.TransformTools;

namespace REXTools.Tiling
{
    //grid implemented in scene - in world space
    public class GridOrientation : MonoBehaviour
    {
        //Non static methods
        //grid size
        public Grid grid;

        //orientation (for world or self)
        public Vector3 offsetCenterPoint = Vector3.zero;
        [Space]
        public AxisOrder offsetPosition;
        public AxisOrder offsetRotation;
        public float offsetScale = 1f;
        
        //space
        public Space space = Space.Self;

        public AverageType scaleConclusion = AverageType.Mean;

        [Space]
        [Header("Scene View Settings")]
        public bool showGridGizmos = true;
        public bool showOnlyWhenSelected = true;

        public Vector3Bool showAxis = new Vector3Bool(true, true, true);

        public float subdivisionRenderDistance = 20f;

        [Space]
        public Vector3T<List<GridPlaneOrientation>> gridOrientations = new Vector3T<List<GridPlaneOrientation>>();

        //private
        private Dictionary<Axis, Color> axisColours = new Dictionary<Axis, Color>
        {
            //{Axis.X, new Color(0.8588235f, 0.2431373f, 0.1137255f, 0.5f)},
            {Axis.X, new Color(237f / 255f, 16f / 255f, 0f, 0.5f) },
            {Axis.Y, new Color(0.6039216f, 0.9529412f, 0.282353f, 0.5f)},
            {Axis.Z, new Color(0.227451f, 0.4784314f, 0.972549f, 0.5f)}
        };

        //properties
        public Vector3 tileSize
        {
            get
            {
                return grid.size * totalScale;
            }
        }

        public Vector3 finalPosition
        {
            get
            {
                if (space == Space.Self)
                {
                    return offsetPosition.ApplyPosition(transform.position, transform.rotation, Vector3.one * totalScale, transform.position);
                }
                else
                {
                    return offsetPosition.ApplyPosition(Vector3.zero, Quaternion.Euler(Vector3.zero), Vector3.one);
                }
            }
        }
        public Quaternion finalRotation
        {
            get
            {
                if (space == Space.Self)
                {
                    return offsetRotation.ApplyRotation(transform);
                }
                else
                {
                    return offsetRotation.ApplyRotation(Quaternion.Euler(Vector3.zero));
                }
            }
        }

        public Vector3 initialPosition(Vector3 current)
        {
            if (space == Space.Self)
            {
                return offsetPosition.ReversePosition(transform.position, transform.rotation, Vector3.one * totalScale, current);
            }
            else
            {
                return offsetPosition.ReversePosition(Vector3.zero, Quaternion.Euler(Vector3.zero), current);
            }
        }
        public Quaternion initialRotation(Quaternion current)
        {
            if (space == Space.Self)
            {
                return offsetRotation.ReverseRotation(transform, current);
            }
            else
            {
                return offsetRotation.ReverseRotation(Quaternion.Euler(Vector3.zero), current);
            }
        }

        public float totalScale
        {
            get
            {
                if (space == Space.Self)
                {
                    return transform.localScale.ToList().GetAverage(scaleConclusion) * offsetScale * grid.scale;
                }
                else
                {
                    return 1f * offsetScale * grid.scale;
                }
            }
        }

        public Vector3 subdividedTileSize(int subdivisions)
        {
            return grid.subdividedTileSize(subdivisions);
        }

        //directions
        public Vector3 up
        {
            get
            {
                return finalRotation * Vector3.up;
            }
        }
        public Vector3 forward
        {
            get
            {
                return finalRotation * Vector3.forward;
            }
        }
        public Vector3 right
        {
            get
            {
                return finalRotation * Vector3.right;
            }
        }

        public Vector3 x
        {
            get => right;
        }
        public Vector3 y
        {
            get => up;
        }
        public Vector3 z
        {
            get => forward;
        }



        //TILES

        ////creates new tile
        //public TileState CreateTile(Tile tile)
        //{
        //    GameObject tileInstance = Instantiate(tile.prefab);
        //    TileState tileState = tileInstance.AddComponent<TileState>();
        //    tileState.tile = tile;
        //    tileState.grid = this;

        //    return tileState;
        //}
        //public TileState CreateTile(Tile tile, Vector3 position, int subdivisions = 1, UnityEngine.Vector3Int rotation = default)
        //{
        //    TileState tileInstance = CreateTile(tile);

        //    tileInstance.subdivisions = subdivisions;
        //    tileInstance.positionRaw = position;
        //    tileInstance.rotation = rotation;

        //    return tileInstance;
        //}
        //public TileState CreateTile(Tile tile, UnityEngine.Vector3Int position, int subdivisions = 1, UnityEngine.Vector3Int rotation = default)
        //{
        //    TileState tileInstance = CreateTile(tile);

        //    tileInstance.subdivisions = subdivisions;
        //    tileInstance.position = position;
        //    tileInstance.rotation = rotation;

        //    return tileInstance;
        //}

        //INHERITED SPACE CONVERSION
        public Vector3 GridToOne(Vector3 position, int subdivisions = 1)
        {
            return /*grid*/Grid.GridToOne(position, subdivisions);
        }
        public Vector3 OneToGrid(Vector3 position, int subdivisions = 1)
        {
            return /*grid*/Grid.OneToGrid(position, subdivisions);
        }
        public Vector3 GridToGrid(Vector3 position, int subdivisions = 1, int newSubdivisions = 1)
        {
            return /*grid*/Grid.GridToGrid(position, subdivisions);
        }


        //SPACE CONVERSION
        public Vector3 GridToWorld(Vector3 position, int subdivisions = 1)
        {
            position = /*grid*/Grid.GridToOne(position, subdivisions);

            position -= offsetCenterPoint;

            //multiplies position
            Vector3 localPos = position.Multiply(tileSize * totalScale);

            //world position
            Vector3 worldPos = Linking.TransformPoint(localPos, finalPosition, finalRotation);

            return worldPos;
        }
        public Vector3 WorldToGrid(Vector3 position, int subdivisions = 1)
        {
            //local position
            Vector3 localPos = Linking.InverseTransformPoint(position, initialPosition(finalPosition), initialRotation(finalRotation));

            localPos = localPos.Divide(tileSize * totalScale);
            //localPos = localPos.Divide(tileSize / totalScale); 


            localPos += offsetCenterPoint;

            return /*grid*/Grid.OneToGrid(localPos, subdivisions);
        }

        public Vector3 GridToWorldDirection (Vector3 direction)
        {
            return Linking.TransformPoint(direction, Vector3.zero, finalRotation).normalized;
        }
        public Vector3 WorldToGridDirection (Vector3 direction)
        {
            return Linking.InverseTransformPoint(direction, Vector3.zero, initialRotation(finalRotation)).normalized;
        }

        public float GridToWorldDistance(Ray ray, float distance, int subdivisions = 1)
        {
            return Vector3.Distance(GridToWorld(ray.origin, subdivisions), GridToWorld(ray.GetPoint(distance), subdivisions));
        }
        public float WorldToGridDistance(Ray ray, float distance, int subdivisions = 1)
        {
            return Vector3.Distance(WorldToGrid(ray.origin, subdivisions), WorldToGrid(ray.GetPoint(distance), subdivisions));
        }

        public Ray GridToWorldRay (Ray ray, int subdivisions = 1)
        {
            return new Ray(GridToWorld(ray.origin, subdivisions), GridToWorldDirection(ray.direction));
        }
        public Ray WorldToGridRay (Ray ray, int subdivisions = 1)
        {
            return new Ray(WorldToGrid(ray.origin, subdivisions), WorldToGridDirection(ray.direction));
        }

        //GRID RAYCASTING ONTO CENTERS OR EDGES
        public GridOrientationCastHit GridToWorldHit(GridCastHit hit, Axis axis, Ray ray)
        {
            if (hit != null)
            {
                GridOrientationCastHit newHit = new GridOrientationCastHit();

                newHit.point = hit.point;
                newHit.distance = hit.distance;

                newHit.worldPoint = GridToWorld(hit.point);
                newHit.worldDistance = GridToWorldDistance(ray, hit.distance);

                return newHit;
            }
            else
            {
                return null;
            }
        }
        public GridCastHit WorldToGridHit(GridOrientationCastHit hit, Axis axis, Ray ray)
        {
            return hit;
        }



        //takes world position 
        //distance in one's unit
        //+++++++++++++++TO BE TESTED OUT!!!!!!!
        public GridOrientationCastHit TileCast(Ray ray, Axis axis, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            GridCastHit hitInfo = /*grid*/Grid.TileCast(WorldToGridRay(ray, subdivisions), axis, WorldToGridDistance(ray, maxDistance), subdivisions);

            return GridToWorldHit(hitInfo, axis, ray);
        }
        public List<GridOrientationCastHit> TileCastAll(Ray ray, Axis axis, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            List<GridOrientationCastHit> hitInfo = new List<GridOrientationCastHit>();

            foreach (GridCastHit i in /*grid*/Grid.TileCastAll(WorldToGridRay(ray, subdivisions), axis, WorldToGridDistance(ray, maxDistance), subdivisions))
            {
                hitInfo.Add(GridToWorldHit(i, axis, ray));
            }

            return hitInfo;
        }

        public Vector3T<GridOrientationCastHit> TileCastAxis (Ray ray, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            Vector3T<GridOrientationCastHit> hitInfo = null;

            foreach (Axis axis in Vectors.axisIterate)
            {
                hitInfo = hitInfo.SetAxis(axis, TileCast(ray, axis, maxDistance, subdivisions/*, AxisMask*/));
            }

            return hitInfo;
        }
        public Vector3T<List<GridOrientationCastHit>> TileCastAxisAll(Ray ray, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            Vector3T<List<GridOrientationCastHit>> hitInfo = null;

            foreach (Axis axis in Vectors.axisIterate)
            {
                hitInfo = hitInfo.SetAxis(axis, TileCastAll(ray, axis, maxDistance, subdivisions/*, AxisMask*/));
            }

            return hitInfo;
        }


        public GridOrientationCastHit EdgeCast(Ray ray, Axis axis, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            GridCastHit hitInfo = /*grid*/Grid.EdgeCast(WorldToGridRay(ray, subdivisions), axis, WorldToGridDistance(ray, maxDistance), subdivisions);

            return GridToWorldHit(hitInfo, axis, ray);
        }
        public List<GridOrientationCastHit> EdgeCastAll(Ray ray, Axis axis, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            List<GridOrientationCastHit> hitInfo = new List<GridOrientationCastHit>();

            foreach (GridCastHit i in /*grid*/Grid.EdgeCastAll(WorldToGridRay(ray, subdivisions), axis, WorldToGridDistance(ray, maxDistance), subdivisions))
            {
                hitInfo.Add(GridToWorldHit(i, axis, ray));
            }

            return hitInfo;
        }

        public Vector3T<GridOrientationCastHit> EdgeCastAxis(Ray ray, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            Vector3T<GridOrientationCastHit> hitInfo = null;

            foreach (Axis axis in Vectors.axisIterate)
            {
                hitInfo = hitInfo.SetAxis(axis, TileCast(ray, axis, maxDistance, subdivisions/*, AxisMask*/));
            }

            return hitInfo;
        }
        public Vector3T<List<GridOrientationCastHit>> EdgeCastAxisAll(Ray ray, float maxDistance = Mathf.Infinity, int subdivisions = 1/*, int AxisMask = 000*/)
        {
            Vector3T<List<GridOrientationCastHit>> hitInfo = null;

            foreach (Axis axis in Vectors.axisIterate)
            {
                hitInfo = hitInfo.SetAxis(axis, TileCastAll(ray, axis, maxDistance, subdivisions/*, AxisMask*/));
            }

            return hitInfo;
        }



        //EDGE DETECTION

        public Vector3Bool WorldOnEdge(Vector3 position, int subdivisions = 1)
        {
            return GridOnEdge(WorldToGrid(position, subdivisions));
        }
        public Vector3Bool GridOnEdge(Vector3 position, int subdivisions = 1)
        { //Checks if position parameter has axis equal to 0.5f or -0.5f
            position = /*grid*/Grid.OneToGrid(position.normalized, subdivisions);

            Vector3Bool edgeCase = new Vector3Bool(false, false, false);
            return (Vector3Bool)edgeCase.Operate((s, a) =>
            {
                if (
                    position.GetAxis(s) % 1f == 0.5f ||
                    position.GetAxis(s) % 1f == -0.5f
                )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }


        //STATIC METHODS

        //Intersection (Tile Occupation)
        //public static bool IntersectsRaw(GridOrientation orientationA, Vector3 positionA, Vector3 sizeA, Quaternion rotationA, int subdivisionsA, GridOrientation orientationB, Vector3 positionB, Vector3 sizeB, Quaternion rotationB, int subdivisionsB)
        //{ //factors position, size and rotation
            
        //    Geometry.Box tileA = new Geometry.Box(orientationA.GridToWorld(positionA, subdivisionsA), Linking.TransformEuler(rotationA, orientationA.finalRotation), orientationA.grid.subdividedTileSize(subdivisionsA).Multiply(sizeA) * orientationA.totalScale);
        //    Geometry.Box tileB = new Geometry.Box(orientationB.GridToWorld(positionB, subdivisionsB), Linking.TransformEuler(rotationB, orientationB.finalRotation), orientationB.grid.subdividedTileSize(subdivisionsB).Multiply(sizeB) * orientationB.totalScale);

        //    return tileA.Intersects(tileB, true);
        //}
        //public static bool IntersectsRaw(GridOrientation orientationA, Vector3 positionA, Vector3 sizeA, int subdivisionsA, GridOrientation orientationB, Vector3 positionB, Vector3 sizeB, int subdivisionsB)
        //{ //factors position and size
        //    return IntersectsRaw(orientationA, positionA, sizeA, Quaternion.identity, subdivisionsA, orientationB, positionB, sizeB, Quaternion.identity, subdivisionsB);
        //}
        //public static bool IntersectsRaw(GridOrientation orientationA, Vector3 a, int subdivisionsA, GridOrientation orientationB, Vector3 b, int subdivisionsB)
        //{ //factors position
        //    return IntersectsRaw(orientationA, a, Vector3.one, subdivisionsA, orientationB, b, Vector3.one, subdivisionsB);
        //}
        //public static bool Intersects(GridOrientation orientationA, UnityEngine.Vector3Int a, int subdivisionsA, GridOrientation orientationB, UnityEngine.Vector3Int b, int subdivisionsB)
        //{ //doesnt use raw values
        //    return IntersectsRaw(orientationA, a, subdivisionsA, orientationB, b, subdivisionsB);
        //}

        //public static bool AdjacentRaw(GridOrientation orientationA, Vector3 positionA, Vector3 sizeA, Quaternion rotationA, int subdivisionsA, GridOrientation orientationB, Vector3 positionB, Vector3 sizeB, Quaternion rotationB, int subdivisionsB)
        //{ //factors position, size and rotation

        //    Geometry.Box tileA = new Geometry.Box(orientationA.GridToWorld(positionA, subdivisionsA), Linking.TransformEuler(rotationA, orientationA.finalRotation), orientationA.grid.subdividedTileSize(subdivisionsA).Multiply(sizeA) * orientationA.totalScale);
        //    Geometry.Box tileB = new Geometry.Box(orientationB.GridToWorld(positionB, subdivisionsB), Linking.TransformEuler(rotationB, orientationB.finalRotation), orientationB.grid.subdividedTileSize(subdivisionsB).Multiply(sizeB) * orientationB.totalScale);

        //    return tileA.Adjacent(tileB);
        //}
        //public static bool AdjacentRaw(GridOrientation orientationA, Vector3 positionA, Vector3 sizeA, int subdivisionsA, GridOrientation orientationB, Vector3 positionB, Vector3 sizeB, int subdivisionsB)
        //{ //factors position and size
        //    return AdjacentRaw(orientationA, positionA, sizeA, Quaternion.identity, subdivisionsA, orientationB, positionB, sizeB, Quaternion.identity, subdivisionsB);
        //}
        //public static bool AdjacentRaw(GridOrientation orientationA, Vector3 a, int subdivisionsA, GridOrientation orientationB, Vector3 b, int subdivisionsB)
        //{ //factors position
        //    return AdjacentRaw(orientationA, a, Vector3.one, subdivisionsA, orientationB, b, Vector3.one, subdivisionsB);
        //}
        //public static bool Adjacent(GridOrientation orientationA, UnityEngine.Vector3Int a, int subdivisionsA, GridOrientation orientationB, UnityEngine.Vector3Int b, int subdivisionsB)
        //{ //doesnt use raw values
        //    return AdjacentRaw(orientationA, a, subdivisionsA, orientationB, b, subdivisionsB);
        //}



        //GIZMOS

        //takes in grid position
        private void DrawTileFill(Vector3 position, Axis axis, int subdivisions = 1, bool deepFill = false)
        {
            List<Axis> drawAxes = new List<Axis>();
            foreach (Axis i in Vectors.axisIterate)
            {
                if (i != axis)
                {
                    drawAxes.Add(i);
                }
            }

            if (!deepFill) {
                Gizmos.color = new Color(
                    Gizmos.color.r,
                    Gizmos.color.g,
                    Gizmos.color.b,
                    0.5f
                );
            } else if (deepFill)
            {
                Gizmos.color = new Color(
                    Gizmos.color.r,
                    Gizmos.color.g,
                    Gizmos.color.b,
                    0.75f
                );
            }

            Vector3 fillSize =
                (Vectors.axisDirections[drawAxes[0]] * tileSize.GetAxis(drawAxes[0])) +
                (Vectors.axisDirections[drawAxes[1]] * tileSize.GetAxis(drawAxes[1]));

            Gizmos.matrix = Matrix4x4.TRS(
                GridToWorld(position, 1),
                finalRotation,
                fillSize * totalScale
            );

            Gizmos.DrawCube(Vector3.zero, Vector3.one);

            //resets
            Gizmos.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(Vector3.zero), Vector3.one);
        }
        private void DrawTileOutline(Vector3 position, Axis axis, int subdivisions = 1)
        {
            Vector2T<Axis> drawAxes = Vectors.axisPlanes[axis];

            Vector3 fillSize =
                (Vectors.axisDirections[drawAxes.x] * tileSize.GetAxis(drawAxes.x)) +
                (Vectors.axisDirections[drawAxes.y] * tileSize.GetAxis(drawAxes.y));

            Gizmos.matrix = Matrix4x4.TRS(
                GridToWorld(position, 1),
                finalRotation,
                fillSize * totalScale
            );

            Vector3 localPoint = SceneView.lastActiveSceneView.camera.transform.InverseTransformPoint(GridToWorld(position));
            localPoint = new Vector3(localPoint.x, localPoint.y, 0f);

            if (localPoint.magnitude <= 10f)
            {
                Gizmos.color = new Color(
                    Gizmos.color.r,
                    Gizmos.color.g,
                    Gizmos.color.b,
                    0.5f
                );
            }
            else
            {
                Gizmos.color = new Color(
                    Gizmos.color.r,
                    Gizmos.color.g,
                    Gizmos.color.b,
                    0.2f
                );
            }

            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

            if (subdivisions > 1 && Vector3.Distance(WorldToGrid(Camera.current.transform.position), position) <= subdivisionRenderDistance)
            {
                for (int x = 0; x < subdivisions; x++)
                {
                    for (int y = 0; y < subdivisions; y++)
                    {
                        Gizmos.color = new Color(
                            Gizmos.color.r,
                            Gizmos.color.g,
                            Gizmos.color.b,
                            0.05f
                        );

                        if (RMath.Odd(subdivisions))
                        {
                            Gizmos.DrawWireCube(
                                Vectors.axisDirections[drawAxes.x] * ((x - Mathf.Floor(subdivisions / 2)) / subdivisions) +
                                Vectors.axisDirections[drawAxes.y] * ((y - Mathf.Floor(subdivisions / 2)) / subdivisions),
                                Vector3.one / subdivisions
                            );
                        }
                        else if (RMath.Even(subdivisions))
                        {
                            Gizmos.DrawWireCube(
                                Vectors.axisDirections[drawAxes.x] * ((x + 0.5f - (subdivisions / 2)) / subdivisions) +
                                Vectors.axisDirections[drawAxes.y] * ((y + 0.5f - (subdivisions / 2)) / subdivisions),
                                Vector3.one / subdivisions
                            );
                        }
                    }
                }
            }
        }
        private void DrawTile(Vector3 position, Axis axis, int subdivisions = 1, bool deepFill = false)
        {
            Vector2T<Axis> drawAxes = Vectors.axisPlanes[axis];

            if (Camera.current.InView(new Vector3[]{ 
                GridToWorld(position + (Vectors.axisDirections[drawAxes.x] * 0.5f) + (Vectors.axisDirections[drawAxes.y] * 0.5f)),
                GridToWorld(position + (Vectors.axisDirections[drawAxes.x] * -0.5f) + (Vectors.axisDirections[drawAxes.y] * 0.5f)),
                GridToWorld(position + (Vectors.axisDirections[drawAxes.x] * -0.5f) + (Vectors.axisDirections[drawAxes.y] * -0.5f)),
                GridToWorld(position + (Vectors.axisDirections[drawAxes.x] * 0.5f) + (Vectors.axisDirections[drawAxes.y] * -0.5f)),
                })) {
                DrawTileFill(position, axis, subdivisions, deepFill);
                DrawTileOutline(position, axis, subdivisions);
            }
        }

        private void DrawGridPlane(Axis axis, GridPlaneOrientation orientation, int subdivisions = 1)
        {
            if (grid != null)
            {
                //axis that the grid uses (always 2)
                Vector2T<Axis> drawAxes = Vectors.axisPlanes[axis];

                float interX = Mathf.Abs(offsetCenterPoint.GetAxis(drawAxes.x) % 1f);
                float interY = Mathf.Abs(offsetCenterPoint.GetAxis(drawAxes.y) % 1f);
                for (float x = -orientation.size.x; x <= orientation.size.x + (interX >= 0.5f ? -1f : 0f); x++)
                {
                    for (float y = -orientation.size.y; y <= orientation.size.y + (interY >= 0.5f ? -1f : 0f); y++)
                    {
                        Gizmos.color = axisColours[axis];
                        if (orientation.planeOnEdge) {
                            Gizmos.color = new Color(
                                Gizmos.color.r,
                                Gizmos.color.g,
                                Gizmos.color.b
                            );
                        }

                        DrawTile(
                            orientation.offset +
                            (Vectors.axisDirections[drawAxes.x] * (x + (interX >= 0.5f ? 1f : 0f))) +
                            (Vectors.axisDirections[drawAxes.y] * (y + (interY >= 0.5f ? 1f : 0f))) + 
                            (Vectors.axisDirections[axis] * orientation.normalOffset),
                            axis,
                            subdivisions,
                            orientation.planeOnEdge
                        );
                    }
                }
            }
        }
        private void DrawGrids()
        {
            foreach (Axis axis in Vectors.axisIterate)
            {
                foreach (GridPlaneOrientation orientation in gridOrientations.GetAxis(axis)) 
                {
                    if (showAxis.GetAxis(axis) && orientation.enabled) {
                        DrawGridPlane(axis, orientation, orientation.subdivisions);
                    }
                }
            }
        }

        //+++++++++THESE FUNCTIONS CAUSE CRASHING!!!!!!!
        private void OnDrawGizmos()
        {
            if (showGridGizmos && !showOnlyWhenSelected)
            {
                DrawGrids();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (showGridGizmos && showOnlyWhenSelected)
            {
                DrawGrids();
            }
        }
    }
}
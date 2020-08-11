using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using REXTools.TransformTools;

namespace REXTools.Tiling
{
    public class GridOrientation : MonoBehaviour
    {
        //grid size
        public Grid grid;

        //orientation (for world or self)
        public AxisOrder offsetPosition;
        public AxisOrder offsetRotation;
        public Vector3 offsetScale = Vector3.one;
        public Vector3 offsetSpacing = Vector3.zero; //offset margin added onto margin

        //space
        public Space space = Space.Self;

        [Space]
        [Header("Scene View Settings")]
        public bool showGridGizmos = true;
        public bool showOnlyWhenSelected = true;

        public Vector3Int gridReach = Vector3Int.one * 10;
        public Vector3 gridOffset = Vector3.zero;

        public Vector3Int gridRepeats = Vector3Int.one;
        public Vector3 gridPlaneSpacing = Vector3.one * 10f;

        [Header("Grid Axis")]
        public bool showX = false;
        public bool showY = true;
        public bool showZ = false;
        
        //private
        private Dictionary<Axis, Color> axisColours = new Dictionary<Axis, Color>
        {
            {Axis.X, new Color(0.8588235f, 0.2431373f, 0.1137255f, 0.5f)},
            {Axis.Y, new Color(0.6039216f, 0.9529412f, 0.282353f, 0.5f)},
            {Axis.Z, new Color(0.227451f, 0.4784314f, 0.972549f, 0.5f)}
        };

        //properties
        public Vector3 spaceSize
        {
            get
            {
                return (grid.spacing + offsetSpacing).Multiply(totalScale);
            }
        }
        public Vector3 tileSize
        {
            get
            {
                return grid.size.Multiply(totalScale);
            }
        }

        public Vector3 finalPosition
        {
            get
            {
                if (space == Space.Self)
                {
                    return offsetPosition.ApplyPosition(transform.position, transform.rotation, totalScale, transform.position);
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
                return offsetPosition.ReversePosition(transform.position, transform.rotation, totalScale, current);
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

        public Vector3 totalScale
        {
            get
            {
                if (space == Space.Self)
                {
                    return transform.localScale.Multiply(offsetScale).Multiply(grid.scale);
                }
                else
                {
                    return offsetScale.Multiply(grid.scale);
                }
            }
        }

        //gismos
        private Dictionary<Axis, bool> showGridAxis
        {
            get
            {
                return new Dictionary<Axis, bool> {
                    {Axis.X, showX},
                    {Axis.Y, showY},
                    {Axis.Z, showZ}
                };
            }
        }

        //methods

        //creates new tile
        public TileState CreateTile(Tile tile)
        {
            GameObject tileInstance = Instantiate(tile.prefab);
            TileState tileState = tileInstance.AddComponent<TileState>();
            tileState.reference = tile;
            tileState.grid = this;

            return tileState;
        }
        //moves existing tile
        public void SetPosition(TileState tile, Vector3 position)
        {

        }
        public void SetPosition(TileState tile, Vector3 position, Vector3Int rotation)
        {

        }

        public Vector3 GridToWorld(Vector3 position)
        {
            if (grid.centerPoint == TileOrigin.Intersect)
            {
                position -= Vector3.one / 2f;
            }
            //multiplies position
            Vector3 localPos = position.Multiply(tileSize);

            //total spacing
            Vector3 spaceCount = position.Operate((s, a) => Mathf.Round(a));
            Vector3 totalSpacing = spaceCount.Multiply(spaceSize);
            totalSpacing.Operate(tileSize, (s, a, b) =>
            {
                return a + ((b / 2f) * RMath.SignZeroed(a));
            });

            localPos.Operate(totalSpacing, (s, ALocalPos, ATotalSpacing) =>
            {
                if (grid.centerOnSpace && position.GetAxis(s) % 1f == 0.5f) //centers position on space
                {

                    //gets closest tile to the right (Ceil)
                    float snapPos = Mathf.Ceil(localPos.Divide(tileSize + spaceSize).GetAxis(s))
                    * (tileSize + spaceSize).GetAxis(s);

                    float up = snapPos - (tileSize / 2f).GetAxis(s);
                    float down = snapPos - ((tileSize / 2f) + spaceSize).GetAxis(s);

                    //checks if in between spaces
                    //make sure goes in center of space
                    return Mathf.Lerp(up, down, 0.5f);
                }
                else //if position doesnt need to be centered on space
                {
                    return ALocalPos + ATotalSpacing;
                }
            });

            //world position
            Vector3 worldPos = Linking.TransformPoint(localPos, finalPosition, finalRotation);

            return worldPos;
        }
        public Vector3 WorldToGrid(Vector3 position)
        {
            position = WorldEdgeSnap(position/*, grid.centerOnSpace*/);

            //local position
            Vector3 localPos = Linking.InverseTransformPoint(position, initialPosition(finalPosition), initialRotation(finalRotation));

            //adds to position if located on center
            if (grid.centerPoint == TileOrigin.Intersect)
            {
                localPos += Vector3.one / 2f;
            }

            //below: gets local position relative to closest tile
            // then divides them into grid units
            Vector3 snapPos = localPos.Divide(tileSize + spaceSize).Operate((s, a) => Mathf.Round(a)).Multiply(tileSize + spaceSize);
            Vector3 difference = localPos - snapPos;

            difference = difference.Divide(tileSize);
            snapPos = snapPos.Divide(tileSize + spaceSize);

            //adds position back to closest tile
            Vector3 gridPos = snapPos + difference;
            return gridPos;
        }

        public bool WorldOnEdge(Vector3 position)
        {
            Vector3 localPos = Linking.InverseTransformPoint(position, initialPosition(finalPosition), initialRotation(finalRotation));

            if (grid.centerPoint == TileOrigin.Intersect)
            {
                position -= Vector3.one / 2f;
            }

            foreach (Axis i in Vectors.axisDefaultOrder)
            {
                float snapPos = Mathf.Ceil(localPos.Divide(tileSize + spaceSize).GetAxis(i))
                    * (tileSize + spaceSize).GetAxis(i);

                float up = snapPos - (tileSize / 2f).GetAxis(i);
                float down = snapPos - ((tileSize / 2f) + spaceSize).GetAxis(i);

                if (
                    localPos.GetAxis(i) < up &&
                    localPos.GetAxis(i) > down
                )
                {
                    return true;
                }
            }

            return false;
        }

        //returns world position
        public Vector3 WorldEdgeSnap(Vector3 position, bool centerOnEdge = false)
        {
            Vector3 localPos = Linking.InverseTransformPoint(position, initialPosition(finalPosition), initialRotation(finalRotation));

            if (grid.centerPoint == TileOrigin.Intersect)
            {
                position -= Vector3.one / 2f;
            }

            Vector3 newPos = position;

            foreach (Axis i in Vectors.axisDefaultOrder)
            {
                float snapPos = Mathf.Ceil(localPos.Divide(tileSize + spaceSize).GetAxis(i))
                    * (tileSize + spaceSize).GetAxis(i);

                float up = snapPos - (tileSize / 2f).GetAxis(i);
                float down = snapPos - ((tileSize / 2f) + spaceSize).GetAxis(i);

                //checks if in between spaces
                if (
                    localPos.GetAxis(i) < up &&
                    localPos.GetAxis(i) > down
                )
                {
                    if (!centerOnEdge)
                    {
                        if (Mathf.Abs(up - localPos.GetAxis(i)) < Mathf.Abs(down - localPos.GetAxis(i)))
                        {
                            newPos = newPos.SetAxis(i, up);
                        }
                        else
                        {
                            newPos = newPos.SetAxis(i, down);
                        }
                    }
                    else
                    {
                        newPos = newPos.SetAxis(i, Mathf.Lerp(up, down, 0.5f));
                    }
                }
            }

            return newPos;
        }

        //takes in grid position
        private void DrawTileFill(Vector3 position, Axis axis, int subdividisions = 1)
        {
            List<Axis> drawAxes = new List<Axis>();
            foreach (Axis i in Vectors.axisIterate)
            {
                if (i != axis)
                {
                    drawAxes.Add(i);
                }
            }

            Gizmos.color = new Color(
                Gizmos.color.r,
                Gizmos.color.g,
                Gizmos.color.b,
                0.3f
            );
            
            Vector3 fillSize =
                (Vectors.axisDirections[drawAxes[0]] * tileSize.GetAxis(drawAxes[0])) +
                (Vectors.axisDirections[drawAxes[1]] * tileSize.GetAxis(drawAxes[1]));

            Gizmos.matrix = Matrix4x4.TRS(
                GridToWorld(position),
                finalRotation,
                fillSize.Multiply(totalScale)
            );

            Gizmos.DrawCube(Vector3.zero, Vector3.one);
            
            //resets
            Gizmos.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(Vector3.zero), Vector3.one);
        }
        private void DrawTileOutline(Vector3 position, Axis axis, int subdividisions = 1)
        {
            List<Axis> drawAxes = new List<Axis>();
            foreach (Axis i in Vectors.axisIterate)
            {
                if (i != axis)
                {
                    drawAxes.Add(i);
                }
            }

            Vector3 fillSize =
                (Vectors.axisDirections[drawAxes[0]] * tileSize.GetAxis(drawAxes[0])) +
                (Vectors.axisDirections[drawAxes[1]] * tileSize.GetAxis(drawAxes[1]));
            
            Gizmos.matrix = Matrix4x4.TRS(
                GridToWorld(position),
                finalRotation,
                fillSize.Multiply(totalScale)
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
                    0.1f
                );
            }

            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }

        //takes in grid position
        //private void DrawTileOutline(Vector3 position, Axis axis, int subdividisions = 1)
        //{
        //    //Gizmos.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(Vector3.zero), Vector3.one);

        //    List<Axis> drawAxes = new List<Axis>();
        //    foreach (Axis i in Vectors.axisIterate)
        //    {
        //        if (i != axis)
        //        {
        //            drawAxes.Add(i);
        //        }
        //    }

        //    //points
        //    //List<Vector3> points = new List<Vector3>()
        //    //{
        //    //    Vectors.axisDirections[drawAxes[0]] * (grid.size.GetAxis(drawAxes[0]) / 2f) +
        //    //    Vectors.axisDirections[drawAxes[1]] * (grid.size.GetAxis(drawAxes[0]) / 2f),

        //    //    Vectors.axisDirections[drawAxes[0]] * (grid.size.GetAxis(drawAxes[0]) / 2f) +
        //    //    -Vectors.axisDirections[drawAxes[1]] * (grid.size.GetAxis(drawAxes[0]) / 2f),

        //    //    -Vectors.axisDirections[drawAxes[0]] * (grid.size.GetAxis(drawAxes[0]) / 2f) +
        //    //    -Vectors.axisDirections[drawAxes[1]] * (grid.size.GetAxis(drawAxes[0]) / 2f),

        //    //    -Vectors.axisDirections[drawAxes[0]] * (grid.size.GetAxis(drawAxes[0]) / 2f) +
        //    //    Vectors.axisDirections[drawAxes[1]] * (grid.size.GetAxis(drawAxes[0]) / 2f),
        //    //};
        //    List<Vector3> points = new List<Vector3>()
        //    {
        //        Vectors.axisDirections[drawAxes[0]] / 2f +
        //        Vectors.axisDirections[drawAxes[1]] / 2f,

        //        Vectors.axisDirections[drawAxes[0]] / 2f +
        //        -Vectors.axisDirections[drawAxes[1]] / 2f,

        //        -Vectors.axisDirections[drawAxes[0]] / 2f +
        //        -Vectors.axisDirections[drawAxes[1]] / 2f,

        //        -Vectors.axisDirections[drawAxes[0]] / 2f +
        //        Vectors.axisDirections[drawAxes[1]] / 2f,
        //    };

        //    //making it local
        //    for (int i = 0; i < 4; i++)
        //    {
        //        points[i] = GridToWorld(position + points[i])/*.SetAxis(axis, position.GetAxis(axis))*/;
        //    }

        //    //connects points to draw gizmos
        //    for (int i = 0; i < 4; i++)
        //    {
        //        Vector3[] linePoints;

        //        if (i != 3)
        //        {
        //            linePoints = new Vector3[] { points[i], points[i + 1] };
        //        }
        //        else
        //        {
        //            linePoints = new Vector3[] { points[i], points[0] };
        //        }

        //        //local to camera
        //        Vector3 localPoint = SceneView.lastActiveSceneView.camera.transform.InverseTransformPoint(Vector3.Lerp(linePoints[0], linePoints[1], 0.5f));
        //        localPoint = new Vector3(localPoint.x, localPoint.y, 0f);

        //        if (localPoint.magnitude <= 10f)
        //        {
        //            Gizmos.color = new Color(
        //                Gizmos.color.r,
        //                Gizmos.color.g,
        //                Gizmos.color.b,
        //                0.8f
        //            );
        //        }
        //        else
        //        {
        //            Gizmos.color = new Color(
        //                Gizmos.color.r,
        //                Gizmos.color.g,
        //                Gizmos.color.b,
        //                0.1f
        //            );
        //        }
                
        //        Gizmos.DrawLine(GridToWorld(linePoints[0]), GridToWorld(linePoints[1]));
        //    }
        //}
        private void DrawTile(Vector3 position, Axis axis, int subdividisions = 1)
        {
            DrawTileFill(position, axis, subdividisions);
            DrawTileOutline(position, axis, subdividisions);
        }

        private void DrawGrid(Axis axis, Vector2Int radius, float offset = 0f)
        {
            if (grid != null)
            {
                List<Axis> drawAxes = new List<Axis>();
                foreach (Axis i in Vectors.axisIterate)
                {
                    if (i != axis)
                    {
                        drawAxes.Add(i);
                    }
                }

                Vector3 direction = Vectors.axisDirections[axis];

                bool inter = grid.centerPoint == TileOrigin.Intersect;
                
                for (float x = -radius.x; x <= radius.x + (inter ? 1f : 0f); x++)
                {
                    for (float y = -radius.y; y <= radius.y + (inter ? 1f : 0f); y++)
                    {
                        /*Gizmos.color = new Color(
                            (x + radius.x) / ((radius.x * 2f) + 1f),
                            0f,//(((x + radius.x) / ((radius.x * 2f) + 1f)) + ((y + radius.y) / ((radius.y * 2f) + 1f))) / 2f, 
                            (y + radius.y) / ((radius.y * 2f) + 1f)
                        );*/

                        Gizmos.color = axisColours[axis];

                        DrawTile(/*GridToWorld(*/(Vectors.axisDirections[drawAxes[0]] * x) + (Vectors.axisDirections[drawAxes[1]] * y) + (direction * offset)/*)*/, axis);

                        //Gizmos.DrawSphere(GridToWorld((Vectors.axisDirections[drawAxes[0]] * x) + (Vectors.axisDirections[drawAxes[1]] * y) + (direction * offset)), 0.5f);
                    }
                }
            }
        }
        private void DrawGrids()
        {
            foreach (KeyValuePair<Axis, bool> i in showGridAxis)
            {
                if (i.Value)
                {
                    for (int j = 0; j < Mathf.Abs(gridRepeats.GetAxis(i.Key)); j++)
                    {
                        DrawGrid(i.Key, Vector2Int.one * gridReach.GetAxis(i.Key), gridOffset.GetAxis(i.Key) + (j * gridPlaneSpacing.GetAxis(i.Key)));
                        DrawGrid(i.Key, Vector2Int.one * gridReach.GetAxis(i.Key), gridOffset.GetAxis(i.Key) - (j * gridPlaneSpacing.GetAxis(i.Key)));
                    }
                }
            }
        }

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
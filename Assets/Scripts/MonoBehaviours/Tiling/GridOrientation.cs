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
        //grid size
        public Grid grid;

        //orientation (for world or self)
        public Vector3 offsetCenterPoint = Vector3.zero;
        [Space]
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

        public UnityEngine.Vector3Int gridReach = UnityEngine.Vector3Int.one * 10;
        public Vector3 gridOffset = Vector3.zero;

        public UnityEngine.Vector3Int gridRepeats = UnityEngine.Vector3Int.one;
        public Vector3 gridPlaneSpacing = Vector3.one * 10f;

        [Header("Grid Axis")]
        public bool showX = false;
        public bool showY = true;
        public bool showZ = false;

        public Vector3Bool showAxis = new Vector3Bool(true, true, true);



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

        public int maxSubdivisions
        {
            get
            {
                int subdivisions = 1;
                bool packed = false;

                while (!packed)
                {
                    ((subdivisions - 1) * spaceSize).OperateBool(tileSize, (s, ASubdivisions, ASize) =>
                    {
                        if (ASubdivisions >= ASize)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });

                    subdivisions++;
                }

                return subdivisions;
            }
        }
        public Vector3 subdividedTileSize(int subdivisions)
        {
            return tileSize - (spaceSize * (subdivisions - 1));
        }

        //directions
        public Vector3 x
        {
            get => finalRotation * Vector3.right;
        }
        public Vector3 y
        {
            get => finalRotation * Vector3.up;
        }
        public Vector3 z
        {
            get => finalRotation * Vector3.forward;
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



        //TILES

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
        public void SetPosition(TileState tile, Vector3 position, UnityEngine.Vector3Int rotation, bool centerOnSpace)
        {

        }

        //SPACE CONVERSION

        public Vector3 GridToWorld(Vector3 position, int subdiviisons = 1, bool centerOnSpace = false)
        {
            //if (grid.centerPoint == TileOrigin.Intersect)
            //{
            //    position -= Vector3.one * 0.5f;
            //}
            position -= offsetCenterPoint;

            //multiplies position
            Vector3 localPos = position.Multiply(tileSize);

            //total spacing
            Vector3 spaceCount = position.Operate((s, a) => Mathf.Round(a));
            Vector3 totalSpacing = spaceCount.Multiply(spaceSize);
            //adds half tilesize offset
            //totalSpacing = totalSpacing.Operate(tileSize, (s, ATotalSpacing, ATileSize) =>
            //{
            //    return ATotalSpacing + ((ATileSize / 2f) * RMath.SignZeroed(ATotalSpacing));
            //});

            localPos = localPos.Operate(totalSpacing, (s, ALocalPos, ATotalSpacing) =>
            {
                if (centerOnSpace) //centers position on space
                {
                    if (position.GetAxis(s) % 1f == 0.5f || position.GetAxis(s) % 1f == -0.5f)
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
                    else
                    {
                        return ALocalPos + ATotalSpacing;
                    }
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
        public Vector3 WorldToGrid(Vector3 position, int subdivisions = 1)
        {
            position = WorldEdgeSweep(position);

            //local position
            Vector3 localPos = Linking.InverseTransformPoint(position, initialPosition(finalPosition), initialRotation(finalRotation));

            //adds to position if located on center
            //if (grid.centerPoint == TileOrigin.Intersect)
            //{
            //    localPos += Vector3.one / 2f;
            //}
            localPos += offsetCenterPoint;

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
        

        public bool WorldOnEdge(Vector3 position, int subdivisions = 1)
        {
            Vector3 localPos = Linking.InverseTransformPoint(position, initialPosition(finalPosition), initialRotation(finalRotation));

            //if (grid.centerPoint == TileOrigin.Intersect)
            //{
            //    position -= Vector3.one / 2f;
            //}
            position -= offsetCenterPoint;

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
        public Vector3 WorldEdgeSweep(Vector3 position, int subdivisions = 1, bool centerOnSpace = false)
        {
            Vector3 localPos = Linking.InverseTransformPoint(position, initialPosition(finalPosition), initialRotation(finalRotation));

            position -= offsetCenterPoint;

            Vector3 newPos = position;

            newPos = newPos.Operate(localPos, (s, ANewPos, ALocalPos) =>
            {
                float snapPos = Mathf.Ceil(localPos.Divide(tileSize + spaceSize).GetAxis(s))
                    * (tileSize + spaceSize).GetAxis(s);

                float up = snapPos - (tileSize / 2f).GetAxis(s);
                float down = snapPos - ((tileSize / 2f) + spaceSize).GetAxis(s);

                //checks if in between spaces
                if (
                    ALocalPos < up &&
                    ALocalPos > down
                )
                {
                    if (!centerOnSpace)
                    {
                        if (Mathf.Abs(up - ALocalPos) < Mathf.Abs(down - ALocalPos))
                        {
                            return up;
                        }
                        else
                        {
                            return down;
                        }
                    }
                    else
                    {
                        return Mathf.Lerp(up, down, 0.5f);
                    }
                }
                else
                {
                    return ANewPos;
                }
            });

            return newPos;
        }

        //snaps ANY world position to tile edge
        public Vector3 WorldEdgeSnap(Vector3 position, int subdivisions = 1, bool centerOnSpace = false)
        {
            Vector3 localPos = Linking.InverseTransformPoint(position, initialPosition(finalPosition), initialRotation(finalRotation));

            position -= offsetCenterPoint;

            Vector3 newPos = position;

            newPos = newPos.Operate(localPos, (s, ANewPos, ALocalPos) =>
            {
                if (!centerOnSpace)
                {
                    //snaps to center of tile
                    float snapPosSpace = localPos.Divide(tileSize + spaceSize).Ceil().Multiply(
                        (tileSize + spaceSize)).GetAxis(s);
                    float upSpace = snapPosSpace - (tileSize / 2f).GetAxis(s);
                    float downSpace = snapPosSpace - ((tileSize / 2f) + spaceSize).GetAxis(s);
                    
                    float snapPosTile = localPos.Divide(tileSize + spaceSize).Round().Multiply(
                        (tileSize + spaceSize)).GetAxis(s);
                    float upTile = snapPosTile + (tileSize / 2f).GetAxis(s);
                    float downTile = snapPosTile - (tileSize / 2f).GetAxis(s);

                    List<float> snapPoints = new List<float>
                    {
                        upSpace, downSpace,
                        upTile, downTile
                    };

                    return snapPoints.Aggregate((oldPoint, newPoint) =>
                    {
                        if (Mathf.Abs(ALocalPos - oldPoint) < Mathf.Abs(ALocalPos - newPoint))
                        {
                            return oldPoint;
                        }
                        else
                        {
                            return newPoint;
                        }
                    });
                }
                else
                {
                    //not center on space
                    float snapPosSpace = localPos.Divide(tileSize + spaceSize).Ceil().Multiply(
                        (tileSize + spaceSize) -
                        ((tileSize + spaceSize) / 2f)).GetAxis(s);

                    return snapPosSpace;
                }
            });

            return newPos;
        }

        //takes world position
        public bool TileCast(Ray ray, out GridCastHit hitInfo, float maxDistance = Mathf.Infinity, int subdivisions = 1, int AxisMask = 000)
        {
            //hitInfo.point =
            //hitInfo.worldPoint =

            //hitInfo.distance =
            //hitInfo.worldDistance =




            throw new System.NotImplementedException();
        }

        //public bool TileCastAll(Ray ray, out GridCastHit[] hitInfo, float maxDistance = Mathf.Infinity, int subdivions = 1, int AxisMask = 000)
        //{

        //}

        //public bool EdgeCast (Ray ray, out GridCastHit hitInfo, float maxDistance = Mathf.Infinity, int subdivisions = 1, bool centerOnSpace = false, int AxisMask = 000)
        //{
        //    //hitInfo.point = 
        //    //hitInfo.worldPoint = 

        //    //hitInfo.distance = 
        //    //hitInfo.worldDistance = 

        //    //hitInfo.axis  =


        //}

        //public bool EdgeCastAll(Ray ray, out GridCastHit[] hitInfo, float maxDistance = Mathf.Infinity, int subdivisions = 1, bool centerOnSpace = false, int AxisMask = 000)
        //{

        //}
        //GIZMOS

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
                0.1f
            );

            Vector3 fillSize =
                (Vectors.axisDirections[drawAxes[0]] * tileSize.GetAxis(drawAxes[0])) +
                (Vectors.axisDirections[drawAxes[1]] * tileSize.GetAxis(drawAxes[1]));

            Gizmos.matrix = Matrix4x4.TRS(
                GridToWorld(position, 1, true),
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
                GridToWorld(position, 1, true),
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
        private void DrawTile(Vector3 position, Axis axis, int subdividisions = 1)
        {
            DrawTileFill(position, axis, subdividisions);
            DrawTileOutline(position, axis, subdividisions);
        }

        private void DrawGrid(Axis axis, UnityEngine.Vector2Int radius, float offset = 0f)
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

                //bool inter = grid.centerPoint == TileOrigin.Intersect;

                float inter1 = Mathf.Abs(offsetCenterPoint.GetAxis(drawAxes[0]) % 1f);
                float inter2 = Mathf.Abs(offsetCenterPoint.GetAxis(drawAxes[1]) % 1f);
                for (float x = -radius.x; x <= radius.x + (inter1 >= 0.5f ? 1f : 0f); x++)
                {
                    for (float y = -radius.y; y <= radius.y + (inter2 >= 0.5f ? 1f : 0f); y++)
                    {
                        /*Gizmos.color = new Color(
                            (x + radius.x) / ((radius.x * 2f) + 1f),
                            0f,//(((x + radius.x) / ((radius.x * 2f) + 1f)) + ((y + radius.y) / ((radius.y * 2f) + 1f))) / 2f, 
                            (y + radius.y) / ((radius.y * 2f) + 1f)
                        );*/

                        Gizmos.color = axisColours[axis];

                        DrawTile(/*GridToWorld(*/(Vectors.axisDirections[drawAxes[0]] * x) + (Vectors.axisDirections[drawAxes[1]] * y) + (direction * offset)/*)*/, axis);
                    }
                }
            }
        }
        private void DrawGrids()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(finalPosition, 0.2f);

            foreach (KeyValuePair<Axis, bool> i in showGridAxis)
            {
                if (i.Value)
                {
                    for (int j = 0; j < Mathf.Abs(gridRepeats.GetAxis(i.Key)); j++)
                    {
                        DrawGrid(i.Key, UnityEngine.Vector2Int.one * gridReach.GetAxis(i.Key), gridOffset.GetAxis(i.Key) + (j * gridPlaneSpacing.GetAxis(i.Key)));
                        DrawGrid(i.Key, UnityEngine.Vector2Int.one * gridReach.GetAxis(i.Key), gridOffset.GetAxis(i.Key) - (j * gridPlaneSpacing.GetAxis(i.Key)));
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using REXTools.TransformTools;

namespace REXTools.Tiling
{
    public class REXGridOrientation : MonoBehaviour
    {
        //grid size
        public REXGrid grid;

        //orientation (for world or self)
        public AxisOrder offsetPosition;
        public AxisOrder offsetRotation;
        public Vector3 offsetScale = Vector3.one;
        public Vector3 offsetSpacing = Vector3.zero; //offset margin added onto margin

        //space
        public Space space = Space.Self;

        //properties
        public Vector3 spaceSize
        {
            get
            {
                return Vectors.MultiplyVector3(grid.spacing + offsetSpacing, totalScale);
            }
        }
        public Vector3 tileSize
        {
            get
            {
                return Vectors.MultiplyVector3(grid.size, totalScale);
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
                    return Vectors.MultiplyVector3(Vectors.MultiplyVector3(transform.localScale, offsetScale), grid.scale);
                }
                else
                {
                    return Vectors.MultiplyVector3(offsetScale, grid.scale);
                }
            }
        }

        [Space]
        public Vector3 input;
        public Vector3 result;
        [Space]
        public Vector3 edgeDetect;
        public bool result2;

        [HideInInspector]
        public bool showGridGizmos = false;


        //methods

        //takes in grid position
        public Vector3 SnapPosition(Vector3 position, Vector3 increment)
        {
            return new Vector3(
                Mathf.Round(position.x / increment.x) * increment.x,
                Mathf.Round(position.y / increment.y) * increment.y,
                Mathf.Round(position.z / increment.z) * increment.z);

            //throw new System.NotImplementedException();
        }
        //takes in grid position
        public void SetPosition(RexTileProfile tile, Vector3 position)
        {

        }

        public Vector3 GridToWorld(Vector3 position)
        {
            //BLOCK

            //multiplies position
            Vector3 localPos = Vectors.MultiplyVector3(position, tileSize);

            //BLOCK
            Vector3 spaceCount = Vectors.OperateVector3(position, (a) => Vectors.SignCeil(a, true));

            Vector3 totalSpacing =
                Vectors.MultiplyVector3(
                    spaceCount,
                    spaceSize
                );

            localPos += totalSpacing;

            //BLOCK
            Vector3 worldPos = Linking.TransformPoint(localPos, finalPosition, finalRotation);

            return worldPos;
        }
        public Vector3 WorldToGrid(Vector3 position)
        {
            //BLOCK

            //actually local pos
            Vector3 worldPos = Linking.InverseTransformPoint(position, initialPosition(position), initialRotation(finalRotation));

            //BLOCK
            Vector3 spaceCount = Vectors.OperateVector3(worldPos, (a) => Vectors.SignCeil(a, true));

            Vector3 totalSpacing =
                Vectors.MultiplyVector3(
                    spaceCount,
                    spaceSize
                );

            worldPos -= totalSpacing;

            //BLOCK

            //actually world position
            Vector3 localPos = Vectors.DivideVector3(worldPos, totalScale);

            return localPos;
        }

        public bool WorldOnEdge(Vector3 position)
        {
            Vector3 localPos = Linking.InverseTransformPoint(position, initialPosition(finalPosition), initialRotation(finalRotation));

            foreach (Axis i in Vectors.axisDefaultOrder)
            {
                float snapPos = Mathf.Ceil(Vectors.GetAxis(i, localPos) / (Vectors.GetAxis(i, tileSize) + Vectors.GetAxis(i, spaceSize)))
                    * (Vectors.GetAxis(i, tileSize) + Vectors.GetAxis(i, spaceSize));

                float up = snapPos - ((Vectors.GetAxis(i, tileSize) / 2f));
                float down = snapPos - (((Vectors.GetAxis(i, tileSize) / 2f) + Vectors.GetAxis(i, spaceSize)));

                if (
                    Vectors.GetAxis(i, localPos) < up &&
                    Vectors.GetAxis(i, localPos) > down
                )
                {
                    return true;
                }
            }

            return false;
        }

        public Vector3 EdgePointClear(Vector3 position)
        {
            Vector3 localPos = Linking.InverseTransformPoint(position, initialPosition(finalPosition), initialRotation(finalRotation));

            Vector3 newPos = position;

            foreach (Axis i in Vectors.axisDefaultOrder)
            {
                float snapPos = Mathf.Ceil(Vectors.GetAxis(i, localPos) / (Vectors.GetAxis(i, tileSize) + Vectors.GetAxis(i, spaceSize)))
                    * (Vectors.GetAxis(i, tileSize) + Vectors.GetAxis(i, spaceSize));

                float up = snapPos - ((Vectors.GetAxis(i, tileSize) / 2f));
                float down = snapPos - (((Vectors.GetAxis(i, tileSize) / 2f) + Vectors.GetAxis(i, spaceSize)));

                if (
                    Vectors.GetAxis(i, localPos) < up &&
                    Vectors.GetAxis(i, localPos) > down
                )
                {
                    if (Mathf.Abs(up - Vectors.GetAxis(i, localPos)) < Mathf.Abs(down - Vectors.GetAxis(i, localPos)))
                    {
                        newPos = Vectors.SetAxis(i, up, newPos);
                    }
                    else
                    {
                        newPos = Vectors.SetAxis(i, down, newPos);
                    }
                }
            }

            return newPos;
        }

        private void DrawGrid(Axis axis, Vector2Int radius, float offset = 0f)
        {
            Vector3 direction = Vectors.axisDirections[axis];

            for (int x = -radius.x; x <= radius.x; x++)
            {
                for (int y = -radius.y; y <= radius.y; y++)
                {
                    Gizmos.color = new Color(
                        (x + radius.x) / ((radius.x * 2f) + 1f),
                        0f,//(((x + radius.x) / ((radius.x * 2f) + 1f)) + ((y + radius.y) / ((radius.y * 2f) + 1f))) / 2f, 
                        (y + radius.y) / ((radius.y * 2f) + 1f)
                    );

                    Gizmos.DrawSphere(GridToWorld(new Vector3(x, 0f, y)), 0.5f);

                }
            }
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR

            //Works! SceneView.lastActiveSceneView.camera
            //Gizmos.DrawSphere(SceneView.lastActiveSceneView.camera.transform.TransformPoint(Vector3.forward * 5f), 1f);
#endif
            if (grid != null)
            {
                DrawGrid(Axis.Y, Vector2Int.one * 10, 0);
            }

            //result = WorldToGrid(worldToGrid);
            result = GridToWorld(GridToWorld(input));

            result2 = WorldOnEdge(edgeDetect);

            Gizmos.color = Color.yellow;

            Gizmos.DrawSphere(edgeDetect, 0.5f);

            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(EdgePointClear(edgeDetect), 0.5f);
        }
    }
}
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
        public float offsetScale = 1f;
        public Vector3 offsetSpacing = Vector3.zero; //offset margin added onto margin

        //space
        public Space space = Space.Self;

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
            Vector3 localPos = Vectors.MultiplyVector3(Vectors.MultiplyVector3(position, grid.size), transform.localScale) * offsetScale;

            Vector3 spaceSize = Vectors.MultiplyVector3(grid.spacing + offsetSpacing, transform.localScale) * offsetScale;
            Vector3 spaceCount = Vectors.OperateVector3(position, (a) => Vectors.SignCeil(a, true));

            Vector3 totalSpacing =
                Vectors.MultiplyVector3(
                    spaceCount,
                    spaceSize
                ); //(Mathf.Ceil(position.x) - 1f) * ((grid.spacing.x + offsetSpacing.x) * offsetScale * transform.localScale),


            localPos += totalSpacing;


            //Vector3 finalPos = space == Space.World ? Vector3.zero : transform.position/*, offsetScale*/;
            Vector3 finalPos = offsetPosition.ApplyPosition(transform, space == Space.World ? Vector3.zero : transform.position/* + localPos*/, offsetScale);
            Quaternion finalRot = offsetRotation.ApplyRotation(transform, space == Space.World ? Quaternion.Euler(Vector3.zero) : transform.rotation);


            Vector3 worldPos = Linking.TransformPoint(localPos, finalPos, finalRot);

            return worldPos;
        }
        public Vector3 WorldToGrid(Vector3 position)
        {


            throw new System.NotImplementedException();
        }

        public bool WorldOnEdge(Vector3 position)
        {
            throw new System.NotImplementedException();
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
        }
    }
}
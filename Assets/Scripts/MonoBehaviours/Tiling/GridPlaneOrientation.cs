using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

namespace REXTools.Tiling
{
    [System.Serializable]
    public struct GridPlaneOrientation
    {
        public bool enabled;// = true;


        //Gizmos orientation
        public UnityEngine.Vector2Int size;// = new UnityEngine.Vector2Int(1, 1);

        public UnityEngine.Vector3Int offset;// = UnityEngine.Vector3Int.zero;
        public float normalOffset;// = 0f;

        public int subdivisions;// = 1;

        public bool planeOnEdge
        {
            get
            {
                return normalOffset % 1f == 0.5f || normalOffset % 1f == -0.5f;
            }
        }

        public GridPlaneOrientation(
            UnityEngine.Vector2Int size = default,
            UnityEngine.Vector3Int offset = default,
            float normalOffset = 0f,
            int subdivisions = 1
        ) {
            this.enabled = true;

            this.size = size;
            this.offset = offset;
            this.normalOffset = normalOffset;
            this.subdivisions = subdivisions;
        }
    }
}
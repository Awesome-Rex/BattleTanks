using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

namespace REXTools.Tiling
{
    public class GridCastHit
    {
        public Vector3 point;
        public Vector3 worldPoint;

        public float distance;
        public float worldDistance;

        public Axis axis;
    }
}

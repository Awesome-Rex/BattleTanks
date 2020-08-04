using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

namespace REXTools.Tiling {
    public class RexTileProfile : REXTileBase
    {
        public GameObject target;

        public Vector3 offsetPosition;
        public Vector3 offsetRotation;

        public float offsetScale;
    }
}
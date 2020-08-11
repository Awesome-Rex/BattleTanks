using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

namespace REXTools.Tiling {
    public class Tile : TileBase
    {
        public GameObject prefab;

        public Vector3 offsetPosition;
        public Vector3 offsetRotation;

        public float offsetScale;

        public override void ClearArea()
        {
            throw new System.NotImplementedException();
        }

        public override void Occupy(GameObject prefab, Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public override void OccupyAdditive(GameObject prefab, Vector3 position)
        {
            throw new System.NotImplementedException();
        }
    }
}
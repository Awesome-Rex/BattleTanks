using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

namespace REXTools.Tiling
{
    public abstract class TileBase : ScriptableObject
    {
        public Grid grid;

        public int subdivisions = 1;

        public abstract void Occupy(GameObject prefab, Vector3 position);

        public abstract void OccupyAdditive(GameObject prefab, Vector3 position);

        public abstract void ClearArea();
    }
}
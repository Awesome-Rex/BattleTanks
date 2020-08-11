using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

namespace REXTools.Tiling
{
    public class TileState : MonoBehaviour
    {
        public Tile reference;
        public GridOrientation grid;

        public Vector3Int rotation = Vector3Int.zero;
        
        public Quaternion offsetRotation
        {
            get
            {
                return Quaternion.Euler(rotation.Operate((s, a) => a - (int)(Mathf.Floor(a / 4) * 4)) * 90);
            }
        }

        public void SwitchGrid (GridOrientation newGrid)
        {
            grid = newGrid;
        }
    }
}
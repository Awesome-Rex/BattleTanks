using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

namespace REXTools.Tiling {
    public enum TileOrigin { Inside, Intersect }

    public class AreaTile : Tile
    {
        //public Dictionary<float, Dictionary<float, Dictionary<float, bool>>> area;
        
        public Vector3Int[] area;

        public TileOrigin originType = TileOrigin.Inside;

        public Vector3Int origin;

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
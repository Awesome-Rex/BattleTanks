using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

namespace REXTools.Tiling
{
    public enum TileOrigin { Inside, Intersect }

    [CreateAssetMenu(fileName = "New REX Area Tile", menuName = "REX/Tiling/Area Tile", order = 2)]
    public class AreaTile : Tile
    {
        public UnityEngine.Vector3Int[] area;

        //origin point of tile
        public TileOrigin originType = TileOrigin.Inside;
        public UnityEngine.Vector3Int origin;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

namespace REXTools.Tiling
{
    [CreateAssetMenu(fileName = "New REX Rule Tile", menuName = "REX/Tiling/Rule Tile")]
    public class RuleTile : TileBase
    {
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
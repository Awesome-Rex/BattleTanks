using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.TransformTools
{
    [System.Serializable]
    public class AdjacentHit
    {
        public BoxAdjacency adjacency = BoxAdjacency.None;
        //public BoxAlignment alignment;

        //attributes
        public Vector3Sign adjacencySign = new Vector3Sign(Vector3Sign.zero.Clone());
        //public Vector3Sign secondaryAdjacencySign; //adjacency normal from second box
        //+++++++++++RENAME TO ADJACENCY SIGN

        public AdjacentHit()
        {
            
        }
    }
}
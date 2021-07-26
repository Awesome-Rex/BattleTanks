using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.TransformTools
{
    [System.Serializable]
    public class ContainHit
    {
        public Containment containment = Containment.Inside;
        //public BoxAlignment alignment;

        //attributes
        public BoxAdjacency adjacency = BoxAdjacency.None;
        public Vector3Sign adjacencySign = new Vector3Sign(Vector3Sign.zero.Clone());
        public Vector3Sign leanSign = new Vector3Sign(Vector3Sign.zero.Clone()); //orientation inside box

        //hits
        public Dictionary<Vector3Sign, AdjacentHit> adjacentHits = null;// = new Dictionary<Vector3Sign, AdjacentHit>();
        //public int adjacentHitsCount = 0;
        //public AdjacentHit adjacentHits;

        public ContainHit()
        {

        }
    }
}
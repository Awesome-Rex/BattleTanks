using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.TransformTools
{
    [System.Serializable]
    public class IntersectHit
    {
        public Intersection intersection = Intersection.Intersect;
        //public BoxAlignment alignment;

        //attributes
        //...

        //hits
        public AdjacentHit adjacentHit = null;
        public ContainHit containHit = null;

        public IntersectHit()
        {

        }
    }
}
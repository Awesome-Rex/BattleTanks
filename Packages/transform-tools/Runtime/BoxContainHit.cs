using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.TransformTools
{
    [System.Serializable]
    public class BoxContainHit : ContainHit
    {
        public BoxAlignment alignment = BoxAlignment.Parallel;

        //attributes
        public bool adjacencyWhole = false;
        public float volume = 0f;
        public float volumeRatio = 0f;
        public bool dominant = false;

        //hits
        //public Dictionary<Vector3Sign, BoxAdjacentHit> adjacentHits
        //{
        //    get
        //    {
        //        //Dictionary<Vector3Sign, BoxAdjacentHit> newAdjacentHits = new Dictionary<Vector3Sign, BoxAdjacentHit>();

        //        //foreach (KeyValuePair<Vector3Sign, AdjacentHit> hit in base.adjacentHits)
        //        //{
        //        //    newAdjacentHits.Add(hit.Key, hit.Value as BoxAdjacentHit);
        //        //}

        //        //return newAdjacentHits;
        //        return base.adjacentHits;
        //    }
        //    set
        //    {
        //        Dictionary<Vector3Sign, AdjacentHit> newAdjacentHits = new Dictionary<Vector3Sign, AdjacentHit>();

        //        foreach (KeyValuePair<Vector3Sign, BoxAdjacentHit> hit in value)
        //        {
        //            newAdjacentHits.Add(hit.Key, hit.Value);
        //        }

        //        base.adjacentHits = newAdjacentHits;
        //    }
        //}
        //public new BoxAdjacentHit adjacentHits
        //{
        //    get
        //    {
        //        return base.adjacentHits as BoxAdjacentHit;
        //    }
        //    set
        //    {
        //        base.adjacentHits = value;
        //    }
        //}

        //public void Rotate(UnityEngine.Vector3Int eulers)
        //{
        //    alignment = BoxAlignment.Tilt;


        //}
    }
}
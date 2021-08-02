using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.TransformTools
{
    [System.Serializable]
    public class BoxIntersectHit : IntersectHit
    {
        public BoxAlignment alignment = BoxAlignment.Parallel;

        //attributes
        public float volume = 0f;
        public float volumeRatio = 0f; //volume taken from b
        public bool dominant = false; //collision takes over 50% space in box
        public BoxAdjacency type = BoxAdjacency.None; //# of corners inside b (corner = 1 corner, edge = 2 corners, face = 4 corners, object = 8 corners)

        //hits
        //public new BoxAdjacentHit adjacentHit
        //{
        //    get
        //    {
        //        return base.adjacentHit as BoxAdjacentHit;
        //    }
        //    set
        //    {
        //        base.adjacentHit = value;
        //    }
        //}
        //public new BoxContainHit containHit
        //{
        //    get
        //    {
        //        return base.containHit as BoxContainHit;
        //    }
        //    set
        //    {
        //        base.containHit = value;
        //    }
        //}

        //public void Rotate(UnityEngine.Vector3Int eulers)
        //{
        //    alignment = BoxAlignment.Tilt;


        //}
    }
}
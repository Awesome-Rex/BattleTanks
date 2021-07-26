using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.TransformTools
{
    [System.Serializable]
    public class BoxAdjacentHit : AdjacentHit
    {
        //public BoxAdjacency adjacency = BoxAdjacency.None;
        public BoxAlignment alignment = BoxAlignment.Parallel;

        //attributes
        //public Vector3Sign adjacencySign;
        public float surfaceArea = 0f;
        public float surfaceAreaRatio = 0f; //surface area between faces of a taken from b
        public bool dominant = false; //adjacent faces takes over 50% of surface area
        public BoxAdjacency type = BoxAdjacency.None;//# of corners adjacent to b (corner = 1 corner, edge = 2 corners, face = 4 corners, object = 8 corners)

        //public void Rotate(UnityEngine.Vector3Int eulers)
        //{
        //    alignment = BoxAlignment.Tilt;


        //}
    }
}
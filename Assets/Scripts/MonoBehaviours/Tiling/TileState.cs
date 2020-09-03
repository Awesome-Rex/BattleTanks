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

        //gives equivelent gameobject scasle with given scale
        //e.g. Vector3(1, 1, 1) = Vector3(-2, 1, 1)
        public Vector3 Scale(Vector3 newScale, Quaternion? parentRotation = null)
        {
            newScale *= reference.offsetScale;
            
            if (parentRotation == null)
            {
                parentRotation = transform.parent.rotation;
            }

            Quaternion actualRotation = Linking.InverseTransformEuler(transform.rotation, (Quaternion)parentRotation) * Quaternion.Inverse(offsetRotation);

            throw new System.NotImplementedException();
        }
    }
}
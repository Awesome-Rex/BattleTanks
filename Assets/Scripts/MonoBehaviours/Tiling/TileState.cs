using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

namespace REXTools.Tiling
{
    public class TileState : MonoBehaviour
    {
        public Tile tile;
        public virtual GridOrientation grid
        {
            get
            {
                return _grid;
            }
            set
            {
                _grid = value;
            }
        }
        [SerializeField]
        protected GridOrientation _grid;

        public Vector3 offsetPosition;
        public Quaternion offsetRotation;
        public Vector3 offsetScale;

        //*ORDER OF OFFSETS:
        //  tile.prefab.transform.position
        //  tile.offsetPosition
        //  offsetPosition

        public virtual Vector3 position
        {
            get
            {
                //apply opposite offset position (with subdivisions)
                return grid.WorldToGrid(transform.position - offsetPosition - tile.offsetPosition - tile.prefab.transform.position, subdivisions);
            }
            set
            {
                transform.position = grid.GridToWorld(value, subdivisions) + tile.prefab.transform.position + tile.offsetPosition + offsetPosition;
            }
        }
        public virtual UnityEngine.Vector3Int rotation
        {
            get
            {
                //apply opposite offset rotation (and round)

                return ((TransformTools.Vector3Int)UnityEngine.Vector3Int.zero.Operate((s, a) => Mathf.RoundToInt((transform.rotation * Quaternion.Inverse(offsetRotation) * Quaternion.Inverse(tile.offsetRotation) * Quaternion.Inverse(tile.prefab.transform.rotation)).eulerAngles.GetAxis(s) / 90f))).UValue;
            }
            set
            {
                transform.rotation = Quaternion.Euler(Vector3.zero.Operate((s, a) => value.GetAxis(s) * 90f)) * offsetRotation * tile.offsetRotation * tile.prefab.transform.rotation;
            }
        }
        public virtual int subdivisions
        {
            get
            {
                //apply offset scale (use reciprocol) (and round to nearest subdivision)

                return Mathf.RoundToInt(1f / transform.localScale.Divide(tile.prefab.transform.localScale).Divide(tile.offsetScale).ToList().GetAverage(AverageType.Mode));
            }
            set
            {
                transform.localScale = offsetScale.Multiply(tile.prefab.transform.localScale.Multiply(tile.offsetScale)) / value;
            }
        }

        //public Quaternion offsetRotation
        //{
        //    get
        //    {
        //        TransformTools.Vector3T<int> temp = new TransformTools.Vector3Int(rotation).Operate((s, a) => (a - (int)(Mathf.Floor(a / 4) * 4) * 90))/* * 90*/;

        //        return Quaternion.Euler(temp.x, temp.y, temp.z);
        //    }
        //}
        
        //switch grid to new grid with same position, scale and rotation
        //(may snap position and rotation to new grid)
        public virtual void SwitchGrid (GridOrientation newGrid, bool snap = true)
        {
            if (newGrid.grid == grid.grid) 
            {
                Vector3 originalPosition = position;
                UnityEngine.Vector3Int originalRotation = rotation;
                int originalSubdivisions = subdivisions;

                grid = newGrid;

                //*ORDER OF OFFSETS:
                //  tile.prefab.transform.position
                //  tile.offsetPosition
                //  offsetPosition

                if (!snap)
                {
                    position = newGrid.WorldToGrid(position, subdivisions);

                } 
                else if (snap)
                {

                }
            }
        }

        //gives equivelent gameobject scasle with given scale
        //e.g. Vector3(1, 1, 1) = Vector3(-2, 1, 1)
        //public Vector3 Scale(Vector3 newScale, Quaternion? parentRotation = null)
        //{
        //    newScale *= tile.offsetScale;

        //    if (parentRotation == null)
        //    {
        //        parentRotation = transform.parent.rotation;
        //    }

        //    Quaternion actualRotation = Linking.InverseTransformEuler(transform.rotation, (Quaternion)parentRotation) * Quaternion.Inverse(offsetRotation);

        //    throw new System.NotImplementedException();
        //}
    }
}
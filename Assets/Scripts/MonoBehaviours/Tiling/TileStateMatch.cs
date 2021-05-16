using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;
using REXTools.CustomTransforms;

namespace REXTools.Tiling
{
    [RequireComponent(typeof(CustomPosition), typeof(CustomRotation))]
    public class TileStateMatch : TileState
    {
        public override GridOrientation grid
        {
            get
            {
                return _grid;
            }
            set
            {
                _grid = value;

                if (grid != null)
                {
                    customPosition.SetParent(new TransformObject(() => grid.finalPosition, () => grid.finalRotation, () => Vector3.one * grid.totalScale));
                    customRotation.SetParent(new TransformObject(() => grid.finalPosition, () => grid.finalRotation, () => Vector3.one * grid.totalScale));
                }
            }
        }

        public override Vector3 position
        {
            get
            {
                //apply opposite offset position (with subdivisions)
                return grid.WorldToGrid(customPosition.position - offsetPosition - tile.offsetPosition - tile.prefab.transform.position, subdivisions);
            }
            set
            {
                customPosition.position = grid.GridToWorld(value, subdivisions) + tile.prefab.transform.position + tile.offsetPosition + offsetPosition;
            }
        }
        public override UnityEngine.Vector3Int rotation
        {
            get
            {
                //apply opposite offset rotation (and round)

                return ((TransformTools.Vector3Int)UnityEngine.Vector3Int.zero.Operate((s, a) => Mathf.RoundToInt((customRotation.rotation * Quaternion.Inverse(offsetRotation) * Quaternion.Inverse(tile.offsetRotation) * Quaternion.Inverse(tile.prefab.transform.rotation)).eulerAngles.GetAxis(s) / 90f))).UValue;
            }
            set
            {
                customRotation.rotation = Quaternion.Euler(Vector3.zero.Operate((s, a) => value.GetAxis(s) * 90f)) * offsetRotation * tile.offsetRotation * tile.prefab.transform.rotation;
            }
        }
        public override int subdivisions
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

        //components
        private CustomPosition customPosition;
        private CustomRotation customRotation;

        //public virtual void SwitchGrid(GridOrientation newGrid, bool snap = true)
        //{
        //    if (newGrid.grid == grid.grid)
        //    {
        //        Vector3 originalPosition = position;
        //        UnityEngine.Vector3Int originalRotation = rotation;
        //        int originalSubdivisions = subdivisions;

        //        grid = newGrid;

        //        //*ORDER OF OFFSETS:
        //        //  tile.prefab.transform.position
        //        //  tile.offsetPosition
        //        //  offsetPosition

        //        if (!snap)
        //        {
        //            position = newGrid.WorldToGrid(position, subdivisions);

        //        }
        //        else if (snap)
        //        {

        //        }
        //    }
        //}

        public void Awake()
        {
            customPosition = GetComponent<CustomPosition>();
            customRotation = GetComponent<CustomRotation>();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

namespace REXTools.Tiling
{
    public class TileStateConstant : TileState
    {
        public override Vector3 positionRaw
        {
            get
            {
                //apply opposite offset position (with subdivisions)
                return grid.WorldToGrid(
                    transform.position -
                    offsetPosition,
                    subdivisions
                ) -
                tile.offsetPosition -
                tile.prefab.transform.position;
            }
            set
            {
                transform.position = grid.GridToWorld(
                    value +
                    tile.prefab.transform.position +
                    tile.offsetPosition,
                    subdivisions
                ) +
                offsetPosition;
            }
        } //grid position (factors subdivisions)
        public override Quaternion rotationRaw
        {
            get
            {
                return Linking.InverseTransformEuler(
                    transform.rotation.Subtract(
                    offsetRotation, Space.World),

                    grid.finalRotation
                ).Subtract(
                tile.offsetRotation).Subtract(
                tile.prefab.transform.rotation);
            }
            set
            {
                transform.rotation = Linking.TransformEuler(
                    value.Add(
                    tile.prefab.transform.rotation).Add(
                    tile.offsetRotation),

                    grid.finalRotation
                ).Add(
                offsetRotation, Space.World);
            }
        }

        public override void SwitchGrid(GridOrientation newGrid, bool snapSubdivisions = true, bool snapPosition = true, bool snapRotation = true)
        {
            if (grid != null)
            {
                float originalSubdivisions = subdivisions;
                Vector3 originalPosition = transform.position;
                Quaternion originalRotation = transform.rotation;

                grid = newGrid;

                subdivisionsRaw = originalSubdivisions;
                transform.position = originalPosition;
                transform.rotation = originalRotation;

                if (snapSubdivisions)
                {
                    subdivisionsRaw = Grid.SnapSubdivisions(subdivisionsRaw);
                }
                if (snapPosition)
                {
                    positionRaw = Grid.SnapPosition(positionRaw);
                }
                if (snapRotation)
                {
                    rotationRaw = Grid.SnapRotation(rotationRaw);
                }
            }
        }

        void Awake()
        {

        }
    }
}

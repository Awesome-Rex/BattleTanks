using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.REXCore;
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
                    TransformObject newParent = new TransformObject(
                        () => grid.finalPosition,
                        () => grid.finalRotation,
                        () => Vector3.one * Mathf.Pow(grid.totalScale, 2)
                    );

                    customPosition.SetParent(newParent);
                    customPosition.Switch(Space.Self, Link.Match);

                    customRotation.SetParent(newParent);
                    customRotation.Switch(Space.Self, Link.Match);
                }
            }
        }

        public override Vector3 positionRaw
        {
            get
            {
                //apply opposite offset position (with subdivisions)
                return grid.WorldToGrid(
                    customPosition.position -
                    offsetPosition,
                    subdivisions
                ) -
                tile.offsetPosition -
                tile.prefab.transform.position;
            }
            set
            {
                customPosition.position = grid.GridToWorld(
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
                    customRotation.rotation.Subtract(
                    offsetRotation, Space.World),

                    grid.finalRotation
                ).Subtract(
                tile.offsetRotation).Subtract(
                tile.prefab.transform.rotation);
            }
            set
            {
                customRotation.rotation = Linking.TransformEuler(
                    value.Add(
                    tile.prefab.transform.rotation).Add(
                    tile.offsetRotation),

                    grid.finalRotation
                ).Add(
                offsetRotation, Space.World);
            }
        }
        public override float subdivisionsRaw
        {
            get
            {
                return base.subdivisionsRaw;
            }
            set
            {
                base.subdivisionsRaw = value;

                SetPrevious();
            }
        }

        //variables
        private float previousSubdivisions;

        //components
        private CustomPosition customPosition;
        private CustomRotation customRotation;



        private void SetToTarget()
        {
            subdivisionsRaw = previousSubdivisions;
        }

        private void SetPrevious ()
        {
            previousSubdivisions = subdivisionsRaw;
        }



        public override void SwitchGrid(GridOrientation newGrid, bool snapSubdivisions = true, bool snapPosition = true, bool snapRotation = true)
        {
            if (grid != null)
            {
                float originalSubdivisions = subdivisions;
                Vector3 originalPosition = customPosition.position;
                Quaternion originalRotation = customRotation.rotation;

                grid = newGrid;

                subdivisionsRaw = originalSubdivisions;
                customPosition.position = originalPosition;
                customRotation.rotation = originalRotation;

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

        public void Awake()
        {
            customPosition = GetComponent<CustomPosition>();
            customRotation = GetComponent<CustomRotation>();



            //SetPrevious();
        }

        public void Update()
        {
            if (_ETERNAL.I.counter) {
                SetToTarget();
            }
        }

        public void LateUpdate()
        {
            //SetPrevious();
        }
    }
}
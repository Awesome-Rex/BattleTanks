using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

namespace REXTools.Tiling
{
    [CreateAssetMenu(fileName = "New REX Grid", menuName = "REX/Tiling/Grid", order = 0)]
    public class Grid : ScriptableObject
    {
        public Vector3 size = Vector3.one;
        public Vector3 spacing = Vector3.zero;
        public Vector3 scale = Vector3.one;

        //properties
        public Dictionary<Axis, TileOrigin> center = new Dictionary<Axis, TileOrigin>
        {
            {Axis.X, TileOrigin.Inside},
            {Axis.Y, TileOrigin.Inside},
            {Axis.Z, TileOrigin.Inside}
        };



        //properties
        public bool equal
        {
            get => size.x == size.y && size.x == size.z;
        }
        public bool equalSpacing
        {
            get => spacing.x == spacing.y && spacing.x == spacing.z;
        }

        public Vector3Bool canPivot
        {
            get
            {
                return new Vector3Bool(
                    size.y == size.z,
                    size.x == size.z,
                    size.x == size.y
                );
            }
        }

        public bool spaced
        {
            get => spacing == Vector3.zero;
        }


        public int maxSubdivisions
        {
            get
            {
                int subdivisions = 1;
                bool packed = false;
                
                while (!packed)
                {
                    ((subdivisions - 1) * spacing).OperateBool(size, (s, ASubdivisions, ASize) =>
                    {
                        if (ASubdivisions >= ASize)
                        {
                            return true;
                        } else
                        {
                            return false;
                        }
                    });

                    subdivisions++;
                }

                return subdivisions;
            }
        }
        public Vector3 subdividedTileSize (int subdivisions)
        {
            return size - (spacing * (subdivisions - 1));
        }

        //STILL NEEDS TO FACTOR SPACING
        public Vector3 GridToGridUnspaced(Vector3 position, int subdivisions = 1, int newSubdivisions = 1)
        {
            return position * (subdivisions / newSubdivisions);
        }

        //multiple subdivisions to one subdivision
        public Vector3 GridToOne(Vector3 position, int subdivisions = 1)
        {
            Vector3 newPosition = position;
            newPosition /= subdivisions;

            Vector3 totalSpaces;
            if (subdivisions.Even())
            {
                totalSpaces = Vector3.one / 2f;
                totalSpaces += position.Operate((s, a) => a.SignFloor() - a.SignFloor());
            }
            else
            {
                totalSpaces = position.Operate((s, a) => a.SignFloor());
            }

            Vector3 totalSpacing = spacing.Multiply(totalSpaces);
            newPosition += totalSpacing;

            return newPosition;
        }
        //one subidivion to multiple subdivisions
        public Vector3 OneToGrid (Vector3 position, int subdivisions = 1)
        {
            Vector3 totalSpacing;
        }
        public Vector3 GridToGrid(Vector3 position, int subdivisions = 1, int newSubdivisions = 1)
        {
            //converts 
            //Position => One
            //One => Position
        }

    }
}
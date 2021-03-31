using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

namespace REXTools.Tiling
{
    public enum TileRepeatMethod { Point, Outline }
    public enum TileAppearance { Border, Cross }

    [CreateAssetMenu(fileName = "New REX Grid", menuName = "REX/Tiling/Grid", order = 0)]
    public class Grid : ScriptableObject
    {
        public Vector3 size = Vector3.one;
        public float scale = 1f;

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


        
        public Vector3 subdividedTileSize (int subdivisions)
        {
            //return size - (spacing * (subdivisions - 1));
            return size / subdivisions;
        }


        //multiple subdivisions to one subdivision
        public Vector3 GridToOne(Vector3 position, int subdivisions = 1)
        {
            subdivisions = RMath.ClampMin(subdivisions, 1);

            if (RMath.Odd(subdivisions))
            {
                return position / subdivisions;
            } 
            else 
            {
                return (position / subdivisions) + (Vector3.one * (-0.25f) );
            }
        }
        //one subdivision to multiple subdivisions
        public Vector3 OneToGrid(Vector3 position, int subdivisions = 1)
        {
            subdivisions = RMath.ClampMin(subdivisions, 1);

            if (RMath.Odd(subdivisions)) {
                return position * subdivisions;
            } 
            else
            {
                return (position + (Vector3.one * 0.25f)) * subdivisions;
            }
        }
        
        
        public Vector3 GridToGrid(Vector3 position, int subdivisions = 1, int newSubdivisions = 1)
        {
            //converts 
            //Position => One
            //One => Position

            subdivisions = RMath.ClampMin(subdivisions, 1);
            newSubdivisions = RMath.ClampMin(newSubdivisions, 1);

            return OneToGrid(GridToOne(position, subdivisions), newSubdivisions);
        }
        
        //Takes subdivided position
        //returns position in "one's" space
        //public Vector3 SubdivisionRound(Vector3 position, int subdivisions, Vector3 increment, Vector3 offset)
        //{
        //    return GridToOne(position.CustomRound(increment, offset), subdivisions);
        //}
    }
}
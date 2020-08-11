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

        public TileOrigin centerPoint = TileOrigin.Inside;
        public bool centerOnSpace = false;

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

        public Dictionary<Axis, bool> canPivot
        {
            get
            {
                return new Dictionary<Axis, bool> {
                    {Axis.X, size.y == size.z},
                    {Axis.Y, size.x == size.z},
                    {Axis.Z, size.x == size.y}
                };
            }
        }

        public bool spaced
        {
            get => spacing == Vector3.zero;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace REXTools.Tiling
{
    [CreateAssetMenu(fileName = "New REX Grid", menuName = "REX/Tiling/Grid", order = 0)]
    public class REXGrid : ScriptableObject
    {
        public Vector3 size = Vector3.one;
        public Vector3 spacing = Vector3.zero;
    }
}
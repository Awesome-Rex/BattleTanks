using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.TransformTools
{
    [System.Serializable]
    public struct AxisApplied
    {
        public Axis axis;
        public float units;

        public Space space;

        /*[HideInInspector]
        public SpaceVariety variety;*/

        public AxisApplied(Axis axis, float units,/* SpaceVariety variety = SpaceVariety.OneSided, */Space space = Space.Self)
        {
            this.axis = axis;
            this.units = units;

            this.space = space;
            //this.variety = variety;
        }
    }
}
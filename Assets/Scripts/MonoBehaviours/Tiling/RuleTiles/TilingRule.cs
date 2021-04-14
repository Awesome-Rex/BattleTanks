using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

namespace REXTools.Tiling
{
    public enum RuleBehaviour { DontCare, This, NotThis}

    [System.Serializable]
    public class TilingRule : SignAdjacency<RuleBehaviour>
    {
        
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;
using REXTools.Tiling;

public class PropertyDrawerTESTING : MonoBehaviour
{
    [HideInInspector] public TilingRule rule = new TilingRule();
    [HideInInspector] public TilingRule rule2 = new TilingRule();
    [HideInInspector] public TilingRule rule3 = new TilingRule();

    [ContextMenu("Context Menu Func")]
    public void ContextMenuFunc ()
    {

    }
}

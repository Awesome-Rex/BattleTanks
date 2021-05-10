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

    [HideInInspector] public TransformObjectConstant transformObjectConstant;
    [HideInInspector] public TransformObjectComponent transformObjectComponent;
    [HideInInspector] public TransformObjectReference transformObjectReference;

    [Space]
    public TransformObject defaultConstant = new TransformObjectConstant();
    
    [ContextMenu("Context Menu Func")]
    public void ContextMenuFunc ()
    {
        Debug.Log(defaultConstant != null);

    }
}

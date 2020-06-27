﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TransformTools;

public class IgnoreParentRotation : IgnoreLink
{
    //private previous'
    private Quaternion parentRot;

    public override void MoveToTarget()
    {
        if (enabled)
        {
            transform.rotation *= Quaternion.Inverse((transform.parent.rotation * Quaternion.Inverse(parentRot)));
        }
    }

    public override void SetPrevious()
    {
        parentRot = transform.parent.rotation;
    }

    private void Start() { }
}

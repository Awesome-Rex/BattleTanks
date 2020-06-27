using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TransformControl;

public class IgnoreParentRotation : IgnoreLink
{
    //private previous'
    private Quaternion parentRotation;

    public override void MoveToTarget()
    {
        if (enabled)
        {
            transform.rotation *= Quaternion.Inverse((transform.parent.rotation * Quaternion.Inverse(parentRotation)));
        }
    }

    public override void SetPrevious()
    {
        parentRotation = transform.parent.rotation;
    }

    private void Start() { }
}

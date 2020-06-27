using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TransformControl;

public class IgnoreParentPosition : IgnoreLink
{
    public bool factorScale;

    //private previous'
    private Vector3 localPosition;

    private Vector3 parentPosition;
    private Quaternion parentRotation;
    private Vector3 parentScale;


    public override void MoveToTarget()
    {
        if (enabled) {
            transform.position += -((transform.parent.TransformPoint(localPosition) - transform.parent.position) - (Linking.TransformPoint(localPosition, parentPosition, parentRotation, parentScale) - parentPosition));
            transform.position += -(transform.parent.position - parentPosition);
            if (!factorScale) {
                transform.localPosition =
                    Vectors.DivideVector3(transform.localPosition, Vectors.DivideVector3(parentScale, transform.parent.localScale));
            }
        }
    }

    public override void SetPrevious ()
    {
        localPosition = transform.localPosition;

        parentPosition = transform.parent.position;
        parentRotation = transform.parent.rotation;
        parentScale = transform.parent.localScale;
    }

    private void Start() { }
}

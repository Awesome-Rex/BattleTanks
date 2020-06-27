using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TransformTools;

public class IgnoreParentPosition : IgnoreLink
{
    public bool factorScale;

    //private previous'
    private Vector3 localPosition;

    private Vector3 parentPos;
    private Quaternion parentRot;
    private Vector3 parentScale;


    public override void MoveToTarget()
    {
        if (enabled) {
            transform.position += -((transform.parent.TransformPoint(localPosition) - transform.parent.position) - (Linking.TransformPoint(localPosition, parentPos, parentRot, parentScale) - parentPos));
            transform.position += -(transform.parent.position - parentPos);
            if (!factorScale) {
                transform.localPosition =
                    Vectors.DivideVector3(transform.localPosition, Vectors.DivideVector3(parentScale, transform.parent.localScale));
            }
        }
    }

    public override void SetPrevious ()
    {
        localPosition = transform.localPosition;

        parentPos = transform.parent.position;
        parentRot = transform.parent.rotation;
        parentScale = transform.parent.localScale;
    }

    private void Start() { }
}

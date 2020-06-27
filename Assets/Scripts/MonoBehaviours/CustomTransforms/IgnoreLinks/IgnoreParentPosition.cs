using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreParentPosition : MonoBehaviour
{
    //previous
    //Vector3 position;
    Vector3 localPosition;

    Vector3 parentPosition;
    Quaternion parentRotation;
    Vector3 parentScale;

    void MoveToTarget ()
    {
        transform.position += -(transform.parent.position - parentPosition);
        //SAVE FOR IGNORE PARENT ROTATION -> transform.rotation *= Quaternion.Inverse((transform.parent.rotation * Quaternion.Inverse(parentRotation)));

        //transform.position += CustomTransform.TransformPoint()transform.parent.TransformPoint(localPosition);
        
        //transform.localPosition = AxisOrder.MultiplyVector3(transform.localPosition, AxisOrder.DivideVector3(transform.parent.localScale, parentScale));
    }

    void SetPrevious ()
    {
       // position = transform.position;
        localPosition = transform.localPosition;

        parentPosition = transform.parent.position;
        parentRotation = transform.parent.rotation;
        parentScale = transform.parent.localScale;
    }

    private void Awake()
    {
        _ETERNAL.R.lateRecorder.lateCallbackF += SetPrevious;
        _ETERNAL.R.earlyRecorder.earlyCallbackF += MoveToTarget;
    }
}

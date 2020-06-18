using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRotation : MonoBehaviour
{
    public Space space = Space.Self;

    public bool follow = false;
    public Transition transition;

    //global
    public Quaternion rotation;

    //local
    public Transform parent;

    public Link link = Link.Offset;

    public Vector3 localOffset;
    public Vector3 globalOffset;

    //previous
    private Quaternion previous;

    [ContextMenu("Apply to target")]
    public void ApplyToTarget()
    {
        transform.rotation = GetTarget();
    }

    public Quaternion GetTarget()
    {
        Quaternion target = Quaternion.Euler(Vector3.zero);

        if (space == Space.World)
        {
            target = rotation;
        }
        else if (space == Space.Self)
        {
            if (link == Link.Offset)
            {
                Quaternion temp = transform.rotation;

                transform.rotation = parent.rotation;
                transform.Rotate(localOffset, Space.Self);
                transform.Rotate(globalOffset, Space.World);

                target = transform.rotation;
                transform.rotation = temp;
            } else if (link == Link.Match)
            {
                target = parent.rotation * previous;
            }
        }

        return target;
    }

    private void Awake()
    {
        previous = Quaternion.Inverse(parent.rotation) * transform.rotation;

        _ETERNAL.r.lateRecorder.callback += () => previous = Quaternion.Inverse(parent.rotation) * transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!follow || link == Link.Match)
        {
            transform.rotation = GetTarget();
        }
        else
        {
            if (transition.type == Curve.Interpolate)
            {
                transform.rotation = transition.MoveTowards(transform.rotation, GetTarget());
            } else if (transition.type == Curve.Custom)
            {

            }
        }
    }
}

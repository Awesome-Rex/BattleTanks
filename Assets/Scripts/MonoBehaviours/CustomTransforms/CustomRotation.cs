using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRotation : CustomTransformLinks<Quaternion>
{
    [ContextMenu("Apply to target")]
    public override void ApplyToTarget()
    {
        if (space == Space.Self && link == Link.Match)
        {
            Debug.LogWarning("Cannot apply to target position if link is set to \"match!\"", gameObject);
            return;
        }

        transform.rotation = GetTarget();
    }

    public override void SetTarget ()
    {
        if (enabled) {
            if (!follow || link == Link.Match)
            {
                transform.rotation = GetTarget();
            }
            else
            {
                if (transition.type == Curve.Interpolate)
                {
                    transform.rotation = transition.MoveTowards(transform.rotation, GetTarget());
                }
                else if (transition.type == Curve.Custom)
                {

                }
            }
        }
    }

    public override Quaternion GetTarget()
    {
        Quaternion target = Quaternion.Euler(Vector3.zero);

        if (space == Space.World)
        {
            target = value;
        }
        else if (space == Space.Self)
        {
            if (link == Link.Offset)
            {
                Quaternion temp = transform.rotation;

                transform.rotation = parent.rotation;
                transform.Rotate(value.eulerAngles, Space.Self);
                transform.Rotate(globalOffset.eulerAngles, Space.World);

                target = transform.rotation;
                transform.rotation = temp;
            } else if (link == Link.Match)
            {
                target = parent.rotation * previous;
            }
        }

        return target;
    }

    public override void SetPrevious ()
    {
        previous = Quaternion.Inverse(parent.rotation) * transform.rotation;
    }

    protected override void Awake()
    {
        SetPrevious();

        _ETERNAL.r.lateRecorder.callback += SetPrevious;
        _ETERNAL.r.earlyRecorder.callback += SetTarget;
    }

    protected override void OnDestroy()
    {
        _ETERNAL.r.lateRecorder.callback -= SetPrevious;
        _ETERNAL.r.earlyRecorder.callback -= SetTarget;
    }

    private void Start()
    {
        
    }
}

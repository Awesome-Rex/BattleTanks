using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;

public class CustomPosition : CustomTransformLinks<Vector3>
{

    [ContextMenu("Apply to target")]
    public override void ApplyToTarget ()
    {
        if (space == Space.Self && link == Link.Match)
        {
            Debug.LogWarning("Cannot apply to target position if link is set to \"match!\"", gameObject);
            return;
        }

        transform.position = GetTarget();
    }

    public override void SetTarget ()
    {
        if (enabled) {
            if (!follow || link == Link.Match)
            {
                transform.position = GetTarget();
            }
            else
            {
                if (transition.type == Curve.Interpolate)
                {
                    transform.position = transition.MoveTowards(transform.position, GetTarget());
                }
                else if (transition.type == Curve.Custom)
                {

                }
            }
        }
    }

    public override Vector3 GetTarget()
    {
        Vector3 target = Vector3.zero;

        if (space == Space.World)
        {
            target = value;
        }
        else if (space == Space.Self)
        {
            if (link == Link.Offset) {
                target = parent.position + parent.TransformDirection(value) + globalOffset;
            } else if (link == Link.Match)
            {
                target = parent.position + parent.TransformDirection(previous);
            }
        }

        return target;
    }


    public override void SetPrevious ()
    {
        previous = parent.InverseTransformPoint(transform.position);
    }

    private void Start()
    {
        
    }
}

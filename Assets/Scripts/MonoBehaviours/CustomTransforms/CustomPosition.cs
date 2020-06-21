using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;

public class CustomPosition : CustomTransformLinks<Vector3>
{
    public Vector3 position
    {
        get 
        {
            return GetPosition(Space.World);
        }
        set
        {
            SetPosition(value, Space.World);
        }
    }
    public Vector3 localPosition
    {
        get
        {
            return GetPosition(Space.Self);
        }
        set
        {
            SetPosition(value, Space.Self);
        }
    }

    [ContextMenu("Set to target")]
    public override void SetToTarget ()
    {
        if (space == Space.Self && link == Link.Match)
        {
            Debug.LogWarning("Cannot apply to target position if link is set to \"match!\"", gameObject);
            return;
        }

        transform.position = GetTarget();
    }

    public override void MoveToTarget ()
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
                    //+++++still have to add curves!
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
                //target = parent.position + parent.TransformDirection(value) + offset;

                //STILL HAVE TO FIX +++++++++++OFFSET
            } else if (link == Link.Match)
            {
                target = parent.position + parent.TransformDirection(previous);
            }
        }

        return target;
    }

    public Vector3 Translate(Vector3 translation, Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self) {
            return transform.position + parent.TransformPoint(translation); //WORKS!
        } else
        {
            return transform.position + translation; //WORKS!
        }
    }

    public Vector3 SetPosition (Vector3 position, Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self)
        {
            return parent.position + parent.TransformPoint(position); //WORKS!
        } else
        {
            return transform.position; //WORKS!
        }
    }

    public Vector3 GetPosition (Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self)
        {
            return parent.InverseTransformPoint(transform.position); //WORKS!
        }
        else
        {
            return transform.position; //WORKS!
        }
    }

    public override void SetPrevious ()
    {
        previous = parent.InverseTransformPoint(transform.position);
    }

    private void Start()
    {
        
    }
}

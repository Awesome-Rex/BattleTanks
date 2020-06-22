using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
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
            transform.position = SetPosition(value, Space.World);
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
            transform.position = SetPosition(value, Space.Self);
        }
    }

    public bool factorScale = true;
    public float offsetScale = 1f;

    private Vector3 previousDirection;

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
        target = GetTarget();

        if (enabled) {
            if (!follow || link == Link.Match)
            {
                transform.position = target;
            }
            else
            {
                if (transition.type == Curve.Linear)
                {
                    transform.position = transition.MoveTowards(transform.position, target);
                } else if (transition.type == Curve.Interpolate)
                {
                    transform.position = transition.MoveTowards(transform.position, target);
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
                if (factorScale) {
                    target = parent.TransformPoint(value * offsetScale); //WORKS!
                } else
                {
                    target = parent.position + parent.TransformDirection(value); //WORKS!
                }

                target = offset.ApplyPosition(this, target);
            } else if (link == Link.Match)
            {
                if (factorScale) {
                    target = parent.TransformPoint(previous * offsetScale); //WORKS!
                } else
                {
                    target = parent.TransformPoint(AxisOrder.DivideVector3(previousDirection, parent.localScale)); //WORKS!
                }
            }
        }

        return target;
    }

    public Vector3 Translate(Vector3 translation, Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self) {
            if (factorScale)
            {
                return transform.position + (parent.TransformPoint(translation * offsetScale) - parent.position); //WORKS!
            }
            else
            {
                return AxisOrder.DivideVector3(parent.TransformPoint(transform.position + translation), parent.localScale); //WORKS!
            }
        } else
        {
            return transform.position + translation; //WORKS!
        }
    }

    public Vector3 Translate(Vector3 from, Vector3 translation, Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self)
        {
            if (factorScale)
            {
                return from + (parent.TransformPoint(translation * offsetScale) - parent.position); //WORKS!
            }
            else
            {
                return AxisOrder.DivideVector3(parent.TransformPoint(from + translation), parent.localScale); //WORKS!
            }
        }
        else
        {
            return from + translation; //WORKS!
        }
    }

    public Vector3 SetPosition (Vector3 position, Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self)
        {
            if (factorScale) {
                return parent.TransformPoint(position * offsetScale); //WORKS!
            } else
            {
                return AxisOrder.DivideVector3(parent.TransformPoint(position), parent.localScale); //WORKS!
            }
        } else
        {
            return position; //WORKS!
        }
    }

    public Vector3 GetPosition (Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self)
        {
            if (factorScale)
            {
                if (offsetScale != 0f) //ALL WORKS!
                {
                    return parent.InverseTransformPoint(transform.position) / offsetScale;
                } else
                {
                    return Vector3.zero;
                }
            }
            else
            {
                return AxisOrder.MultiplyVector3(parent.InverseTransformPoint(transform.position), parent.localScale); //WORKS
            }
        }
        else
        {
            return transform.position; //WORKS!
        }
    }

    public override void SetPrevious () //WORKS!
    {
        if (factorScale)
        {
            if (offsetScale != 0f)
            {
                previous = parent.InverseTransformPoint(target) / offsetScale;
                previousDirection = AxisOrder.MultiplyVector3(parent.InverseTransformPoint(target), parent.localScale / offsetScale); //for no scale
            }
            else
            {
                previous = Vector3.zero;
            }
        }
        else
        {
            previous = parent.InverseTransformPoint(target);
            previousDirection = AxisOrder.MultiplyVector3(parent.InverseTransformPoint(target), parent.localScale);
        }
    }

    private void Start()
    {
        
    }
}

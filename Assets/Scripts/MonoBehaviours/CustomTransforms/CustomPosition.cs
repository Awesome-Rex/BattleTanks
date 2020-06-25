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
            operationalPosition = SetPosition(value, Space.World);
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
            operationalPosition = SetPosition(value, Space.Self);
        }
    }

    public bool factorScale = true;
    public float offsetScale = 1f;

    private Vector3 previousDirection;


    private Vector3 operationalPosition
    {
        get
        {
            if (rigidbody != null)
            {
                return rigidbody.position;
            }
            else
            {
                return transform.position;
            }
        }
        set
        {
            if (rigidbody != null)
            {
                rigidbody.position = value;
            }
            else
            {
                transform.position = value;
            }
        }
    }
    //
    private new Rigidbody rigidbody;


    //check if even or odd
    //private int counter = 0;

    [ContextMenu("Set to target")]
    public override void SetToTarget ()
    {
        if (space == Space.Self && link == Link.Match)
        {
            Debug.LogWarning("Cannot apply to target position if link is set to \"match!\"", gameObject);
            return;
        }

        operationalPosition = GetTarget();
    }


    private bool counter;
    public override void MoveToTarget ()
    {
        target = GetTarget();

        if (enabled) {
            if (counter) {
                if (!follow || link == Link.Match)
                {
                    operationalPosition = target;
                }
                else
                {
                    if (transition.type == Curve.Linear)
                    {
                        operationalPosition = transition.MoveTowards(operationalPosition, target);
                    }
                    else if (transition.type == Curve.Interpolate)
                    {
                        operationalPosition = transition.MoveTowards(operationalPosition, target);
                    }
                    else if (transition.type == Curve.Custom)
                    {
                        //+++++still have to add curves!
                    }
                }
            }

            counter = !counter;
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
                return operationalPosition + (parent.TransformPoint(translation * offsetScale) - parent.position); //WORKS!
            }
            else
            {
                return AxisOrder.DivideVector3(parent.TransformPoint(operationalPosition + translation), parent.localScale); //WORKS!
            }
        } else
        {
            return operationalPosition + translation; //WORKS!
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
                    return parent.InverseTransformPoint(operationalPosition) / offsetScale;
                } else
                {
                    return Vector3.zero;
                }
            }
            else
            {
                return AxisOrder.MultiplyVector3(parent.InverseTransformPoint(operationalPosition), parent.localScale); //WORKS
            }
        }
        else
        {
            return operationalPosition; //WORKS!
        }
    }

    public override void SetPrevious () //WORKS!
    {
        if (factorScale)
        {
            if (offsetScale != 0f)
            {
                previous = parent.InverseTransformPoint(operationalPosition) / offsetScale;
                previousDirection = AxisOrder.MultiplyVector3(parent.InverseTransformPoint(operationalPosition), parent.localScale / offsetScale); //for no scale
            }
            else
            {
                previous = Vector3.zero;
            }
        }
        else
        {
            previous = parent.InverseTransformPoint(operationalPosition);
            previousDirection = AxisOrder.MultiplyVector3(parent.InverseTransformPoint(operationalPosition), parent.localScale);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        
    }
}

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
    //private Vector3 previousPosition;
    
    //https://answers.unity.com/questions/124486/need-equivalent-of-transforminversetransformpoint.html
    private Vector3 parentPos;
    private Quaternion parentRot; //USE THE STUFF HERE 
    private Vector3 parentScale;
    
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
            /*if (rigidbody != null)
            {
                rigidbody.position = value;
            }
            else
            {*/
                transform.position = value;
            //}
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

        operationalPosition = GetTarget();
    }


    private bool counter;
    public override void MoveToTarget ()
    {
        target = GetTarget();

        if (enabled)
        {
            if (space == Space.World)
            {
                operationalPosition = target;
            }
            else if (space == Space.Self)
            {
                if (link == Link.Offset)
                {
                    if (!follow)
                    {
                        operationalPosition = target;
                    }
                    else
                    {
                        operationalPosition = transition.MoveTowards(operationalPosition, target);
                    }
                }
                else if (link == Link.Match)
                {
                    if (counter)
                    {
                        /*Vector3 local = Vector3.Scale(
                            AxisOrder.DivideVector3(Vector3.one, parentScale),
                            (Quaternion.Inverse(parentRot) * (operationalPosition - parentPos))
                        );*/

                        Vector3 local = InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale);

                        if (
                            !float.IsNaN(local.x) && 
                            !float.IsNaN(local.y) && 
                            !float.IsNaN(local.z)) {
                            operationalPosition = parent.TransformPoint(local);
                        }

                        //previousPosition = operationalPosition;
                        parentPos = parent.position;
                        parentRot = parent.rotation;
                        parentScale = parent.localScale;
                    }
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
            if (link == Link.Offset)
            {
                if (factorScale)
                {
                    target = TransformPoint(value * offsetScale, parentPos, parentRot, parentScale); //WORKS!
                }
                else
                {
                    target = TransformPoint(value, parentPos, parentRot); ;
                }

                target = offset.ApplyPosition(this, target);
            }
            else if (link == Link.Match)
            {
                if (factorScale)
                {
                    target = TransformPoint(previous * offsetScale, parentPos, parentRot, parentScale); //WORKS!
                }
                else
                {
                    target = TransformPoint(AxisOrder.DivideVector3(previousDirection, parentScale), parentPos, parentRot, parentScale); //WORKS!
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
                return operationalPosition + (TransformPoint(translation * offsetScale, parentPos, parentRot, parentScale) - parentPos); //WORKS!
            }
            else
            {
                return AxisOrder.DivideVector3(TransformPoint(operationalPosition + translation, parentPos, parentRot, parentScale), parent.localScale); //WORKS!
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
                return from + (TransformPoint(translation * offsetScale, parentPos, parentRot, parentScale) - parent.position); //WORKS!
            }
            else
            {
                return AxisOrder.DivideVector3(TransformPoint(from + translation, parentPos, parentRot, parentScale), parentScale); //WORKS!
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
                return TransformPoint(position * offsetScale, parentPos, parentRot, parentScale); //WORKS!
            } else
            {
                return AxisOrder.DivideVector3(TransformPoint(position, parentPos, parentRot, parentScale), parent.localScale); //WORKS!
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
                    return InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale) / offsetScale;
                } else
                {
                    return Vector3.zero;
                }
            }
            else
            {
                return AxisOrder.MultiplyVector3(InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale), parent.localScale); //WORKS
            }
        }
        else
        {
            return operationalPosition; //WORKS!
        }
    }

    public override void SetPrevious() //WORKS!
    {
        if (factorScale)
        {
            if (offsetScale != 0f)
            {
                previous = InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale) / offsetScale;
                previousDirection = AxisOrder.MultiplyVector3(InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale), parentScale / offsetScale); //for no scale
            }
            else { previous = Vector3.zero; }
        }
        else
        {
            previous = InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale);
            previousDirection = AxisOrder.MultiplyVector3(InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale), parentScale);
        }

        counter = !counter;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(operationalPosition, parent.position);
    }

    protected override void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        base.Awake();

        _ETERNAL.r.earlyRecorder.callbackF -= SetPrevious;
    }

    private void Start() { }
}

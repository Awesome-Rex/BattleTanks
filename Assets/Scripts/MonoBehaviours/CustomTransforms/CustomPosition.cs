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
            modifiable = SetPosition(value, Space.World);
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
            modifiable = SetPosition(value, Space.Self);
        }
    }

    public bool factorScale = true;
    public float offsetScale = 1f;

    private Vector3 previousDirection;
    private Vector3 previousPosition;


    //https://answers.unity.com/questions/124486/need-equivalent-of-transforminversetransformpoint.html
    private Vector3 previousParentPosition;
    private Quaternion previousParentRotation; //USE THE STUFF HERE 
    private Vector3 previousParentScale;
    
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
                modifiable = target;
            }
            else if (space == Space.Self)
            {
                if (link == Link.Offset)
                {
                    if (!follow)
                    {
                        modifiable = target;
                    }
                    else
                    {
                        modifiable = transition.MoveTowards(modifiable, target);
                    }
                }
                else if (link == Link.Match)
                {
                    modifiable = target;
                    
                    if (counter)
                    {
                        //operationalPosition = 

                        Vector3 local = Vector3.Scale(
                            AxisOrder.DivideVector3(Vector3.one, previousParentScale),
                            (Quaternion.Inverse(previousParentRotation) * (operationalPosition - previousParentPosition))
                        );

                        if (
                            !float.IsNaN(local.x) && 
                            !float.IsNaN(local.y) && 
                            !float.IsNaN(local.z)) {
                            operationalPosition = parent.TransformPoint(local);
                        }

                        //operationalPosition += (modifiable - operationalPosition);
                    }
                    else if (!counter)
                    {
                        
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
                    target = parent.TransformPoint(value * offsetScale); //WORKS!
                }
                else
                {
                    target = parent.position + parent.TransformDirection(value); //WORKS!
                }

                target = offset.ApplyPosition(this, target);
            }
            else if (link == Link.Match)
            {
                if (factorScale)
                {
                    target = parent.TransformPoint(previous * offsetScale); //WORKS!
                }
                else
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
                return modifiable + (parent.TransformPoint(translation * offsetScale) - parent.position); //WORKS!
            }
            else
            {
                return AxisOrder.DivideVector3(parent.TransformPoint(modifiable + translation), parent.localScale); //WORKS!
            }
        } else
        {
            return modifiable + translation; //WORKS!
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
                    return parent.InverseTransformPoint(modifiable) / offsetScale;
                } else
                {
                    return Vector3.zero;
                }
            }
            else
            {
                return AxisOrder.MultiplyVector3(parent.InverseTransformPoint(modifiable), parent.localScale); //WORKS
            }
        }
        else
        {
            return modifiable; //WORKS!
        }
    }

    public override void SetPrevious () //WORKS!
    {
        //if (counter) {
            if (factorScale)
            {
                if (offsetScale != 0f)
                {
                    previous = parent.InverseTransformPoint(modifiable) / offsetScale;
                    previousDirection = AxisOrder.MultiplyVector3(parent.InverseTransformPoint(modifiable), parent.localScale / offsetScale); //for no scale
                } else { previous = Vector3.zero; }
            }
            else
            {
                previous = parent.InverseTransformPoint(modifiable);
                previousDirection = AxisOrder.MultiplyVector3(parent.InverseTransformPoint(modifiable), parent.localScale);
            }
        //}

        if (counter) {
            previousPosition = operationalPosition;
            previousParentPosition = parent.position;
            previousParentRotation = parent.rotation;
            previousParentScale = parent.localScale;

            //previousParentTransform = new Transform();
        }

        counter = !counter;
        //counter = !counter;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(modifiable, 0.5f);
        Gizmos.DrawLine(operationalPosition, parent.position);
    }

    protected override void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        modifiable = operationalPosition;

        base.Awake();

        _ETERNAL.r.earlyRecorder.callbackF -= SetPrevious;

        //_ETERNAL.r.earlyRecorder.lateCallbackF += SetPrevious;
    }

    private void Start()
    {
        
    }
}

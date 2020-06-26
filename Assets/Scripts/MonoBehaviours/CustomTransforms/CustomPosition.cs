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
                    

                    //modifiable = target;

                    /*operationalPosition += (modifiable - previousPosition);

                    modifiable = operationalPosition;*/

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

                        previousPosition = operationalPosition;
                        previousParentPosition = parent.position;
                        previousParentRotation = parent.rotation;
                        previousParentScale = parent.localScale;

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
                    target = TransformPoint(value * offsetScale, previousParentPosition, previousParentRotation, previousParentScale); //WORKS!
                }
                else
                {
                    //target = parent.position + parent.TransformDirection(value); //WORKS!
                    target = TransformPoint(value, previousParentPosition, previousParentRotation); ;
                }

                target = offset.ApplyPosition(this, target);
            }
            else if (link == Link.Match)
            {
                if (factorScale)
                {
                    target = TransformPoint(previous * offsetScale, previousParentPosition, previousParentRotation, previousParentScale); //WORKS!
                }
                else
                {
                    target = TransformPoint(AxisOrder.DivideVector3(previousDirection, previousParentScale), previousParentPosition, previousParentRotation, previousParentScale); //WORKS!
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
                return operationalPosition + (TransformPoint(translation * offsetScale, previousParentPosition, previousParentRotation, previousParentScale) - previousParentPosition); //WORKS!
            }
            else
            {
                return AxisOrder.DivideVector3(TransformPoint(operationalPosition + translation, previousParentPosition, previousParentRotation, previousParentScale), parent.localScale); //WORKS!
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
                return from + (TransformPoint(translation * offsetScale, previousParentPosition, previousParentRotation, previousParentScale) - parent.position); //WORKS!
            }
            else
            {
                return AxisOrder.DivideVector3(TransformPoint(from + translation, previousParentPosition, previousParentRotation, previousParentScale), previousParentScale); //WORKS!
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
                return TransformPoint(position * offsetScale, previousParentPosition, previousParentRotation, previousParentScale); //WORKS!
            } else
            {
                return AxisOrder.DivideVector3(TransformPoint(position, previousParentPosition, previousParentRotation, previousParentScale), parent.localScale); //WORKS!
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
                    return InverseTransformPoint(operationalPosition, previousParentPosition, previousParentRotation, previousParentScale) / offsetScale;
                } else
                {
                    return Vector3.zero;
                }
            }
            else
            {
                return AxisOrder.MultiplyVector3(InverseTransformPoint(operationalPosition, previousParentPosition, previousParentRotation, previousParentScale), parent.localScale); //WORKS
            }
        }
        else
        {
            return operationalPosition; //WORKS!
        }
    }

    public override void SetPrevious () //WORKS!
    {
        //if (counter) {
            if (factorScale)
            {
                if (offsetScale != 0f)
                {
                    previous = InverseTransformPoint(operationalPosition, previousParentPosition, previousParentRotation, previousParentScale) / offsetScale;
                    previousDirection = AxisOrder.MultiplyVector3(InverseTransformPoint(operationalPosition, previousParentPosition, previousParentRotation, previousParentScale), previousParentScale / offsetScale); //for no scale
                } else { previous = Vector3.zero; }
            }
            else
            {
                previous = InverseTransformPoint(operationalPosition, previousParentPosition, previousParentRotation, previousParentScale);
                previousDirection = AxisOrder.MultiplyVector3(InverseTransformPoint(operationalPosition, previousParentPosition, previousParentRotation, previousParentScale), previousParentScale);
            }
        //}

        //if (counter) {
        //previousPosition = operationalPosition;
        //previousParentPosition = parent.position;
        //previousParentRotation = parent.rotation;
        //previousParentScale = parent.localScale;

        ////previousParentTransform = new Transform();
        //}

        counter = !counter;
        //counter = !counter;
    }


    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(operationalPosition, 0.5f);
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

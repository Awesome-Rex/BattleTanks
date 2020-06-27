using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;

using TransformControl;

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
            if (!(space == Space.Self && link == Link.Offset)) {
                if (space == Space.World)
                {
                    this.value = value;
                }
                operationalPosition = SetPosition(value, Space.World);
            }
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
            if (!(space == Space.Self && link == Link.Offset))
            {
                operationalPosition = SetPosition(value, Space.Self);
            }
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
            return transform.position;
        }
        set
        {
            transform.position = value; //////////MAKE IT WORK FOR CHANGING OFFSET POSITION
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


    //private bool counter;
    public override void MoveToTarget ()
    {
        target = GetTarget();

        if (enabled/* && _ETERNAL.R.counter*/)
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
                    if (_ETERNAL.R.counter)
                    {
                        Vector3 local = Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale);

                        /*if (
                            !float.IsNaN(local.x) && 
                            !float.IsNaN(local.y) && 
                            !float.IsNaN(local.z)) {*/
                            operationalPosition = parent.TransformPoint(local);
                        //}
                        
                        parentPos = parent.position;
                        parentRot = parent.rotation;
                        parentScale = parent.localScale;
                    }
                }
            }
            if (_ETERNAL.R.counter)
            {
                parentPos = parent.position;
                parentRot = parent.rotation;
                parentScale = parent.localScale;
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
                    target = Linking.TransformPoint(value * offsetScale, parentPos, parentRot, parentScale); //WORKS!
                }
                else
                {
                    target = Linking.TransformPoint(value, parentPos, parentRot);
                }

                target = offset.ApplyPosition(this, target);
            }
            else if (link == Link.Match)
            {
                Vector3 newTarget;

                if (factorScale)
                {
                    
                    newTarget = Linking.TransformPoint(previous * offsetScale, parentPos, parentRot, parentScale); //WORKS!
                }
                else
                {
                    newTarget = Linking.TransformPoint(Vectors.DivideVector3(previousDirection, parentScale), parentPos, parentRot, parentScale); //WORKS!
                }

                /*if (
                    !float.IsNaN(newTarget.x) &&
                    !float.IsNaN(newTarget.y) &&
                    !float.IsNaN(newTarget.z)
                    )
                {*/
                    target = newTarget;
                //}
            }
        }

        return target;
    }

    public Vector3 Translate(Vector3 translation, Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self) {
            if (factorScale)
            {
                return operationalPosition + (Linking.TransformPoint(translation * offsetScale, parentPos, parentRot, parentScale) - parentPos); //WORKS!
            }
            else
            {
                return Vectors.DivideVector3(Linking.TransformPoint(operationalPosition + translation, parentPos, parentRot, parentScale), parent.localScale); //WORKS!
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
                return from + (Linking.TransformPoint(translation * offsetScale, parentPos, parentRot, parentScale) - parent.position); //WORKS!
            }
            else
            {
                return Vectors.DivideVector3(Linking.TransformPoint(from + translation, parentPos, parentRot, parentScale), parentScale); //WORKS!
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
                return Linking.TransformPoint(position * offsetScale, parentPos, parentRot, parentScale); //WORKS!
            } else
            {
                return Vectors.DivideVector3(Linking.TransformPoint(position, parentPos, parentRot, parentScale), parent.localScale); //WORKS!
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
                    return Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale) / offsetScale;
                } else
                {
                    return Vector3.zero;
                }
            }
            else
            {
                return Vectors.MultiplyVector3(Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale), parent.localScale); //WORKS
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
                previous = Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale) / offsetScale;
                previousDirection = Vectors.MultiplyVector3(Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale), parentScale / offsetScale); //for no scale
            }
            else { previous = Vector3.zero; }
        }
        else
        {
            previous = Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale);
            previousDirection = Vectors.MultiplyVector3(Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale), parentScale);
        }

        //counter = !counter;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(operationalPosition, parent.position);
    }

    protected override void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        base.Awake();

        _ETERNAL.R.earlyRecorder.callbackF -= SetPrevious;


        parentPos = parent.position;
        parentRot = parent.rotation;
        parentScale = parent.localScale;
    }

    private void Start() { }
}

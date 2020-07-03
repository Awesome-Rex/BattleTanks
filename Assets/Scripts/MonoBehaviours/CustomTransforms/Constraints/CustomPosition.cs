using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using TransformTools;

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
    
    public override void SetToTarget ()
    {
        operationalPosition = GetTarget();

        RecordParent();
    }
    
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
                    if ((_ETERNAL.I != null && _ETERNAL.I.counter) || editorApply)
                    {
                        operationalPosition = target;
                    }
                }
            }
            if ((_ETERNAL.I != null && _ETERNAL.I.counter) || editorApply)
            {
                RecordParent();
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

                SetPrevious();

                if (factorScale)
                {
                    //newTarget = Linking.TransformPoint(previous * offsetScale, parentPos, parentRot, parentScale); //WORKS!
                    newTarget = Linking.TransformPoint(previous * offsetScale, parent.position, parent.rotation, parent.localScale);
                }
                else
                {
                    //newTarget = Linking.TransformPoint(previousDirection, parentPos, parentRot);
                    newTarget = Linking.TransformPoint(previousDirection, parent.position, parent.rotation); //++++++++ ATTENTION
                }
                
                target = newTarget;
            }
        }

        return target;
    }

    public override void RecordParent()
    {
        parentPos = parent.position;
        parentRot = parent.rotation;
        parentScale = parent.localScale;
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
                //return Vectors.DivideVector3(Linking.TransformPoint(operationalPosition + translation, parentPos, parentRot, parentScale), parent.localScale); //WORKS!
                return Linking.TransformPoint(operationalPosition + translation, parentPos, parentRot); //WORKS!
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
                //return Vectors.DivideVector3(Linking.TransformPoint(from + translation, parentPos, parentRot, parentScale), parentScale); //WORKS!
                return Linking.TransformPoint(from + translation, parentPos, parentRot);
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
                //return Vectors.DivideVector3(Linking.TransformPoint(position, parentPos, parentRot, parentScale), parent.localScale); //WORKS!
                return Linking.TransformPoint(position, parentPos, parentRot); //WORKS!
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
                //return Vectors.MultiplyVector3(Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale), parent.localScale); //WORKS
                return Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot); //WORKS
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
                //previousDirection = Vectors.MultiplyVector3(Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale), parentScale / offsetScale); //for no scale

                previousDirection = Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot) / offsetScale;
                //previousDirection = Vectors.DivideVector3(Linking.InverseTransformPoint(operationalPosition, parent.position, parent.rotation), parent.localScale) / offsetScale;
            }
            else { previous = Vector3.zero; }
        }
        else
        {
            previous = Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale);
            //previousDirection = Vectors.MultiplyVector3(Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale), parentScale);

            previousDirection = Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot);
            //previousDirection = Vectors.DivideVector3(Linking.InverseTransformPoint(operationalPosition, parent.position, parent.rotation), parent.localScale);
        }
    }

    protected override void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        base.Awake();
    }

    private void Start() { }

#if UNITY_EDITOR
    [CustomEditor(typeof(CustomPosition))]
    public class E : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif
}

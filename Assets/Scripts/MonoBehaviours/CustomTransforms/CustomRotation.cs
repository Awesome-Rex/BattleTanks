using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

[System.Serializable]
public class CustomRotation : CustomTransformLinks<Quaternion>
{
    public Quaternion rotation
    {
        get
        {
            return GetRotation(Space.World);
        }

        set
        {
            transform.rotation = SetRotation(value.eulerAngles, Space.World);
        }
    }
    public Quaternion localRotation
    {
        get
        {
            return GetRotation(Space.Self);
        }
        set
        {
            transform.rotation = SetRotation(value.eulerAngles, Space.Self);
        }
    }

    public Vector3 eulerAngles
    {
        get
        {
            return rotation.eulerAngles;
        }
        set
        {
            rotation = Quaternion.Euler(value);
        }
    }
    public Vector3 localEulerAngles
    {
        get
        {
            return localRotation.eulerAngles;
        }
        set
        {
            localRotation = Quaternion.Euler(value);
        }
    }

    private Quaternion operationalRotation
    {
        get
        {
            if (rigidbody != null)
            {
                return rigidbody.rotation;
            }
            else
            {
                return transform.rotation;
            }
        }
        set
        {
            if (rigidbody != null)
            {
                rigidbody.rotation = value;
            }
            else
            {
                transform.rotation = value;
            }
        }
    }

    //
    private new Rigidbody rigidbody;

    [ContextMenu("Set to target")]
    public override void SetToTarget()
    {
        if (space == Space.Self && link == Link.Match)
        {
            Debug.LogWarning("Cannot apply to target position if link is set to \"match!\"", gameObject);
            return;
        }

        transform.rotation = GetTarget();
    }

    public override void MoveToTarget ()
    {
        target = GetTarget();

        if (enabled) {
            if (!follow || link == Link.Match)
            {
                transform.rotation = target;
            }
            else
            {
                if (transition.type == Curve.Linear)
                {
                    transform.rotation = transition.MoveTowards(transform.rotation, target);
                }
                else if (transition.type == Curve.Interpolate)
                {
                    transform.rotation = transition.MoveTowards(transform.rotation, target);
                }
                else if (transition.type == Curve.Custom)
                {
                    //++++++CURVES
                }
            }
        }
    }

    public override Quaternion GetTarget()
    {
        Quaternion target = new Quaternion();

        if (space == Space.World)
        {
            target = value;
        }
        else if (space == Space.Self)
        {
            if (link == Link.Offset)
            {
                target = parent.rotation * value; //++++++++offset
                target = offset.ApplyRotation(this, target);
            } else if (link == Link.Match)
            {
                target = parent.rotation * previous; //WORKS!
            }
        }

        return target;
    }

    public Quaternion Rotate (Vector3 eulers, Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self)
        {
            return transform.rotation * Quaternion.Euler(eulers); //WORKS!
        } else
        {
            return Quaternion.Euler(eulers) * transform.rotation; //WORKS!
        }
    }

    public Quaternion Rotate(Quaternion from, Vector3 eulers, Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self)
        {
            return from * Quaternion.Euler(eulers); //WORKS!
        }
        else
        {
            return Quaternion.Euler(eulers) * from; //WORKS!
        }
    }

    public Quaternion SetRotation (Vector3 rotation, Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self)
        {
            return parent.rotation * Quaternion.Euler(rotation); //WORKS!
        }
        else 
        {
            return Quaternion.Euler(rotation); //WORKS!
        }
    }

    public Quaternion GetRotation (Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self) {
            return Quaternion.Inverse(parent.rotation) * transform.rotation; //WORKS!
        } else
        {
            return transform.rotation; //WORKS!
        }
    }

    public override void SetPrevious ()
    {
        //previous = Quaternion.Inverse(parent.rotation) * transform.rotation;
        previous = Quaternion.Inverse(parent.rotation) * target;
    }

    protected override void Awake()
    {
        SetPrevious();

        rigidbody = GetComponent<Rigidbody>();

        _ETERNAL.r.lateRecorder.callback += SetPrevious;
        _ETERNAL.r.earlyRecorder.callback += MoveToTarget;
    }

    protected override void OnDestroy()
    {
        _ETERNAL.r.lateRecorder.callback -= SetPrevious;
        _ETERNAL.r.earlyRecorder.callback -= MoveToTarget;
    }

    private void Start()
    {
        
    }

#if UNITY_EDITOR
    //[CustomEditor(typeof(CustomRotation))]
    //public class E : Editor {
    //    private SerializedProperty transitionP;
    //    //private SerializedProperty axisOrder;

    //    private void OnEnable()
    //    {
    //        transitionP = serializedObject.FindProperty("transition");
    //        //axisOrder = serializedObject.FindProperty("axisOrder");
    //    }

    //    public override void OnInspectorGUI()
    //    {
    //        var t = (CustomRotation)target;


    //        EditorGUILayout.LabelField("Space", EditorStyles.boldLabel);
    //        t.space = (Space)EditorGUILayout.EnumPopup(t.space);

    //        EditorGUILayout.Space();
            
    //        if (t.space == Space.Self) t.parent = (Transform)EditorGUILayout.ObjectField("Parent", t.parent, typeof(Transform), true);

    //        t.value = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", t.value.eulerAngles));

    //        EditorGUILayout.Space();

    //        if (t.space == Space.Self)
    //        {
    //            EditorGUILayout.LabelField("Local Space", EditorStyles.boldLabel);

    //            t.link = (Link)EditorGUILayout.EnumPopup("Link", t.link);

    //            if (t.link == Link.Offset)
    //            {
    //                //t.globalOffset = Quaternion.Euler(EditorGUILayout.Vector3Field("Global Offset", t.globalOffset.eulerAngles));
    //                //EditorGUILayout.PropertyField(axisOrder);

    //                //EditorGUILayout.BeginHorizontal();

                    
    //                /*foreach (Axis i in t.axisOrder)
    //                {
    //                }*/
    //            }
    //        }

    //        EditorGUILayout.Space();
            
    //        t.follow = EditorGUILayout.Toggle("Transitioning", t.follow);
    //        if (t.follow)
    //        {
    //            //t.transition = (Transition)EditorGUILayout.ObjectField("Transition", t.transition, typeof(Transition), true);
    //            EditorGUILayout.PropertyField(transitionP);
    //        }
    //    }
    //}
#endif
}

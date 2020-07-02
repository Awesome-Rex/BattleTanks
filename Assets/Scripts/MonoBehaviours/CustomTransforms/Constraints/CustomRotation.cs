using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using TransformTools;

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
            if (!(space == Space.Self && link == Link.Offset))
            {
                if (space == Space.World)
                {
                    this.value = value;
                }
                operationalRotation = SetRotation(value.eulerAngles, Space.World);
            }
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
            if (!(space == Space.Self && link == Link.Offset))
            {
                operationalRotation = SetRotation(value.eulerAngles, Space.Self);
            }
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
            transform.rotation = value;
        }
    }

    public Vector3 up
    {
        get
        {
            return (rotation * Vector3.up).normalized;
        }
        set
        {
            rotation = (Quaternion.LookRotation(value) * Quaternion.Euler(90f, 0f, 0f));
        }
    }
    public Vector3 forward
    {
        get
        {
            return (rotation * Vector3.forward).normalized;
        }
        set
        {
            rotation = Quaternion.LookRotation(value);
        }
    }
    public Vector3 right
    {
        get
        {
            return (rotation * Vector3.right).normalized;
        }
        set
        {
            rotation = (Quaternion.LookRotation(value) * Quaternion.Euler(0f, -90f, 0f));
        }
    }

    private Quaternion parentRot;

    [ContextMenu("Set to target")]
    public override void SetToTarget()
    {
        if (space == Space.Self && link == Link.Match)
        {
            Debug.LogWarning("Cannot apply to target position if link is set to \"match!\"", gameObject);
            return;
        }

        RecordParent();

        operationalRotation = GetTarget();
    }

    //private bool counter;
    public override void MoveToTarget()
    {
        target = GetTarget();

        if (enabled)
        {
            if (space == Space.World)
            {
                operationalRotation = target;
            }
            else if (space == Space.Self)
            {
                if (link == Link.Offset)
                {
                    if (!follow)
                    {
                        operationalRotation = target;
                    } else {
                        operationalRotation = transition.MoveTowards(operationalRotation, target);
                    }
                }
                else if (link == Link.Match)
                {
                    if (_ETERNAL.I.counter)
                    {
                        /*Quaternion local = Linking.InverseTransformEuler(operationalRotation, parentRot);

                        operationalRotation = Linking.TransformEuler(local, parent.rotation);*/

                        operationalRotation = target;
                    }
                }
            }

            if (_ETERNAL.I.counter)
            {
                RecordParent();
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
                target = parentRot * value; //++++++++offset
                target = offset.ApplyRotation(this, target);
            } else if (link == Link.Match)
            {
                SetPrevious();

                //target = parentRot * previous; //WORKS!
                target = Linking.TransformEuler(previous, parent.rotation);
            }
        }

        return target;
    }

    public override void RecordParent()
    {
        parentRot = parent.rotation;
    }

    public Quaternion Rotate (Vector3 eulers, Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self)
        {
            return operationalRotation * Quaternion.Euler(eulers); //WORKS!
        } else
        {
            return Quaternion.Euler(eulers) * operationalRotation; //WORKS!
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
            return parentRot * Quaternion.Euler(rotation); //WORKS!
        }
        else 
        {
            return Quaternion.Euler(rotation); //WORKS!
        }
    }

    public Quaternion GetRotation (Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self) {
            return Quaternion.Inverse(parentRot) * operationalRotation; //WORKS!
        } else
        {
            return operationalRotation; //WORKS!
        }
    }

    public override void SetPrevious ()
    {
        //previous = Quaternion.Inverse(parentRot) * operationalRotation;
        previous = Linking.InverseTransformEuler(operationalRotation, parentRot);
        //counter = !counter;
    }

    protected override void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        base.Awake();

        RecordParent();
    }

    private void Start() { }

#if UNITY_EDITOR
    [CustomEditor(typeof(CustomRotation))]
    public class E : EditorPRO<CustomRotation>
    {
        private SerializedProperty transitionP;
        private SerializedProperty axisOrder;

        protected override void DeclareProperties ()
        {
            AddProperty("transition");
            AddProperty("offset");
            AddProperty("link");
        }

        public override void OnInspectorGUI()
        {
            OnInspectorGUIPRO(() => {
                //EditorGUILayout.LabelField("Space", EditorStyles.boldLabel);
                target.space = (Space)EditorGUILayout.EnumPopup(target.space);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Rotation", EditorStyles.boldLabel);
                if (target.space == Space.Self)
                {
                    target.parent = (Transform)EditorGUILayout.ObjectField("Parent", target.parent, typeof(Transform), true);
                }
                if (!(target.space == Space.Self && target.link == Link.Match)) {
                    
                    target.value = Quaternion.Euler(EditorGUILayout.Vector3Field("Value", target.value.eulerAngles));
                }

                EditorGUILayout.Space();

                if (target.space == Space.Self)
                {
                    EditorGUILayout.LabelField("Local", EditorStyles.boldLabel);

                    EditorGUILayout.PropertyField(FindProperty("link"));

                    if (target.link == Link.Offset)
                    {
                        EditorGUILayout.PropertyField(FindProperty("offset"));
                    }
                }

                EditorGUILayout.Space();

                if (target.space == Space.Self && target.link == Link.Offset) {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Transition", EditorStyles.boldLabel);
                    
                    target.follow = EditorGUILayout.Toggle(string.Empty, target.follow);

                    EditorGUILayout.EndHorizontal();
                    if (target.follow)
                    {
                        EditorGUILayout.PropertyField(FindProperty("transition"));
                    }
                }
            });
        }
    }
#endif
}

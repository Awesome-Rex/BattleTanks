using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using TransformTools;
using EditorTools;

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
            else
            {
                this.value = SetRotationLocal(offset.ReverseRotation(value).eulerAngles, Space.World);
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
            else
            {
                this.value = SetRotationLocal(offset.ReverseRotation(SetRotation(value.eulerAngles, Space.Self)).eulerAngles, Space.World);
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
            return transform.rotation;
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

    public override void SetToTarget()
    {
        target = GetTarget();

        if (enabled) {
            operationalRotation = target;

            RecordParent();
        }
    }
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

    public override void TargetToCurrent()
    {
        if (space == Space.Self)
        {
            if (link == Link.Offset)
            {
                rotation = operationalRotation;
            }
            else if (link == Link.Match)
            {
                //already set!!!
                //++++++++++++++++++++++MAKE A DEBUG.LOG or EXCEPTION
            }
        }
        else if (space == Space.World)
        {
            value = operationalRotation;
        }
    }

    public override void RecordParent()
    {
        parentRot = parent.rotation;
    }

    public Quaternion Rotate(Vector3 eulers, Space relativeTo = Space.Self)
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

    public Quaternion SetRotation(Vector3 rotation, Space relativeTo = Space.Self)
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
    public Quaternion SetRotationLocal (Vector3 rotation, Space relativeTo = Space.Self) {
        if (relativeTo == Space.Self)
        {
            return Quaternion.Euler(rotation);
        }
        else
        {
            return Linking.InverseTransformEuler(Quaternion.Euler(rotation), parentRot);
        }
    }
    public Quaternion GetRotation(Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self) {
            return Quaternion.Inverse(parentRot) * operationalRotation; //WORKS!
        } else
        {
            return operationalRotation; //WORKS!
        }
    }

    public override void SetPrevious()
    {
        previous = Linking.InverseTransformEuler(operationalRotation, parentRot);
    }

    public override void Switch(Space newSpace, Link newLink)
    {
        Quaternion originalRotation = rotation;
        Quaternion originalLocalRotation = localRotation;

        if (space == Space.World)
        {
            if (newSpace == Space.Self)
            {
                if (newLink == Link.Offset) //world > offset
                {
                    space = Space.Self;
                    link = Link.Offset;
                    
                    //auto keep offset
                    SetToTarget();
                    value = offset.ReverseRotation(this, Linking.InverseTransformEuler(originalRotation, parent.rotation));
                }
                else if (newLink == Link.Match) //world > match
                {
                    space = Space.Self;
                    link = Link.Match;
                }
            }
        }
        else if (space == Space.Self)
        {
            if (link == Link.Offset)
            {
                if (newSpace == Space.World) //offset > world
                {
                    space = Space.World;
                    rotation = originalRotation;
                }
                else
                {
                    if (newLink == Link.Match) //offset > match
                    {
                        link = Link.Match;
                    }
                }
            }
            else if (link == Link.Match)
            {
                if (newSpace == Space.World) //match > world
                {
                    space = Space.World;
                    rotation = originalRotation;
                }
                else
                {
                    if (newLink == Link.Offset) //match > offset
                    {
                        link = Link.Offset;
                        
                        //auto keep offset
                        SetToTarget();

                        value = offset.ReverseRotation(this, Linking.InverseTransformEuler(originalRotation, parent.rotation));
                    }
                }
            }
        }
    }
    public override void SwitchParent(Transform newParent)
    {
        if (space == Space.Self)
        {
            Quaternion originalRotation = rotation;
            Quaternion originalLocalRotation = localRotation;

            if (link == Link.Offset)
            {
                parent = newParent;

                rotation = offset.ReverseRotation(originalRotation);

            }
            else if (link == Link.Match)
            {
                parent = newParent;

                rotation = originalRotation;
            }
        }
    }
    public override void RemoveOffset()
    {
        if (space == Space.Self && link == Link.Offset)
        {
            rotation = offset.ApplyRotation(this, rotation);
        }

        offset = new AxisOrder(null, offset.variety, offset.space);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start() { }

#if UNITY_EDITOR
    [CustomEditor(typeof(CustomRotation))]
    public class E : EditorPRO<CustomRotation>
    {
        private bool showContextInfo = false;

        protected override void DeclareProperties ()
        {
            AddProperty("transition");
            AddProperty("offset");
            AddProperty("link");

            AddProperty("space");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            target.RecordParent();
        }

        public override void OnInspectorGUI()
        {
            OnInspectorGUIPRO(() =>
            {
                EditorGUILayout.PropertyField(FindProperty("space"));

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Rotation", EditorStyles.boldLabel);
                if (target.space == Space.Self)
                {
                    target.parent = (Transform)EditorGUILayout.ObjectField("Parent", target.parent, typeof(Transform), true);
                }
                if (!(target.space == Space.Self && target.link == Link.Match))
                {

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

                if (target.space == Space.Self && target.link == Link.Offset)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Transition", EditorStyles.boldLabel);

                    target.follow = EditorGUILayout.Toggle(string.Empty, target.follow);

                    EditorGUILayout.EndHorizontal();
                    if (target.follow)
                    {
                        EditorGUILayout.PropertyField(FindProperty("transition"));
                    }
                }

                EditorGUILayout.Space();

                showContextInfo = EditorGUILayout.Foldout(showContextInfo, "Context Info".bold(), EditorStyles.foldout.clone().richText());
                if (showContextInfo)
                {
                    target.eulerAngles = EditorGUILayout.Vector3Field("eulerAngles", target.eulerAngles);
                    target.localEulerAngles = EditorGUILayout.Vector3Field("localEulerAngles", target.localEulerAngles);
                }

                if (EditorApplication.isPaused || !EditorApplication.isPlaying)
                {
                    EditorGUILayout.Space();

                    if (!target.applyInEditor)
                    {
                        if (GUILayout.Button("Apply in Editor", EditorStyles.miniButton))
                        {
                            target.SetPrevious();
                            target.RecordParent();

                            target.applyInEditor = true;
                        }

                        if (EditorApplication.isPaused)
                        {
                            target.EditorApplyCheck();
                        }

                        if (GUILayout.Button("Set to Current", EditorStyles.miniButton))
                        {
                            Undo.RecordObject(target.gameObject, "Re-Oriented CustomRotation");

                            target.TargetToCurrent();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Don't Apply in Editor".colour(Color.red).bold(), EditorStyles.miniButton.clone().richText()))
                        {
                            target.applyInEditor = false;
                        }

                        if (EditorApplication.isPaused)
                        {
                            target.EditorApplyCheck();
                        }
                    }
                }
            });
        }
    }
#endif
}

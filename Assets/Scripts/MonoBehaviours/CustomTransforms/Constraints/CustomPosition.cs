using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;

using EditorTools;
using UnityEngine.SocialPlatforms;

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

    public override void SetToTarget()
    {
        target = GetTarget();

        if (enabled) {
            operationalPosition = target;

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
                    if (_ETERNAL.I.counter)
                    {
                        operationalPosition = target;
                    }
                }
            }
            if (_ETERNAL.I.counter)
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

    public override void TargetToCurrent(bool keepOffset = false)
    {
        if (space == Space.Self)
        {
            if (link == Link.Offset)
            {
                if (!keepOffset)
                {
                    offset = new AxisOrder();

                    if (factorScale) {
                        value = parent.InverseTransformPoint(operationalPosition) / offsetScale;
                    } else
                    {
                        value = Vectors.DivideVector3(parent.InverseTransformPoint(operationalPosition), parent.localScale);
                    }
                } else
                {
                    if (factorScale)
                    {
                        value = parent.InverseTransformPoint(operationalPosition) / offsetScale;

                        value = offset.ReversePosition(this, value);
                    }
                    else
                    {
                        value = Vectors.DivideVector3(parent.InverseTransformPoint(operationalPosition), parent.localScale);
                        value = offset.ReversePosition(this, value);
                    }
                }
            } else if (link == Link.Match)
            {
                //already set!!!
                //++++++++++++++++++++++MAKE A DEBUG.LOG or EXCEPTION
            }
        } else if (space == Space.World)
        {
            value = operationalPosition;
        }
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

    public Vector3 SetPosition(Vector3 position, Space relativeTo = Space.Self)
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
    public Vector3 GetPosition(Space relativeTo = Space.Self)
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

    public override void Switch(Space newSpace, Link newLink, bool keepOffset = false)
    {
        Vector3 originalPositon = position;
        Vector3 originalLocalPosition = localPosition;

        if (space == Space.World)
        {
            if (newSpace == Space.Self)
            {
                if (newLink == Link.Offset) //world > offset
                {
                    space = Space.Self;
                    link = Link.Offset;

                    if (!keepOffset) //dont keep offset
                    {
                        offset = new AxisOrder();

                        if (factorScale) //factor scale
                        {
                            value = Linking.InverseTransformPoint(originalPositon, parent.position, parent.rotation, parent.localScale * offsetScale);
                        }
                        else //dont factor scale
                        {
                            value = Linking.InverseTransformPoint(originalPositon, parent.position, parent.rotation);
                        }
                    }
                    else //keep offset
                    {
                        if (factorScale) //factor scale
                        {
                            SetToTarget();

                            Vector3 from = Linking.InverseTransformPoint(position, parent.position, parent.rotation, parent.localScale * offsetScale);
                            Vector3 to = Linking.InverseTransformPoint(originalPositon, parent.position, parent.rotation, parent.localScale * offsetScale);

                            value += to - from;
                        }
                        else //dont factor scale
                        {
                            SetToTarget();

                            Vector3 from = Linking.InverseTransformPoint(position, parent.position, parent.rotation);
                            Vector3 to = Linking.InverseTransformPoint(originalPositon, parent.position, parent.rotation);

                            value += to - from;
                        }
                    }
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
                    position = originalPositon;
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
                    position = originalPositon;
                }
                else
                {
                    if (newLink == Link.Offset) //match > offset
                    {
                        link = Link.Offset;

                        if (!keepOffset) //dont keep offset
                        {
                            offset = new AxisOrder();

                            if (factorScale) //factor scale
                            {
                                value = Linking.InverseTransformPoint(originalPositon, parent.position, parent.rotation, parent.localScale * offsetScale);
                            }
                            else //dont factor scale
                            {
                                value = Linking.InverseTransformPoint(originalPositon, parent.position, parent.rotation);
                            }
                        }
                        else //keep offset
                        {
                            if (factorScale) //factor scale
                            {
                                SetToTarget();

                                Vector3 from = Linking.InverseTransformPoint(position, parent.position, parent.rotation, parent.localScale * offsetScale);
                                Vector3 to = Linking.InverseTransformPoint(originalPositon, parent.position, parent.rotation, parent.localScale * offsetScale);

                                value += to - from;
                            }
                            else //dont factor scale
                            {
                                SetToTarget();

                                Vector3 from = Linking.InverseTransformPoint(position, parent.position, parent.rotation);
                                Vector3 to = Linking.InverseTransformPoint(originalPositon, parent.position, parent.rotation);

                                value += to - from;
                            }
                        }
                    }
                }
            }
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
    public class E : EditorPRO<CustomPosition>
    {
        private bool showContextInfo = false;

        protected override void DeclareProperties()
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
            target.rigidbody = target.GetComponent<Rigidbody>();
        }

        public override void OnInspectorGUI()
        {
            OnInspectorGUIPRO(() =>
            {
                EditorGUILayout.PropertyField(FindProperty("space"));

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Position", EditorStyles.boldLabel);
                if (target.space == Space.Self)
                {
                    target.parent = (Transform)EditorGUILayout.ObjectField("Parent", target.parent, typeof(Transform), true);
                }
                if (!(target.space == Space.Self && target.link == Link.Match))
                {

                    target.value = EditorGUILayout.Vector3Field("Value", target.value);
                }

                EditorGUILayout.Space();

                //Local
                if (target.space == Space.Self)
                {
                    EditorGUILayout.LabelField("Local", EditorStyles.boldLabel);

                    EditorGUILayout.PropertyField(FindProperty("link"));

                    if (target.link == Link.Offset)
                    {
                        EditorGUILayout.PropertyField(FindProperty("offset"));
                    }

                    target.factorScale = EditorGUILayout.Toggle("Factor Scale?", target.factorScale);
                    
                    DisableGroup(target.factorScale, () =>
                    {
                        target.offsetScale = EditorGUILayout.FloatField("Offset Scale", target.offsetScale);

                    });
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
                    target.position = EditorGUILayout.Vector3Field("position", target.position);
                    target.localPosition = EditorGUILayout.Vector3Field("localPosition", target.localPosition);
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
                            Undo.RecordObject(target.gameObject, "Re-Oriented CustomPosition");

                            target.TargetToCurrent(true);
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

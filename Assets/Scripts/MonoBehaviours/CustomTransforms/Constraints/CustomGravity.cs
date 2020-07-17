using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TransformTools;
using UnityEngine.Animations.Rigging;
using System;
using System.Runtime.CompilerServices;
using EditorTools;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody))]
public class CustomGravity : CustomTransform<Vector3>
{
    public float gravity = 9.81f;
    public float gravityScale = 1f;

    public AxisOrder offset = new AxisOrder(null, SpaceVariety.Mixed);  //local
    
    public Vector3 direction
    {
        get 
        {
            return GetDirection(Space.World);
        }
        set
        {
            this.value = SetDirectionLocal(value, Space.World);
        }
    }
    public Vector3 localDirection
    {
        get
        {
            return GetDirection(Space.Self);
        }
        set
        {
            this.value = SetDirectionLocal(value, Space.Self);
        }
    }

    public Vector3 directionRaw
    {
        get
        {
            return GetDirectionRaw(Space.World);
        }
        set
        {
            this.value = SetDirectionRawLocal(value, Space.World);
        }
    }
    public Vector3 localDirectionRaw
    {
        get
        {
            return GetDirectionRaw(Space.Self);
        }
        set
        {
            this.value = SetDirectionRawLocal(value, Space.Self);
        }
    }

    public Vector3 velocity
    {
        get
        {
            return GetVelocity(Space.World);
        }
        set
        {
            rigidbody.velocity = SetVelocity(value, Space.World);
        }
    }
    public Vector3 localVelocity
    {
        get
        {
            return GetVelocity(Space.Self);
        }
        set
        {
            rigidbody.velocity = SetVelocity(value, Space.Self);
        }
    }

    private Vector3 operationalDirection //operational
    {
        get
        {
            if (space == Space.Self)
            {
                return (offset.ApplyRotation(parent.rotation) * value);
            }
            else
            {
                return (offset.ApplyRotation(Quaternion.LookRotation(value)) * Vector3.forward);
            }
        }
    } // WORKS!

    //previous
    private Vector3 parentPos;
    private Quaternion parentRot;

    //script reference
    private new Rigidbody rigidbody;

    public void Apply()
    {
        if (enabled)
        {
            //Rotate velocity
            rigidbody.velocity = GetTarget();

            if (space == Space.World)
            {
                rigidbody.AddForce(operationalDirection.normalized * gravity * gravityScale, ForceMode.Acceleration);
            }
            else if (space == Space.Self)
            {
                rigidbody.AddForce(operationalDirection.normalized * gravity * gravityScale, ForceMode.Acceleration);
            }
        }
    }

    public override Vector3 GetTarget()
    {
        Vector3 target = Vector3.zero;

        if (space == Space.World)
        {
            target = rigidbody.velocity;
        }
        else if (space == Space.Self)
        {
            target = (parent.TransformPoint(Linking.InverseTransformPoint(parentPos + rigidbody.velocity, parentPos, parentRot)) - parent.position).normalized * rigidbody.velocity.magnitude;
        }

        return target;
    }

    public Vector3 GetDirection (Space relativeTo)
    {
        if (relativeTo == Space.Self) // self
        {
            return parent.InverseTransformPoint(parent.position + operationalDirection);
        }
        else // world
        {
            return operationalDirection;
        }
    }
    public Vector3 SetDirectionLocal(Vector3 direction, Space relativeTo) //SHOULD work
    {
        if (relativeTo == Space.Self)
        {
            return offset.ReverseRotation(Quaternion.LookRotation(direction)) * Vector3.forward;
        }
        else
        {
            return offset.ReverseRotation(Quaternion.LookRotation(parent.InverseTransformPoint(parent.position + direction))) * Vector3.forward;
        }
    }

    public Vector3 GetDirectionRaw(Space relativeTo)
    {
        if (relativeTo == Space.Self) // self
        {
            //return parent.InverseTransformPoint(parent.position + operationalDirection);
            return parent.InverseTransformPoint(parent.position + (offset.ReverseRotation(Quaternion.LookRotation(operationalDirection)) * Vector3.forward));
        }
        else // world
        {
            //return operationalDirection;
            return offset.ReverseRotation(Quaternion.LookRotation(operationalDirection)) * Vector3.forward;
        }
    }
    public Vector3 SetDirectionRawLocal(Vector3 direction, Space relativeTo) //SHOULD work
    {
        if (relativeTo == Space.Self)
        {
            return /*offset.ReverseRotation(Quaternion.LookRotation(*/direction/*)) * Vector3.forward*/;
        }
        else
        {
            return /*offset.ReverseRotation(Quaternion.LookRotation(*/parent.InverseTransformPoint(parent.position + direction)/*)) * Vector3.forward*/;
        }
    }

    public Vector3 GetVelocity(Space relativeTo)
    {
        if (relativeTo == Space.Self)
        {

            return (parent.InverseTransformPoint(parent.position + rigidbody.velocity));

        }
        else // world
        {
            return rigidbody.velocity;
        }
    }
    public Vector3 SetVelocity(Vector3 velocity, Space relativeTo)
    {
        if (relativeTo == Space.Self)
        {
            return parent.TransformPoint(parent.position + velocity);
        }
        else // world
        {
            return velocity;
        }
    }

    public override void Switch(Space newSpace, Link newLink) //WORKS!
    {
        Vector3 originalDirection = direction;
        Vector3 originalLocalDirection = localDirection;

        if (space == Space.World)
        {
            if (newSpace == Space.Self) //world > self
            {
                space = Space.Self;

                //auto keep offset
                value = offset.ReverseRotation(
                    Linking.InverseTransformEuler(Quaternion.LookRotation(originalDirection), parent.rotation)) * Vector3.forward;
            }
        }
        else if (space == Space.Self)
        {
            if (newSpace == Space.World) //self > world
            {
                space = Space.World;

                value = direction;
            }
        }
    }
    public override void SwitchParent(Transform newParent)
    {
        if (space == Space.Self)
        {
            Vector3 originalDirection = direction;
            Vector3 originalLocalDirection = localDirection;

            //NO LINKS (compared to Position and Rotation)
            parent = newParent;

            direction = originalDirection; //Should automatically FACTOR OFFSET, no need for offset.Reverse
        }
    }
    public void RemoveOffset()
    {
        if (space == Space.Self)
        {
            direction = offset.ApplyRotation (Quaternion.LookRotation(direction)) * Vector3.forward;
        }

        offset = new AxisOrder(null, offset.variety, offset.space);
    }

    public override void SetPrevious()
    {
        previous = parent.InverseTransformPoint(parent.position + rigidbody.velocity);

        parentPos = parent.position;
        parentRot = parent.rotation;
    }

    public void Enable(bool enabled)
    {
        if (enabled)
        {
            //disable rigidhodies, add custom behaviour

            _ETERNAL.I.earlyRecorder.lateCallbackF += Apply;
            _ETERNAL.I.lateRecorder.callbackF += SetPrevious;

            rigidbody.useGravity = false;

            this.enabled = true;
        }
        else
        {
            //enable rigidbodies, disable this script

            _ETERNAL.I.earlyRecorder.lateCallbackF -= Apply;
            _ETERNAL.I.lateRecorder.callbackF -= SetPrevious;
            
            rigidbody.useGravity = true;

            this.enabled = false;
        }
    }

    protected override void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        base.Awake();
    }
    
    void Start() { }

    private void OnEnable()
    {
        Enable(true);
    }
    private void OnDisable()
    {
        Enable(false);
    }

    protected override void OnDestroy()
    {
        Enable(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (operationalDirection.normalized * 2f));

        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawLine(transform.position, transform.position + (localDirection.normalized * 2f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (direction.normalized * 2f));
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CustomGravity))]
    public class E : EditorPRO<CustomGravity>
    {
        private bool showContextInfo = false;
        private bool showMethods = false;

        //method parameters
        private Space P_Switch_Space;
        private Link P_Switch_Link;

        private Transform P_SwitchParent_Parent;

        private LinkSpace P_SetContext_Type;
        private Vector3 P_SetContext_New;

        private Space P_SetVelocity_Type;
        private Vector3 P_SetVelocity_New;

        protected override void DeclareProperties()
        {
            AddProperty("offset");
            
            AddProperty("space");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            target.rigidbody = target.GetComponent<Rigidbody>();
        }

        public override void OnInspectorGUI()
        {
            OnInspectorGUIPRO(() =>
            {
            target.expanded = EditorGUILayout.Foldout(target.expanded, "Expanded".bold(), true, EditorStyles.foldout.clone().richText());

                //<-----------ACTUAL FIELDS------------>
                if (target.expanded)
                {
                    EditorGUILayout.PropertyField(FindProperty("space"));

                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Gravity Direction", EditorStyles.boldLabel);
                    if (target.space == Space.Self)
                    {
                        target.parent = (Transform)EditorGUILayout.ObjectField("Parent", target.parent, typeof(Transform), true);
                    }
                    target.value = EditorGUILayout.Vector3Field("Value", target.value);
                    target.gravity = EditorGUILayout.FloatField("Force", target.gravity);
                    target.gravityScale = EditorGUILayout.FloatField("Gravity Scale", target.gravityScale);

                    EditorGUILayout.Space();

                    //Local
                    if (target.space == Space.Self)
                    {
                        EditorGUILayout.LabelField("Local", EditorStyles.boldLabel);

                        EditorGUILayout.PropertyField(FindProperty("offset"));
                    }

                    EditorGUILayout.Space();

                    showContextInfo = EditorGUILayout.Foldout(showContextInfo, "Context Info".bold(), EditorStyles.foldout.clone().richText());
                    if (showContextInfo)
                    {
                        GUI.enabled = false;

                        //local and global directions
                        EditorGUILayout.Vector3Field("Direction", target.direction);
                        EditorGUILayout.Vector3Field("Local Direction", target.localDirection);

                        if (target.space == Space.Self)
                        {
                            EditorGUILayout.Space();

                            EditorGUILayout.Vector3Field("Direction Raw", target.directionRaw);
                            EditorGUILayout.Vector3Field("Local Direction Raw", target.localDirectionRaw);
                        }

                        EditorGUILayout.Space();

                        //local and global velocity
                        EditorGUILayout.Vector3Field("Velocity", target.velocity);
                        EditorGUILayout.Vector3Field("Local Velocity", target.localVelocity);

                        GUI.enabled = true;
                    }

                    //Editor methods!
                    Line();

                    if (EditorApplication.isPaused || !EditorApplication.isPlaying)
                    {
                        showMethods = EditorGUILayout.Foldout(showMethods, "Show Methods".bold(), EditorStyles.foldout.clone().richText());
                        if (showMethods)
                        {
                            Function("Switch", () =>
                            {
                                target.Switch(P_Switch_Space, P_Switch_Link);
                            }, new Action[] {
                                () => P_Switch_Space = (Space)EditorGUILayout.EnumPopup("New Space", P_Switch_Space),
                                () => P_Switch_Link = (Link)EditorGUILayout.EnumPopup("New Link", P_Switch_Link)
                            }, "Switched CustomGravity Space and/or Link", target.gameObject);

                            Function("Switch Parent", () =>
                            {
                                target.SwitchParent(P_SwitchParent_Parent);
                            }, new Action[] {
                                () => P_SwitchParent_Parent = (Transform)EditorGUILayout.ObjectField("New Parent", P_SwitchParent_Parent, typeof(Transform), true)
                            }, "Switched CustomGravity Parent", target.gameObject);

                            Function("Remove Offset", () =>
                            {
                                if (EditorUtility.DisplayDialog(
                                    "Remove Offset?",
                                    "Are you sure you want to remove the offset of \"CustomGravity?\"",
                                    "Yes", "Cancel"))
                                {
                                    Undo.RecordObject(target.gameObject, "Removed CustomGravity Offset");

                                    target.RemoveOffset();
                                }
                            }, new Action[] {});


                            Function("Set Context", () =>
                            {
                                if (P_SetContext_Type == LinkSpace.World)
                                {
                                    target.direction = P_SetContext_New;
                                }
                                else if (P_SetContext_Type == LinkSpace.Self)
                                {
                                    target.localDirection = P_SetContext_New;
                                }
                                else if (P_SetContext_Type == LinkSpace.WorldRaw)
                                {
                                    target.directionRaw = P_SetContext_New;
                                }
                                else if (P_SetContext_Type == LinkSpace.SelfRaw)
                                {
                                    target.localDirectionRaw = P_SetContext_New;
                                }
                            },
                                new Action[] {
                                    () => {
                                    EditorGUI.BeginChangeCheck();
                                    P_SetContext_Type = (LinkSpace)EditorGUILayout.EnumPopup("Type", P_SetContext_Type);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        if (P_SetContext_Type == LinkSpace.World)
                                        {
                                            P_SetContext_New = target.direction;
                                        }
                                        else if (P_SetContext_Type == LinkSpace.Self)
                                        {
                                            P_SetContext_New = target.localDirection;
                                        }
                                        else if (P_SetContext_Type == LinkSpace.WorldRaw)
                                        {
                                            P_SetContext_New = target.directionRaw;
                                        }
                                        else if (P_SetContext_Type == LinkSpace.SelfRaw)
                                        {
                                            P_SetContext_New = target.localDirectionRaw;
                                        }
                                    }
                                    },
                                    () => P_SetContext_New = EditorGUILayout.Vector3Field(GUIContent.none, P_SetContext_New)
                                }, "Changed Context Value of CustomGravity", target.gameObject);

                            //
                            Function("Set Velocity", () =>
                            {
                                if (P_SetVelocity_Type == Space.World)
                                {
                                    target.velocity = P_SetVelocity_New;
                                }
                                else if (P_SetVelocity_Type == Space.Self)
                                {
                                    target.localVelocity = P_SetVelocity_New;
                                }
                            },
                                new Action[] {
                                    () => {
                                    EditorGUI.BeginChangeCheck();
                                    P_SetVelocity_Type = (Space)EditorGUILayout.EnumPopup("Type", P_SetVelocity_Type);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        if (P_SetVelocity_Type == Space.World)
                                        {
                                            P_SetVelocity_New = target.velocity;
                                        }
                                        else if (P_SetVelocity_Type == Space.Self)
                                        {
                                            P_SetVelocity_New = target.localVelocity;
                                        }
                                    }
                                    },
                                    () => P_SetVelocity_New = EditorGUILayout.Vector3Field(GUIContent.none, P_SetVelocity_New)
                                }, "Changed Rigidbody Velocity from CustomGravity", target.gameObject);
                        }
                    }
                }
            });
        }
    }
#endif
}

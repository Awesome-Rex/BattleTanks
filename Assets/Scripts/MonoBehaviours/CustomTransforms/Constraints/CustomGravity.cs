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

    public AxisOrder offset;  //local
    
    public LocalRelativity linkTo;

    public Vector3 direction
    {
        get 
        {
            return GetDirection(Space.World);
        }
        set
        {
            value = SetDirection(value, Space.World);
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
            value = SetDirection(value, Space.Self);
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

    //previous
    private Vector3 parentPos;
    private Quaternion parentRot;


    //script reference
    private new Rigidbody rigidbody;
    private CustomGravity parentGravity;

    public void Apply()
    {
        if (enabled)
        {
            //Rotate velocity
            rigidbody.velocity = GetTarget();

            //Apply force
            Vector3 fallDirection = Vector3.zero;


            if (linkTo == LocalRelativity.Natural || parentGravity == null) //Transform Direction Vector
            {
                ///////////// VALUE IS A DIRECTION VECTOR NOT EULER ANGLES
                fallDirection = (offset.ApplyRotation(parent.rotation) * value).normalized;
            }
            else if (linkTo == LocalRelativity.Constraint && parentGravity != null) //CustomGravity
            {
                //fallDirection = (offset.ApplyRotation(parentRotation) * value).normalized;
                fallDirection = (offset.ApplyRotation(
                    Quaternion.LookRotation(parentGravity.offset.ApplyRotation(parent.rotation) * parentGravity.value)) * value).normalized * parentGravity.gravity * parentGravity.gravityScale;
            }

            if (space == Space.World)
            {
                rigidbody.AddForce((offset.ApplyRotation(Quaternion.LookRotation(value)) * Vector3.forward).normalized * gravity * gravityScale, ForceMode.Acceleration);
            }
            else if (space == Space.Self)
            {
                //rigidbody.AddForce((offset.ApplyRotation(Quaternion.LookRotation(parent.TransformPoint(value) - parent.position)) * Vector3.forward).normalized * gravity * gravityScale/* * 0.5f*/, ForceMode.Acceleration);
                rigidbody.AddForce(fallDirection * gravity * gravityScale, ForceMode.Acceleration);
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

    public override void TargetToCurrent()
    {
        
    }

    public Vector3 GetDirection (Space space)
    {
        if (space == Space.Self)
        {
            if (this.space == Space.Self)
            {
                return value.normalized;
            }
            else
            {
                return (parent.InverseTransformPoint(parent.position + value.normalized));
            }
        }
        else // world
        {
            if (this.space == Space.Self)
            {
                return parent.TransformPoint(value.normalized) - parent.position;
            }
            else
            {
                return value.normalized;
            }
        }
    }
    public Vector3 SetDirection (Vector3 direction, Space space)
    {
        if (space == Space.Self)
        {
            if (this.space == Space.Self)
            {
                return direction.normalized;
            }
            else
            {
                return parent.TransformPoint(parent.position + direction.normalized);
            }
        } else // world
        {
            if (this.space == Space.Self)
            {
                return (parent.InverseTransformPoint(parent.position + direction.normalized));
            }
            else
            {
                return direction.normalized;
            }
        }
    }

    public Vector3 GetVelocity(Space space)
    {
        if (space == Space.Self)
        {

            return (parent.InverseTransformPoint(parent.position + rigidbody.velocity));

        }
        else // world
        {
            return rigidbody.velocity;
        }
    }
    public Vector3 SetVelocity(Vector3 velocity, Space space)
    {
        if (space == Space.Self)
        {
            return parent.TransformPoint(parent.position + velocity);
        }
        else // world
        {
            return velocity;
        }
    }

    public override void Switch(Space newSpace, Link newLink, bool keepOffset = false)
    {
        Vector3 originalDirection = direction;
        Vector3 originalLocalDirection = localDirection;

        if (space == Space.World)
        {
            if (newSpace == Space.Self) //world > self
            {
                space = Space.Self;

                if (!keepOffset) //dont keep offset
                {
                    offset = new AxisOrder();
                    value = Linking.InverseTransformPoint(originalDirection, parent.position, parent.rotation);
                }
                else //keep offset
                {

                }
            }
        }
        else if (space == Space.Self)
        {
            if (newSpace == Space.World) //self > world
            {
                space = Space.World;

                value = originalDirection;
            }
        }
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
        parentGravity = parent.GetComponent<CustomGravity>();

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

#if UNITY_EDITOR
    [CustomEditor(typeof(CustomGravity))]
    public class E : EditorPRO<CustomGravity>
    {
        private bool showContextInfo = false;

        protected override void DeclareProperties()
        {
            AddProperty("offset");
            
            AddProperty("space");

            AddProperty("linkTo");
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

                    EditorGUILayout.PropertyField(FindProperty("linkTo"));
                }

                EditorGUILayout.Space();

                showContextInfo = EditorGUILayout.Foldout(showContextInfo, "Context Info".bold(), EditorStyles.foldout.clone().richText());
                if (showContextInfo)
                {
                    //local and global directions
                    target.direction = EditorGUILayout.Vector3Field("Direction", target.direction);
                    target.localDirection = EditorGUILayout.Vector3Field("Local Direction", target.localDirection);

                    //local and global velocity
                    target.velocity = EditorGUILayout.Vector3Field("Velocity", target.velocity);
                    target.localVelocity = EditorGUILayout.Vector3Field("Local Velocity", target.localVelocity);
                }
            });
        }
    }
#endif
}

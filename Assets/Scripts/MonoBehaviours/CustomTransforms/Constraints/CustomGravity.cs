using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TransformTools;
using UnityEngine.Animations.Rigging;
using System;
using System.Runtime.CompilerServices;

[RequireComponent(typeof(Rigidbody))]
public class CustomGravity : CustomTransform<Vector3>
{
    public float gravity = 9.81f;
    public float gravityScale = 1f;

    public AxisOrder offset;  //local
    
    public LocalRelativity linkTo;

    //previous
    private Vector3 parentPos;
    private Quaternion parentRot;


    //script reference
    private new Rigidbody rigidbody;
    private CustomGravity parentGravity;

    public void ApplyGravity()
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

    public override void SetPrevious()
    {
        previous = parent.InverseTransformPoint(parent.position + rigidbody.velocity);

        parentPos = parent.position;
        parentRot = parent.rotation;
    }

    public void EnableGravity(bool enabled)
    {
        if (enabled)
        {
            //disable rigidhodies, add custom behaviour

            _ETERNAL.R.earlyRecorder.lateCallbackF += ApplyGravity;
            _ETERNAL.R.lateRecorder.callbackF += SetPrevious;

            this.enabled = true;
        }
        else
        {
            //enable rigidbodies, disable this scirpt

            _ETERNAL.R.earlyRecorder.lateCallbackF -= ApplyGravity;
            _ETERNAL.R.lateRecorder.callbackF -= SetPrevious;

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
        EnableGravity(true);
    }
    private void OnDisable()
    {
        EnableGravity(false);
    }

    protected override void OnDestroy()
    {
        EnableGravity(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TransformTools;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(Rigidbody))]
public class CustomGravity : CustomTransform<Vector3>
{
    public float gravity = 9.81f;
    public float gravityScale = 1f;

    public AxisOrder offset;  //local

    private Vector3 parentPos;
    private Quaternion parentRot;


    //script reference
    private new Rigidbody rigidbody;

    //private bool counter;
    public void ApplyGravity()
    {
        if (enabled)
        {
            rigidbody.velocity = (parent.TransformPoint(Linking.InverseTransformPoint(parentPos + rigidbody.velocity, parentPos, parentRot)) - parent.position).normalized * rigidbody.velocity.magnitude;

            if (space == Space.World)
            {
                //rigidbody.AddForce(value.normalized * gravity * gravityScale * 0.5f, ForceMode.Acceleration);
                rigidbody.AddForce((offset.ApplyRotation(Quaternion.LookRotation(value)) * Vector3.forward).normalized * gravity * gravityScale/* * 0.5f*/, ForceMode.Acceleration);
            }
            else if (space == Space.Self)
            {
                //rigidbody.AddForce((parent.TransformPoint(value.normalized) - parent.position) * gravity * gravityScale * 0.5f, ForceMode.Acceleration);
                rigidbody.AddForce((offset.ApplyRotation(Quaternion.LookRotation(parent.TransformPoint(value) - parent.position)) * Vector3.forward).normalized * gravity * gravityScale/* * 0.5f*/, ForceMode.Acceleration);
            }
        }
    }

    public override Vector3 GetTarget()
    {
        Vector3 target = Vector3.zero;

        if (space == Space.World)
        {
            //target = (offset.ApplyRotation(Quaternion.LookRotation(value)) * Vector3.forward).normalized * rigidbody.velocity.magnitude;
            target = previous.normalized * rigidbody.velocity.magnitude;
        }
        else if (space == Space.Self)
        {
            //target = (parent.TransformPoint(previous) - parent.position);
            //target = (offset.ApplyRotation(Quaternion.LookRotation(target)) * Vector3.forward).normalized * rigidbody.velocity.magnitude;
            target = (Linking.TransformPoint(previous, parentPos, parentRot) - parentPos).normalized * rigidbody.velocity.magnitude;
        }

        return target;
    }

    public override void SetPrevious()
    {
        previous = parent.InverseTransformPoint(parent.position + rigidbody.velocity);
        //previous = parent.TransformPoint(rigidbody.velocity);
        //previous = Linking.TransformPoint(rigidbody.velocity, parent.position, Quaternion.Euler(Vector3.zero)) - parent.position;

        /*if (_ETERNAL.R.counter)
        {*/
        parentPos = parent.position;
            parentRot = parent.rotation;
        //}
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

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(transform.position, transform.position + (offset.ApplyRotation(Quaternion.LookRotation(parent.TransformPoint(value) - parent.position)) * Vector3.forward).normalized * 2f);

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position, transform.position + (parent.TransformPoint(previous) - parent.position).normalized * rigidbody.velocity.magnitude);

    //}
}

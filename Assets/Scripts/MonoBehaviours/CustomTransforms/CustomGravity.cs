using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomGravity : CustomTransform<Vector3>
{
    public float gravity = 9.81f;
    public float gravityScale = 1f;

    public AxisOrder offset;  //local

    private Quaternion previousParentRot;

    //script reference
    private new Rigidbody rigidbody;

    private bool counter;
    public void ApplyGravity()
    {
        if (enabled)
        {
            if (!counter){
                /*if (space == Space.Self)
                {
                    rigidbody.velocity = ((parent.rotation * Quaternion.Inverse(previousParentRot)) * (GetTarget().normalized)) * rigidbody.velocity.magnitude;
                    //rigidbody.velocity = ((parent.rotation * Quaternion.Inverse(previousParentRot)) * ((parent.TransformPoint(rigidbody.velocity) - parent.position).normalized)) * rigidbody.velocity.magnitude;
                }*/

                rigidbody.velocity = GetTarget();
            }

            //Debug.Log((parent.rotation * Quaternion.Inverse(previousParentRot)).eulerAngles);
            
            if (space == Space.World)
            {
                //rigidbody.AddForce(offset.ApplyRotation(Quaternion.Euler(Vector3.zero)) * Vector3.down * gravity * gravityScale, ForceMode.Acceleration);
                rigidbody.AddForce(value.normalized * gravity * gravityScale * 0.5f, ForceMode.Acceleration);
            }
            else if (space == Space.Self)
            {
                //rigidbody.AddForce(offset.ApplyRotation(parent.rotation) * Vector3.down * gravity * gravityScale, ForceMode.Acceleration);
                rigidbody.AddForce((parent.TransformPoint(value.normalized) - parent.position) * gravity * gravityScale * 0.5f, ForceMode.Acceleration);
                //Debug.Log((parent.TransformPoint(value.normalized) - parent.position));
            }

            //if (counter) {

            counter = !counter;
            //SetPrevious();
            //GetComponent<CustomPosition>().SetPrevious();
            //}
        }
    }

    public override Vector3 GetTarget()
    {
        Vector3 target = Vector3.zero;

        if (space == Space.World)
        {
            //target = previous;
            target = rigidbody.velocity;
        }
        else if (space == Space.Self)
        {
            //target = parent.TransformPoint(previous) - parent.position;
            //target = parent.TransformPoint(rigidbody.velocity) - parent.position;
            //target = ((parent.rotation * Quaternion.Inverse(previousParentRot))) * (rigidbody.velocity.normalized) * rigidbody.velocity.magnitude;
            target = ((parent.rotation * Quaternion.Euler(90f, 0f, 0f)) * Vector3.forward) * rigidbody.velocity.magnitude;
        }

        return target;
    }

    public override void SetPrevious()
    {
        previous = parent.TransformDirection(rigidbody.velocity) - parent.position;
        previousParentRot = parent.rotation;
    }

    public void EnableGravity(bool enabled)
    {
        if (enabled)
        {
            //disable rigidhodies, add custom behaviour
            _ETERNAL.r.earlyRecorder.lateCallbackF += ApplyGravity;
            _ETERNAL.r.lateRecorder.earlyCallbackF += SetPrevious;

            this.enabled = true;
        }
        else
        {
            //enable rigidbodies, disable this scirpt

            _ETERNAL.r.earlyRecorder.lateCallbackF -= ApplyGravity;
            _ETERNAL.r.lateRecorder.earlyCallbackF -= SetPrevious;

            this.enabled = false;
        }
    }

    protected override void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        EnableGravity(enabled);

        base.Awake();

        _ETERNAL.r.lateRecorder.callback -= SetPrevious;
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
        //base.OnDestroy();
        EnableGravity(false);
    }
}

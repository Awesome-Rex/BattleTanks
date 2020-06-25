using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomGravity : CustomTransform<Vector3>
{
    public float gravity = 9.81f;
    public float gravityScale = 1f;

    public AxisOrder offset;  //local

    //script reference
    private new Rigidbody rigidbody;

    private bool counter;
    public void ApplyGravity()
    {
        if (enabled)
        {
            if (counter) {
                rigidbody.velocity = GetTarget().normalized * rigidbody.velocity.magnitude;
            }
            counter = !counter;

            if (space == Space.World)
            {
                //rigidbody.AddForce(offset.ApplyRotation(Quaternion.Euler(Vector3.zero)) * Vector3.down * gravity * gravityScale, ForceMode.Acceleration);
                rigidbody.AddForce(value.normalized * gravity * gravityScale, ForceMode.Acceleration);
            }
            else if (space == Space.Self)
            {
                //rigidbody.AddForce(offset.ApplyRotation(parent.rotation) * Vector3.down * gravity * gravityScale, ForceMode.Acceleration);
                rigidbody.AddForce((parent.TransformPoint(value.normalized) - parent.position) * gravity * gravityScale, ForceMode.Acceleration);
            }

            SetPrevious();
        }
    }

    public override Vector3 GetTarget()
    {
        Vector3 target = Vector3.zero;

        if (space == Space.World)
        {
            target = previous;
        }
        else if (space == Space.Self)
        {
            target = parent.TransformPoint(previous) - parent.position;
        }

        return target;
    }

    public override void SetPrevious()
    {
        previous = parent.TransformPoint(rigidbody.velocity) - parent.position;
    }

    public void EnableGravity(bool enabled)
    {
        if (enabled)
        {
            //disable rigidhodies, add custom behaviour
            _ETERNAL.r.earlyRecorder.lateCallbackF += ApplyGravity;
            //_ETERNAL.r.lateRecorder.earlyCallbackF += SetPrevious;

            this.enabled = true;
        }
        else
        {
            //enable rigidbodies, disable this scirpt

            _ETERNAL.r.earlyRecorder.lateCallbackF -= ApplyGravity;
            //_ETERNAL.r.lateRecorder.earlyCallbackF -= SetPrevious;

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
        base.OnDestroy();
        EnableGravity(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomGravity : CustomTransform<Quaternion>
{
    public float gravity = 9.81f;
    public float gravityScale = 1f;

    public AxisOrder offset;  //local

    //previous
    private float previousMagnitude;
    private Vector3 previousVel;

    //script reference
    private new Rigidbody rigidbody;

    public void ApplyGravity()
    {
        if (enabled)
        {
            if (rigidbody.velocity != Vector3.zero)
            {
                //rigidbody.velocity = GetTarget() * Vector3.down * previousMagnitude;
                //rigidbody.velocity = previousVel;
            }

            if (space == Space.World)
            {
                rigidbody.AddForce(offset.ApplyRotation(Quaternion.Euler(Vector3.zero)) * Vector3.down * gravity * gravityScale, ForceMode.Acceleration);
            }
            else
            {
                rigidbody.AddForce(offset.ApplyRotation(parent.rotation) * Vector3.down * gravity * gravityScale, ForceMode.Acceleration);
            }

            SetPrevious();
        }
    }

    public override Quaternion GetTarget()
    {
        Quaternion target = new Quaternion();

        if (space == Space.World)
        {
            target = offset.ApplyRotation(Quaternion.Euler(Vector3.zero));
        }
        else if (space == Space.Self)
        {
            target = offset.ApplyRotation(parent.rotation) * previous;
        }

        return target;
    }

    public override void SetPrevious()
    {
        previousMagnitude = rigidbody.velocity.magnitude;
        previous = Quaternion.Inverse(parent.rotation) * Quaternion.LookRotation(rigidbody.velocity.normalized);

        previousVel = rigidbody.velocity;

        //Debug.Log(previous * Vector3.down);
        //Debug.Log(previousMagnitude);
        Debug.Log(previousVel);
    }

    public void EnableGravity(bool enabled)
    {
        if (enabled)
        {
            //disable rigidhodies, add custom behaviour
            
            this.enabled = true;
        }
        else
        {
            //enable rigidbodies, disable this scirpt

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

    private void FixedUpdate()
    {
        ApplyGravity();
    }

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TransformTools;

[RequireComponent(typeof(Rigidbody))]
public class CustomTorque : CustomTransform<Vector3>
{
    public float torque = 9.81f;
    public float torqueScale = 1f;

    public AxisOrder offset;  //local

    public LocalRelativity linkTo;

    //previous
    private Quaternion parentRot;


    //script reference
    private new Rigidbody rigidbody;
    private CustomTorque parentTorque;

    public void Apply()
    {
        if (enabled)
        {
            //Rotate velocity
            rigidbody.velocity = GetTarget();

            //Apply force
            Vector3 fallDirection = Vector3.zero;


            if (linkTo == LocalRelativity.Natural || parentTorque == null) //Transform Direction Vector
            {
                
            }
            else if (linkTo == LocalRelativity.Constraint && parentTorque != null) //CustomGravity
            {
                
            }

            if (space == Space.World)
            {
                
            }
            else if (space == Space.Self)
            {
                
            }
        }
    }

    public override Vector3 GetTarget()
    {
        Vector3 target = Vector3.zero;

        if (space == Space.World)
        {
            target = rigidbody.angularVelocity;
        }
        else if (space == Space.Self)
        {
            target = parent.eulerAngles + Vectors.Rad2Deg(rigidbody.angularVelocity);
        }

        return target;
    }

    public override void SetPrevious()
    {
        previous = parent.eulerAngles;

        //parentRot = parent.rotation;
    }

    public override void Switch(Space newSpace, Link newLink)
    {
        throw new System.NotImplementedException();
    }
    public override void SwitchParent(Transform newParent)
    {
        throw new System.NotImplementedException();
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
            //enable rigidbodies, disable this scirpt

            _ETERNAL.I.earlyRecorder.lateCallbackF -= Apply;
            _ETERNAL.I.lateRecorder.callbackF -= SetPrevious;

            rigidbody.useGravity = true;

            this.enabled = false;
        }
    }

    protected override void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        parentTorque = parent.GetComponent<CustomTorque>();

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
}
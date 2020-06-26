using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomTransformLinks<T> : CustomTransform<T>
{
    protected T target;

    public bool follow = false;
    public Transition transition;
    
    public Link link = Link.Offset;
    
    public AxisOrder offset;  //local

    //components
    protected new Rigidbody rigidbody;

    //methods
    public abstract void SetToTarget();

    public abstract void MoveToTarget();


    protected override void Awake ()
    {
        base.Awake();
        _ETERNAL.r.earlyRecorder.callbackF += MoveToTarget;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _ETERNAL.r.earlyRecorder.callbackF -= MoveToTarget;
    }
}

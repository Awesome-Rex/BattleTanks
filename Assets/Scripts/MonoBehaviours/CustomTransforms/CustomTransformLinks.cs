using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomTransformLinks<T> : CustomTransform<T>
{
    public bool follow = false;
    public Transition transition;
    
    public Link link = Link.Offset;
    

    public AxisOrder offset;  //local
    
    public abstract void SetToTarget();

    public abstract void MoveToTarget();


    protected override void Awake ()
    {
        base.Awake();
        _ETERNAL.r.earlyRecorder.callback += MoveToTarget;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _ETERNAL.r.earlyRecorder.callback -= MoveToTarget;
    }
}

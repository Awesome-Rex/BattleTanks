using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomTransformLinks<T> : CustomTransform<T>
{
    public bool follow = false;
    public Transition transition;
    
    public Link link = Link.Offset;
    

    public T globalOffset;  //local
    
    public abstract void ApplyToTarget();

    public abstract void SetTarget();


    protected override void Awake ()
    {
        base.Awake();
        _ETERNAL.r.earlyRecorder.callback += SetTarget;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _ETERNAL.r.earlyRecorder.callback -= SetTarget;
    }
}

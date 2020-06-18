using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomTransform<T> : MonoBehaviour
{
    public Space space = Space.Self;

    public bool follow = false;
    public Transition transition;

    //global
    public T value;

    //local
    public Transform parent;

    public Link link = Link.Offset;

    public T localOffset;
    public T globalOffset;


    //previous
    private T previous;

    [ContextMenu("Apply to target")]
    public abstract void ApplyToTarget();

    public abstract void SetTarget();

    public abstract Vector3 GetTarget();


    public abstract void SetPrevious();

    private void Awake()
    {
        SetPrevious();

        _ETERNAL.r.lateRecorder.callback += SetPrevious;
        _ETERNAL.r.earlyRecorder.callback += SetTarget;
    }

    private void OnDestroy()
    {
        _ETERNAL.r.lateRecorder.callback -= SetPrevious;
        _ETERNAL.r.earlyRecorder.callback -= SetTarget;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomTransform<T> : MonoBehaviour
{
    //properties
    public Space space = Space.Self;
    
    public Transform parent;
    public T value;
    
    protected T previous;

    //methods
    public abstract T GetTarget();

    public abstract void SetPrevious();

    protected virtual void Awake()
    {
        SetPrevious();

        _ETERNAL.r.lateRecorder.callback += SetPrevious;
    }

    protected virtual void OnDestroy()
    {
        _ETERNAL.r.lateRecorder.callback -= SetPrevious;
    }
}

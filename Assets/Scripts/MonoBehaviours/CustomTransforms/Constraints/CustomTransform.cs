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

    public abstract void TargetToCurrent(bool keepOffset = false);

    public abstract void Switch(Space newSpace, Link newLink, bool keepOffset = false);

    protected virtual void Awake()
    {
        SetPrevious();

        _ETERNAL.I.lateRecorder.callbackF += SetPrevious;
    }

    protected virtual void OnDestroy()
    {
        _ETERNAL.I.lateRecorder.callbackF -= SetPrevious;
    }
}

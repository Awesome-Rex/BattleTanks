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


    /*public static Vector3 TransformPoint (Vector3 point, Vector3 position, Quaternion rotation, Vector3 scale)
    {

    }*/

    protected virtual void Awake()
    {
        SetPrevious();

        _ETERNAL.r.lateRecorder.callbackF += SetPrevious;
    }

    protected virtual void OnDestroy()
    {
        _ETERNAL.r.lateRecorder.callbackF -= SetPrevious;
    }
}

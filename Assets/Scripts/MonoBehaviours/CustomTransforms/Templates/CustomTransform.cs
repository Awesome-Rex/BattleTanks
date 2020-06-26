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


    //local > world
    public static Vector3 TransformPoint(Vector3 point, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        return Matrix4x4.TRS(position, rotation, scale).MultiplyPoint3x4(point);
    }
    public static Vector3 TransformPoint(Vector3 point, Vector3 position, Quaternion rotation)
    {
        return rotation * point + position;
    }

    //world > local
    public static Vector3 InverseTransformPoint (Vector3 point, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        // *ALTERNATE* return Matrix4x4.TRS(position, rotation, scale).inverse.MultiplyPoint3x4(point);
        return Vector3.Scale(
            AxisOrder.DivideVector3(Vector3.one, scale), 
            (Quaternion.Inverse(rotation) * (point - position))
        );
    }
    public static Vector3 InverseTransformPoint(Vector3 point, Vector3 position, Quaternion rotation)
    {
        return Vector3.Scale(
            AxisOrder.DivideVector3(Vector3.one, Vector3.one/**/),
            (Quaternion.Inverse(rotation) * (point - position))
        );
    }

    //local > world
    public static Quaternion TransformEuler (Quaternion eulers, Quaternion rotation)
    {
        return rotation * eulers;
    }
    //world > local
    public static Quaternion InverseTransformEuler (Quaternion eulers, Quaternion rotation)
    {
        return Quaternion.Inverse(rotation) * eulers;
    }

    protected virtual void Awake()
    {
        SetPrevious();

        _ETERNAL.R.lateRecorder.callbackF += SetPrevious;
    }

    protected virtual void OnDestroy()
    {
        _ETERNAL.R.lateRecorder.callbackF -= SetPrevious;
    }
}

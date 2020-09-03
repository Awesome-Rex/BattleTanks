using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialOrientation : MonoBehaviour
{
    private Vector3 _up;
    private Vector3 _forward;

    public Vector3 up
    {
        get
        {
            return _up;
        }
        set
        {
            _up = value;
        }
    }
    public Vector3 forward
    {
        get
        {
            return _forward;
        }
        set
        {
            _forward = value;
        }
    }
    public Vector3 right
    {
        get
        {
            return Quaternion.LookRotation(forward, up) * Vector3.right;
        }
        set
        {

        }
    }
    
    public Vector3 down
    {
        get
        {
            return -up;
        }
        set
        {
            up = -value;
        }
    }
    public Vector3 back
    {
        get
        {
            return -forward;
        }
        set
        {
            forward = -value;
        }
    }
    public Vector3 left
    {
        get
        {
            return -right;
        }
        set
        {
            right = -value;
        }
    }
}

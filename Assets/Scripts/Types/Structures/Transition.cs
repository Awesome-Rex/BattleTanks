using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum Curve { Linear, Interpolate, Custom}
public enum Link { Offset, Match }

[Serializable]
public struct Transition
{
    public Curve type;

    public float speed;
    //linear, units per second

    public float degrees;

    public float percent;
    //percent per second


    //  public curve curve
    
    public float MoveTowards (float a, float b)
    {
        if (type == Curve.Linear)
        {
            return Mathf.MoveTowards(a, b, speed * Time.deltaTime);
        }
        else if (type == Curve.Interpolate)
        {
            return Mathf.Lerp(a, b, percent * Time.deltaTime);
        } else if (type == Curve.Custom) {
            
        }

        return a;
    }

    public Vector3 MoveTowards(Vector3 a, Vector3 b)
    {
        if (type == Curve.Linear)
        {
            return Vector3.MoveTowards(a, b, speed * Time.deltaTime);
        }
        else if (type == Curve.Interpolate)
        {
            return Vector3.Lerp(a, b, percent * Time.deltaTime);
        }
        else if (type == Curve.Custom)
        {

        }

        return a;
    }

    public Quaternion MoveTowards (Quaternion a, Quaternion b)
    {
        if (type == Curve.Linear)
        {
            return Quaternion.RotateTowards(a, b, degrees * Time.deltaTime);
        }
        else if (type == Curve.Interpolate)
        {
            return Quaternion.Lerp(a, b, percent * Time.deltaTime);
        }
        else if (type == Curve.Custom)
        {

        }

        return a;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : MonoBehaviour
{
    public float speed;
    //units per second

    public bool overriding = false;
    public float localOverride = 0f;
    public Vector3 directionalOverride = Vector3.zero;

    public List<float> localInfluence;
    public List<float> localMultiplier;

    public List<Vector3> directionalInfluence;
    public List<float> directionallMultiplier;

    public float CalculatedLocal ()
    {
        if (!overriding)
        {
            float total = 0;

            foreach (float i in localInfluence)
            {
                total += i;
            }

            float totalMultiplier = 1;

            foreach (float i in localMultiplier)
            {
                totalMultiplier *= i;
            }

            return total * totalMultiplier;
        } else
        {
            return localOverride;
        }
    }

    public Vector3 CalculatedDirectional ()
    {
        if (!overriding)
        {
            Vector3 total = Vector3.zero;

            foreach (Vector3 i in directionalInfluence)
            {
                total += i;
            }

            float totalMultiplier = 1;

            foreach (float i in localMultiplier)
            {
                totalMultiplier *= i;
            }

            return total * totalMultiplier;
        }
        else
        {
            return directionalOverride;
        }
    }

    public Vector3 calculatedMovement (Vector3 direction)
    {
        return CalculatedDirectional() + (direction.normalized * CalculatedLocal());
    }
}

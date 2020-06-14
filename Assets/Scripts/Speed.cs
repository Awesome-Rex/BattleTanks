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
    public Vector3 directionalOveride = Vector3.zero;

    public List<float> localInfluence;

    public List<Vector3> directionalInfluence;

    public float CalculatedLocal (Vector3 direction)
    {
        throw new NotImplementedException();
    }

    public Vector3 CalculatedDirectional (Vector3 direction)
    {
        throw new NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class _ETERNAL : MonoBehaviour
{
    public static _ETERNAL r;

    //children
    public bool transformableUsed;
    private Transform transformable;


    //component references
    public LateRecorder lateRecorder;
    public EarlyRecorder earlyRecorder;

    public void UseTransformable (Action<Transform> modifier)
    {
        if (!transformableUsed) {
            transformableUsed = true;

            transformable.transform.position = Vector3.zero;
            transformable.transform.eulerAngles = Vector3.zero;
            transformable.localScale = Vector3.one;

            modifier(transformable.transform);

            transformableUsed = false;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        r = this;

        //children
        transformableUsed = false;
        transformable = GameObject.FindGameObjectWithTag("Transformable").transform;

        //component references
        lateRecorder = GetComponent<LateRecorder>();
        earlyRecorder = GetComponent<EarlyRecorder>();
    }
}

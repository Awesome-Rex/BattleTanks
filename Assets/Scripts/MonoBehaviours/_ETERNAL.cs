using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class _ETERNAL : MonoBehaviour
{
    public static _ETERNAL r;

    //component references
    public LateRecorder lateRecorder;
    public EarlyRecorder earlyRecorder;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        r = this;

        //component references
        lateRecorder = GetComponent<LateRecorder>();
        earlyRecorder = GetComponent<EarlyRecorder>();
    }
}

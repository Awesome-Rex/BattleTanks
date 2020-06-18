using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateRecorder : MonoBehaviour
{
    public System.Action callback;
    
    // Update is called once per frame
    void LateUpdate()
    {
        if (callback != null) {
            callback();
        }
    }
}

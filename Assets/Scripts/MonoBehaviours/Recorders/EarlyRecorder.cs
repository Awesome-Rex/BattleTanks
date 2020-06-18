using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarlyRecorder : MonoBehaviour
{
    public System.Action callback;
    
    // Update is called once per frame
    private void Update()
    {
        if (callback != null) {
            callback();
        }
    }
}

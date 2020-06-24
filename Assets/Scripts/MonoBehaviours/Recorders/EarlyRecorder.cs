using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarlyRecorder : MonoBehaviour
{
    public System.Action callback;
    public System.Action lateCallback;

    public System.Action callbackF;

    private void FixedUpdate()
    {
        callbackF?.Invoke();
    }

    // Update is called once per frame
    private void Update()
    {
        callback?.Invoke();
        lateCallback?.Invoke();
    }
}

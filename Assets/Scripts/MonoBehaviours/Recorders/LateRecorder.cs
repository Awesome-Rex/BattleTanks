using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateRecorder : MonoBehaviour
{
    public System.Action earlyCallbackF;
    public System.Action callback;

    public System.Action earlyCallback;
    public System.Action callbackF;

    private void FixedUpdate()
    {
        earlyCallbackF?.Invoke();
        callbackF?.Invoke();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        earlyCallback?.Invoke();
        callback?.Invoke();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.REXCore
{
    //[ExecuteAlways]
    public class EarlyRecorder : MonoBehaviour
    {
        public System.Action earlyCallback;
        public System.Action earlyCallbackF;

        public System.Action callback;
        public System.Action callbackF;

        public System.Action lateCallback;
        public System.Action lateCallbackF;

        private void FixedUpdate()
        {
            earlyCallbackF?.Invoke();
            callbackF?.Invoke();
            lateCallbackF?.Invoke();
        }

        // Update is called once per frame
        private void Update()
        {
            earlyCallback?.Invoke();
            callback?.Invoke();
            lateCallback?.Invoke();
        }
    }
}
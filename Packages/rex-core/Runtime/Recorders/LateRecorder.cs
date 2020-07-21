using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.REXCore
{
    //[ExecuteAlways]
    public class LateRecorder : MonoBehaviour
    {
        public System.Action earlyCallbackF;
        public System.Action earlyCallback;

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
        void LateUpdate()
        {
            earlyCallback?.Invoke();
            callback?.Invoke();
            lateCallback?.Invoke();
        }
    }
}
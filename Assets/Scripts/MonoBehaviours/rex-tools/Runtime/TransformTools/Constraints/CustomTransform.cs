using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TransformTools
{
    public enum Curve { Linear, Interpolate }
    public enum Link { Offset, Match }
    public enum LocalRelativity { Natural, Constraint }

    public abstract class CustomTransform<T> : MonoBehaviour
    {
        //properties
        public Space space = Space.Self;

        public Transform parent;
        public T value;

        protected T previous;

        //methods
        public abstract T GetTarget();

        public abstract void SetPrevious();

        public abstract void Switch(Space newSpace, Link newLink);

        public abstract void SwitchParent(Transform newParent);

        protected virtual void Awake()
        {
            SetPrevious();

            _ETERNAL.I.lateRecorder.callbackF += SetPrevious;
        }

        protected virtual void OnDestroy()
        {
            _ETERNAL.I.lateRecorder.callbackF -= SetPrevious;
        }

        //Accessed in editor
        protected bool showContextInfo = false;
        protected bool showMethods = false;
    }
}
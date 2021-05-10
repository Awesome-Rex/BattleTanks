using REXTools.REXCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.CustomTransforms
{
    public enum Link { Offset, Match }
    public enum LocalRelativity { Natural, Constraint }

    public abstract class CustomTransform<T> : MonoBehaviour
    {
        //properties
        public Space space = Space.Self;

        public Transform parent;
        public T value; //original position/rotation/direction in world space

        protected T previous;

        //methods
        public abstract T GetTarget();

        public abstract void SetPrevious();

        public abstract void Switch(Space newSpace, Link newLink);

        public abstract void SwitchParent(Transform newParent);
        
        public abstract void SwitchParent(Vector3 newPosition, Quaternion newRotation, Vector3 newScale);
        public void SwitchParent(Vector3 newPosition, Quaternion newRotation)
        {
            SwitchParent(newPosition, newRotation, Vector3.one);
        }
        public void SwitchParent(Vector3 newPosition)
        {
            SwitchParent(newPosition, Quaternion.Euler(Vector3.zero));
        }

        public abstract void SwitchParent(System.Func<Vector3> newPosition, System.Func<Quaternion> newRotation, System.Func<Vector3> newScale);
        public void SwitchParent(System.Func<Vector3> newPosition, System.Func<Quaternion> newRotation)
        {
            //++++++++ will change default scale parameter depending on current parent referencetype
            //referencetype == transform => set delegate to og transform scale
            //referencetype == constant => set delegate to return og constant scale

            //SwitchParent(newPosition, newRotation, )
        }
        public void SwitchParent(System.Func<Vector3> newPosition)
        {
            
        }

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
        public bool showContextInfo = false;
        public bool showMethods = false;
    }
}
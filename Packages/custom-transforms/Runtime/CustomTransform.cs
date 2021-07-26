using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.REXCore;
using REXTools.TransformTools;

namespace REXTools.CustomTransforms
{
    public enum Link { Offset, Match }
    public enum LocalRelativity { Natural, Constraint }

    public abstract class CustomTransform<T> : MonoBehaviour
    {
        //properties
        public Space space = Space.World;

        //public Transform parent;
        public TransformObject parent
        {
            get
            {
                if (_parent != null)
                {
                    if (!_parent.isNull) {
                        return _parent;
                    } else
                    {
                        return null;
                    }
                } else
                {
                    return null;
                }
            }
        }
        [SerializeField]
        public TransformObject _parent;

        public T value; //original position/rotation/direction in world space

        
        protected T previous;

        //methods
        public abstract T GetTarget();

        public abstract void SetPrevious();

        //inspector method
        public abstract void Switch(Space newSpace, Link newLink);

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
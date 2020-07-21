using REXTools.Referencing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.REXCore
{
    [RequireComponent(typeof(ScriptReference), typeof(Tagged))]
    public abstract class MonoBehaviourPRO : MonoBehaviour
    {
        public Tagged T
        {
            get
            {
                return SR.Get<Tagged>();
            }
        }

        public ScriptReference SR
        {
            get
            {
                if (_SR == null)
                {
                    _SR = base.GetComponent<ScriptReference>();
                }

                return _SR;
            }
        }
        private ScriptReference _SR;

        public new T GetComponent<T>()
        {
            return SR.Get<T>();
        }

        public T ResourcesLoad<T>(string path) where T : Object
        {
            return _ETERNAL.I.resourceReference.Load<T>(path);
        }
    }
}
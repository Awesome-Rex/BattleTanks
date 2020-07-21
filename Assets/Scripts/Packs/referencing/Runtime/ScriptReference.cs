using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.Referencing
{
    public class ScriptReference : MonoBehaviour
    {
        public Dictionary<System.Type, dynamic> components;

        public TYPE Get<TYPE>()
        {
            if (components == null)
            {
                components = new Dictionary<System.Type, dynamic>();
            }

            if (!Viable<TYPE>())
            {
                Remove<TYPE>();

                Add<TYPE>();
            }

            return (TYPE)(components[typeof(TYPE)]);
        }

        public bool Viable<TYPE>()
        {
            dynamic instance = null;

            components.TryGetValue(typeof(TYPE), out instance);

            if (instance != null)
            { // does exist -> check value -> return value
                if (((Component)instance).gameObject != gameObject)
                {
                    return false;
                }
            }
            else // doesn't exist -> add
            {
                return false;
            }

            return true;
        }

        public void Remove<TYPE>()
        {
            components.Remove(typeof(TYPE));
        }

        public void Add<TYPE>()
        {
            components.Add(typeof(TYPE), GetComponent<TYPE>());
        }
    }
}
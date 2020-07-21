using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.Referencing
{
    public class ResourceReference : MonoBehaviour
    {
        public Dictionary<string, dynamic> resources;

        public TYPE Load<TYPE>(string path) where TYPE : Object
        {
            if (!Viable<TYPE>(path))
            {
                Remove(path);

                Add<TYPE>(path);
            }
            return (TYPE)(resources[path]);
        }

        public bool Viable<TYPE>(string path) where TYPE : Object
        {
            dynamic instance = null;

            resources.TryGetValue(path, out instance);

            if (instance != null)
            { // does exist -> check value -> return value
                if ((TYPE)instance == null)
                {
                    return false;
                }
            }
            else // doesn't exist -> add
            {
                /*if (!Available<TYPE>(path))
                {
                    return false;
                }*/

                return false;
            }

            return true;
        }

        public bool Available<TYPE>(string path) where TYPE : Object
        {
            if (Resources.Load<TYPE>(path))
            {
                return false;
            }

            return true;
        }

        public void Remove(string path)
        {
            resources.Remove(path);
        }

        public void Add<TYPE>(string path) where TYPE : Object
        {
            resources.Add(path, (Resources.Load<TYPE>(path)));
        }

        private void Awake()
        {
            resources = new Dictionary<string, dynamic>();
        }
    }
}
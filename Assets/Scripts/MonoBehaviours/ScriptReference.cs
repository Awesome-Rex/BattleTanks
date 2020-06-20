using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptReference : MonoBehaviour
{
    public Tag T;

    public Dictionary<System.Type, dynamic> components;

    public TYPE Get<TYPE>()
    {
        if (!Viable<TYPE>())
        {
            Remove<TYPE>();

            Add<TYPE>();
        }

        return components[typeof(TYPE)];
    }

    public bool Viable<TYPE> ()
    {
        dynamic instance = null;

        if (components.TryGetValue(typeof(TYPE), out instance))
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

    private void Awake()
    {
        T = GetComponent<Tag>();

        components = new Dictionary<System.Type, dynamic>();
    }
}

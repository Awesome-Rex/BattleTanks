using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

[RequireComponent(typeof(ScriptReference), typeof(Tagged))]
public class MonoBehaviourPRO : MonoBehaviour
{
    public Tagged T
    {
        get
        {
            if (_T == null)
            {
                _T = base.GetComponent<Tagged>();
                
            }
            return _T;
        }
    }
    private Tagged _T;
    
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

    public new T GetComponent<T> ()
    {
        return SR.Get<T>();
    }
}

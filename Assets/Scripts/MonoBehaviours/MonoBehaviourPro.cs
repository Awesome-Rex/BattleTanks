﻿using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

[RequireComponent(typeof(ScriptReference))]
public class MonoBehaviourPRO : MonoBehaviour
{
    [HideInInspector]
    public Tagged T
    {
        get
        {
            if (_T == null)
            {
                _T = GetComponent<Tagged>();
            }

            return _T;
        }
    }
    private Tagged _T;


    [HideInInspector]
    public ScriptReference SR
    {
        get
        {
            if (_SR == null)
            {
                _SR = GetComponent<ScriptReference>();
            }

            return _SR;
        }
    }

    private ScriptReference _SR;

    public new T GetComponent<T> ()
    {
        return SR.Get<T>();
    }

    protected void Awake()
    {
        _SR = base.GetComponent<ScriptReference>();
        _T = base.GetComponent<Tagged>();
    }
}

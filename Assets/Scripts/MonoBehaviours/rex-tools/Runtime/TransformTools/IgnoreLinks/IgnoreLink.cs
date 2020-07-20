using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IgnoreLink : MonoBehaviour
{
    public abstract void MoveToTarget();
    public abstract void SetPrevious();

    protected void Awake()
    {
        _ETERNAL.I.lateRecorder.lateCallbackF += SetPrevious;
        _ETERNAL.I.earlyRecorder.earlyCallbackF += MoveToTarget;

        SetPrevious();
    }

    protected void OnDestroy()
    {
        _ETERNAL.I.lateRecorder.lateCallbackF -= SetPrevious;
        _ETERNAL.I.earlyRecorder.earlyCallbackF -= MoveToTarget;
    }
}

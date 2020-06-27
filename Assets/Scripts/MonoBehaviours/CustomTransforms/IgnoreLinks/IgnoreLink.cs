using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IgnoreLink : MonoBehaviour
{
    public abstract void MoveToTarget();
    public abstract void SetPrevious();

    protected void Awake()
    {
        _ETERNAL.R.lateRecorder.lateCallbackF += SetPrevious;
        _ETERNAL.R.earlyRecorder.earlyCallbackF += MoveToTarget;

        SetPrevious();
    }

    protected void OnDestroy()
    {
        _ETERNAL.R.lateRecorder.lateCallbackF -= SetPrevious;
        _ETERNAL.R.earlyRecorder.earlyCallbackF -= MoveToTarget;
    }
}

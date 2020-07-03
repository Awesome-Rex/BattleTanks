using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class CustomTransformLinks<T> : CustomTransform<T>
{
    protected T target;

    public bool follow = false;
    public Transition transition;
    
    public Link link = Link.Offset;
    
    public AxisOrder offset;  //local

    public bool applyInEditor = false;
    public bool editorApply
    {
        get
        {
            if (EditorApplication.isPaused || !EditorApplication.isPlaying)
            {
                return applyInEditor;
            }
            else if (Application.isPlaying)
            {
                return false;
            }

            return false;
        }
    }

    //components
    protected new Rigidbody rigidbody;

    //methods
    public abstract void SetToTarget();

    public abstract void MoveToTarget();

    public abstract void RecordParent();


    protected override void Awake ()
    {
        //base.Awake();
        if (editModeLoop != null)
        {
            StopCoroutine(editModeLoop);
        }
        //applyInEditor = false;


        _ETERNAL.I.earlyRecorder.callbackF += MoveToTarget;

        RecordParent();
    }

    protected override void OnDestroy()
    {
        //base.OnDestroy();
        _ETERNAL.I.earlyRecorder.callbackF -= MoveToTarget;
    }

    private IEnumerator EditModeLoop ()
    {
        while (true)
        {
            SetToTarget();
            
            //yield return new (Time.fixedDeltaTime * 2f);
        }
    }
    private Coroutine editModeLoop;
    protected virtual void OnDrawGizmos()
    {
        if (editorApply)
        {
            if (editModeLoop == null) {
                editModeLoop = StartCoroutine(EditModeLoop());

                Debug.Log("Started Loop!");
            }
        } else
        {
            if (editModeLoop != null) {
                StopCoroutine(editModeLoop);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Unity.EditorCoroutines.Editor;
#endif

public enum LinkSpace
{
    World, Self, WorldRaw, SelfRaw
}

[RequireComponent(typeof(CustomTransformEditorExecuter))]
public abstract class CustomTransformLinks<T> : CustomTransform<T>
{
    protected T target;

    public bool follow = false;
    public Transition transition;
    
    public Link link = Link.Offset;
    
    public AxisOrder offset = new AxisOrder(null, TransformTools.SpaceVariety.Mixed);  //local

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

    public abstract void TargetToCurrent();

    public abstract void RemoveOffset();

    public abstract void RecordParent();

    protected override void Awake ()
    {
        //base.Awake();
        if (editModeLoop != null) //stops loop in play mode
        {
            EditorCoroutineUtility.StopCoroutine(editModeLoop);
            editModeLoop = null;
        }

        _ETERNAL.I.earlyRecorder.callbackF += MoveToTarget;

        RecordParent();
    }

    protected override void OnDestroy()
    {
        //base.OnDestroy();
        _ETERNAL.I.earlyRecorder.callbackF -= MoveToTarget;

        if (editModeLoop != null) {
            EditorCoroutineUtility.StopCoroutine(editModeLoop);
            editModeLoop = null;
        }
    }

    public void SyncEditModeLoops ()
    {
        foreach (CustomPosition i in GetComponents<CustomPosition>())
        {
            EditorCoroutineUtility.StopCoroutine(i.editModeLoop);
            i.editModeLoop = null;

            i.EditorApplyCheck();
        }
        foreach (CustomRotation i in GetComponents<CustomRotation>())
        {
            EditorCoroutineUtility.StopCoroutine(i.editModeLoop);
            i.editModeLoop = null;

            i.EditorApplyCheck();
        }
    }

    private IEnumerator EditModeLoop ()
    {
        while (true)
        {
            SetToTarget();
            
            yield return new EditorWaitForSeconds(Time.fixedDeltaTime/* * 2f*/);
        }
    }
    private EditorCoroutine editModeLoop;
    public void EditorApplyCheck()
    {
        //Starts loop during editor or pause
        if (editorApply) // starts loop if not already looping
        {
            if (editModeLoop == null)
            {
                SetPrevious();

                RecordParent();

                editModeLoop = EditorCoroutineUtility.StartCoroutineOwnerless(EditModeLoop()/*, this*/);
            }
        }
        else //stops loop if exists
        {
            if (editModeLoop != null)
            {
                EditorCoroutineUtility.StopCoroutine(editModeLoop);
                editModeLoop = null;
            }
        }
    }
}

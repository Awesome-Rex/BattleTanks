using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using REXTools.REXCore;
using REXTools.TransformTools;

#if UNITY_EDITOR
using UnityEditor;
using Unity.EditorCoroutines.Editor;
#endif

namespace REXTools.CustomTransforms
{
    public enum LinkSpace
    {
        World, Self, WorldRaw, SelfRaw
    }

    [ExecuteAlways]
    public abstract class CustomTransformLinks<T> : CustomTransform<T>
    {
        protected T target;

        public bool follow = false;
        public Transition transition;

        public Link link = Link.Offset;

        public AxisOrder offset = new AxisOrder(null, SpaceVariety.Mixed);  //local

        public bool applyInEditor = false;
        public bool editorApply
        {
            get
            {
#if UNITY_EDITOR
                if (EditorApplication.isPaused || !EditorApplication.isPlaying)
                {
                    return applyInEditor;
                }
#endif

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

#if UNITY_EDITOR
        private void EditorStateChanged(PlayModeStateChange n)
        {
            EditorApplyCheck();
        }
        private void EditorStateChanged(PauseState n)
        {
            EditorApplyCheck();
        }
#endif

        protected override void Awake()
        {
            if (!inEditor)
            {
#if UNITY_EDITOR
                //base.Awake();
                if (editModeLoop != null) //stops loop in play mode
                {
                    EditorCoroutineUtility.StopCoroutine(editModeLoop);
                    editModeLoop = null;
                }
#endif

                _ETERNAL.I.earlyRecorder.callbackF += MoveToTarget;

                RecordParent();
            }
        }

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += EditorStateChanged;
            EditorApplication.pauseStateChanged += EditorStateChanged;
#endif

#if UNITY_EDITOR
            EditorApplyCheck();
#endif
        }

        protected override void OnDestroy()
        {
#if UNITY_EDITOR
            //removes subscribed methods
            EditorApplication.playModeStateChanged -= EditorStateChanged;
            EditorApplication.pauseStateChanged -= EditorStateChanged;
#endif

            if (!inEditor)
            {
                //base.OnDestroy();
                _ETERNAL.I.earlyRecorder.callbackF -= MoveToTarget;
            }

            //removes coroutines
            if (editModeLoop != null)
            {
                EditorCoroutineUtility.StopCoroutine(editModeLoop);
                editModeLoop = null;
            }
        }

        public void SyncEditModeLoops()
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

#if UNITY_EDITOR
        private IEnumerator EditModeLoop()
        {
            while (true)
            {
                SetToTarget();

                yield return new EditorWaitForSeconds(Time.fixedDeltaTime/* * 2f*/);
            }
        }
#endif
        private EditorCoroutine editModeLoop;
#if UNITY_EDITOR
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
#endif

        private bool inEditor
        {
            get
            {
                bool temp = false;

#if UNITY_EDITOR
                if (!EditorApplication.isPlaying || EditorApplication.isPaused)
                {
                    temp = true;
                }
#endif
                return temp;
            }
        }
    }
}
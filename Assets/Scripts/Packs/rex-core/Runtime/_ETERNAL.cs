using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using REXTools.Referencing;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace REXTools.REXCore
{
    [RequireComponent(typeof(EarlyRecorder), typeof(LateRecorder), typeof(ResourceReference))]
    public class _ETERNAL : MonoBehaviour
    {
        public static _ETERNAL I;

        public GameSettings gameSettings;
        public GameEditorSettings gameEditorSettings;

        //children
        public bool transformableUsed;
        private Transform transformable;

        //even/odd frames
        public bool counter;


        //component references
        public ResourceReference resourceReference;

        public LateRecorder lateRecorder;
        public EarlyRecorder earlyRecorder;

        public void UseTransformable(Action<Transform> modifier)
        {
            if (!transformableUsed)
            {
                transformableUsed = true;

                transformable.transform.position = Vector3.zero;
                transformable.transform.eulerAngles = Vector3.zero;
                transformable.localScale = Vector3.one;

                modifier(transformable.transform);

                transformableUsed = false;
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            I = this;

            //children
            transformableUsed = false;
            transformable = GameObject.FindGameObjectWithTag("Transformable").transform;

            counter = false;

            //component references
            resourceReference = GetComponent<ResourceReference>();

            lateRecorder = GetComponent<LateRecorder>();
            earlyRecorder = GetComponent<EarlyRecorder>();


            //settings
            lateRecorder.lateCallbackF += () => counter = !counter;
        }

#if UNITY_EDITOR
        /*[CustomEditor(typeof(_ETERNAL))]
        public class E : EditorPRO<_ETERNAL>
        {
            protected override void DeclareProperties()
            {

            }

            //private void OnEnable()
            //{
            //    //Debug.Log("called");
            //    I = (_ETERNAL)target;
            //}

            //public override void OnInspectorGUI()
            //{
            //    base.OnInspectorGUI();
            //}

            /*protected override void OnEnable()
            {
                base.OnEnable();
                I = target;
            }
        }*/
#endif
    }
}
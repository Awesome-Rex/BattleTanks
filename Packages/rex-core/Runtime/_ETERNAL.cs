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
        public static _ETERNAL I
        {
            get
            {
                if (_I != null)
                {
                    return _I;
                }
                else
                {
                    _I = Instantiate(Resources.Load("_ETERNAL") as GameObject).GetComponent<_ETERNAL>();
                    //Instantiate(Resources.Load("_ETERNAL") as GameObject).GetComponent<_ETERNAL>();
                    _I.transform.SetAsFirstSibling();
                    return _I;
                }
            }
        }
        private static _ETERNAL _I;

        //Scriptable Objects
        [HideInInspector] public GameSettings gameSettings;
        [HideInInspector] public GameEditorSettings gameEditorSettings;

        //Children
        [HideInInspector] public bool transformableUsed;
        [HideInInspector] private Transform transformable;

        //Component References
        [HideInInspector] public ResourceReference resourceReference;

        [HideInInspector] public LateRecorder lateRecorder;
        [HideInInspector] public EarlyRecorder earlyRecorder;

        //Properties

        //even/odd frames
        //public bool counter;
        public bool counter
        {
            get
            {
                if (Time.frameCount > 0)
                {
                    //alternates between frames
                    return Time.frameCount % 2 == 0;
                }
                else
                {
                    //return false on awake
                    return false;
                }
            }
        }

        //Methods
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

        //public bool CollisionEnter ()
        //{

        //}

        // Start is called before the first frame update
        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            //children
            transformableUsed = false;
            transformable = GameObject.FindGameObjectWithTag("REX_Transformable").transform;

            //component references
            resourceReference = GetComponent<ResourceReference>();

            lateRecorder = GetComponent<LateRecorder>();
            earlyRecorder = GetComponent<EarlyRecorder>();


            //settings
            //lateRecorder.lateCallbackF += () => counter = !counter;
        }

        //private void OnDestroy() //TESTING
        //{
        //    lateRecorder.earlyCallback = null;
        //    lateRecorder.earlyCallbackF = null;

        //    lateRecorder.callback = null;
        //    lateRecorder.callbackF = null;

        //    lateRecorder.lateCallback = null;
        //    lateRecorder.lateCallbackF = null;



        //    earlyRecorder.earlyCallback = null;
        //    earlyRecorder.earlyCallbackF = null;

        //    earlyRecorder.callback = null;
        //    earlyRecorder.callbackF = null;

        //    earlyRecorder.lateCallback = null;
        //    earlyRecorder.lateCallbackF = null;
        //}

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
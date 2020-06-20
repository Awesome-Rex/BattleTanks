using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

[System.Serializable]
public class CustomRotation : CustomTransformLinks<Quaternion>
{
    public new AxisOrder globalOffset = new AxisOrder();

    [ContextMenu("Apply to target")]
    public override void ApplyToTarget()
    {
        if (space == Space.Self && link == Link.Match)
        {
            Debug.LogWarning("Cannot apply to target position if link is set to \"match!\"", gameObject);
            return;
        }

        transform.rotation = GetTarget();
    }

    public override void SetTarget ()
    {
        if (enabled) {
            if (!follow || link == Link.Match)
            {
                transform.rotation = GetTarget();
            }
            else
            {
                if (transition.type == Curve.Interpolate)
                {
                    transform.rotation = transition.MoveTowards(transform.rotation, GetTarget());
                }
                else if (transition.type == Curve.Custom)
                {

                }
            }
        }
    }

    public override Quaternion GetTarget()
    {
        Quaternion target = Quaternion.Euler(Vector3.zero);

        if (space == Space.World)
        {
            target = value;
        }
        else if (space == Space.Self)
        {
            if (link == Link.Offset)
            {
                Quaternion temp = transform.rotation;

                transform.rotation = parent.rotation;
                transform.Rotate(value.eulerAngles, Space.Self);
                
                //transform

                target = transform.rotation;
                transform.rotation = temp;
            } else if (link == Link.Match)
            {
                target = parent.rotation * previous;
            }
        }

        return target;
    }

    public override void SetPrevious ()
    {
        previous = Quaternion.Inverse(parent.rotation) * transform.rotation;
    }

    protected override void Awake()
    {
        SetPrevious();

        _ETERNAL.r.lateRecorder.callback += SetPrevious;
        _ETERNAL.r.earlyRecorder.callback += SetTarget;
    }

    protected override void OnDestroy()
    {
        _ETERNAL.r.lateRecorder.callback -= SetPrevious;
        _ETERNAL.r.earlyRecorder.callback -= SetTarget;
    }

    private void Start()
    {
        
    }



#if UNITY_EDITOR
    [CustomEditor(typeof(CustomRotation))]
    public class E : Editor {
        private SerializedProperty transitionP;
        //private SerializedProperty axisOrder;

        private void OnEnable()
        {
            transitionP = serializedObject.FindProperty("transition");
            //axisOrder = serializedObject.FindProperty("axisOrder");
        }

        public override void OnInspectorGUI()
        {
            var t = (CustomRotation)target;


            EditorGUILayout.LabelField("Space", EditorStyles.boldLabel);
            t.space = (Space)EditorGUILayout.EnumPopup(t.space);

            EditorGUILayout.Space();
            
            if (t.space == Space.Self) t.parent = (Transform)EditorGUILayout.ObjectField("Parent", t.parent, typeof(Transform), true);

            t.value = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", t.value.eulerAngles));

            EditorGUILayout.Space();

            if (t.space == Space.Self)
            {
                EditorGUILayout.LabelField("Local Space", EditorStyles.boldLabel);

                t.link = (Link)EditorGUILayout.EnumPopup("Link", t.link);

                if (t.link == Link.Offset)
                {
                    //t.globalOffset = Quaternion.Euler(EditorGUILayout.Vector3Field("Global Offset", t.globalOffset.eulerAngles));
                    //EditorGUILayout.PropertyField(axisOrder);

                    //EditorGUILayout.BeginHorizontal();

                    
                    /*foreach (Axis i in t.axisOrder)
                    {
                    }*/
                }
            }

            EditorGUILayout.Space();
            
            t.follow = EditorGUILayout.Toggle("Transitioning", t.follow);
            if (t.follow)
            {
                //t.transition = (Transition)EditorGUILayout.ObjectField("Transition", t.transition, typeof(Transition), true);
                EditorGUILayout.PropertyField(transitionP);
            }
        }
    }
#endif
}

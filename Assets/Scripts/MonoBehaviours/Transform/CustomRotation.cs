using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRotation : MonoBehaviour
{
    public Space space = Space.Self;

    public bool follow = false;
    public Transition transition;

    //global
    public Quaternion rotation;

    //local
    public Transform parent;

    public Link link = Link.Offset;

    public Vector3 localOffset;
    public Vector3 globalOffset;

    //previous
    private Quaternion previous;

    [ContextMenu("Apply to target")]
    public void ApplyToTarget()
    {
        transform.rotation = GetTarget();
    }

    public void SetTarget ()
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

    public Quaternion GetTarget()
    {
        Quaternion target = Quaternion.Euler(Vector3.zero);

        if (space == Space.World)
        {
            target = rotation;
        }
        else if (space == Space.Self)
        {
            if (link == Link.Offset)
            {
                Quaternion temp = transform.rotation;

                transform.rotation = parent.rotation;
                transform.Rotate(localOffset, Space.Self);
                transform.Rotate(globalOffset, Space.World);

                target = transform.rotation;
                transform.rotation = temp;
            } else if (link == Link.Match)
            {
                target = parent.rotation * previous;
            }
        }

        return target;
    }

    public void SetPrevious ()
    {
        previous = Quaternion.Inverse(parent.rotation) * transform.rotation;
    }

    private void Awake()
    {
        SetPrevious();

        _ETERNAL.r.lateRecorder.callback += SetPrevious;
        _ETERNAL.r.earlyRecorder.callback += SetTarget;
    }

    /*private void OnEnable()
    {
        _ETERNAL.r.earlyRecorder.callback += SetTarget;
    }

    private void OnDisable()
    {
        _ETERNAL.r.earlyRecorder.callback -= SetTarget;
    }*/

    void Update()
    {
        transform.Rotate(new Vector3(90f, 0f, 0f) * Time.deltaTime);
    }

    private void OnDestroy()
    {
        _ETERNAL.r.lateRecorder.callback -= SetPrevious;
        _ETERNAL.r.earlyRecorder.callback -= SetTarget;
    }
}

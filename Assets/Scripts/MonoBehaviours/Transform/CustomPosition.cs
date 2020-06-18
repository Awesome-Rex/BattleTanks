using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;

public class CustomPosition : MonoBehaviour
{
    public Space space = Space.Self;

    public bool follow = false;
    public Transition transition;

    //global
    public Vector3 position;

    //local
    public Transform parent;

    public Link link = Link.Offset;

    public Vector3 localOffset;
    public Vector3 globalOffset;


    //previous
    private Vector3 previous;

    [ContextMenu("Apply to target")]
    public void ApplyToTarget ()
    {
        if (!(space == Space.Self && link == Link.Match)) {
            transform.position = GetTarget();
        } else
        {
            Debug.LogWarning("Cannot apply to target position if link is set to \"match!\"", gameObject);
        }
    }

    public void SetTarget ()
    {
        if (enabled) {
            if (!follow || link == Link.Match)
            {
                transform.position = GetTarget();
            }
            else
            {
                if (transition.type == Curve.Interpolate)
                {
                    transform.position = transition.MoveTowards(transform.position, GetTarget());
                }
                else if (transition.type == Curve.Custom)
                {

                }
            }
        }
    }

    public Vector3 GetTarget()
    {
        Vector3 target = Vector3.zero;

        if (space == Space.World)
        {
            target = position;
        }
        else if (space == Space.Self)
        {
            if (link == Link.Offset) {
                target = parent.position + parent.TransformDirection(localOffset) + globalOffset;
            } else if (link == Link.Match)
            {
                target = parent.position + parent.TransformDirection(previous);
            }
        }

        return target;
    }


    public void SetPrevious ()
    {
        previous = parent.InverseTransformPoint(transform.position);
    }

    private void Awake()
    {
        SetPrevious();

        _ETERNAL.r.lateRecorder.callback += SetPrevious;
        _ETERNAL.r.earlyRecorder.callback += SetTarget;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(Vector3.forward * 1 * Time.deltaTime);
    }

    private void OnDestroy()
    {
        _ETERNAL.r.lateRecorder.callback -= SetPrevious;
        _ETERNAL.r.earlyRecorder.callback -= SetTarget;
    }
}

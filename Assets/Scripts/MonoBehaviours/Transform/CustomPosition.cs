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
    public Vector3 localOffset;
    public Vector3 globalOffset;

    [ContextMenu("Apply to target")]
    public void ApplyToTarget ()
    {
        Vector3 target = Vector3.zero;

        if (space == Space.World)
        {
            target = position;
        }
        else if (space == Space.Self)
        {
            target = parent.position + parent.TransformDirection(localOffset) + globalOffset;
        }

        transform.position = target;
    }

    public void Apply()
    {
        Vector3 target = Vector3.zero;

        if (space == Space.World)
        {
            target = position;
        }
        else if (space == Space.Self)
        {
            target = parent.position + parent.TransformDirection(localOffset) + globalOffset;
        }


        if (!follow)
        {
            transform.position = target;
        }
        else
        {
            transform.position = transition.MoveTowards(transform.position, target);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Apply();
    }
}

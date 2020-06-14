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
    public Vector3 localOffset;
    public Vector3 globalOffset;

    [ContextMenu("Apply to target")]
    public void ApplyToTarget()
    {
        Quaternion target = Quaternion.Euler(Vector3.zero);

        if (space == Space.World)
        {
            target = rotation;
        }
        else if (space == Space.Self)
        {
            Quaternion temp = transform.rotation;

            transform.rotation = parent.rotation;
            transform.Rotate(localOffset, Space.Self);
            transform.Rotate(globalOffset, Space.World);

            target = transform.rotation;
            transform.rotation = temp;
        }

        transform.rotation = target;
    }

    public void Apply()
    {
        Quaternion target = Quaternion.Euler(Vector3.zero);

        if (space == Space.World)
        {
            target = rotation;
        }
        else if (space == Space.Self)
        {
            Quaternion temp = transform.rotation;

            transform.rotation = parent.rotation;
            transform.Rotate(localOffset, Space.Self);
            transform.Rotate(globalOffset, Space.World);

            target = transform.rotation;
            transform.rotation = temp;
        }

        if (!follow)
        {
            transform.rotation = target;
        }
        else
        {
            transform.rotation = transition.MoveTowards(transform.rotation, target);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Apply();
    }
}

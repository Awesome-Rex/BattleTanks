using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRotation : MonoBehaviour
{
    public Space space = Space.Self;

    //global
    public Quaternion rotation;

    //local
    public Transform parent;
    public Vector3 localOffset;
    public Vector3 globalOffset;

    [ContextMenu("Apply")]
    public void Apply ()
    {
        if (space == Space.World)
        {
            transform.rotation = rotation;
        }
        else if (space == Space.Self)
        {
            transform.rotation = parent.rotation;
            transform.Rotate(localOffset, Space.Self);
            transform.Rotate(globalOffset, Space.World);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Apply();
    }
}

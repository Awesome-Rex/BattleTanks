using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomPosition : MonoBehaviour
{
    public Space space = Space.Self;

    //global
    public Vector3 position;

    //local
    public Transform parent;
    public Vector3 localOffset;
    public Vector3 globalOffset;

    [ContextMenu("Apply")]
    public void Apply ()
    {
        if (space == Space.World)
        {
            transform.position = position;
        }
        else if (space == Space.Self)
        {
            transform.position = parent.position + parent.TransformDirection(localOffset) + globalOffset;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Apply();
    }
}

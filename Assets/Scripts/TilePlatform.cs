using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePlatform : MonoBehaviour
{

    public Vector3 forward;
    public Vector3 upwards;

    [ContextMenu("Set orietentation to self")]
    public void SetToSelf ()
    {
        forward = transform.forward;
        upwards = transform.up;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

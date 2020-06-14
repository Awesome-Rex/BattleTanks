using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    public bool applied
    {
        get
        {
            return _applied;
        }

        set
        {
            _applied = value;

            if (_applied)
            {
                //disable rigidhodies, add custom behaviour
            } else
            {
                //emable rigidbodies, disable this scirpt
            }
        }
    }
    protected bool _applied;

    public float gravity = -9.81f;
    public Vector3 direction = Vector3.down;

    public float gravityScale;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (applied)
        {

        }
    }
}

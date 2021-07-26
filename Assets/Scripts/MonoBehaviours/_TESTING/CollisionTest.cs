using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTest : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger");
    }

    // Start is called before the first frame update
    void Start()
    {
        //Collider[] colliders = Physics.OverlapBox(transform.position + transform.right.normalized, Vector3.one * /*0.5f*/ 0.49f, transform.rotation);

        //if (colliders.Length > 0)
        //{
        //    Debug.Log("Raycast");
        //}

        //Debug.Log((new Bounds(Vector3.zero, Vector3.one)).Intersects(new Bounds(Vector3.forward, Vector3.one)));
        //Debug.Log((new Bounds(Vector3.zero, Vector3.one)).Intersects(new Bounds(Vector3.forward, 0.99999999f * Vector3.one)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

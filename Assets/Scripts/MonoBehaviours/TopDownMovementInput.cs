using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.REXCore;
using REXTools.TransformTools;
using REXTools.CustomTransforms;

public class TopDownMovementInput : MonoBehaviourPRO
{
    public Vector3 planeDirection = new Vector3(0f, 1f, 0f);
    public Vector3 planeLocation = Vector3.zero;


    public Vector3 MoveDirection(Vector2 input)
    {
        //Vector3.ProjectOnPlane

        throw new System.NotImplementedException();
    }

    private void Update()
    {
        GetComponent<Rigidbody>().MovePosition(
            transform.position +
            ((new Plane(GetComponent<CustomPosition>().parent.up, Vector3.zero)).ProjectVector2(
                new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")),
                Camera.main.transform.rotation//, 
                //GetComponent<CustomPosition>().parent.forward
            ) * 5f * Time.deltaTime)
        );
    }
}

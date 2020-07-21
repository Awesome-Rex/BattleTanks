using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.Referencing;
using REXTools.TransformTools;

public class ForTESTING : MonoBehaviour
{
    public bool boolean;

    public AxisApplied testSerializable;

    private ScriptReference SR;

    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<ScriptReference>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        //SR.Get<CustomRotation>().forward = Vector3.right;


        //SR.Get<CustomPosition>().position = SR.Get<CustomPosition>().Translate(new Vector3(0.5f, 0f, 0f) * Time.deltaTime, Space.Self);

        //SR.Get<CustomRotation>().rotation = SR.Get<CustomRotation>().Rotate(new Vector3(0f, 0f, -90f) * Time.deltaTime);

        /*transform.Translate(new Vector3(Input.GetAxis("Horizontal") * 5f, Input.GetAxis("Vertical") * 5f, 0f) * Time.deltaTime, Space.World);


        if (boolean) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(SR.Get<DirectionRecorder>().estimatedDirection, -Vector3.right), 180f * Time.deltaTime);
        } else
        {
            transform.forward = SR.Get<DirectionRecorder>().estimatedDirection;
        }*/

        //if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) {
        //    //SR.Get<CustomGravity>().direction = (new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f)).normalized;
        //}
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    //stuff
        //    SR.Get<CustomPosition>().Switch(Space.Self, Link.Match, true);
        //    SR.Get<CustomRotation>().Switch(Space.Self, Link.Match, true);

        //    SR.Get<CustomGravity>().enabled = true;
        //    SR.Get<CustomGravity>().Switch(Space.Self, Link.Offset, true);

        //    //SR.Get<CustomPosition>().factorScale = false;
        //    //SR.Get<CustomPosition>().position = SR.Get<CustomPosition>().Translate(Vector3.right, Space.World);
        //}
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    //SR.Get<CustomPosition>().factorScale = true;
        //}
    }
}

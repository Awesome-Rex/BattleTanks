using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForTESTING : MonoBehaviour
{
    public bool boolean;

    private ScriptReference SR;

    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<ScriptReference>();
    }

    // Update is called once per frame
    void Update() {

        Debug.Log(SR.Get<DirectionRecorder>().positionHistory.ToArray().Length);
        if (boolean) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(SR.Get<DirectionRecorder>().estimatedDirection, -Vector3.right), 180f * Time.deltaTime);
        } else
        {
            transform.forward = SR.Get<DirectionRecorder>().estimatedDirection;

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //stuff

            //SR.Get<CustomPosition>().factorScale = false;
            //SR.Get<CustomPosition>().position = SR.Get<CustomPosition>().Translate(Vector3.right, Space.World);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            //SR.Get<CustomPosition>().factorScale = true;
        }
    }
}

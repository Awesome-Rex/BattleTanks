using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForTESTING : MonoBehaviour
{
    private ScriptReference SR;

    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<ScriptReference>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = SR.Get<CustomRotation>().SetRotation(new Vector3(45f, 45f, 0f), Space.Self);

        transform.position = SR.Get<CustomPosition>().Translate(new Vector3(5f, 2f, 0f) * Time.deltaTime, Space.Self);

        //Debug.Log(SR.Get<CustomRotation>().GetRotation(Space.World).eulerAngles);
    }
}

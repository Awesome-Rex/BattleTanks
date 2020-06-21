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
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //stuff

            SR.Get<CustomPosition>().factorScale = false;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            SR.Get<CustomPosition>().factorScale = true;
        }
    }
}

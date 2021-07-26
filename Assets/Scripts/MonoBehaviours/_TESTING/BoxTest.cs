using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTest : MonoBehaviour
{
    public Vector3 positionA = Vector3.zero;
    public Vector3 sizeA = Vector3.one;
    public Quaternion rotationA = Quaternion.identity;

    public Vector3 positionB = Vector3.zero;
    public Vector3 sizeB = Vector3.one;
    public Quaternion rotationB = Quaternion.identity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.GetChild(0).position = positionA;
        //transform.GetChild(0).localScale = sizeA/* * 2f*/;
        //transform.GetChild(0).rotation = rotationA;

        //transform.GetChild(1).position = positionB;
        //transform.GetChild(1).localScale = sizeB/* * 2f*/;
        //transform.GetChild(1).rotation = rotationB;

        //Debug.Log(Intersects(positionA, sizeA, rotationA, positionB, sizeB, rotationB));
    }

    private void OnDrawGizmos()
    {
        
    }
}

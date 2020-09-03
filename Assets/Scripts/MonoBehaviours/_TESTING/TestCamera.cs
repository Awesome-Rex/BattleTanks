using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

public class TestCamera : MonoBehaviour
{
    //resources
    public Transform pivotObject;

    //set
    [Space]
    public float speed = 5f;

    //dynamic
    [Space]
    public Space space;

    public Vector3 pivot
    {
        get
        {
            return pivotObject.transform.position;
        }
        set
        {
            pivotObject.transform.position = value;
        }
    }

    //get values
    public float radius
    {
        get
        {
            return Vector3.Distance(transform.position, pivotObject.transform.position);
        }
        set
        {
            Vector3 direction = transform.position - pivot;

            transform.position = pivot + (direction.normalized * value);
        }
    }
    public float circumference
    {
        get
        {
            return radius * 2f * Mathf.PI;
        }
    }



    public void Orbit(Vector3 degrees)
    {
        //Vector3 direction = 
        //    //new direction
        //    Quaternion.LookRotation(
        //        (transform.position + (transform.up * RMath.SignZeroed(degrees.y)) + (transform.right * RMath.SignZeroed(degrees.x))) - 
        //        pivot).eulerAngles -
        //    //current direction
        //    Quaternion.LookRotation(pivot - transform.position).eulerAngles;

        //direction = direction.normalized * degrees.magnitude;


        ////degrees = new Vector3(degrees.y, -degrees.x, degrees.z);
        
        //transform.position =
        //    Linking.TransformPoint(-Vector3.forward * radius, pivot,
        //    Quaternion.Euler(Quaternion.LookRotation(pivot - transform.position).eulerAngles + direction/*degrees*/));


    }

    public void Move(Vector3 units)
    {
        pivot += units;
        transform.position += units;
    }

    public void PushPivot(float distance)
    {
        Vector3 direction = pivot - transform.position;

        pivot = transform.position + (direction.normalized * distance);
    }

    public float units2Deg
    {
        get
        {
            return 360f / circumference;
        }
    }

    public float deg2Units
    {
        get
        {
            return circumference / 360f;
        }
    }


    void Update()
    {
        transform.forward = pivot - transform.position;

        //switches modes
        if (Input.GetKey(KeyCode.Keypad0))
        {
            if (space == Space.Self)
            {
                space = Space.World;
            } else if (space == Space.World)
            {
                space = Space.Self;
            }
        }

        //moves camera
        if (space == Space.Self) {
            if (Input.GetKey(KeyCode.Keypad5))
            {
                Orbit(Vector2.up * speed * units2Deg * Time.deltaTime);
            } else if (Input.GetKey(KeyCode.Keypad2))
            {
                Orbit(Vector2.down * speed * units2Deg * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.Keypad1))
            {
                Orbit(Vector2.left * speed * units2Deg * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.Keypad3))
            {
                Orbit(Vector2.right * speed * units2Deg * Time.deltaTime);
            }
        } else if (space == Space.World)
        {
            if (Input.GetKey(KeyCode.Keypad5))
            {
                Move(transform.up * speed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.Keypad2))
            {
                Move(-transform.up * speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.Keypad1))
            {
                Move(-transform.right * speed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.Keypad3))
            {
                Move(transform.right * speed * Time.deltaTime);
            }
        }

        //moving camera forward/back
        if (Input.GetKey(KeyCode.Keypad7))
        {
            radius -= speed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.Keypad4))
        {
            radius -= -speed * Time.deltaTime;
        }

        //moving pivot forward/back
        if (Input.GetKey(KeyCode.Keypad9))
        {
            PushPivot(radius + (speed * Time.deltaTime));
        }
        else if (Input.GetKey(KeyCode.Keypad6))
        {
            PushPivot(radius + (-speed * Time.deltaTime));
        }
    }
}

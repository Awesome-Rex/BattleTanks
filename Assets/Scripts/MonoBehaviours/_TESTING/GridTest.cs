using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.Referencing;
using REXTools.Tiling;
using REXTools.TransformTools;

public class GridTest : REXTools.REXCore.MonoBehaviourPRO
{
    public float distance = 2f;
    public int subdivisions = 2;
    public Vector3 origin = (Vector3.right * 0f) - Vector3.forward;
    public Vector3 direction = Vector3.right;
    public Ray ray
    {
        get
        {
            return new Ray(origin, direction);
        }
    }

    public void Start()
    {
        GetComponent<ScriptReference>().Add<GridOrientation>();
    }

    //STILL NEED TO TEST Distance of 0f and starting ray in plane
    public void OnDrawGizmos()
    {
        GridOrientationCastHit hit = SR.GetComponent<GridOrientation>().TileCast(ray, Axis.X, distance, subdivisions);
        Debug.Log(distance + " ==> " + SR.GetComponent<GridOrientation>().WorldToGridDistance(ray, distance, subdivisions));
        //Debug.Log(ray.direction + " ==> " + SR.GetComponent<GridOrientation>().WorldToGridDirection(ray.direction));

        Gizmos.color = Color.white;

        if (hit != null) {
            Debug.Log("Worked!!!!");
            //Debug.Log(hit.worldPoint);
            //Debug.Log(hit.worldDistance);
            //Debug.Log(SR.GetComponent<GridOrientation>().grid.OneToGridDistance(hit.distance, 2));

            Gizmos.DrawLine(ray.origin, hit.worldPoint);

            Gizmos.DrawSphere(ray.origin, 0.1f);
            Gizmos.DrawSphere(hit.worldPoint, 0.1f);
        }



        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(SR.GetComponent<GridOrientation>().GridToWorld(Vector3.zero, 3), 0.1f);
        Gizmos.DrawSphere(SR.GetComponent<GridOrientation>().GridToWorld(Vector3.right, 3), 0.1f);
        Gizmos.DrawSphere(SR.GetComponent<GridOrientation>().GridToWorld(Vector3.up, 3), 0.1f);
        Gizmos.DrawSphere(SR.GetComponent<GridOrientation>().GridToWorld(Vector3.forward, 3), 0.1f);



        Gizmos.color = Color.red;
        Gizmos.DrawSphere(SR.GetComponent<GridOrientation>().GridToWorld(SR.GetComponent<GridOrientation>().WorldToGrid(Vector3.one)), 0.1f);

        
    }
}

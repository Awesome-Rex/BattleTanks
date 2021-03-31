using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.Referencing;
using REXTools.Tiling;
using REXTools.TransformTools;

public class GridTest : REXTools.REXCore.MonoBehaviourPRO
{
    public void Start()
    {
        GetComponent<ScriptReference>().Add<GridOrientation>();
    }

    //STILL NEED TO TEST Distance of 0f and starting ray in plane
    /*public void OnDrawGizmos()
    {
        Ray ray = new Ray((Vector3.right * 0.8f) - Vector3.forward, Vector3.right);

        GridCastHit hit = SR.GetComponent<GridOrientation>().TileCast(ray, Axis.X, 10f, 2);

        Gizmos.color = Color.white;

        if (hit != null) {
            Debug.Log("Worked!!!!");
            Debug.Log(hit.point);

            Gizmos.DrawLine(ray.origin, hit.worldPoint);

            Gizmos.DrawSphere(ray.origin, 0.1f);
            Gizmos.DrawSphere(hit.worldPoint, 0.1f);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(SR.GetComponent<GridOrientation>().GridToWorld(Vector3.zero, 3), 0.1f);
        Gizmos.DrawSphere(SR.GetComponent<GridOrientation>().GridToWorld(Vector3.right, 3), 0.1f);
        Gizmos.DrawSphere(SR.GetComponent<GridOrientation>().GridToWorld(Vector3.up, 3), 0.1f);
        Gizmos.DrawSphere(SR.GetComponent<GridOrientation>().GridToWorld(Vector3.forward, 3), 0.1f);
    }*/
}

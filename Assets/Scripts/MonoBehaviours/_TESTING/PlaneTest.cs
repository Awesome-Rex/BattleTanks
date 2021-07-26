using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;
using REXTools.Tiling;

public class PlaneTest : MonoBehaviour
{
    public Vector3 planePoint;
    public Vector3 planeNormal;

    [Space]
    public Vector3 cameraOrigin;
    public Vector2 direction2;
    public Quaternion view;
    //public PlaneProjection projection;

    [Space]
    public Vector3 rayOrigin;
    public Vector3 rayDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        //Debug.Log(REXTools.Tiling.Grid.SnapPositionSubdivided(new Vector3(-0.4f, 0.4f, 0.3f), 1, 2));
        //Debug.Log(REXTools.Tiling.Grid.SnapPositionSubdivided(new Vector3(-0.4f, 0.4f, 0.3f), 1, 3));

        //Debug.Log(Vector3.Dot(planeNormal, rayDirection));
        //Debug.Log(Vector3.Dot(planeNormal.normalized, -planeNormal.normalized) == -1f);

        if (enabled) {
            //float dist;
            //Debug.Log("-> | <-" + (new Plane(Vector3.up, Vector3.down)).Raycast(new Ray(Vector3.up, Vector3.down), out dist) + " <=> " + dist);
            //Debug.Log("-> | ->" + (new Plane(-Vector3.up, Vector3.down)).Raycast(new Ray(Vector3.up, Vector3.down), out dist) + " <=> " + dist);
            //Debug.Log("<- | <-" + (new Plane(Vector3.up, Vector3.down)).Raycast(new Ray(Vector3.up, Vector3.up), out dist) + " <=> " + dist);
            //Debug.Log("<- | ->" + (new Plane(-Vector3.up, Vector3.down)).Raycast(new Ray(Vector3.up, Vector3.up), out dist) + " <=> " + dist);





            Plane plane = new Plane(planeNormal, planePoint);



            //plane
            Gizmos.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(plane.normal), Vector3.one);
            Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
            Gizmos.DrawCube(Vector3.zero, new Vector3(10f, 10f, 0.01f));

            //camera
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(cameraOrigin, 0.2f);

            Gizmos.matrix = Matrix4x4.TRS(cameraOrigin, view, Vector3.one);
            Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
            Gizmos.DrawCube(planePoint, new Vector3(2f, 2f, 0.01f));
            Gizmos.color = Color.magenta;
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.DrawLine(cameraOrigin, cameraOrigin + (view * direction2).normalized);
            //Gizmos.DrawLine(Vector3.zero, direction2.normalized);

            //Plane Center
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(plane.ClosestPointOnPlane(cameraOrigin), 0.2f);

            //Reorient (closest)
            Gizmos.DrawLine(plane.ClosestPointOnPlane(cameraOrigin), plane.ClosestPointOnPlane(cameraOrigin) + (plane.ProjectVector2(direction2, view, PlaneProjection.Reorient).normalized * 2f));
            Gizmos.DrawLine(cameraOrigin, plane.ClosestPointOnPlane(cameraOrigin));
            Gizmos.DrawLine(cameraOrigin + (view * direction2).normalized, plane.ClosestPointOnPlane(cameraOrigin + (view * direction2).normalized));

            Gizmos.matrix = Matrix4x4.TRS(plane.ClosestPointOnPlane(cameraOrigin), Quaternion.LookRotation(planeNormal, plane.ClosestPointOnPlane(view * Vector3.up).normalized), Vector3.one);
            Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
            Gizmos.DrawCube(Vector3.zero, new Vector3(2f, 2f, 0.01f));
            Gizmos.matrix = Matrix4x4.identity;

            //Reorient (ray)
            Gizmos.color = Color.magenta;
            //Gizmos.DrawLine((Vector3)plane.RaycastPoint(new Ray(cameraOrigin, view * Vector3.forward)), (Vector3)plane.RaycastPoint(new Ray(cameraOrigin, view * Vector3.forward)) + (plane.ProjectVector2(direction2, view, PlaneProjection.Reorient).normalized * 2f));

            //View
            Gizmos.DrawLine(plane.ClosestPointOnPlane(cameraOrigin), plane.ClosestPointOnPlane(cameraOrigin) + (plane.ProjectVector2(direction2, view, PlaneProjection.View).normalized * 2f));
            Gizmos.DrawLine((Vector3)plane.RaycastPoint(new Ray(cameraOrigin, view * Vector3.forward)), (Vector3)plane.RaycastPoint(new Ray(cameraOrigin, view * Vector3.forward)) + (plane.ProjectVector2(direction2, view, PlaneProjection.View).normalized * 2f));
            Gizmos.DrawLine(cameraOrigin, (Vector3)plane.RaycastPoint(new Ray(cameraOrigin, view * Vector3.forward)));
            Gizmos.DrawLine(cameraOrigin + (view * direction2).normalized, (Vector3)plane.RaycastPoint(new Ray(cameraOrigin + (view * direction2).normalized, view * Vector3.forward)));


            //ProjectRay()
            Gizmos.matrix = Matrix4x4.identity;
            Ray ray = new Ray(rayOrigin, rayDirection);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(rayOrigin, 0.2f);
            Gizmos.DrawLine(rayOrigin, plane.ClosestPointOnPlane(rayOrigin));
            Gizmos.DrawLine(rayOrigin, (Vector3)plane.RaycastPoint(ray));
            Gizmos.DrawLine(plane.ClosestPointOnPlane(rayOrigin), plane.ClosestPointOnPlane(rayOrigin) + (plane.ProjectRay(ray).normalized * 5f));
            //Debug.Log(plane.ProjectRay(ray).normalized);
        }
    }
}

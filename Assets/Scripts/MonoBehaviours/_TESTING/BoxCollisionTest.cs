using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

public class BoxCollisionTest : MonoBehaviour
{
    [Header("Bounds")]
    public Bounds boundsA;
    public Bounds boundsB;

    public BoxIntersectHit boundsIntersect;

    //[Space]
    [Header("Other")]
    public BoxAdjacentHit boundsAdjacentHit;
    public BoxContainHit boundsContainHit;

    [Space]
    [Space]
    [Space]
    [Space]
    [Space]
    [Header("Boxes")]
    public Box boxA;
    public Box boxB;

    public BoxIntersectHit boxIntersect;

    //[Space]
    [Header("Other")]
    public BoxAdjacentHit boxAdjacentHit;
    public BoxContainHit boxContainHit;

    [Header("EXTRA")]
    public Vector3 inside;
    public Vector3 adjacent;
    public Quaternion testQuaternion;

    [ContextMenu("Test Stuff")]
    void TestStuff() 
    {
        boxIntersect = boxA.IntersectsCast(boxB);
        if (boxIntersect != null)
        {
            Debug.Log("Intersect!");
            if (boxIntersect.adjacentHit != null)
            {
                Debug.Log("Adjacent!");
                boxAdjacentHit = boxIntersect.adjacentHit;
            }
            //else Debug.Log("Not adjacent");
            if (boundsIntersect.containHit != null)
            {
                Debug.Log("Contains!");
                boxContainHit = boxIntersect.containHit;
            }
            //else Debug.Log("Not contains");
        }
    }

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

        if (enabled)
        {
            //Debug.Log((new Bounds(Vector3.zero, Vector3.one - (Vector3.one * Geometry.faceTrim * 2f))).IntersectRay(new Ray(Vector3.left * -0.5f, Vector3.right)));
            //Debug.Log("EDGE ADJ: " + boxB.EdgeAdjacent(boxA.edges[new Vector3Sign(Sign.Negative, Sign.Positive, Sign.Neutral)].Key, boxA.edges[new Vector3Sign(Sign.Negative, Sign.Positive, Sign.Neutral)].Value));
            //Debug.Log(boxB.EdgeAdjacent(new Vector3(0.5f, 0f, -0.5f), new Vector3(0.5f, 0f, 0.5f)));

            //Debug.Log(boxA.edges[new Vector3Sign(Sign.Negative, Sign.Positive, Sign.Neutral)].Key.ToString("F10") + " <=> " + boxA.edges[new Vector3Sign(Sign.Negative, Sign.Positive, Sign.Neutral)].Value.ToString("F10"));

            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(boxA.edges[new Vector3Sign(Sign.Negative, Sign.Positive, Sign.Neutral)].Key, 0.15f);
            Gizmos.DrawSphere(boxA.edges[new Vector3Sign(Sign.Negative, Sign.Positive, Sign.Neutral)].Value, 0.15f);


            Gizmos.matrix = Matrix4x4.identity;

            Gizmos.color = new Color(1f, 1f, 1f, 0.2f);

            //boxA.orientation = Quaternion.Euler(boxA.orientation.eulerAngles);
            Gizmos.matrix = Matrix4x4.TRS(boxA.center, boxA.orientation, boxA.size);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);

            //boxB.orientation = Quaternion.Euler(boxB.orientation.eulerAngles);
            Gizmos.matrix = Matrix4x4.TRS(boxB.center, boxB.orientation, boxB.size);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);

            Gizmos.matrix = Matrix4x4.identity;

            foreach (var face in boxA.faces)
            {
                Gizmos.color = new Color(1f, 0f, 1f, 0.5f);
                Gizmos.matrix = Matrix4x4.TRS(face.Value.center, face.Value.orientation, face.Value.size);
                Gizmos.DrawCube(Vector3.zero, Vector3.one * 0.8f);
            }
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.blue;
            foreach (var edge in boxA.edges)
            {
                Gizmos.DrawLine(edge.Value.Key, edge.Value.Value);
            }
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(boxA.center, 0.1f);


            TestStuff();

            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.yellow;
            if (boxAdjacentHit.adjacency == BoxAdjacency.Edge)
            {
                Gizmos.DrawLine(boxA.edges[boxAdjacentHit.adjacencySign].Key, boxA.edges[boxAdjacentHit.adjacencySign].Value);
            }
            else if (boxAdjacentHit.adjacency == BoxAdjacency.Corner)
            {
                Gizmos.DrawSphere(boxA.corners[boxAdjacentHit.adjacencySign], 0.1f);
            }
            else if (boxAdjacentHit.adjacency == BoxAdjacency.Face)
            {
                Gizmos.matrix = Matrix4x4.TRS(boxA.faces[boxAdjacentHit.adjacencySign].center, boxA.faces[boxAdjacentHit.adjacencySign].orientation, boxA.faces[boxAdjacentHit.adjacencySign].size);
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
            boxIntersect = boxA.IntersectsCast(boxB);
            if (boxIntersect != null)
            {
                Debug.Log("Intersect!");
                if (boxIntersect.adjacentHit != null)
                {
                    Debug.Log("Adjacent!");
                    boxAdjacentHit = boxIntersect.adjacentHit;
                }
                //else Debug.Log("Not adjacent");
                if (boundsIntersect.containHit != null)
                {
                    Debug.Log("Contains!");
                    boxContainHit = boxIntersect.containHit;
                }
                //else Debug.Log("Not contains");
            }
        }
    }
}

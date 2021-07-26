using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.REXCore;
using REXTools.Referencing;
using REXTools.Tiling;
using REXTools.TransformTools;

public class GridTest : MonoBehaviourPRO
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

    [System.Serializable]
    public class TestClass : Component {
        public float val1;
        public bool val2;

        public TestClass(float val1, bool val2) {
            this.val1 = val1;
            this.val2 = val2;
        }
    }
    public TestClass testClass;

    public TestClass testClass2 = new TestClass(5, true);

    [Space]
    public Tile tile;
    public Vector3 tilePosition;

    [Space]
    public Bounds boundsA;
    public Bounds boundsB;

    [Space]
    public TileArea area;

    [Space]
    public Vector2Float vector2Float;
    public Vector2 vector2;
    public Vector2Bool vector2Bool;

    [Space]
    //public Vector3Float vector3Float;
    //public Vector3 vector3;
    //public UnityEngine.Vector3Int vector3Int;
    //public REXTools.TransformTools.Vector3Int vector3IntREX;
    //public Vector3Bool vector3Bool;
    public AxisApplied axisApplied;
    public Axis axis;

    private void Awake()
    {

    }

    void Start()
    {
        ///TileState tileInstance = tile.CreateTileMatch(SR.GetComponent<GridOrientation>(), tilePosition, 1, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
        //tileInstance.subdivisionsRaw = 1f;
        tile.CreateTileMatch(SR.GetComponent<GridOrientation>(), Vector3.one * 2, 2, Quaternion.Euler(new Vector3(0f, 0f, 0f)));

        tile.CreateTileMatch(SR.GetComponent<GridOrientation>(), new Vector3(0, 0, 0), 4, Quaternion.Euler(new Vector3(0f, 0f, 0f)));

        tile.CreateTileMatch(SR.GetComponent<GridOrientation>(), new Vector3(2, 2, -1), 3, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        tile.CreateTileMatch(SR.GetComponent<GridOrientation>(), new Vector3(2, 2, 0), 3, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        tile.CreateTileMatch(SR.GetComponent<GridOrientation>(), new Vector3(2, 2, 1), 3, Quaternion.Euler(new Vector3(0f, 0f, 0f)));

        //Debug.Log((new Bounds(Vector3.zero, Vector3.one)).Contains(Vector3.one * 0.5f));
        Debug.Log(new Bounds(Vector3.zero, Vector3.one).Intersects(new Bounds(Vector3.one, Vector3.one)));
    }

    void Update()
    {

    }

    //STILL NEED TO TEST Distance of 0f and starting ray in plane
    public void OnDrawGizmos()
    {
        //Debug.Log(area.isBox);

        if (enabled) {
            GridOrientationCastHit hit = SR.GetComponent<GridOrientation>().TileCast(ray, Axis.X, distance, subdivisions);
            //Debug.Log(distance + " ==> " + SR.GetComponent<GridOrientation>().WorldToGridDistance(ray, distance, subdivisions));
            //Debug.Log(ray.direction + " ==> " + SR.GetComponent<GridOrientation>().WorldToGridDirection(ray.direction));

            Gizmos.color = Color.white;

            if (hit != null)
            {
                //Debug.Log("Worked!!!!");
                //Debug.Log(hit.worldPoint);
                //Debug.Log(hit.worldDistance);
                //Debug.Log(SR.GetComponent<GridOrientation>().grid.OneToGridDistance(hit.distance, 2));

                Gizmos.DrawLine(ray.origin, hit.worldPoint);

                Gizmos.DrawSphere(ray.origin, 0.1f);
                Gizmos.DrawSphere(hit.worldPoint, 0.1f);
            }



            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(SR.GetComponent<GridOrientation>().GridToWorld(Vector3.zero, 6), 0.1f);
            Gizmos.DrawSphere(SR.GetComponent<GridOrientation>().GridToWorld(Vector3.right, 6), 0.1f);
            Gizmos.DrawSphere(SR.GetComponent<GridOrientation>().GridToWorld(Vector3.up, 6), 0.1f);
            Gizmos.DrawSphere(SR.GetComponent<GridOrientation>().GridToWorld(Vector3.forward, 6), 0.1f);

            //Gizmos.color = Color.grey;
            //Gizmos.DrawSphere(SR.GetComponent<GridOrientation>().GridToWorld(SR.GetComponent<GridOrientation>().WorldToGrid(SR.GetComponent<GridOrientation>().GridToWorld(Vector3.zero, 6), 6), 6), 0.05f);
            //Gizmos.DrawSphere(SR.GetComponent<GridOrientation>().GridToWorld(SR.GetComponent<GridOrientation>().WorldToGrid(SR.GetComponent<GridOrientation>().GridToWorld(Vector3.right, 6), 6), 6), 0.05f);
            //Gizmos.DrawSphere(SR.GetComponent<GridOrientation>().GridToWorld(SR.GetComponent<GridOrientation>().WorldToGrid(SR.GetComponent<GridOrientation>().GridToWorld(Vector3.up, 6), 6), 6), 0.05f);
            //Gizmos.DrawSphere(SR.GetComponent<GridOrientation>().GridToWorld(SR.GetComponent<GridOrientation>().WorldToGrid(SR.GetComponent<GridOrientation>().GridToWorld(Vector3.forward, 6), 6), 6), 0.05f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(SR.GetComponent<GridOrientation>().GridToWorld(SR.GetComponent<GridOrientation>().WorldToGrid(Vector3.one)), 0.1f);

            //INNER BOX
            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
            Gizmos.DrawCube(boundsA.center, boundsA.size);
            Gizmos.DrawCube(boundsB.center, boundsB.size);

            Gizmos.color = Color.red;
            Gizmos.DrawCube(boundsA.IntersectionArea(boundsB).center, boundsA.IntersectionArea(boundsB).size);

            //DEBUGGING

            //Debug.Log(area.islands.Count);
        }
    }
}

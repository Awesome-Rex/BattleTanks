using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using REXTools.TransformTools;

namespace REXTools.Tiling
{
    public abstract class TileState : MonoBehaviour
    {
        public Tile tile;
        public virtual GridOrientation grid
        {
            get
            {
                return _grid;
            }
            set
            {
                _grid = value;
            }
        }
        [SerializeField]
        protected GridOrientation _grid;

        public Vector3 offsetPosition = Vector3.zero;
        public Quaternion offsetRotation = Quaternion.identity;
        public Vector3 offsetScale = Vector3.one;

        //PROPERTIES

        //*ORDER OF OFFSETS:
        //  tile.prefab.transform.position
        //  tile.offsetPosition
        //  offsetPosition

        public UnityEngine.Vector3Int position
        {
            get
            {
                return Grid.SnapPositionToInt(positionRaw);
            }
            set
            {
                positionRaw = value;
            }
        }
        public abstract Vector3 positionRaw
        {
            get; set;
        } //grid position (factors subdivisions)
        public UnityEngine.Vector3Int rotation
        {
            get
            {
                //apply opposite offset rotation (and round)
                return Grid.SnapRotationToInt(rotationRaw);
            }
            set
            {
                rotationRaw = Quaternion.Euler(value * 90);
            }
        } //grid rotation
        public abstract Quaternion rotationRaw
        {
            get; set;
        }
        public int subdivisions
        {
            get
            {
                //apply offset scale (use reciprocol) (and round to nearest subdivision)
                return Grid.SnapSubdivisions(subdivisionsRaw);
            }
            set
            {
                subdivisionsRaw = value;
            }
        } //current subdivisions (rounded)
        public virtual float subdivisionsRaw
        {
            get
            {
                return Vector3.one.Divide(transform.localScale.Divide(/*grid != null ? */Vector3.one * grid.totalScale * grid.totalScale/* : Vector3.one*/).Divide(offsetScale).Divide(tile.offsetScale).Divide(offsetScale)).ToList().GetAverage(AverageType.Mean);
            }
            set
            {
                transform.localScale = tile.prefab.transform.localScale.Multiply(tile.offsetScale).Multiply(offsetScale).Multiply(/*grid != null ? */Vector3.one * grid.totalScale * grid.totalScale/* : Vector3.one*/).Divide(Vector3.one * value);
            }
        } //current subdivisions (acurrate)

        public void SnapPosition()
        {
            position = Grid.SnapPositionToInt(positionRaw);
        }
        public void SnapRotation()
        {
            rotation = Grid.SnapRotationToInt(rotationRaw);
        }
        public void SnapScale()
        {
            subdivisions = Grid.SnapSubdivisions(subdivisionsRaw);
        }



        //INTERSECTION
        //private static List<Geometry.Box> TileToArea(TileState tile, UnityEngine.Vector3Int position, UnityEngine.Vector3Int rotation, int subdivisions)
        //{
        //    List<Geometry.Box> area = new List<Geometry.Box>();

        //    if (tile.tile as AreaTile == null)
        //    {
        //        area.Add(new Geometry.Box(
        //            tile.grid.GridToWorld(
        //                Grid.SnapPosition(position),
        //                subdivisions
        //            ),
        //            Linking.TransformEuler(Grid.SnapRotation(Quaternion.Euler(rotation * 90), tile.grid.grid.canPivot), tile.grid.finalRotation),
        //            tile.grid.grid.subdividedTileSize(subdivisions) * tile.grid.totalScale
        //        ));
        //    }
        //    else
        //    {
        //        foreach (UnityEngine.Vector3Int occupation in (tile.tile as AreaTile).area)
        //        {
        //            area.Add(new Geometry.Box(
        //                Linking.TransformPoint(
        //                    tile.grid.GridToWorld(
        //                        Grid.SnapPosition(position),
        //                        subdivisions
        //                    ),
        //                    Vector3.zero,
        //                    Linking.TransformEuler(Grid.SnapRotation(Quaternion.Euler(rotation * 90), tile.grid.grid.canPivot), tile.grid.finalRotation),
        //                    tile.grid.grid.subdividedTileSize(subdivisions) * tile.grid.totalScale
        //                ),
        //                Linking.TransformEuler(Grid.SnapRotation(Quaternion.Euler(rotation * 90), tile.grid.grid.canPivot), tile.grid.finalRotation),
        //                tile.grid.grid.subdividedTileSize(subdivisions) * tile.grid.totalScale
        //            ));
        //        }
        //    }

        //    return area;
        //}
        //private static List<Geometry.Box> TileToAreaRaw(TileState tile, Vector3 position, Quaternion rotation, int subdivisions)
        //{
        //    List<Geometry.Box> area = new List<Geometry.Box>();

        //    if (tile.tile as AreaTile == null)
        //    {
        //        area.Add(new Geometry.Box(
        //            tile.grid.GridToWorld(
        //                position,
        //                subdivisions
        //            ),
        //            Linking.TransformEuler(rotation, tile.grid.finalRotation),
        //            tile.grid.grid.subdividedTileSize(subdivisions) * tile.grid.totalScale
        //        ));
        //    }
        //    else
        //    {
        //        foreach (UnityEngine.Vector3Int occupation in (tile.tile as AreaTile).area)
        //        {
        //            area.Add(new Geometry.Box(
        //                Linking.TransformPoint(
        //                    tile.grid.GridToWorld(
        //                        position,
        //                        subdivisions
        //                    ),
        //                    Vector3.zero,
        //                    Linking.TransformEuler(rotation, tile.grid.finalRotation),
        //                    tile.grid.grid.subdividedTileSize(subdivisions) * tile.grid.totalScale
        //                ),
        //                Linking.TransformEuler(rotation, tile.grid.finalRotation),
        //                tile.grid.grid.subdividedTileSize(subdivisions) * tile.grid.totalScale
        //            ));
        //        }
        //    }

        //    return area;
        //}
        //private static Geometry.Box AreaToBounding(List<Geometry.Box> area) {

        //    Bounds areaBounding = new Bounds(Linking.InverseTransformPoint(area[0].position, Vector3.zero, area[0].rotation), Vector3.zero);

        //    foreach (Geometry.Box occupation in area)
        //    {
        //        areaBounding.Encapsulate(Linking.InverseTransformPoint(occupation.position, Vector3.zero, occupation.rotation));
        //    }

        //    areaBounding.Expand(area[0].extents);

        //    return new Geometry.Box(Linking.TransformPoint(areaBounding.center, Vector3.zero, area[0].rotation), area[0].rotation, areaBounding.size);
        //}
        //private static bool IntersectsArea(List<Geometry.Box> a, List<Geometry.Box> b, bool trim = true)
        //{
        //    Geometry.Box boundingA = AreaToBounding(a);
        //    Geometry.Box boundingB = AreaToBounding(b);

        //    //escape if bounding boxes dont intersect
        //    if (!boundingA.Intersects(boundingB)) return false;

        //    Vector3 centroidA = boundingA.ClosestPoint(boundingB.position);
        //    Vector3 centroidB = boundingB.ClosestPoint(boundingA.position);

        //    //order tiles by order to other tiles
        //    a = a.OrderBy(occupation => Vector3.Distance(occupation.position, centroidB)).ToList();
        //    b = b.OrderBy(occupation => Vector3.Distance(occupation.position, centroidA)).ToList();

        //    //detect collision
        //    //return true if tiles intersect
        //    foreach (Geometry.Box occupationA in a)
        //    {
        //        foreach (Geometry.Box occupationB in b)
        //        {
        //            if (occupationA.Intersects(occupationB, trim))
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    //return false if tiles dont intersect
        //    return false;
        //}
        //private static bool AdjacentArea(List<Geometry.Box> a, List<Geometry.Box> b, bool useTilt = true, bool useShear = false)
        //{
        //    Geometry.Box boundingA = AreaToBounding(a);
        //    Geometry.Box boundingB = AreaToBounding(b);

        //    //escape if bounding boxes dont intersect
        //    if (!boundingA.Intersects(boundingB)) return false;

        //    Vector3 centroidA = boundingA.ClosestPoint(boundingB.center);
        //    Vector3 centroidB = boundingB.ClosestPoint(boundingA.center);

        //    //order tiles by order to other tiles
        //    a = a.OrderBy(occupation => Vector3.Distance(occupation.position, centroidB)).ToList();
        //    b = b.OrderBy(occupation => Vector3.Distance(occupation.position, centroidA)).ToList();

        //    //detect collision
        //    //return true if tiles intersect
        //    foreach (Geometry.Box occupationA in a)
        //    {
        //        foreach (Geometry.Box occupationB in b)
        //        {
        //            if (occupationA.Adjacent(occupationB, useTilt, useShear))
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    //return false if tiles dont intersect
        //    return false;
        //}
        


        ////Intersection between tiles with custom orientations
        //public static bool IntersectsRaw(TileState tileA, Vector3 positionA, Vector3 sizeA, Quaternion rotationA, int subdivisionsA, TileState tileB, Vector3 positionB, Vector3 sizeB, Quaternion rotationB, int subdivisionsB)
        //{
        //    List<Geometry.Box> areaA = TileToAreaRaw(tileA, positionA, rotationA, subdivisionsA);
        //    List<Geometry.Box> areaB = TileToAreaRaw(tileB, positionB, rotationB, subdivisionsB);

        //    return IntersectsArea(areaA, areaB);
        //}
        //public static bool IntersectsRaw(TileState tileA, Vector3 positionA, Vector3 sizeA, int subdivisionsA, TileState tileB, Vector3 positionB, Vector3 sizeB, int subdivisionsB)
        //{
        //    return IntersectsRaw(tileA, positionA, sizeA, Quaternion.identity, subdivisionsA, tileB, positionB, sizeB, Quaternion.identity, subdivisionsB);
        //}
        //public static bool IntersectsRaw(TileState tileA, Vector3 positionA, int subdivisionsA, TileState tileB, Vector3 positionB, int subdivisionsB)
        //{
        //    return IntersectsRaw(tileA, positionA, Vector3.one, Quaternion.identity, subdivisionsA, tileB, positionB, Vector3.one, Quaternion.identity, subdivisionsB);
        //}
        //public static bool Intersects(TileState tileA, UnityEngine.Vector3Int positionA, UnityEngine.Vector3Int rotationA, int subdivisionsA, TileState tileB, UnityEngine.Vector3Int positionB, UnityEngine.Vector3Int rotationB, int subdivisionsB)
        //{
        //    return IntersectsRaw(tileA, positionA, Vector3.one, Quaternion.Euler(rotationA * 90), subdivisionsA, tileB, positionB, Vector3.one, Quaternion.Euler(rotationB * 90), subdivisionsB);
        //}
        //public static bool Intersects(TileState tileA, UnityEngine.Vector3Int positionA, int subdivisionsA, TileState tileB, UnityEngine.Vector3Int positionB, int subdivisionsB)
        //{
        //    return Intersects(tileA, positionA, UnityEngine.Vector3Int.zero, subdivisionsA, tileB, positionB, UnityEngine.Vector3Int.zero, subdivisionsB);
        //}

        ////Intersection between tiles in place
        //public static bool Intersects(TileState a, bool useRawA, TileState b, bool useRawB)
        //{
        //    List<Geometry.Box> areaA;
        //    List<Geometry.Box> areaB;
        //    if (!useRawA)
        //    {
        //        areaA = TileToArea(a, a.position, a.rotation, a.subdivisions);
        //    }
        //    else
        //    {
        //        areaA = TileToAreaRaw(a, a.positionRaw, a.rotationRaw, a.subdivisions);
        //    }
        //    if (!useRawB)
        //    {
        //        areaB = TileToArea(b, b.position, b.rotation, b.subdivisions);
        //    }
        //    else
        //    {
        //        areaB = TileToAreaRaw(b, b.positionRaw, b.rotationRaw, b.subdivisions);
        //    }

        //    return IntersectsArea(areaA, areaB);
        //}
        //public static bool Intersects(TileState a, TileState b)
        //{
        //    return Intersects(a, false, b, false);
        //}
        //public static bool IntersectsRaw(TileState a, TileState b)
        //{
        //    return Intersects(a, true, b, true);
        //}

        ////Intersection between tile and any other tile in the world
        //public bool IntersectsRaw(Vector3 position, int subdivisions, Quaternion rotation, bool useRaw = true)
        //{
        //    List<Geometry.Box> areaA = TileToAreaRaw(this, position, rotation, subdivisions);
        //    Bounds boundsA = new Bounds(areaA[0].position, Vector3.zero);
        //    foreach (Geometry.Box occupation in areaA)
        //    {
        //        boundsA.Encapsulate(occupation.position);
        //    }
        //    boundsA.Expand(areaA[0].extents * 2f);

        //    //check for all tiles within bounding box
        //    Collider[] tiles = Physics.OverlapBox(boundsA.center, boundsA.extents, Quaternion.identity, LayerMask.NameToLayer(!useRaw ? "REX_TileOccupation" : "REX_TileOccuptionRaw"), QueryTriggerInteraction.Collide);

        //    if (tiles == null || tiles.Length == 0)
        //    { //returns false from no tile collision at all
        //        return false;
        //    }
        //    else
        //    { //searches fro compatible tiles
        //        foreach (Collider tile in tiles)
        //        {
        //            TileState b = tile.GetComponentInParent<TileState>();

        //            List<Geometry.Box> areaB;
        //            if (!useRaw)
        //            {
        //                areaB = TileToArea(b, b.position, b.rotation, b.subdivisions);
        //            }
        //            else
        //            {
        //                areaB = TileToAreaRaw(b, b.positionRaw, b.rotationRaw, b.subdivisions);
        //            }

        //            if (IntersectsArea(areaA, areaB))
        //            {
        //                return true;
        //            }
        //        }

        //        return false; //returns false on no tiles found
        //    }
        //}
        //public bool IntersectsRaw(Vector3 position)
        //{
        //    return IntersectsRaw(position, subdivisions, rotationRaw);
        //}
        //public bool IntersectsRaw()
        //{
        //    return IntersectsRaw(positionRaw, subdivisions, rotationRaw);
        //}
        //public bool Intersects(UnityEngine.Vector3Int position, int subdivisions, UnityEngine.Vector3Int rotation, bool useRaw = false)
        //{
        //    List<Geometry.Box> areaA = TileToArea(this, position, rotation, subdivisions);
        //    Bounds boundsA = new Bounds(areaA[0].position, Vector3.zero);
        //    foreach (Geometry.Box occupation in areaA)
        //    {
        //        boundsA.Encapsulate(occupation.position);
        //    }
        //    boundsA.Expand(areaA[0].extents * 2f);

        //    Collider[] tiles = Physics.OverlapBox(boundsA.center, boundsA.extents, Quaternion.identity, LayerMask.NameToLayer(!useRaw ? "REX_TileOccupation" : "REX_TileOccuptionRaw"), QueryTriggerInteraction.Collide);

        //    if (tiles == null || tiles.Length == 0)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        foreach (Collider tile in tiles)
        //        {
        //            TileState b = tile.GetComponentInParent<TileState>();

        //            List<Geometry.Box> areaB;
        //            if (!useRaw)
        //            {
        //                areaB = TileToArea(b, b.position, b.rotation, b.subdivisions);
        //            }
        //            else
        //            {
        //                areaB = TileToAreaRaw(b, b.positionRaw, b.rotationRaw, b.subdivisions);
        //            }

        //            if (IntersectsArea(areaA, areaB))
        //            {
        //                return true;
        //            }
        //        }

        //        return false;
        //    }
        //}
        //public bool Intersects(UnityEngine.Vector3Int position)
        //{
        //    return Intersects(position, subdivisions, rotation);
        //}
        //public bool Intersects()
        //{
        //    return Intersects(position, subdivisions, rotation);
        //}



        ////Adjacency between tiles with custom orientations
        //public static bool AdjacentRaw(TileState tileA, Vector3 positionA, Vector3 sizeA, Quaternion rotationA, int subdivisionsA, TileState tileB, Vector3 positionB, Vector3 sizeB, Quaternion rotationB, int subdivisionsB)
        //{
        //    List<Geometry.Box> areaA = TileToAreaRaw(tileA, positionA, rotationA, subdivisionsA);
        //    List<Geometry.Box> areaB = TileToAreaRaw(tileB, positionB, rotationB, subdivisionsB);

        //    return AdjacentArea(areaA, areaB);
        //}
        //public static bool AdjacentRaw(TileState tileA, Vector3 positionA, Vector3 sizeA, int subdivisionsA, TileState tileB, Vector3 positionB, Vector3 sizeB, int subdivisionsB)
        //{
        //    return AdjacentRaw(tileA, positionA, sizeA, Quaternion.identity, subdivisionsA, tileB, positionB, sizeB, Quaternion.identity, subdivisionsB);
        //}
        //public static bool AdjacentRaw(TileState tileA, Vector3 positionA, int subdivisionsA, TileState tileB, Vector3 positionB, int subdivisionsB)
        //{
        //    return AdjacentRaw(tileA, positionA, Vector3.one, Quaternion.identity, subdivisionsA, tileB, positionB, Vector3.one, Quaternion.identity, subdivisionsB);
        //}
        //public static bool Adjacent(TileState tileA, UnityEngine.Vector3Int positionA, UnityEngine.Vector3Int rotationA, int subdivisionsA, TileState tileB, UnityEngine.Vector3Int positionB, UnityEngine.Vector3Int rotationB, int subdivisionsB)
        //{
        //    return AdjacentRaw(tileA, positionA, Vector3.one, Quaternion.Euler(rotationA * 90), subdivisionsA, tileB, positionB, Vector3.one, Quaternion.Euler(rotationB * 90), subdivisionsB);
        //}
        //public static bool Adjacent(TileState tileA, UnityEngine.Vector3Int positionA, int subdivisionsA, TileState tileB, UnityEngine.Vector3Int positionB, int subdivisionsB)
        //{
        //    return Adjacent(tileA, positionA, UnityEngine.Vector3Int.zero, subdivisionsA, tileB, positionB, UnityEngine.Vector3Int.zero, subdivisionsB);
        //}

        ////Adjacency between tiles in place
        //public static bool Adjacent(TileState a, bool useRawA, TileState b, bool useRawB)
        //{
        //    List<Geometry.Box> areaA;
        //    List<Geometry.Box> areaB;
        //    if (!useRawA)
        //    {
        //        areaA = TileToArea(a, a.position, a.rotation, a.subdivisions);
        //    }
        //    else
        //    {
        //        areaA = TileToAreaRaw(a, a.positionRaw, a.rotationRaw, a.subdivisions);
        //    }
        //    if (!useRawB)
        //    {
        //        areaB = TileToArea(b, b.position, b.rotation, b.subdivisions);
        //    }
        //    else
        //    {
        //        areaB = TileToAreaRaw(b, b.positionRaw, b.rotationRaw, b.subdivisions);
        //    }

        //    return AdjacentArea(areaA, areaB);
        //}
        //public static bool Adjacent(TileState a, TileState b)
        //{
        //    return Adjacent(a, false, b, false);
        //}
        //public static bool AdjacentRaw(TileState a, TileState b)
        //{
        //    return Adjacent(a, true, b, true);
        //}

        ////Adjacency between tile and any other tile in the world
        //public bool AdjacentRaw(Vector3 position, int subdivisions, Quaternion rotation, bool useRaw = true)
        //{
        //    List<Geometry.Box> areaA = TileToAreaRaw(this, position, rotation, subdivisions);
        //    Bounds boundsA = new Bounds(areaA[0].position, Vector3.zero);
        //    foreach (Geometry.Box occupation in areaA)
        //    {
        //        boundsA.Encapsulate(occupation.position);
        //    }
        //    boundsA.Expand(areaA[0].extents * 2f);

        //    //check for all tiles within bounding box
        //    Collider[] tiles = Physics.OverlapBox(boundsA.center, boundsA.extents, Quaternion.identity, LayerMask.NameToLayer(!useRaw ? "REX_TileOccupation" : "REX_TileOccuptionRaw"), QueryTriggerInteraction.Collide);

        //    if (tiles == null || tiles.Length == 0)
        //    { //returns false from no tile collision at all
        //        return false;
        //    }
        //    else
        //    { //searches fro compatible tiles
        //        foreach (Collider tile in tiles)
        //        {
        //            TileState b = tile.GetComponentInParent<TileState>();

        //            List<Geometry.Box> areaB;
        //            if (!useRaw)
        //            {
        //                areaB = TileToArea(b, b.position, b.rotation, b.subdivisions);
        //            }
        //            else
        //            {
        //                areaB = TileToAreaRaw(b, b.positionRaw, b.rotationRaw, b.subdivisions);
        //            }

        //            if (IntersectsArea(areaA, areaB))
        //            {
        //                return true;
        //            }
        //        }

        //        return false; //returns false on no tiles found
        //    }
        //}
        //public bool AdjacentRaw(Vector3 position)
        //{
        //    return IntersectsRaw(position, subdivisions, rotationRaw);
        //}
        //public bool AdjacentRaw()
        //{
        //    return IntersectsRaw(positionRaw, subdivisions, rotationRaw);
        //}
        //public bool Adjacent(UnityEngine.Vector3Int position, int subdivisions, UnityEngine.Vector3Int rotation, bool useRaw = false)
        //{
        //    List<Geometry.Box> areaA = TileToArea(this, position, rotation, subdivisions);
        //    Bounds boundsA = new Bounds(areaA[0].position, Vector3.zero);
        //    foreach (Geometry.Box occupation in areaA)
        //    {
        //        boundsA.Encapsulate(occupation.position);
        //    }
        //    boundsA.Expand(areaA[0].extents * 2f);

        //    Collider[] tiles = Physics.OverlapBox(boundsA.center, boundsA.extents, Quaternion.identity, LayerMask.NameToLayer(!useRaw ? "REX_TileOccupation" : "REX_TileOccuptionRaw"), QueryTriggerInteraction.Collide);

        //    if (tiles == null || tiles.Length == 0)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        foreach (Collider tile in tiles)
        //        {
        //            TileState b = tile.GetComponentInParent<TileState>();

        //            List<Geometry.Box> areaB;
        //            if (!useRaw)
        //            {
        //                areaB = TileToArea(b, b.position, b.rotation, b.subdivisions);
        //            }
        //            else
        //            {
        //                areaB = TileToAreaRaw(b, b.positionRaw, b.rotationRaw, b.subdivisions);
        //            }

        //            if (IntersectsArea(areaA, areaB))
        //            {
        //                return true;
        //            }
        //        }

        //        return false;
        //    }
        //}
        //public bool Adjacent(UnityEngine.Vector3Int position)
        //{
        //    return Intersects(position, subdivisions, rotation);
        //}
        //public bool Adjacent()
        //{
        //    return Intersects(position, subdivisions, rotation);
        //}


        //switch grid to new grid with same position, scale and rotation
        //(may snap position and rotation to new grid)
        public abstract void SwitchGrid(GridOrientation newGrid, bool snapSubdivisions = true, bool snapPosition = true, bool snapRotation = true);

        void Awake()
        {

        }
    }
}
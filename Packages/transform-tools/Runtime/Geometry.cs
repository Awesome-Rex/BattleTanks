using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace REXTools.TransformTools
{
    public enum BoxAlignment
    {
        Parallel, //same rotation
        Tilt,  //rotation off by 90 degree increments
        Shear, //parallel on 2 axes
        Free //not parallel
    }
    public enum Intersection { Contain, Intersect, Adjacent }
    public enum BoxAdjacency { Object, Face, Edge, Corner, None }
    public enum Containment { Inside, InsideAdjacent }

    public static class Geometry
    {
        public static float zeroRound = 0.0001f;
        public static float faceTrim = 0.0001f;

        //THE GEOMETRY BIBLE
        public readonly static Vector3Sign[] faceSigns = new Vector3Sign[]
        {
            new Vector3Sign(Sign.Negative, Sign.Neutral, Sign.Neutral),
            new Vector3Sign(Sign.Positive, Sign.Neutral, Sign.Neutral),
            new Vector3Sign(Sign.Neutral, Sign.Negative, Sign.Neutral),
            new Vector3Sign(Sign.Neutral, Sign.Positive, Sign.Neutral),
            new Vector3Sign(Sign.Neutral, Sign.Neutral, Sign.Negative),
            new Vector3Sign(Sign.Neutral, Sign.Neutral, Sign.Positive)
        };
        public readonly static Vector3Sign[] edgeSigns = new Vector3Sign[]
        {
            new Vector3Sign(Sign.Negative, Sign.Negative, Sign.Neutral),
            new Vector3Sign(Sign.Positive, Sign.Positive, Sign.Neutral),
            new Vector3Sign(Sign.Negative, Sign.Positive, Sign.Neutral),
            new Vector3Sign(Sign.Positive, Sign.Negative, Sign.Neutral),
            new Vector3Sign(Sign.Neutral, Sign.Negative, Sign.Negative),
            new Vector3Sign(Sign.Neutral, Sign.Positive, Sign.Positive),
            new Vector3Sign(Sign.Neutral, Sign.Negative, Sign.Positive),
            new Vector3Sign(Sign.Neutral, Sign.Positive, Sign.Negative),
            new Vector3Sign(Sign.Negative, Sign.Neutral, Sign.Negative),
            new Vector3Sign(Sign.Positive, Sign.Neutral, Sign.Positive),
            new Vector3Sign(Sign.Positive, Sign.Neutral, Sign.Negative),
            new Vector3Sign(Sign.Negative, Sign.Neutral, Sign.Positive)
        };
        public readonly static Vector3Sign[] cornerSigns = new Vector3Sign[]
        {
            new Vector3Sign(Sign.Negative, Sign.Negative, Sign.Negative),
            new Vector3Sign(Sign.Negative, Sign.Positive, Sign.Positive),
            new Vector3Sign(Sign.Negative, Sign.Negative, Sign.Positive),
            new Vector3Sign(Sign.Negative, Sign.Positive, Sign.Negative),
            new Vector3Sign(Sign.Positive, Sign.Negative, Sign.Negative),
            new Vector3Sign(Sign.Positive, Sign.Positive, Sign.Positive),
            new Vector3Sign(Sign.Positive, Sign.Negative, Sign.Positive),
            new Vector3Sign(Sign.Positive, Sign.Positive, Sign.Negative)
        };



        //0.00000001f

        //EXTENSION METHODS
        public static float volume(this Bounds bounds)
        {
            return bounds.size.x * bounds.size.y * bounds.size.z;
        }
        public static void volume(this Bounds bounds, float volume)
        {
            throw new System.NotImplementedException();
        }
        public static float surfaceArea(this Bounds bounds)
        {
            return (
                bounds.size.x * bounds.size.y +
                bounds.size.y * bounds.size.z +
                bounds.size.z * bounds.size.x
            ) * 2f;
        }
        public static void surfaceArea(this Bounds bounds, float surfaceArea)
        {
            throw new System.NotImplementedException();
        }

        public static Dictionary<Vector3Sign, Vector3> corners(this Bounds bounds)
        {
            Dictionary<Vector3Sign, Vector3> newCorners = new Dictionary<Vector3Sign, Vector3>();

            for (int x = -1; x <= 1; x += 2)
            {
                for (int y = -1; y <= 1; y += 2)
                {
                    for (int z = -1; z <= 1; z += 2)
                    {
                        newCorners.Add(
                            new Vector3(x, y, z).ToSign(), 
                            bounds.center + Vector3.one.Multiply(new Vector3(x, y, z)).Multiply(bounds.extents)
                        );
                    }
                }
            }

            return newCorners;
        }
        public static Dictionary<Vector3Sign, KeyValuePair<Vector3, Vector3>> edges(this Bounds bounds)
        {
            Dictionary<Vector3Sign, KeyValuePair<Vector3, Vector3>> newEdges = new Dictionary<Vector3Sign, KeyValuePair<Vector3, Vector3>>();

            foreach (Axis s in Vectors.axisIterate)
            {
                for (int axisA = -1; axisA <= 1; axisA += 2)
                {
                    for (int axisB = -1; axisB <= 1; axisB += 2)
                    {
                        newEdges.Add(
                            Vector3.zero.SetAxis(Vectors.axisPlanes[s].x, axisA).SetAxis(Vectors.axisPlanes[s].y, axisB).ToSign(), 
                            new KeyValuePair<Vector3, Vector3> (
                                bounds.center + Vector3.one.Multiply(Vector3.zero.SetAxis(Vectors.axisPlanes[s].x, axisA).SetAxis(Vectors.axisPlanes[s].y, axisB).SetAxis(s, 1f)).Multiply(bounds.extents),
                                bounds.center + Vector3.one.Multiply(Vector3.zero.SetAxis(Vectors.axisPlanes[s].x, axisA).SetAxis(Vectors.axisPlanes[s].y, axisB).SetAxis(s, -1f)).Multiply(bounds.extents)
                            )
                        );
                    }
                }
            }

            return newEdges;
        }
        public static Dictionary<Vector3Sign, Bounds> faces(this Bounds bounds)
        {
            Dictionary<Vector3Sign, Bounds> newFaces = new Dictionary<Vector3Sign, Bounds>();

            foreach (Axis s in Vectors.axisIterate)
            {
                for (int axis = -1; axis <= 1; axis += 2)
                {
                    newFaces.Add(
                        Vector3.zero.SetAxis(s, axis).ToSign(), 
                        new Bounds(
                            bounds.center + Vector3.one.Multiply(Vector3.zero.SetAxis(s, axis)).Multiply(bounds.extents), 
                            bounds.size.SetAxis(s, 0f)
                        )
                    );
                }
            }

            return newFaces;
        }
        public static Mesh mesh(this Bounds bounds)
        {
            throw new System.NotImplementedException();
        }

        public static Box ToBox(this Bounds bounds)
        {
            return new Box(bounds.center, Quaternion.identity, bounds.size);
        }
        
        public static Bounds IntersectionArea(this Bounds self, Bounds bounds)
        { //Returns inner box formed from intersection
            Bounds newBounds = new Bounds();

            newBounds.SetMinMax
            (
                Vector3.zero.Operate((s, a) =>
                {
                    return Mathf.Max(self.min.GetAxis(s), bounds.min.GetAxis(s));
                }),
                Vector3.zero.Operate((s, a) =>
                {
                    return Mathf.Min(self.max.GetAxis(s), bounds.max.GetAxis(s));
                })
            );

            return newBounds;
        }
        public static bool IntersectEdge(this Bounds self, Vector3 a, Vector3 b)
        {
            float d;
            bool result = self.IntersectRay(new Ray(a, b - a), out d);

            return result && d <= Vector3.Distance(a, b);
        }
        public static bool IntersectEdge(this Bounds self, Vector3 a, Vector3 b, out float distance)
        {
            float d;
            bool result = self.IntersectRay(new Ray(a, b - a), out d);

            distance = d;
            return result && d <= Vector3.Distance(a, b);
        }



        //INTERSECTION METHODS
        public static bool Intersects(this Bounds self, Vector3 point, bool trim = false, bool allowance = false)
        {
            return Vector3Bool.falsey.Operate((s, a) =>
            {
                return RMath.Intersects(point.GetAxis(s), self.min.GetAxis(s), self.max.GetAxis(s), trim, allowance);
            }) == Vector3Bool.truthy;
        }
        public static bool Adjacent(this Bounds self, Vector3 point, bool allowance = false)
        {
            if (self.Intersects(point, false, allowance) && !self.Intersects(point, true, allowance))
            {
                return new Vector3Bool(Vector3Bool.falsey.Operate((s, a) =>
                {
                    return RMath.Adjacent(point.GetAxis(s), self.min.GetAxis(s), self.max.GetAxis(s), allowance);
                })).anyTrue;
            }
            else
            {
                return false;
            }
        }
        public static bool Contains(this Bounds self, Vector3 point, bool trim = false, bool allowance = false)
        {
            return Vector3Bool.falsey.Operate((s, a) =>
            {
                return RMath.Contains(point.GetAxis(s), self.min.GetAxis(s), self.max.GetAxis(s), trim, allowance);
            }) == Vector3Bool.truthy;
        }

        public static bool Intersects(this Bounds self, Bounds bounds, bool trim = false, bool allowance = false)
        {
            return Vector3Bool.falsey.Operate((s, a) =>
            {
                return RMath.Intersects(self.min.GetAxis(s), self.max.GetAxis(s), bounds.min.GetAxis(s), bounds.max.GetAxis(s), trim, allowance);
            }) == Vector3Bool.truthy;
        }
        public static bool Adjacent(this Bounds self, Bounds bounds, bool allowance = false)
        {
            if (self.Intersects(bounds, false, allowance) && !self.Intersects(bounds, true, allowance) && !self.Contains(bounds, false, allowance))
            {
                return new Vector3Bool(Vector3Bool.falsey.Operate((s, a) =>
                {
                    return RMath.Adjacent(self.min.GetAxis(s), self.max.GetAxis(s), bounds.min.GetAxis(s), bounds.max.GetAxis(s), allowance);
                })).anyTrue;
            }
            else
            {
                return false;
            }
        }
        public static bool Contains(this Bounds self, Bounds bounds, bool trim = false, bool allowance = false)
        {
            return Vector3Bool.falsey.Operate((s, a) =>
            {
                return RMath.Contains(self.min.GetAxis(s), self.max.GetAxis(s), bounds.min.GetAxis(s), bounds.max.GetAxis(s), trim, allowance);
            }) == Vector3Bool.truthy;
        }

        public static IntersectHit IntersectsCast(this Bounds self, Vector3 point, bool trim = false, bool allowance = false)
        {
            if (self.Intersects(point, trim, allowance))
            {
                IntersectHit hit = new IntersectHit();

                //primary
                //hit.intersection =

                //hits
                hit.adjacentHit = self.AdjacentCast(point, allowance);
                hit.containHit = self.ContainsCast(point, trim, allowance);

                //primary
                if (hit.adjacentHit != null)
                {
                    hit.intersection = Intersection.Adjacent;
                }
                else if (hit.containHit != null)
                {
                    hit.intersection = Intersection.Contain;
                }
                else
                {
                    hit.intersection = Intersection.Intersect;
                }

                //attributes
                //...

                return hit;
            }
            else
            {
                return null;
            }
        }
        public static AdjacentHit AdjacentCast(this Bounds self, Vector3 point, bool allowance = false)
        {
            if (self.Adjacent(point, allowance))
            {
                AdjacentHit hit = new AdjacentHit();

                //primary
                Vector3Bool adjacentAxes = new Vector3Bool(Vector3Bool.falsey.Operate((s, a) =>
                {
                    return RMath.Adjacent(point.GetAxis(s), self.min.GetAxis(s), self.max.GetAxis(s), allowance);
                }));
                int axisCount = adjacentAxes.ToList().FindAll(axis => axis).Count;
                if (axisCount == 1)
                {
                    hit.adjacency = BoxAdjacency.Face;
                }
                else if (axisCount == 2)
                {
                    hit.adjacency = BoxAdjacency.Edge;
                }
                else if (axisCount == 3)
                {
                    hit.adjacency = BoxAdjacency.Corner;
                }

                //attributes
                hit.adjacencySign = (self.center - point).ToSign();
                hit.adjacencySign = new Vector3Sign(Vector3Sign.zero.Operate((s, a) =>
                {
                    return adjacentAxes.GetAxis(s) ? hit.adjacencySign.GetAxis(s) : Sign.Neutral;
                }));



                return hit;
            }
            else
            {
                return null;
            }
        }
        public static ContainHit ContainsCast(this Bounds self, Vector3 point, bool trim = false, bool allowance = false)
        {
            if (self.Contains(point, trim, allowance))
            {
                ContainHit hit = new ContainHit();

                //primary
                //hit.containment = 

                //hits
                hit.adjacentHits = new Dictionary<Vector3Sign, AdjacentHit>();
                foreach (KeyValuePair<Vector3Sign, Bounds> face in self.faces())
                {
                    hit.adjacentHits.Add(face.Key, face.Value.AdjacentCast(point, allowance));
                }

                int adjacentHitsCount = 0;
                foreach (KeyValuePair<Vector3Sign, AdjacentHit> adjacentHit in hit.adjacentHits)
                {
                    if (adjacentHit.Value != null)
                    {
                        adjacentHitsCount++;
                    }
                }

                //primary
                if (adjacentHitsCount > 0)
                {
                    hit.containment = Containment.InsideAdjacent;
                }
                else
                {
                    hit.containment = Containment.Inside;
                }

                //attributes
                if (adjacentHitsCount > 0)
                {
                    bool SetSign(BoxAdjacency adjacency, List<UnityEngine.Vector3Int> offsets, List<UnityEngine.Vector3Int> sign)
                    {
                        int adjacentHitsCount = 0;
                        foreach (KeyValuePair<Vector3Sign, AdjacentHit> adjacentHit in hit.adjacentHits)
                        {
                            if (adjacentHit.Value != null)
                            {
                                adjacentHitsCount++;
                            }
                        }

                        if (adjacentHitsCount == offsets.Count + 1)
                        {
                            foreach (KeyValuePair<Vector3Sign, AdjacentHit> adjacentHit in hit.adjacentHits)
                            {
                                if (adjacentHit.Value != null)
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        bool match = true;
                                        foreach (UnityEngine.Vector3Int offset in offsets)
                                        {
                                            if (hit.adjacentHits[adjacentHit.Key.Rotate(offset).Rotate(adjacentHit.Key.ToInt() * i, Space.World)] == null)
                                            {
                                                match = false;
                                            }
                                        }

                                        if (match)
                                        {
                                            hit.adjacency = adjacency;
                                            
                                            hit.adjacencySign = new Vector3Sign(adjacentHit.Key.Clone());
                                            foreach (UnityEngine.Vector3Int offset in offsets)
                                            {
                                                hit.adjacencySign = (hit.adjacencySign.ToInt() + adjacentHit.Key.Rotate(offset).Rotate(adjacentHit.Key.ToInt() * i, Space.World).ToInt()).ToSign();
                                            }

                                            return true;
                                        }
                                    }
                                }
                            }
                        }

                        return false;
                    }

                    //Face
                    SetSign( //partial
                        BoxAdjacency.Face, 
                        new List<UnityEngine.Vector3Int>()
                        {

                        },
                        new List<UnityEngine.Vector3Int>()
                        {
                            UnityEngine.Vector3Int.forward
                        }
                    );
                    //Edge
                    SetSign( //partial
                        BoxAdjacency.Edge, 
                        new List<UnityEngine.Vector3Int>()
                        {
                            UnityEngine.Vector3Int.right
                        },
                        new List<UnityEngine.Vector3Int>()
                        {
                            UnityEngine.Vector3Int.forward,
                            UnityEngine.Vector3Int.right
                        }
                    );
                    //Corner
                    SetSign(
                        BoxAdjacency.Corner, 
                        new List<UnityEngine.Vector3Int>()
                        {
                            UnityEngine.Vector3Int.right,
                            UnityEngine.Vector3Int.up
                        },
                        new List<UnityEngine.Vector3Int>()
                        {
                            UnityEngine.Vector3Int.forward,
                            UnityEngine.Vector3Int.right,
                            UnityEngine.Vector3Int.up
                        }
                    );
                }

                
                hit.leanSign = (point - self.center).ToSign();

                return hit;
            }
            else
            {
                return null;
            }
        }

        public static BoxIntersectHit IntersectsCast(this Bounds self, Bounds bounds, bool trim = false, bool allowance = false)
        {
            if (self.Intersects(bounds, trim, allowance))
            {
                BoxIntersectHit hit = new BoxIntersectHit();

                //primary
                //hit.intersection =
                hit.alignment = BoxAlignment.Parallel;

                //hits
                hit.adjacentHit = self.AdjacentCast(bounds, allowance);
                hit.containHit = self.ContainsCast(bounds, trim, allowance);

                //primary
                if (hit.adjacentHit != null)
                {
                    hit.intersection = Intersection.Adjacent;
                }
                else if (hit.containHit != null)
                {
                    hit.intersection = Intersection.Contain;
                }
                else
                {
                    hit.intersection = Intersection.Intersect;
                }

                //attributes
                hit.volume = self.IntersectionArea(bounds).volume();
                hit.volumeRatio = hit.volume / bounds.volume();
                hit.dominant = hit.volumeRatio > 0.5f;



                int cornerCount = 0;
                foreach (KeyValuePair<Vector3Sign, Vector3> corner in self.corners())
                {
                    if (bounds.Contains(corner.Value, trim, allowance))
                    {
                        cornerCount++;
                    }
                }
                if (cornerCount == 8)
                {
                    hit.type = BoxAdjacency.Object;
                }
                else if (cornerCount == 4)
                {
                    hit.type = BoxAdjacency.Face;
                }
                else if (cornerCount == 2)
                {
                    hit.type = BoxAdjacency.Edge;
                }
                else if (cornerCount == 1)
                {
                    hit.type = BoxAdjacency.Corner;
                }
                else if (cornerCount == 0)
                {
                    hit.type = BoxAdjacency.None;
                }

                return hit;
            }
            else
            {
                return null;
            }
        }
        public static BoxAdjacentHit AdjacentCast(this Bounds self, Bounds bounds, bool allowance = false)
        {
            if (self.Adjacent(bounds))
            {
                BoxAdjacentHit hit = new BoxAdjacentHit();

                //primary
                Vector3Bool adjacentAxes = new Vector3Bool(Vector3Bool.falsey.Operate((s, a) =>
                {
                    return RMath.Adjacent(self.min.GetAxis(s), self.max.GetAxis(s), bounds.min.GetAxis(s), bounds.max.GetAxis(s), allowance);
                }));
                int axisCount = adjacentAxes.ToList().FindAll(axis => axis).Count;
                if (axisCount == 1)
                {
                    hit.adjacency = BoxAdjacency.Face;
                }
                else if (axisCount == 2)
                {
                    hit.adjacency = BoxAdjacency.Edge;
                }
                else if (axisCount == 3)
                {
                    hit.adjacency = BoxAdjacency.Corner;
                }
                hit.alignment = BoxAlignment.Parallel;

                //attributes
                hit.adjacencySign = (bounds.center - self.center).ToSign();
                hit.adjacencySign = new Vector3Sign(Vector3Sign.zero.Operate((s, a) =>
                {
                    return adjacentAxes.GetAxis(s) ? hit.adjacencySign.GetAxis(s) : Sign.Neutral;
                }));
                //hit.secondaryAdjacencySign = 
                if (hit.adjacency == BoxAdjacency.Face)
                {
                    hit.surfaceArea = self.IntersectionArea(bounds).surfaceArea() / 2f;
                    hit.surfaceAreaRatio = hit.surfaceArea / (bounds.faces()[hit.adjacencySign.Negative()].surfaceArea() / 2f);
                    hit.dominant = hit.surfaceAreaRatio > 0.5f;
                }
                else
                {
                    hit.surfaceArea = 0f;
                    hit.surfaceAreaRatio = 0f;
                    hit.dominant = false;
                }

                int cornerCount = 0;
                foreach (KeyValuePair<Vector3Sign, Vector3> corner in self.corners())
                {
                    if (bounds.Adjacent(corner.Value, allowance))
                    {
                        cornerCount++;
                    }
                }
                if (cornerCount == 4)
                {
                    hit.type = BoxAdjacency.Face;
                }
                else if (cornerCount == 2)
                {
                    hit.type = BoxAdjacency.Edge;
                }
                else if (cornerCount == 1)
                {
                    hit.type = BoxAdjacency.Corner;
                }



                return hit;
            }
            else
            {
                return null;
            }
        }
        public static BoxContainHit ContainsCast(this Bounds self, Bounds bounds, bool trim = false, bool allowance = false)
        {
            if (self.Contains(bounds, trim, allowance))
            {
                BoxContainHit hit = new BoxContainHit();

                //primary
                //hit.containment = 
                hit.alignment = BoxAlignment.Parallel;

                //hits
                hit.adjacentHits = new Dictionary<Vector3Sign, AdjacentHit>();
                foreach (KeyValuePair<Vector3Sign, Bounds> face in bounds.faces())
                {
                    hit.adjacentHits.Add(face.Key, self.AdjacentCast(face.Value, allowance));
                }

                int adjacentHitsCount = 0;
                foreach (KeyValuePair<Vector3Sign, AdjacentHit> adjacentHit in hit.adjacentHits)
                {
                    if (adjacentHit.Value != null)
                    {
                        adjacentHitsCount++;
                    }
                }

                //primary
                if (adjacentHitsCount > 0)
                {
                    hit.containment = Containment.InsideAdjacent;
                }
                else
                {
                    hit.containment = Containment.Inside;
                }

                //attributes
                if (adjacentHitsCount > 0)
                {
                    if (hit.alignment == BoxAlignment.Parallel || hit.alignment == BoxAlignment.Tilt)
                    {
                        bool SetSign(BoxAdjacency adjacency, bool whole, List<UnityEngine.Vector3Int> offsets, List<UnityEngine.Vector3Int> sign)
                        {
                            int adjacentHitsCount = 0;
                            foreach (KeyValuePair<Vector3Sign, AdjacentHit> adjacentHit in hit.adjacentHits)
                            {
                                if (adjacentHit.Value != null)
                                {
                                    adjacentHitsCount++;
                                }
                            }

                            if (adjacentHitsCount == offsets.Count + 1) {
                                foreach (KeyValuePair<Vector3Sign, AdjacentHit> adjacentHit in hit.adjacentHits)
                                {
                                    if (adjacentHit.Value != null)
                                    {
                                        for (int i = 0; i < 4; i++)
                                        {
                                            bool match = true;
                                            foreach (UnityEngine.Vector3Int offset in offsets)
                                            {
                                                if (hit.adjacentHits[adjacentHit.Key.Rotate(offset).Rotate(adjacentHit.Key.ToInt() * i, Space.World)] == null)
                                                {
                                                    match = false;
                                                }
                                            }

                                            if (match)
                                            {
                                                hit.adjacency = adjacency;
                                                hit.adjacencyWhole = whole;

                                                hit.adjacencySign = new Vector3Sign(adjacentHit.Key.Clone());
                                                foreach (UnityEngine.Vector3Int offset in offsets)
                                                {
                                                    hit.adjacencySign = (hit.adjacencySign.ToInt() + adjacentHit.Key.Rotate(offset).Rotate(adjacentHit.Key.ToInt() * i, Space.World).ToInt()).ToSign();
                                                }

                                                return true;
                                            }
                                        }
                                    }
                                }
                            }

                            return false;
                        }

                        //Object
                        SetSign(
                            BoxAdjacency.Object, true,
                            new List<UnityEngine.Vector3Int>()
                            {
                                UnityEngine.Vector3Int.back,
                                UnityEngine.Vector3Int.right,
                                UnityEngine.Vector3Int.left,
                                UnityEngine.Vector3Int.up,
                                UnityEngine.Vector3Int.down
                            },
                            new List<UnityEngine.Vector3Int>()
                            {
                                UnityEngine.Vector3Int.forward,
                                UnityEngine.Vector3Int.back,
                                UnityEngine.Vector3Int.right,
                                UnityEngine.Vector3Int.left,
                                UnityEngine.Vector3Int.up,
                                UnityEngine.Vector3Int.down
                            }
                        );
                        //Face
                        SetSign( //partial
                            BoxAdjacency.Face, false,
                            new List<UnityEngine.Vector3Int>()
                            { 
                                
                            }, 
                            new List<UnityEngine.Vector3Int>() 
                            { 
                                UnityEngine.Vector3Int.forward
                            }
                        );
                        SetSign( //whole
                            BoxAdjacency.Face, true,
                            new List<UnityEngine.Vector3Int>()
                            {
                                UnityEngine.Vector3Int.right,
                                UnityEngine.Vector3Int.left,
                                UnityEngine.Vector3Int.up,
                                UnityEngine.Vector3Int.down
                            },
                            new List<UnityEngine.Vector3Int>()
                            {
                                UnityEngine.Vector3Int.forward
                            }
                        );
                        //Edge
                        SetSign( //partial
                            BoxAdjacency.Edge, false,
                            new List<UnityEngine.Vector3Int>()
                            {
                                UnityEngine.Vector3Int.right
                            },
                            new List<UnityEngine.Vector3Int>()
                            {
                                UnityEngine.Vector3Int.forward,
                                UnityEngine.Vector3Int.right
                            }
                        );
                        SetSign( //whole
                            BoxAdjacency.Edge, true,
                            new List<UnityEngine.Vector3Int>()
                            {
                                UnityEngine.Vector3Int.right,

                                UnityEngine.Vector3Int.up,
                                UnityEngine.Vector3Int.down,
                            },
                            new List<UnityEngine.Vector3Int>()
                            {
                                UnityEngine.Vector3Int.forward,
                                UnityEngine.Vector3Int.right
                            }
                        );
                        //Corner
                        SetSign(
                            BoxAdjacency.Corner, true,
                            new List<UnityEngine.Vector3Int>()
                            {
                                UnityEngine.Vector3Int.right,
                                UnityEngine.Vector3Int.up
                            },
                            new List<UnityEngine.Vector3Int>()
                            {
                                UnityEngine.Vector3Int.forward,
                                UnityEngine.Vector3Int.right,
                                UnityEngine.Vector3Int.up
                            }
                        );
                    }
                    else
                    {
                        hit.adjacency = BoxAdjacency.None;
                    }
                }

                hit.volume = self.volume();
                hit.volumeRatio = self.volume() / bounds.volume();
                hit.dominant = hit.volumeRatio > 0.5f;
                hit.leanSign = (self.center - bounds.center).ToSign();

                return hit;
            }
            else
            {
                return null;
            }
        }



        public static void Rotate(this Bounds bounds, UnityEngine.Vector3Int eulers)
        {
            //bounds.size = Linking.InverseTransformPoint(bounds.extents, Vector3.zero, Quaternion.Euler((Vector3)eulers * 90f)).Operate((s, a) => Mathf.Abs(a)) * 2f;
            bounds.size = RotateScale(bounds.size, eulers);
        }

        public static bool CornerAdjacent(this Bounds self, Vector3 point, bool allowance = false)
        {
            return self.Adjacent(point, allowance);
        }
        public static bool EdgeAdjacent(this Bounds self, Vector3 a, Vector3 b, bool allowance = false)
        {
            Bounds selfTrim = new Bounds(self.center, self.size - (Vector3.one * Geometry.faceTrim * 2f));

            float intersectDistanceA;
            float intersectDistanceB;
            if (
                (
                    self.IntersectRay(new Ray(a, b - a), out intersectDistanceA) && 
                    intersectDistanceA <= Vector3.Distance(a, b) &&
                    self.IntersectRay(new Ray(b, a - b), out intersectDistanceB) &&
                    intersectDistanceB <= Vector3.Distance(a, b)
                ) 
                &&
                (
                    !selfTrim.IntersectRay(new Ray(a, b - a)) &&
                    !selfTrim.IntersectRay(new Ray(b, a - b))
                )
            ) {
                bool sameFace = false;
                Vector3Sign faceSign = default;

                foreach (Vector3Sign i in Geometry.faceSigns)
                {
                    Axis axis = default;
                    foreach (Axis s in Vectors.axisIterate)
                    {
                        if (i.GetAxis(s) != Sign.Neutral)
                        {
                            axis = s;
                        }
                    }

                    if (
                        !allowance ? (
                            a.GetAxis(axis) == b.GetAxis(axis) &&
                            a.GetAxis(axis) == self.faces()[i].center.GetAxis(axis)
                        ) : Mathf.Abs(a.GetAxis(axis) - b.GetAxis(axis)) <= Geometry.faceTrim &&
                            Mathf.Abs(a.GetAxis(axis) - self.faces()[i].center.GetAxis(axis)) <= Geometry.faceTrim
                    )
                    { //on same face 
                        sameFace = true;
                        faceSign = i;
                    }
                }

                float distanceA;
                bool resultA = self.IntersectRay(new Ray(a, b - a), out distanceA);
                distanceA = Mathf.Clamp(distanceA, 0f, Mathf.Infinity);
                Vector3 pointA = (new Ray(a, b - a)).GetPoint(distanceA);

                float distanceB;
                bool resultB = self.IntersectRay(new Ray(b, a - b), out distanceB);
                distanceB = Mathf.Clamp(distanceB, 0f, Mathf.Infinity);
                Vector3 pointB = (new Ray(b, a - b)).GetPoint(distanceB);

                if (resultA && resultB)
                {
                    if (sameFace)
                    { //points on same face
                        return pointA != pointB;
                    }
                    else
                    { //points on same edge / corner
                        return pointA == pointB;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static bool FaceAdjacent(this Bounds self, Vector3 center, Quaternion orientation, Vector3 size, bool allowance = false)
        {
            Box face = new Box(center, orientation, size);
            if (self.ToBox().Intersects(face, false, allowance) && !self.ToBox().Intersects(face, true, allowance))
            {
                Axis faceAxis = default;
                foreach (Axis i in Vectors.axisIterate)
                {
                    if (size.GetAxis(i) == 0f)
                    {
                        faceAxis = i;
                    }
                }

                int cornerCount = 0;
                for (int x = -1; x <= 1; x += 2)
                {
                    for (int y = -1; y <= 1; y += 2)
                    {
                        if (self.CornerAdjacent(face.corners[(Vector3.zero.SetAxis(Vectors.axisPlanes[faceAxis].x, x).SetAxis(Vectors.axisPlanes[faceAxis].y, y).SetAxis(faceAxis, 1f)).ToSign()], allowance))
                        {
                            cornerCount++;
                        }
                    }
                }

                int edgeCount = 0;
                for (int axis = 0; axis < 2; axis++)
                {
                    for (int sign = -1; sign <= 1; sign += 2)
                    {
                        if (
                            self.EdgeAdjacent(
                                face.edges[Vector3.zero.SetAxis(Vectors.axisPlanes[faceAxis].GetAxis(Vectors.axisIterate[axis]), sign).SetAxis(faceAxis, 1f).ToSign()].Key,
                                face.edges[Vector3.zero.SetAxis(Vectors.axisPlanes[faceAxis].GetAxis(Vectors.axisIterate[axis]), sign).SetAxis(faceAxis, 1f).ToSign()].Value,
                                allowance
                            )
                        ) {
                            edgeCount++;
                        }
                    }
                }

                if (cornerCount == 1 && edgeCount == 0)
                { //1 corner touching
                    return false;
                }
                else if (edgeCount == 1)
                { //1 edge touching
                    return false;
                }
                else
                { //face touching
                    return true;
                }
            }
            else
            {
                return false;
            }
        }



        //STATIC METHODS
        public static Vector3 GetClosestPointOnLine(Vector3 a, Vector3 b, Vector3 point)
        {
            return a + Vector3.Project(point - a, b - a);
        }

        //SNAPPING
        public static Vector3 SnapPosition(Vector3 position, List<Vector3> snapPositions)
        {
            return snapPositions.Aggregate((a, b) => Vector3.Distance(position, a) < Vector3.Distance(position, b) ? a : b);
        }
        public static Vector3 SnapPosition(Vector3 position, Vector3[] snapPositions)
        {
            return SnapPosition(position, snapPositions.ToList());
        }

        public static Quaternion SnapRotation(Quaternion rotation, List<Quaternion> snapRotations)
        {
            return snapRotations.Aggregate((a, b) => Quaternion.Angle(rotation, a) < Quaternion.Angle(rotation, b) ? a : b);
        }
        public static Quaternion SnapRotation(Quaternion rotation, Quaternion[] snapRotations)
        {
            return SnapRotation(rotation, snapRotations.ToList());
        }

        public static Vector3 SnapDirection(Vector3 direction, List<Vector3> snapDirections)
        {
            return snapDirections.Aggregate((a, b) => Vector3.Angle(direction, a) < Vector3.Angle(direction, b) ? a : b);
        }
        public static Vector3 SnapDirection(Vector3 direction, Vector3[] snapDirections)
        {
            return SnapDirection(direction, snapDirections.ToList());
        }

        public static Vector3 SnapScale(Vector3 scale, List<Vector3> snapScales, bool absolute = true)
        {
            if (!absolute)
            {
                return snapScales.Aggregate((a, b) => Vector3.Distance(scale, a) < Vector3.Distance(scale, b) ? a : b);
            }
            else
            {
                return snapScales.Aggregate((a, b) => Vector3.Distance(scale.Abs(), a.Abs()) < Vector3.Distance(scale.Abs(), b.Abs()) ? a : b);
            }
        }
        public static Vector3 SnapScale(Vector3 scale, Vector3[] snapScales, bool absolute = true)
        {
            return SnapScale(scale, snapScales.ToList());
        }

        //ROTATING
        public static Vector3 RotatePosition(Vector3 position, UnityEngine.Vector3Int eulers) 
        {
            return Linking.InverseTransformPoint(position, Vector3.zero, Quaternion.Euler((Vector3)eulers * 90f));
        }
        public static Quaternion RotateRotation(Quaternion rotation, UnityEngine.Vector3Int eulers)
        {
            return rotation.Add(Quaternion.Euler((Vector3)eulers * 90f), Space.World);
        }
        public static Vector3 RotateDirection(Vector3 direction, UnityEngine.Vector3Int eulers)
        {
            return Linking.InverseTransformDirection(direction, Quaternion.Euler((Vector3)eulers * 90f));
        }
        public static Vector3 RotateScale(Vector3 scale, UnityEngine.Vector3Int eulers)
        {
            return Linking.InverseTransformPoint(scale, Vector3.zero, Quaternion.Euler((Vector3)eulers * 90f)).Operate((s, a) => Mathf.Abs(a) * Mathf.Sign(scale.GetAxis(s)));
        }
        public static Vector3Sign RotateSign(Vector3Sign sign, UnityEngine.Vector3Int eulers)
        {
            return sign.Rotate(eulers, Space.World);
        }
        public static Axis RotateAxis(Axis axis, UnityEngine.Vector3Int eulers)
        {
            return axis.Rotate(eulers, Space.World);
        }

        //public abstract class Polygon
        //{
        //    public Vector3 position;
        //    public Quaternion rotation;

        //    public abstract float volume { get; set; }
        //    public abstract float surfaceArea { get; set; }

        //    public abstract Vector3 max { get; }
        //    public abstract Vector3 min { get; }
        //}

        //public class Box : Polygon
        //{
        //    public Vector3 size;
        //    public Vector3 extents
        //    {
        //        get
        //        {
        //            return size * 0.5f;
        //        }
        //        set
        //        {
        //            size = value * 2f;
        //        }
        //    }
        //    public Vector3[] points
        //    {
        //        get
        //        {
        //            Vector3[] newPoints = new Vector3[8];
        //            int index = 0;

        //            for (int x = -1; x != 1; x = 1)
        //            {
        //                for (int y = -1; y != 1; y = 1)
        //                {
        //                    for (int z = -1; z != 1; z = 1)
        //                    {
        //                        newPoints[index] =
        //                            position +
        //                            (rotation * Vector3.right).normalized * x * size.x * 0.5f +
        //                            (rotation * Vector3.up).normalized * y * size.y * 0.5f +
        //                            (rotation * Vector3.forward).normalized * z * size.z * 0.5f;

        //                        index++;
        //                    }
        //                }
        //            }

        //            return newPoints;
        //        }
        //    }
        //    public Vector3 center
        //    {
        //        get
        //        {
        //            return position;
        //        }
        //        set
        //        {
        //            position = value;
        //        }
        //    }

        //    public override float volume
        //    {
        //        get
        //        {
        //            return size.x * size.y * size.z;
        //        }
        //        set
        //        {
        //            throw new System.NotImplementedException();
        //        }
        //    }
        //    public override float surfaceArea
        //    {
        //        get
        //        {
        //            throw new System.NotImplementedException();
        //        }
        //        set
        //        {
        //            throw new System.NotImplementedException();
        //        }
        //    }

        //    public override Vector3 max
        //    {
        //        get
        //        {
        //            Bounds bounds = new Bounds(position, Vector3.zero);

        //            foreach (Vector3 point in points)
        //            {
        //                bounds.Encapsulate(point);
        //            }

        //            return bounds.max;
        //        }
        //    }
        //    public override Vector3 min
        //    {
        //        get
        //        {
        //            Bounds bounds = new Bounds(position, Vector3.zero);

        //            foreach (Vector3 point in points)
        //            {
        //                bounds.Encapsulate(point);
        //            }

        //            return bounds.min;
        //        }
        //    }

        //    private struct BoxDef
        //    {
        //        public Vector3 pos, n1, n2, n3;
        //        public float min, max;
        //        private void UpdateMinMax(Vector3 aPos, ref Vector3 aNormal)
        //        {
        //            float p = Vector3.Dot(aPos, aNormal);
        //            if (p > max) max = p;
        //            if (p < min) min = p;
        //        }
        //        public void GetMinMax(ref Vector3 aAxis)
        //        {
        //            min = float.PositiveInfinity;
        //            max = float.NegativeInfinity;
        //            UpdateMinMax(pos + n1 + n2 + n3, ref aAxis);
        //            UpdateMinMax(pos + n1 + n2 - n3, ref aAxis);
        //            UpdateMinMax(pos + n1 - n2 + n3, ref aAxis);
        //            UpdateMinMax(pos + n1 - n2 - n3, ref aAxis);
        //            UpdateMinMax(pos - n1 + n2 + n3, ref aAxis);
        //            UpdateMinMax(pos - n1 + n2 - n3, ref aAxis);
        //            UpdateMinMax(pos - n1 - n2 + n3, ref aAxis);
        //            UpdateMinMax(pos - n1 - n2 - n3, ref aAxis);
        //        }
        //    }
        //    private struct TwoBoxes
        //    {
        //        public BoxDef A, B;
        //        // returns true if there is no overlap, false if they do overlap
        //        public bool SeperatingAxisTheorom(Vector3 aAxis)
        //        {
        //            A.GetMinMax(ref aAxis);
        //            B.GetMinMax(ref aAxis);
        //            return A.min > B.max || B.min > A.max;
        //        }
        //    }

        //    public Vector3 ClosestPoint(Vector3 point)
        //    {
        //        return Linking.TransformPoint((new Bounds(Linking.InverseTransformPoint(position, Vector3.one, rotation), size)).ClosestPoint(Linking.InverseTransformPoint(point, Vector3.zero, rotation)), Vector3.zero, rotation);
        //    }

        //    public bool Intersects(Box box, bool trim = false)
        //    {
        //        if (!trim)
        //        {
        //            return Intersects(position, rotation, box.size, box.position, box.rotation, box.size);
        //        }
        //        else
        //        {
        //            return Intersects(position, rotation, box.size - (faceTrim * Vector3.one), box.position, box.rotation, box.size);
        //        }
        //    }
        //    private static bool Intersects(Vector3 positionA, Quaternion rotationA, Vector3 sizeA, Vector3 positionB, Quaternion rotationB, Vector3 sizeB)
        //    {
        //        sizeA /= 2f;
        //        sizeB /= 2f;

        //        TwoBoxes data = new TwoBoxes();
        //        data.A.pos = positionA;
        //        data.A.n1 = rotationA * Vector3.right * sizeA.x;
        //        data.A.n2 = rotationA * Vector3.up * sizeA.y;
        //        data.A.n3 = rotationA * Vector3.forward * sizeA.z;
        //        data.B.pos = positionB;
        //        data.B.n1 = rotationB * Vector3.right * sizeB.x;
        //        data.B.n2 = rotationB * Vector3.up * sizeB.y;
        //        data.B.n3 = rotationB * Vector3.forward * sizeB.z;
        //        if (data.SeperatingAxisTheorom(data.A.n1)) return false;
        //        if (data.SeperatingAxisTheorom(data.A.n2)) return false;
        //        if (data.SeperatingAxisTheorom(data.A.n3)) return false;
        //        if (data.SeperatingAxisTheorom(data.B.n1)) return false;
        //        if (data.SeperatingAxisTheorom(data.B.n2)) return false;
        //        if (data.SeperatingAxisTheorom(data.B.n3)) return false;

        //        if (data.SeperatingAxisTheorom(Vector3.Cross(data.A.n1, data.B.n1))) return false;
        //        if (data.SeperatingAxisTheorom(Vector3.Cross(data.A.n1, data.B.n2))) return false;
        //        if (data.SeperatingAxisTheorom(Vector3.Cross(data.A.n1, data.B.n3))) return false;
        //        if (data.SeperatingAxisTheorom(Vector3.Cross(data.A.n2, data.B.n1))) return false;
        //        if (data.SeperatingAxisTheorom(Vector3.Cross(data.A.n2, data.B.n2))) return false;
        //        if (data.SeperatingAxisTheorom(Vector3.Cross(data.A.n2, data.B.n3))) return false;
        //        if (data.SeperatingAxisTheorom(Vector3.Cross(data.A.n3, data.B.n1))) return false;
        //        if (data.SeperatingAxisTheorom(Vector3.Cross(data.A.n3, data.B.n2))) return false;
        //        if (data.SeperatingAxisTheorom(Vector3.Cross(data.A.n3, data.B.n3))) return false;
        //        return true;
        //    }

        //    public bool Adjacent(Box box)
        //    {
        //        if (this.Intersects(box) && !this.Intersects(box, true))
        //        {
        //            //first check if tiles are parallel
        //            Vector3Bool localRotationB = new Vector3Bool(Vector3Bool.falsey.Operate((s, a) =>
        //            {
        //                return Linking.InverseTransformEuler(box.rotation, this.rotation).eulerAngles.GetAxis(s) % 90f == 0f;
        //            }));
        //            List<bool> localRotationBList = localRotationB.ToList(); //not important (OPTIMIZATION)


        //            if (Linking.InverseTransformEuler(box.rotation, this.rotation).normalized.eulerAngles == Vector3.zero)
        //            { //parallel on 3 axes (same orientation)
        //                return
        //                    (new Bounds(Linking.InverseTransformPoint(this.position, Vector3.zero, this.rotation), this.size)).Adjacent(
        //                    (new Bounds(Linking.InverseTransformPoint(box.position, Vector3.zero, this.rotation), box.size)));
        //            }
        //            else if (useTilt && localRotationBList.FindAll((axis) => axis == true).Count == 0)
        //            { //parallel on 3 axes with offset (different orientation)
        //                return
        //                    (new Bounds(Linking.InverseTransformPoint(this.position, Vector3.zero, this.rotation), this.size)).Adjacent(
        //                    (new Bounds(Linking.InverseTransformPoint(box.position, Vector3.zero, this.rotation), box.size).Rotate((Linking.InverseTransformEuler(box.rotation, this.rotation).eulerAngles.Divide(90f * Vector3.one)).RoundToInt())));
        //            }
        //            else if (useShear && localRotationBList.FindAll((axis) => axis == true).Count == 1)
        //            { //parallel on 2 axes (different orientation)

        //                Axis parallelAxis = localRotationB.ToDictionary().FirstOrDefault(axis => axis.Value).Key; //(OPTIMIZATION)

        //                Bounds thisBounds = new Bounds(Linking.InverseTransformPoint(this.position, Vector3.zero, this.rotation), this.size); //(OPTIMIZATION)
        //                Bounds boxBounds = new Bounds(Linking.InverseTransformPoint(box.position, Vector3.zero, this.rotation), box.size).Rotate((Linking.InverseTransformEuler(box.rotation, this.rotation).eulerAngles.Divide(90f * Vector3.one)).RoundToInt()); //(OPTIMIZATION)

        //                return RMath.Adjacent(thisBounds.min.GetAxis(parallelAxis), thisBounds.max.GetAxis(parallelAxis), boxBounds.min.GetAxis(parallelAxis), boxBounds.max.GetAxis(parallelAxis));
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }

        //    public Box()
        //    {
        //        position = Vector3.zero;
        //        rotation = Quaternion.identity;
        //        size = Vector3.one;
        //    }
        //    public Box(Vector3 position, Quaternion rotation, Vector3 size)
        //    {
        //        this.position = position;
        //        this.rotation = rotation;
        //        this.size = size;
        //    }
        //}

        //public class Sphere : Polygon
        //{
        //    public float radius;
        //    public float diameter
        //    {
        //        get
        //        {
        //            return radius * 2f;
        //        }
        //        set
        //        {
        //            radius = value / 2f;
        //        }
        //    }
        //    public override float volume
        //    {
        //        get
        //        {
        //            throw new System.NotImplementedException();
        //        }
        //        set
        //        {
        //            throw new System.NotImplementedException();
        //        }
        //    }
        //    public override float surfaceArea
        //    {
        //        get
        //        {
        //            throw new System.NotImplementedException();
        //        }
        //        set
        //        {
        //            throw new System.NotImplementedException();
        //        }
        //    }
        //    public override Vector3 max
        //    {
        //        get
        //        {
        //            throw new System.NotImplementedException();
        //        }
        //    }
        //    public override Vector3 min
        //    {
        //        get
        //        {
        //            throw new System.NotImplementedException();
        //        }
        //    }

        //    public Sphere()
        //    {

        //    }
        //    public Sphere(Vector3 position, Quaternion rotation, float radius)
        //    {
        //        this.position = position;
        //        this.rotation = rotation;
        //        this.radius = radius;
        //    }
        //}
    }
}
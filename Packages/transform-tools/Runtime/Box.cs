using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.TransformTools
{
    [System.Serializable]
    public struct Box
    {
        public Vector3 center;
        public Quaternion orientation;
        public Vector3 size; //cannot currently handle NEGATIVE size

        public Vector3 position
        {
            get
            {
                return center;
            }
            set
            {
                center = value;
            }
        }
        public Quaternion rotation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;
            }
        }
        public Vector3 extents
        {
            get
            {
                return size * 0.5f;
            }
            set
            {
                size = value * 2f;
            }
        }
        public float volume
        {
            get
            {
                return size.x * size.y * size.z;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }
        public float surfaceArea
        {
            get
            {
                return (
                    size.x * size.y +
                    size.y * size.z +
                    size.z * size.x
                ) * 2f;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }
        public Vector3 min //NEEDS TESTING
        {
            get
            {
                return center + (orientation * extents).normalized * extents.magnitude;
                //return new Vector3T<Vector3>(
                //    center + orientation * (Vector3.right * -extents.x),
                //    center + orientation * (Vector3.up * -extents.y),
                //    center + orientation * (Vector3.forward * -extents.z)
                //);
            }
            set
            {
                Vector3 originalMin = min;
                Vector3 originalMax = max;

                center = Vector3.Lerp(originalMax, value, 0.5f);
                size *= Vector3.Distance(value, originalMax) / Vector3.Distance(originalMin, originalMax);
            }
        }
        public Vector3 max //NEEDS TESTING
        {
            get
            {
                return center + (orientation * extents).normalized * -extents.magnitude;
                //return new Vector3T<Vector3>(
                //    center + orientation * (Vector3.right * extents.x),
                //    center + orientation * (Vector3.up * extents.y),
                //    center + orientation * (Vector3.forward * extents.z)
                //);
            }
            set
            {
                Vector3 originalMin = min;
                Vector3 originalMax = max;

                center = Vector3.Lerp(value, originalMin, 0.5f);
                size *= Vector3.Distance(originalMin, value) / Vector3.Distance(originalMin, originalMax);
            }
        }

        public Vector3 right
        {
            get
            {
                return orientation * Vector3.right;
            }
        }
        public Vector3 up
        {
            get
            {
                return orientation * Vector3.up;
            }
        }
        public Vector3 forward
        {
            get
            {
                return orientation * Vector3.forward;
            }
        }

        public Dictionary<Vector3Sign, Vector3> corners
        {
            get
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
                                center + (orientation * Vector3.one.Multiply(new Vector3(x, y, z)).Multiply(extents))
                            );
                        }
                    }
                }

                return newCorners;
            }
        }
        public Dictionary<Vector3Sign, KeyValuePair<Vector3, Vector3>> edges
        {
            get
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
                                new KeyValuePair<Vector3, Vector3>(
                                    center + (orientation * Vector3.zero.SetAxis(Vectors.axisPlanes[s].x, axisA).SetAxis(Vectors.axisPlanes[s].y, axisB).SetAxis(s, 1f).Multiply(extents)),
                                    center + (orientation * Vector3.zero.SetAxis(Vectors.axisPlanes[s].x, axisA).SetAxis(Vectors.axisPlanes[s].y, axisB).SetAxis(s, -1f).Multiply(extents))
                                )
                            );
                        }
                    }
                }

                return newEdges;
            }
        }
        public Dictionary<Vector3Sign, Box> faces
        {
            get
            {
                var center = this.center;
                var size = this.size;
                var extents = this.extents;
                var orientation = this.orientation;

                Dictionary<Vector3Sign, Box> newFaces = new Dictionary<Vector3Sign, Box>();

                foreach (Axis s in Vectors.axisIterate)
                {
                    for (int axis = -1; axis <= 1; axis += 2)
                    {
                        newFaces.Add(
                            Vector3.zero.SetAxis(s, axis).ToSign(), 
                            new Box(
                                center + orientation * Vector3.one.Multiply(Vector3.zero.SetAxis(s, axis)).Multiply(extents), 
                                orientation, 
                                size.SetAxis(s, 0f)
                            )
                        );
                    }
                }

                return newFaces;
            }
        }
        public Mesh mesh
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }
        //public Vector3[] faces
        //{
        //    get
        //    {
        //        throw new System.NotImplementedException();
        //    }
        //}
        //public Vector3[] edges
        //{
        //    get
        //    {
        //        throw new System.NotImplementedException();
        //    }
        //}

        public Bounds bounds
        {
            get
            {
                Bounds newBounds = new Bounds(center, Vector3.zero);

                foreach (KeyValuePair<Vector3Sign, Vector3> corner in corners)
                {
                    newBounds.Encapsulate(corner.Value);
                }

                return newBounds;
            }
        }

        public Bounds ToBounds()
        {
            Box newBox = this;
            newBox.Rotate(rotation.eulerAngles.Operate((s, a) =>
            {
                return RMath.CustomRound(a, 90f, 0f) / 90f;
            }).RoundToInt());

            return new Bounds(center, newBox.size);
        }

        //Conventional methods:

        //modification
        public void Expand(float amount)
        {
            size += Vector3.one * amount;
        }
        public void Expand(Vector3 amount)
        {
            size += amount;
        }
        public void Encapsulate(Vector3 point)
        {
            Bounds newBounds = new Bounds(Linking.InverseTransformPoint(center, Vector3.zero, orientation), size);
            newBounds.Encapsulate(Linking.InverseTransformPoint(point, Vector3.zero, orientation));

            //modification of self
            center = Linking.TransformPoint(newBounds.center, Vector3.zero, orientation);
            size = newBounds.size;
        }
        public void Encapsulate(Box box)
        {
            Bounds newBounds = new Bounds(Linking.InverseTransformPoint(center, Vector3.zero, orientation), size);
            foreach (KeyValuePair<Vector3Sign, Vector3> point in box.corners)
            {
                newBounds.Encapsulate(Linking.InverseTransformPoint(point.Value, Vector3.zero, orientation));
            }

            //modification of self
            center = Linking.TransformPoint(newBounds.center, Vector3.zero, orientation);
            size = newBounds.size;
        }
        public void SetMinMax(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
        }

        //detection
        public bool Contains(Vector3 point)
        {
            return (new Bounds(Linking.InverseTransformPoint(center, Vector3.zero, orientation), size)).Contains(Linking.InverseTransformPoint(point, Vector3.zero, orientation));
        }
        public bool Intersects(Box box)
        {
            return Intersects(center, orientation, size, box.center, box.orientation, box.size);
        }
        public bool IntersectRay(Ray ray)
        {
            return (new Bounds(Linking.InverseTransformPoint(center, Vector3.zero, orientation), size)).IntersectRay(new Ray(Linking.InverseTransformPoint(ray.origin, Vector3.zero, orientation), Linking.InverseTransformDirection(ray.direction, orientation)));
        }
        public bool IntersectRay(Ray ray, out float distance)
        {
            return (new Bounds(Linking.InverseTransformPoint(center, Vector3.zero, orientation), size)).IntersectRay(new Ray(Linking.InverseTransformPoint(ray.origin, Vector3.zero, orientation), Linking.InverseTransformDirection(ray.direction, orientation)), out distance);
        } //"Out" statement may not work
        public Vector3 ClosestPoint(Vector3 point)
        {
            return Linking.TransformPoint((new Bounds(Linking.InverseTransformPoint(center, Vector3.one, orientation), size)).ClosestPoint(Linking.InverseTransformPoint(point, Vector3.zero, orientation)), Vector3.zero, orientation);
        }
        public float SqrDistance(Vector3 point)
        {
            return (new Bounds(Linking.InverseTransformPoint(center, Vector3.zero, orientation), size)).SqrDistance(Linking.InverseTransformPoint(point, Vector3.zero, orientation));
        }


        public void Rotate(UnityEngine.Vector3Int eulers)
        {
            Bounds newBounds = (new Bounds(Vector3.zero, this.size));
            newBounds.Rotate(eulers);

            this.size = newBounds.size;
        }

        public bool CornerAdjacent(Vector3 point, bool allowance = true)
        {
            return (new Bounds(Linking.InverseTransformPoint(center, Vector3.zero, orientation), size)).CornerAdjacent(Linking.InverseTransformPoint(point, Vector3.zero, orientation), allowance);
        }
        public bool EdgeAdjacent(Vector3 a, Vector3 b, bool allowance = true)
        {
            return (new Bounds(Linking.InverseTransformPoint(center, Vector3.zero, orientation), size)).EdgeAdjacent(Linking.InverseTransformPoint(a, Vector3.zero, orientation), Linking.InverseTransformPoint(b, Vector3.zero, orientation), allowance);
        }
        public bool FaceAdjacent(Vector3 center, Quaternion orientation, Vector3 size, bool allowance = true)
        {
            return (new Bounds(Linking.InverseTransformPoint(this.center, Vector3.zero, this.orientation), this.size)).FaceAdjacent(Linking.InverseTransformPoint(center, Vector3.zero, this.orientation), Linking.InverseTransformEuler(orientation, this.orientation), size, allowance);
        }



        //Specialized collision methods:
        public bool Intersects(Vector3 point, bool trim = false, bool allowance = true)
        {
            return (new Bounds(Linking.InverseTransformPoint(center, Vector3.zero, orientation), size)).Intersects(Linking.InverseTransformPoint(point, Vector3.zero, orientation), trim, allowance);
            //return (new Box(center, orientation, size - Vector3.one * (trim ? Geometry.faceTrim : 0f))).Contains(point);
        }
        public bool Adjacent(Vector3 point, bool allowance = true)
        {
            return (new Bounds(Linking.InverseTransformPoint(center, Vector3.zero, orientation), size)).Adjacent(Linking.InverseTransformPoint(point, Vector3.zero, orientation), allowance);
            //if (Intersects(point) && !Intersects(point, true))
            //{
            //    return true;
            //}
            //else 
            //{
            //    return false;
            //}
        }
        public bool Contains(Vector3 point, bool trim = false, bool allowance = true)
        {
            return (new Bounds(Linking.InverseTransformPoint(center, Vector3.zero, orientation), size)).Contains(Linking.InverseTransformPoint(point, Vector3.zero, orientation), trim, allowance);
            //return (new Box(center, orientation, size - Vector3.one * (trim ? Geometry.faceTrim : 0f))).Contains(point);
        }

        //allowance MAY OR MAY NOT BE NEEDED
        public bool Intersects(Box box, bool trim = false, bool allowance = true)
        {
            return Intersects(center, orientation, size, box.center, box.orientation, box.size, trim);
        }
        public bool Adjacent(Box box, bool allowance = true)
        {
            return Intersects(box, false, allowance) && !Intersects(box, true, allowance) && !Contains(box, false, allowance);
        }
        public bool Contains(Box box, bool trim = false, bool allowance = true)
        {
            foreach (KeyValuePair<Vector3Sign, Vector3> corner in corners)
            {
                if (!box.Contains(corner.Value, trim, allowance)) 
                {
                    return false;
                }
            }

            return true;
        }


        public IntersectHit IntersectsCast(Vector3 point, bool trim = false, bool allowance = true)
        {
            return (new Bounds(Linking.InverseTransformPoint(center, Vector3.zero, orientation), size)).IntersectsCast(Linking.InverseTransformPoint(point, Vector3.zero, orientation), trim, allowance);
        }
        public AdjacentHit AdjacentCast(Vector3 point, bool allowance = true)
        {
            return (new Bounds(Linking.InverseTransformPoint(center, Vector3.zero, orientation), size)).AdjacentCast(Linking.InverseTransformPoint(point, Vector3.zero, orientation), allowance);
        }
        public ContainHit ContainsCast(Vector3 point, bool trim = false, bool allowance = true)
        {
            return (new Bounds(Linking.InverseTransformPoint(center, Vector3.zero, orientation), size)).ContainsCast(Linking.InverseTransformPoint(point, Vector3.zero, orientation), trim, allowance);
        }

        public BoxIntersectHit IntersectsCast(Box box, bool trim = false, bool allowance = true)
        {
            if (Intersects(box, trim, allowance))
            {
                BoxIntersectHit hit = new BoxIntersectHit();

                //primary
                //hit.intersection =
                hit.alignment = GetAlignment(orientation, box.orientation);

                //hits
                hit.adjacentHit = AdjacentCast(box, allowance);
                hit.containHit = ContainsCast(box, trim, allowance);

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
                //hit.volume = IntersectionArea(box).volume;
                if (hit.intersection == Intersection.Adjacent)
                {
                    hit.volume = 0f;
                }
                else if (hit.intersection == Intersection.Contain)
                {
                    hit.volume = volume;
                }
                else if (hit.intersection == Intersection.Intersect)
                {
                    hit.volume = box.volume * 0.5f;
                }
                hit.volumeRatio = hit.volume / box.volume;
                hit.dominant = hit.volumeRatio > 0.5f;



                int cornerCount = 0;
                foreach (KeyValuePair<Vector3Sign, Vector3> corner in corners)
                {
                    if (box.Contains(corner.Value, trim, allowance))
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
        public BoxAdjacentHit AdjacentCast(Box box, bool allowance = true)
        {
            if (Adjacent(box, allowance))
            {
                BoxAdjacentHit hit = new BoxAdjacentHit();

                //primary
                GetAdjacency(this, box, out hit.adjacency, out hit.adjacencySign, allowance);
                hit.alignment = GetAlignment(orientation, box.orientation);

                //attributes
                //hit.adjacencySign = 
                if (hit.adjacency == BoxAdjacency.Face)
                {
                    BoxAdjacency boxAdjacency;
                    Vector3Sign boxAdjacencySign;
                    GetAdjacency(box, this, out boxAdjacency, out boxAdjacencySign, allowance);

                    if (boxAdjacency == BoxAdjacency.Face)
                    {
                        hit.surfaceArea = (box.faces[boxAdjacencySign].surfaceArea / 2f) * 0.5f;
                        hit.surfaceAreaRatio = hit.surfaceArea / (box.faces[boxAdjacencySign].surfaceArea / 2f);
                    }
                    else
                    {
                        hit.surfaceArea = 0f;
                        hit.surfaceAreaRatio = 0f;
                    }
                }
                else
                {
                    hit.surfaceArea = 0f;
                    hit.surfaceAreaRatio = 0f;
                }
                hit.dominant = hit.surfaceAreaRatio > 0.5f;

                int cornerCount = 0;
                foreach (KeyValuePair<Vector3Sign, Vector3> corner in corners)
                {
                    if (box.CornerAdjacent(corner.Value, allowance)/*box.Adjacent(corner.Value)*/)
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
        public BoxContainHit ContainsCast(Box box, bool trim = false, bool allowance = true)
        {
            if (Contains(box, trim, allowance))
            {
                BoxContainHit hit = new BoxContainHit();

                //primary
                //hit.containment = 
                hit.alignment = GetAlignment(orientation, box.orientation);

                //hits
                hit.adjacentHits = new Dictionary<Vector3Sign, AdjacentHit>();
                foreach (KeyValuePair<Vector3Sign, Box> face in box.faces)
                {
                    hit.adjacentHits.Add(face.Key, AdjacentCast(face.Value, allowance));
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

                        //if (hit.alignment == BoxAlignment.Tilt) 
                        //{ 
                        //    hit.adjacencySign = hit.adjacencySign.Rotate()
                        //}
                    }
                    else
                    {
                        hit.adjacency = BoxAdjacency.None;
                    }
                }

                hit.volume = volume;
                hit.volumeRatio = volume / box.volume;
                hit.dominant = hit.volumeRatio > 0.5f;
                hit.leanSign = Linking.InverseTransformPoint(center, box.center, box.rotation).ToSign();

                return hit;
            }
            else
            {
                return null;
            }
        }
        

        public Box(Vector3 center, Quaternion orientation, Vector3 size)
        {
            this.center = center;
            this.orientation = orientation;
            this.size = size;
        }


        private static BoxAlignment GetAlignment(Quaternion a, Quaternion b)
        {
            Vector3Bool zeroAxes = new Vector3Bool(Vector3Bool.falsey.Operate((s, v) =>
            {
                return RMath.CustomRound(b.Subtract(a).eulerAngles.GetAxis(s), Geometry.zeroRound) == 0f;
            }));
            Vector3Bool tiltAxes = new Vector3Bool(Vector3Bool.falsey.Operate((s, v) =>
            {
                return RMath.CustomRound(b.Subtract(a).eulerAngles.GetAxis(s) % 90f, Geometry.zeroRound) == 0f;
            }));
            if (zeroAxes == Vector3Bool.truthy)
            {
                return BoxAlignment.Parallel;
            }
            else if (tiltAxes == Vector3Bool.truthy)
            {
                return BoxAlignment.Tilt;
            }
            else if (tiltAxes.ToList().FindAll(axis => axis).Count == 2/*1*/)
            {
                return BoxAlignment.Shear;
            }
            else
            {
                return BoxAlignment.Free;
            }
        }
        private static bool GetAdjacency(Box a, Box b, out BoxAdjacency adjacency, out Vector3Sign adjacencySign, bool allowance = true)
        {
            foreach (var face in a.faces)
            {
                if (b.FaceAdjacent(face.Value.center, face.Value.orientation, face.Value.size, allowance))
                {
                    adjacency = BoxAdjacency.Face;
                    adjacencySign = face.Key;

                    return true;
                }
            }
            foreach (var edge in a.edges)
            {
                if (b.EdgeAdjacent(edge.Value.Key, edge.Value.Value, allowance))
                {
                    adjacency = BoxAdjacency.Edge;
                    adjacencySign = edge.Key;

                    return true;
                }
            }
            foreach (var corner in a.corners)
            {
                if (b.CornerAdjacent(corner.Value, allowance))
                {
                    adjacency = BoxAdjacency.Corner;
                    adjacencySign = corner.Key;

                    return true;
                }
            }

            adjacency = BoxAdjacency.None;
            adjacencySign = Vector3Sign.zero;

            return false;
        }


        private struct BoxDef
        {
            public Vector3 position;

            public Vector3 rightFace;
            public Vector3 upFace;
            public Vector3 forwardFace;
            
            public float min;
            public float max;
            
            private void UpdateMinMax(Vector3 aPos, ref Vector3 aNormal)
            {
                float p = Vector3.Dot(aPos, aNormal);
                if (p > max) max = p;
                if (p < min) min = p;
                //if (p >= max) max = p;
                //if (p <= min) min = p;
            }
            public void GetMinMax(ref Vector3 aAxis)
            {
                min = float.PositiveInfinity;
                max = float.NegativeInfinity;
                UpdateMinMax(position + rightFace + upFace + forwardFace, ref aAxis);
                UpdateMinMax(position + rightFace + upFace - forwardFace, ref aAxis);
                UpdateMinMax(position + rightFace - upFace + forwardFace, ref aAxis);
                UpdateMinMax(position + rightFace - upFace - forwardFace, ref aAxis);
                UpdateMinMax(position - rightFace + upFace + forwardFace, ref aAxis);
                UpdateMinMax(position - rightFace + upFace - forwardFace, ref aAxis);
                UpdateMinMax(position - rightFace - upFace + forwardFace, ref aAxis);
                UpdateMinMax(position - rightFace - upFace - forwardFace, ref aAxis);
            }
        }
        private struct TwoBoxes
        {
            public BoxDef A, B;

            // returns true if there is no overlap, false if they do overlap
            public bool SeperatingAxisTheorom(Vector3 aAxis, bool trim = false)
            {
                A.GetMinMax(ref aAxis);
                B.GetMinMax(ref aAxis);

                //Debug.Log(A.min + " - " + A.max + " <=> " + B.min + " - " + B.max);
                //Debug.Log((A.max + A.min) + " <=> " + (B.max + B.min));

                return A.min > B.max || B.min > A.max;
                //return A.min < B.max || B.min < A.max;
            }
        }
        private static bool Intersects(Vector3 positionA, Quaternion rotationA, Vector3 sizeA, Vector3 positionB, Quaternion rotationB, Vector3 sizeB, bool trim = false)
        {
            //sets up variables
            Vector3 extentsA = sizeA / 2f - (Vector3.one * (trim ? Geometry.faceTrim * 2f : 0f));
            Vector3 extentsB = sizeB / 2f;

            TwoBoxes data = new TwoBoxes();
            data.A.position = positionA;

            data.A.rightFace = rotationA * Vector3.right * extentsA.x;
            data.A.upFace = rotationA * Vector3.up * extentsA.y;
            data.A.forwardFace = rotationA * Vector3.forward * extentsA.z;

            data.B.position = positionB;

            data.B.rightFace = rotationB * Vector3.right * extentsB.x;
            data.B.upFace = rotationB * Vector3.up * extentsB.y;
            data.B.forwardFace = rotationB * Vector3.forward * extentsB.z;


            //List<bool> faceOverlap = new List<bool>
            //{
            //    data.SeperatingAxisTheorom(data.A.rightFace),
            //    data.SeperatingAxisTheorom(data.A.upFace),
            //    data.SeperatingAxisTheorom(data.A.forwardFace),
            //    data.SeperatingAxisTheorom(data.B.rightFace),
            //    data.SeperatingAxisTheorom(data.B.upFace),
            //    data.SeperatingAxisTheorom(data.B.forwardFace)
            //};
            if (data.SeperatingAxisTheorom(data.A.rightFace)) return false;
            if (data.SeperatingAxisTheorom(data.A.upFace)) return false;
            if (data.SeperatingAxisTheorom(data.A.forwardFace)) return false;
            if (data.SeperatingAxisTheorom(data.B.rightFace)) return false;
            if (data.SeperatingAxisTheorom(data.B.upFace)) return false;
            if (data.SeperatingAxisTheorom(data.B.forwardFace)) return false;

            //List<bool> crossOverlap = new List<bool>
            //{
            //    data.SeperatingAxisTheorom(Vector3.Cross(data.A.rightFace, data.B.rightFace)),
            //    data.SeperatingAxisTheorom(Vector3.Cross(data.A.rightFace, data.B.upFace)),
            //    data.SeperatingAxisTheorom(Vector3.Cross(data.A.rightFace, data.B.forwardFace)),
            //    data.SeperatingAxisTheorom(Vector3.Cross(data.A.upFace, data.B.rightFace)),
            //    data.SeperatingAxisTheorom(Vector3.Cross(data.A.upFace, data.B.upFace)),
            //    data.SeperatingAxisTheorom(Vector3.Cross(data.A.upFace, data.B.forwardFace)),
            //    data.SeperatingAxisTheorom(Vector3.Cross(data.A.forwardFace, data.B.rightFace)),
            //    data.SeperatingAxisTheorom(Vector3.Cross(data.A.forwardFace, data.B.upFace)),
            //    data.SeperatingAxisTheorom(Vector3.Cross(data.A.forwardFace, data.B.forwardFace))
            //};
            if (data.SeperatingAxisTheorom(Vector3.Cross(data.A.rightFace, data.B.rightFace))) return false;
            if (data.SeperatingAxisTheorom(Vector3.Cross(data.A.rightFace, data.B.upFace))) return false;
            if (data.SeperatingAxisTheorom(Vector3.Cross(data.A.rightFace, data.B.forwardFace))) return false;
            if (data.SeperatingAxisTheorom(Vector3.Cross(data.A.upFace, data.B.rightFace))) return false;
            if (data.SeperatingAxisTheorom(Vector3.Cross(data.A.upFace, data.B.upFace))) return false;
            if (data.SeperatingAxisTheorom(Vector3.Cross(data.A.upFace, data.B.forwardFace))) return false;
            if (data.SeperatingAxisTheorom(Vector3.Cross(data.A.forwardFace, data.B.rightFace))) return false;
            if (data.SeperatingAxisTheorom(Vector3.Cross(data.A.forwardFace, data.B.upFace))) return false;
            if (data.SeperatingAxisTheorom(Vector3.Cross(data.A.forwardFace, data.B.forwardFace))) return false;

            //if (faceOverlap.Contains(true) || crossOverlap.Contains(true))
            //{
            //    return false;
            //}



            return true;
        }
    }
}
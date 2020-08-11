using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.TransformTools
{
    [System.Serializable]
    public class AxisOrder
    {
        [SerializeField]
        public List<AxisApplied> axes = new List<AxisApplied>();
        public SpaceVariety variety = SpaceVariety.OneSided;

        public Space space = Space.World;

        public AxisOrder(List<AxisApplied> axes = null, SpaceVariety variety = SpaceVariety.OneSided, Space space = Space.World)
        {
            if (axes == null)
            {
                this.axes = new List<AxisApplied>();
            }
            else
            {
                this.axes = axes;
            }

            this.variety = variety;
            this.space = space;
        }
        public AxisOrder(Vector3 axes, Space space = Space.Self) //set simple (only 3 axes)
        {
            this.axes = new List<AxisApplied>();

            foreach (Axis i in Vectors.axisDefaultOrder)
            {
                this.axes.Add(new AxisApplied(i, axes.GetAxis(i),/* SpaceVariety.OneSided,*/ space));
            }

            this.space = space;
        }

        //Methods

        //apply
        public Quaternion ApplyRotation(Quaternion rotation, Quaternion? current = null)
        {
            Quaternion newRot;

            if (current != null)
            {
                newRot = (Quaternion)current;
            }
            else
            {
                newRot = rotation;
            }

            if (variety == SpaceVariety.OneSided)
            {
                foreach (AxisApplied i in axes)
                {
                    if (space == Space.Self)
                    {
                        newRot = newRot * Quaternion.Euler(Vectors.axisDirections[i.axis] * i.units);
                    }
                    else
                    {
                        newRot = Quaternion.Euler(Vectors.axisDirections[i.axis] * i.units) * newRot;
                    }
                }
            }
            else if (variety == SpaceVariety.Mixed)
            {
                foreach (AxisApplied i in axes)
                {
                    if (space == Space.Self)
                    {
                        newRot = newRot * Quaternion.Euler(Vectors.axisDirections[i.axis] * i.units);
                    }
                    else
                    {
                        newRot = Quaternion.Euler(Vectors.axisDirections[i.axis] * i.units) * newRot;
                    }
                }
            }
            return newRot;
        } //works
        public Quaternion ApplyRotation(Transform relative, Quaternion? current = null)
        {
            Quaternion newRot;

            if (current != null)
            {
                newRot = (Quaternion)current;
            }
            else
            {
                newRot = relative.rotation;
            }

            if (variety == SpaceVariety.OneSided)
            {
                foreach (AxisApplied i in axes)
                {
                    if (space == Space.Self)
                    {
                        newRot = newRot * Quaternion.Euler(Vectors.axisDirections[i.axis] * i.units);
                    }
                    else
                    {
                        newRot = Quaternion.Euler(Vectors.axisDirections[i.axis] * i.units) * newRot;
                    }
                }
            }
            else if (variety == SpaceVariety.Mixed)
            {
                foreach (AxisApplied i in axes)
                {
                    if (space == Space.Self)
                    {
                        newRot = newRot * Quaternion.Euler(Vectors.axisDirections[i.axis] * i.units);
                    }
                    else
                    {
                        newRot = Quaternion.Euler(Vectors.axisDirections[i.axis] * i.units) * newRot;
                    }
                }
            }
            return newRot;
        }

        public Vector3 ApplyPosition(Transform relative, Vector3? current = null, float scale = 1)
        {
            Vector3 newPos;

            if (current != null)
            {
                newPos = (Vector3)current;
            }
            else
            {
                newPos = relative.position;
            }

            if (variety == SpaceVariety.OneSided)
            {
                foreach (AxisApplied i in axes)
                {
                    if (space == Space.Self)
                    {
                        newPos += relative.parent.TransformPoint(Vectors.axisDirections[i.axis] * i.units * scale);
                    }
                    else
                    {
                        newPos += (Vectors.axisDirections[i.axis] * i.units * scale);
                    }
                }
            }
            else if (variety == SpaceVariety.Mixed)
            {
                foreach (AxisApplied i in axes)
                {
                    if (i.space == Space.Self)
                    {
                        newPos += relative.parent.TransformPoint(Vectors.axisDirections[i.axis] * i.units * scale);
                    }
                    else
                    {
                        newPos += (Vectors.axisDirections[i.axis] * i.units * scale);
                    }
                }
            }
            return newPos;
        } //works probably
        public Vector3 ApplyPosition(Vector3 position, Quaternion rotation, Vector3 scale, Vector3? current = null)
        {
            Vector3 newPos;

            if (current != null)
            {
                newPos = (Vector3)current;
            }
            else
            {
                newPos = position;
            }

            if (variety == SpaceVariety.OneSided)
            {
                foreach (AxisApplied i in axes)
                {
                    if (space == Space.Self)
                    {
                        newPos += Linking.TransformPoint(Vectors.axisDirections[i.axis] * i.units, position, rotation, scale);
                    }
                    else
                    {
                        newPos += (Vectors.axisDirections[i.axis] * i.units);
                    }
                }
            }
            else if (variety == SpaceVariety.Mixed)
            {
                foreach (AxisApplied i in axes)
                {
                    if (i.space == Space.Self)
                    {
                        newPos += Linking.TransformPoint(Vectors.axisDirections[i.axis] * i.units, position, rotation, scale);
                    }
                    else
                    {
                        newPos += (Vectors.axisDirections[i.axis] * i.units);
                    }
                }
            }
            return newPos;
        }
        public Vector3 ApplyPosition(Vector3 position, Quaternion rotation, Vector3? current = null)
        {
            Vector3 newPos;

            if (current != null)
            {
                newPos = (Vector3)current;
            }
            else
            {
                newPos = position;
            }

            return ApplyPosition(position, rotation, Vector3.one, newPos);
        }


        //reverse
        public Quaternion ReverseRotation(Quaternion rotation, Quaternion? current = null)
        {
            Quaternion newRot;

            if (current != null)
            {
                newRot = (Quaternion)current;
            }
            else
            {
                newRot = rotation;
            }
            if (variety == SpaceVariety.OneSided)
            {
                for (int j = axes.Count; j > 0; j--)
                {
                    AxisApplied i = axes[j - 1];

                    if (space == Space.Self)
                    {
                        newRot = newRot * Quaternion.Euler(Vectors.axisDirections[i.axis] * -i.units);
                    }
                    else
                    {
                        newRot = Quaternion.Euler(Vectors.axisDirections[i.axis] * -i.units) * newRot;
                    }
                }
            }
            else if (variety == SpaceVariety.Mixed)
            {
                for (int j = axes.Count; j > 0; j--)
                {
                    AxisApplied i = axes[j - 1];

                    if (space == Space.Self)
                    {
                        newRot = newRot * Quaternion.Euler(Vectors.axisDirections[i.axis] * -i.units);
                    }
                    else
                    {
                        newRot = Quaternion.Euler(Vectors.axisDirections[i.axis] * -i.units) * newRot;
                    }
                }
            }
            return newRot;
        } //works
        public Quaternion ReverseRotation(Transform relative, Quaternion? current = null)
        {
            Quaternion newRot;

            if (current != null)
            {
                newRot = (Quaternion)current;
            }
            else
            {
                newRot = relative.rotation;
            }

            if (variety == SpaceVariety.OneSided)
            {
                foreach (AxisApplied i in axes)
                {
                    if (space == Space.Self)
                    {
                        newRot = newRot * Quaternion.Euler(-(Vectors.axisDirections[i.axis] * i.units));
                    }
                    else
                    {
                        newRot = Quaternion.Euler(-(Vectors.axisDirections[i.axis] * i.units)) * newRot;
                    }
                }
            }
            else if (variety == SpaceVariety.Mixed)
            {
                foreach (AxisApplied i in axes)
                {
                    if (space == Space.Self)
                    {
                        newRot = newRot * Quaternion.Euler(-(Vectors.axisDirections[i.axis] * i.units));
                    }
                    else
                    {
                        newRot = Quaternion.Euler(-(Vectors.axisDirections[i.axis] * i.units)) * newRot;
                    }
                }
            }
            return newRot;
        }

        public Vector3 ReversePosition(Transform relative, Vector3? current = null, float scale = 1)
        {
            Vector3 newPos;

            if (current != null)
            {
                newPos = (Vector3)current;
            }
            else
            {
                newPos = relative.position;
            }

            if (variety == SpaceVariety.OneSided)
            {
                foreach (AxisApplied i in axes)
                {
                    if (space == Space.Self)
                    {
                        newPos += relative.parent.TransformPoint(-(Vectors.axisDirections[i.axis] * i.units * scale));
                    }
                    else
                    {
                        newPos += -(Vectors.axisDirections[i.axis] * i.units * scale);
                    }
                }
            }
            else if (variety == SpaceVariety.Mixed)
            {
                foreach (AxisApplied i in axes)
                {
                    if (i.space == Space.Self)
                    {
                        newPos += relative.parent.TransformPoint(-(Vectors.axisDirections[i.axis] * i.units * scale));
                    }
                    else
                    {
                        newPos += -(Vectors.axisDirections[i.axis] * i.units * scale);
                    }
                }
            }
            return newPos;
        } //works probably
        public Vector3 ReversePosition(Vector3 position, Quaternion rotation, Vector3 scale, Vector3? current = null)
        {
            Vector3 newPos;

            if (current != null)
            {
                newPos = (Vector3)current;
            }
            else
            {
                newPos = position;
            }

            if (variety == SpaceVariety.OneSided)
            {
                foreach (AxisApplied i in axes)
                {
                    if (space == Space.Self)
                    {
                        newPos += Linking.TransformPoint(-(Vectors.axisDirections[i.axis] * i.units), position, rotation, scale);
                    }
                    else
                    {
                        newPos += -(Vectors.axisDirections[i.axis] * i.units);
                    }
                }
            }
            else if (variety == SpaceVariety.Mixed)
            {
                foreach (AxisApplied i in axes)
                {
                    if (i.space == Space.Self)
                    {
                        newPos += Linking.TransformPoint(-(Vectors.axisDirections[i.axis] * i.units), position, rotation, scale);
                    }
                    else
                    {
                        newPos += -(Vectors.axisDirections[i.axis] * i.units);
                    }
                }
            }
            return newPos;
        }
        public Vector3 ReversePosition(Vector3 position, Quaternion rotation, Vector3? current = null)
        {
            Vector3 newPos;

            if (current != null)
            {
                newPos = (Vector3)current;
            }
            else
            {
                newPos = position;
            }

            return ReversePosition(position, rotation, Vector3.one, newPos);
        }


        //simpler methods
        public Vector3 ApplyPosition(Transform relative, float scale = 1)
        {
            return ApplyPosition(relative, null, scale);
        }
        public Vector3 ReversePosition(Transform relative, float scale = 1)
        {
            return ReversePosition(relative, null, scale);
        }
    }
}
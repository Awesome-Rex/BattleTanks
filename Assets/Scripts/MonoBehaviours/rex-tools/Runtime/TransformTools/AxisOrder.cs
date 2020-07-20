using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

using TransformTools;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace TransformTools
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
                this.axes.Add(new AxisApplied(i, Vectors.GetAxis(i, axes),/* SpaceVariety.OneSided,*/ space));
            }

            this.space = space;
        }

        //Methods
        public Quaternion ApplyRotation(CustomRotation relative, Quaternion? current = null) //WORKS!
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
                    newRot = relative.Rotate(newRot, (Vectors.axisDirections[i.axis] * i.units), space);
                }
            }
            else if (variety == SpaceVariety.Mixed)
            {
                foreach (AxisApplied i in axes)
                {
                    newRot = relative.Rotate(newRot, (Vectors.axisDirections[i.axis] * i.units), i.space);
                }
            }

            return newRot;
        }
        public Quaternion ApplyRotation(Quaternion relative)
        {
            Quaternion newRot = relative;

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
        public Quaternion ApplyRotation(Transform relative)
        {
            Quaternion newRot = relative.rotation;

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
        } //works probably

        public Vector3 ApplyPosition(CustomPosition relative, Vector3? current = null, float scale = 1f)
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
                    newPos = relative.Translate(newPos, (Vectors.axisDirections[i.axis] * i.units) * scale, space);
                }
            }
            else if (variety == SpaceVariety.Mixed)
            {
                foreach (AxisApplied i in axes)
                {
                    newPos = relative.Translate(newPos, (Vectors.axisDirections[i.axis] * i.units) * scale, i.space);
                }
            }
            return newPos;
        } //WORKS!
        public Vector3 ApplyPosition(Transform relative)
        {
            Vector3 newPos = relative.position;

            if (variety == SpaceVariety.OneSided)
            {
                foreach (AxisApplied i in axes)
                {
                    if (space == Space.Self)
                    {
                        newPos += relative.parent.TransformPoint(Vectors.axisDirections[i.axis] * i.units);
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
                        newPos += relative.parent.TransformPoint(Vectors.axisDirections[i.axis] * i.units);
                    }
                    else
                    {
                        newPos += (Vectors.axisDirections[i.axis] * i.units);
                    }
                }
            }
            return newPos;
        } //works probably

        public Quaternion ReverseRotation(CustomRotation relative, Quaternion? current = null) //WORKS!
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
                for (int j = axes.Count; j > 0; j--)
                {
                    AxisApplied i = axes[j - 1];

                    newRot = relative.Rotate(newRot, (Vectors.axisDirections[i.axis] * -i.units), space);
                }
            }
            else if (variety == SpaceVariety.Mixed)
            {
                for (int j = axes.Count; j > 0; j--)
                {
                    AxisApplied i = axes[j - 1];

                    newRot = relative.Rotate(newRot, (Vectors.axisDirections[i.axis] * -i.units), i.space);
                }
            }

            return newRot;
        }
        public Quaternion ReverseRotation(Quaternion relative)
        {
            Quaternion newRot = relative;

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

        public Vector3 ReversePosition(CustomPosition relative, Vector3? current = null, float scale = 1f) //takes and return GLOBAL
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
                for (int j = axes.Count; j > 0; j--)
                {
                    AxisApplied i = axes[j - 1];

                    newPos = relative.Translate(newPos, -(Vectors.axisDirections[i.axis] * i.units) * scale, space);
                }
            }
            else if (variety == SpaceVariety.Mixed)
            {
                for (int j = axes.Count; j > 0; j--)
                {
                    AxisApplied i = axes[j - 1];

                    newPos = relative.Translate(newPos, -(Vectors.axisDirections[i.axis] * i.units) * scale, i.space);
                }
            }
            return newPos;
        } //WORKS!
        public Vector3 ReversePosition(Transform relative)
        {
            Vector3 newPos = relative.position;

            if (variety == SpaceVariety.OneSided)
            {
                foreach (AxisApplied i in axes)
                {
                    if (space == Space.Self)
                    {
                        newPos += relative.parent.TransformPoint(-(Vectors.axisDirections[i.axis] * i.units));
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
                        newPos += relative.parent.TransformPoint(-(Vectors.axisDirections[i.axis] * i.units));
                    }
                    else
                    {
                        newPos += -(Vectors.axisDirections[i.axis] * i.units);
                    }
                }
            }
            return newPos;
        } //works probably
    }
}
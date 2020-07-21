using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

namespace REXTools.CustomTransforms
{
    public static class AxisOrderExtensions
    {
        public static Quaternion ApplyRotation(this AxisOrder target, CustomRotation relative, Quaternion? current = null) //WORKS!
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

            if (target.variety == SpaceVariety.OneSided)
            {
                foreach (AxisApplied i in target.axes)
                {
                    newRot = relative.Rotate(newRot, (Vectors.axisDirections[i.axis] * i.units), target.space);
                }
            }
            else if (target.variety == SpaceVariety.Mixed)
            {
                foreach (AxisApplied i in target.axes)
                {
                    newRot = relative.Rotate(newRot, (Vectors.axisDirections[i.axis] * i.units), i.space);
                }
            }

            return newRot;
        }
        public static Vector3 ApplyPosition(this AxisOrder target, CustomPosition relative, Vector3? current = null, float scale = 1f)
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

            if (target.variety == SpaceVariety.OneSided)
            {
                foreach (AxisApplied i in target.axes)
                {
                    newPos = relative.Translate(newPos, (Vectors.axisDirections[i.axis] * i.units) * scale, target.space);
                }
            }
            else if (target.variety == SpaceVariety.Mixed)
            {
                foreach (AxisApplied i in target.axes)
                {
                    newPos = relative.Translate(newPos, (Vectors.axisDirections[i.axis] * i.units) * scale, i.space);
                }
            }
            return newPos;
        }
        public static Quaternion ReverseRotation(this AxisOrder target, CustomRotation relative, Quaternion? current = null) //WORKS!
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

            if (target.variety == SpaceVariety.OneSided)
            {
                for (int j = target.axes.Count; j > 0; j--)
                {
                    AxisApplied i = target.axes[j - 1];

                    newRot = relative.Rotate(newRot, (Vectors.axisDirections[i.axis] * -i.units), target.space);
                }
            }
            else if (target.variety == SpaceVariety.Mixed)
            {
                for (int j = target.axes.Count; j > 0; j--)
                {
                    AxisApplied i = target.axes[j - 1];

                    newRot = relative.Rotate(newRot, (Vectors.axisDirections[i.axis] * -i.units), i.space);
                }
            }

            return newRot;
        }
        public static Vector3 ReversePosition(this AxisOrder target, CustomPosition relative, Vector3? current = null, float scale = 1f) //takes and return GLOBAL
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

            if (target.variety == SpaceVariety.OneSided)
            {
                for (int j = target.axes.Count; j > 0; j--)
                {
                    AxisApplied i = target.axes[j - 1];

                    newPos = relative.Translate(newPos, -(Vectors.axisDirections[i.axis] * i.units) * scale, target.space);
                }
            }
            else if (target.variety == SpaceVariety.Mixed)
            {
                for (int j = target.axes.Count; j > 0; j--)
                {
                    AxisApplied i = target.axes[j - 1];

                    newPos = relative.Translate(newPos, -(Vectors.axisDirections[i.axis] * i.units) * scale, i.space);
                }
            }
            return newPos;
        } //WORKS!
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

[System.Serializable]
public class AxisOrder
{
    // THE AXIS BIBLE
    public static Axis[] axisIterate = new Axis[]
    {
        Axis.X, Axis.Y, Axis.Z
    };
    public static Axis[] axisDefaultOrder = new Axis[]
    {
        Axis.X, Axis.Y, Axis.Z
    };

    public static Dictionary<Axis, string> axisNames = new Dictionary<Axis, string>
    {
        { Axis.X, "X" },
        { Axis.Y, "Y" },
        { Axis.Z, "Z" }
    };
    public static Dictionary<Axis, Vector3> axisDirections = new Dictionary<Axis, Vector3>
    {
        { Axis.X, new Vector3(1f, 0f, 0f) },
        { Axis.Y, new Vector3(0f, 1f, 0f) },
        { Axis.Z, new Vector3(0f, 0f, 1f) }
    };

    public static float GetAxis (Axis axis, Vector3 from)
    {
        if (axis == Axis.X)
        {
            return from.x;
        } else if (axis == Axis.Y)
        {
            return from.y;
        } else if (axis == Axis.Z)
        {
            return from.z;
        }

        return 0f;
    }

    public static Vector3 MultiplyVector3 (Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
    public static Vector3 DivideVector3(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }
    //END

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
    public AxisOrder (Vector3 axes, Space space = Space.Self) //set simple (only 3 axes)
    {
        this.axes = new List<AxisApplied>();

        foreach (Axis i in axisDefaultOrder)
        {
            this.axes.Add(new AxisApplied(i, GetAxis(i, axes), SpaceVariety.OneSided, space));
        }

        this.space = space;
    }


    public Quaternion ApplyRotation(CustomRotation relative, Quaternion? current = null) //WORKS!
    {
        Quaternion newRot;

        if (current != null)
        {
            newRot = (Quaternion)current;
        } else
        {
            newRot = relative.rotation;
        }

        if (variety == SpaceVariety.OneSided)
        {
            foreach (AxisApplied i in axes)
            {
                newRot = relative.Rotate(newRot, (axisDirections[i.axis] * i.units), space);
            }
        }
        else if (variety == SpaceVariety.Mixed)
        {
            foreach (AxisApplied i in axes)
            {
                newRot = relative.Rotate(newRot, (axisDirections[i.axis] * i.units), i.space);
            }
        }

        return newRot;
    }
    public Quaternion ApplyRotation (Quaternion relative)
    {
        Quaternion newRot = relative;

        if (variety == SpaceVariety.OneSided)
        {
            foreach (AxisApplied i in axes)
            {
                if (space == Space.Self)
                {
                    newRot = newRot * Quaternion.Euler(axisDirections[i.axis] * i.units);
                }
                else
                {
                    newRot = Quaternion.Euler(axisDirections[i.axis] * i.units) * newRot;
                }
            }
        }
        else if (variety == SpaceVariety.Mixed)
        {
            foreach (AxisApplied i in axes)
            {
                if (space == Space.Self)
                {
                    newRot = newRot * Quaternion.Euler(axisDirections[i.axis] * i.units);
                }
                else
                {
                    newRot = Quaternion.Euler(axisDirections[i.axis] * i.units) * newRot;
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
                if (space == Space.Self) {
                    newRot = newRot * Quaternion.Euler(axisDirections[i.axis] * i.units);
                } else
                {
                    newRot = Quaternion.Euler(axisDirections[i.axis] * i.units) * newRot;
                }
            }
        }
        else if (variety == SpaceVariety.Mixed)
        {
            foreach (AxisApplied i in axes)
            {
                if (space == Space.Self)
                {
                    newRot = newRot * Quaternion.Euler(axisDirections[i.axis] * i.units);
                }
                else
                {
                    newRot = Quaternion.Euler(axisDirections[i.axis] * i.units) * newRot;
                }
            }
        }
        return newRot;
    } //works probably

    public Vector3 ApplyPosition(CustomPosition relative, Vector3? current = null) 
    {
        Vector3 newPos;
        if (current != null) {
            newPos = (Vector3)current;
        } else
        {
            newPos = relative.position;
        }

        if (variety == SpaceVariety.OneSided)
        {
            foreach (AxisApplied i in axes)
            {
                newPos = relative.Translate(newPos, (axisDirections[i.axis] * i.units), space);
            }
        }
        else if (variety == SpaceVariety.Mixed)
        {
            foreach (AxisApplied i in axes)
            {
                newPos = relative.Translate(newPos, (axisDirections[i.axis] * i.units), i.space);
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
                    newPos += relative.parent.TransformPoint(axisDirections[i.axis] * i.units);
                }
                else
                {
                    newPos += (axisDirections[i.axis] * i.units);
                }
            }
        }
        else if (variety == SpaceVariety.Mixed)
        {
            foreach (AxisApplied i in axes)
            {
                if (i.space == Space.Self)
                {
                    newPos += relative.parent.TransformPoint(axisDirections[i.axis] * i.units);
                }
                else
                {
                    newPos += (axisDirections[i.axis] * i.units);
                }
            }
        }
        return newPos;
    } //works probably

#if UNITY_EDITOR

    /*[CustomPropertyDrawer(typeof(AxisOrder))]
    public class E : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Keyboard), label);
            EditorGUI.PropertyField(position, property, GUIContent.none);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }*/
#endif
}

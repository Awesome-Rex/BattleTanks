using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif



public class AxisOrder
{
    public static Axis[] axisIterate = new Axis[]
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

    public static Dictionary<Axis, Color> axisColours = new Dictionary<Axis, Color>
    {
        { Axis.X, new Color(219f / 255f, 62f / 255f, 29f / 255f) },
        { Axis.Y, new Color(154 / 255f, 243 / 255f, 72f / 255f) },
        { Axis.Z, new Color(58f / 255f, 122f / 255f, 237f / 255f) }
    };

    public List<AxisApplied> axes = new List<AxisApplied>();

    public AxisOrder(List<AxisApplied> axes = null)
    {
        if (axes == null)
        {
            this.axes = new List<AxisApplied>();
        }
        else
        {
            this.axes = axes;
        }
    }

    public Quaternion apply (Quaternion current, Space space = Space.World)
    {
        if (space == Space.World) {
            Quaternion newRot = new Quaternion();

            _ETERNAL.r.UseTransformable((t) =>
            {
                t.rotation = current;

                foreach (AxisApplied i in axes)
                {
                    t.Rotate(axisDirections[i.axis] * i.units, space);
                }

                newRot = t.rotation;
            });

            return newRot;
        } else if (space == Space.Self)
        {
            Vector3 total = current.eulerAngles;

            foreach (AxisApplied i in axes)
            {
                total += axisDirections[i.axis] * i.units;
            }

            return Quaternion.Euler(total);
        }

        return new Quaternion();
    }

    public Quaternion applyMixed (Quaternion current)
    {

    }

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

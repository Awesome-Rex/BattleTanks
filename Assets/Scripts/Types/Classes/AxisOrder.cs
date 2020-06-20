using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public enum Axis
{
    X, Y, Z
}

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

    public static Dictionary<Axis, Color> axisColours = new Dictionary<Axis, Color>
    {
        { Axis.X, new Color(219f / 255f, 62f / 255f, 29f / 255f) },
        { Axis.Y, new Color(154 / 255f, 243 / 255f, 72f / 255f) },
        { Axis.Z, new Color(58f / 255f, 122f / 255f, 237f / 255f) }
    };

    public List<Axis> axes = new List<Axis> { Axis.Z, Axis.Y, Axis.X };

    public bool only3 = true;

    public AxisOrder (List<Axis> axes = null, bool only3 = true)
    {
        if (axes == null)
        {
            this.axes = new List<Axis> { Axis.Z, Axis.Y, Axis.X };
        }
        else
        {
            this.axes = axes;
        }

        if (only3)
        {
            Fix();
        }
    }

    public void Fix ()
    {
        if (only3) {
            axes.Distinct();
            if (axes.Count > 3)
            {
                //axes = axes.GetRange(0, 3);
            }
            else if (axes.Count < 3)
            {
                if (!axes.Contains(Axis.X))
                {
                    axes.Add(Axis.X);
                }
                if (!axes.Contains(Axis.Y))
                {
                    axes.Add(Axis.Y);
                }
                if (!axes.Contains(Axis.Z))
                {
                    axes.Add(Axis.Z);
                }
            }
        }
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

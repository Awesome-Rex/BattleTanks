using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomPropertyDrawer(typeof(Space))]
public class SpacePD : PropertyDrawerPRO
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        OnGUIPRO(position, property, label, () => {
            if (property.enumValueIndex == (int)Space.Self) {
                if (GUI.Button(newPosition, "Self"))
                {
                    property.enumValueIndex = (int)Space.World;
                }
            } else
            {
                if (GUI.Button(newPosition, "World"))
                {
                    property.enumValueIndex = (int)Space.Self;
                }
            }
        });
    }
}

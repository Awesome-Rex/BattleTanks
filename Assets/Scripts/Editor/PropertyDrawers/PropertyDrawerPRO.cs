using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public class PropertyDrawerPRO : PropertyDrawer
{
    protected Rect indentedPosition;

    protected float lineHeight;
    protected float lines = 1;

    private int originalIndentLevel;

    //dynamic
    protected Rect newPosition;
    

    public void GuidedOnGUI(Rect position, SerializedProperty property, GUIContent label, System.Action action)
    {
        lineHeight = base.GetPropertyHeight(property, label);

        indentedPosition = EditorGUI.IndentedRect(position);
        indentedPosition.height = lineHeight;
        
        newPosition = indentedPosition;

        originalIndentLevel = EditorGUI.indentLevel;

        EditorGUI.BeginProperty(indentedPosition, label, property);

        action();

        EditorGUI.EndProperty();

        EditorGUI.indentLevel = originalIndentLevel;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * lines;
    }

    
}

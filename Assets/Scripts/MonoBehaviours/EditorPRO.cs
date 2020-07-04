#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public abstract class EditorPRO<T> : Editor where T : Object
{
    protected new T target;

    protected Dictionary<string, SerializedProperty> props = new Dictionary<string, SerializedProperty>();

    protected abstract void DeclareProperties();

    protected void AddProperty(string name)
    {
        props.Add(name, serializedObject.FindProperty(name));
    }
    protected SerializedProperty FindProperty (string name)
    {
        return props[name];
    }

    public static void Window (string title, System.Action content)
    {
        GUILayout.BeginVertical(title, "window");
        content();
        GUILayout.EndVertical();
    }

    //protected void Line(int height = 1, Color colour = default)
    //{
    //    if (colour == default)
    //    {
    //        colour = Color.gray;
    //    }

    //    Rect rect = EditorGUILayout.GetControlRect(false, height);
    //    rect.height = height;
    //    EditorGUI.DrawRect(rect, colour);
    //}

    public static void Line(Color? colour = default, int thickness = 2, int padding = 10)
    {
        if (colour == default)
        {
            colour = Color.gray;
        }

        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, (Color)colour);
    }

    public static void DisableGroup (bool value, System.Action content)
    {
        bool originalValue = GUI.enabled;

        GUI.enabled = value;
        content();
        GUI.enabled = originalValue;
    }

    protected virtual void OnEnable()
    {
        target = (T)(base.target);
        DeclareProperties();
    }

    public void OnInspectorGUIPRO(System.Action action)
    {
        EditorGUI.BeginChangeCheck();   //start

        action(); //content

        end(); //end
    }

    private void end()
    {
        serializedObject.ApplyModifiedProperties();
        if (EditorGUI.EndChangeCheck())
        {

        }
    }
}
#endif
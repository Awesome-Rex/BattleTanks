#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public abstract class EditorPRO<T> : Editor where T : Object
{
    protected new T target;

    //serialized properties
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

    //widgets and components
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

    public static void Function (string name, System.Action action, System.Action[] parameters, string undoMessage = null, Object target = null)
    {
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button(name, GUILayout.Width(EditorGUIUtility.labelWidth), GUILayout.ExpandHeight(true), GUILayout.Height(
                EditorGUIUtility.singleLineHeight * (parameters.Length > 0 ? parameters.Length : 1)
                )))
            {
                if (undoMessage != null && target != null) {
                    Undo.RecordObject(target, undoMessage);
                }

                action();
            }
            EditorGUILayout.BeginVertical();
            {
                foreach (System.Action i in parameters)
                {
                    i();
                }
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }

    //events
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
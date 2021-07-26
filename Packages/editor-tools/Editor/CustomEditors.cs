using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Reflection;
using UnityEditor;

namespace REXTools.EditorTools
{
    public static class CustomEditors
    {
        public static object GetParent(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }
        public static object GetValue(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f == null)
            {
                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p == null)
                    return null;
                return p.GetValue(source, null);
            }
            return f.GetValue(source);
        }
        public static object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as IEnumerable;
            var enm = enumerable.GetEnumerator();
            while (index-- >= 0)
                enm.MoveNext();
            return enm.Current;
        }



        //if "press" is pressed, set "set" to "val"
        //if "press" is already selected, disable it
        public static void EnumButton<T>(this Editor editor, System.Func<bool> press, T val, ref T set)// where T : System.Enum
        {
            bool original = GUI.enabled;
            if (set.Equals(val))
            {
                GUI.enabled = false;
            }

            if (press())
            {
                set = val;
            }

            GUI.enabled = original;
        }
        public static void EnumButton<T>(this EditorWindow editor, System.Func<bool> press, T val, ref T set)// where T : System.Enum
        {
            bool original = GUI.enabled;
            if (set.Equals(val))
            {
                GUI.enabled = false;
            }

            if (press())
            {
                set = val;
            }

            GUI.enabled = original;
        }
        public static void EnumButton<T>(this PropertyDrawer editor, System.Func<bool> press, T val, ref T set)// where T : System.Enum
        {
            bool original = GUI.enabled;
            if (set.Equals(val))
            {
                GUI.enabled = false;
            }

            if (press())
            {
                set = val;
            }

            GUI.enabled = original;
        }

        //if "press" if pressed, do nothing
        //if "press" is already selected, set checked
        public static void EnumMenuItem<T>(string press, T val, ref T set)
        {
            Menu.SetChecked(press, set.Equals(val));
        }
        //public static void EnumMenuItems<T>(Dictionary<string, T> menuItems) //equivelent to val and set ^^
        //{

        //}

        //private static void SerializeList(FieldInfo field)
        //{
        //    Debug.Log("Is a List");
        //    ICollection list = field.GetValue(focusedScript) as ICollection;
        //    serializedScript.ints.Add(list.Count);//Store the length of this list for later access

        //    foreach (var c in list)
        //    {//For every member of the list, get all the info from it
        //        FieldInfo[] subInfo = c.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        //        foreach (FieldInfo sub in subInfo)
        //        {//Then send every fieldInfo off to be processed
        //            target = c;//Set our collection to be the current target
        //            SerializeField(sub);//Send the field off to be serialized
        //            target = focusedScript;//When we get back here, set the target to be the focusedScript
        //        }
        //    }
        //}

        //private static void DeserializeList(FieldInfo field)
        //{
        //    int listLength = serializedScript.GetInt();//Get the length of this list
        //    System.Type type = field.FieldType.GetGenericArguments()[0];//Get the type of field

        //    Debug.Log("Deserializing a List of type " + type);
        //    var instancedList = (IList)typeof(List<>)//Create a Generic List that can hold our type
        //        .MakeGenericType(type)
        //        .GetConstructor(System.Type.EmptyTypes)
        //        .Invoke(null);

        //    for (int i = 0; i < listLength; i++)
        //    {//Then, create listLength instances of our deserialized class/struct
        //        FieldInfo[] subInfo = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        //        var member = System.Activator.CreateInstance(type);//Create a new member which will be added to  our instancedList

        //        foreach (FieldInfo sub in subInfo)
        //        {//Then
        //            target = member;
        //            DeserializeField(sub);
        //            target = focusedScript;
        //        }
        //        instancedList.Add(member);
        //    }
        //    field.SetValue(target, instancedList);//Once we get here, assign our deserialized list to our target script
        //}



        public static void Vector3TField(this PropertyDrawer editor, Rect position, Action<Rect> propertyX, Action<Rect> propertyY, Action<Rect> propertyZ, Action<Rect> label)
        {
            label(position);

            if (EditorGUIUtility.wideMode)
            {
                position.x += EditorGUIUtility.labelWidth + 2f;
                position.width -= EditorGUIUtility.labelWidth;//EditorGUI.PrefixLabel(position, GUIContent.none);



                float subLabelSpacing = 2f;//4f;
                float width = (position.width - (3 - 1) * subLabelSpacing) / 3;

                // backup gui settings
                int indent = EditorGUI.indentLevel;
                float labelWidth = EditorGUIUtility.labelWidth;

                //modify gui settings
                EditorGUI.indentLevel = 0;

                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent("X")).x; //M => substitute X, Y, or Z
                propertyX(new Rect(position.x + ((width + subLabelSpacing) * 0f), position.y, width - subLabelSpacing, position.height));
                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent("X")).x; //M => substitute X, Y, or Z
                propertyY(new Rect(position.x + ((width + subLabelSpacing) * 1f), position.y, width - subLabelSpacing, position.height));
                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent("X")).x; //M => substitute X, Y, or Z
                propertyZ(new Rect(position.x + ((width + subLabelSpacing) * 2f), position.y, width - subLabelSpacing, position.height));

                // restore gui settings
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUI.indentLevel = indent;
            }
            else
            {
                EditorGUI.indentLevel += 1;
                position = EditorGUI.IndentedRect(position);
                EditorGUI.indentLevel -= 1;
                position.y += EditorGUIUtility.singleLineHeight;
                position.height = EditorGUIUtility.singleLineHeight;



                float subLabelSpacing = 4f;
                float width = (position.width - (3 - 1) * subLabelSpacing) / 3;

                // backup gui settings
                int indent = EditorGUI.indentLevel;
                float labelWidth = EditorGUIUtility.labelWidth;

                //modify gui settings
                EditorGUI.indentLevel = 0;

                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent("X")).x; //M => substitute X, Y, or Z
                propertyX(new Rect(position.x + ((width + subLabelSpacing) * 0f), position.y, width, position.height));
                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent("X")).x; //M => substitute X, Y, or Z
                propertyY(new Rect(position.x + ((width + subLabelSpacing) * 1f), position.y, width, position.height));
                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent("X")).x; //M => substitute X, Y, or Z
                propertyZ(new Rect(position.x + ((width + subLabelSpacing) * 2f), position.y, width, position.height));

                // restore gui settings
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUI.indentLevel = indent;
            }
        }
        public static void Vector3TField(this PropertyDrawer editor, Rect position, Action<Rect> propertyX, Action<Rect> propertyY, Action<Rect> propertyZ, GUIContent label, bool axisLabels = true)
        {
            Vector3TField(editor, position, propertyX, propertyY, propertyZ, (position) =>
            {
                EditorGUI.LabelField(position, label);
            });
        }

        public static void Vector2TField(this PropertyDrawer editor, Rect position, Action<Rect> propertyX, Action<Rect> propertyY, Action<Rect> label)
        {
            label(position);

            if (EditorGUIUtility.wideMode)
            {
                position.x += EditorGUIUtility.labelWidth + 2f;
                position.width -= EditorGUIUtility.labelWidth;//EditorGUI.PrefixLabel(position, GUIContent.none);



                float subLabelSpacing = 2f;//4f;
                float width = (position.width - (3 - 1) * subLabelSpacing) / 3;

                // backup gui settings
                int indent = EditorGUI.indentLevel;
                float labelWidth = EditorGUIUtility.labelWidth;

                //modify gui settings
                EditorGUI.indentLevel = 0;

                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent("X")).x; //M => substitute X, Y, or Z
                propertyX(new Rect(position.x + ((width + subLabelSpacing) * 0f), position.y, width - subLabelSpacing, position.height));
                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent("X")).x; //M => substitute X, Y, or Z
                propertyY(new Rect(position.x + ((width + subLabelSpacing) * 1f), position.y, width - subLabelSpacing, position.height));
                
                // restore gui settings
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUI.indentLevel = indent;
            }
            else
            {
                EditorGUI.indentLevel += 1;
                position = EditorGUI.IndentedRect(position);
                EditorGUI.indentLevel -= 1;
                position.y += EditorGUIUtility.singleLineHeight;
                position.height = EditorGUIUtility.singleLineHeight;



                float subLabelSpacing = 4f;
                float width = (position.width - (2 - 1) * subLabelSpacing) / 2;

                // backup gui settings
                int indent = EditorGUI.indentLevel;
                float labelWidth = EditorGUIUtility.labelWidth;

                //modify gui settings
                EditorGUI.indentLevel = 0;

                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent("X")).x; //M => substitute X, Y, or Z
                propertyX(new Rect(position.x + ((width + subLabelSpacing) * 0f), position.y, width, position.height));
                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent("X")).x; //M => substitute X, Y, or Z
                propertyY(new Rect(position.x + ((width + subLabelSpacing) * 1f), position.y, width, position.height));
                
                // restore gui settings
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUI.indentLevel = indent;
            }
        }
        public static void Vector2TField(this PropertyDrawer editor, Rect position, Action<Rect> propertyX, Action<Rect> propertyY, GUIContent label, bool axisLabels = true)
        {
            Vector2TField(editor, position, propertyX, propertyY, (position) =>
            {
                EditorGUI.LabelField(position, label);
            });
        }
    }
}
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Reflection;
using UnityEditor;

namespace EditorTools
{
    public static class CustomInspector
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
    }
    public static class StyleExtensions
    {
        //<---------------------PROPERTIES----------------------->
        
        public static GUIStyle clone (this GUIStyle style)
        {
            return new GUIStyle(style);
        }

        public static GUIStyle fontStyle(this GUIStyle style, FontStyle fontStyle)
        {
            style.fontStyle = fontStyle;

            return style;
        }

        public static GUIStyle fontSize(this GUIStyle style, float size)
        {
            style.fontSize = (int)size;

            return style;
        }
        public static GUIStyle richText(this GUIStyle style)
        {
            style.richText = true;

            return style;
        }

        public static GUIStyle wordWrap(this GUIStyle style)
        {
            style.wordWrap = true;

            return style;
        }

        public static GUIStyle contentOffset(this GUIStyle style, Vector2 offset)
        {
            style.contentOffset = offset;

            return style;
        }

        public static GUIStyle padding(this GUIStyle style, RectOffset padding)
        {
            style.padding = padding;

            return style;
        }
        public static GUIStyle margin(this GUIStyle style, RectOffset margin)
        {
            style.margin = margin;

            return style;
        }
        public static GUIStyle border(this GUIStyle style, RectOffset border)
        {
            style.border = border;

            return style;
        }

        public static GUIStyle fixedHeight(this GUIStyle style, float val)
        {
            style.fixedHeight = val;

            return style;
        }
        public static GUIStyle fixedWidth(this GUIStyle style, float val)
        {
            style.fixedWidth = val;

            return style;
        }

        public static GUIStyle stretchHeight(this GUIStyle style)
        {
            style.stretchHeight = true;

            return style;
        }
        public static GUIStyle stretchWidth(this GUIStyle style)
        {
            style.stretchWidth = true;

            return style;
        }

        public static GUIStyle alignment(this GUIStyle style, TextAnchor anchor)
        {
            style.alignment = anchor;

            return style;
        }

        //<-----------------other layout stuff------------------->

        public static readonly RectOffset zero = new RectOffset(0, 0, 0, 0);


        //<-----------------UNITY HTML TAGS------------------->

        //single attribute, attribute list
        public static string atr(string attribute, string value)
        {
            return $"{attribute}={value}";
        }
        public static string atrL(params string[] attributes)
        {
            string all = "";

            for (int i = 0; i < attributes.Length; i += 2)
            {
                if (i + 1 < attributes.Length)
                {
                    all += atr(attributes[i], attributes[i + 1]) + " ";
                }
            }

            all.TrimEnd(' ');

            return all;
        }

        public static string tag(this string text, string name, string value = null, string attributes = null)
        {
            if (attributes != null)
            {
                if (value != null)
                {
                    return $"<{atr(name, value)} {attributes}>{text}</{name}>";
                }
                else
                {
                    return $"<{name} {attributes}>{text}</{name}>";
                }
            }
            else
            {
                if (value != null)
                {
                    return $"<{atr(name, value)}>{text}</{name}>";
                }
                else
                {
                    return $"<{name}>{text}</{name}>";
                }
            }
        }

        public static string colour(this string text, Color colour)
        {
            return text.tag("color", $"#{ColorUtility.ToHtmlStringRGBA(colour)}");
        }

        public static string bold(this string text)
        {
            return text.tag("b");
        }
        public static string italic(this string text)
        {
            return text.tag("i");
        }

        public static string textSize(this string text, float pixels)
        {
            return text.tag("size", pixels.ToString());
        }

        public static string material(this string text, int index)
        {
            return text.tag("material", index.ToString());
        }
        public static string quad(int materialIndex = 0, float pixelHeight = 20, Rect rectangle = default)
        {
            if (rectangle != default)
            {
                return $"<quad {atr("material", materialIndex.ToString())} {atr("size", pixelHeight.ToString())} {atr("x", rectangle.x.ToString())} {atr("y", rectangle.y.ToString())} {atr("width", rectangle.width.ToString())} {atr("height", rectangle.height.ToString())}/>";
            }
            else
            {
                return $"<quad {atr("material", materialIndex.ToString())} {atr("size", pixelHeight.ToString())} {atr("x", "0".ToString())} {atr("y", "0".ToString())} {atr("width", "0".ToString())} {atr("height", "0".ToString())}/>";
            }
        }
    }
}
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.EditorTools 
{
    public static class StyleExtensions
    {
        //<---------------------PROPERTIES----------------------->

        public static GUIStyle clone(this GUIStyle style)
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

        public static GUIStyle stretchHeight(this GUIStyle style, bool val = true)
        {
            style.stretchHeight = val;

            return style;
        }
        public static GUIStyle stretchWidth(this GUIStyle style, bool val = true)
        {
            style.stretchWidth = val;

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
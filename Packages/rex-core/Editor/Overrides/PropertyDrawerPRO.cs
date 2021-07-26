using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

using UnityEditor;

namespace REXTools.REXCore
{
    public abstract class PropertyDrawerPRO : PropertyDrawer
    {
        protected Renter renter
        {
            get
            {
                if (_renter == null)
                {
                    _renter = new Renter();
                }
                return _renter;
            }
        }
        private Renter _renter;

        protected Rect indentedPosition
        {
            get
            {
                return _indentedPosition;
            }
        }
        private Rect _indentedPosition;

        protected float lineHeight
        {
            get
            {
                return EditorGUIUtility.singleLineHeight;//_lineHeight;
            }
        }
        //protected float _lineHeight;
        protected float labelWidth
        {
            get
            {
                return EditorGUIUtility.labelWidth;
            }
        }
        protected float fieldWidth
        {
            get
            {
                return EditorGUIUtility.fieldWidth;
            }
        }

        private int originalIndentLevel;

        //dynamic
        protected float lines = 1;
        protected bool resetIndent = true;

        protected Rect newPosition; //starts at indented position (EdiotrGUI.IndentedRect)

        //protected SerializedObject _property;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
        }

        public void OnGUIPRO(Rect position, SerializedProperty property, GUIContent label, System.Action action)
        {
            start(position, property, label);
            action();
            end();
        }

        private void start (Rect position, SerializedProperty property, GUIContent label)
        {
            //private variables
            //_lineHeight = EditorGUIUtility.singleLineHeight;//base.GetPropertyHeight(property, label);

            _indentedPosition = EditorGUI.IndentedRect(position);
            _indentedPosition.height = lineHeight;

            //properties and public variables
            newPosition = indentedPosition;

            originalIndentLevel = EditorGUI.indentLevel;

            //_property = new SerializedObject(property.FindPropertyRelative("_private").objectReferenceValue);
            //_property = property.FindPropertyRelative("_private").serializedObject;
            
            EditorGUI.BeginProperty(indentedPosition, label, property);
        }
        private void end()
        {
            EditorGUI.EndProperty();

            if (resetIndent)
            {
                EditorGUI.indentLevel = originalIndentLevel;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * lines;
        }

        //local methods
        public void DrawDefaultProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            MethodInfo defaultDraw = typeof(EditorGUI).GetMethod("DefaultPropertyField", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            defaultDraw.Invoke(null, new object[3] { position, property, label });
        }

        //static methods
        public static void ApplyModifiedProperties(System.Action action, SerializedProperty property)
        {
            action();

            property.serializedObject.ApplyModifiedProperties();
        }
        public static void ApplyModifiedProperties(System.Action action, SerializedObject property)
        {
            action();

            property.ApplyModifiedProperties();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                return _lineHeight;
            }
        }
        protected float _lineHeight;

        private int originalIndentLevel;

        //dynamic
        protected float lines = 1;
        protected bool resetIndent = true;

        protected Rect newPosition;


        public void OnGUIPRO(Rect position, SerializedProperty property, GUIContent label, System.Action action)
        {
            _lineHeight = base.GetPropertyHeight(property, label);

            _indentedPosition = EditorGUI.IndentedRect(position);
            _indentedPosition.height = lineHeight;

            newPosition = indentedPosition;

            originalIndentLevel = EditorGUI.indentLevel;

            EditorGUI.BeginProperty(indentedPosition, label, property);

            action();

            end();
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
    }
}
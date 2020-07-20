using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct Tag
{
    public string name;

    public int id;

    public List<Tag> tags;

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Tag))]
    public class P : PropertyDrawerPRO
    {
        private bool expanded = false;
        private bool showID = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIPRO(position, property, label, () => {
                EditorGUI.indentLevel = 0;
                if (expanded) {
                    lines = 1f + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("tags"));
                } else
                {
                    lines = 1f;
                }

                //CONTENT
                newPosition = indentedPosition;

                if (showID) {
                    newPosition.width /= 2;
                } else
                {
                    newPosition.width -= lineHeight;
                }
                EditorGUI.PropertyField(newPosition, property.FindPropertyRelative("name"), GUIContent.none);

                if (showID) {
                    newPosition = indentedPosition;
                    newPosition.width /= 2;
                    newPosition.x += indentedPosition.width / 2f;
                    newPosition.width -= lineHeight;
                    EditorGUI.PropertyField(newPosition, property.FindPropertyRelative("id"), GUIContent.none);
                }

                newPosition = indentedPosition;
                newPosition.size = Vector2.one * lineHeight;
                newPosition.x = indentedPosition.width;
                if (GUI.Button(newPosition, GUIContent.none))
                {
                    showID = !showID;
                }
                
                newPosition.x = indentedPosition.x - lineHeight;
                if (GUI.Button(newPosition, GUIContent.none))
                {
                    expanded = !expanded;
                }

                if (expanded)
                {
                    newPosition = indentedPosition;
                    newPosition.y += lineHeight * 1f;
                    newPosition.height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("tags"));
                    EditorGUI.PropertyField(newPosition, property.FindPropertyRelative("tags"));
                }
                //END
            });
        }
    }
#endif
}

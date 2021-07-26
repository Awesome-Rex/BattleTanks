using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using REXTools.REXCore;
using REXTools.EditorTools;

[CustomPropertyDrawer(typeof(Space))]
public class SpacePropertyDrawer : PropertyDrawerPRO
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        OnGUIPRO(position, property, label, () =>
        {
            if (position.size.x / position.size.y < 1.5f && position.size.y == lineHeight)
            {
                GUIStyle ruleStyle = GUI.skin.button.padding(new RectOffset(0, 0, 0, 0));

                if (property.enumValueIndex == (int)Space.Self)
                {
                    if (GUI.Button(newPosition, new GUIContent(Resources.Load("Textures/Spaces/Self") as Texture, "Space.Self"), ruleStyle))
                    {
                        property.enumValueIndex = (int)Space.World;
                    }
                }
                else
                {
                    if (GUI.Button(newPosition, new GUIContent(Resources.Load("Textures/Spaces/World") as Texture, "Space.World"), ruleStyle))
                    {
                        property.enumValueIndex = (int)Space.Self;
                    }
                }
            }
            else
            {
                if (property.enumValueIndex == (int)Space.Self)
                {
                    if (GUI.Button(newPosition, "Self"))
                    {
                        property.enumValueIndex = (int)Space.World;
                    }
                }
                else
                {
                    if (GUI.Button(newPosition, "World"))
                    {
                        property.enumValueIndex = (int)Space.Self;
                    }
                }
            }
        });
    }
}

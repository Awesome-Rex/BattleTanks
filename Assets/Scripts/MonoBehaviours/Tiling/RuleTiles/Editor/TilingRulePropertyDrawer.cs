using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using REXTools.TransformTools;
using REXTools.REXCore;

namespace REXTools.Tiling {
    [CustomPropertyDrawer(typeof(TilingRule))]
    public class TilingRulePropertyDrawer : PropertyDrawerPRO
    {
        RuleBehaviour BehaviourButton(Rect position, RuleBehaviour behaviour, string tooltip = "")
        {
            bool eraserOn = Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.LeftAlt || Event.current.keyCode == KeyCode.RightAlt);

            if (behaviour == RuleBehaviour.DontCare) //don't care -> this
            {
                if (GUI.Button(position, new GUIContent(string.Empty, tooltip)))
                {
                    if (!eraserOn)
                    {
                        return RuleBehaviour.This;
                    } else
                    {
                        return RuleBehaviour.DontCare;
                    }
                }
            }
            else if (behaviour == RuleBehaviour.This) //this -> not this
            {
                if (GUI.Button(position, new GUIContent("O", tooltip)/*, REXCore.GUITextTools*/))
                {
                    if (!eraserOn)
                    {
                        return RuleBehaviour.NotThis;
                    }
                    else
                    {
                        return RuleBehaviour.DontCare;
                    }
                }
            }
            else if (behaviour == RuleBehaviour.NotThis) //not this -> don't care
            {
                if (GUI.Button(position, new GUIContent("X", tooltip)))
                {
                    if (!eraserOn)
                    {
                        return RuleBehaviour.DontCare;
                    }
                    else
                    {
                        return RuleBehaviour.DontCare;
                    }
                }
            }

            return behaviour;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIPRO(position, property, label, () => {
                //foreach (KeyValuePair<Vector3T<TileLean>, RuleBehaviour> i in ((TilingRule)property.serializedObject.targetObject).rules)
                //{

                //}
                newPosition.width = lineHeight;
                
                
                BehaviourButton(newPosition, (RuleBehaviour)property.FindPropertyRelative("ppn").enumValueIndex, "Something");
                newPosition.x += lineHeight;
                BehaviourButton(newPosition, (RuleBehaviour)property.FindPropertyRelative("_pn").enumValueIndex, "Something");
                newPosition.x += lineHeight;
                BehaviourButton(newPosition, (RuleBehaviour)property.FindPropertyRelative("npn").enumValueIndex, "Something");
                newPosition.y += lineHeight * 2f;

                _lineHeight = 9f + 2f;
            });
        }
    }
}
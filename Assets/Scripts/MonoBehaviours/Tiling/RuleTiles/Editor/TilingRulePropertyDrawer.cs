using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using REXTools.TransformTools;
using REXTools.REXCore;
using REXTools.EditorTools;
using System.Linq;

namespace REXTools.Tiling
{
    [CustomPropertyDrawer(typeof(TilingRule))]
    public class TilingRulePropertyDrawer : PropertyDrawerPRO
    {
        public static RuleBehaviour? brushType = null;

        //editor settings
        //_

        //private settings
        private bool maximized = true;

        private Vector2 scrollPosition = Vector2.zero;


        //GUI methods
        private RuleBehaviour BehaviourButton(Rect position, RuleBehaviour behaviour, string tooltip = "")
        {
            RuleBehaviour ButtonResponse(RuleBehaviour newBehaviour)
            {
                if (brushType == null)
                {
                    return newBehaviour;
                }
                else if (brushType == RuleBehaviour.DontCare)
                {
                    return RuleBehaviour.DontCare;
                }
                else if (brushType == RuleBehaviour.This)
                {
                    return RuleBehaviour.This;
                }
                else if (brushType == RuleBehaviour.NotThis)
                {
                    return RuleBehaviour.NotThis;
                }

                return default;
            }

            if (behaviour == RuleBehaviour.DontCare)
            {
                tooltip += " (don't care)";
            }
            else if (behaviour == RuleBehaviour.This)
            {
                tooltip += " (this)";
            }
            else if (behaviour == RuleBehaviour.NotThis)
            {
                tooltip += " (not this)";
            }

            GUIStyle ruleStyle = /*GUI.skin.box.clone()*/new GUIStyle().padding(new RectOffset(2, 2, 2, 2)).margin(new RectOffset(0, 0, 0, 0));
            //ruleStyle.border = new RectOffset(5, 5, 5, 5);
            ruleStyle.normal.background = Resources.Load("Textures/RuleBehaviours/Background") as Texture2D;

            if (behaviour == RuleBehaviour.DontCare) //don't care -> this
            {
                if (GUI.Button(position, new GUIContent(string.Empty, tooltip), ruleStyle))
                {
                    return ButtonResponse(RuleBehaviour.This);
                }
            }
            else if (behaviour == RuleBehaviour.This) //this -> not this
            {
                //if (GUI.Button(position, new GUIContent("O", tooltip)))
                if (GUI.Button(position, new GUIContent(Resources.Load("Textures/RuleBehaviours/This") as Texture, tooltip), ruleStyle))
                {
                    return ButtonResponse(RuleBehaviour.NotThis);
                }
            }
            else if (behaviour == RuleBehaviour.NotThis) //not this -> don't care
            {
                //if (GUI.Button(position, new GUIContent("X", tooltip)))
                if (GUI.Button(position, new GUIContent(Resources.Load("Textures/RuleBehaviours/NotThis") as Texture, tooltip), ruleStyle))
                {
                    return ButtonResponse(RuleBehaviour.DontCare);
                }
            }

            return behaviour;
        }

        private Vector2 RulePosition(string rule)
        {
            ////vertical
            //return new Vector2(
            //    -TilingRule.signPositions[TilingRule.ruleSigns[rule]].x + 1, // prevents x = -1
            //    //((TilingRule.signPositions[TilingRule.ruleSigns[rule]].y + -1 /* prevents x = 1 */) * 3f) + (-TilingRule.signPositions[TilingRule.ruleSigns[rule]].z - 1 /* prevents z = 1 */)
            //    ((-TilingRule.signPositions[TilingRule.ruleSigns[rule]].y + 1/* prevents x = -1 */) * 3f) +
            //    RMath.ClampMin(-TilingRule.signPositions[TilingRule.ruleSigns[rule]].y + 1/* prevents x = -1 */, 0) +
            //    (TilingRule.signPositions[TilingRule.ruleSigns[rule]].z + 1 /* prevents z = -1 */)
            //);

            ////horizontal
            return new Vector2(
                (-TilingRule.ruleSigns[rule].direction.x + 1) + //x
                ((TilingRule.ruleSigns[rule].direction.y + 1) * 3f) + //y
                RMath.ClampMin(TilingRule.ruleSigns[rule].direction.y + 1, 0), //z

                (TilingRule.ruleSigns[rule].direction.z + 1)
            );
        }

        private string RuleTooltip(string rule)
        {
            Vector3 position = TilingRule.ruleSigns[rule].direction;

            return position.x.ToString() + " " + position.y.ToString() + " " + position.z.ToString();
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TilingRule PropertyToTilingRule (SerializedProperty prop)
            {
                //converts property to local tiling rule
                TilingRule tilingRule = new TilingRule();

                foreach (string rule in TilingRule.ruleNames)
                {
                    tilingRule.SetRule(rule, (RuleBehaviour)prop.FindPropertyRelative(rule).enumValueIndex);
                }

                return tilingRule;
            }
            SerializedProperty TilingRuleToProperty (TilingRule tilingRule, SerializedProperty prop)
            {
                //sets property values to newTilingRule values
                foreach (string rule in TilingRule.ruleNames)
                {
                    prop.FindPropertyRelative(rule).enumValueIndex = (int)tilingRule.GetRule(rule);
                }

                return prop;
            }


            //Context Menu methods
            void Clear()
            {
                TilingRule clearRule = new TilingRule();

                foreach (string rule in TilingRule.ruleNames)
                {
                    clearRule.SetRule(rule, RuleBehaviour.DontCare);
                }

                TilingRuleToProperty(clearRule, property);

                property.serializedObject.ApplyModifiedProperties();
            }
            void Rotate(UnityEngine.Vector3Int euler)
            {//MAIN OPERATION - makes new tiling rule with flipped rules along axis
                TilingRule oldTilingRule = PropertyToTilingRule(property);
                TilingRule newTilingRule = new TilingRule();

                foreach (string rule in TilingRule.ruleNames)
                {
                    newTilingRule.SetRule(
                        SignAdjacency<dynamic>.ruleSigns.First(x => x.Value.Equals(SignAdjacency<dynamic>.ruleSigns[rule].Rotate(euler))).Key,
                        oldTilingRule.GetRule(rule)
                    );
                }

                TilingRuleToProperty(newTilingRule, property);

                property.serializedObject.ApplyModifiedProperties();
            }
            void Mirror(Axis axis)
            {//MAIN OPERATION - makes new tiling rule with flipped rules along axis
                TilingRule oldTilingRule = PropertyToTilingRule(property);
                TilingRule newTilingRule = new TilingRule();

                foreach (string rule in TilingRule.ruleNames)
                {
                    //string newRule = TilingRule.ruleSigns.First(x => x.Value == TilingRule.ruleSigns[rule].SetAxis(axis, TilingRule.ruleSigns[rule].GetAxis(axis).Negative())).Key;

                    //newTilingRule.SetRule(rule, oldTilingRule.GetRule(newRule));

                    newTilingRule.SetRule(
                        SignAdjacency<dynamic>.ruleSigns.First(x => x.Value.Equals(SignAdjacency<dynamic>.ruleSigns[rule].Mirror(axis))).Key, 
                        oldTilingRule.GetRule(rule)
                    );
                }

                TilingRuleToProperty(newTilingRule, property);

                property.serializedObject.ApplyModifiedProperties();
            }

            OnGUIPRO(position, property, label, () =>
            {
                //Vector3T<Sign> test = (new Vector3T<Sign>(Sign.Negative, Sign.Negative, Sign.Negative)).Mirror(Axis.X);
                //Vector3T<Sign> test = (new Vector3T<Sign>(Sign.Neutral, Sign.Positive, Sign.Positive)).Rotate(new UnityEngine.Vector3Int(1, 0, 0));
                //Debug.Log(test.x.ToString() + " " + test.y.ToString() + " " + test.z.ToString());

                lines = 1f;

                maximized = EditorGUI.Foldout(newPosition, maximized, /*maximized ? GUIContent.none : new GUIContent(*/property.displayName/*)*/);


                //Context menu
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && position.Contains(Event.current.mousePosition))
                {
                    GenericMenu context = new GenericMenu();

                    //context.AddItem(new GUIContent("Show Tiling Rule Settings Window"), false, () => TilingRuleWindow.ShowWindow());
                    context.AddItem(new GUIContent("Tools/Select Brush/Cycle"), brushType == null, () => brushType = null);
                    context.AddItem(new GUIContent("Tools/Select Brush/Eraser"), brushType == RuleBehaviour.DontCare, () => brushType = RuleBehaviour.DontCare);
                    context.AddItem(new GUIContent("Tools/Select Brush/Continuous"), brushType == RuleBehaviour.This, () => brushType = RuleBehaviour.This);
                    context.AddItem(new GUIContent("Tools/Select Brush/Hard Edge"), brushType == RuleBehaviour.NotThis, () => brushType = RuleBehaviour.NotThis);

                    context.AddItem(new GUIContent("Edit/Rotate/+X"), false, () => Rotate(new UnityEngine.Vector3Int(1, 0, 0)));
                    context.AddItem(new GUIContent("Edit/Rotate/-X"), false, () => Rotate(new UnityEngine.Vector3Int(-1, 0, 0)));
                    context.AddItem(new GUIContent("Edit/Rotate/+Y"), false, () => Rotate(new UnityEngine.Vector3Int(0, 1, 0)));
                    context.AddItem(new GUIContent("Edit/Rotate/-Y"), false, () => Rotate(new UnityEngine.Vector3Int(0, -1, 0)));
                    context.AddItem(new GUIContent("Edit/Rotate/+Z"), false, () => Rotate(new UnityEngine.Vector3Int(0, 0, 1)));
                    context.AddItem(new GUIContent("Edit/Rotate/-Z"), false, () => Rotate(new UnityEngine.Vector3Int(0, 0, -1)));

                    context.AddItem(new GUIContent("Edit/Mirror/X"), false, () => Mirror(Axis.X));
                    context.AddItem(new GUIContent("Edit/Mirror/Y"), false, () => Mirror(Axis.Y));
                    context.AddItem(new GUIContent("Edit/Mirror/Z"), false, () => Mirror(Axis.Z));
                    
                    context.AddItem(new GUIContent("Edit/Clear"), false, () => Clear());

                    context.ShowAsContext();
                }


                newPosition.width = lineHeight;

                Rect originPosition = newPosition;
                originPosition.y += lineHeight;

                //Draws tiling rules
                if (maximized)
                {
                    //lines += 9f + 2f;
                    lines += 3f;

                    //Debug.Log(lines);

                    newPosition.y += lineHeight * 1f;
                    newPosition.height = lineHeight * (3f + GUI.skin.horizontalScrollbar.fixedHeight);
                    newPosition.width = indentedPosition.width;

                    //scrollPosition = GUI.BeginScrollView(newPosition, scrollPosition, new Rect(originPosition.position, new Vector2(lineHeight * (9f + 2f + 5f), lineHeight * 3f)));

                    foreach (string rule in TilingRule.ruleNames)
                    {
                        property.FindPropertyRelative(rule).enumValueIndex = (int)BehaviourButton(
                            new Rect(
                                originPosition.position + ((RulePosition(rule) + Vector2.right) * lineHeight), 
                                Vector2.one * lineHeight
                            ),
                            (RuleBehaviour)property.FindPropertyRelative(rule).enumValueIndex,
                            RuleTooltip(rule)
                        );
                    }

                    //GUI.EndScrollView();
                }
            });
        }
    }
}
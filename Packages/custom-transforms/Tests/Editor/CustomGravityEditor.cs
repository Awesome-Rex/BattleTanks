using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using REXTools.REXCore;
using REXTools.EditorTools;
using System;

namespace REXTools.CustomTransforms
{
    [CustomEditor(typeof(CustomGravity))]
    public class CustomGravityEditor : EditorPRO<CustomGravity>
    {
        //method parameters
        private Space P_Switch_Space;
        private Link P_Switch_Link;

        private Transform P_SwitchParent_Parent;

        private LinkSpace P_SetContext_Type;
        private Vector3 P_SetContext_New;

        private Space P_SetVelocity_Type;
        private Vector3 P_SetVelocity_New;

        protected override void DeclareProperties()
        {
            AddProperty("offset");

            AddProperty("space");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            target.rigidbody = target.GetComponent<Rigidbody>();
        }

        public override void OnInspectorGUI()
        {
            OnInspectorGUIPRO(() =>
            {
                EditorGUILayout.PropertyField(FindProperty("space"));

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Direction", EditorStyles.boldLabel);
                if (target.space == Space.Self)
                {
                    target.parent = (Transform)EditorGUILayout.ObjectField("Parent", target.parent, typeof(Transform), true);
                }
                target.value = EditorGUILayout.Vector3Field("Value", target.value);
                target.gravity = EditorGUILayout.FloatField("Force", target.gravity);
                target.gravityScale = EditorGUILayout.FloatField("Gravity Scale", target.gravityScale);

                EditorGUILayout.Space();

            //Local
            if (target.space == Space.Self)
                {
                    EditorGUILayout.LabelField("Local", EditorStyles.boldLabel);

                    EditorGUILayout.PropertyField(FindProperty("offset"));

                    EditorGUILayout.Space();
                }

                target.showContextInfo = EditorGUILayout.Foldout(target.showContextInfo, "Info", true, EditorStyles.foldout.clone().richText());
                if (target.showContextInfo)
                {
                    GUI.enabled = false;

                //local and global directions
                EditorGUILayout.Vector3Field("Direction", target.direction);
                    if (target.space == Space.Self)
                    {
                        EditorGUILayout.Vector3Field("Local Direction", target.localDirection);
                    }

                    if (target.space == Space.Self)
                    {
                        EditorGUILayout.Space();

                        EditorGUILayout.Vector3Field("Direction Raw", target.directionRaw);
                        if (target.space == Space.Self)
                        {
                            EditorGUILayout.Vector3Field("Local Direction Raw", target.localDirectionRaw);
                        }
                    }

                    if (target.space == Space.Self)
                    {
                        EditorGUILayout.Space();
                    }

                //local and global velocity
                EditorGUILayout.Vector3Field("Velocity", target.velocity);
                    if (target.space == Space.Self)
                    {
                        EditorGUILayout.Vector3Field("Local Velocity", target.localVelocity);
                    }

                    GUI.enabled = true;

                    EditorGUILayout.Space();
                }

            //Editor methods!
            //Line();

            if (EditorApplication.isPaused || !EditorApplication.isPlaying)
                {
                    target.showMethods = EditorGUILayout.Foldout(target.showMethods, "Methods", true, EditorStyles.foldout.clone().richText());
                    if (target.showMethods)
                    {
                        Function("Switch", () =>
                        {
                            target.Switch(P_Switch_Space, P_Switch_Link);
                        }, new Action[] {
                                () => P_Switch_Space = (Space)EditorGUILayout.EnumPopup(GUIContent.none, P_Switch_Space),
                                () => P_Switch_Link = (Link)EditorGUILayout.EnumPopup(GUIContent.none, P_Switch_Link)
                                }, "Switched CustomGravity Space and/or Link", target.gameObject);

                        Function("Switch Parent", () =>
                        {
                            target.SwitchParent(P_SwitchParent_Parent);
                        }, new Action[] {
                                () => P_SwitchParent_Parent = (Transform)EditorGUILayout.ObjectField(GUIContent.none, P_SwitchParent_Parent, typeof(Transform), true)
                                }, "Switched CustomGravity Parent", target.gameObject);

                        Function("Remove Offset", () =>
                        {
                            if (EditorUtility.DisplayDialog(
                                "Remove Offset?",
                                "Are you sure you want to remove the offset of \"CustomGravity?\"",
                                "Yes", "Cancel"))
                            {
                                Undo.RecordObject(target.gameObject, "Removed CustomGravity Offset");

                                target.RemoveOffset();
                            }
                        }, new Action[] { });


                        Function("Set Context", () =>
                        {
                            if (P_SetContext_Type == LinkSpace.World)
                            {
                                target.direction = P_SetContext_New;
                            }
                            else if (P_SetContext_Type == LinkSpace.Self)
                            {
                                target.localDirection = P_SetContext_New;
                            }
                            else if (P_SetContext_Type == LinkSpace.WorldRaw)
                            {
                                target.directionRaw = P_SetContext_New;
                            }
                            else if (P_SetContext_Type == LinkSpace.SelfRaw)
                            {
                                target.localDirectionRaw = P_SetContext_New;
                            }
                        },
                            new Action[] {
                                    () => {
                                    EditorGUI.BeginChangeCheck();
                                    P_SetContext_Type = (LinkSpace)EditorGUILayout.EnumPopup("Type", P_SetContext_Type);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        if (P_SetContext_Type == LinkSpace.World)
                                        {
                                            P_SetContext_New = target.direction;
                                        }
                                        else if (P_SetContext_Type == LinkSpace.Self)
                                        {
                                            P_SetContext_New = target.localDirection;
                                        }
                                        else if (P_SetContext_Type == LinkSpace.WorldRaw)
                                        {
                                            P_SetContext_New = target.directionRaw;
                                        }
                                        else if (P_SetContext_Type == LinkSpace.SelfRaw)
                                        {
                                            P_SetContext_New = target.localDirectionRaw;
                                        }
                                    }
                                    },
                                    () => P_SetContext_New = EditorGUILayout.Vector3Field(GUIContent.none, P_SetContext_New)
                            }, "Changed Context Value of CustomGravity", target.gameObject);

                    //
                    Function("Set Velocity", () =>
                        {
                            if (P_SetVelocity_Type == Space.World)
                            {
                                target.velocity = P_SetVelocity_New;
                            }
                            else if (P_SetVelocity_Type == Space.Self)
                            {
                                target.localVelocity = P_SetVelocity_New;
                            }
                        },
                            new Action[] {
                                    () => {
                                    EditorGUI.BeginChangeCheck();
                                    P_SetVelocity_Type = (Space)EditorGUILayout.EnumPopup("Type", P_SetVelocity_Type);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        if (P_SetVelocity_Type == Space.World)
                                        {
                                            P_SetVelocity_New = target.velocity;
                                        }
                                        else if (P_SetVelocity_Type == Space.Self)
                                        {
                                            P_SetVelocity_New = target.localVelocity;
                                        }
                                    }
                                    },
                                    () => P_SetVelocity_New = EditorGUILayout.Vector3Field(GUIContent.none, P_SetVelocity_New)
                            }, "Changed Rigidbody Velocity from CustomGravity", target.gameObject);
                    }

                }
            });
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using REXTools.REXCore;
using REXTools.EditorTools;
using System;

namespace REXTools.CustomTransforms
{
    [CustomEditor(typeof(CustomRotation))]
    public class CustomRotationEditor : EditorPRO<CustomRotation>
    {
        //method parameters
        private Space P_Switch_Space;
        private Link P_Switch_Link;

        private Transform P_SwitchParent_Parent;

        private LinkSpace P_SetContext_Type;
        private Vector3 P_SetContext_New;

        protected override void DeclareProperties()
        {
            AddProperty("value");

            AddProperty("transition");
            AddProperty("offset");
            AddProperty("link");

            AddProperty("space");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            target.RecordParent();
        }

        public override void OnInspectorGUI()
        {
            OnInspectorGUIPRO(() =>
            {
                EditorGUILayout.PropertyField(FindProperty("space"));

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Rotation", EditorStyles.boldLabel);
                if (target.space == Space.Self)
                {
                    target.parent = (Transform)EditorGUILayout.ObjectField("Parent", target.parent, typeof(Transform), true);
                }
                if (!(target.space == Space.Self && target.link == Link.Match))
                {
                    //target.value = Quaternion.Euler(EditorGUILayout.Vector3Field("Value", target.value.eulerAngles));
                    EditorGUILayout.PropertyField(FindProperty("value"));
                }

                EditorGUILayout.Space();

                if (target.space == Space.Self)
                {
                    EditorGUILayout.LabelField("Local", EditorStyles.boldLabel);

                    EditorGUILayout.PropertyField(FindProperty("link"));

                    if (target.link == Link.Offset)
                    {
                        EditorGUILayout.PropertyField(FindProperty("offset"));
                    }

                    EditorGUILayout.Space();
                }

                if (target.space == Space.Self && target.link == Link.Offset)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Transition", EditorStyles.boldLabel);

                    target.follow = EditorGUILayout.Toggle(string.Empty, target.follow);

                    EditorGUILayout.EndHorizontal();
                    if (target.follow)
                    {
                        EditorGUILayout.PropertyField(FindProperty("transition"));
                    }

                    EditorGUILayout.Space();
                }

                target.showContextInfo = EditorGUILayout.Foldout(target.showContextInfo, "Info", true, EditorStyles.foldout.clone().richText());
                if (target.showContextInfo)
                {
                    GUI.enabled = false;

                    EditorGUILayout.Vector3Field("Euler Angles", target.eulerAngles);
                    if (target.space == Space.Self)
                    {
                        EditorGUILayout.Vector3Field("Local Euler Angles", target.localEulerAngles);
                    }

                    if (target.space == Space.Self && target.link == Link.Offset)
                    {
                        EditorGUILayout.Space();

                        EditorGUILayout.Vector3Field("Euler Angles Raw", target.eulerAnglesRaw);
                        if (target.space == Space.Self)
                        {
                            EditorGUILayout.Vector3Field("Local Euler Angles Raw", target.localEulerAnglesRaw);
                        }
                    }

                    GUI.enabled = true;

                    EditorGUILayout.Space();
                }

                if (EditorApplication.isPaused || !EditorApplication.isPlaying)
                {
                    //Line();

                    target.showMethods = EditorGUILayout.Foldout(target.showMethods, "Methods", true, EditorStyles.foldout.clone().richText());

                    if (target.showMethods)
                    {
                        if (target.applyInEditor)
                        {
                            GUI.enabled = false;
                        }

                        if (GUILayout.Button("Target to Current"))
                        {
                            Undo.RecordObject(target.gameObject, "Re-Oriented CustomRotation");

                            target.TargetToCurrent();
                        }

                        Function("Switch", () =>
                        {
                            target.Switch(P_Switch_Space, P_Switch_Link);
                        },
                        new Action[] {
                                    () => P_Switch_Space = (Space)EditorGUILayout.EnumPopup(GUIContent.none, P_Switch_Space),
                                    () => P_Switch_Link = (Link)EditorGUILayout.EnumPopup(GUIContent.none, P_Switch_Link)
                        }, "Switched CustomRotation Space and/or Link", target.gameObject);

                        Function("Switch Parent", () =>
                        {
                            target.Switch(P_Switch_Space, P_Switch_Link);
                        },
                    new Action[] {
                                    () => P_SwitchParent_Parent = (Transform)EditorGUILayout.ObjectField(GUIContent.none, P_SwitchParent_Parent, typeof(Transform), true)
                    }, "Switched CustomRotation Parent", target.gameObject);

                        Function("Remove Offset", () =>
                        {
                            if (EditorUtility.DisplayDialog(
                                "Remove Offset?",
                                "Are you sure you want to remove the offset of \"CustomRotation?\"",
                                "Yes", "Cancel"))
                            {
                                Undo.RecordObject(target.gameObject, "Removed CustomRotation Offset");

                                target.RemoveOffset();
                            }
                        },
                    new Action[] { });


                        GUI.enabled = true;

                        if (!target.applyInEditor)
                        {
                            GUI.enabled = false;
                        }

                        //setContext Function
                        Function("Set Property", () =>
                        {
                            if (P_SetContext_Type == LinkSpace.World)
                            {
                                target.rotation = Quaternion.Euler(P_SetContext_New);
                            }
                            else if (P_SetContext_Type == LinkSpace.Self)
                            {
                                target.localRotation = Quaternion.Euler(P_SetContext_New);
                            }
                            else if (P_SetContext_Type == LinkSpace.WorldRaw)
                            {
                                target.rotationRaw = Quaternion.Euler(P_SetContext_New);
                            }
                            else if (P_SetContext_Type == LinkSpace.SelfRaw)
                            {
                                target.localRotationRaw = Quaternion.Euler(P_SetContext_New);
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
                                        P_SetContext_New = target.rotation.eulerAngles;
                                    }
                                    else if (P_SetContext_Type == LinkSpace.Self)
                                    {
                                        P_SetContext_New = target.localRotation.eulerAngles;
                                    }
                                    else if (P_SetContext_Type == LinkSpace.WorldRaw)
                                    {
                                        P_SetContext_New = target.rotationRaw.eulerAngles;
                                    }
                                    else if (P_SetContext_Type == LinkSpace.SelfRaw)
                                    {
                                        P_SetContext_New = target.localRotationRaw.eulerAngles;
                                        }
                                    }
                            },
                                    () => P_SetContext_New = EditorGUILayout.Vector3Field(GUIContent.none, P_SetContext_New)
                                }, "Changed Context Value of CustomRotation", target.gameObject);

                        GUI.enabled = true;
                    }

                    EditorGUILayout.Space();

                    //Apply button
                    if (!target.applyInEditor)
                    {
                        if (EditorApplication.isPaused)
                        {
                            target.EditorApplyCheck();
                        }

                        if (GUILayout.Button(
                            "Apply in Editor".bold(),
                            EditorStyles.miniButton.clone().richText().fixedHeight(EditorGUIUtility.singleLineHeight * 1.5f)
                            ))
                        {
                            Undo.RecordObject(target.gameObject, "Applied CustomRotation Values in Editor");

                            target.applyInEditor = true;

                            target.EditorApplyCheck();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(
                            "Don't Apply in Editor".colour(Color.red).bold(),
                            EditorStyles.miniButton.clone().richText().fixedHeight(EditorGUIUtility.singleLineHeight * 1.5f)
                            ))
                        {
                            target.applyInEditor = false;

                            target.EditorApplyCheck();
                        }
                    }
                }
            });
        }
    }
}
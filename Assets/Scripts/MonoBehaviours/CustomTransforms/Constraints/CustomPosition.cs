using System;
using UnityEngine;

using EditorTools;

#if UNITY_EDITOR
using UnityEditor;
#endif

using TransformTools;

public class CustomPosition : CustomTransformLinks<Vector3>
{
    public Vector3 position
    {
        get
        {
            return GetPosition(Space.World);
        }
        set
        {
            if (!(space == Space.Self && link == Link.Offset))
            {
                if (space == Space.World)
                {
                    this.value = value;
                }
                operationalPosition = SetPosition(value, Space.World);
            }
            else
            {
                this.value = SetPositionLocal(offset.ReversePosition(this, value), Space.World);
            }
        }
    }
    public Vector3 localPosition
    {
        get
        {
            return GetPosition(Space.Self);
        }
        set
        {
            if (space == Space.Self)
            {
                if (link != Link.Offset)
                {
                    operationalPosition = SetPosition(value, Space.Self);
                }
                else
                {
                    this.value = SetPositionLocal(offset.ReversePosition(this, SetPosition(value, Space.Self)), Space.World);
                }
            }
            else
            {
                position = value;
            }
        }
    }

    public Vector3 positionRaw
    {
        get
        {
            if (space == Space.Self && link == Link.Offset)
            {
                return GetPositionRaw(Space.World);
            }
            else
            {
                return position;
            }
        }
        set
        {
            if (space == Space.Self && link == Link.Offset)
            {
                this.value = SetPositionRawLocal(value, Space.World);
            } else
            {
                position = value;
            }
        }
    }
    public Vector3 localPositionRaw
    {
        get
        {
            if (space == Space.Self && link == Link.Offset)
            {
                return GetPositionRaw(Space.Self);
            }
            else
            {
                return localPosition;
            }
        }
        set
        {
            if (space == Space.Self && link == Link.Offset)
            {
                this.value = SetPositionRawLocal(value, Space.Self);
            } else
            {
                localPosition = value;
            }
        }
    }

    public bool factorScale = true;
    public float offsetScale = 1f;

    private Vector3 previousDirection;

    private Vector3 parentPos;
    private Quaternion parentRot; //USE THE STUFF HERE 
    private Vector3 parentScale;

    private Vector3 operationalPosition
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
        }
    }

    public override void SetToTarget()
    {
        //Debug.Log("Position - " + value);

        target = GetTarget();

        if (enabled)
        {
            operationalPosition = target;

            RecordParent();
        }
    }
    public override void MoveToTarget()
    {
        target = GetTarget();

        if (enabled)
        {
            if (space == Space.World)
            {
                operationalPosition = target;
            }
            else if (space == Space.Self)
            {
                if (link == Link.Offset)
                {
                    if (!follow)
                    {
                        operationalPosition = target;
                    }
                    else
                    {
                        operationalPosition = transition.MoveTowards(operationalPosition, target);
                    }
                }
                else if (link == Link.Match)
                {
                    if (_ETERNAL.I.counter)
                    {
                        operationalPosition = target;
                    }
                }
            }
            if (_ETERNAL.I.counter)
            {
                RecordParent();
            }
        }
    }
    public override Vector3 GetTarget()
    {
        Vector3 target = Vector3.zero;

        if (space == Space.World)
        {
            target = value;
        }
        else if (space == Space.Self)
        {
            if (link == Link.Offset)
            {
                if (factorScale)
                {
                    target = Linking.TransformPoint(value * offsetScale, parentPos, parentRot, parentScale); //WORKS!
                }
                else
                {
                    target = Linking.TransformPoint(value, parentPos, parentRot);
                }

                target = offset.ApplyPosition(this, target);
            }
            else if (link == Link.Match)
            {
                Vector3 newTarget;

                //if (!editorApply) // (Cannot change position while applying to parent) {
                SetPrevious();
                //}

                if (factorScale)
                {
                    newTarget = Linking.TransformPoint(previous * offsetScale, parent.position, parent.rotation, parent.localScale);
                }
                else
                {
                    newTarget = Linking.TransformPoint(previousDirection, parent.position, parent.rotation); //++++++++ ATTENTION
                }

                target = newTarget;
            }
        }

        return target;
    }

    public override void TargetToCurrent()
    {
        if (space == Space.Self)
        {
            if (link == Link.Offset)
            {
                position = operationalPosition;
            }
            else if (link == Link.Match)
            {
                //already set!!!
                //++++++++++++++++++++++MAKE A DEBUG.LOG or EXCEPTION
            }
        }
        else if (space == Space.World)
        {
            value = operationalPosition;
        }
    }

    public override void RecordParent()
    {
        parentPos = parent.position;
        parentRot = parent.rotation;
        parentScale = parent.localScale;
    }

    public Vector3 Translate(Vector3 translation, Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self)
        {
            if (factorScale)
            {
                return operationalPosition + (Linking.TransformPoint(translation * offsetScale, parentPos, parentRot, parentScale) - parentPos); //WORKS!
            }
            else
            {
                return Linking.TransformPoint(operationalPosition + translation, parentPos, parentRot); //WORKS!
            }
        }
        else
        {
            return operationalPosition + translation; //WORKS!
        }
    }
    public Vector3 Translate(Vector3 from, Vector3 translation, Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self)
        {
            if (factorScale)
            {
                return from + (Linking.TransformPoint(translation * offsetScale, parentPos, parentRot, parentScale) - parent.position); //WORKS!
            }
            else
            {
                //return Vectors.DivideVector3(Linking.TransformPoint(from + translation, parentPos, parentRot, parentScale), parentScale); //WORKS!
                return Linking.TransformPoint(from + translation, parentPos, parentRot);
            }
        }
        else
        {
            return from + translation; //WORKS!
        }
    }

    public Vector3 SetPosition(Vector3 position, Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self)
        {
            if (factorScale)
            {
                return Linking.TransformPoint(position * offsetScale, parentPos, parentRot, parentScale); //WORKS!
            }
            else
            {
                return Linking.TransformPoint(position, parentPos, parentRot); //WORKS!
            }
        }
        else
        {
            return position; //WORKS!
        }
    } //returns world
    public Vector3 SetPositionLocal(Vector3 position, Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self)
        {
            return position;
        }
        else
        {
            if (factorScale)
            {
                return Linking.InverseTransformPoint(position, parentPos, parentRot, parentScale/* * offsetScale*/) / offsetScale; //WORKS!
            }
            else
            {
                return Vectors.DivideVector3(Linking.InverseTransformPoint(position, parentPos, parentRot, parentScale), parentScale); //WORKS!
            }
        }
    } //returns self
    public Vector3 GetPosition(Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.Self)
        {
            if (factorScale)
            {
                if (offsetScale != 0f) //ALL WORKS!
                {
                    return Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale) / offsetScale;
                }
                else
                {
                    return Vector3.zero;
                }
            }
            else
            {
                return Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot); //WORKS
            }
        }
        else
        {
            return operationalPosition; //WORKS!
        }
    }

    public Vector3 SetPositionRaw(Vector3 position, Space relativeTo = Space.Self)
    {
        //return offset.ApplyPosition(this, SetPosition(position, relativeTo));
        return SetPosition(position, relativeTo);
    }
    public Vector3 SetPositionRawLocal(Vector3 position, Space relativeTo = Space.Self)
    {
        //return SetPositionLocal(offset.ApplyPosition(this, SetPosition(SetPositionLocal(position, relativeTo), Space.Self)), Space.World);
        return SetPositionLocal(SetPosition(SetPositionLocal(position, relativeTo), Space.Self), Space.World);
    }
    public Vector3 GetPositionRaw(Space relativeTo = Space.Self)
    {
        if (space == Space.Self && link == Link.Offset)
        {
            if (relativeTo == Space.Self)
            {
                return SetPosition(offset.ReversePosition(this, target/*SetPosition(GetPosition(relativeTo), relativeTo)*/), Space.Self);
            }
            else // relative to world
            {
                return offset.ReversePosition(this, target);
            }
        }
        else
        {
            if (space == Space.Self)
            {
                //return GetPosition(relativeTo);
                return SetPositionLocal(target, Space.World);
            }
            else // relative to world
            {
                return SetPosition(target, Space.World);
            }
        }
    }

    public override void SetPrevious() //WORKS!
    {
        if (factorScale)
        {
            if (offsetScale != 0f)
            {
                previous = Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale) / offsetScale;

                previousDirection = Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot) / offsetScale;
            }
            else { previous = Vector3.zero; }
        }
        else
        {
            previous = Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot, parentScale);

            previousDirection = Linking.InverseTransformPoint(operationalPosition, parentPos, parentRot);
        }
    }

    //inspector methods
    public override void Switch(Space newSpace, Link newLink)
    {
        Vector3 originalPositon = position;
        Vector3 originalLocalPosition = localPosition;

        if (space == Space.World)
        {
            if (newSpace == Space.Self)
            {
                if (newLink == Link.Offset) //world > offset
                {
                    space = Space.Self;
                    link = Link.Offset;

                    //auto keep offset
                    if (factorScale) //factor scale
                    {
                        SetToTarget();

                        Vector3 from = Linking.InverseTransformPoint(position, parent.position, parent.rotation, parent.localScale * offsetScale);
                        Vector3 to = Linking.InverseTransformPoint(originalPositon, parent.position, parent.rotation, parent.localScale * offsetScale);

                        value += to - from;
                    }
                    else //dont factor scale
                    {
                        SetToTarget();

                        Vector3 from = Linking.InverseTransformPoint(position, parent.position, parent.rotation);
                        Vector3 to = Linking.InverseTransformPoint(originalPositon, parent.position, parent.rotation);

                        value += to - from;
                    }
                }
                else if (newLink == Link.Match) //world > match
                {
                    space = Space.Self;
                    link = Link.Match;
                }
            }
        }
        else if (space == Space.Self)
        {
            if (link == Link.Offset)
            {
                if (newSpace == Space.World) //offset > world
                {
                    space = Space.World;
                    position = originalPositon;
                }
                else
                {
                    if (newLink == Link.Match) //offset > match
                    {
                        link = Link.Match;
                    }
                }
            }
            else if (link == Link.Match)
            {
                if (newSpace == Space.World) //match > world
                {
                    space = Space.World;
                    position = originalPositon;
                }
                else
                {
                    if (newLink == Link.Offset) //match > offset
                    {
                        link = Link.Offset;

                        //auto keep offset
                        if (factorScale) //factor scale
                        {
                            SetToTarget();

                            Vector3 from = Linking.InverseTransformPoint(position, parent.position, parent.rotation, parent.localScale * offsetScale);
                            Vector3 to = Linking.InverseTransformPoint(originalPositon, parent.position, parent.rotation, parent.localScale * offsetScale);

                            value += to - from;
                        }
                        else //dont factor scale
                        {
                            SetToTarget();

                            Vector3 from = Linking.InverseTransformPoint(position, parent.position, parent.rotation);
                            Vector3 to = Linking.InverseTransformPoint(originalPositon, parent.position, parent.rotation);

                            value += to - from;
                        }
                    }
                }
            }
        }
    }
    public override void SwitchParent(Transform newParent)
    {
        if (newParent != null) {
            if (space == Space.Self)
            {
                Vector3 originalPosition = position;
                Vector3 originalLocalPosition = localPosition;

                if (link == Link.Offset)
                {
                    parent = newParent;

                    position = offset.ReversePosition(this, originalPosition);

                }
                else if (link == Link.Match)
                {
                    parent = newParent;

                    position = originalPosition;
                }
            }
        }
    }
    public void SwitchFactorScale (bool factor)
    {
        if (space == Space.Self)
        {
            Vector3 originalPos = position;

            factorScale = true;

            position = originalPos;
        }
    }
    public void ApplyOffsetScale(float newScale = 1f)
    {
        if (space == Space.Self && factorScale)
        {
            Vector3 originalPos = position;

            offsetScale = newScale;

            position = originalPos;
        }
    }
    public override void RemoveOffset()
    {
        if (space == Space.Self && link == Link.Offset)
        {
            position = offset.ApplyPosition(this, position);
        }

        offset = new AxisOrder(null, offset.variety, offset.space);
    }

    //events

    private void Start() { }


#if UNITY_EDITOR
    [CustomEditor(typeof(CustomPosition))]
    public class E : EditorPRO<CustomPosition>
    {
        //method parameters
        private Space P_Switch_Space;
        private Link P_Switch_Link;

        private Transform P_SwitchParent_Parent;

        private LinkSpace P_SetContext_Type;
        private Vector3 P_SetContext_New;

        private bool P_SwitchFactorScale_Factor;

        private float P_ApplyOffsetScale_NewScale = 1f;

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

                EditorGUILayout.LabelField("Position", EditorStyles.boldLabel);
                if (target.space == Space.Self)
                {
                    target.parent = (Transform)EditorGUILayout.ObjectField("Parent", target.parent, typeof(Transform), true);
                }
                if (!(target.space == Space.Self && target.link == Link.Match))
                {

                    //target.value = EditorGUILayout.Vector3Field("Value", target.value);
                    EditorGUILayout.PropertyField(FindProperty("value"));
                }

                EditorGUILayout.Space();

                //Local
                if (target.space == Space.Self)
                {
                    EditorGUILayout.LabelField("Local", EditorStyles.boldLabel);

                    EditorGUILayout.PropertyField(FindProperty("link"));

                    if (target.link == Link.Offset)
                    {
                        EditorGUILayout.PropertyField(FindProperty("offset"));
                    }

                    target.factorScale = EditorGUILayout.Toggle("Factor Scale?", target.factorScale);

                    DisableGroup(target.factorScale, () =>
                    {
                        target.offsetScale = EditorGUILayout.FloatField("Offset Scale", target.offsetScale);
                    });

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

                    EditorGUILayout.Vector3Field("Position", target.position);
                    if (target.space == Space.Self) {
                        EditorGUILayout.Vector3Field("Local Position", target.localPosition);
                    }

                    if (target.space == Space.Self && target.link == Link.Offset)
                    {
                        EditorGUILayout.Space();

                        EditorGUILayout.Vector3Field("Position Raw", target.positionRaw);
                        if (target.space == Space.Self)
                        {
                            EditorGUILayout.Vector3Field("Local Position Raw", target.localPositionRaw);
                        }
                    }

                    GUI.enabled = true;

                    EditorGUILayout.Space();
                }

                //Editor methods!

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
                            Undo.RecordObject(target.gameObject, "Re-Oriented CustomPosition");

                            target.TargetToCurrent();
                        }

                        Function("Switch", () =>
                        {
                            target.Switch(P_Switch_Space, P_Switch_Link);
                        },
                        new Action[] {
                                    () => P_Switch_Space = (Space)EditorGUILayout.EnumPopup(GUIContent.none, P_Switch_Space),
                                    () => P_Switch_Link = (Link)EditorGUILayout.EnumPopup(GUIContent.none, P_Switch_Link)
                        }, "Switched CustomPosition Space and/or Link", target.gameObject);

                        Function("Switch Parent", () =>
                        {
                            target.Switch(P_Switch_Space, P_Switch_Link);
                        },
                        new Action[] {
                                    () => P_SwitchParent_Parent = (Transform)EditorGUILayout.ObjectField(GUIContent.none, P_SwitchParent_Parent, typeof(Transform), true)
                        }, "Switched CustomPosition Parent", target.gameObject);

                        Function("Switch Factor Scale", () =>
                        {
                            target.SwitchFactorScale(P_SwitchFactorScale_Factor);
                        },
                        new Action[] {
                                    () => P_SwitchFactorScale_Factor = EditorGUILayout.Toggle(GUIContent.none, P_SwitchFactorScale_Factor)
                        }, "Switched CustomPosition FactorScale", target.gameObject);

                        Function("Apply Offset Scale", () =>
                        {
                            target.ApplyOffsetScale(P_ApplyOffsetScale_NewScale);
                        },
                        new Action[] {
                                () => P_ApplyOffsetScale_NewScale = EditorGUILayout.FloatField(GUIContent.none, P_ApplyOffsetScale_NewScale)
                        }, "Applied CustomPosition OffsetScale", target.gameObject);

                        Function("Remove Offset", () =>
                        {
                            if (EditorUtility.DisplayDialog(
                                "Remove Offset?",
                                "Are you sure you want to remove the offset of \"CustomPosition?\"",
                                "Yes", "Cancel"))
                            {
                                Undo.RecordObject(target.gameObject, "Removed CustomPosition Offset");

                                target.RemoveOffset();
                            }
                        },
                        new Action[] { });

                        GUI.enabled = true;

                        if (!target.applyInEditor)
                        {
                            GUI.enabled = false;
                        }

                        Function("Set Property", () =>
                        {
                            if (P_SetContext_Type == LinkSpace.World)
                            {
                                target.position = P_SetContext_New;
                            }
                            else if (P_SetContext_Type == LinkSpace.Self)
                            {
                                target.localPosition = P_SetContext_New;
                            }
                            else if (P_SetContext_Type == LinkSpace.WorldRaw)
                            {
                                target.positionRaw = P_SetContext_New;
                            }
                            else if (P_SetContext_Type == LinkSpace.SelfRaw)
                            {
                                target.localPositionRaw = P_SetContext_New;
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
                                            P_SetContext_New = target.position;
                                        }
                                        else if (P_SetContext_Type == LinkSpace.Self)
                                        {
                                            P_SetContext_New = target.localPosition;
                                        }
                                        else if (P_SetContext_Type == LinkSpace.WorldRaw)
                                        {
                                            P_SetContext_New = target.positionRaw;
                                        }
                                        else if (P_SetContext_Type == LinkSpace.SelfRaw)
                                        {
                                            P_SetContext_New = target.localPositionRaw;
                                            }
                                        }
                                    },
                                    () => P_SetContext_New = EditorGUILayout.Vector3Field(GUIContent.none, P_SetContext_New)
                            }, "Changed Context Value of CustomPosition", target.gameObject);

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
#endif
}

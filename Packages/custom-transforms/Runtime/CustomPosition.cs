using System;
using UnityEngine;

using REXTools.REXCore;
using REXTools.CustomTransforms;
using REXTools.TransformTools;

namespace REXTools.CustomTransforms
{
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
                }
                else
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
                }
                else
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
                    return Linking.InverseTransformPoint(position, parentPos, parentRot, parentScale).Divide(parentScale); //WORKS!
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
            if (newParent != null)
            {
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
        public void SwitchFactorScale(bool factor)
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


        //context menu methods
#if UNITY_EDITOR
        //[ContextMenu("Open Handles Window")]
        //private void OpenHandlesWindow ()
        //{
        //    CustomTransformHandlesWindow.ShowWindow();
        //}

        public static void SetCheckedEnumMenuItems()
        {
            EditorTools.CustomEditors.EnumMenuItem("REX Custom Transforms/Custom Position/Self Space Handle/Self", LinkSpaceRotation.Self, ref CustomPositionEditor.selfHandleRot);
            EditorTools.CustomEditors.EnumMenuItem("REX Custom Transforms/Custom Position/Self Space Handle/Parent", LinkSpaceRotation.Parent, ref CustomPositionEditor.selfHandleRot);
            EditorTools.CustomEditors.EnumMenuItem("REX Custom Transforms/Custom Position/Self Space Handle/World", LinkSpaceRotation.World, ref CustomPositionEditor.selfHandleRot);

            EditorTools.CustomEditors.EnumMenuItem("REX Custom Transforms/Custom Position/World Space Handle/Self", Space.Self, ref CustomPositionEditor.worldHandleRot);
            EditorTools.CustomEditors.EnumMenuItem("REX Custom Transforms/Custom Position/World Space Handle/World", Space.World, ref CustomPositionEditor.worldHandleRot);
        }

        [UnityEditor.MenuItem("REX Custom Transforms/Custom Position/Self Space Handle/Self")]
        private static void SelfHandleRotation_Self()
        {
            CustomPositionEditor.selfHandleRot = LinkSpaceRotation.Self;

            SetCheckedEnumMenuItems();
        }
        [UnityEditor.MenuItem("REX Custom Transforms/Custom Position/Self Space Handle/Parent")]
        private static void SelfHandleRotation_Parent()
        {
            CustomPositionEditor.selfHandleRot = LinkSpaceRotation.Parent;

            SetCheckedEnumMenuItems();
        }
        [UnityEditor.MenuItem("REX Custom Transforms/Custom Position/Self Space Handle/World")]
        private static void SelfHandleRotation_World()
        {
            CustomPositionEditor.selfHandleRot = LinkSpaceRotation.World;

            SetCheckedEnumMenuItems();
        }

        [UnityEditor.MenuItem("REX Custom Transforms/Custom Position/Self Space Handle/Self", true)]
        [UnityEditor.MenuItem("REX Custom Transforms/Custom Position/Self Space Handle/Parent", true)]
        [UnityEditor.MenuItem("REX Custom Transforms/Custom Position/Self Space Handle/World", true)]
        private static bool SelfHandleRotation_Valid()
        {
            return CustomTransformHandlesWindow.activeType == typeof(CustomPosition) && ((CustomPosition)CustomTransformHandlesWindow.activeCustomTransform).space == Space.Self;
        }



        [UnityEditor.MenuItem("REX Custom Transforms/Custom Position/World Space Handle/Self")]
        private static void WorldHandleRotation_Self()
        {
            CustomPositionEditor.worldHandleRot = Space.Self;

            SetCheckedEnumMenuItems();
        }
        [UnityEditor.MenuItem("REX Custom Transforms/Custom Position/World Space Handle/World")]
        private static void WorldHandleRotation_Parent()
        {
            CustomPositionEditor.worldHandleRot = Space.World;

            SetCheckedEnumMenuItems();
        }

        [UnityEditor.MenuItem("REX Custom Transforms/Custom Position/World Space Handle/Self", true)]
        [UnityEditor.MenuItem("REX Custom Transforms/Custom Position/World Space Handle/World", true)]
        private static bool WorldHandleRotation_Valid()
        {
            return CustomTransformHandlesWindow.activeType == typeof(CustomPosition) && ((CustomPosition)CustomTransformHandlesWindow.activeCustomTransform).space == Space.World;
        }
#endif

        //events

        private void Start() { }


        //Startup
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoad]
        public static class Startup
        {
            static Startup()
            {
                SetCheckedEnumMenuItems();
            }
        }
#endif
    }
}
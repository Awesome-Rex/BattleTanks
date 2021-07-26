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
        { //world position
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
        { //world position
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
        { //world position without transition delay
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
        { //local position without transition delay
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

        public bool factorScale = true; //factor scaling
        public float offsetScale = 1f; //offset scaling

        private Vector3 previousDirection;

        private Vector3 parentPos;
        private Quaternion parentRot; //USE THE STUFF HERE 
        private Vector3 parentScale;

        private Vector3 operationalPosition
        { //current position to use
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
        { //set to target without transition
            //Debug.Log("Position - " + value);

            target = GetTarget();

            if (enabled)
            {
                operationalPosition = target;

                RecordParent();
            }
        }
        public override void MoveToTarget()
        { //move to target with transition
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
        { //get target position
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
                        newTarget = Linking.TransformPoint(previous * offsetScale, parent.position, parent.rotation, parent.scale);
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
            if (parent != null)
            {
                parentPos = parent.position;
                parentRot = parent.rotation;
                parentScale = parent.scale;
            }
            else //TESTING
            {
                parentPos = Vector3.zero;
                parentRot = Quaternion.identity;
                parentScale = Vector3.one;
            }
        }

        public Vector3 Translate(Vector3 translation, Space relativeTo = Space.Self)
        { //transform translate
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
        { //transform translate
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
        { //sets position and returns world position
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
        { //sets position and returns local position
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
        { //gets position with specified space
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
        { //sets position and returns world position
            //return offset.ApplyPosition(this, SetPosition(position, relativeTo));
            return SetPosition(position, relativeTo);
        }
        public Vector3 SetPositionRawLocal(Vector3 position, Space relativeTo = Space.Self)
        { //sets position and returns local position
            //return SetPositionLocal(offset.ApplyPosition(this, SetPosition(SetPositionLocal(position, relativeTo), Space.Self)), Space.World);
            return SetPositionLocal(SetPosition(SetPositionLocal(position, relativeTo), Space.Self), Space.World);
        }
        public Vector3 GetPositionRaw(Space relativeTo = Space.Self)
        { //gets raw position with specified space
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

        public override void SetPrevious() //Only called during Link = Match
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

        public override void SetParent(TransformObject parent, bool worldPositionStays = false)
        {
            if (!worldPositionStays)
            {
                if (parent != null)
                {
                    //if (!_parent.isNull)
                    //{
                    //    this._parent = parent;
                    //}
                    //else
                    //{
                    //    this._parent = null;
                    //}
                    this._parent = parent;

                    RecordParent(); //TESTING
                    if (space == Space.Self && link == Link.Match) //TESTING
                    {
                        SetPrevious(); //TESTING
                    }
                }
                else
                {
                    this._parent = null;
                }
            }
            else if (worldPositionStays)
            {
                if (parent != null && !_parent.isNull)
                {
                    if (space == Space.Self)
                    {
                        Vector3 originalPosition = position;
                        Vector3 originalLocalPosition = localPosition;

                        if (link == Link.Offset)
                        {
                            SetParent(parent);

                            position = offset.ReversePosition(this, originalPosition);
                        }
                        else if (link == Link.Match)
                        {
                            SetParent(parent);

                            position = originalPosition;
                        }
                    }
                }
            }
        }

        //inspector methods
        public override void Switch(Space newSpace, Link newLink)
        { //switch spaces and link
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

                            Vector3 from = Linking.InverseTransformPoint(position, parent.position, parent.rotation, parent.scale * offsetScale);
                            Vector3 to = Linking.InverseTransformPoint(originalPositon, parent.position, parent.rotation, parent.scale * offsetScale);

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

                                Vector3 from = Linking.InverseTransformPoint(position, parent.position, parent.rotation, parent.scale * offsetScale);
                                Vector3 to = Linking.InverseTransformPoint(originalPositon, parent.position, parent.rotation, parent.scale * offsetScale);

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
        public void SwitchFactorScale(bool factor)
        { //switch factor scale
            if (space == Space.Self)
            {
                Vector3 originalPos = position;

                factorScale = true;

                position = originalPos;
            }
        }
        public void ApplyOffsetScale(float newScale = 1f)
        { //switches factor scale and moves current position to match previous
            if (space == Space.Self && factorScale)
            {
                Vector3 originalPos = position;

                offsetScale = newScale;

                position = originalPos;
            }
        }
        public override void RemoveOffset()
        { //removes offset and moves current position ot match previous
            if (space == Space.Self && link == Link.Offset)
            {
                position = offset.ApplyPosition(this, position);
            }

            offset = new AxisOrder(null, offset.variety, offset.space);
        }
        
        //events

        private void Start() { }
    }
}
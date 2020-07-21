using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.REXCore;
using REXTools.TransformTools;

namespace REXTools.CustomTransforms
{
    [RequireComponent(typeof(Rigidbody))]
    public class CustomGravity : CustomTransform<Vector3>
    {
        public float gravity = 9.81f;
        public float gravityScale = 1f;

        public AxisOrder offset = new AxisOrder(null, SpaceVariety.Mixed);  //local

        public Vector3 direction
        {
            get
            {
                return GetDirection(Space.World);
            }
            set
            {
                this.value = SetDirectionLocal(value, Space.World);
            }
        }
        public Vector3 localDirection
        {
            get
            {
                if (space == Space.Self)
                {
                    return GetDirection(Space.Self);
                }
                else
                {
                    return direction;
                }
            }
            set
            {
                if (space == Space.Self)
                {
                    this.value = SetDirectionLocal(value, Space.Self);
                }
                else
                {
                    direction = value;
                }
            }
        }

        public Vector3 directionRaw
        {
            get
            {
                if (space == Space.Self)
                {
                    return GetDirectionRaw(Space.World);
                }
                else
                {
                    return direction;
                }
            }
            set
            {
                if (space == Space.Self)
                {
                    this.value = SetDirectionRawLocal(value, Space.World);
                }
                else
                {
                    direction = value;
                }
            }
        }
        public Vector3 localDirectionRaw
        {
            get
            {
                if (space == Space.Self)
                {
                    return GetDirectionRaw(Space.Self);
                }
                else
                {
                    return localDirection;
                }
            }
            set
            {
                if (space == Space.Self)
                {
                    this.value = SetDirectionRawLocal(value, Space.Self);
                }
                else
                {
                    localDirection = value;
                }
            }
        }

        public Vector3 velocity
        {
            get
            {
                return GetVelocity(Space.World);
            }
            set
            {
                rigidbody.velocity = SetVelocity(value, Space.World);
            }
        }
        public Vector3 localVelocity
        {
            get
            {
                if (space == Space.Self)
                {
                    return GetVelocity(Space.Self);
                }
                else
                {
                    return velocity;
                }
            }
            set
            {
                if (space == Space.Self)
                {
                    rigidbody.velocity = SetVelocity(value, Space.Self);
                }
                else
                {
                    velocity = value;
                }
            }
        }

        private Vector3 operationalDirection //operational
        {
            get
            {
                if (space == Space.Self)
                {
                    return (offset.ApplyRotation(parent.rotation) * value);
                }
                else
                {
                    return (offset.ApplyRotation(Quaternion.LookRotation(value)) * Vector3.forward);
                }
            }
        }

        //previous
        private Vector3 parentPos;
        private Quaternion parentRot;

        //script reference
        public new Rigidbody rigidbody;

        public void Apply()
        {
            if (enabled)
            {
                //Rotate velocity
                rigidbody.velocity = GetTarget();

                if (space == Space.World)
                {
                    rigidbody.AddForce(operationalDirection.normalized * gravity * gravityScale, ForceMode.Acceleration);
                }
                else if (space == Space.Self)
                {
                    rigidbody.AddForce(operationalDirection.normalized * gravity * gravityScale, ForceMode.Acceleration);
                }
            }
        }

        public override Vector3 GetTarget()
        {
            Vector3 target = Vector3.zero;

            if (space == Space.World)
            {
                target = rigidbody.velocity;
            }
            else if (space == Space.Self)
            {
                target = (parent.TransformPoint(Linking.InverseTransformPoint(parentPos + rigidbody.velocity, parentPos, parentRot)) - parent.position).normalized * rigidbody.velocity.magnitude;
            }

            return target;
        }

        public Vector3 GetDirection(Space relativeTo)
        {
            if (relativeTo == Space.Self) // self
            {
                return parent.InverseTransformPoint(parent.position + operationalDirection);
            }
            else // world
            {
                return operationalDirection;
            }
        }
        public Vector3 SetDirectionLocal(Vector3 direction, Space relativeTo) //SHOULD work
        {
            if (relativeTo == Space.Self)
            {
                return offset.ReverseRotation(Quaternion.LookRotation(direction)) * Vector3.forward;
            }
            else
            {
                return offset.ReverseRotation(Quaternion.LookRotation(parent.InverseTransformPoint(parent.position + direction))) * Vector3.forward;
            }
        }

        public Vector3 GetDirectionRaw(Space relativeTo)
        {
            if (relativeTo == Space.Self) // self
            {
                //return parent.InverseTransformPoint(parent.position + operationalDirection);
                return parent.InverseTransformPoint(parent.position + (offset.ReverseRotation(Quaternion.LookRotation(operationalDirection)) * Vector3.forward));
            }
            else // world
            {
                //return operationalDirection;
                return offset.ReverseRotation(Quaternion.LookRotation(operationalDirection)) * Vector3.forward;
            }
        }
        public Vector3 SetDirectionRawLocal(Vector3 direction, Space relativeTo) //SHOULD work
        {
            if (relativeTo == Space.Self)
            {
                return /*offset.ReverseRotation(Quaternion.LookRotation(*/direction/*)) * Vector3.forward*/;
            }
            else
            {
                return /*offset.ReverseRotation(Quaternion.LookRotation(*/parent.InverseTransformPoint(parent.position + direction)/*)) * Vector3.forward*/;
            }
        }

        public Vector3 GetVelocity(Space relativeTo)
        {
            if (relativeTo == Space.Self)
            {

                return (parent.InverseTransformPoint(parent.position + rigidbody.velocity));

            }
            else // world
            {
                return rigidbody.velocity;
            }
        }
        public Vector3 SetVelocity(Vector3 velocity, Space relativeTo)
        {
            if (relativeTo == Space.Self)
            {
                return parent.TransformPoint(parent.position + velocity);
            }
            else // world
            {
                return velocity;
            }
        }

        public override void Switch(Space newSpace, Link newLink) //WORKS!
        {
            Vector3 originalDirection = direction;
            Vector3 originalLocalDirection = localDirection;

            if (space == Space.World)
            {
                if (newSpace == Space.Self) //world > self
                {
                    space = Space.Self;

                    //auto keep offset
                    value = offset.ReverseRotation(
                        Linking.InverseTransformEuler(Quaternion.LookRotation(originalDirection), parent.rotation)) * Vector3.forward;
                }
            }
            else if (space == Space.Self)
            {
                if (newSpace == Space.World) //self > world
                {
                    space = Space.World;

                    value = direction;
                }
            }
        }
        public override void SwitchParent(Transform newParent)
        {
            if (space == Space.Self)
            {
                Vector3 originalDirection = direction;
                Vector3 originalLocalDirection = localDirection;

                //NO LINKS (compared to Position and Rotation)
                parent = newParent;

                direction = originalDirection; //Should automatically FACTOR OFFSET, no need for offset.Reverse
            }
        }
        public void RemoveOffset()
        {
            if (space == Space.Self)
            {
                direction = offset.ApplyRotation(Quaternion.LookRotation(direction)) * Vector3.forward;
            }

            offset = new AxisOrder(null, offset.variety, offset.space);
        }

        public override void SetPrevious()
        {
            previous = parent.InverseTransformPoint(parent.position + rigidbody.velocity);

            parentPos = parent.position;
            parentRot = parent.rotation;
        }

        public void Enable(bool enabled)
        {
            if (enabled)
            {
                //disable rigidhodies, add custom behaviour

                _ETERNAL.I.earlyRecorder.lateCallbackF += Apply;
                _ETERNAL.I.lateRecorder.callbackF += SetPrevious;

                rigidbody.useGravity = false;

                this.enabled = true;
            }
            else
            {
                //enable rigidbodies, disable this script

                _ETERNAL.I.earlyRecorder.lateCallbackF -= Apply;
                _ETERNAL.I.lateRecorder.callbackF -= SetPrevious;

                rigidbody.useGravity = true;

                this.enabled = false;
            }
        }

        protected override void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();

            base.Awake();
        }

        void Start() { }

        private void OnEnable()
        {
            Enable(true);
        }
        private void OnDisable()
        {
            Enable(false);
        }

        protected override void OnDestroy()
        {
            Enable(false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (operationalDirection.normalized * 2f));

            Gizmos.color = new Color(1f, 0.5f, 0f);
            Gizmos.DrawLine(transform.position, transform.position + (localDirection.normalized * 2f));

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + (direction.normalized * 2f));
        }
    }
}
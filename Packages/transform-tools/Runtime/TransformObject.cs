using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.TransformTools
{
    [System.Serializable]
    public class TransformObject
    {
        public enum ReferenceType { Transform, Constant, Function }

        public ReferenceType referenceType = ReferenceType.Transform;

        public bool isNull
        {
            get
            {
                if (referenceType == ReferenceType.Transform)
                {
                    return transform == null;
                }
                else if (referenceType == ReferenceType.Constant)
                {
                    return false;
                }
                else if (referenceType == ReferenceType.Function)
                {
                    return _positionFunction == default || _rotationFunction == default || _scaleFunction == default;
                }

                return default;
            }
        }

        public virtual Vector3 position
        {
            get
            {
                if (referenceType == ReferenceType.Transform)
                {
                    return transform.position;
                }
                else if (referenceType == ReferenceType.Constant)
                {
                    return _positionConstant;
                }
                else if (referenceType == ReferenceType.Function)
                {
                    return _positionFunction();
                }

                return default;
            }
            set
            {
                if (referenceType == ReferenceType.Transform)
                {
                    transform.position = value;
                }
                else if (referenceType == ReferenceType.Constant)
                {
                    _positionConstant = value;
                }
                else if (referenceType == ReferenceType.Function)
                {
                    _positionFunction = () => value;
                }
            }
        }
        public virtual Quaternion rotation
        {
            get
            {
                if (referenceType == ReferenceType.Transform)
                {
                    return transform.rotation;
                }
                else if (referenceType == ReferenceType.Constant)
                {
                    return _rotationConstant;
                }
                else if (referenceType == ReferenceType.Function)
                {
                    return _rotationFunction();
                }

                return default;
            }
            set
            {
                if (referenceType == ReferenceType.Transform)
                {
                    transform.rotation = value;
                }
                else if (referenceType == ReferenceType.Constant)
                {
                    _rotationConstant = value;
                }
                else if (referenceType == ReferenceType.Function)
                {
                    _rotationFunction = () => value;
                }
            }
        }
        public virtual Vector3 scale
        {
            get
            {
                if (referenceType == ReferenceType.Transform)
                {
                    return transform.localScale;
                }
                else if (referenceType == ReferenceType.Constant)
                {
                    return _scaleConstant;
                }
                else if (referenceType == ReferenceType.Function)
                {
                    return _scaleFunction();
                }

                return default;
            }
            set
            {
                if (referenceType == ReferenceType.Transform)
                {
                    transform.localScale = value;
                }
                else if (referenceType == ReferenceType.Constant)
                {
                    _scaleConstant = value;
                }
                else if (referenceType == ReferenceType.Function)
                {
                    _scaleFunction = () => value;
                }
            }
        }

        public Transform transform;

        public Vector3 _positionConstant;
        public Quaternion _rotationConstant;
        public Vector3 _scaleConstant;

        public System.Func<Vector3> _positionFunction;
        public System.Func<Quaternion> _rotationFunction;
        public System.Func<Vector3> _scaleFunction;
    }
}
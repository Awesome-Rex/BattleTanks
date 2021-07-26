using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.TransformTools
{
    [System.Serializable]
    public class TransformObject
    {
        public enum ReferenceType { Transform, Constant, Function }
        
        public ReferenceType referenceType
        {
            get
            {
                return _referenceType;
            }
        }
        [SerializeField] private ReferenceType _referenceType = ReferenceType.Transform;


        public bool isNull
        {
            get
            {
                if (_referenceType == ReferenceType.Transform)
                {
                    return transform == null;
                }
                else if (_referenceType == ReferenceType.Constant)
                {
                    return false;
                }
                else if (_referenceType == ReferenceType.Function)
                { //return _positionFunction == default || _rotationFunction == default || _scaleFunction == default;
                    try
                    {
                        Vector3 position = _positionFunction();
                        Quaternion rotation = _rotationFunction();
                        Vector3 scale = _scaleFunction();
                    }
                    catch// (System.Exception e)
                    {
                        return true;
                    }

                    return false;
                }

                return default;
            }
        }

        public virtual Vector3 position
        {
            get
            {
                if (!isNull)
                {
                    if (_referenceType == ReferenceType.Transform)
                    {
                        return transform.position;
                    }
                    else if (_referenceType == ReferenceType.Constant)
                    {
                        return _positionConstant;
                    }
                    else if (_referenceType == ReferenceType.Function)
                    {
                        return _positionFunction();
                    }

                    return default;
                }
                else
                {
                    throw new System.NullReferenceException();
                }
            }
            set
            {
                if (_referenceType == ReferenceType.Transform)
                {
                    transform.position = value;
                }
                else if (_referenceType == ReferenceType.Constant)
                {
                    _positionConstant = value;
                }
                //else if (referenceType == ReferenceType.Function)
                //{
                //    _positionFunction = () => value;
                //}
            }
        }
        public virtual Quaternion rotation
        {
            get
            {
                if (!isNull)
                {
                    if (_referenceType == ReferenceType.Transform)
                    {
                        return transform.rotation;
                    }
                    else if (_referenceType == ReferenceType.Constant)
                    {
                        return _rotationConstant;
                    }
                    else if (_referenceType == ReferenceType.Function)
                    {
                        return _rotationFunction();
                    }

                    return default;
                }
                else
                {
                    throw new System.NotImplementedException();
                }
            }
            set
            {
                if (_referenceType == ReferenceType.Transform)
                {
                    transform.rotation = value;
                }
                else if (_referenceType == ReferenceType.Constant)
                {
                    _rotationConstant = value;
                }
                //else if (referenceType == ReferenceType.Function)
                //{
                //    _rotationFunction = () => value;
                //}
            }
        }
        public virtual Vector3 scale
        {
            get
            {
                if (!isNull)
                {
                    if (_referenceType == ReferenceType.Transform)
                    {
                        return transform.localScale;
                    }
                    else if (_referenceType == ReferenceType.Constant)
                    {
                        return _scaleConstant;
                    }
                    else if (_referenceType == ReferenceType.Function)
                    {
                        return _scaleFunction();
                    }

                    return default;
                }
                else
                {
                    throw new System.NotImplementedException();
                }
            }
            set
            {
                if (_referenceType == ReferenceType.Transform)
                {
                    transform.localScale = value;
                }
                else if (_referenceType == ReferenceType.Constant)
                {
                    _scaleConstant = value;
                }
                //else if (referenceType == ReferenceType.Function)
                //{
                //    _scaleFunction = () => value;
                //}
            }
        }

        public virtual Vector3 eulerAngles
        {
            get
            {
                return rotation.eulerAngles;
            }
            set
            {
                rotation = Quaternion.Euler(value);
            }
        }

        public Vector3 up
        {
            get
            {
                return (rotation * Vector3.up).normalized;
            }
            set
            {
                rotation = (Quaternion.LookRotation(value) * Quaternion.Euler(90f, 0f, 0f));
            }
        }
        public Vector3 forward
        {
            get
            {
                return (rotation * Vector3.forward).normalized;
            }
            set
            {
                rotation = Quaternion.LookRotation(value);
            }
        }
        public Vector3 right
        {
            get
            {
                return (rotation * Vector3.right).normalized;
            }
            set
            {
                rotation = (Quaternion.LookRotation(value) * Quaternion.Euler(0f, -90f, 0f));
            }
        }



        public Transform transform;

        [SerializeField] private Vector3 _positionConstant;
        [SerializeField] private Quaternion _rotationConstant;
        [SerializeField] private Vector3 _scaleConstant;

        [SerializeField] private readonly System.Func<Vector3> _positionFunction;
        [SerializeField] private readonly System.Func<Quaternion> _rotationFunction;
        [SerializeField] private readonly System.Func<Vector3> _scaleFunction;




        public Vector3 TransformPoint(Vector3 position)
        {
            if (!isNull)
            {
                return Linking.TransformPoint(position, this.position, this.rotation, this.scale);
            }

            return position;
        }
        public Vector3 InverseTransformPoint(Vector3 position)
        {
            if (!isNull)
            {
                return Linking.InverseTransformPoint(position, this.position, this.rotation, this.scale);
            }

            return position;
        }

        public Vector3 TransformDirection(Vector3 direction)
        {
            if (!isNull)
            {
                return Linking.TransformDirection(position, this.rotation);
            }

            return direction;
        }
        public Vector3 InverseTransformDirection(Vector3 direction)
        {
            if (isNull)
            {
                return Linking.InverseTransformDirection(position, this.rotation);
            }

            return direction;
        }

        //public Vector3 TransformVector(Vector3 vector)
        //{

        //}
        //public Vector3 InverseTransformVector(Vector3 vector)
        //{

        //}



        public TransformObject()
        {
            this._referenceType = ReferenceType.Transform;

            this.transform = null;
        }
        public TransformObject(Transform transform)
        {
            this._referenceType = ReferenceType.Transform;

            this.transform = transform;
        }


        public TransformObject(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this._referenceType = ReferenceType.Constant;

            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
        public TransformObject(Vector3 position, Quaternion rotation)
        {
            this._referenceType = ReferenceType.Constant;

            this.position = position;
            this.rotation = rotation;
            this.scale = Vector3.one;
        }
        public TransformObject(Vector3 position)
        {
            this._referenceType = ReferenceType.Constant;

            this.position = position;
            this.rotation = Quaternion.Euler(Vector3.zero);
            this.scale = Vector3.one;
        }

        public TransformObject(System.Func<Vector3> position, System.Func<Quaternion> rotation, System.Func<Vector3> scale)
        {
            this._referenceType = ReferenceType.Function;

            this._positionFunction = position;
            this._rotationFunction = rotation;
            this._scaleFunction = scale;
        }
        public TransformObject(System.Func<Vector3> position, System.Func<Quaternion> rotation)
        {
            this._referenceType = ReferenceType.Function;

            this._positionFunction = position;
            this._rotationFunction = rotation;
            this._scaleFunction = () => Vector3.one;
        }
        public TransformObject(System.Func<Vector3> position)
        {
            this._referenceType = ReferenceType.Function;

            this._positionFunction = position;
            this._rotationFunction = () => Quaternion.Euler(Vector3.zero);
            this._scaleFunction = () => Vector3.one;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.TransformTools
{
    [System.Serializable]
    public class TransformObjectConstant : TransformObject
    {
        public Vector3 _position;
        public override Vector3 position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        public Quaternion _rotation;
        public override Quaternion rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
            }
        }

        public Vector3 _scale;
        public override Vector3 scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
            }
        }

        public void SetPosition(Vector3 position)
        {
            this._position = position;
        }
        public void SetRotation(Quaternion rotation)
        {
            this._rotation = rotation;
        }
        public void SetScale(Vector3 scale)
        {
            this._scale = scale;
        }
    }
}
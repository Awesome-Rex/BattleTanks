using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.TransformTools
{
    [System.Serializable]
    public class TransformObjectComponent : TransformObject
    {
        public Transform transform;

        public override Vector3 position
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
        public override Quaternion rotation
        {
            get
            {
                return transform.rotation;
            }
            set
            {
                transform.rotation = value;
            }
        }
        public override Vector3 scale
        {
            get
            {
                return transform.localScale;
            }
            set
            {
                transform.localScale = value;
            }
        }
    }
}
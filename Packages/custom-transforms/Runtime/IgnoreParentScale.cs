using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.CustomTransforms;
using REXTools.TransformTools;

namespace REXTools.CustomTransforms
{
    public class IgnoreParentScale : IgnoreLink
    {
        //private previous'
        private Vector3 parentScale;

        public override void MoveToTarget()
        {
            if (enabled)
            {
                transform.localScale = transform.localScale.Divide(transform.parent.localScale.Divide(parentScale));
            }
        }

        public override void SetPrevious()
        {
            parentScale = transform.parent.localScale;
        }

        private void Start() { }
    }
}
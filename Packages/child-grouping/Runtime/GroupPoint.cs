using REXTools.REXCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.ChildGrouping
{
    public class GroupPoint : MonoBehaviourPRO //referrs back to group and onto points
    {
        public dynamic group;

        public void GetGroup<T>() where T : ChildGroup
        {
            group = GetComponentInParent<T>();
        }
    }
}
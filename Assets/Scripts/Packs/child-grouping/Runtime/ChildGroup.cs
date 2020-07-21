using REXTools.REXCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.ChildGrouping
{
    public class ChildGroup : MonoBehaviourPRO //refers back to group
    {
        public dynamic group;
        public List<dynamic> points;

        public void GetGroup<T>() where T : ChildGroup
        {
            group = GetComponentInParent<T>();
        }

        private dynamic GetComponentInParent<T>() where T : ChildGroup
        {
            throw new System.NotImplementedException();
        }

        public void GetPoints<T>() where T : GroupPoint
        {
            points = new List<dynamic>();

            foreach (T i in GetComponentsInChildren<T>())
            {
                points.Add(i);
                ((GroupPoint)i).group = this;
            }
        }
    }
}
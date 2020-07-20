using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildGrouping : MonoBehaviourPRO //refers back to group
{
    public dynamic group;
    public List<dynamic> points;

    public void GetGroup<T>() where T : ChildGrouping
    {
        group = GetComponentInParent<T>();
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

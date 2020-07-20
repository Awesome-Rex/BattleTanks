using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupPoint : MonoBehaviourPRO //referrs back to group and onto points
{
    public dynamic group;
    
    public void GetGroup<T> () where T : ChildGrouping
    {
        group = GetComponentInParent<T>();
    }
}

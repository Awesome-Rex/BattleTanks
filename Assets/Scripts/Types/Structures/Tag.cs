using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Tag
{
    public string name;

    public int id;

    public List<Tag> tags;
}

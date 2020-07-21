using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace REXTools.REXCore
{
    [System.Serializable]
    public struct Tag
    {
        public string name;

        public int id;

        public List<Tag> tags;
    }
}
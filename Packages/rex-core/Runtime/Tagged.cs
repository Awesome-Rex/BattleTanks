using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.REXCore
{
    public class Tagged : MonoBehaviour
    {
        public List<string> tags = new List<string>();


        public bool HasTag(string tag)
        {
            if (tags.Contains(tag))
            {
                return true;
            }

            return false;
        }

        public void AddTag(string tag, bool removeDoubles)
        {

        }

        public void RemoveTag(string tag)
        {

        }

        public void ClearTags()
        {
            tags = new List<string>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace REXTools.REXCore
{
    public class SceneControl : MonoBehaviour
    {
        private void Awake()
        {
            transform.SetAsFirstSibling();
        }
    }
}
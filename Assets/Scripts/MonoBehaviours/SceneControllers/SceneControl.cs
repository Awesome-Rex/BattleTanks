﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    private void Awake()
    {
        transform.SetAsFirstSibling();

        if (_ETERNAL.I == null)
        {
            GameObject instance = Instantiate(Resources.Load("_ETERNAL") as GameObject);
            instance.transform.SetAsFirstSibling();
        }
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

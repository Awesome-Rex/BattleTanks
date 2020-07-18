using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class CustomTransformEditorExecuter : MonoBehaviour
{
#if UNITY_EDITOR
    void Awake()
    {
        EditorApplication.update += EditorUpdate;
    }

    void EditorUpdate()
    {
        if ((!EditorApplication.isPlaying) || EditorApplication.isPaused)
        {
            //if (!Application.isPlaying) {
                if (GetComponent<CustomPosition>() != null)
                {
                    GetComponent<CustomPosition>().EditorApplyCheck();
                }
                if (GetComponent<CustomRotation>() != null)
                {
                    GetComponent<CustomRotation>().EditorApplyCheck();
                }
            //}
        }
    }

    void OnDestroy()
    {
        EditorApplication.update -= EditorUpdate;
    }
#endif
}

//public class CustomTransformEditorExecuter : Executer
//{
//    private void Update()
//    {
//        if (!Application.isPlaying)
//        {
//            if (GetComponent<CustomPosition>() != null)
//            {
//                GetComponent<CustomPosition>().EditorApplyCheck();
//            }
//            if (GetComponent<CustomRotation>() != null)
//            {
//                GetComponent<CustomPosition>().EditorApplyCheck();
//            }
//        }
//    }
//}

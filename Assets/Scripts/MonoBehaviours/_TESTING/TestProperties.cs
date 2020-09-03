using REXTools.TransformTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProperties : MonoBehaviour
{
    public Vector2Bool vector2Bool;
    public Vector3Bool vector3Bool = Vector3Bool.truthy;
    
    public Vector2 vector2;
    public Vector3 vector3;
    public Vector4 vector4;

    [Space]
    public Color colour;
}

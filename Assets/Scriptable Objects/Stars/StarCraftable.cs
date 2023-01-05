using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Star", menuName = "Star System/Star")]
public class Star : ScriptableObject
{
    public GameObject StarBody;
    public Vector3 start;
    public Vector3 final;
    public AnimationCurve x;
    public AnimationCurve y;
    public AnimationCurve z;
    // put a fixed camera on the position curve calculated
    // and then destroy the object once the animation is completed
    // or the star has reached its final destination
}
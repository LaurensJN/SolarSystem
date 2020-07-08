using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class SolarSystemSettings : MonoBehaviour
{
    [Range(0.0001f, 1)]
    public double globalScale = 0.000001f;

    [Range(0.0001f, 1f)]
    public double distanceScale;

    public bool sqrtDistance;

    [Range(0.0001f, 500)]
    public float framesPerDay = 30f;
}

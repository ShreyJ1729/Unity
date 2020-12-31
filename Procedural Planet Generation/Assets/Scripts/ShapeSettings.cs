using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
    public float planetRadius;

    public Vector3 ScaleByRadius(Vector3 point)
    {
        return point * planetRadius;
    }

    private void OnValidate()
    {
        if (planetRadius <= 0.01f)
        {
            planetRadius = 0.01f;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
    public float planetRadius;
    public bool useFirstLayerAsMask;
    public NoiseLayer[] noiseLayers;
    
    private void OnValidate()
    {
        if (planetRadius <= 0.01f)
        {
            planetRadius = 0.01f;
        }
    }

    [Serializable]
    public class NoiseLayer
    {
        public NoiseSettings noiseSettings;
        public bool isEnabled = true;
        public bool useFirstLayerAsMask;
    }
}

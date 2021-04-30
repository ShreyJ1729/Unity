using System;
using UnityEngine;

[Serializable]
public class NoiseSettings
{
    public enum FilterType
    {
        Simple,
        Ridge
    }

    public FilterType filterType;
    [Range(1, 10)] public int numLayers;
    public float strength = 1f;
    public float baseRoughness = 1f;
    public float roughness = 2f;
    [Range(0, 1)] public float persistance = 0.5f;
    public float minVal;
    
    public Vector3 center;

}

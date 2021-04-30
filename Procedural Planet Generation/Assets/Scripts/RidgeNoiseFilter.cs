using System;
using UnityEngine;

public class RidgeNoiseFilter : BaseNoiseFilter
{
    private SimplexNoise noise = new SimplexNoise();
    private NoiseSettings settings;

    public RidgeNoiseFilter(NoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseVal = 0f;
        float frequency = settings.baseRoughness;
        float amplitude = 1f;
        float weight = 1f;

        for (int i = 0; i < settings.numLayers; i++)
        {
            // Construct ridges by inverse-sharpening
            float layerVal = 1 - Mathf.Abs(noise.Evaluate(point * frequency + settings.center));
            layerVal *= layerVal;
            layerVal *= weight;
            weight = layerVal;
            
            noiseVal += layerVal * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistance;
        }

        noiseVal = Mathf.Max(0, noiseVal - settings.minVal);
        return noiseVal * settings.strength;
    }
}
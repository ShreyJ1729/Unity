using System;
using UnityEngine;

public class SimpleNoiseFilter : BaseNoiseFilter
{
    private SimplexNoise noise = new SimplexNoise();
    private NoiseSettings settings;

    public SimpleNoiseFilter(NoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseVal = 0f;
        float frequency = settings.baseRoughness;
        float amplitude = 1f;

        for (int i = 0; i < settings.numLayers; i++)
        {
            float layerVal = noise.Evaluate(point * frequency + settings.center);
            noiseVal += (layerVal + 1) * 0.5f * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistance;
        }

        noiseVal = Mathf.Max(0, noiseVal - settings.minVal);
        return noiseVal * settings.strength;
    }
}

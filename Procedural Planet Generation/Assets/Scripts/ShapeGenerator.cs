using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    public ShapeSettings shapeSettings;
    public BaseNoiseFilter[] noiseFilters;
    public MinMax elevationMinMax;

    public ShapeGenerator(ShapeSettings shapeSettings)
    {
        this.shapeSettings = shapeSettings;
        this.noiseFilters = new BaseNoiseFilter[shapeSettings.noiseLayers.Length];

        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = NoiseFilterConstructor.ConstructNoiseFilter(shapeSettings.noiseLayers[i].noiseSettings);
        }

        elevationMinMax = new MinMax();
    }
        
    public Vector3 CalcPointOnSphere(Vector3 pointOnUnitSphere)
    {
        float firstLayerVal = 0;

        if (noiseFilters.Length > 1)
        {
            firstLayerVal = noiseFilters[0].Evaluate(pointOnUnitSphere);
        }
        
        float elevation = 0f;
        
        for (int i = 0; i < noiseFilters.Length; i++)
        {
            if (shapeSettings.noiseLayers[i].isEnabled)
            {
                float mask = (shapeSettings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerVal : 1;
                elevation +=  noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
            }
        }

        elevation = (1 + elevation) * shapeSettings.planetRadius;
        elevationMinMax.AddValue(elevation);
        Vector3 calculatedPoint = elevation * pointOnUnitSphere;
        return calculatedPoint;
    }
}

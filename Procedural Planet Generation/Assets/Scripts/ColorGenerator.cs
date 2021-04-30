using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator
{
    private ColorSettings colorSettings;

    public ColorGenerator(ColorSettings colorSettings)
    {
        this.colorSettings = colorSettings;
    }

    public void UpdateElevation(MinMax minMax)
    {
        colorSettings.planetMaterial.SetVector("_elevationMinMax", new Vector4(minMax.min, minMax.max));
    }
}

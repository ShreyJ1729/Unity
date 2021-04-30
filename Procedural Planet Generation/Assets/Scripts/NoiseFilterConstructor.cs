using UnityEngine;

public static class NoiseFilterConstructor
{
    public static BaseNoiseFilter ConstructNoiseFilter(NoiseSettings noiseSettings)
    {
        switch (noiseSettings.filterType)
        {
            case NoiseSettings.FilterType.Simple:
                return new SimpleNoiseFilter(noiseSettings);
            case NoiseSettings.FilterType.Ridge:
                return new RidgeNoiseFilter(noiseSettings);
        }

        return null;
    }
}
using UnityEngine;

public static class NoiseGenerator
{
    public enum NormalizeMode
    {
        local,
        global
    }
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float noiseScale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode)
    {
        float[,] noiseMap = new float[mapHeight, mapWidth];
        System.Random numGen = new System.Random(seed);

        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxHeight = 0;
        float frequency = 1;
        float amplitude = 1;
        
        for (int i = 0; i < octaves; i++)
        {
            octaveOffsets[i] = new Vector2(
                numGen.Next(-1000, 1000) + offset.x,
                numGen.Next(-1000, 1000) - offset.y
                );

            maxHeight += amplitude;
            amplitude *= persistance;
        }

        float minLocalVal = float.MaxValue;
        float maxLocalVal = float.MinValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;
        
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                frequency = 1;
                amplitude = 1;
                float noiseHeight = 0;
                
                
                // Construct noise by overlaying multiple noise octaves
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = frequency * (x - halfWidth + octaveOffsets[i].x) / noiseScale ;
                    float sampleY = frequency * (y - halfHeight + octaveOffsets[i].y) / noiseScale;
                    float noiseVal = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += amplitude * noiseVal;
                    
                    // Decay amplitude and frequency
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                // Add point to array
                noiseMap[y, x] = noiseHeight;
                
                // Finding min/max vals
                if (noiseHeight < minLocalVal)
                {
                    minLocalVal = noiseHeight;
                }

                if (noiseHeight > maxLocalVal)
                {
                    maxLocalVal = noiseHeight;
                }
            }
        }
        
        // Normalize noiseMap (0,1) by inverse-lerping with min/max vals in array
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (normalizeMode == NormalizeMode.local)
                {
                    noiseMap[y, x] = Mathf.InverseLerp(minLocalVal, maxLocalVal, noiseMap[y, x]);
                }
                else
                {
                    // Normalize height
                    noiseMap[y, x] = Mathf.Clamp((1.75f * (noiseMap[y, x]+1) / (2*maxHeight)), 0, int.MaxValue);
                }
            }
        }
        
        return noiseMap;
    }
}

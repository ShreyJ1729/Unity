using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallOffGenerator
{
    public static float[,] GenerateFalloffMap(int size)
    {
        float[,] falloffMap = new float[size,size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float x_ = x / (float) size * 2 - 1;
                float y_ = y / (float) size * 2 - 1;

                falloffMap[y, x] = Mathf.Max(Mathf.Abs(x_), Mathf.Abs(y_));
            }
        }

        return falloffMap;
    }

}

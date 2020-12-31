using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap,
        ColorMap,
        Mesh
    };
    
    public NoiseGenerator.NormalizeMode normalizeMode;

    public DrawMode drawMode;
    public const int mapChunkSize = 241;
    [Range(1, 6)]
    public int EditorLOD;
    public float meshHeightMultiplier;
    
    public int seed;
    
    public float noiseScale;
    
    public int numOctaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public Vector2 offset;
    
    public bool autoUpdate = true;
    
    public AnimationCurve meshHeightCurve;
    public TerrainType[] regions;

    private Queue<ThreadInfo<MapData>> mapDataThreadQueue = new Queue<ThreadInfo<MapData>>();
    private Queue<ThreadInfo<MeshData>> meshDataThreadQueue = new Queue<ThreadInfo<MeshData>>();

    public void Update()
    {
        lock (mapDataThreadQueue)
        {
            if (mapDataThreadQueue.Count > 0)
            {
                for (int i = 0; i < mapDataThreadQueue.Count; i++)
                {
                    ThreadInfo<MapData> threadInfo = mapDataThreadQueue.Dequeue();
                    threadInfo.callback(threadInfo.data);
                }
            }            
        }
        lock (meshDataThreadQueue)
        {
            if (meshDataThreadQueue.Count > 0)
            {
                for (int i = 0; i < meshDataThreadQueue.Count; i++)
                {
                    ThreadInfo<MeshData> threadInfo = meshDataThreadQueue.Dequeue();
                    threadInfo.callback(threadInfo.data);
                }
            }            
        }
    }

    public void RequestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(center, callback);
        };
        
        new Thread(threadStart).Start();
    }

    public void MapDataThread(Vector2 center, Action<MapData> callback)
    {
        MapData mapData = GenerateMap(center);
        lock (mapDataThreadQueue)
        {
            mapDataThreadQueue.Enqueue(new ThreadInfo<MapData>(mapData, callback));
        }
    }
    
    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, lod, callback);
        };
        
        new Thread(threadStart).Start();
    }

    public void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, lod);
        lock (meshDataThreadQueue)
        {
            meshDataThreadQueue.Enqueue(new ThreadInfo<MeshData>(meshData, callback));
        }
    }

    
    public void BuildMapInEditor()
    {
        MapData mapData = GenerateMap(Vector2.zero);
        MapDisplay display = FindObjectOfType<MapDisplay>();
        
        // Build texture from heightMap based on drawMode
        if (drawMode == DrawMode.NoiseMap)
        {
            display.BuildTexture(TextureGenerator.TextureFromNoiseMap(mapData.heightMap));
        } else if (drawMode == DrawMode.ColorMap)
        {
            Color[] colorMap = ColorMapFromHeightMap(mapData.heightMap);
            display.BuildTexture(TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        } else if (drawMode == DrawMode.Mesh)
        {
            Color[] colorMap = ColorMapFromHeightMap(mapData.heightMap);
            display.BuildMesh(MeshGenerator.GenerateMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, EditorLOD), TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
    }
    
    public MapData GenerateMap(Vector2 center)
    {
        // Generate heightMap with passed in params
        float[,] heightMap = NoiseGenerator.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, numOctaves, persistance, lacunarity, center + offset, normalizeMode);

        return new MapData(heightMap, ColorMapFromHeightMap(heightMap));
    }

    // Generate colorMap from heightMap using regions
    private Color[] ColorMapFromHeightMap(float[,] heightMap)
    {
        int height = heightMap.GetLength(0);
        int width = heightMap.GetLength(1);

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float currHeight = heightMap[y, x];
                foreach (TerrainType region in regions)
                {
                    if (currHeight >= region.height)
                    {
                        colorMap[y * width + x] = region.color;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return colorMap;
    }
    
    private void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (numOctaves < 0)
        {
            numOctaves = 0;
        }
        if (noiseScale <= 0)
        {
            noiseScale = 0.001f;
        }
    }
    
    // Struct to keep track of map/mesh data info gotten from threads
    public struct ThreadInfo<T>
    {
        public T data;
        public Action<T> callback;

        public ThreadInfo(T data, Action<T> callback)
        {
            this.data = data;
            this.callback = callback;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}

public struct MapData
{
    public float[,] heightMap;
    public Color[] colorMap;

    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float viewerMoveThresholdChunkUpdate = 25f;
    public const float sqrViewerMoveThresholdChunkUpdate = viewerMoveThresholdChunkUpdate*viewerMoveThresholdChunkUpdate;
    
    public Transform viewer;
    public Transform parent;
    public Material chunkMaterial;

    public LODInfo[] detailLevels;

    public static float maxViewDistance;

    public static Vector2 viewerPos;
    private Vector2 viewerPosOld;

    private static MapGenerator mapGenerator;

    private int chunkSize;
    private int chunksVisibleInViewDistance;

    private Dictionary<Vector2, TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
    private static List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

    private void Start()
    {
        maxViewDistance = detailLevels[detailLevels.Length - 1].visibleRangeThreshold;
        mapGenerator = FindObjectOfType<MapGenerator>();
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);
        
        UpdateVisibleChunks();
    }

    private void Update()
    {
        viewerPos = new Vector2(viewer.position.x, viewer.position.z);
        if ((viewerPos - viewerPosOld).sqrMagnitude > sqrViewerMoveThresholdChunkUpdate)
        {
            UpdateVisibleChunks();
            viewerPosOld = viewerPos;
        }
    }

    void UpdateVisibleChunks()
    {
        // Set all terrain chunks invisible
        foreach (TerrainChunk terrainChunk in visibleTerrainChunks)
        {
            terrainChunk.SetActive(false);
        }

        visibleTerrainChunks.Clear();

        Vector2 currentChunkCoord = new Vector2(
            Mathf.RoundToInt(viewerPos.x / chunkSize),
            Mathf.RoundToInt(viewerPos.y / chunkSize)
        );

        // Iterate over all coordinates adjacent to chunk that player is in
        for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
        {
            for (int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
            {
                Vector2 chunkCoord = new Vector2(currentChunkCoord.x + xOffset, currentChunkCoord.y + yOffset);

                // If chunk is around player, set it active
                if (terrainChunkDict.ContainsKey(chunkCoord))
                {
                    terrainChunkDict[chunkCoord].UpdateTerrainChunk();
                }
                else
                {
                    terrainChunkDict.Add(chunkCoord,
                        new TerrainChunk(chunkCoord, chunkSize, parent, chunkMaterial, detailLevels));
                }

                visibleTerrainChunks.Add(terrainChunkDict[chunkCoord]);
            }
        }
    }


    public class TerrainChunk
    {
        private GameObject terrainChunk;

        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private Bounds bounds;

        private LODInfo[] detailLevels;
        private LODMesh[] lodMeshes;

        private MapData mapData;
        private bool hasMapData;
        private int prevLodIndex = -1;

        public TerrainChunk(Vector2 coord, int size, Transform parent, Material material, LODInfo[] detailLevels)
        {
            // Define position
            Vector2 position = coord * size;
            Vector3 positionVector3 = new Vector3(position.x, 0, position.y);

            this.detailLevels = detailLevels;
            // Create object, setting position
            terrainChunk = new GameObject("Terrain Chunk");
            meshRenderer = terrainChunk.AddComponent<MeshRenderer>();
            meshFilter = terrainChunk.AddComponent<MeshFilter>();

            meshRenderer.material = material;

            terrainChunk.transform.position = positionVector3;

            // Create bounds so we can get nearestDistanceToViewer
            bounds = new Bounds(position, Vector3.one * size);

            // Parent of all chunks is TerrainGenerator object
            terrainChunk.transform.parent = parent;

            // Initialize and populate lodMeshes
            lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < lodMeshes.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
            }

            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }

        public void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            hasMapData = true;

            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;
            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk()
        {
            if (hasMapData)
            {
                float nearestDistanceToViewer = Mathf.Sqrt(bounds.SqrDistance(viewerPos));
                bool visibility = nearestDistanceToViewer <= maxViewDistance;

                if (visibility)
                {
                    int currLodIndex = 0;
                    for (int i = detailLevels.Length - 1; i >= 0; i--)
                    {
                        if (nearestDistanceToViewer > detailLevels[i].visibleRangeThreshold)
                        {
                            currLodIndex = i + 1;
                        }
                    }

                    if (currLodIndex != prevLodIndex)
                    {
                        LODMesh lodMesh = lodMeshes[currLodIndex];

                        if (lodMesh.hasMesh)
                        {
                            meshFilter.mesh = lodMesh.mesh;
                            prevLodIndex = currLodIndex;
                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }
                    }
                    visibleTerrainChunks.Add(this);
                }

                // If in view distance, render chunk, else un-render
                SetActive(visibility);
            }
        }

        public void SetActive(bool activeState)
        {
            terrainChunk.SetActive(activeState);
        }

        public bool isVisible()
        {
            return terrainChunk.activeSelf;
        }
    }

    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        public int lod;

        public Action updateCallback;

        public LODMesh(int lod, Action updateCallback)
        {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        public void RequestMesh(MapData mapdata)
        {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapdata, lod, OnMeshDataReceived);
        }

        public void OnMeshDataReceived(MeshData meshdata)
        {
            mesh = meshdata.CreateMesh();
            hasMesh = true;
            updateCallback();
        }
    }

    [Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleRangeThreshold;
    }
}
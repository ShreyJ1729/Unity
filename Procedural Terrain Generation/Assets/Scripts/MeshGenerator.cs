using System.Collections;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateMesh(float[,] heightMap, float meshHeightMultiplier, AnimationCurve _meshHeightCurve, int levelOfDetail)
    {
        AnimationCurve meshHeightCurve = new AnimationCurve(_meshHeightCurve.keys);
        int height = heightMap.GetLength(0);
        int width = heightMap.GetLength(1);
        
        // To center the mesh, we need offsets for topLeftX and topLeftZ
        float topLeftX = - (width - 1) / 2f;
        float topLeftZ = (height - 1) / 2f;
        
        int vertexIndex = 0;
        
        // Calculations to implement level of detail
        int lodIncrement;
        if (levelOfDetail == 0)
        {
            lodIncrement = 1;
        }
        else
        {
            lodIncrement = 2 * levelOfDetail;
        }
        
        int verticesPerDimension = (width - 1) / lodIncrement + 1;

        MeshData meshData = new MeshData(verticesPerDimension, verticesPerDimension);
        
        for (int y = 0; y < height; y += lodIncrement)
        {
            for (int x = 0; x < width; x += lodIncrement)
            {
                    meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, meshHeightCurve.Evaluate(heightMap[y, x]) * meshHeightMultiplier, topLeftZ - y);
                    meshData.uvs[vertexIndex] = new Vector2( x/(float)width, y/(float)height);
                 
                // If we are not on the edge, then add two triangles to the mesh
                if ((x != width - 1) && (y != height - 1))
                {
                    // Make sure to add vertices in clockwise order
                    meshData.AddTriangle(
                        vertexIndex,
                        vertexIndex + verticesPerDimension + 1,
                        vertexIndex + verticesPerDimension
                    );
                    meshData.AddTriangle(
                        vertexIndex,
                        vertexIndex + 1,
                        vertexIndex + verticesPerDimension + 1 
                    );
                }
                
                vertexIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public Vector2[] uvs;
    public int[] triangles;

    public int triangleIndex;
    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = this.vertices;
        mesh.uv = this.uvs;
        mesh.triangles = this.triangles;
        
        mesh.RecalculateNormals();
        return mesh;
    }
}


using UnityEngine;

public class SphereFace
{
    // Specify resolution of SphereFace (vertices per dimension)
    public ShapeGenerator shapeGenerator;
    public int resolution;
    public Mesh mesh;
    
    // Local coordinate system
    private Vector3 localI;
    private Vector3 localJ;
    private Vector3 localK;
    
    public SphereFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localK, ShapeSettings shapeSettings)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localK = localK;

        // Find local directions. localI from swapping around coords and k x i = j
        localI = new Vector3(localK.y, localK.z, localK.x);
        localJ = Vector3.Cross(localK, localI);
    }

    public void BuildMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];

        // Populating vertices
        int vertexIndex = 0;
        int triangleIndex = 0;
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                Vector2 percentTraveled = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localK + (percentTraveled.x-0.5f)*2*localI + (percentTraveled.y-0.5f)*2*localJ;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[y * resolution + x] = shapeGenerator.CalcPointOnSphere(pointOnUnitSphere);
                
                if ( (x != resolution - 1) && (y != resolution - 1))
                {
                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + 1;
                    triangles[triangleIndex + 2] = vertexIndex + resolution + 1;

                    triangles[triangleIndex + 3] = vertexIndex;
                    triangles[triangleIndex + 4] = vertexIndex + resolution + 1;
                    triangles[triangleIndex + 5] = vertexIndex + resolution;
                    
                    triangleIndex += 6;
                }

                vertexIndex++;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        
        // Normals point straight out from center but manually assigning causes material rendering problem
        // mesh.normals = vertices;
        mesh.RecalculateNormals();
    }
}
using System;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public bool autoUpdate = true;
    [Range(2, 255)] public int resolution;
    public SphereFace[] sphereFaces;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;
    [HideInInspector] public bool shapeSettingsFoldout;
    [HideInInspector] public bool colorSettingsFoldout;

    public ShapeGenerator shapeGenerator;
    public ColorGenerator colorGenerator;
    
    private MeshFilter[] meshFilters;

    private Vector3[] directions =
        {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

    private void Start()
    {
        InitializePlanet();
        BuildMesh();
    }

    public void InitializePlanet()
    {
        shapeGenerator = new ShapeGenerator(shapeSettings);
        colorGenerator = new ColorGenerator(colorSettings);
        
        if (meshFilters == default(MeshFilter[]) || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }

        sphereFaces = new SphereFace[6];

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == default(MeshFilter))
            {
                GameObject sphereFace = new GameObject("SphereFace");
                sphereFace.transform.parent = this.transform;

                meshFilters[i] = sphereFace.AddComponent<MeshFilter>();
                sphereFace.AddComponent<MeshRenderer>();
            }

            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;

            meshFilters[i].sharedMesh = new Mesh();
            sphereFaces[i] = new SphereFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i], shapeSettings);
        }
    }

    public void BuildPlanet()
    {
        InitializePlanet();
        BuildMesh();
        UpdateColors();
    }

    public void OnShapeSettingsUpdate()
    {
        InitializePlanet();
        BuildMesh();
    }

    public void OnColorSettingsUpdate()
    {
        UpdateColors();
    }

    public void UpdateColors()
    {
        foreach (MeshFilter meshFilter in meshFilters)
        {
            meshFilter.GetComponent<MeshRenderer>().sharedMaterial.color = colorSettings.planetColor;
        }
    }

    public void BuildMesh()
    {
        foreach (SphereFace face in sphereFaces)
        {
            face.BuildMesh();
        }
        colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }
}
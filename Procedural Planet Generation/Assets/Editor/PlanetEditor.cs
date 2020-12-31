using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    Planet planet;
    private Editor shapeEditor;
    private Editor colorEditor;
    
    public override void OnInspectorGUI()
    {
        // If anything in the main inspector is updated and autoUpdate is on, rebuild planet
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();

            if (planet.autoUpdate && check.changed)
            {
                planet.BuildPlanet();
            }
        }
        
        // If button pressed, rebuild planet
        if (GUILayout.Button("Build Planet"))
        {
            planet.BuildPlanet();
        }
        
        // Add shape and color editors.
        // - Foldout passed as ref since we want to change value in other class.
        // - Editor because we use CachedEditor
        AddSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdate, ref planet.shapeSettingsFoldout, ref shapeEditor);
        AddSettingsEditor(planet.colorSettings, planet.OnColorSettingsUpdate, ref planet.colorSettingsFoldout, ref colorEditor);
    }

    public void AddSettingsEditor(Object settings, System.Action onSettingsUpdate, ref bool foldout, ref Editor editor)
    {
        // If no settings object attached, exit
        if (settings == null) return;
        
        // Foldout toggling
        foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
        CreateCachedEditor(settings, null, ref editor);

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            if (foldout)
            {
                editor.OnInspectorGUI();
                
                if (check.changed)
                {
                    onSettingsUpdate();
                }
            }
        }
    }

    public void OnEnable()
    {
        planet = (Planet) target;
    }
}
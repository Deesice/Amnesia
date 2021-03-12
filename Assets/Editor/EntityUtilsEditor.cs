using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EntityUtils))]
public class EntityUtilsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Fill materials"))
            ((EntityUtils)target).FillMaterials();
        if (GUILayout.Button("Fix specs"))
            ((EntityUtils)target).FixSpecs();
    }
}

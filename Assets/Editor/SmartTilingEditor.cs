using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SmartTiling))]
public class SmartTilingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Recompute"))
            ((SmartTiling)target).Compute();
    }
}

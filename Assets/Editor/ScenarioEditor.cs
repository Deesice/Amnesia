using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Scenario))]
public class ScenarioEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Create scenario"))
            ((Scenario)target).CreateScenario();
    }
}

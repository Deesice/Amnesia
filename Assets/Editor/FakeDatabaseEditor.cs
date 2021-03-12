using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FakeDatabase))]
public class FakeDatabaseEditor : Editor
{    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Collect All"))
            ((FakeDatabase)target).CollectAll();
    }
}

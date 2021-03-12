using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LampLitController))]
public class LampLitControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Relit"))
        {
            var l = (LampLitController)target;
            l.Start();
            l.Relit();
        }
    }
}
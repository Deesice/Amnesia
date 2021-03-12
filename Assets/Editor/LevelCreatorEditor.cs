using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelCreator))]
public class LevelCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Create"))
            foreach (var i in ((LevelCreator)target).CreateLevel())
                TNRD.Utilities.IconManager.SetIcon(i, TNRD.Utilities.LabelIcon.Red);
        if (GUILayout.Button("Clear"))
            ((LevelCreator)target).ClearLevel();
    }
}

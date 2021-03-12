#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.IO;

class MyCustomBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;
    public void OnPreprocessBuild(BuildReport report)
    {
        foreach (var i in Directory.GetFiles("Assets/Scenes"))
        {
            if (!i.Contains(".meta"))
            {
                EditorSceneManager.OpenScene(i);
                foreach (var j in GameObject.FindObjectsOfType<SmartTiling>())
                    j.ApplyUV();
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), i);
            }
        }
    }
}
#endif
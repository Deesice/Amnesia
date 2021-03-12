using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finder : MonoBehaviour
{
    static Finder _instance;
    Dictionary<string, GameObject> buffer = new Dictionary<string, GameObject>();
    static Finder instance
    {
        get { if (_instance == null) { _instance = new GameObject().AddComponent<Finder>(); _instance.gameObject.name = "Finder"; } return _instance; }
    }
    public static GameObject Find(string name)
    {
        GameObject g;
        if (instance.buffer.TryGetValue(name, out g))
            return g;

        foreach (var i in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            g = FindRecoursively(name, i.transform);
            if (g != null)
                return g;
        }
        return null;
    }
    public static void Bufferize(params string[] s)
    {
        foreach (var i in SceneManager.GetActiveScene().GetRootGameObjects())
            foreach (var j in i.GetComponentsInChildren<Transform>(true))
                if (s.Contains(j.name))// && !instance.buffer.ContainsKey(j.name))
                {
                    instance.buffer.Remove(j.name);
                    instance.buffer.Add(j.name, j.gameObject);
                }

        Debug.Log("Cached " + instance.buffer.Count + " objects");
    }

    static GameObject FindRecoursively(string name, Transform t)
    {
        if (t.gameObject.name == name)
        {
            instance.buffer.Add(name, t.gameObject);
            return t.gameObject;
        }
        GameObject g = null;
        for (int i = 0; i < t.childCount; i++)
        {
            g = FindRecoursively(name, t.GetChild(i));
            if (g != null)
                return g;
        }

        return null;
    }
}

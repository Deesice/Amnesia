#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class EntityUtils : MonoBehaviour
{
    // Start is called before the first frame update
    public void FillMaterials()
    {
        int count = int.MaxValue;
        foreach (var i in CollectRecoursively("Assets", ".ent"))
        {
            var path = i.Insert(7, "Prefabs\\").Replace(".ent", ".prefab");
            GameObject prefab;
            try
            {
                prefab = PrefabUtility.LoadPrefabContents(path);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                continue;
            }
            XmlDocument mainDoc = new XmlDocument();
            mainDoc.Load(i);
            var root = mainDoc.DocumentElement;
            var rb = prefab.GetComponentsInChildren<Rigidbody>();
            List<GameObject> objects = new List<GameObject>();
            if (rb != null && rb.Length > 0)
            {
                foreach (var r in rb)
                    objects.Add(r.gameObject);
            }
            else
            {
                var meshes = prefab.GetComponentsInChildren<Collider>();
                foreach (var m in meshes)
                    objects.Add(m.gameObject);
            }
            var bodies = root.GetAttributes("ModelData", "Bodies");
            int k = 0;
            for (; k < Mathf.Min(objects.Count, bodies.Count); k++)
            {
                var matter = objects[k].GetComponent<Matter>();
                if (matter == null)
                    matter = objects[k].AddComponent<Matter>();
                matter.SetByString(bodies[k].Attributes.GetNamedItem("Material").InnerText);
            }
            for (; k < objects.Count; k++)
            {
                var matter = objects[k].GetComponent<Matter>();
                if (matter == null)
                    matter = objects[k].AddComponent<Matter>();
            }
            PrefabUtility.SaveAsPrefabAsset(prefab, path);
            PrefabUtility.UnloadPrefabContents(prefab);
            count--;
            if (count == 0)
                break;
        }
        Debug.Log("Processed!");
    }
    public void FixSpecs()
    {
        foreach (var i in CollectRecoursively("Assets", " 1.mat"))
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>(i);
            if (mat == null)
            {
                //Debug.Log("Failed to load " + i);
                continue;
            }
            var orig = mat.GetTexture("_MetallicGlossMap");
            if (orig == null)
            {
                //Debug.Log(i + " not contain spec");
                continue;
            }
            var name = i.Substring(0, i.LastIndexOf('\\') + 1) + orig.name + ".png";
            var spec = AssetDatabase.LoadAssetAtPath<Texture>(name);
            if (spec)
            {
                mat.SetTexture("_MetallicGlossMap", spec);
                mat.SetFloat("_GlossMapScale", 0.75f);
            }
            else
                Debug.Log("Error to load " + i.Replace(" 1.mat", "_spec.png"));
        }
    }
    IEnumerable<string> CollectRecoursively(string dir, string mustContain)
    {
        foreach (var i in Directory.GetFiles(dir))
            if (i.Contains(mustContain) && !i.Contains(".meta"))
                yield return i;

        foreach (var i in Directory.GetDirectories(dir))
            foreach (var j in CollectRecoursively(i, mustContain))
                yield return j;
    }
}
#endif

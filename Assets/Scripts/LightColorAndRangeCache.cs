using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightColorAndRangeCache : MonoBehaviour, ISave
{
    public void OnLoad(Data data)
    {
        float r, g, b, a, range;
        var l = GetComponent<Light>();

        data.FloatKeys.TryGetValue(this.GetHierarchyPath() + "_ColorR", out r, l.color.r);
        data.FloatKeys.TryGetValue(this.GetHierarchyPath() + "_ColorG", out g, l.color.g);
        data.FloatKeys.TryGetValue(this.GetHierarchyPath() + "_ColorB", out b, l.color.b);
        data.FloatKeys.TryGetValue(this.GetHierarchyPath() + "_ColorA", out a, l.color.a);
        data.FloatKeys.TryGetValue(this.GetHierarchyPath() + "_Radius", out range, l.range);

        l.color = new Color(r, g, b, a);
        l.range = range;
    }

    public void OnSave(Data data)
    {
        var color = GetComponent<Light>().color;
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath() + "_ColorR", color.r);
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath() + "_ColorG", color.g);
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath() + "_ColorB", color.b);
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath() + "_ColorA", color.a);
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath() + "_Radius", GetComponent<Light>().range);
    }
}

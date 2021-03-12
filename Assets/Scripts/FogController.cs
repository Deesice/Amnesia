using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FogController : MonoBehaviour, ISave
{
    static Fog instance;
    void Awake()
    {
        GetComponent<PostProcessVolume>().sharedProfile.TryGetSettings(out instance);
    }
    public static void Enable(bool b)
    {
        instance.strength.value = b ? 1 : 0;
    }

    public static void SetFogProperties(float afStart, float afEnd, float afFalloffExp, bool abCulling)
    {
        Debug.Log("Меняем параметры тумана");
        instance.start.value = afStart;
        instance.end.value = afEnd;
        instance.exp.value = afFalloffExp;
        Camera.main.farClipPlane = afEnd;
        instance.cameraClipPlane.value = afEnd;
    }
    public static void SetFogColor(Color color)
    {
        instance.color.value = color / 3.5f;
    }

    public void OnLoad(Data data)
    {
        float f;
        bool boo;
        if (data.FloatKeys.TryGetValue(this.GetHierarchyPath() + "_Start", out f, instance.start.value))
            instance.start.value = f;
        if (data.FloatKeys.TryGetValue(this.GetHierarchyPath() + "_End", out f, instance.end.value))
        {
            instance.end.value = f;
            instance.cameraClipPlane.value = f;
            Camera.main.farClipPlane = f;
        }
        if (data.FloatKeys.TryGetValue(this.GetHierarchyPath() + "_Exp", out f, instance.exp.value))
            instance.exp.value = f;
        if (data.BoolKeys.TryGetValue(this.GetHierarchyPath(), out boo, instance.enabled))
            instance.strength.value = boo ? 1 : 0;

        float r, g, b, a;

        data.FloatKeys.TryGetValue(this.GetHierarchyPath() + "_ColorR", out r, 0);
        data.FloatKeys.TryGetValue(this.GetHierarchyPath() + "_ColorG", out g, 0);
        data.FloatKeys.TryGetValue(this.GetHierarchyPath() + "_ColorB", out b, 0);
        data.FloatKeys.TryGetValue(this.GetHierarchyPath() + "_ColorA", out a, 0);

        instance.color.value = new Color(r, g, b, a);
    }

    public void OnSave(Data data)
    {
        var c = instance.color.value;
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath() + "_Start", instance.start.value);
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath() + "_End", instance.end.value);
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath() + "_Exp", instance.exp.value);
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath() + "_ColorR", c.r);
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath() + "_ColorG", c.g);
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath() + "_ColorB", c.b);
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath() + "_ColorA", c.a);
        data.BoolKeys.SetValueSafety(this.GetHierarchyPath(), instance.strength.value == 1);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxLight : MonoBehaviour, ISave
{
    public Color color;
    [HideInInspector] public float addedValue;
    static BoxLight master;
    static List<BoxLight> list = new List<BoxLight>();
    void Awake()
    {
        master = this;
        list.Clear();
    }
    private void Update()
    {
        if (master != this || Time.timeSinceLevelLoad < 1)
            return;

        var averageColor = AverageColor();
        var ambColor = RenderSettings.ambientLight;

        if (ambColor == averageColor)
            return;

        RenderSettings.ambientLight = Color.Lerp(ambColor, averageColor, Time.deltaTime);
    }
    Color AverageColor()
    {
        var color = Color.black;
        if (list.Count == 0)
            return color;
        foreach (var i in list)
            color += i.color;
        color /= list.Count;
        color.a = 1;
        return color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            addedValue = Mathf.Max(color.r, color.g, color.b);
            list.Add(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            addedValue = 0;
            list.Remove(this);
        }
    }
    public void FadeTo(Color newColor, float time)
    {
        if (time <= 0)
            color = newColor;
        else
            StartCoroutine(Fading(newColor, time));
    }
    IEnumerator Fading(Color newColor, float time)
    {
        var startColor = color;
        float i = 0;
        while (i < 1)
        {
            yield return null;
            color = Color.Lerp(startColor, newColor, i);
            i += Time.deltaTime / time;
        }
        color = newColor;
    }

    public void OnLoad(Data data)
    {
        float r, g, b, a;

        data.FloatKeys.TryGetValue(this.GetHierarchyPath() + "_ColorR", out r, color.r);
        data.FloatKeys.TryGetValue(this.GetHierarchyPath() + "_ColorG", out g, color.g);
        data.FloatKeys.TryGetValue(this.GetHierarchyPath() + "_ColorB", out b, color.b);
        data.FloatKeys.TryGetValue(this.GetHierarchyPath() + "_ColorA", out a, color.a);

        color = new Color(r, g, b, a);
    }

    public void OnSave(Data data)
    {
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath() + "_ColorR", color.r);
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath() + "_ColorG", color.g);
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath() + "_ColorB", color.b);
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath() + "_ColorA", color.a);
    }
}

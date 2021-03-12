using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    public Color offColor;
    public float onMinTime;
    public float onMaxTime;
    public float offMinTime;
    public float offMaxTime;
    public float minRadius;
    public float strength = 1;
    Color startColor;
    float startRadius;
    new Light light;
    void OnEnable()
    {
        if (light == null)
            light = GetComponent<Light>();
        startColor = light.color;
        startRadius = light.range;
        StartCoroutine(Flicking());
    }

    IEnumerator Flicking()
    {
        float time;
        float i;
        while (true)
        {
            if (!light.enabled)
            {
                yield return null;
                continue;
            }
            i = 0;
            time = Random.Range(offMinTime, offMaxTime);
            while (i < 1)
            {
                light.color = Color.Lerp(startColor, offColor, i * strength);
                light.range = Mathf.Lerp(startRadius, minRadius, i * strength);
                yield return null;
                i += Time.deltaTime / time;
            }
            i = 0;
            time = Random.Range(onMinTime, onMaxTime);
            while (i < 1)
            {
                light.color = Color.Lerp(offColor, startColor, i * strength + 1 - strength);
                light.range = Mathf.Lerp(minRadius, startRadius, i * strength + 1 - strength);
                yield return null;
                i += Time.deltaTime / time;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LampLitController : MonoBehaviour, ISave
{
    float intensity = 1;
    public MeshRenderer[] lampMeshes;
    public Material litMat;
    public Material unlitMat;
    Light[] allLights;    
    public bool lit = true;
    ParticleSystem[] particles;
    public event Action<bool> OnLit;
    SoundPropetyContainer audioSource;
    public void Start()
    {
        audioSource = GetComponentInChildren<SoundPropetyContainer>();
        allLights = GetComponentsInChildren<Light>();
        particles = GetComponentsInChildren<ParticleSystem>();

        foreach (var l in allLights)
            l.enabled = lit;
    }
    public void Lit()
    {
        if (!lit)
            Relit();
    }
    public void UnLit()
    {
        if (lit)
            Relit();
    }
    public void Relit()
    {
        Start();
        lit = !lit;
        OnLit?.Invoke(lit);
        foreach (var i in lampMeshes)
            i.material = lit ? litMat : unlitMat;
        foreach (var i in particles)
        {
            var e = i.emission;
            e.enabled = lit;
            
        }
        if (Application.isPlaying)
        {
            StartCoroutine(GradientIntensity(!lit));
            if (audioSource)
            {
                if (audioSource.enabled)
                    audioSource.Source.enabled = lit;
                else
                    audioSource.enabled = true;
            }
        }
        else
        {
            StartCoroutine(GradientIntensity(!lit, 0));
            if (audioSource)
                audioSource.enabled = lit;
#if UNITY_EDITOR
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
#endif
        }
    }
    IEnumerator GradientIntensity(bool unlit, float time = 1)
    {
        foreach (var l in allLights)
            l.enabled = true;
        if (time == 0)
        {
            foreach (var l in allLights)
            {
                l.intensity = (unlit ? 0 : intensity);
                l.enabled = !unlit;
            }
        }
        else
        {
            float i = 0;
            while (i < 1)
            {
                foreach (var l in allLights)
                    l.intensity = intensity * (unlit ? 1 - i : i);
                yield return null;
                i += Time.deltaTime / time;
            }
            foreach (var l in allLights)
            {
                l.intensity = (unlit ? 0 : intensity);
                l.enabled = !unlit;
            }

        }
    }

    public void OnLoad(Data data)
    {
        if (data.BoolKeys.TryGetValue(this.GetHierarchyPath(), out var b, false))
        {
            if (b)
                Lit();
            else
                UnLit();
        }
    }

    public void OnSave(Data data)
    {
        data.BoolKeys.SetValueSafety(this.GetHierarchyPath(), lit);
    }
}

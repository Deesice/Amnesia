using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShaderController : MonoBehaviour
{
    public Material material;
    Material origMaterial;
    MaskableGraphic image;
    static float startTime;
    void Awake()
    {
        image = GetComponent<MaskableGraphic>();
        origMaterial = image.material;
    }
    private void Update()
    {
        material.SetFloat("_MyTime", Time.unscaledTime - startTime);
    }

    private void OnEnable()
    {
        startTime = Time.unscaledTime;
        material.SetFloat("_MyTime", 0);
        image.material = material;
    }
    private void OnDisable()
    {
        image.material = origMaterial;
        material.SetFloat("_MyTime", 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialBlurController : CustomImageEffect
{
    static RadialBlurController instance;
    IEnumerator coroutine = null;
    void Awake()
    {
        instance = this;
        var resolution = Screen.currentResolution;
        Debug.Log(resolution);
        effectMaterial.SetFloat("_Aspect", ((float)resolution.width) / resolution.height);
        effectMaterial.SetFloat("_afSize", 0);
        enabled = false;
    }
    public static void SetRadialBlurStartDist(float value)
    {
        //NOT WORK RIGHT
        //instance.effectMaterial.SetFloat("_afBlurStartDist", value);
    }
    public static void FadeRadialBlurTo(float afSize, float afSpeed)
    {
        if (instance)
        {
            instance.enabled = true;
            if (instance.coroutine != null)
                instance.StopCoroutine(instance.coroutine);
            instance.coroutine = instance.Fading(afSize, afSpeed);
            instance.StartCoroutine(instance.coroutine);
        }
    }
    IEnumerator Fading(float afAmount, float afSpeed)
    {
        bool flag = true;
        float blurAmount = effectMaterial.GetFloat("_afSize");
        while (flag)
        {
            if (afAmount < blurAmount)
            {
                blurAmount -= afSpeed * Time.deltaTime;
                if (blurAmount <= afAmount)
                {
                    blurAmount = afAmount;
                    flag = false;
                }
            }
            else
            {
                blurAmount += afSpeed * Time.deltaTime;
                if (blurAmount >= afAmount)
                {
                    blurAmount = afAmount;
                    flag = false;
                }
            }
            effectMaterial.SetFloat("_afSize", blurAmount);
            yield return null;
        }
        coroutine = null;
        if (blurAmount == 0)
            enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SepiaController : MonoBehaviour
{
    PostProcessVolume volume;
    static SepiaController instance;
    IEnumerator coroutine = null;
    void Start()
    {
        instance = this;
        volume = GetComponent<PostProcessVolume>();
    }
    public static void FadeSepiaColorTo(float afAmount, float afSpeed)
    {
        instance.gameObject.SetActive(true);
        if (instance.coroutine != null)
            instance.StopCoroutine(instance.coroutine);
        instance.coroutine = instance.Fading(afAmount, afSpeed);
        instance.StartCoroutine(instance.coroutine);
    }
    IEnumerator Fading(float afAmount, float afSpeed)
    {
        bool flag = true;
        while (flag)
        {
            if (afAmount < volume.weight)
            {
                volume.weight -= afSpeed * Time.deltaTime;
                if (volume.weight <= afAmount)
                {
                    volume.weight = afAmount;
                    flag = false;
                }
            }
            else
            {
                volume.weight += afSpeed * Time.deltaTime;
                if (volume.weight >= afAmount)
                {
                    volume.weight = afAmount;
                    flag = false;
                }
            }
            yield return null;
        }
        coroutine = null;
        if (volume.weight == 0)
            gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    MaskableGraphic image = null;
    IEnumerator coroutine = null;
    [SerializeField] bool fadeOffOnEnable;
    [SerializeField] bool fadeOnOnEnable;
    bool isOn;
    private void Awake()
    {
        image = GetComponent<MaskableGraphic>();
        if (image == null)
            Destroy(this);
        else
            isOn = image.color.a > 0;
    }
    private void OnEnable()
    {
        if (fadeOffOnEnable)
        {
            var color = image.color;
            color.a = 0;
            image.color = color;
            isOn = false;
            FadeOn(Color.white, 0.25f);
        }
        else if (fadeOnOnEnable)
        {
            var color = image.color;
            color.a = 1;
            image.color = color;
            isOn = true;
            FadeOff();
        }
    }
    private void OnDisable()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = null;
    }
    public void FadeOn(Color color, float time = 1)
    {
        if (isOn)
            return;
        if (image == null)
            image = GetComponent<MaskableGraphic>();
        if (image == null)
            Destroy(this);
        if (coroutine != null)
            StopCoroutine(coroutine);
        isOn = true;
        if (time == 0)
        {
            image.color = color;
            coroutine = null;
        }
        else
        {
            if (gameObject.activeSelf)
            {
                coroutine = Fading(color, time, true);
                StartCoroutine(coroutine);
            }
        }
    }
    public void FadeOff(float time = 1)
    {
        if (!isOn)
            return;
        if (image == null)
            image = GetComponent<MaskableGraphic>();
        if (image == null)
            Destroy(this);
        if (coroutine != null)
            StopCoroutine(coroutine);
        isOn = false;
        if (time == 0)
        {
            var color = image.color;
            color.a = 0;
            image.color = color;
            coroutine = null;
        }
        else
        {
            if (gameObject.activeSelf)
            {
                coroutine = Fading(Color.white, time, false);
                StartCoroutine(coroutine);
            }
        }
    }
    IEnumerator Fading(Color targetColor, float time, bool toSolidColor)
    {        
        float i;
        if (!toSolidColor)
        {
            targetColor = image.color;
            targetColor.a = 0;
        }
        else
        {
            i = targetColor.a;
            targetColor.a = image.color.a;
            image.color = targetColor;
            targetColor.a = i;
        }
        Color startColor = image.color;
        i = 0;
        yield return null;
        while (i < 1)
        {
            image.color = Color.Lerp(startColor, targetColor, i);
            yield return null;
            i += Time.unscaledDeltaTime / time;
        }
        image.color = targetColor;
        coroutine = null;
    }
}

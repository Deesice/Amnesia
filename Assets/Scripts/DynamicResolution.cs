using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicResolution : MonoBehaviour
{    
    public float lowerThreshold;
    public float upperThreshold;
    public float timeDelay;
    public float maxResolutionScale = 1.0f;
    public float minResolutionScale = 0.5f;
    public float scaleIncrement = 0.1f;

    float currentFPS;
    static float currentScale = 1.0f;
    private void Start()
    {
        ScalableBufferManager.ResizeBuffers(currentScale, currentScale);
        StartCoroutine(AdaptiveResolution());
    }
    IEnumerator AdaptiveResolution()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeDelay);
            currentFPS /= timeDelay;
            if (currentFPS < lowerThreshold)
            {
                currentScale -= scaleIncrement;
                currentScale = Mathf.Clamp(currentScale, minResolutionScale, maxResolutionScale);
                ScalableBufferManager.ResizeBuffers(currentScale, currentScale);
            }
            else if (currentFPS > upperThreshold)
            {
                currentScale += scaleIncrement;
                currentScale = Mathf.Clamp(currentScale, minResolutionScale, maxResolutionScale);
                ScalableBufferManager.ResizeBuffers(currentScale, currentScale);
            }
            currentFPS = 0;
        }
    }

    void Update()
    {
        currentFPS += 1;
    }
}

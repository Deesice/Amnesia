using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVSaver : MonoBehaviour
{
    Camera cam;
    float prevValue;
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cam.fieldOfView < 10)
            cam.fieldOfView = prevValue;
        else
            prevValue = cam.fieldOfView;
    }
}

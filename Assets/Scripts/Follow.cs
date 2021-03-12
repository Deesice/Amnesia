using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public float damp;
    public bool invertZ;
    public bool invertY;

    // Update is called once per frame
    void LateUpdate()
    {
        if (damp == 0)
            transform.rotation = Quaternion.LookRotation(invertZ ? -target.forward : target.forward, invertY ? -target.up : target.up);
        else
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(invertZ ? -target.forward : target.forward, invertY ? -target.up: target.up), Time.deltaTime * damp);
    }
}

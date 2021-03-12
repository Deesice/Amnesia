using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeGravity : MonoBehaviour
{
    public float limit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var r = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(r.x, r.y, 0);
        r = transform.localRotation.eulerAngles;
        if (r.z > 180)
            r.z -= 360;
        r.z = Mathf.Clamp(r.z, -limit, limit);
        transform.localRotation = Quaternion.Euler(r);
    }
}

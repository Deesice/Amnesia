using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAfterBreak : MonoBehaviour
{
    public float breakImpulse = 3;
    void Start()
    {
        foreach (var i in GetComponentsInChildren<Rigidbody>())
            i.AddForce((i.transform.position - transform.position).normalized * breakImpulse, ForceMode.VelocityChange);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antimotor : MonoBehaviour
{
    Rigidbody rb;
    RigidbodyConstraints originalConstraints;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalConstraints = rb.constraints;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
            rb.constraints = originalConstraints | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
           
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Player")
            rb.constraints = originalConstraints;
    }

    private void OnDisable()
    {
        rb.constraints = originalConstraints;
    }
}

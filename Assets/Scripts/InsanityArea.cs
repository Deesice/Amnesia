using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsanityArea : ReflectableMonoBehaviour
{
    public bool autoDisable;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            SanitySystem.OnEnterInsanityArea();
            if (autoDisable)
                gameObject.SetActive(false);
        }
    }
}

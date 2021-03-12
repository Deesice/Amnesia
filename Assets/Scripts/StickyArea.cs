using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyArea : ReflectableMonoBehaviour
{
    public bool moveBody;
    public bool rotateBody;
    public bool checkCenterInArea;
    public bool canDetach;
    public float poseTime;
    public string attachableBodyName;
    public string attachFunction;
    public string detachFunction;
    public string attachSound;
    public string detachSound;
    public string attachPS;
    public string detachPS;
    BoxCollider myCollider;
    bool targetInside;
    private void Start()
    {
        myCollider = GetComponent<BoxCollider>();
        transform.Rotate(Vector3.up * 180, Space.Self);
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (targetInside)
            return;
        if (collision.gameObject.name.Contains(attachableBodyName) && (!checkCenterInArea || myCollider.bounds.Contains(collision.bounds.center)))
        {
            targetInside = true;
            if (PlayerController.instance.grabObject == collision.gameObject)
                PlayerController.instance.DenyHandByForce();
            var interactableCollision = collision.gameObject.GetComponentInChildren<Interactable>();
            if (interactableCollision == null)
                interactableCollision = collision.gameObject.GetComponentInParent<Interactable>();

            if (moveBody)
                StartCoroutine(MoveCoroutine(interactableCollision.root.transform, poseTime));
            if (rotateBody)
                StartCoroutine(RotateCoroutine(interactableCollision.root.transform, poseTime));
           
            if (!string.IsNullOrEmpty(attachSound))
                SoundManager.PlaySoundAtEntity(gameObject + "_attachSound", attachSound, gameObject.name, 0);
            if (!string.IsNullOrEmpty(attachPS))
                Scenario.currentScenario.CreateParticleSystemAtEntity(gameObject + "_attachPS", attachPS, gameObject.name, false);
            if (!string.IsNullOrEmpty(attachFunction))
            {
                string n;
                if (collision.gameObject.name.Contains("Body"))
                    n = interactableCollision.root.name + "_" + collision.gameObject.name;
                else
                    n = interactableCollision.root.name + "_Body_1";
                Type.GetType(Scenario.currentScenario.ClassName).GetMethod(attachFunction)
                    .Invoke(Scenario.currentScenario, new object[] { gameObject.name, n });
            }
            if (!canDetach)
            {
                foreach (var j in interactableCollision.root.GetComponentsInChildren<Rigidbody>())
                    Destroy(j);
                foreach (var j in interactableCollision.root.GetComponentsInChildren<Transform>())
                    j.gameObject.tag = "Untagged";
                interactableCollision.enabled = false;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!canDetach)
            return;
        else
            throw new NotImplementedException();
    }
    IEnumerator MoveCoroutine(Transform t, float time)
    {
        Vector3 startPos = t.position;
        float i = 0;
        while (i < 1)
        {
            t.position = Vector3.Lerp(startPos, transform.position, i);
            yield return null;
            i += Time.deltaTime / time;
        }
        t.position = transform.position;
    }
    IEnumerator RotateCoroutine(Transform t, float time)
    {
        Quaternion start = t.rotation;
        float i = 0;
        while (i < 1)
        {
            t.rotation = Quaternion.Lerp(start, transform.rotation, i);
            yield return null;
            i += Time.deltaTime / time;
        }
        t.rotation = transform.rotation;
    }
}

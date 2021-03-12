using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : Interactable, ISave
{
    public float acceleration = 5;
    public float grabThrowSpeed = 10;
    int startLayer;
    Quaternion startRotation;
    Vector3 startPosition;
    List<Collision> collisions = new List<Collision>();
    new Rigidbody rigidbody;
    void Start()
    {
        gameObject.tag = "Grab";
        startLayer = gameObject.layer;
        rigidbody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (isGrabbed)
        {
            var targetPosition = hand.position + hand.transform.forward * startPosition.z + hand.transform.up * startPosition.y + hand.transform.right * startPosition.x;
            rigidbody.velocity = (targetPosition - transform.position) * acceleration;
            Rotate();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.isStatic)
            collisions.Add(collision);
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.isStatic)
            collisions.Remove(collision);
    }
    void Rotate()
    {
        if (collisions.Count == 0)
        {
            var targetRotation = Quaternion.LookRotation(hand.transform.forward, hand.transform.up) * startRotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * acceleration);
            rigidbody.freezeRotation = true;
        }
        else
        {
            rigidbody.freezeRotation = false;
        }
    }

    protected override void ApplyHandContent(Rigidbody hand)
    {
        this.hand = hand;
        rigidbody.useGravity = false;

        gameObject.layer = 8;
        var buf = transform.parent;
        transform.SetParent(hand.transform);
        startRotation = transform.localRotation;
        startPosition = transform.localPosition;
        transform.SetParent(buf);
        startPosition *= 0.75f;
        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public override void DenyHand()
    {
        if (rigidbody)
        {
            rigidbody.AddForce(hand.transform.forward * grabThrowSpeed, ForceMode.VelocityChange);
            rigidbody.useGravity = true;
            rigidbody.interpolation = RigidbodyInterpolation.None;
            rigidbody.freezeRotation = false;
        }
        hand = null;
        gameObject.layer = startLayer;
    }

    public override void RecieveFingerDelta(Vector2 mouseInput, Vector2 joystickInput)
    {
    }

    void ISave.OnLoad(Data data)
    {
        if (data.VectorKeys.TryGetValue(this.GetHierarchyPath() + "/position", out var pos, Vector3.zero))
            transform.position = pos;

        if (data.VectorKeys.TryGetValue(this.GetHierarchyPath() + "/rotation", out var rot, Vector3.zero))
            transform.rotation = Quaternion.Euler(rot);

        OnLoad(data);
    }

    void ISave.OnSave(Data data)
    {
        data.VectorKeys.SetValueSafety(this.GetHierarchyPath() + "/position", transform.position);
        data.VectorKeys.SetValueSafety(this.GetHierarchyPath() + "/rotation", transform.rotation.eulerAngles);
        OnSave(data);
    }
}

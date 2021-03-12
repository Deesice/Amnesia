using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObject : Interactable, ISave
{
    Rigidbody rb;
    Antimotor antimotor;
    public float force = 0.3f;
    public float angularDrag = 20;
    public float drag = 5;
    float origDrag;
    float origLin;
    protected override void ApplyHandContent(Rigidbody hand)
    {
        antimotor.enabled = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        this.hand = hand;
        rb.angularDrag = angularDrag;
        rb.drag = drag;
        PlayerController.instance.pushMultipliers.runSpeedMul = 1.5f / 1.7f;
        PlayerController.instance.pushMultipliers.moveSpeedMul = 0.8f;
    }

    public override void DenyHand()
    {
        antimotor.enabled = true;
        rb.interpolation = RigidbodyInterpolation.None;
        hand = null;
        rb.angularDrag = origDrag;
        rb.drag = origLin;
        PlayerController.instance.pushMultipliers.runSpeedMul = 1;
        PlayerController.instance.pushMultipliers.moveSpeedMul = 1;
    }

    public override void RecieveFingerDelta(Vector2 mouseInput, Vector2 joystickInput)
    {
    }
    void Start()
    {
        if ((antimotor = GetComponent<Antimotor>()) == null)
            antimotor = gameObject.AddComponent<Antimotor>();
        blockView = true;
        foreach (var t in root.GetComponentsInChildren<Transform>())
            t.gameObject.tag = "Push";
        rb = GetComponentInChildren<Rigidbody>();        
        origDrag = rb.angularDrag;
        origLin = rb.drag;
    }
    private void FixedUpdate()
    {
        if (isGrabbed)
        {
            var dir = PlayerController.instance.playerBody.velocity * force;
            dir.y = 0;
            rb.AddForceAtPosition(dir, transform.position, ForceMode.VelocityChange);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, PlayerController.instance.speed * PlayerController.instance.totalWalkMultiplier);
        }
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

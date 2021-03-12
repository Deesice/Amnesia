using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideJoint : Interactable, IState
{
    public Door.FaceAxe face;
    Rigidbody rb;
    Vector3 startPos;
    Vector3 endPos;
    public float amplitude = 1;
    public bool NearBoard { get { return !((transform.position - endPos).magnitude < amplitude && (transform.position - startPos).magnitude < amplitude); } }
    public GameObject[] otherParts;
    Vector3 forward { get
        {
            switch (face)
            {
                case Door.FaceAxe.invX:
                    return -transform.right;
                case Door.FaceAxe.invY:
                    return -transform.up;
                case Door.FaceAxe.X:
                    return transform.right;
                case Door.FaceAxe.Y:
                    return transform.up;
                case Door.FaceAxe.Z:
                    return transform.forward;
                default:
                    return -transform.forward;
            }
        } }
    Vector3 targetPosition;

    public event Action<float> OnStateChanged;
    float stuckState;

    private void Awake()
    {
        blockView = true;
        startPos = transform.position;
        targetPosition = startPos;
        endPos = startPos + forward * amplitude;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        gameObject.tag = "Grab";
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | (Mathf.Abs(forward.z) < 0.5f ? RigidbodyConstraints.FreezePositionZ : RigidbodyConstraints.FreezePositionX);
        foreach (var o in otherParts)
            foreach (var c in o.GetComponents<Collider>())
                foreach (var m in GetComponents<Collider>())
                    Physics.IgnoreCollision(c, m, true);

        foreach (var m in GetComponents<Collider>())
            foreach (var c in GetComponents<Collider>())
                if (c != m)
                    Physics.IgnoreCollision(c, m, true);
    }
    protected override void ApplyHandContent(Rigidbody hand)
    {
        this.hand = hand;
        targetPosition = transform.position;
    }

    public override void DenyHand()
    {
        hand = null;
    }

    public override void RecieveFingerDelta(Vector2 mouseInput, Vector2 joystickInput)
    {
        var v = hand.transform.forward;
        v.y = 0;
        v.Normalize();
        var handDelta = Vector3.Project(v * mouseInput.y - hand.transform.right * mouseInput.x, forward);

        targetPosition += handDelta * 0.01f;
        targetPosition = Vector3.ClampMagnitude(targetPosition - startPos, amplitude) + startPos;
        if ((endPos - targetPosition).magnitude > amplitude)
            targetPosition = startPos;
    }
    private void Update()
    {
        if (stuckState != 0)
        {
            rb.velocity = ((stuckState == 1 ? endPos : startPos) - transform.position) * 10;
        }
        else
        {
            if (isGrabbed)
            {
                rb.velocity = (targetPosition - transform.position) * 10;
                return;
            }

            var prevPos = transform.position;

            transform.position = Vector3.ClampMagnitude(transform.position - startPos, amplitude) + startPos;
            if ((endPos - transform.position).magnitude > amplitude)
                transform.position = startPos;

            if (prevPos != transform.position)
                rb.velocity = Vector3.zero;
        }
    }

    public void SetState(float state)
    {
        throw new NotImplementedException();
    }

    public float GetState()
    {
        throw new NotImplementedException();
    }

    public void SetStuckState(float state)
    {
        stuckState = state;
    }
    void ISave.OnLoad(Data data)
    {
        data.FloatKeys.TryGetValue(this.GetHierarchyPath(), out stuckState, stuckState);
        OnLoad(data);
    }
    void ISave.OnSave(Data data)
    {
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath(), stuckState);
        OnSave(data);
    }
}

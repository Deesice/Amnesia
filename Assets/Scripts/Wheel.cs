using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : Interactable, ILimit, IState
{
    Vector2 prevMouseInput;
    public Door.FaceAxe axe;
    public float rotationSpeed = 0.1f;
    public float minLimit;
    public float maxLimit;
    float goalRotate;
    public float currentRotation;
    float delta;
    Vector3 rotationAxe;

    public event Action<float> OnStateChanged;
    float state;
    bool isStuck;

    public float commitRange = 5;

    public override void DenyHand()
    {
        hand = null;
        prevMouseInput = Vector2.zero;
    }

    public override void RecieveFingerDelta(Vector2 mouseInput, Vector2 joystickInput)
    {
        if (Vector2.Angle(mouseInput, prevMouseInput) < 60)
            delta = Vector2.SignedAngle(mouseInput, prevMouseInput) * rotationSpeed * Mathf.Sign(Vector3.Dot(rotationAxe, hand.transform.forward));
        else
            delta = 0;
        goalRotate -= delta;
        prevMouseInput = mouseInput;
        if (!isStuck)
        {
            if (delta > 0)
            {
                goalRotate = Mathf.Clamp(goalRotate, -100000000, maxLimit);
                return;
            }
            if (delta < 0)
                goalRotate = Mathf.Clamp(goalRotate, minLimit, 100000000);
        }
        else
        {
            if (state == -1)
            {
                goalRotate = Mathf.Clamp(goalRotate, minLimit, minLimit + commitRange);
                return;
            }
            else
            {
                goalRotate = Mathf.Clamp(goalRotate, maxLimit - commitRange, maxLimit);
            }
        }
    }

    protected override void ApplyHandContent(Rigidbody hand)
    {
        this.hand = hand;
    }
    void Start()
    {
        blockView = true;
        foreach (var t in root.GetComponentsInChildren<Transform>())
            t.gameObject.tag = "Grab";
        switch (axe)
        {
            case Door.FaceAxe.Y:
                rotationAxe = transform.up;
                break;
            case Door.FaceAxe.X:
                rotationAxe = transform.right;
                break;
            case Door.FaceAxe.Z:
                rotationAxe = transform.forward;
                break;
            case Door.FaceAxe.invX:
                rotationAxe = -transform.right;
                break;
            case Door.FaceAxe.invY:
                rotationAxe = -transform.up;
                break;
            case Door.FaceAxe.invZ:
                rotationAxe = -transform.forward;
                break;
        }
    }
    private void Update()
    {
        delta = Mathf.Lerp(currentRotation, goalRotate, Time.deltaTime * 5) - currentRotation;
        if (delta != 0)
        {
            delta = Mathf.Clamp(delta, minLimit - currentRotation, maxLimit - currentRotation);
            transform.Rotate(-rotationAxe * delta, Space.World);
            //transform.rotation = Quaternion.Euler(Mathf.Lerp(startRotation.eulerAngles.x, transform.rotation.eulerAngles.x, Mathf.Abs(rotationAxe.x)),
            //    Mathf.Lerp(startRotation.eulerAngles.y, transform.rotation.eulerAngles.y, Mathf.Abs(rotationAxe.y)),
            //    Mathf.Lerp(startRotation.eulerAngles.z, transform.rotation.eulerAngles.z, Mathf.Abs(rotationAxe.z)));
            currentRotation += delta;
            if (NearMax(commitRange))
            {
                SetState(1);
                return;
            }
            if (NearMin(commitRange))
            {
                SetState(-1);
                return;
            }
            SetState(0);
        }
    }

    public bool NearMin(float eps)
    {
        return currentRotation <= minLimit + eps;
    }

    public bool NearMax(float eps)
    {
        return currentRotation >= maxLimit - eps;
    }

    public void SetState(float state)
    {
        if (this.state == state)
            return;
        this.state = state;
        OnStateChanged?.Invoke(state);
    }

    public float GetState()
    {
        return state;
    }
    public void SetStuckState(float alState)
    {
        switch (alState)
        {
            case 0:
                isStuck = false;
                break;
            case -1:
                isStuck = true;
                goalRotate = minLimit;
                break;
            case 1:
                isStuck = true;
                goalRotate = maxLimit;
                break;
        }
    }
    void ISave.OnSave(Data data)
    {
        data.BoolKeys.SetValueSafety(this.GetHierarchyPath(), isStuck);
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath(), state);
        OnSave(data);
    }
    void ISave.OnLoad(Data data)
    {
        data.FloatKeys.TryGetValue(this.GetHierarchyPath(), out state, state);
        if (data.BoolKeys.TryGetValue(this.GetHierarchyPath(), out isStuck, isStuck))
            SetStuckState(state);
        OnLoad(data);
    }
}

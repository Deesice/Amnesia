using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Interactable, IState
{
    HingeJoint joint;
    public enum AutoMoveGoal { Middle, Min, Max, None}
    public AutoMoveGoal autoMoveGoal;
    [Range(-1, 1)]
    public float middleAngleAmount;
    float state;
    bool isStuck;
    public event Action<float> OnStateChanged;
    /////////
    [HideInInspector] public string stuckState;
    protected override void ApplyHandContent(Rigidbody hand)
    {
        this.hand = hand;
        joint.useSpring = true;

        var s = joint.spring;
        s.targetPosition = joint.angle;
        joint.spring = s;
    }

    public override void DenyHand()
    {
        hand = null;

        if (isStuck)
            return;

        if (autoMoveGoal != AutoMoveGoal.None)
        {
            joint.useSpring = true;
            var s = joint.spring;
            switch (autoMoveGoal)
            {
                case AutoMoveGoal.Middle:
                    s.targetPosition = middleAngleAmount * joint.limits.max;
                    break;
                case AutoMoveGoal.Max:
                    s.targetPosition = joint.limits.max;
                    break;
                case AutoMoveGoal.Min:
                    s.targetPosition = joint.limits.min;
                    break;
            }
            joint.spring = s;
            if (s.targetPosition == joint.limits.min)
            {
                SetState(-1);
                return;
            }
            if (s.targetPosition == joint.limits.max)
            {
                SetState(1);
                return;
            }
            SetState(0);
        }
        else
            joint.useSpring = false;
    }

    public override void RecieveFingerDelta(Vector2 mouseInput, Vector2 joystickInput)
    {
        if (isStuck)
            return;

        var s = joint.spring;
        s.targetPosition -= mouseInput.y;
        s.targetPosition = Mathf.Clamp(s.targetPosition, joint.limits.min, joint.limits.max);
        joint.spring = s;
        if (s.targetPosition == joint.limits.min)
        {
            SetState(-1);
            return;
        }
        if (s.targetPosition == joint.limits.max)
        {
            SetState(1);
            return;
        }
        SetState(0);
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (joint == null)
            joint = GetComponent<HingeJoint>();
        gameObject.tag = "Grab";
        blockView = true;
        DenyHand();
        switch (stuckState.ToLower())
        {
            case "max":
                SetStuckState(1);
                break;
            case "min":
                SetStuckState(-1);
                break;
            default:
                break;
        }
    }
    public void SetStuckState(float alState)
    {
        if (joint == null)
            joint = GetComponent<HingeJoint>();
        isStuck = false;
        switch (alState)
        {
            case 0:
                joint.useSpring = false;
                break;
            default:
                RecieveFingerDelta(alState * Vector3.up * 100, Vector3.zero);
                joint.useSpring = true;
                isStuck = true;
                break;
        }
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
    void ISave.OnSave(Data data)
    {
        data.BoolKeys.SetValueSafety(this.GetHierarchyPath(), isStuck);
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath(), state);
        OnSave(data);
    }
    void ISave.OnLoad(Data data)
    {
        data.FloatKeys.TryGetValue(this.GetHierarchyPath(), out state, state);
        data.BoolKeys.TryGetValue(this.GetHierarchyPath(), out isStuck, isStuck);
        if (isStuck)
            SetStuckState(state);

        OnLoad(data);
    }
}

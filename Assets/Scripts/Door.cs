using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door : Interactable, ISave
{
    public enum FaceAxe { X, Y, Z, invX, invY, invZ }
    public FaceAxe face = FaceAxe.Y;
    public bool _locked;
    public float openAmount;
    public bool locked { get { return _locked; } set {
            if (_locked != value)
            {
                _locked = value;
                if (value)
                {
                    if (lockOnSound)
                        SoundManager.PlaySoundAtEntity(root.name + lockOnSound.name, lockOnSound.name, root.name, 0);
                }
                else
                {
                    if (lockOffSound)
                        SoundManager.PlaySoundAtEntity(root.name + lockOffSound.name, lockOnSound.name, root.name, 0);
                }
            }
        } }
    bool horizontalDoor;
    bool mirrored;

    HingeJoint joint;
    Quaternion startRotation;

    [HideInInspector] public bool AutoClose = true;
    public AudioClip closeOnSound;
    public AudioClip closeOffSound;
    public AudioClip lockOnSound;
    public AudioClip lockOffSound;
    public AudioClip interactLockedSound;
    float currentAngle
    {
        get { return Quaternion.Angle(startRotation, joint.transform.rotation) * (mirrored ? -1 : 1); }
        set {
            if (!horizontalDoor)
            {
                value -= currentAngle;
                Vector3 actualAxis = joint.axis.x * joint.transform.right +
                    joint.axis.y * joint.transform.up +
                    joint.axis.z * joint.transform.forward;
                actualAxis = actualAxis.normalized;
                Vector3 offset = joint.anchor.x * joint.transform.right +
                    joint.anchor.y * joint.transform.up +
                    joint.anchor.z * joint.transform.forward;
                offset.y = 0;
                joint.transform.RotateAround(offset + joint.transform.position, actualAxis * (mirrored ? -1 : 1), value);
            }
        }
        
    }
    private void Update()
    {
        if (AutoClose && IsNearToClose())
            SetClosed(true, true);
    }
    public void SetClosed(bool abClosed, bool abEffect)
    {
        if (abClosed && enabled)
        {
            if (closeOnSound != null && !locked && abEffect)
                SoundManager.PlaySoundAtEntity(root.name + closeOnSound.name, closeOnSound.name, root.name, 0);
            joint.useSpring = true;
            var s = joint.spring;
            s.targetPosition = mirrored ? joint.limits.max : joint.limits.min;
            joint.spring = s;
            enabled = false;
            return;
        }
        else if (!abClosed && !enabled)
        {
            if (closeOffSound != null && !locked && abEffect)
                SoundManager.PlaySoundAtEntity(root.name + closeOffSound.name, closeOffSound.name, root.name, 0);
            joint.useSpring = false;
            enabled = true;
        }
        if (locked && interactLockedSound != null)
            SoundManager.PlaySoundAtEntity(root.name + interactLockedSound.name, interactLockedSound.name, root.name, 0);
    }
    Vector3 forward { get {
            switch (face)
            {
                case FaceAxe.Y:
                    return joint.transform.up * (mirrored ? 1 : -1);
                case FaceAxe.X:
                    return joint.transform.right * (mirrored ? 1 : -1);
                case FaceAxe.Z:
                    return joint.transform.forward * (mirrored ? 1 : -1);
                case FaceAxe.invX:
                    return joint.transform.right * (mirrored ? 1 : -1) * -1;
                case FaceAxe.invY:
                    return joint.transform.up * (mirrored ? 1 : -1) * -1;
                default:
                    return Vector3.zero;
            }
        } }
    protected override void ApplyHandContent(Rigidbody hand)
    {
        SetClosed(false, true);
        AutoClose = false;
        this.hand = hand;
        joint.useSpring = true;
        var s = joint.spring;
        s.targetPosition = currentAngle;
        joint.spring = s;        
    }

    public override void DenyHand()
    {
        AutoClose = true;
        hand = null;
        if (locked)
        {
            var s = joint.spring;
            s.targetPosition = 0;
            joint.spring = s;
        }
        joint.useSpring = locked;
        
    }

    public override void RecieveFingerDelta(Vector2 mouseInput, Vector2 joystickInput)
    {
        mouseInput += joystickInput * 2;
        var v = hand.transform.forward;
        if (horizontalDoor)
        {
            v = Vector3.up;
        }
        else
            v.y = 0;
        v.Normalize();
        var handDelta = -v * mouseInput.y + hand.transform.right * mouseInput.x;
        var s = joint.spring;
        s.targetPosition += Vector3.Dot(forward, handDelta);
        s.targetPosition = Mathf.Clamp(s.targetPosition, joint.limits.min, joint.limits.max * (locked ? 0.05f: 1));
        joint.spring = s;
    }

    void Awake()
    { 
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (!t.gameObject.isStatic)
                t.gameObject.tag = "Grab";
        blockView = true;
        if (joint == null)
            joint = GetComponentInChildren<HingeJoint>(true);
        startRotation = joint.transform.rotation;

        mirrored = (Mathf.Abs(joint.spring.targetPosition - joint.limits.max) < Mathf.Abs(joint.spring.targetPosition - joint.limits.min));

        if (Vector3.Angle(forward, Vector3.up) < 10 || Vector3.Angle(forward, Vector3.down) < 10)
            horizontalDoor = true;

        if (openAmount != 0)
        {
            SetClosed(false, false);
            AutoClose = false;
            currentAngle = Mathf.Lerp(joint.limits.min, joint.limits.max, openAmount);
        }
        if (AutoClose && Mathf.Abs(currentAngle) <= Mathf.Abs(Mathf.Lerp(joint.limits.min, joint.limits.max, mirrored ? 0.9f : 0.1f)))
            SetClosed(true, false);
    }
    public bool IsNearToClose()
    {
        return Mathf.Abs(currentAngle) <= Mathf.Abs(Mathf.Lerp(joint.limits.min, joint.limits.max, mirrored ? 0.9f : 0.1f));
    }
    void ISave.OnSave(Data data)
    {
        if (joint == null)
            joint = GetComponentInChildren<HingeJoint>(true);
        data.BoolKeys.SetValueSafety(this.GetHierarchyPath(), _locked);
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath(), currentAngle);
        OnSave(data);
    }
    void ISave.OnLoad(Data data)
    {
        if (joint == null)
            joint = GetComponentInChildren<HingeJoint>(true);
        data.BoolKeys.TryGetValue(this.GetHierarchyPath(), out _locked, _locked);
        if (data.FloatKeys.TryGetValue(this.GetHierarchyPath(), out var f, currentAngle) && f != 0)
        {
            SetClosed(false, false);
            AutoClose = false;
            currentAngle = f;
        }
        OnLoad(data);
    }
}

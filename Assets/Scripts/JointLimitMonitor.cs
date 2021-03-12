using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointLimitMonitor : MonoBehaviour, ILimit
{
    HingeJoint joint;
    private void Awake()
    {
        joint = GetComponent<HingeJoint>();
    }
    public bool NearMax(float eps)
    {
        return joint.angle >= joint.limits.max - eps;
    }

    public bool NearMin(float eps)
    {
        return joint.angle <= joint.limits.min + eps;
    }
}

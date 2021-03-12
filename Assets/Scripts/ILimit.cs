using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILimit
{
    public bool NearMin(float eps);
    public bool NearMax(float eps);
}

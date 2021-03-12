using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEntityActiveCache : MonoBehaviour, ISave
{
    public void OnLoad(Data data)
    {
        if (data.BoolKeys.TryGetValue(this.GetHierarchyPath(), out var b, true))
            gameObject.SetActive(b);
    }

    public void OnSave(Data data)
    {
        data.BoolKeys.SetValueSafety(this.GetHierarchyPath(), gameObject.activeSelf);
    }
}

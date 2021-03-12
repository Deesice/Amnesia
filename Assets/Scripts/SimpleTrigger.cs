using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleTrigger : MonoBehaviour, ISave
{
    public string target;
    public string Callback;
    public bool isInverted;
    public bool destroyAfterTrigger = true;
    public int mode = 1;

    public delegate void TriggerCallback(string asParent, string asChild, int alState);
    public event TriggerCallback OnTriggered;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == target && mode >= 0 && !string.IsNullOrEmpty(Callback))
        {
            var m = Type.GetType(Scenario.currentScenario.ClassName).GetMethod(Callback);
            if (isInverted)
                m.Invoke(Scenario.currentScenario, new object[] { other.gameObject.name, gameObject.name, 1 });
            else
                m.Invoke(Scenario.currentScenario, new object[] { gameObject.name, other.gameObject.name, 1 });
            if (destroyAfterTrigger)
                Callback = "";
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == target && mode <= 0 && !string.IsNullOrEmpty(Callback))
        {
            var m = Type.GetType(Scenario.currentScenario.ClassName).GetMethod(Callback);
            if (isInverted)
                m.Invoke(Scenario.currentScenario, new object[] { other.gameObject.name, gameObject.name, -1 });
            else
                m.Invoke(Scenario.currentScenario, new object[] { gameObject.name, other.gameObject.name, -1 });
            if (destroyAfterTrigger)
                Callback = "";
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == target && mode >= 0 && !string.IsNullOrEmpty(Callback))
        {
            var m = Type.GetType(Scenario.currentScenario.ClassName).GetMethod(Callback);
            if (isInverted)
                m.Invoke(Scenario.currentScenario, new object[] { other.gameObject.name, gameObject.name, 1 });
            else
                m.Invoke(Scenario.currentScenario, new object[] { gameObject.name, other.gameObject.name, 1 });
            if (destroyAfterTrigger)
                Callback = "";
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.name == target && mode <= 0 && !string.IsNullOrEmpty(Callback))
        {
            var m = Type.GetType(Scenario.currentScenario.ClassName).GetMethod(Callback);
            if (isInverted)
                m.Invoke(Scenario.currentScenario, new object[] { other.gameObject.name, gameObject.name, -1 });
            else
                m.Invoke(Scenario.currentScenario, new object[] { gameObject.name, other.gameObject.name, -1 });
            if (destroyAfterTrigger)
                Callback = "";
        }
    }

    public void OnLoad(Data data)
    {
        data.StringKeys.TryGetValue(this.GetHierarchyPath() + "/" + nameof(target), out target, target);
        data.StringKeys.TryGetValue(this.GetHierarchyPath() + "/" + nameof(Callback), out Callback, Callback);
        data.BoolKeys.TryGetValue(this.GetHierarchyPath() + "/" + nameof(isInverted), out isInverted, isInverted);
        data.BoolKeys.TryGetValue(this.GetHierarchyPath() + "/" + nameof(destroyAfterTrigger), out destroyAfterTrigger, destroyAfterTrigger);
        data.IntKeys.TryGetValue(this.GetHierarchyPath() + "/" + nameof(mode), out mode, mode);
    }

    public void OnSave(Data data)
    {
        data.StringKeys.SetValueSafety(this.GetHierarchyPath() + "/" + nameof(target), target);
        data.StringKeys.SetValueSafety(this.GetHierarchyPath() + "/" + nameof(Callback), Callback);
        data.BoolKeys.SetValueSafety(this.GetHierarchyPath() + "/" + nameof(isInverted), isInverted);
        data.BoolKeys.SetValueSafety(this.GetHierarchyPath() + "/" + nameof(destroyAfterTrigger), destroyAfterTrigger);
        data.IntKeys.SetValueSafety(this.GetHierarchyPath() + "/" + nameof(mode), mode);
    }
}

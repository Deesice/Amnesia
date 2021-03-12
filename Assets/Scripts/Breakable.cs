using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : ReflectableMonoBehaviour, ISave
{
    public GameObject breakVariant;
    public float maxImpulse = 15;
    public string Callback;
    public string breakSound;
    public string breakParticleSystem;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > maxImpulse)
        {
            Break();
        }
    }
    public void Break()
    {
        var pos = transform.position;
        var q = transform.rotation;
        var t = transform.parent;
        if (breakVariant != null)
        {
            var g = Instantiate(breakVariant, pos, q);
            g.transform.SetParent(t);
            g.name = gameObject.name + "_broken";

            if (!string.IsNullOrEmpty(breakSound))
                SoundManager.PlaySoundAtEntity(g.name + "_sound", breakSound, g.name, 0);

            if (!string.IsNullOrEmpty(breakParticleSystem))
                Scenario.currentScenario.CreateParticleSystemAtEntity(g.name + "_breakPS", breakParticleSystem, g.name, false);

            if (!string.IsNullOrEmpty(Callback))
            {
                var m = Type.GetType(Scenario.currentScenario.ClassName).GetMethod(Callback);
                m.Invoke(Scenario.currentScenario, new object[] { gameObject.name, "" });
                Callback = "";
            }

            gameObject.SetActive(false);
        }
        else
        {
            if (!string.IsNullOrEmpty(breakSound))
                SoundManager.PlaySoundAtEntity(gameObject.name + "_sound", breakSound, gameObject.name, 0);

            if (!string.IsNullOrEmpty(breakParticleSystem))
                Scenario.currentScenario.CreateParticleSystemAtEntity(gameObject.name + "_breakPS", breakParticleSystem, gameObject.name, false);

            if (!string.IsNullOrEmpty(Callback))
            {
                var m = Type.GetType(Scenario.currentScenario.ClassName).GetMethod(Callback);
                m.Invoke(Scenario.currentScenario, new object[] { gameObject.name, "" });
                Callback = "";
            }

            if (!string.IsNullOrEmpty(breakSound))
                Scenario.currentScenario.SetPropActiveAndFade(gameObject.name, false, FakeDatabase.FindProperty(breakSound).GetClip().length);
            else
                Scenario.currentScenario.SetPropActiveAndFade(gameObject.name, false, 2);
        }
    }
    void ISave.OnLoad(Data data)
    {
        if (data.VectorKeys.TryGetValue(this.GetHierarchyPath() + "/position", out var pos, Vector3.zero))
            transform.position = pos;

        if (data.VectorKeys.TryGetValue(this.GetHierarchyPath() + "/rotation", out var rot, Vector3.zero))
            transform.rotation = Quaternion.Euler(rot);

        data.StringKeys.TryGetValue(this.GetHierarchyPath(), out Callback, Callback);

        if (data.BoolKeys.TryGetValue(this.GetHierarchyPath(), out var b, true) && !b)
            Break();
    }

    void ISave.OnSave(Data data)
    {
        data.BoolKeys.SetValueSafety(this.GetHierarchyPath(), gameObject.activeSelf);
        data.StringKeys.SetValueSafety(this.GetHierarchyPath(), Callback);
        data.VectorKeys.SetValueSafety(this.GetHierarchyPath() + "/position", transform.position);
        data.VectorKeys.SetValueSafety(this.GetHierarchyPath() + "/rotation", transform.rotation.eulerAngles);
    }
}

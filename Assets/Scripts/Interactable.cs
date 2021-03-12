using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
public abstract class Interactable : ReflectableMonoBehaviour, ISave
{
    [HideInInspector] [SerializeField] public GameObject root;
    public string Callback;
    public bool removeCallbackAfterInteraction = true;
    public bool isGrabbed { get { return hand != null; } }
    [HideInInspector] public bool blockView = false;
    [HideInInspector] public bool disableDenying = false;
    protected Rigidbody hand = null;
    public void ApplyHand(Rigidbody hand)
    {
        ApplyHandContent(hand);
        if (string.IsNullOrEmpty(Callback))
            return;
        var m = Type.GetType(Scenario.currentScenario.ClassName).GetMethod(Callback);

        if (m == null)
        {
            Debug.Log(Callback + " not exist in script");
            if (removeCallbackAfterInteraction)
                Callback = "";
            return;
        }

        if (removeCallbackAfterInteraction)
            Callback = "";

        if (root == null)
            root = gameObject;

        if (m.GetParameters().Length == 1)
        {
            m.Invoke(Scenario.currentScenario, new object[] { root.name });
            return;
        }
        if (m.GetParameters().Length == 2 && this as PickUpObject && (this as PickUpObject).item.SubType == Item.EntitySubType.Diary)
        {
            m.Invoke(Scenario.currentScenario, new object[] { root.name, Journal.GetDiaryIdx() });
            return;
        }
        if (m.GetParameters().Length == 2)
        {
            m.Invoke(Scenario.currentScenario, new object[] { root.name, "" });
        }
    }
    public void SetCallback(string s)
    {
        Callback = s;
        if (gameObject.layer == 2)
            gameObject.layer = 0;
    }
    protected abstract void ApplyHandContent(Rigidbody hand);
    public abstract void DenyHand();
    public abstract void RecieveFingerDelta(Vector2 mouseInput, Vector2 joystickInput);
    public void RecordRoot()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            root = UnityEditor.PrefabUtility.GetNearestPrefabInstanceRoot(this);
            if (root == null)
                root = gameObject;
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }
        else
            root = gameObject;
#else
        root = gameObject;
#endif
        //UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
    }

    public void OnLoad(Data data)
    {
        data.StringKeys.TryGetValue(this.GetHierarchyPath(), out Callback, Callback);
        if (!string.IsNullOrEmpty(Callback) && gameObject.layer == 2)
            gameObject.layer = 0;
        data.BoolKeys.TryGetValue(this.GetHierarchyPath(), out removeCallbackAfterInteraction, removeCallbackAfterInteraction);
        if (data.BoolKeys.TryGetValue(this.GetHierarchyPath() + "/enabled", out var b, true))
            enabled = b;
    }

    public void OnSave(Data data)
    {
        data.StringKeys.SetValueSafety(this.GetHierarchyPath(), Callback);
        data.BoolKeys.SetValueSafety(this.GetHierarchyPath(), removeCallbackAfterInteraction);
        data.BoolKeys.SetValueSafety(this.GetHierarchyPath() + "/enabled", enabled);
    }
}

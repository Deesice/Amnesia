using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : ReflectableMonoBehaviour, ISave
{
    public string textCategory;
    public string textEntry;
    List<GameObject> mySons = new List<GameObject>();
    public string playerLookAtCallback;
    public bool autoRemove = true;
    bool isShowingNow;

    private void Start()
    {
        if (gameObject.layer == 2)
            gameObject.layer = 0;
        PlayerController.instance.OnScanChanged += Handler;
        foreach (var t in GetComponentsInChildren<Transform>())
            mySons.Add(t.gameObject);
        Handler(PlayerController.instance.scannedObject);
    }
    public void Handler(GameObject scan)
    {
        if (mySons.Contains(scan))
        {
            isShowingNow = true;
            var message = LangAdapter.FindEntry(textCategory, textEntry);
            if (!string.IsNullOrEmpty(message))
                SignController.On(message);

            if (!string.IsNullOrEmpty(playerLookAtCallback))
            {
                Debug.Log("Invoking " + playerLookAtCallback);
                var m = Type.GetType(Scenario.currentScenario.ClassName).GetMethod(playerLookAtCallback);
                m.Invoke(Scenario.currentScenario, new object[] { gameObject.name, 1 });
            }

            if (autoRemove)
                playerLookAtCallback = "";

        }
        else if (isShowingNow)
        {
            isShowingNow = false;
            if (!string.IsNullOrEmpty(playerLookAtCallback))
            {
                var m = Type.GetType(Scenario.currentScenario.ClassName).GetMethod(playerLookAtCallback);
                m.Invoke(Scenario.currentScenario, new object[] { gameObject.name, -1 });
            }

            if (autoRemove)
                playerLookAtCallback = "";
        }
    }

    public void OnLoad(Data data)
    {
        data.StringKeys.TryGetValue(this.GetHierarchyPath(), out playerLookAtCallback, playerLookAtCallback);
        data.BoolKeys.TryGetValue(this.GetHierarchyPath(), out autoRemove, autoRemove);
    }

    public void OnSave(Data data)
    {
        data.StringKeys.SetValueSafety(this.GetHierarchyPath(), playerLookAtCallback);
        data.BoolKeys.SetValueSafety(this.GetHierarchyPath(), autoRemove);
    }
}

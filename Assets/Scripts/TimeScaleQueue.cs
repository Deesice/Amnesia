using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleQueue
{
    static List<Action> actions = new List<Action>();
    public static void AddFunc(Action action)
    {
        if (Time.timeScale == 0)
            actions.Add(action);
        else
            action.Invoke();
    }
    public static void Invoke()
    {
        foreach (var i in actions)
            i.Invoke();
        actions.Clear();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartInvoke : MonoBehaviour
{
    public delegate bool MyPredicate();
    static SmartInvoke _instance;
    static List<Tuple<object, IEnumerator>> coroutines = new List<Tuple<object, IEnumerator>>();
    static SmartInvoke instance
    {
        get {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<SmartInvoke>();
            if (_instance == null)
            {
                _instance = new GameObject().AddComponent<SmartInvoke>();
                _instance.gameObject.name = "SmartInvoke";
            }
            return _instance; }
    }
    public static void Invoke(Action action, float time)
    {
        var cor = instance.InvokeCoroutine(action, time);
        coroutines.Add(new Tuple<object, IEnumerator>(action.Target, cor));
        instance.StartCoroutine(cor);
    }
    public static void CancelInvoke(object target)
    {
        foreach (var i in coroutines.FindAll((t) => t.Item1 == target))
            instance.StopCoroutine(i.Item2);
    }
    public static void ResumeInvoke(object target)
    {
        foreach (var i in coroutines.FindAll((t) => t.Item1 == target))
            i.Item2.MoveNext();
    }
    public static void WhenTrue(MyPredicate p, Action a)
    {
        instance.StartCoroutine(instance.WhenTrueCoroutine(p, a));
    }
    IEnumerator InvokeCoroutine(Action action, float time)
    {
        if (time <= 0)
            yield return null;
        else
            yield return new WaitForSecondsRealtime(time);
        action.Invoke();
    }
    IEnumerator WhenTrueCoroutine(MyPredicate p, Action a)
    {
        while (!p.Invoke())
            yield return null;
        a.Invoke();
    }
}

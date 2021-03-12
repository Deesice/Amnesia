using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignController : MonoBehaviour
{
    static SignController instance;
    Text text;
    Fader fader;
    bool flag;
    void Awake()
    {
        text = GetComponent<Text>();
        fader = GetComponent<Fader>();
        instance = this;
        PlayerController.instance.OnScanChanged += (g) => flag = false;
    }
    private void LateUpdate()
    {
        if (!flag)
            Off();
        flag = true;
    }
    public static void On(string entry)
    {
        instance.flag = true;
        instance.text.text = entry.Replace("\\n", "\n");
        instance.fader.FadeOn(Color.white, 0.5f);
    }
    public static void Off()
    {
        instance.fader.FadeOff(0.5f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviour
{
    static Message instance;
    Text text;
    Fader fader;
    void Start()
    {
        text = GetComponent<Text>();
        fader = GetComponent<Fader>();
        instance = this;
    }

    public static void ShowMessage(string message, float time = 0)
    {
        instance.CancelInvoke();
        instance.text.text = message.Replace("\\n", "\n");
        instance.fader.FadeOn(Color.white);
        if (time <= 0)
            time = message.Length * 0.1f;
        instance.Invoke(nameof(HideMessage), time);
    }

    void HideMessage()
    { 
        instance.fader.FadeOff();
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{
    static List<string> blockedHints = new List<string>();
    static List<string> showedHints = new List<string>();
    static Hint instance;
    public Fader titleFader;
    public UIShaderController titleShader;
    Text text;
    Fader fader;
    void Start()
    {
        text = GetComponent<Text>();
        fader = GetComponent<Fader>();
        instance = this;
    }

    public static void ShowHint(string name, string message, float time = 0)
    {
        if (blockedHints.Contains(name) || showedHints.Contains(name))
            return;
        showedHints.Add(name);
        instance.CancelInvoke();
        instance.text.text = message.Replace("\\n", "\n");
        instance.fader.FadeOn(Color.white);
        instance.titleFader.FadeOn(Color.white);
        instance.titleShader.enabled = true;
        if (time <= 0)
            time = message.Length * 0.1f;
        instance.Invoke(nameof(FadeOn), time);
    }
    void FadeOn()
    {
        instance.fader.FadeOff();
        instance.titleFader.FadeOff();
        SmartInvoke.Invoke(() => titleShader.enabled = false, 1);
    }
    public static void BlockHint(string asName)
    { 
        blockedHints.Add(asName);
    }
    public static void UnBlockHint(string asName)
    {
        if(blockedHints.Contains(asName))
            blockedHints.Remove(asName);
    }
    public static void RemoveHint(string asName)
    {
        if (showedHints.Contains(asName))
            showedHints.Remove(asName);
    }
}

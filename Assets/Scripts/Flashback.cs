using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashback : ReflectableMonoBehaviour, ISave
{
    public List<string> voices = new List<string>();
    public List<string> textEntries = new List<string>();
    public AudioClip sfx;
    public string callback;
    SoundProperty property;

    void Start()
    {
        property = FakeDatabase.FindProperty("flashback_flash");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            SmartInvoke.WhenTrue(() => !Scenario.currentScenario.GetEffectVoiceActive(),() =>
            {
                float blurSize = 0.09f;
                SoundManager.ManipulateAudioListener(0.3f, 0.7f / 1.5f);
                SmartInvoke.Invoke(() =>
                {
                    PlayerController.instance.baseMultiplierProperty.runSpeedMul = 0.6f;
                    PlayerController.instance.baseMultiplierProperty.moveSpeedMul = 0.6f;
                    Scenario.currentScenario.StartEffectFlash(0.5f, 0.5f, 2.5f);
                    SoundManager.PlaySound(property);
                    SoundManager.PlayClip(sfx);
                }, 0.5f);
                SmartInvoke.Invoke(() =>
                {
                    for (int i = 0; i < voices.Count; i++)
                    {
                        Scenario.currentScenario.AddEffectVoice(voices[i], "", "Flashbacks", textEntries[i], false, "", 0, 0);
                    }
                    if (!string.IsNullOrEmpty(callback))
                        Scenario.currentScenario.SetEffectVoiceOverCallback(callback);

                    SepiaController.FadeSepiaColorTo(1, 1.0f / 3.5f);

                    BlurController.FadeRadialBlurTo(blurSize, blurSize / 3.5f);
                    RadialBlurController.SetRadialBlurStartDist(0.3f);
                }, 1.5f);
                SmartInvoke.Invoke(() =>
                {
                    PlayerController.instance.baseMultiplierProperty.runSpeedMul = 1;
                    PlayerController.instance.baseMultiplierProperty.moveSpeedMul = 1;
                    SoundManager.ManipulateAudioListener(1, 0.25f);
                    BlurController.FadeRadialBlurTo(0, blurSize / 4);
                    Scenario.currentScenario.StartEffectFlash(1, 0.1f, 2.5f);
                    SepiaController.FadeSepiaColorTo(0, 0.25f);
                }, sfx.length);
            });
            gameObject.SetActive(false);
        }        
    }

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering.PostProcessing;
using System.Threading.Tasks;

public class SanitySystem : MonoBehaviour
{
    public AudioClip damageClip;
    public AudioClip gainClip;
    public AnimationCurve curve;
    public Material effectMaterial;
    public PostProcessVolume volume;
    public SoundLibrary[] drainSounds;

    public static SanitySystem instance;
    public static float sanityAmount = 100;
    public int SanityLevel { get
        {
            var j = 4;
            float h = sanityAmount;
            while (h > 0)
            {
                h -= 25;
                j--;
            }
            return j;
        } }
    IEnumerable<Light> allLights;
    IEnumerable<BoxLight> boxLights;
    Light darknessLight;
    const float maxDaknessLightIntensity = 0.32f;
    bool _inDark;
    public bool InDark { get { return _inDark; } set {            
            if (value != _inDark)
            {
                drainLastTime = Time.time;
                _inDark = value;
                if (coroutine != null)
                    StopCoroutine(coroutine);
                coroutine = SendMagnitudeToShader();
                StartCoroutine(coroutine);
            }
        } }
    IEnumerator coroutine = null;
    IEnumerator booCoroutine = null;
    float drainLastTime;
    public bool setSanityDrainDisabled;
    IEnumerator SendMagnitudeToShader()
    {
        var currentMagnitude = effectMaterial.GetFloat("_Magnitude");
        var currentIntensity = darknessLight.intensity;
        float i = 0;
        while (i < 1)
        {
            effectMaterial.SetFloat("_Magnitude", Mathf.Lerp(currentMagnitude, _inDark ? 1 : 0, i));
            darknessLight.intensity = Mathf.Lerp(currentIntensity, _inDark ? maxDaknessLightIntensity : 0, i);
            yield return null;
            i += Time.deltaTime / 3;
        }
        effectMaterial.SetFloat("_Magnitude", _inDark ? 1 : 0);
        darknessLight.intensity = _inDark ? maxDaknessLightIntensity : 0;
        coroutine = null;
    }
    IEnumerator BooCoroutine()
    {
        SoundManager.PlayClip(damageClip);
        float i = 0;
        while (i < 1)
        {
            volume.weight = curve.Evaluate(i);
            yield return null;
            i += Time.deltaTime / 2;
        }
        volume.weight = 0;
        booCoroutine = null;
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, effectMaterial);
    }
    private void Awake()
    {
        effectMaterial.SetFloat("_Magnitude", 0);
        instance = this;
        darknessLight = GetComponent<Light>();
        allLights = FindObjectsOfType<Light>(true).Where(l => !l.gameObject.CompareTag("BoxLight") && l != darknessLight);
        boxLights = FindObjectsOfType<BoxLight>(true);
        StartCoroutine(UpdateDarknessState(0.5f));
    }
    private void Update()
    {
        if (InDark)
        {
            GiveSanityDamage(Time.deltaTime / 5, false);
            if ((Time.time - drainLastTime) > 2)
            {
                SoundManager.PlaySound(drainSounds[0]);
                drainLastTime = Time.time;
            }
        }
        else
        {
            GiveSanityDamage(-Time.deltaTime / 5, false);
        }
    }
    IEnumerator UpdateDarknessState(float freqTime)
    {
        while(true)
        {
            yield return new WaitForSeconds(freqTime);
            if (!setSanityDrainDisabled)
                InDark = CheckInDarkness();
        }
    }
    public static void OnEnterInsanityArea()
    {

    }
    public static void GiveSanityDamage(float value, bool useEffects)
    {
        sanityAmount -= value;
        if (sanityAmount > 100)
            sanityAmount = 100;
        else if (sanityAmount < 0)
            sanityAmount = 0;
        if (value > 0 && useEffects)
        {
            if (instance.booCoroutine != null)
                instance.StopCoroutine(instance.booCoroutine);
            instance.booCoroutine = instance.BooCoroutine();
            instance.StartCoroutine(instance.booCoroutine);
        }
        if (value < 0 && useEffects)
        {
            SmartInvoke.WhenTrue(() => !PlayerController.instance.FreezeMovement, async () =>
            {
                PlayerController.instance.fader.FadeOn(new Color(0.3f, 0.3f, 0.5f), 0.25f);
                await Task.Delay(250);
                PlayerController.instance.fader.FadeOff(1.25f);
            });
            SoundManager.PlayClip(instance.gainClip);
        }
    }
    bool CheckInDarkness()
    {
        if (Lantern.IsLit)
            return false;
        float fAmount;
        float fLightLevel = 0;
        Vector3 toPlayer;
        float fT;
        foreach (var i in boxLights)
        {
            fLightLevel += i.addedValue;
            if (fLightLevel > 0.15f)
                return false;
        }
        foreach (var i in allLights)
        {
            toPlayer = transform.position - i.transform.position;
            if (i.enabled && i.gameObject.activeInHierarchy && toPlayer.magnitude <= i.range && (i.type == LightType.Point || Vector3.Angle(i.transform.forward, toPlayer) <= i.innerSpotAngle/2))
            {
                //Get highest value of rg b
                fAmount = Mathf.Max(i.color.r, i.color.g, i.color.b);

                //Calculate attenuation
                fT = 1 - toPlayer.magnitude / (i.range + 3);
                if (fT < 0) fT = 0;
                fAmount *= fT;
            }
            else
                fAmount = 0;

            fLightLevel += fAmount;
            if (fLightLevel > 0.15f)
                return false;
        }
        return true;
    }
    public void OnLoad(Data data)
    {
        data.FloatKeys.TryGetValue("SanityAmount", out sanityAmount, 100);
    }

    public void OnSave(Data data)
    {
        data.FloatKeys.SetValueSafety("SanityAmount", sanityAmount);
    }
}

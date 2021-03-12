using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lantern : MonoBehaviour, ISave
{
    Animator animator;
    static Lantern instance;
    public AudioClip onSound;
    public AudioClip offSound;
    public static bool IsLit { get {
            if (instance.animator == null)
                instance.animator = instance.GetComponent<Animator>();
            var state = instance.animator.GetCurrentAnimatorStateInfo(0);
            prevLitState = state.IsName("Idle") || state.IsName("Draw");
            return prevLitState;
        } }

    float lastSwitchTime;
    public static float Oil = 100;
    static bool prevLitState;
    void Awake()
    {
        animator = GetComponent<Animator>();
        instance = this;
        if (prevLitState)
            On(true);
    }
    public static void AddOil(float value)
    {
        Oil += value;
        if (Oil > 100)
            Oil = 100;
    }
    private void Update()
    {
        if (IsLit && Oil > 0)
        {
            Oil -= Time.deltaTime * 0.23f; // Подсмотрено в .cfg
            if (Oil <= 0)
            {
                Off();
                Oil = 0;
            }
        }
    }

    public static void On(bool withoutSound = false)
    {
        if (!IsLit)
        {
            if (Oil > 0)
            {
                instance.animator.SetTrigger("On");
                if (!withoutSound)
                    SoundManager.PlayClip(instance.onSound, false, 0.8f);
            }
            else if(!withoutSound)
                SoundManager.PlayClip(instance.offSound);
        }
    }
    public static void Off()
    {
        if (IsLit)
        {
            instance.animator.SetTrigger("Off");
            SoundManager.PlayClip(instance.offSound);
        }
    }
    public static void Switch()
    {
        if (Time.time - instance.lastSwitchTime < 1)
            return;

        instance.lastSwitchTime = Time.time;

        if (IsLit)
            Off();
        else
            On();
    }

    public void OnLoad(Data data)
    {
        data.BoolKeys.TryGetValue("Lantern", out prevLitState, prevLitState);
        data.FloatKeys.TryGetValue("Lantern", out Oil, Oil);
        if (prevLitState)
            On(true);
    }

    public void OnSave(Data data)
    {
        data.BoolKeys.SetValueSafety("Lantern", prevLitState);
        data.FloatKeys.SetValueSafety("Lantern", Oil);
    }
}

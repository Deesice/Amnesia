using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour, ISave
{
    public static HealthSystem instance;
    public static float healthAmount = 100;
    public int HealthLevel
    {
        get
        {
            var j = 4;
            float h = healthAmount;
            while (h > 0)
            {
                h -= 25;
                j--;
            }
            return j;
        }
    }
    public static void AddDamage(float value)
    {
        healthAmount -= value;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);
    }

    public void OnLoad(Data data)
    {
        data.FloatKeys.TryGetValue("HealthAmount", out healthAmount, 100);
    }

    public void OnSave(Data data)
    {
        data.FloatKeys.SetValueSafety("HealthAmount", healthAmount);
    }

    void Start()
    {
        instance = this;
    }
    
}

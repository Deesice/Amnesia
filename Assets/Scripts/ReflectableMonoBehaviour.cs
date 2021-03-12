using System.Reflection;
using UnityEngine;

public class ReflectableMonoBehaviour : MonoBehaviour
{
    public object this[string key]
    {
        get
        {
            return this.GetType().GetField(key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(this);
        }
        set
        {
            this.GetType().GetField(key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(this, value);
        }
    }
}

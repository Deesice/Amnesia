using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Subtitle : MonoBehaviour
{
    Text text;
    static Subtitle instance;
    IEnumerator coroutine;
    void Start()
    {
        instance = this;
        text = GetComponent<Text>();
    }

    public static void ShowSubtitle(string message, float time = 0)
    {
        instance.text.text = message;
        if (time != 0)
        {
            if (instance.coroutine != null)
                instance.StopCoroutine(instance.coroutine);
            instance.coroutine = instance.HideSubtitle(time);
            instance.StartCoroutine(instance.coroutine);
        }
    }
    IEnumerator HideSubtitle(float time)
    {
        yield return new WaitForSeconds(time);
        text.text = "";
        instance.coroutine = null;
    }
}

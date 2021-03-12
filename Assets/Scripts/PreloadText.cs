using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreloadText : MonoBehaviour
{
    Text text;
    public string category;
    public string entry;
    void Start()
    {
        text = GetComponent<Text>();
        text.text = LangAdapter.FindEntry(category, entry);
    }
}

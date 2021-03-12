using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class UIButton : MonoBehaviour, IPointerDownHandler
{
    public bool pressedByEscape;
    public AudioClip clip;
    public event Action<string, Item> PointerDownEvent;
    public string Data;
    public Item Item;
    public UIShaderController highlight;
    public UnityEvent events;
    public static UIButton lastButtonPressed;
    static List<UIButton> allInstances = new List<UIButton>();
    private void Start()
    {
        allInstances.Add(this);
        if (highlight == null)
            highlight = GetComponent<UIShaderController>();
        if (highlight != null)
            highlight.enabled = false;
    }
    private void Update()
    {
        if (pressedByEscape && Input.GetKeyDown(KeyCode.Escape))
        {
            if (highlight != null)
                highlight.enabled = false;
            lastButtonPressed = null;
            events.Invoke();
            if (clip != null)
                SoundManager.PlayClip(clip);
            PointerDownEvent?.Invoke(Data, Item);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (lastButtonPressed == this)
        {
            if (highlight != null && (Item != null || events.GetPersistentEventCount() > 0))
                highlight.enabled = false;
            lastButtonPressed = null;
            events.Invoke();
            if (clip != null)
                SoundManager.PlayClip(clip);
            PointerDownEvent?.Invoke(Data, Item);
        }
        else
        {
            lastButtonPressed = this;
            foreach (var b in allInstances)
                if (b.highlight && b != this)
                    b.highlight.enabled = false;
            PointerDownEvent?.Invoke(Data, Item);
            if (highlight != null)
                highlight.enabled = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public static Crosshair instance;
    Image image;
    public Sprite[] sprites;
    public bool puzzleMode;
    UIShaderController highlight;
    void Awake()
    {
        instance = this;
        image = GetComponent<Image>();
        highlight = GetComponent<UIShaderController>();
    }
    public static void ShowCrosshair(bool show)
    {
        instance.image.enabled = show;
    }
    public static void EnablePuzzleMode(Item item)
    {
        if (item)
        {
            instance.image.sprite = item.ImageFile;
            instance.puzzleMode = true;
        }
        else
        {
            instance.puzzleMode = false;
            instance.highlight.enabled = false;            
        }
        if (PlayerController.instance.scannedObject)
            ApplyTagOfFocused(PlayerController.instance.scannedObject.tag);
        else
            ApplyTagOfFocused("");
    }

    public static void ApplyTagOfFocused(string tag)
    {
        if (instance.puzzleMode)
        {
            switch (tag)
            {
                case "Untagged":
                case "":
                    instance.highlight.enabled = false;
                    break;
                default:
                    instance.highlight.enabled = true;
                    break;
            }
            return;
        }
        switch (tag)
        {
            case "Grab":
                instance.image.sprite = instance.sprites[1];
                break;
            case "Ignite":
                instance.image.sprite = instance.sprites[2];
                SignController.On("x " + Inventory.TinderboxCount);
                break;
            case "LevelDoor":
                instance.image.sprite = instance.sprites[3];
                break;
            case "Pick":
                instance.image.sprite = instance.sprites[4];
                break;
            case "Push":
                instance.image.sprite = instance.sprites[5];
                break;
            case "Ladder":
                instance.image.sprite = instance.sprites[6];
                break;
            default:
                instance.image.sprite = instance.sprites[0];
                break;
        }
    }
}

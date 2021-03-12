using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : Interactable
{
    private void Start()
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.tag = "Grab";
            if (PlayerController.instance.scannedObject == t.gameObject)
                Crosshair.ApplyTagOfFocused("Grab");
        }        
    }
    protected override void ApplyHandContent(Rigidbody hand)
    {        
        PlayerController.instance.DenyHandByForce();
        if (removeCallbackAfterInteraction)
            gameObject.layer = 2;
    }

    public override void DenyHand()
    {
    }

    public override void RecieveFingerDelta(Vector2 mouseInput, Vector2 joystickInput)
    {
    }
}

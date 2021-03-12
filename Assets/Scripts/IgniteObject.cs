using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgniteObject : Interactable
{
    LampLitController litController;
    public AudioClip igniteSound;

    // Start is called before the first frame update
    void Start()
    {
        litController = GetComponent<LampLitController>();
        if (litController == null)
            litController = transform.GetComponentInChildren<LampLitController>();
        if (litController == null)
            litController = transform.GetComponentInParent<LampLitController>();
        
        gameObject.tag = litController.lit ? "Untagged" : "Ignite";

        litController.OnLit += (b) => gameObject.tag = b ? "Untagged" : "Ignite";
    }
    protected override void ApplyHandContent(Rigidbody hand)
    {
        if (litController.lit)
        {
            PlayerController.instance.DenyHandByForce();
            return;
        }
        if (Inventory.TinderboxCount > 0)
        {
            SoundManager.PlayClip(igniteSound);
            litController.Relit();
            Inventory.RemoveItem(Item.EntitySubType.Tinderbox);
            PlayerController.instance.DenyHandByForce();
        }
        else
        {
            Message.ShowMessage(LangAdapter.FindEntry("Game", "NoMoreTinderboxes"));
            PlayerController.instance.grabObject = null;
        }
    }

    public override void DenyHand()
    {
    }

    public override void RecieveFingerDelta(Vector2 mouseInput, Vector2 joystickInput)
    {
    }
}

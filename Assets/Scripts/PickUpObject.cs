using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : Interactable, ISave
{
    public Item item;
    Color illumColor = new Color(0.4980392f, 0.4980392f, 1, 1);
    float PlayerIsNear { get
        {
            return Mathf.Clamp(4 - (PlayerController.instance.transform.position - transform.position).magnitude, 0, 1);
        } }
    bool playerLookAtMe;
    List<Material> mat = new List<Material>();
    List<GameObject> allChildrenAndMe = new List<GameObject>();
    public Transform lookTarget;
    private void Awake()
    {
        GetComponentInChildren<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        var m1 = GetComponent<MeshRenderer>();
        if (m1 != null)
            mat.Add(m1.material);
        foreach (var m in GetComponentsInChildren<MeshRenderer>())
            mat.Add(m.material);

        foreach (var m in mat)
        {
            m.EnableKeyword("_EMISSION");
            if (m.GetTexture("_DetailAlbedoMap") != null)
                m.SetTexture("_EmissionMap", m.GetTexture("_DetailAlbedoMap"));
            else
                m.SetTexture("_EmissionMap", m.mainTexture);     
        }
        foreach (var i in GetComponentsInChildren<Transform>(true))
        {
            allChildrenAndMe.Add(i.gameObject);
            i.gameObject.tag = "Pick";
        }

        if (lookTarget == null)
            lookTarget = transform;

        PlayerController.instance.OnScanChanged += (g) =>
        {
            playerLookAtMe = allChildrenAndMe.Contains(g);
            if (playerLookAtMe)
            {
                PlayerController.instance.lookAtProperty.target = lookTarget;
                PlayerController.instance.lookAtProperty.speed = 5;
                PlayerController.instance.lookAtProperty.maxSpeed = 10;
            }
            else if (PlayerController.instance.lookAtProperty.target == lookTarget)
                PlayerController.instance.lookAtProperty.target = null;
        };
    }
    private void Update()
    {
        foreach (var m in mat)
            m.SetColor("_EmissionColor", Color.Lerp(Color.black, illumColor *
                (playerLookAtMe ? 2 : 1) * PlayerIsNear, Mathf.Sin(Time.time * 2) / 4 + 0.75f));
    }
    protected override void ApplyHandContent(Rigidbody hand)
    {
        item.InternalName = root == null ? gameObject.name : root.name;
        Inventory.AddItem(item);
        PlayerController.instance.DenyHandByForce();
        if (item.SubType != Item.EntitySubType.Diary && item.SubType != Item.EntitySubType.Note)
            Message.ShowMessage(LangAdapter.FindEntry("Inventory", "PickedUp") + ": " +
                LangAdapter.FindEntry("Inventory", "ItemName_" + item.NameInLangFile));
        SoundManager.PlayClip(item.PickSound);
        gameObject.SetActive(false);
    }

    public override void DenyHand()
    {
    }

    public override void RecieveFingerDelta(Vector2 mouseInput, Vector2 joystickInput)
    {
    }

    void ISave.OnLoad(Data data)
    {
        if (data.BoolKeys.TryGetValue(this.GetHierarchyPath(), out var b, true))
            gameObject.SetActive(b);
       OnLoad(data);
    }

    void ISave.OnSave(Data data)
    {
        data.BoolKeys.SetValueSafety(this.GetHierarchyPath(), gameObject.activeSelf);
        OnSave(data);
    }
}

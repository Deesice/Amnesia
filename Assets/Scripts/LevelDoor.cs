using UnityEngine;

public class LevelDoor : Interactable, ISave
{
    public string nextLevel;
    public string startPos;
    public string lockedTextCat;
    public string lockedTextEntry;
    public AudioClip openSound;
    public AudioClip closeSound;
    public string lockedSoundProperty;
    public bool locked;
    protected override void ApplyHandContent(Rigidbody hand)
    {
        if (locked)
        {
            Message.ShowMessage(LangAdapter.FindEntry(lockedTextCat, lockedTextEntry));
            //SoundManager.PlayClip(lockedSound, false, lockedVolume);
            Scenario.currentScenario.PlaySoundAtEntity(root.gameObject.name + "_lockedSound", lockedSoundProperty, root.gameObject.name, 0, false);
            PlayerController.instance.DenyHandByForce();
        }
        else
        {
            PlayerController.instance.FreezeMovement = true;
            PlayerController.instance.FreezeRotation = true;
            Inventory.SetInventoryDisabled(true);
            PlayerController.instance.fader.FadeOn(Color.black, 1.9f);
            SoundManager.PlayClip(openSound);
            SmartInvoke.Invoke(() => Scenario.currentScenario.ChangeMap(nextLevel, startPos, null, closeSound), 2);
        }
    }

    public override void DenyHand()
    {
        //throw new System.NotImplementedException();
    }

    public override void RecieveFingerDelta(Vector2 mouseInput, Vector2 joystickInput)
    {
        //throw new System.NotImplementedException();
    }
    void Start()
    {
        foreach (var t in root.GetComponentsInChildren<Transform>())
            t.gameObject.tag = "LevelDoor";
        GetComponentInChildren<Sign>().textCategory = "Levels";
        FakeDatabase.FindProperty(lockedSoundProperty);
        Finder.Find(root.name);
    }
    void ISave.OnSave(Data data)
    {
        data.BoolKeys.SetValueSafety(this.GetHierarchyPath(), locked);
        OnSave(data);
    }
    void ISave.OnLoad(Data data)
    {
        data.BoolKeys.TryGetValue(this.GetHierarchyPath(), out locked, locked);
        OnLoad(data);
    }
}

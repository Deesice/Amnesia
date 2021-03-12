using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, ISave
{
    public Image[] slots;
    public Text[] counts;
    public Text tinderboxCount;
    public Text itemName;
    public Text itemDesc;
    public Image oilAmount;
    public Image healthSlot;
    public Image healthGlow;
    public Image sanitySlot;
    public Image sanityGlow;

    public Image journal;
    public Image oilBorder;

    public Sprite[] healthStage;
    public Sprite[] healthGlowStage;
    public Sprite[] sanityStage;
    public Sprite[] sanityGlowStage;

    public Sprite emptySlot;
    public AudioClip useOilSound;
    public AudioClip useHealthSound;

    List<MaskableGraphic> allUIElements = new List<MaskableGraphic>();
    List<MaskableGraphic> castedElements;
    bool isOpen;
    static Dictionary<Item, int> items = new Dictionary<Item, int>();
    static Inventory instance;
    public static int TinderboxCount { get {
            foreach (var i in items)
                if (i.Key.SubType == Item.EntitySubType.Tinderbox)
                    return i.Value;
            return 0;
        } }
    List<UIButton> uIButtons = new List<UIButton>();
    public Item prevSelectedItem;
    string prevDataHandler;
    bool block;
    private void Awake()
    {
        instance = this;
        foreach (var i in slots)
            uIButtons.Add(i.GetComponent<UIButton>());
        uIButtons.Add(healthGlow.GetComponent<UIButton>());
        uIButtons.Add(sanityGlow.GetComponent<UIButton>());
        uIButtons.Add(journal.GetComponent<UIButton>());
        uIButtons.Add(oilBorder.GetComponent<UIButton>());
        uIButtons.Add(tinderboxCount.GetComponent<UIButton>());

        foreach (var b in uIButtons)
            b.PointerDownEvent += MouseHandler;

        allUIElements.Add(GetComponent<Image>());
        foreach (var i in GetComponentsInChildren<MaskableGraphic>(true))
            allUIElements.Add(i);
        castedElements = allUIElements.FindAll((p) => p.raycastTarget);

        foreach (var i in instance.castedElements)
            i.raycastTarget = false;
        instance.isOpen = false;
        foreach (var j in allUIElements)
            j.color = new Color(1, 1, 1, 0);

    }
    void MouseHandler(string inputString, Item inputItem)
    {
        if (!isOpen)
            return;

        if (prevDataHandler == "Journal" && inputString == "Journal")
        {
            Close();
            Journal.Open(true);            
            return;
        }
        prevDataHandler = inputString;

        int level;
        if (prevSelectedItem == inputItem && prevSelectedItem != null)
        {            
            prevSelectedItem = Use(inputItem);
            inputString = "";
        }
        else
            prevSelectedItem = inputItem;
        switch (inputString)
        {
            case "Health":
                level = HealthSystem.instance.HealthLevel;
                level = 3 - level;
                itemName.text = LangAdapter.FindEntry("Inventory", "Health");
                itemDesc.text = LangAdapter.FindEntry("Inventory", "HealthDesc" + level);
                break;
            case "Sanity":
                level = SanitySystem.instance.SanityLevel;
                level = 3 - level;
                itemName.text = LangAdapter.FindEntry("Inventory", "Sanity");
                itemDesc.text = LangAdapter.FindEntry("Inventory", "SanityDesc" + level);
                break;
            case "Tinderboxes":
                itemName.text = LangAdapter.FindEntry("Inventory", "Tinderboxes");
                itemDesc.text = LangAdapter.FindEntry("Inventory", "TinderboxesDesc");
                break;
            case "Journal":
                itemName.text = LangAdapter.FindEntry("Inventory", "Journal");
                itemDesc.text = LangAdapter.FindEntry("Inventory", "JournalDesc");
                break;
            case "LampOil":
                itemName.text = LangAdapter.FindEntry("Inventory", "LampOil");
                itemDesc.text = LangAdapter.FindEntry("Inventory", "LampOilDesc");
                break;
            case "":
                itemName.text = "";
                itemDesc.text = "";
                break;
            default:
                itemName.text = LangAdapter.FindEntry("Inventory", "ItemName_" + inputString);
                itemDesc.text = LangAdapter.FindEntry("Inventory", "ItemDesc_" + inputString);
                break;
        }
    }
    Item Use(Item input)
    {
        switch (input.SubType)
        {
            case Item.EntitySubType.Health:
                SoundManager.PlayClip(useHealthSound);
                HealthSystem.AddDamage(-25);
                RemoveItem(input);
                input = null;
                break;
            case Item.EntitySubType.LampOil:
                if (Lantern.Oil != 100)
                {
                    SoundManager.PlayClip(useOilSound);
                    Lantern.AddOil(25);
                    RemoveItem(input);
                }
                input = null;
                break;
            case Item.EntitySubType.Lantern:
                if (!PlayerController.instance.BlockLantern)
                {
                    Close();
                    SoundManager.ManipulateAudioListener(1, 0.5f);
                    Lantern.Switch();
                }
                input = null;
                break;
            case Item.EntitySubType.Puzzle:
                Crosshair.EnablePuzzleMode(input);
                Close();
                SoundManager.ManipulateAudioListener(1, 0.5f);
                break;
        }        
        Refresh();
        return input;
    }
    public static void RemoveItem(string item)
    {
        foreach (var i in items)
            if (i.Key.InternalName == item)
            {
                var count = i.Value;
                if (count > 1)
                    items[i.Key]--;
                else
                    items.Remove(i.Key);
                return;
            }
    }
    public static bool HasItem(string name)
    {
        foreach (var i in items)
            if (i.Key.InternalName == name)
                return true;
        return false;
    }
    public static void RemoveItem(Item item)
    {
        items[item]--;
        if (items[item] <= 0)
            items.Remove(item);
    }
    public static void RemoveItem(Item.EntitySubType type)
    {
        foreach (var i in items)
            if (i.Key.SubType == type)
            {
                var count = i.Value;
                if (count > 1)
                    items[i.Key]--;
                else
                    items.Remove(i.Key);
                return;
            }
    }
    public static void AddItem(Item item)
    {
        //Debug.Log("Adding " + item.InternalName + " to inventory");
        if (item.SubType == Item.EntitySubType.Diary)
        {
            Journal.AddDiary(item.NameInLangFile);
            return;
        }
        if (item.SubType == Item.EntitySubType.Note)
        {
            Journal.AddNote(item.NameInLangFile, item.ImageFile);
            return;
        }
        int count = 0;
        if (items.TryGetValue(item, out count))
            items.Remove(item);
        items.Add(item, count + 1);
    }
    async void Refresh()
    {
        tinderboxCount.text = "x 0";
        itemName.text = "";
        itemDesc.text = "";
        int j = 0;
        foreach (var i in items)
        {
            switch (i.Key.SubType)
            {
                case Item.EntitySubType.Tinderbox:
                    tinderboxCount.text = "x " + i.Value;
                    break;
                case Item.EntitySubType.Note:
                case Item.EntitySubType.Diary:
                    break;
                default:
                    uIButtons[j].Data = i.Key.NameInLangFile;
                    uIButtons[j].Item = i.Key;
                    slots[j].sprite = i.Key.ImageFile;
                    if (i.Value > 1)
                        counts[j].text = "x" + i.Value;
                    else
                        counts[j].text = "";
                    j++;
                    break;
            }
        }
        while (j < 18)
        {
            uIButtons[j].Data = "";
            uIButtons[j].Item = null;
            slots[j].sprite = emptySlot;
            counts[j].text = "";
            j++;
        }
        oilAmount.fillAmount = Lantern.Oil / 100;
        j = HealthSystem.instance.HealthLevel;
        healthSlot.sprite = healthStage[j];
        healthGlow.sprite = healthGlowStage[j];
        j = SanitySystem.instance.SanityLevel;
        sanitySlot.sprite = sanityStage[j];
        sanityGlow.sprite = sanityGlowStage[j];
        await Task.Delay(1);
    }
    public static void Open()
    {
        SoundManager.ManipulateAudioListener(0.5f, 0.5f);
        Time.timeScale = 0;
        foreach (var i in instance.castedElements)
            i.raycastTarget = true;
        PlayerController.instance.enabled = false;
        instance.prevSelectedItem = null;
        instance.prevDataHandler = "";
        instance.Refresh();
        instance.isOpen = true;
        instance.StartCoroutine(instance.FadingUI(1));
    }
    public static void Close()
    {        
        Time.timeScale = 1;
        TimeScaleQueue.Invoke();
        foreach (var i in instance.castedElements)
            i.raycastTarget = false;
        PlayerController.instance.enabled = true;
        instance.isOpen = false;
        instance.StartCoroutine(instance.FadingUI(0));
        foreach (var i in instance.uIButtons)
            if(i.highlight)
                i.highlight.enabled = false;
        UIButton.lastButtonPressed = null;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !block && !Journal.isOpen)
        {
            if (isOpen)
            {
                Close();
                SoundManager.ManipulateAudioListener(1, 0.5f);
            }
            else
                Open();
        }
    }
    IEnumerator FadingUI(float targetAlpha)
    {
        Color color = allUIElements[0].color;
        Color newColor = new Color(1, 1, 1, targetAlpha);
        float i = 0;
        while (i < 1)
        {
            foreach (var j in allUIElements)
                j.color = Color.Lerp(color, newColor, i);
            yield return null;
            i += Time.unscaledDeltaTime * 4;
        }
        foreach (var j in allUIElements)
            j.color = newColor;
    }
    public static void TryUsePuzzleItem()
    {
        Scenario.currentScenario.ApplyUseItem(instance.prevSelectedItem, PlayerController.instance.scannedObject);        
    }
    public static void SetInventoryDisabled(bool abX)
    {
        instance.block = abX;
    }

    public void OnLoad(Data data)
    {
        items.Clear();
        foreach (var i in data.inventoryItems)
            Scenario.currentScenario.GiveItemFromFile(i.Item1, i.Item2);
    }

    public void OnSave(Data data)
    {
        data.inventoryItems.Clear();
        foreach (var i in items)
            for (int j = 0; j < i.Value; j++)
                data.inventoryItems.Add(new Tuple<string, string>(i.Key.InternalName, i.Key.name));
    }
}

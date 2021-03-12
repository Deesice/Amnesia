using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Journal : MonoBehaviour, ISave
{
    int numberOfQuestsOnMap;
    int diaryIdx;
    static Journal instance;
    static Dictionary<string, string> quests = new Dictionary<string, string>();
    static List<string> completeQuests = new List<string>();
    static List<string> diaries = new List<string>();
    static List<string> notes = new List<string>();
    static List<int> notePreviewIndices = new List<int>();

    public Sprite[] notePreviews;

    public AudioClip openSound;
    public AudioClip closeSound;
    public SoundLibrary questSound;
    public GameObject[] slots;
    public GameObject slotsRoot;
    public GameObject bgDiaries;
    public GameObject bgNotes;
    public GameObject bgMain;
    public Image bgText;
    public Text entryText;
    public Text entryTitle;
    List<UIButton> slotsButtons = new List<UIButton>();
    public string lastSelectedText = "";
    public GameObject nextPage;
    public GameObject prevPage;
    public GameObject nextDiaryPage;
    public GameObject nextNotePage;
    public GameObject backButton;
    public GameObject resumeButton;
    public ScrollRect scrollRect;
    public Sprite diaryPreview;
    public GameObject questAddedUINotification;
    int currentPage;
    AudioSource audioSource;
    Fader fader;
    bool fromInventory;
    public static bool isOpen;
    void Awake()
    {
        fader = GetComponent<Fader>();
        instance = this;
        foreach (var i in slots)
        {
            slotsButtons.Add(i.GetComponent<UIButton>());
            slotsButtons[slotsButtons.Count - 1].PointerDownEvent += Handler;
        }
        isOpen = false;
        gameObject.SetActive(false);
    }
    public static void ResumeInvoke()
    {
        SmartInvoke.ResumeInvoke(instance);
    }

    public static void Close()
    {
        Time.timeScale = 1;
        SoundManager.PlayClip(instance.closeSound);
        if (PlayerController.instance != null)
            PlayerController.instance.enabled = true;
        isOpen = false;
        foreach (var i in instance.GetComponentsInChildren<Fader>())
            i.FadeOff(0.25f);
        instance.fader.FadeOff(0.25f);

        if (instance.fromInventory)
            Inventory.Open();
        else
        {
            SoundManager.ManipulateAudioListener(1, 0.5f);
            TimeScaleQueue.Invoke();
        }

        SmartInvoke.Invoke(() => instance.gameObject.SetActive(false), 0.25f);
    }
    public static void Open(bool fromInventory)
    {
        Time.timeScale = 0;
        SoundManager.PlayClip(instance.openSound);
        PlayerController.instance.enabled = false;
        instance.bgMain.SetActive(fromInventory);
        instance.bgText.gameObject.SetActive(!fromInventory);
        isOpen = true;
        instance.fromInventory = fromInventory;
        instance.gameObject.SetActive(true);
        if (fromInventory)
            instance.lastSelectedText = "";
        else
            SoundManager.ManipulateAudioListener(0.5f, 0.5f);
    }
    void ShowText(int page = 0, bool playVoice = false)
    {
        if (!isOpen)
            Open(false);
        currentPage = page;
        bgDiaries.SetActive(false);
        bgNotes.SetActive(false);
        slotsRoot.SetActive(false);
        bgText.gameObject.SetActive(true);
        bgText.SetNativeSize();
        entryTitle.text = LangAdapter.FindEntry("Journal", lastSelectedText.Replace("Text", "Name"));
        var total = LangAdapter.FindEntries("Journal", lastSelectedText);
        var text = total[page];
        if (text.StartsWith("voice "))
        {
            if (playVoice)
            {
                audioSource = SoundManager.PlayClip(FakeDatabase.FindVoice(text.Substring(6, text.IndexOf(".ogg") - 6)));

                if (page == total.Count - 1)
                    SmartInvoke.Invoke(() => { audioSource.Stop(); audioSource = null; Back(); }, audioSource.clip.length);
                else
                    SmartInvoke.Invoke(() => { audioSource.Stop(); audioSource = null; NextPage(true); }, audioSource.clip.length);
            }
            text = text.Substring(text.IndexOf(".ogg") + 4);
        }
        else
            playVoice = false;
        entryText.text = text;
        scrollRect.verticalNormalizedPosition = 1;
        if (!playVoice)
        {
            if (page == 0)
                prevPage.SetActive(false);
            else
                prevPage.SetActive(true);
            if (page == total.Count - 1)
                nextPage.SetActive(false);
            else
                nextPage.SetActive(true);
            backButton.SetActive(true);
            resumeButton.SetActive(false);
        }
        else
        {
            resumeButton.SetActive(true);
            backButton.SetActive(false);
            nextPage.SetActive(false);
            prevPage.SetActive(false);
        }
    }
    public void NextPage(bool playVoice)
    {
        ShowText(currentPage + 1, playVoice);
    }
    public void PrevPage()
    {
        ShowText(currentPage - 1);
    }
    public void Back()
    {
        if (!fromInventory)
        {
            Close();
            return;
        }
        bgText.gameObject.SetActive(false);
        if (lastSelectedText.Contains("Diary"))
        {
            bgDiaries.SetActive(true);
            lastSelectedText = "";
            slotsRoot.SetActive(true);
            return;
        }
        if (lastSelectedText.Contains("Note"))
        {
            lastSelectedText = "";
            bgNotes.SetActive(true);
            slotsRoot.SetActive(true);
            return;
        }
    }
    void Handler(string data, Item item)
    {
        if (lastSelectedText == data)
        {
            var split = data.Split('*');
            if (split.Length == 1)
                bgText.sprite = diaryPreview;
            else
            {
                bgText.sprite = notePreviews[notePreviewIndices[int.Parse(split[0])]];
                lastSelectedText = split[1];
            }
            ShowText();
        }
        else
            lastSelectedText = data;
    }
    public static void AddDiary(string asNameAndTextEntry)
    {
        instance.diaryIdx = diaries.FindAll(s => s.Contains(asNameAndTextEntry)).Count + 1;
        diaries.Add(asNameAndTextEntry + "_" + instance.diaryIdx);        
        instance.lastSelectedText = "Diary_" + asNameAndTextEntry + "_Text" + instance.diaryIdx;
        isOpen = false;
        instance.bgText.sprite = instance.diaryPreview;
        instance.ShowText(0, true);
    }
    public static void AddNote(string asNameAndTextEntry, Sprite preview)
    {
        notes.Add(asNameAndTextEntry);
        for (int i = 0; i < instance.notePreviews.Length; i++)
            if (preview == instance.notePreviews[i])
            {
                notePreviewIndices.Add(i);
                break;
            }
        instance.lastSelectedText = "Note_" + asNameAndTextEntry + "_Text";
        isOpen = false;
        instance.bgText.sprite = preview;
        instance.ShowText(0, true);
    }
    public static async void AddQuest(string asName, string asNameAndTextEntry)
    {
        if (!quests.ContainsKey(asName) && !completeQuests.Contains(asName))
        {
            quests.Add(asName, asNameAndTextEntry);
            SoundManager.PlaySound(instance.questSound);
            instance.questAddedUINotification.SetActive(true);
            await Task.Delay(4000);
            instance.questAddedUINotification.GetComponent<Fader>().FadeOff();
            await Task.Delay(1000);
            instance.questAddedUINotification.SetActive(false);
        }
    }
    public static void CompleteQuest(string asName)
    {
        if (quests.ContainsKey(asName))
            quests.Remove(asName);
        completeQuests.Add(asName);
    }
    public void FillQuestText(Text text)
    {
        string s = "";
        foreach (var i in quests)
            s += "-   " + LangAdapter.FindEntry("Journal", "Quest_" + i.Value + "_Text") + "\n\n";
        text.text = s;
    }
    public void FillDiaresSlots(int page)
    {
        if (diaries.Count <= 12)
            nextDiaryPage.SetActive(false);
        int i = page * slots.Length;
        string actuallyDiaryName;
        string diaryIdx = "";
        for (; i < diaries.Count; i++)
        {
            actuallyDiaryName = "Diary_";
            foreach (var s in diaries[i].Split('_'))
                if (s != "1" && s != "2" && s != "3" && s != "4" && s != "5" && s != "6" && s != "7" && s != "8" && s != "9")
                    actuallyDiaryName += s + "_";
                else
                    diaryIdx = s;
            slots[i - page * slots.Length].SetActive(true);
            slots[i - page * slots.Length].GetComponent<Text>().text = LangAdapter.FindEntry("Journal",
                actuallyDiaryName + "Name" + diaryIdx);
            slots[i - page * slots.Length].GetComponent<UIButton>().Data = actuallyDiaryName + "Text" + diaryIdx;
            slots[i - page * slots.Length].GetComponentInChildren<Image>().sprite = diaryPreview;
        }
        i %= slots.Length;
        for (; i < slots.Length; i++)
            slots[i].SetActive(false);
    }
    public void FillNotesSlots(int page)
    {
        if (notes.Count <= 12)
            nextNotePage.SetActive(false);
        int i = page * slots.Length;
        for (; i < notes.Count; i++)
        {
            slots[i - page * slots.Length].SetActive(true);
            slots[i - page * slots.Length].GetComponent<Text>().text = LangAdapter.FindEntry("Journal",
                "Note_" + notes[i] + "_Name");
            slots[i - page * slots.Length].GetComponent<UIButton>().Data = i + "*Note_" + notes[i] + "_Text";
            slots[i - page * slots.Length].GetComponentInChildren<Image>().sprite = notePreviews[notePreviewIndices[i]];
        }
        i %= slots.Length;
        for (; i < slots.Length; i++)
            slots[i].SetActive(false);
    }
    public static int GetDiaryIdx()
    {
        return instance.diaryIdx;
    }

    public void OnLoad(Data data)
    {
        if (data.IntKeys.TryGetValue(this.GetHierarchyPath() + "/diaryIdx", out var i, 0))
            diaryIdx = i;
        notes.Clear();
        int count;
        data.IntKeys.TryGetValue("Journal/NotesCount", out count, 0);
        for (i = 0; i < count; i++)
        {
            string s = "";
            data.StringKeys.TryGetValue("Note_" + i, out s, s);
            notes.Add(s);
        }

        diaries.Clear();
        data.IntKeys.TryGetValue("Journal/DiariesCount", out count, 0);
        for (i = 0; i < count; i++)
        {
            string s = "";
            data.StringKeys.TryGetValue("Diary_" + i, out s, s);
            diaries.Add(s);
        }

        completeQuests.Clear();
        data.IntKeys.TryGetValue("Journal/CompleteQuestsCount", out count, 0);
        for (i = 0; i < count; i++)
        {
            string s = "";
            data.StringKeys.TryGetValue("CompleteQuest_" + i, out s, s);
            completeQuests.Add(s);
        }

        notePreviewIndices.Clear();
        data.IntKeys.TryGetValue("Journal/NotePreviewsCount", out count, 0);
        for (i = 0; i < count; i++)
        {
            int j;
            data.IntKeys.TryGetValue("NotePreview_" + i, out j, 0);
            notePreviewIndices.Add(j);
        }

        quests.Clear();
        data.IntKeys.TryGetValue("Journal/QuestsCount", out count, 0);
        for (i = 0; i < count; i++)
        {
            string s = "";
            data.StringKeys.TryGetValue("Quest_" + i, out s, s);
            quests.Add(s.Split('/')[0], s.Split('/')[1]);
        }
    }

    public void OnSave(Data data)
    {
        data.IntKeys.SetValueSafety(this.GetHierarchyPath() + "/diaryIdx", diaryIdx);
        int i;

        for (i = 0; i < notes.Count; i++)
            data.StringKeys.SetValueSafety("Note_" + i, notes[i]);
        data.IntKeys.SetValueSafety("Journal/NotesCount", notes.Count);

        for (i = 0; i < diaries.Count; i++)
            data.StringKeys.SetValueSafety("Diary_" + i, diaries[i]);
        data.IntKeys.SetValueSafety("Journal/DiariesCount", diaries.Count);

        for (i = 0; i < completeQuests.Count; i++)
            data.StringKeys.SetValueSafety("CompleteQuest_" + i, completeQuests[i]);
        data.IntKeys.SetValueSafety("Journal/CompleteQuestsCount", completeQuests.Count);

        for (i = 0; i < notePreviewIndices.Count; i++)
            data.IntKeys.SetValueSafety("NotePreview_" + i, notePreviewIndices[i]);
        data.IntKeys.SetValueSafety("Journal/NotePreviewsCount", notePreviewIndices.Count);

        i = 0;
        foreach (var q in quests)
        {
            string s = q.Key + "/" + q.Value;
            data.StringKeys.SetValueSafety("Quest_" + i, s);
            i++;
        }
        data.IntKeys.SetValueSafety("Journal/QuestsCount", i);
    }
}

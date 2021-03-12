using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FakeDatabase : MonoBehaviour
{    
    public List<GameObject> Particles = new List<GameObject>();

    Dictionary<string, AudioClip> clipBuffer = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> musicBuffer = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> voiceRUSBuffer = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> voiceENGBuffer = new Dictionary<string, AudioClip>();

    public List<TupleScriptable> clipRelation = new List<TupleScriptable>();
    public List<TupleScriptable> musicRelation = new List<TupleScriptable>();
    public List<TupleScriptable> voiceRUSRelation = new List<TupleScriptable>();
    public List<TupleScriptable> voiceENGRelation = new List<TupleScriptable>();
    public List<SoundProperty> sntRelation = new List<SoundProperty>();

    static FakeDatabase Instance;
    private void Awake()
    {
        Instance = this;
    }
#if UNITY_EDITOR
    public void CollectAll()
    {
        Instance = this;
        CollectAudioClips();
        CollectSNT();
        CollectParticles();
        CollectMusic();
        CollectVoices();
    }
    void CollectVoices()
    {
        voiceENGRelation.Clear();
        voiceRUSRelation.Clear();

        foreach (var i in Directory.GetDirectories("Assets/Resources/lang/rus/voices"))
            foreach (var j in Directory.GetFiles(i))
                if (j.Contains(".ogg") && !j.Contains(".meta"))
                {
                    var s = j.Split('/', '\\');
                    var p = ScriptableObject.CreateInstance<TupleScriptable>();
                    p.Key = s[s.Length - 1];
                    p.Value = j.Replace("Assets/Resources/", "").Replace("\\","/").Replace(".ogg", "");
                    voiceRUSRelation.Add(p);
                    //VoicesRUS.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(j));
                }

        foreach (var i in Directory.GetDirectories("Assets/Resources/lang/eng/voices"))
            foreach (var j in Directory.GetFiles(i))
                if (j.Contains(".ogg") && !j.Contains(".meta"))
                {
                    var s = j.Split('/', '\\');
                    var p = ScriptableObject.CreateInstance<TupleScriptable>();
                    p.Key = s[s.Length - 1];
                    p.Value = j.Replace("Assets/Resources/", "").Replace("\\", "/").Replace(".ogg", "");
                    voiceENGRelation.Add(p);
                    //VoicesENG.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(j));
                }
    }
    void CollectAudioClips()
    {        
        clipRelation.Clear();

        foreach (var i in Directory.GetDirectories("Assets/Resources/sounds"))
            foreach (var j in Directory.GetFiles(i))
                if (j.Contains(".ogg") && !j.Contains(".meta"))
                {
                    //Clips.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(j));
                    var s = j.Split('/', '\\');
                    var p = ScriptableObject.CreateInstance<TupleScriptable>();
                    p.Key = s[s.Length - 1];
                    p.Value = j.Replace("Assets/Resources/", "").Replace("\\", "/").Replace(".ogg", "");
                    clipRelation.Add(p);
                    //if (p.Key == "react_scare.ogg")
                    //    Debug.Log(p.Key + " was found");
                }

        foreach (var i in Directory.GetDirectories("Assets/Resources/sounds/enemy"))
            foreach (var j in Directory.GetFiles(i))
                if (j.Contains(".ogg") && !j.Contains(".meta"))
                {
                    //Clips.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(j));
                    var s = j.Split('/', '\\');
                    var p = ScriptableObject.CreateInstance<TupleScriptable>();
                    p.Key = s[s.Length - 2] + "/" + s[s.Length - 1];
                    p.Value = j.Replace("Assets/Resources/", "").Replace("\\", "/").Replace(".ogg", "");
                    clipRelation.Add(p);
                    //if (p.Key == "react_scare.ogg")
                    //    Debug.Log(p.Key + " was found");
                }
    }
    void CollectMusic()
    {
        musicRelation.Clear();

        foreach (var j in Directory.GetFiles("Assets/Resources/music"))
            if (j.Contains(".ogg") && !j.Contains(".meta"))
            {
                //Musics.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(i));
                var s = j.Split('/', '\\');
                var p = ScriptableObject.CreateInstance<TupleScriptable>();
                p.Key = s[s.Length - 1];
                p.Value = j.Replace("Assets/Resources/", "").Replace("\\", "/").Replace(".ogg", "");
                musicRelation.Add(p);
            }
    }
    void CollectSNT()
    {
        sntRelation.Clear();

        foreach (var i in Directory.GetDirectories("Assets/Resources/sounds"))
            foreach (var j in Directory.GetFiles(i))
                if (j.Contains(".snt") && !j.Contains(".meta"))
                {
                    var s = j.Split('/', '\\');
                    var p = ScriptableObject.CreateInstance<SoundProperty>();
                    p.LoadFromSNT(j);
                    sntRelation.Add(p);
                    //var p = ScriptableObject.CreateInstance<SoundProperty>();
                    //p.LoadFromSNT(j);
                    //Properties.Add(p);
                }

        foreach (var i in Directory.GetDirectories("Assets/Resources/sounds/enemy"))
            foreach (var j in Directory.GetFiles(i))
                if (j.Contains(".snt") && !j.Contains(".meta"))
                {
                    var s = j.Split('/', '\\');
                    var p = ScriptableObject.CreateInstance<SoundProperty>();
                    p.LoadFromSNT(j);
                    sntRelation.Add(p);
                    //var p = ScriptableObject.CreateInstance<SoundProperty>();
                    //p.LoadFromSNT(j);
                    //Properties.Add(p);
                }
    }
    void CollectParticles()
    {
        Particles.Clear();
        foreach (var i in Directory.GetFiles("Assets/Prefabs/Particles"))
            if (!i.Contains(".meta"))
                Particles.Add(AssetDatabase.LoadAssetAtPath<GameObject>(i));
    }
#endif
    public static void PreloadVoicesByPrefix(string prefix)
    {
        foreach (var i in Instance.voiceRUSRelation.FindAll((t) => t.Key.Contains(prefix)))
        {
            //Debug.Log("Preload " + i.Key);
            FindVoice(i.Key);
        }
    }
    public static AudioClip FindClip(string name)
    {
        if (name.StartsWith("critter/"))
            name = name.Split('/').Last().Replace(".ogg", "") + ".ogg";
        else
            name = name.Replace(".ogg", "") + ".ogg";
        if (Instance.clipBuffer.TryGetValue(name, out var a))
            return a;
        else
        {
            var tuple = Instance.clipRelation.Find((t) => t.Key.Contains(name));
            if (tuple == null)
            {
                Debug.Log("Error to find " + name + " sound on database");
                return null;
            }
            a = Resources.Load<AudioClip>(Instance.clipRelation.Find((t) => t.Key == name).Value);
            Instance.clipBuffer.Add(name, a);
            if (a == null)
                Debug.Log("Error to find " + name + " sound on disk");
            return a;
        }
    }
    public static SoundProperty FindProperty(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        name = name.Replace(".snt", "") + ".snt";
        var names = name.Split('/');
        name = names[names.Length - 1];
        var s = Instance.sntRelation.Find((t) => t.name == name);
        if (s != null)
        {
            s.Preload();
            return s;
        }
        else
        {
            Debug.Log(name + " do not found in database");
            return null;
        }
    }
    public static GameObject FindParticle(string name)
    {
        name = name.Replace(".ps", "");
        return Instance.Particles.Find((c) => c.name == name);
    }
    public static AudioClip FindMusic(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        name = name.Replace(".ogg", "") + ".ogg";
        if (Instance.musicBuffer.TryGetValue(name, out var a))
            return a;
        else
        {
            var relation = Instance.musicRelation.Find((t) => t.Key == name);
            if (relation == null)
            {
                Debug.Log(name + " music not registered in database");
                return null;
            }
            a = Resources.Load<AudioClip>(Instance.musicRelation.Find((t) => t.Key == name).Value);
            Instance.musicBuffer.Add(name, a);
            return a;
        }
    }
    public static AudioClip FindVoice(string name)
    {
        name = name.Replace(".ogg", "") + ".ogg";
        if (LangAdapter.CurrentLanguage == LangAdapter.Language.Russian)
        {
            if (Instance.voiceRUSBuffer.TryGetValue(name, out var a))
                return a;
            else
            {
                a = Resources.Load<AudioClip>(Instance.voiceRUSRelation.Find((t) => t.Key == name).Value);
                Instance.voiceRUSBuffer.Add(name, a);
                return a;
            }
        }
        else
        {
            if (Instance.voiceENGBuffer.TryGetValue(name, out var a))
                return a;
            else
            {
                a = Resources.Load<AudioClip>(Instance.voiceENGRelation.Find((t) => t.Key == name).Value);
                Instance.voiceENGBuffer.Add(name, a);
                return a;
            }
        }
    }
}

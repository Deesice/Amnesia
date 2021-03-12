using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenario : MonoBehaviour, ISave
{
    float MUSIC_VOLUME = 0.85f;
    static bool firstLevelInDebugMode;
    public class UseItemCallback
    {
        public string InternalName;
        public string Item;
        public string Entity;
        public string Callback;
        public bool AutoDestroy;
    }
    class Timer
    {
        public string Name;
        public float TimeLeft;
        public string Callback;
        public Timer(string n, float t, string c)
        {
            Name = n;
            TimeLeft = t;
            Callback = c;
        }
    }
    class CallbackInfo
    {
        public MethodInfo Method;
        public string TimerName;
        public CallbackInfo(MethodInfo m, string t)
        {
            Method = m;
            TimerName = t;
        }
    }

    static List<AudioSource> musicSources = new List<AudioSource>();
    List<Timer> timers = new List<Timer>();
    AudioSource effectVoicesSource;
    bool skipFirstFrame;

    string scriptPath { get { return "Assets/Resources/Maps/" + SceneManager.GetActiveScene().name + ".hps"; } }
    public string ClassName { get {
            var s = scriptPath.Split('/');
            return "Scenario_" + s[s.Length - 1].Replace(".hps", "");
        } }
    public static Scenario currentScenario;
    Dictionary<string, IEnumerator> lightFaders = new Dictionary<string, IEnumerator>();
    static SaveManager saveManager;
    private void Start()
    {
        if (saveManager == null)
            saveManager = new SaveManager();            
            //musicSources = SoundManager.GetUnmanagingSources().ToList();
            //foreach (var i in musicSources)
            //    i.priority--;
        currentScenario = gameObject.AddComponent(Type.GetType(ClassName)) as Scenario;
        var s = SceneManager.GetActiveScene().name.Split('_');
        FakeDatabase.PreloadVoicesByPrefix("CH01L" + s[0]);
        SetFogActive(false);
        if (!saveManager.LoadCurrentScene())
            currentScenario.OnStart();
        if (!string.IsNullOrEmpty(LoadingScreen.newPlayerPosition))
            currentScenario.TeleportPlayer(LoadingScreen.newPlayerPosition);
        currentScenario.OnEnter();
        var c = Finder.Find("Player");
        if (c != null)
            SmartInvoke.WhenTrue(() => c.GetComponentInChildren<Collider>().enabled, () =>
            {
                skipFirstFrame = true;
                currentScenario.skipFirstFrame = true;
            });
    }
    private void Update()
    {
        if (!skipFirstFrame)
            return;

        bool flag = false;
        var callbacks = new List<CallbackInfo>();
        foreach (var i in timers)
        {
            i.TimeLeft -= Time.deltaTime;
            if (i.TimeLeft <= 0)
            {
                flag = true;
                if (!string.IsNullOrEmpty(i.Callback))
                    callbacks.Add(new CallbackInfo(GetType().GetMethod(i.Callback), i.Name));
            }
        }
        if (flag)
            timers = timers.FindAll((t) => t.TimeLeft > 0);
        foreach (var i in callbacks)
        {
            var a = i.Method.GetParameters();
            if (a == null || a.Length == 0)
                i.Method.Invoke(this, null);
            else
                i.Method.Invoke(this, new object[] { i.TimerName });
        }
    }
    public virtual void OnStart() { }
    public virtual void OnLeave() { }
    public virtual void OnEnter() { }
#if UNITY_EDITOR
    public void CreateScenario()
    {
        string copyPath = "Assets/Scripts/Scenarios/" + ClassName + ".cs";
        if (File.Exists(scriptPath))
        {
            using (StreamWriter outfile =
                new StreamWriter(copyPath))
            {
                outfile.WriteLine("using UnityEngine;");
                outfile.WriteLine("using System.Collections;");
                outfile.WriteLine("");
                outfile.WriteLine("public class " + ClassName + " : Scenario {");
                outfile.WriteLine("private void Start() {}");
                var result = AddFloatIdentifiers(File.ReadAllText(scriptPath).Replace("&in", "")
                    .Replace("void ", "public void ")
                    .Replace("void OnStart()", "override void OnStart()")
                    .Replace("void OnEnter()", "override void OnEnter()")
                    .Replace("void OnLeave()", "override void OnLeave()")
                    .Replace(" or ", " || ")
                    .Replace("::", "this.")
                    .Replace("int i = i == 150 ? -150 : 150;",  "int i = 150;"));
                foreach (var i in AllMusic(result))
                    result = result.AddToOnEnter("\n    FakeDatabase.FindMusic(\"" + i + "\");");
                string allNames = "";
                foreach (var i in AllNames(result))
                    if (!i.Contains("."))
                        allNames += "\"" + i + "\", ";
                allNames = allNames.Substring(0, allNames.Length - 2);
                        result = result.AddToOnEnter("\n    Finder.Bufferize(" + allNames + ");");
                outfile.WriteLine(result);
                outfile.WriteLine("}");
            }
        }
        else
        {
            throw new Exception(scriptPath + " do not exits");
        }
        AssetDatabase.Refresh();
    }
    IEnumerable<string> AllMusic(string input)
    {
        int index;
        List<string> buf = new List<string>();
        string select;
        while ((index = input.IndexOf("PlayMusic")) > 0)
        {
            input = input.Substring(index + 11);
            select = input.Substring(0, input.IndexOf('"'));
            if (!buf.Contains(select))
                yield return select;
            buf.Add(select);
        }
    }
    IEnumerable<string> AllNames(string input)
    {
        int index;
        List<string> buf = new List<string>();
        string select;
        while ((index = input.IndexOf('"')) > 0)
        {
            input = input.Substring(index + 1);
            select = input.Substring(0, input.IndexOf('"'));
            input = input.Substring(input.IndexOf('"') + 1);
            if (select.EndsWith("_"))
                for (int i = 0; i < 100; i++)
                {
                    var s = select + i;
                    if (!buf.Contains(s))
                    {
                        yield return s;
                    }
                    buf.Add(s);
                }
            else
            {
                if (!buf.Contains(select))
                    yield return select;
                buf.Add(select);
            }
        }
    }
    string AddFloatIdentifiers(string input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '.' && (input[i+1] == '0' || input[i + 1] == '1' || input[i + 1] == '2' || input[i + 1] == '3'
                || input[i + 1] == '4' || input[i + 1] == '5' || input[i + 1] == '6' || input[i + 1] == '7'
                || input[i + 1] == '8' || input[i + 1] == '9'))
            {
                //Debug.Log("Find float at " + i);
                var j = i + 1;
                while (input[j] == '0' || input[j] == '1' || input[j] == '2' || input[j] == '3'
                || input[j] == '4' || input[j] == '5' || input[j] == '6' || input[j] == '7'
                || input[j] == '8' || input[j] == '9')
                    j++;
                if (input[j] != 'f')
                {
                    //Debug.Log("Add float at " + j);
                    input = input.Insert(j, "f");
                    //Debug.Log(input.Substring(j-10, 20));
                }
            }
        }
        return input;
    }
#endif
    /////////////////////
    protected float RandFloat(float afMin, float afMax)
    {
        return UnityEngine.Random.Range(afMin, afMax);
    }
    protected int RandFloat(int afMin, int afMax)
    {
        return UnityEngine.Random.Range(afMin, afMax);
    }
    protected int RandInt(int alMin, int alMax)
    {
        return UnityEngine.Random.Range(alMin, alMax + 1);
    }
    protected bool StringContains(string asString, string asSubString)
    {
        return asString.Contains(asSubString);
    }
    protected string StringSub(string asString, int alStart, int alCount)
    {
        return asString.Substring(alStart, alCount);
    }
    protected int StringToInt(string asString)
    {
        if (int.TryParse(asString, out var i))
            return i;
        return 0;
    }
    protected float StringToFloat(string asString)
    {
        if (float.TryParse(asString, out var i))
            return i;
        return 0;
    }
    protected bool StringToBool(string asString)
    {
        if (bool.TryParse(asString, out var i))
            return i;
        return false;
    }
    //////////////MATH///////////////
    protected float MathSin(float afX)
    {
        return Mathf.Sin(afX);
    }
    protected float MathCos(float afX)
    {
        return Mathf.Cos(afX);
    }
    protected float MathTan(float afX)
    {
        return Mathf.Tan(afX);
    }
    protected float MathAsin(float afX)
    {
        return Mathf.Asin(afX);
    }
    protected float MathAcos(float afX)
    {
        return Mathf.Acos(afX);
    }
    protected float MathAtan(float afX)
    {
        return Mathf.Atan(afX);
    }
    protected float MathAtan2(float afX, float afY)
    {
        return Mathf.Atan2(afY, afX);
    }
    protected float MathSqrt(float afX)
    {
        return Mathf.Sqrt(afX);
    }
    protected float MathPow(float afBase, float afExp)
    {
        return Mathf.Pow(afBase, afExp);
    }
    protected float MathMin(float afA, float afB)
    {
        return Mathf.Min(afA, afB);
    }
    protected float MathMax(float afA, float afB)
    {
        return Mathf.Max(afA, afB);
    }
    protected float MathClamp(float afX, float afMin, float afMax)
    {
        return Mathf.Clamp(afX, afMin, afMax);
    }
    protected float MathAbs(float afX)
    {
        return Mathf.Abs(afX);
    }
    ////////////DEBUGGING///////////////
    protected void Print(string asString)
    {
        //Debug.Log(asString);
    }
    protected void AddDebugMessage(string asString, bool abCheckForDuplicates)
    {
        Print(asString);
    }
    protected void ProgLog(string asLevel, string asMessage)
    {
        Print(asLevel + ": " + asMessage);
    }
    protected bool ScriptDebugOn()
    {
        return false;
#if UNITY_EDITOR
        if (!firstLevelInDebugMode)
        {
            firstLevelInDebugMode = true;
            return true;
        }
        return false;
#else
        return false;
#endif
    }
    ////////////VARIABLES////////////
    protected void SetLocalVarInt(string asName, int alVal)
    {
        asName = ClassName + "/" + asName;
        saveManager.data.IntKeys.SetValueSafety(asName, alVal);
    }
    protected void AddLocalVarInt(string asName, int alVal)
    {
        asName = ClassName + "/" + asName;
        int i;
        saveManager.data.IntKeys.TryGetValue(asName, out i, 0);
        saveManager.data.IntKeys.SetValueSafety(asName, i + alVal);
    }
    protected int GetLocalVarInt(string asName)
    {
        asName = ClassName + "/" + asName;
        int i;
        saveManager.data.IntKeys.TryGetValue(asName, out i, 0);
        return i;
    }
    protected void SetLocalVarFloat(string asName, float alVal)
    {
        asName = ClassName + "/" + asName;
        saveManager.data.FloatKeys.SetValueSafety(asName, alVal);
    }
    protected void AddLocalVarFloat(string asName, float alVal)
    {
        asName = ClassName + "/" + asName;
        float i;
        saveManager.data.FloatKeys.TryGetValue(asName, out i, 0);
        saveManager.data.FloatKeys.SetValueSafety(asName, i + alVal);
    }
    protected float GetLocalVarFloat(string asName)
    {
        asName = ClassName + "/" + asName;
        float i;
        saveManager.data.FloatKeys.TryGetValue(asName, out i, 0);
        return i;
    }
    protected void SetLocalVarString(string asName, string alVal)
    {
        asName = ClassName + "/" + asName;
        saveManager.data.StringKeys.SetValueSafety(asName, alVal);
    }
    protected void AddLocalVarString(string asName, string alVal)
    {
        asName = ClassName + "/" + asName;
        string i;
        saveManager.data.StringKeys.TryGetValue(asName, out i, string.Empty);
        saveManager.data.StringKeys.SetValueSafety(asName, i + alVal);
    }
    protected string GetLocalVarString(string asName)
    {
        asName = ClassName + "/" + asName;
        string i;
        saveManager.data.StringKeys.TryGetValue(asName, out i, string.Empty);
        return i;
    }
    protected void SetGlobalVarInt(string asName, int alVal)
    {
        saveManager.data.IntKeys.SetValueSafety(asName, alVal);
    }
    protected void AddGlobalVarInt(string asName, int alVal)
    {
        int i;
        saveManager.data.IntKeys.TryGetValue(asName, out i, 0);
        saveManager.data.IntKeys.SetValueSafety(asName, i + alVal);
    }
    protected int GetGlobalVarInt(string asName)
    {
        int i;
        saveManager.data.IntKeys.TryGetValue(asName, out i, 0);
        return i;
    }
    protected void SetGlobalVarFloat(string asName, float alVal)
    {
        saveManager.data.FloatKeys.SetValueSafety(asName, alVal);
    }
    protected void AddGlobalVarFloat(string asName, float alVal)
    {
        float i;
        saveManager.data.FloatKeys.TryGetValue(asName, out i, 0);
        saveManager.data.FloatKeys.SetValueSafety(asName, i + alVal);
    }
    protected float GetGlobalVarFloat(string asName)
    {
        float i;
        saveManager.data.FloatKeys.TryGetValue(asName, out i, 0);
        return i;
    }
    protected void SetGlobalVarString(string asName, string alVal)
    {
        saveManager.data.StringKeys.SetValueSafety(asName, alVal);
    }
    protected void AddGlobalVarString(string asName, string alVal)
    {
        string i;
        saveManager.data.StringKeys.TryGetValue(asName, out i, string.Empty);
        saveManager.data.StringKeys.SetValueSafety(asName, i + alVal);
    }
    protected string GetGlobalVarString(string asName)
    {
        string i;
        saveManager.data.StringKeys.TryGetValue(asName, out i, string.Empty);
        return i;
    }
    ///////Particle Systems//////////
    protected void PreloadParticleSystem(string asPSFile)
    {

    }
    public void CreateParticleSystemAtEntity(string asPSName, string asPSFile, string asEntity, bool abSavePS)
    {
        var parent = Finder.Find(asEntity);
        if (parent == null)
        {
            Debug.Log(asEntity + " do not found");
            return;
        }
        var path = "Assets/Prefabs/Particles/" + asPSFile.Replace(".ps", ".prefab");
        var size = Vector3.one;
        if (path.Contains("_large"))
        {
            path = path.Replace("_large", "");
            size *= 2;
        }
        var g = FakeDatabase.FindParticle(asPSFile);
        if (g != null)
        {
            g = Instantiate(g);
            g.transform.localScale = size;
            g.name = asPSName;
            g.transform.parent = parent.transform;
            g.transform.localPosition = Vector3.zero;
        }
        else
        {
            Debug.Log(path + " do not found in database");
        }
    }
    protected void CreateParticleSystemAtEntityExt(string asPSName, string asPSFile, string asEntity, bool abSavePS,
float afR, float afG, float afB, float afA, bool abFadeAtDistance, float afFadeMinEnd, float afFadeMinStart,
float afFadeMaxStart, float afFadeMaxEnd)
    {
        CreateParticleSystemAtEntity(asPSName, asPSFile, asEntity, abSavePS);
    }
    protected void DestroyParticleSystem(string asName)
    {
        Destroy(Finder.Find(asName));
    }
    /////////SOUND&MUSIC//////////////
    protected void PreloadSound(string asSoundFile)
    {
        FakeDatabase.FindProperty(asSoundFile);
    }
    public void PlaySoundAtEntity
        (string asSoundName, string asSoundFile, string asEntity, float afFadeTime, bool abSaveSound)
    {
        if (SoundManager.PlaySoundAtEntity(asSoundName, asSoundFile, asEntity, afFadeTime))
        saveManager.data.soundSaves.Add(new SoundSave()
        {
            asEntity = asEntity,
            afFadeTime = afFadeTime,
            asSoundFile = asSoundFile,
            asSoundName = asSoundName,
            map = SceneManager.GetActiveScene().name
        });
    }
    protected void StopSound(string asSoundName, float afFadeTime)
    {
        saveManager.data.soundSaves.Remove(saveManager.data.soundSaves.Find(s => s.asSoundName == asSoundName && s.map == SceneManager.GetActiveScene().name));
        SoundManager.StopSound(asSoundName, afFadeTime);
    }
    protected void PlayMusic(string asMusicFile, bool abLoop, float afVolume, float afFadeTime, int alPrio, bool abResume)
    {
        if (Mathf.Approximately(afVolume, 0))
            return;
        alPrio++;
        //Debug.Log("Music ordered: " + asMusicFile + "; Volume = " + afVolume + "; Time = " + afFadeTime);
        AudioClip clip = FakeDatabase.FindMusic(asMusicFile);
        if (clip == null)
        {
            Debug.Log(asMusicFile + " was not found in database");
        }    
        AudioSource s = musicSources.Find(m => m.priority == alPrio);
        if (s == null)
        {
            s = SoundManager.AllocAudioSource();
            s.ignoreListenerVolume = true;
            s.playOnAwake = false;
            s.priority = alPrio;
            musicSources.Add(s);
        }
        if (s.clip == clip && s.isPlaying)
            return;
        if (s.clip != clip)
            s.clip = clip;
        s.loop = abLoop;
        var highestPlaying = (from i in musicSources where i.isPlaying orderby i.priority descending select i).FirstOrDefault();
        if (highestPlaying == null || alPrio >= highestPlaying.priority)
        {
            if (highestPlaying != null)
                StopMusic(afFadeTime, highestPlaying);
            s.volume = 0;
            if (abResume && s.time != 0)
                s.UnPause();
            else
                s.Play();
            SoundManager.VolumeFaderExtern(s, afFadeTime, afVolume * MUSIC_VOLUME);
            if (!abLoop)
                SmartInvoke.Invoke(() => TimeScaleQueue.AddFunc(PlayHighestPriority), clip.length);
        }
        else
        {
            s.volume = afVolume * MUSIC_VOLUME;
            SmartInvoke.Invoke(() => TimeScaleQueue.AddFunc(PlayHighestPriority), highestPlaying.clip.length - highestPlaying.time);
        }            
    }
    protected bool StopMusic(float afFadeTime, AudioSource source)
    {
        if (source != null && source.isPlaying)
        {
            if (afFadeTime == 0)
                source.Pause();
            else
                SoundManager.VolumeFaderExtern(source, afFadeTime, 0);
            return true;
        }
        return false;
    }
    protected void StopMusic(float afFadeTime, int alPrio)
    {
        alPrio++;
        var s = musicSources.Find(m => m.priority == alPrio);
        if (StopMusic(afFadeTime, s))
        {
            musicSources.Remove(s);
            SoundManager.AddToManagingSources(s);
            PlayHighestPriority();
        }
    }
    void PlayHighestPriority()
    {
        var highestPrio = (from i in musicSources orderby i.priority descending select i.priority).FirstOrDefault();
        for (int i = highestPrio; i >= 0; --i)
        {
            var t = musicSources.Find(m => m.priority == i);
            if (t != null)
            {
                if (t.loop)
                {
                    PlayMusic(t.clip.name, true, t.volume / MUSIC_VOLUME, 1 / 0.3f, t.priority - 1, true);
                    return;
                }
            }
        }
    }
    protected void FadeGlobalSoundVolume(float afDestVolume, float afTime)
    {
        SoundManager.ManipulateAudioListener(afDestVolume, afTime);
    }
    protected void FadeGlobalSoundSpeed(float afDestSpeed, float afTime)
    {
        //throw new NotImplementedException();
    }
    ///////LIGHTS///////
    protected void SetLightVisible(string asLightName, bool abVisible)
    {
        SetEntityActive(asLightName, abVisible);
    }
    protected void FadeLightTo(string asLightName, float afR, float afG, float afB, float afA, float afRadius, float afTime)
    {
        var g = Finder.Find(asLightName);
        if (g == null)
        {
            Debug.Log(asLightName + " do not found");
            return;
        }
        var l = g.GetComponent<Light>();
        if (l != null)
        {
            l.gameObject.AddComponent<LightColorAndRangeCache>();
            saveManager.data.AddedComponent.SetValueSafety(SceneManager.GetActiveScene().name + "/" + l.gameObject.name + "/" + nameof(LightColorAndRangeCache), typeof(LightColorAndRangeCache));
            IEnumerator cor;
            if (lightFaders.TryGetValue(asLightName, out cor))
            {
                StopCoroutine(cor);
                lightFaders.Remove(asLightName);
            }
            Color color;
            if (afR == -1)
                color.r = l.color.r;
            else
                color.r = afR;
            if (afG == -1)
                color.g = l.color.g;
            else
                color.g = afG;
            if (afB == -1)
                color.b = l.color.b;
            else
                color.b = afB;
            if (afA == -1)
                color.a = l.color.a;
            else
                color.a = afA;
            if (afRadius == -1)
                afRadius = l.range;
            cor = LightFader(l, color, afRadius, afTime);
            StartCoroutine(cor);
            lightFaders.Add(asLightName, cor);
        }
        else
        {
            var b = g.GetComponent<BoxLight>();
            if (b != null)
            {
                Color color;
                if (afR == -1)
                    color.r = b.color.r;
                else
                    color.r = afR;
                if (afG == -1)
                    color.g = b.color.g;
                else
                    color.g = afG;
                if (afB == -1)
                    color.b = b.color.b;
                else
                    color.b = afB;
                if (afA == -1)
                    color.a = b.color.a;
                else
                    color.a = afA;
                b.FadeTo(color, afTime);
            }
            else
                Debug.Log(asLightName + " not contain Light or BoxLight");
        }
    }
    protected void SetLightFlickerActive(string asLightName, bool abActive)
    {
        throw new NotImplementedException();
    }
    ///////GENERAL/////////
    protected void StartCredits(string asMusic, bool abLoopMusic, string asTextCat, string asTextEntry, int alEndNum)
    {
        PlayMusic(asMusic, abLoopMusic, 1, 0, 10, false);
    }
    protected void StartDemoEnd()
    {
        throw new NotImplementedException();
    }
    protected void AutoSave()
    {
        saveManager.SaveCurrentScene();
        saveManager.SaveDataOnDisk();
    }
    protected void CheckPoint(string asName, string asStartPos, string asCallback, string asDeathHintCat, string asDeathHintEntry)
    {
        throw new NotImplementedException();
    }
    public void ChangeMap(string asMapName, string asStartPos, AudioClip asStartSound, AudioClip asEndSound)
    {        
        currentScenario.OnLeave();
        saveManager.SaveCurrentScene();
        LoadingScreen.instance.gameObject.SetActive(true);
        LoadingScreen.newPlayerPosition = asStartPos;
        LoadingScreen.clip = asEndSound;
        SceneManager.LoadScene(asMapName.Replace(".map",""));
    }
    protected void ChangeMap(string asMapName, string asStartPos, string asStartSound, string asEndSound)
    {
        ChangeMap(asMapName, asStartPos, FakeDatabase.FindProperty(asStartSound).GetClip(), FakeDatabase.FindProperty(asEndSound).GetClip());
    }
    protected void ClearSavedMaps()
    {
        Debug.Log("DO NOT FORGET IMPLEMENT ClearSavedMaps()");
    }
    protected void CreateDataCache()
    {
        throw new NotImplementedException();
    }
    protected void DestroyDataCache()
    {
        throw new NotImplementedException();
    }
    protected void SetMapDisplayNameEntry(string asNameEntry)
    {
        //throw new NotImplementedException();
    }
    protected void SetSkyBoxActive(bool abActive)
    {
        //throw new NotImplementedException();
    }
    protected void SetSkyBoxTexture(string asTexture)
    {
        throw new NotImplementedException();
    }
    protected void SetSkyBoxColor(float afR, float afG, float afB, float afA)
    {
        throw new NotImplementedException();
    }
    protected void SetFogActive(bool abActive)
    {
        FogController.Enable(abActive);
    }
    protected void SetFogColor(float afR, float afG, float afB, float afA)
    {
        FogController.SetFogColor(new Color(afR, afG, afB, afA));
    }
    protected void SetFogProperties(float afStart, float afEnd, float afFalloffExp, bool abCulling)
    {
        FogController.SetFogProperties(afStart, afEnd, afFalloffExp, abCulling);
    }
    protected void SetupLoadScreen(string asTextCat, string asTextEntry, int alRandomNum, string asImageFile)
    {
        LoadingScreen.SetupLoadingScreen(asTextCat, asTextEntry, alRandomNum, asImageFile);
    }
    ////////GAME TIMER//////////
    public void AddTimer(string asName, float afTime, string asFunction)
    {
        var timer = timers.Find((t) => t.Name == asName);
        if (timer == null)
            timers.Add(new Timer(asName, afTime, asFunction));
        else
        {
            Debug.Log("Try to add exist timer: " + asName);
            timers.Add(new Timer(asName, afTime, asFunction));
            //timer.TimeLeft = afTime;
            //timer.Callback = asFunction;
        }
    }
    protected void RemoveTimer(string asName)
    {
        var timer = timers.Find((t) => t.Name == asName);
        if (timer != null)
        {
            timer.TimeLeft = 0;
            timers.Remove(timer);
        }
    }
    protected float GetTimerTimeLeft(string asName)
    {
        var timer = timers.Find((t) => t.Name == asName);
        if (timer != null)
            return timer.TimeLeft;
        else
            return 0;
    }
    ///////SCREEN EFFECTS///////
    protected void FadeOut(float afTime)
    {
        PlayerController.instance.fader.FadeOn(Color.black, afTime);
    }
    protected void FadeIn(float afTime)
    {
        PlayerController.instance.fader.FadeOff(afTime);
    }
    protected void FadeImageTrailTo(float afAmount, float afSpeed)
    {
        UnityStandardAssets.ImageEffects.MotionBlur.FadeImageTrailTo(afAmount, afSpeed);
    }
    protected void FadeSepiaColorTo(float afAmount, float afSpeed)
    {
        SepiaController.FadeSepiaColorTo(afAmount, afSpeed);
    }
    protected void FadeRadialBlurTo(float afSize, float afSpeed)
    {
        BlurController.FadeRadialBlurTo(afSize, afSpeed);
    }
    protected void SetRadialBlurStartDist(float afStartDist)
    {
        RadialBlurController.SetRadialBlurStartDist(afStartDist);
    }
    public async void StartEffectFlash(float afFadeIn, float afWhite, float afFadeOut)
    {
        PlayerController.instance.fader.FadeOn(Color.white, afFadeIn);
        await Task.Delay((int)((afFadeIn + afWhite)* 1000));
        PlayerController.instance.fader.FadeOff(afFadeOut);
    }
    protected void StartEffectEmotionFlash(string asTextCat, string asTextEntry, string asSound)
    {
        throw new NotImplementedException();
    }
    public async void AddEffectVoice(string asVoiceFile, string uselessParameter, string asTextCat, string asTextEntry,
bool abUsePosition, string asPosEntity, float afMinDistance, float afMaxDistance)
    {
        var clip = FakeDatabase.FindVoice(asVoiceFile);
        var subtitle = LangAdapter.FindEntry(asTextCat, asTextEntry);
        var time = GetTimerTimeLeft("EffectVoicesTechnicalTimer");
        if (time == 0)
            AddTimer("EffectVoicesTechnicalTimer", clip.length, "");
        else
        {
            timers.Find((t) => t.Name == "EffectVoicesTechnicalTimer").TimeLeft += clip.length;
            await Task.Delay((int)(time * 1000));
            time = GetTimerTimeLeft("EffectVoicesTechnicalTimer");
            if (time <= 0)
                return;
        }
        Subtitle.ShowSubtitle(subtitle, clip.length);
        if (abUsePosition)
        {
            var g = Finder.Find(asPosEntity);
            effectVoicesSource = g.GetComponent<AudioSource>();
            if (effectVoicesSource == null)
                effectVoicesSource = g.AddComponent<AudioSource>();
            effectVoicesSource.minDistance = afMinDistance;
            effectVoicesSource.maxDistance = afMaxDistance;
            effectVoicesSource.spatialBlend = 1;
            effectVoicesSource.rolloffMode = AudioRolloffMode.Linear;
            effectVoicesSource.clip = clip;
            effectVoicesSource.loop = false;
            effectVoicesSource.Play();
        }
        else
            effectVoicesSource = SoundManager.PlayClip(clip);
    }
    protected void StopAllEffectVoices(float afFadeOutTime)
    {
        var timer = timers.Find((t) => t.Name == "EffectVoicesTechnicalTimer");
        if (timer == null)
            return;
        SoundManager.VolumeFaderExtern(effectVoicesSource, afFadeOutTime, 0);
        var callback = timer.Callback;
        timers.Remove(timer);
        Invoke(callback, 0);
    }
    public bool GetEffectVoiceActive()
    {
        return GetTimerTimeLeft("EffectVoicesTechnicalTimer") > 0;
    }
    public void SetEffectVoiceOverCallback(string asFunc)
    {
        var timer = timers.Find((t) => t.Name == "EffectVoicesTechnicalTimer");
        if (timer != null)
            timer.Callback = asFunc;
    }
    protected bool GetFlashbackIsActive()
    {
        throw new NotImplementedException();
    }
    protected void StartPlayerSpawnPS(string asSPSFile)
    {
        throw new NotImplementedException();
    }
    protected void StopPlayerSpawnPS()
    {
        throw new NotImplementedException();
    }
    protected void PlayGuiSound(string asSoundFile, float afVolume)
    {
        asSoundFile = asSoundFile.Replace(".ogg", "").Replace(".snt", "");
        var snt = FakeDatabase.FindProperty(asSoundFile);
        if (snt != null)
            SoundManager.PlayClip(snt.GetClip(), false, afVolume);
        else
            SoundManager.PlayClip(FakeDatabase.FindClip(asSoundFile), false, afVolume);
    }
    protected void StartScreenShake(float afAmount, float afTime, float afFadeInTime, float afFadeOutTime)
    {
        CameraAnim.StartScreenShake(afAmount, afTime, afFadeInTime, afFadeOutTime);
    }
    protected void SetInDarknessEffectsActive(bool abX)
    {
        throw new NotImplementedException();
    }
    //////////INSANITY///////////////
    protected void SetInsanitySetEnabled(string asSet, bool abX)
    {
        //throw new NotImplementedException();
    }
    protected void StartInsanityEvent(string asEventName)
    {
        throw new NotImplementedException();
    }
    protected void StartRandomInsanityEvent()
    {
        throw new NotImplementedException();
    }
    protected void StopCurrentInsanityEvent()
    {
        throw new NotImplementedException();
    }
    protected void InsanityEventIsActive()
    {
        throw new NotImplementedException();
    }
    ////////PLAYER///////
    protected void SetPlayerActive(bool abActive)
    {
        PlayerController.instance.FreezeMovement = !abActive;
    }
    protected void ChangePlayerStateToNormal()
    {
        throw new NotImplementedException();
    }
    protected void SetPlayerCrouching(bool abCrouch)
    {
        CameraAnim.instance.SetCrouch = abCrouch;
    }
    protected void AddPlayerBodyForce(float afX, float afY, float afZ, bool abUseLocalCoords)
    {
        var rb = PlayerController.instance.playerBody;
        if (!abUseLocalCoords)
            rb.AddForce(new Vector3(-afX, afY, afZ));
        else
            throw new NotImplementedException();
    }
    protected void ShowPlayerCrossHairIcons(bool abX)
    {
        Crosshair.ShowCrosshair(abX);
    }
    protected void SetPlayerSanity(float afSanity)
    {
        throw new NotImplementedException();
    }
    protected void AddPlayerSanity(float afSanity)
    {
        throw new NotImplementedException();
    }
    protected float GetPlayerSanity()
    {
        throw new NotImplementedException();
    }
    protected void SetPlayerHealth(float afHealth)
    {
        throw new NotImplementedException();
    }
    protected void AddPlayerHealth(float afHealth)
    {
        HealthSystem.AddDamage(-afHealth);
    }
    protected float GetPlayerHealth()
    {
        throw new NotImplementedException();
    }
    protected void SetPlayerLampOil(float afOil)
    {
        Lantern.Oil = afOil;
    }
    protected void AddPlayerLampOil(float afOil)
    {
        Lantern.Oil += afOil;
    }
    protected float GetPlayerLampOil()
    {
        return Lantern.Oil;
    }
    protected float GetPlayerSpeed()
    {
        return PlayerController.instance.playerBody.velocity.magnitude;
    }
    protected float GetPlayerYSpeed()
    {
        return Mathf.Abs(PlayerController.instance.playerBody.velocity.y);
    }
    protected void SetSanityDrainDisabled(bool abX)
    {
        SanitySystem.instance.setSanityDrainDisabled = abX;
        if (abX)
            SanitySystem.instance.InDark = false;
    }
    protected void GiveSanityBoost()
    {
        if (SanitySystem.sanityAmount < 25)
            SanitySystem.GiveSanityDamage(-(100 - SanitySystem.sanityAmount), true);
        else if (SanitySystem.sanityAmount < 50)
            SanitySystem.GiveSanityDamage(-(90 - SanitySystem.sanityAmount), true);
        else if (SanitySystem.sanityAmount < 75)
            SanitySystem.GiveSanityDamage(-(80 - SanitySystem.sanityAmount), true);
        else
            SanitySystem.GiveSanityDamage(-5, true);
    }
    protected void GiveSanityBoostSmall()
    {
        if (SanitySystem.sanityAmount < 25)
            SanitySystem.GiveSanityDamage(-20, true);
        else if (SanitySystem.sanityAmount < 50)
            SanitySystem.GiveSanityDamage(-15, true);
        else if (SanitySystem.sanityAmount < 75)
            SanitySystem.GiveSanityDamage(-10, true);
        else
            SanitySystem.GiveSanityDamage(-5, true);
    }
    protected void GiveSanityDamage(float afAmount, bool abUseEffect)
    {
        SanitySystem.GiveSanityDamage(afAmount, abUseEffect);
    }
    protected void GivePlayerDamage(float afAmount, string asType, bool abSpinHead, bool abLethal)
    {
        throw new NotImplementedException();
    }
    protected void FadePlayerFOVMulTo(float afX, float afSpeed)
    {
        CameraAnim.FadePlayerFOVMulTo(afX, afSpeed);
    }
    protected void FadePlayerAspectMulTo(float afX, float afSpeed)
    {
        CameraAnim.FadePlayerAspectMulTo(afX, afSpeed);
    }
    protected void FadePlayerRollTo(float afX, float afSpeedMul, float afMaxSpeed)
    {
        PlayerController.instance.rotateProperty.angle = afX;
        PlayerController.instance.rotateProperty.speed = afSpeedMul;
        PlayerController.instance.rotateProperty.maxSpeed = afMaxSpeed;
    }
    protected void MovePlayerHeadPos(float afX, float afY, float afZ, float afSpeed, float afSlowDownDist)
    {
        CameraAnim.SetOffsetProperty(new Vector3(-afX, afY, afZ), afSpeed, afSlowDownDist);
    }
    protected void StartPlayerLookAt(string asEntityName, float afSpeedMul, float afMaxSpeed, string asAtTargetCallback)
    {
        var g = Finder.Find(asEntityName);
        if (g == null)
        {
            Debug.Log(asEntityName + " does not exist in scene");
            return;
        }
        var t = g.transform;
        
        PlayerController.instance.lookAtProperty.target = t;
        PlayerController.instance.lookAtProperty.speed = afSpeedMul;
        PlayerController.instance.lookAtProperty.maxSpeed = afMaxSpeed;
        if (!string.IsNullOrEmpty(asAtTargetCallback))
            throw new Exception("LooaAtCallback not implement");
    }
    protected void StopPlayerLookAt()
    {
        PlayerController.instance.lookAtProperty.target = null;
    }
    protected void SetPlayerMoveSpeedMul(float afMul)
    {
        PlayerController.instance.baseMultiplierProperty.moveSpeedMul = afMul;
    }
    protected void SetPlayerRunSpeedMul(float afMul)
    {
        PlayerController.instance.baseMultiplierProperty.runSpeedMul = afMul;
    }
    protected void SetPlayerLookSpeedMul(float afMul)
    {
        PlayerController.instance.baseMultiplierProperty.lookSpeedMul = afMul;
    }
    protected void SetPlayerJumpForceMul(float afMul)
    {

    }
    protected void SetPlayerJumpDisabled(bool abX)
    {
    }
    protected void SetPlayerCrouchDisabled(bool abX)
    {
        PlayerController.instance.BlockCrouch = !abX;
    }
    public void TeleportPlayer(string asStartPosName)
    {
        if (string.IsNullOrEmpty(asStartPosName))
            return;
        var p = Finder.Find(asStartPosName).transform;
        PlayerController.instance.playerBody.transform.position = p.position + Vector3.up * PlayerController.instance.playerBody.GetComponent<CapsuleCollider>().height / 2;
        PlayerController.instance.rotationCumulative = p.rotation;
        PlayerController.instance.transform.rotation = p.rotation;
    }
    protected void SetLanternActive(bool abX, bool abUseEffects)
    {
        if (abX)
            Lantern.On();
        else
            Lantern.Off();
    }
    protected bool GetLanternActive()
    {
        return Lantern.IsLit;
    }
    protected void SetLanternDisabled(bool abX)
    {
        PlayerController.instance.BlockLantern = abX;
    }
    protected void SetLanternLitCallback(string asCallback)
    {
        throw new NotImplementedException();
    }
    protected void SetMessage(string asTextCategory, string asTextEntry, float afTime)
    {
        Message.ShowMessage(LangAdapter.FindEntry(asTextCategory, asTextEntry), afTime);
    }
    protected void SetDeathHint(string asTextCategory, string asTextEntry)
    {
        throw new NotImplementedException();
    }
    public void MovePlayerForward(float afAmount)
    {
        var forward = PlayerController.instance.transform.forward;
        forward.y = 0;
        forward.Normalize();
        PlayerController.instance.playerBody.transform.position += forward * afAmount * Time.deltaTime;
    }
    protected void SetPlayerFallDamageDisabled(bool abX)
    {
        throw new NotImplementedException();
    }
    protected void SetPlayerPos(float afX, float afY, float afZ)
    {
        PlayerController.instance.playerBody.transform.position = new Vector3(-afX, afY, afZ);
    }
    protected float GetPlayerPosX()
    {
        return -PlayerController.instance.playerBody.transform.position.x;
    }
    protected float GetPlayerPosY()
    {
        return PlayerController.instance.playerBody.transform.position.y;
    }
    protected float GetPlayerPosZ()
    {
        return PlayerController.instance.playerBody.transform.position.z;
    }
    //////JOURNAL////////
    protected void AddNote(string asNameAndTextEntry, string asImage)
    {
        throw new NotImplementedException();
    }
    protected void AddDiary(string asNameAndTextEntry, string asImage)
    {
        throw new NotImplementedException();
    }
    protected void ReturnOpenJournal(bool abOpenJournal)
    {
        if (!abOpenJournal)
            Journal.Close();
        else
            throw new NotImplementedException();
    }
    ////////QUESTS////////
    protected void AddQuest(string asName, string asNameAndTextEntry)
    {
        Journal.AddQuest(asName, asNameAndTextEntry);
    }
    protected void CompleteQuest(string asName, string asNameAndTextEntry)
    {
        Journal.CompleteQuest(asName);
    }
    protected bool QuestIsCompleted(string asName)
    {
        throw new NotImplementedException();
    }
    protected bool QuestIsAdded(string asName)
    {
        throw new NotImplementedException();
    }
    protected void SetNumberOfQuestsInMap(int alNumberOfQuests)
    {
        //throw new NotImplementedException();
    }
    protected void GiveHint(string asName, string asMessageCat, string asMessageEntry, float afTimeShown)
    {
        Hint.ShowHint(asName, LangAdapter.FindEntry(asMessageCat, asMessageEntry), afTimeShown);
    }
    protected void RemoveHint(string asName)
    {
        Hint.RemoveHint(asName);
    }
    protected void BlockHint(string asName)
    {
        Hint.BlockHint(asName);
    }
    protected void UnBlockHint(string asName)
    {
        Hint.UnBlockHint(asName);
    }
    ///////INVENTORY//////
    protected void ExitInventory()
    {
        throw new NotImplementedException();
    }
    protected void SetInventoryDisabled(bool abX)
    {
        Inventory.SetInventoryDisabled(abX);
    }
    protected void SetInventoryMessage(string asTextCategory, string asTextEntry, float afTime)
    {
        throw new NotImplementedException();
    }
    protected void GiveItem(string asName, string asType, string asSubTypeName, string asImageName, float afAmount)
    {
        throw new NotImplementedException();
    }
    protected void RemoveItem(string asName)
    {
        Inventory.RemoveItem(asName);
    }
    protected bool HasItem(string asName)
    {
        return Inventory.HasItem(asName);
    }
    public void GiveItemFromFile(string asName, string asFileName)
    {
        var item = Resources.Load<Item>("Items/" + asFileName.Replace(".ent", ""));
        item.InternalName = asName;
        Inventory.AddItem(item);
    }
    protected void AddCombineCallback(string asName, string asItemA, string asItemB, string asFunction, bool abAutoRemove)
    {
        throw new NotImplementedException();
    }
    protected void RemoveCombineCallback(string asName)
    {
        throw new NotImplementedException();
    }
    protected void AddUseItemCallback(string asName, string asItem, string asEntity, string asFunction, bool abAutoDestroy)
    {
        var c = new UseItemCallback();
        c.InternalName = asName;
        c.Item = asItem;
        c.Entity = asEntity;
        c.Callback = asFunction;
        c.AutoDestroy = abAutoDestroy;
        var g = Finder.Find(asEntity);
        if (g != null)
            g.layer = 0;
        saveManager.data.itemCallbacks.Add(c);
    }
    public void ApplyUseItem(Item asItem, GameObject asEntity)
    {
        if (asEntity == null || (!asEntity.CompareTag("Grab") && !asEntity.CompareTag("LevelDoor") && !asEntity.CompareTag("Area")))
        {
            Message.ShowMessage(LangAdapter.FindEntry("Inventory", "UseItemHasNoObject"));
            return;
        }
        UseItemCallback c = null;
        while (c == null && asEntity != null)
        {
            c = saveManager.data.itemCallbacks.Find((t) => t.Entity == asEntity.name && t.Item == asItem.InternalName);
            if (asEntity.transform.parent)
                asEntity = asEntity.transform.parent.gameObject;
            else
                asEntity = null;
        }
        if (c == null)
        {
            Message.ShowMessage(LangAdapter.FindEntry("Inventory", "UseItemDoesNotWork"));
            return;
        }
        Type.GetType(ClassName).GetMethod(c.Callback).Invoke(this, new object[] { c.Item, c.Entity });
        if (c.AutoDestroy)
        {
            if (asEntity.CompareTag("Area"))
                asEntity.layer = 2;
            saveManager.data.itemCallbacks.Remove(c);
        }
    }
    protected void RemoveUseItemCallback(string asName)
    {
        var c = saveManager.data.itemCallbacks.Find(c => c.InternalName == asName);
        if (c != null)
            saveManager.data.itemCallbacks.Remove(c);
    }
    protected void SetEntityActive(string asName, bool abActive)
    {
        if (asName.Contains("*"))
            for (int i = 0; i < 40; i++)
                SetEntityActive(asName.Replace("*", i.ToString()), abActive);
        else
        {
            var g = Finder.Find(asName);
            if (g == null)
                Debug.Log(asName + " does not exist in scene");
            else
                SetEntityActive(g, abActive);
        }
    }
    void SetEntityActive(GameObject g, bool abActive)
    {
        g.SetActive(abActive);
        g.AddComponent<SetEntityActiveCache>();
        saveManager.data.AddedComponent.SetValueSafety(SceneManager.GetActiveScene().name + "/" + g.name + "/" + nameof(SetEntityActiveCache), typeof(SetEntityActiveCache));
    }
    protected void SetEntityVisible(string asName, bool abVisible)
    {
        throw new NotImplementedException();
    }
    protected bool GetEntityExists(string asName)
    {
        return Finder.Find(asName) != null;
    }
    protected void SetEntityCustomFocusCrossHair(string asName, string asCrossHair)
    {
        Finder.Find(asName).tag = asCrossHair;
        saveManager.data.customCrosshairs.Add(SceneManager.GetActiveScene().name + "/" + asName, asCrossHair);
    }
    protected void CreateEntityAtArea(string asEntityName, string asEntityFile, string asAreaName, bool abFullGameSave)
    {
        throw new NotImplementedException();
    }
    protected void ReplaceEntity(string asName, string asBodyName, string asNewEntityName, string asNewEntityFile, bool abFullGameSave)
    {
        throw new NotImplementedException();
    }
    protected void PlaceEntityAtEntity(string asName, string asTargetEntity, string asTargetBodyName, bool abUseRotation)
    {
        throw new NotImplementedException();
    }
    protected void SetEntityPos(string asName, float afX, float afY, float afZ)
    {
        Finder.Find(asName).transform.position = new Vector3(-afX, afY, afZ);
    }
    protected float GetEntityPosX(string asName)
    {
        return -Finder.Find(asName).transform.position.x;
    }
    protected float GetEntityPosY(string asName)
    {
        return Finder.Find(asName).transform.position.y;
    }
    protected float GetEntityPosZ(string asName)
    {
        return Finder.Find(asName).transform.position.z;
    }
    protected void SetEntityPlayerLookAtCallback(string asName, string asCallback, bool abRemoveWhenLookedAt)
    {
        var g = Finder.Find(asName);
        if (g == null)
        {
            Debug.Log(asName + " does not exist in scene");
            return;
        }
        Sign s;
        if ((s = g.GetComponentInChildren<Sign>()) == null)
        {
            s = g.AddComponent<Sign>();
            saveManager.data.AddedComponent.SetValueSafety(SceneManager.GetActiveScene().name + "/" + asName + "/" + nameof(Sign), typeof(Sign));
        }
        s.playerLookAtCallback = asCallback;
        s.autoRemove = abRemoveWhenLookedAt;
        s.Handler(PlayerController.instance.scannedObject);
    }
    protected void SetEntityPlayerInteractCallback(string asName, string asCallback, bool abRemoveOnInteraction)
    {
        var g = Finder.Find(asName);
        if (g == null)
        {
            Debug.Log(asName + " does not exist in scene");
            return;
        }
        Interactable i;
        if ((i = g.GetComponentInChildren<Interactable>()) == null)
        {
            i = g.AddComponent<Button>();
            saveManager.data.AddedComponent.SetValueSafety(SceneManager.GetActiveScene().name + "/" + asName + "/" + nameof(Button), typeof(Button));
            i.RecordRoot();
        }
        i.SetCallback(asCallback);
        i.removeCallbackAfterInteraction = abRemoveOnInteraction;
    }
    protected void SetEntityCallbackFunc(string asName, string asCallback)
    {
        SetEntityPlayerInteractCallback(asName, asCallback, false);
    }
    protected void SetEntityConnectionStateChangeCallback(string asName, string asCallback)
    {
        throw new NotImplementedException();
        var s = Finder.Find(asName).GetComponentInChildren<IState>();
        if (s == null)
        {
            Debug.Log(asName + " does not contain IState");
        }
        s.OnStateChanged += (i) =>
        {
            Type.GetType(ClassName).GetMethod(asCallback).Invoke(this, new object[] { asName, i });
        };
    }
    protected void SetEntityInteractionDisabled(string asName, bool abDisabled)
    {
        if (asName.Contains("*"))
            for (int i = 0; i < 40; i++)
                SetEntityInteractionDisabled(asName.Replace("*", i.ToString()), abDisabled);
        else
        {
            var g = Finder.Find(asName);
            if (g == null)
            {
                Debug.Log(asName + " not exist in scene");
                return;
            }
            var interactable = g.GetComponentInChildren<Interactable>();
            if (interactable == null)
            {
                Debug.Log(asName + " does not contain Interactable");
                return;
            }
            interactable.enabled = abDisabled;
        }
    }
    protected void BreakJoint(string asName)
    {
        throw new NotImplementedException();
    }

    protected void AddEntityCollideCallback
        (string asParentName, string asChildName, string asFunction, bool abDeleteOnCollide, int alStates)
    {
        var g = Finder.Find(asChildName);
        bool inverted = false;
        if (g.CompareTag("Area"))
        {
            inverted = true;
            var buf = asParentName;
            asParentName = asChildName;
            asChildName = buf;
        }
        g = Finder.Find(asParentName);
        ////ТЕПЕРЬ asParentName ВСЕГДА ЗОНА, НА КОТОРУЮ НУЖНО НАВЕСИТЬ ТРИГГЕР
        var c = g.AddComponent<SimpleTrigger>();
        saveManager.data.AddedComponent.SetValueSafety(SceneManager.GetActiveScene().name + "/" + asParentName + "/" + nameof(SimpleTrigger), typeof(SimpleTrigger));
        c.isInverted = inverted;
        c.target = asChildName;
        c.destroyAfterTrigger = abDeleteOnCollide;
        c.mode = alStates;
        c.Callback = asFunction;
    }
    protected void RemoveEntityCollideCallback(string asParentName, string asChildName)
    {
        var g = Finder.Find(asChildName).GetComponent<SimpleTrigger>();
        if (g != null)
            g.Callback = "";
        else
            Finder.Find(asParentName).GetComponent<SimpleTrigger>().Callback = "";
    }
    protected bool GetEntitiesCollide(string asEntityA, string asEntityB)
    {
        throw new NotImplementedException();
    }
    protected void SetBodyMass(string asName, float afMass)
    {
        throw new NotImplementedException();
    }
    protected float GetBodyMass(string asName)
    {
        throw new NotImplementedException();
    }
    ////////PROPS///////////
    protected void SetPropEffectActive(string asName, bool abActive, bool abFadeAndPlaySounds)
    {
        throw new NotImplementedException();
    }
    public async void SetPropActiveAndFade(string asName, bool abActive, float afFadeTime)
    {
        if (asName.Contains("*"))
            for (int i = 0; i < 40; i++)
                SetPropActiveAndFade(asName.Replace("*", i.ToString()), abActive, afFadeTime);
        else
        {
            var g = Finder.Find(asName);
            if (g == null)
            {
                Debug.Log(asName + " does not exist in scene");
                return;
            }
            var materials = new List<Material>();
            foreach (var i in g.GetComponentsInChildren<MeshRenderer>(true))
                if (!materials.Contains(i.material))
                    materials.Add(i.material);

            foreach (var i in g.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                if (!materials.Contains(i.material))
                    materials.Add(i.material);

            if (abActive)
            {
                SetEntityActive(g, true);
                foreach (var i in materials)
                    StartCoroutine(FadeMaterial(i, abActive, afFadeTime));
            }
            else
            {
                foreach (var i in materials)
                    StartCoroutine(FadeMaterial(i, abActive, afFadeTime));
                await Task.Delay((int)(afFadeTime * 1000));
                SetEntityActive(g, false);
            }
        }
    }
    IEnumerator FadeMaterial(Material material, bool abActive, float afFadeTime)
    {
        float startCutoff, endCutoff;
        if (abActive)
        {
            startCutoff = 1;
            endCutoff = 0.75f;
        }
        else
        {
            startCutoff = 0.75f;
            endCutoff = 1;
        }
        float i = 0;
        material.SetFloat("_Cutoff", startCutoff);
        yield return null;
        i = Time.deltaTime / afFadeTime;
        while (i < 1)
        {            
            material.SetFloat("_Cutoff", Mathf.Lerp(startCutoff, endCutoff, i));
            yield return null;
            i += Time.deltaTime / afFadeTime;
        }
        material.SetFloat("_Cutoff", endCutoff);
    }
    protected void SetPropStaticPhysics(string asName, bool abX)
    {
        throw new NotImplementedException();
    }
    protected bool GetPropIsInteractedWith(string asName)
    {
        throw new NotImplementedException();
    }
    protected void RotatePropToSpeed
        (string asName, float afAcc, float afGoalSpeed, float afAxisX, float afAxisY, float afAxisZ, bool abResetSpeed, string asOffsetArea)
    {
        throw new NotImplementedException();
    }
    protected void StopPropMovement(string asName)
    {
        throw new NotImplementedException();
    }
    protected void AddAttachedPropToProp
        (string asPropName, string asAttachName, string asAttachFile, float afPosX, float afPosY, float afPosZ, float afRotX, float afRotY, float afRotZ)
    {
        throw new NotImplementedException();
    }
    protected void AttachPropToProp
        (string asPropName, string asAttachName, string asAttachFile, float afPosX, float afPosY, float afPosZ, float afRotX, float afRotY, float afRotZ)
    {
        throw new NotImplementedException();
    }
    protected void RemoveAttachedPropFromProp(string asPropName, string asAttachName)
    {
        throw new NotImplementedException();
    }
    protected void SetPropHealth(string asName, float afHealth)
    {
        if (afHealth == 0)
        {
            Finder.Find(asName).GetComponentInChildren<Breakable>().Break();
        }
        else
            throw new NotImplementedException();
    }
    protected void AddPropHealth(string asName, float afHealth)
    {
        throw new NotImplementedException();
    }
    protected float GetPropHealth(string asName)
    {
        throw new NotImplementedException();
    }
    protected void ResetProp(string asName)
    {
        throw new NotImplementedException();
    }
    protected void PlayPropAnimation(string asProp, string asAnimation, float afFadeTime, bool abLoop, string asCallback)
    {
        throw new NotImplementedException();
    }    
    Rigidbody FindBody(string name)
    {
        var s = name.Split('_');
        var entityName = "";
        string bodyName = "";
        for (int i = 0; i < s.Length; i++)
        {
            if (string.IsNullOrEmpty(bodyName) && !s[i].StartsWith("Body"))
                entityName += s[i] + "_";
            else
                bodyName += s[i] + "_";
        }
        entityName = entityName.Substring(0, entityName.Length - 1);
        bodyName = bodyName.Substring(0, bodyName.Length - 1);

        var g = Finder.Find(entityName);
        if (g == null)
        {
            Debug.Log(entityName + " does not exist in scene");
            return null;
        }
        g = g.transform.Find(bodyName).gameObject;

        Rigidbody rb = null;
        if ((rb = g.GetComponent<Rigidbody>()) == null)
            foreach (var i in g.GetComponentsInChildren<Rigidbody>())
                if (!i.isKinematic)
                    return i;

        return rb;
    }
    protected void AddPropForce(string asName, float afX, float afY, float afZ, string asCoordSystem)
    {
        var g = Finder.Find(asName);
        if (g == null)
        {
            Debug.Log(asName + " does not exist in scene");
            return;
        }
        switch (asCoordSystem.ToLower())
        {
            case "world":
                foreach (var rb in g.GetComponentsInChildren<Rigidbody>())
                    rb.AddForce(new Vector3(-afX, afY, afZ) / 50, ForceMode.Impulse);
                return;
            case "local":
                foreach (var rb in g.GetComponentsInChildren<Rigidbody>())
                    rb.AddForce((rb.transform.right * -afX + rb.transform.up * afY + rb.transform.forward * afZ) / 50, ForceMode.Impulse);
                break;
        }
    }
    protected void AddPropImpulse(string asName, float afX, float afY, float afZ, string asCoordSystem)
    {
        var g = Finder.Find(asName);
        if (g == null)
        {
            Debug.Log(asName + " does not exist in scene");
            return;
        }
        switch (asCoordSystem.ToLower())
        {
            case "world":
                foreach (var rb in g.GetComponentsInChildren<Rigidbody>())
                    rb.AddForce(new Vector3(-afX, afY, afZ), ForceMode.VelocityChange);
                return;
            case "local":
                foreach (var rb in g.GetComponentsInChildren<Rigidbody>())
                    rb.AddForce((rb.transform.right * -afX + rb.transform.up * afY + rb.transform.forward * afZ), ForceMode.VelocityChange);
                break;
        }
    }
    protected void AddBodyForce(string asName, float afX, float afY, float afZ, string asCoordSystem)
    {
        var rb = FindBody(asName);
        if (rb == null)
            return;
        switch (asCoordSystem.ToLower())
        {
            case "world":
                rb.AddForce(new Vector3(-afX, afY, afZ) / 50, ForceMode.Impulse);
                return;
            case "local":
                rb.AddForce((rb.transform.right * -afX + rb.transform.up * afY + rb.transform.forward * afZ) / 50, ForceMode.Impulse);
                break;
        }
    }
    protected void AddBodyImpulse(string asName, float afX, float afY, float afZ, string asCoordSystem)
    {
        var rb = FindBody(asName);
        if (rb == null)
            return;
        switch (asCoordSystem.ToLower())
        {
            case "world":
                rb.AddForce(new Vector3(-afX, afY, afZ), ForceMode.VelocityChange);
                return;
            case "local":
                rb.AddForce((rb.transform.right * -afX + rb.transform.up * afY + rb.transform.forward * afZ), ForceMode.VelocityChange);
                break;
        }
    }
    ///////CONNECTION//////////
    protected void InteractConnectPropWithRope
        (string asName, string asPropName, string asRopeName, bool abInteractOnly, float afSpeedMul, float afToMinSpeed, float afToMaxSpeed, bool abInvert, int alStatesUsed)
    {
        throw new NotImplementedException();
    }
    protected void InteractConnectPropWithMoveObject
        (string asName, string asPropName, string asMoveObjectName, bool abInteractOnly, bool abInvert, int alStatesUsed)
    {
        throw new NotImplementedException();
    }
    public void ConnectEntities
        (string asName, string asMainEntity, string asConnectEntity, bool abInvertStateSent, int alStatesUsed, string asCallbackFunc, bool createCopyInSaveManager = true)
    {
        var g = Finder.Find(asMainEntity);
        if (g == null)
        {
            Debug.Log(asMainEntity + " does not exist in scene");
            return;
        }
        var main = g.GetComponentInChildren<IState>();
        if (main == null)
            throw new Exception(asMainEntity + " does not contain IState");

        g = Finder.Find(asConnectEntity);
        if (g == null)
        {
            Debug.Log(asConnectEntity + "does not exist in scene");
            return;
        }

        var connect = g.GetComponentInChildren<IState>();

        main.OnStateChanged += (i) => {
            if (i != 0 && (alStatesUsed == 0 || alStatesUsed == i))
            {
                i = abInvertStateSent ? -i : i;
                if (connect != null)
                    connect.SetState(i);
                if (!string.IsNullOrEmpty(asCallbackFunc))
                    Type.GetType(ClassName).GetMethod(asCallbackFunc).Invoke(this, new object[] { asName, asMainEntity, asConnectEntity, (int)i });
            }
        };

        if (createCopyInSaveManager)
            saveManager.data.entitiesConnections.Add(new EntitiesConnection
            {
                map = SceneManager.GetActiveScene().name,
                asCallbackFunc = asCallbackFunc,
                asConnectEntity = asConnectEntity,
                asMainEntity = asMainEntity,
                abInvertStateSent = abInvertStateSent,
                alStatesUsed = alStatesUsed,
                asName = asName
            });

    }
    /////////LAMPS/////////
    protected void SetLampLit(string asName, bool abLit, bool abEffects)
    {
        var g = Finder.Find(asName);
        if (g == null)
        {
            Debug.Log(asName + " not exist in scene");
            return;
        }
        var l = g.GetComponent<LampLitController>();
        if (l == null)
            l = g.GetComponentInChildren<LampLitController>();
        if (l == null)
        {
            Debug.Log(asName + " does not contain LampLitController");
            return;
        }
        if (abLit)
            l.Lit();
        else
            l.UnLit();
    }
    ////////DOORS////////
    protected void SetSwingDoorLocked(string asName, bool abLocked, bool abEffects)
    {
        var g = Finder.Find(asName);
        if (g == null)
        {
            Debug.Log(asName + " not exist in scene");
            return;
        }
        var door = g.GetComponentInChildren<Door>();
        if (door == null)
        {
            Debug.Log(asName + " does not contain Door");
            return;
        }
        if (abLocked)
            SetSwingDoorClosed(asName, true, abEffects);
        if (abEffects)
            door.locked = abLocked;
        else
            door._locked = abLocked;
    }
    protected void SetSwingDoorClosed(string asName, bool abClosed, bool abEffects)
    {
        Finder.Find(asName).GetComponentInChildren<Door>().SetClosed(abClosed, abEffects);
    }
    protected bool GetSwingDoorLocked(string asName)
    {
        throw new NotImplementedException();
    }
    protected bool GetSwingDoorClosed(string asName)
    {
        return Finder.Find(asName).GetComponentInChildren<Door>().IsNearToClose();
    }
    protected void SetSwingDoorDisableAutoClose(string asName, bool abDisableAutoClose)
    {
        Finder.Find(asName).GetComponentInChildren<Door>().AutoClose = !abDisableAutoClose;
    }
    protected int GetSwingDoorState(string asName)
    {
        throw new NotImplementedException();
    }
    protected void SetLevelDoorLocked(string asName, bool abLocked)
    {
        var g = Finder.Find(asName);
        if (g == null)
        {
            Debug.Log(asName + " does not exist in scene");
            return;
        }
        var d = g.GetComponentInChildren<LevelDoor>();
        if (d == null)
        {
            Debug.Log(asName + " does not contain LevelDoor");
            return;
        }
        d.locked = abLocked;
    }
    protected void SetLevelDoorLockedSound(string asName, string asSound)
    {
        var g = Finder.Find(asName);
        if (g == null)
        {
            Debug.Log(asName + " does not exist in scene");
            return;
        }
        var d = g.GetComponentInChildren<LevelDoor>();
        if (d == null)
        {
            Debug.Log(asName + " does not contain LevelDoor");
            return;
        }
        d.lockedSoundProperty = asSound;
    }
    protected void SetLevelDoorLockedText(string asName, string asTextCat, string asTextEntry)
    {
        var g = Finder.Find(asName);
        if (g == null)
        {
            Debug.Log(asName + " does not exist in scene");
            return;
        }
        var d = g.GetComponentInChildren<LevelDoor>();
        if (d == null)
        {
            Debug.Log(asName + " does not contain LevelDoor");
            return;
        }
        d.lockedTextCat = asTextCat;
        d.lockedTextEntry = asTextEntry;
    }
    protected void SetMoveObjectState(string asName, float afState)
    {
        Finder.Find(asName).GetComponentInChildren<MovableObject>().SetState(afState);
    }
    protected void SetMoveObjectStateExt
        (string asName, float afState, float afAcc, float afMaxSpeed, float afSlowdownDist, bool abResetSpeed)
    {
        throw new NotImplementedException();
    }
    /////////LEVERS, WHEELS AND BUTTINS///////////
    protected void SetPropObjectStuckState(string asName, int alState)
    {
        if (asName.Contains("*"))
            for (int i = 0; i < 40; i++)
                SetPropObjectStuckState(asName.Replace("*", "") + i, alState);
        else
        {
            var g = Finder.Find(asName);
            if (g == null)
            {
                Debug.Log(asName + " does not exist in scene");
            }
            else
                g.GetComponentInChildren<IState>().SetStuckState(alState);
        }
    }
    protected void SetWheelStuckState(string asName, int alState, bool abEffects)
    {
        Finder.Find(asName).GetComponentInChildren<Wheel>().SetStuckState(alState);
        //if (abEffects)
        //    throw new NotImplementedException();
    }
    protected void SetLeverStuckState(string asName, int alState, bool abEffects)
    {
        if (abEffects)
            throw new NotImplementedException();
        var g = Finder.Find(asName);
        if (g == null)
        {
            Debug.Log(asName + " not exist in scene");
            return;
        }
        g.GetComponentInChildren<Lever>().SetStuckState(alState);
    }
    protected void SetWheelAngle(string asName, float afAngle, bool abAutoMove)
    {
        throw new NotImplementedException();
    }
    protected void SetWheelInteractionDisablesStuck(string asName, bool abX)
    {
        throw new NotImplementedException();
    }
    protected void SetLeverInteractionDisablesStuck(string asName, bool abX)
    {
        throw new NotImplementedException();
    }
    protected int GetLeverState(string asName)
    {
        throw new NotImplementedException();
    }
    protected void SetMultiSliderStuckState(string asName, int alStuckState, bool abEffects)
    {
        throw new NotImplementedException();
    }
    protected void SetMultiSliderCallback(string asName, string asCallback)
    {
        throw new NotImplementedException();
    }
    protected void SetButtonSwitchedOn(string asName, bool abSwitchedOn, bool abEffects)
    {
        throw new NotImplementedException();
    }
    /////////STICKY AREAS////////////
    protected void SetAllowStickyAreaAttachment(bool abX)
    {
        throw new NotImplementedException();
    }
    protected void AttachPropToStickyArea(string asAreaName, string asProp)
    {
        throw new NotImplementedException();
    }
    protected void AttachBodyToStickyArea(string asAreaName, string asBody)
    {
        throw new NotImplementedException();
    }
    protected void DetachFromStickyArea(string asAreaName)
    {
        throw new NotImplementedException();
    }
    //////////ENEMIES/////////////
    protected void SetNPCAwake(string asName, bool abAwake, bool abEffects)
    {
        throw new NotImplementedException();
    }
    protected void SetNPCFollowPlayer(string asName, bool abX)
    {
        throw new NotImplementedException();
    }
    protected void SetEnemyDisabled(string asName, bool abDisabled)
    {
        throw new NotImplementedException();
    }
    protected void SetEnemyIsHallucination(string asName, bool abX)
    {
        throw new NotImplementedException();
    }
    protected void FadeEnemyToSmoke(string asName, bool abPlaySound)
    {
        throw new NotImplementedException();
    }
    protected void ShowEnemyPlayerPosition(string asName)
    {
        throw new NotImplementedException();
    }
    protected void AlertEnemyOfPlayerPresence(string asName)
    {
        throw new NotImplementedException();
    }
    protected void SetEnemyDisableTriggers(string asName, bool abX)
    {
        throw new NotImplementedException();
    }
    protected void AddEnemyPatrolNode(string asName, string asNodeName, float afWaitTime, string asAnimation)
    {
        var g = Finder.Find(asName);
        if (g == null)
        {
            Debug.Log(asName + " do not exist in scene");
            return;
        }        
        var pos = Finder.Find(asNodeName);
        if (pos == null)
        {
            Debug.Log(asNodeName + " do not exist in scene");
            return;
        }
        var enemy = g.GetComponentInChildren<Enemy>();
        if (enemy == null)
        {
            Debug.Log(asName + " does not contain Enemy component");
            return;
        }
        enemy.AddEnemyPatrolNode(pos.transform.position, afWaitTime);
        if (!string.IsNullOrEmpty(asAnimation))
            throw new NotImplementedException();
    }
    protected void ClearEnemyPatrolNodes(string asEnemyName)
    {
        throw new NotImplementedException();
    }
    protected void SetEnemySanityDecreaseActive(string asName, bool abX)
    {
        throw new NotImplementedException();
    }
    protected void TeleportEnemyToNode(string asEnemyName, string asNodeName, bool abChangeY)
    {
        throw new NotImplementedException();
    }
    protected void TeleportEnemyToEntity(string asEnemyName, string asTargetEntity, string asTargetBody, bool abChangeY)
    {
        throw new NotImplementedException();
    }
    protected void ChangeManPigPose(string asName, string asPoseType)
    {
        throw new NotImplementedException();
    }
    protected void SetTeslaPigFadeDisabled(string asName, bool abX)
    {
        throw new NotImplementedException();
    }
    protected void SetTeslaPigSoundDisabled(string asName, bool abX)
    {
        throw new NotImplementedException();
    }
    protected void SetTeslaPigEasyEscapeDisabled(string asName, bool abX)
    {
        throw new NotImplementedException();
    }
    protected void ForceTeslaPigSighting(string asName)
    {
        throw new NotImplementedException();
    }
    protected string GetEnemyStateName(string asName)
    {
        throw new NotImplementedException();
    }
    IEnumerator LightFader(Light light, Color color, float radius, float time)
    {
        if (time <= 0)
        {
            light.color = color;
            light.range = radius == -1 ? light.range : radius;
            lightFaders.Remove(light.gameObject.name);
            yield break;
        }
        var startColor = light.color;
        var startRadius = light.range;
        float i = 0;
        while (i < 1)
        {
            light.color = Color.Lerp(startColor, color, i);
            if (radius != -1)
                light.range = Mathf.Lerp(startRadius, radius, i);
            yield return null;
            i += Time.deltaTime / time;
        }
        light.color = color;
        light.range = radius == -1 ? light.range : radius;
        lightFaders.Remove(light.gameObject.name);
    }
    private void OnApplicationQuit()
    {
#if UNITY_EDITOR
        if (saveManager != null)
        {
            saveManager.SaveCurrentScene();
            saveManager.SaveDataOnDisk();
            saveManager = null;
        }
#endif
    }

    public void OnLoad(Data data)
    {
        if (currentScenario == this as Scenario)
        {
            string clipName;
            float volume;
            data.StringKeys.TryGetValue(this.GetHierarchyPath(), out clipName, "");
            data.FloatKeys.TryGetValue(this.GetHierarchyPath(), out volume, 1);
            if (!string.IsNullOrEmpty(clipName))
                PlayMusic(clipName, true, volume, 1, 0, true);
        }
    }

    public void OnSave(Data data)
    {
        if (currentScenario == this as Scenario)
        {
            var s = musicSources.Find(s => s.priority == 1);
            if (s != null)
            {
                data.StringKeys.SetValueSafety(this.GetHierarchyPath(), s.clip.name);
                data.FloatKeys.SetValueSafety(this.GetHierarchyPath(), s.volume / 0.85f);
            }
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISave
{
    public void OnLoad(Data data);
    public void OnSave(Data data);
}

[Serializable]
public class SmartDictionary<T>
{
    [SerializeField]
    public Dictionary<string, T> Tuples;

    public SmartDictionary()
    {
        Tuples = new Dictionary<string, T>();
    }
    public void SetValueSafety(string key, T value)
    {
        if (Tuples.ContainsKey(key))
            Tuples.Remove(key);
        Tuples.Add(key, value);
    }
    public bool TryGetValue(string key, out T output, T defaultValue)
    {
        if (Tuples.TryGetValue(key, out var temp))
        {
            output = temp;
            return true;
        }
        output = defaultValue;
        return false;
    }
    public IEnumerable<Tuple<string, T>> GetValuesStartsWith(string prefix)
    {
        foreach (var i in Tuples)
            if (i.Key.StartsWith(prefix))
                yield return new Tuple<string, T>(i.Key.Replace(prefix, ""), i.Value);
    }

}
[Serializable]
public class EntitiesConnection
{
    public string map;
    public string asName;
    public string asMainEntity;
    public string asConnectEntity;
    public bool abInvertStateSent;
    public int alStatesUsed;
    public string asCallbackFunc;
}
[Serializable]
public class SoundSave
{
    public string asSoundName;
    public string asSoundFile;
    public string asEntity;
    public float afFadeTime;
    public string map;
}
[Serializable]
public class Data
{
    [SerializeField]
    public List<SoundSave> soundSaves;
    [SerializeField]
    public List<EntitiesConnection> entitiesConnections;
    [SerializeField]
    public List<Scenario.UseItemCallback> itemCallbacks;
    [SerializeField]
    public List<Tuple<string, string>> inventoryItems;
    [SerializeField]
    public Dictionary<string, string> customCrosshairs;
    [SerializeField]
    public SmartDictionary<Type> AddedComponent;
    [SerializeField]
    public SmartDictionary<int> IntKeys;
    [SerializeField]
    public SmartDictionary<float> FloatKeys;
    [SerializeField]
    public SmartDictionary<bool> BoolKeys;
    [SerializeField]
    public SmartDictionary<string> StringKeys;
    [SerializeField]
    public SmartDictionary<Vector3> VectorKeys;
    [SerializeField]
    public List<string> locationFlags;
    public Data(bool createEmpty)
    {
        if (createEmpty)
        {
            itemCallbacks = new List<Scenario.UseItemCallback>();
            AddedComponent = new SmartDictionary<Type>();
            IntKeys = new SmartDictionary<int>();
            FloatKeys = new SmartDictionary<float>();
            BoolKeys = new SmartDictionary<bool>();
            StringKeys = new SmartDictionary<string>();
            VectorKeys = new SmartDictionary<Vector3>();
            locationFlags = new List<string>();
            customCrosshairs = new Dictionary<string, string>();
            inventoryItems = new List<Tuple<string, string>>();
            entitiesConnections = new List<EntitiesConnection>();
            soundSaves = new List<SoundSave>();
        }
    }
}
public class SaveManager
{    
    public Data data;
    string path;
    public SaveManager()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, "save");
#else
        path = Path.Combine(Application.dataPath, "save");
#endif
        if (File.Exists(path))
            data = JsonConvert.DeserializeObject<Data>(File.ReadAllText(path));
        else
            data = new Data(true);
    }
    public bool LoadCurrentScene()
    {
        var activeScene = SceneManager.GetActiveScene();
        if (!data.locationFlags.Contains(activeScene.name))
            return false;

        foreach (var i in data.AddedComponent.GetValuesStartsWith(activeScene.name + "/"))
        {
            var component = Finder.Find(i.Item1.Split('/')[0]).AddComponent(i.Item2);
            if (component is Interactable)
                (component as Interactable).RecordRoot();
        }

        foreach (var i in data.entitiesConnections.FindAll(c => c.map == activeScene.name))
            Scenario.currentScenario.ConnectEntities(i.asName, i.asMainEntity, i.asConnectEntity, i.abInvertStateSent, i.alStatesUsed, i.asCallbackFunc, false);

        foreach (var i in data.soundSaves.FindAll(c => c.map == activeScene.name))
            Scenario.currentScenario.PlaySoundAtEntity(i.asSoundName, i.asSoundFile, i.asEntity, i.afFadeTime, true);

        foreach (var i in data.customCrosshairs.GetValuesStartsWith(activeScene.name + "/"))
            Finder.Find(i.Item1).tag = i.Item2;

        foreach (var i in data.itemCallbacks)
        {
            var g = Finder.Find(i.Entity);
            if (g != null)
                g.layer = 0;
        }

        foreach (var i in activeScene.GetRootGameObjects())
            foreach (var j in i.GetComponentsInChildren<ISave>(true))
                j.OnLoad(data);
        return true;
    }
    public void SaveCurrentScene()
    {
        if (data.locationFlags.Contains(SceneManager.GetActiveScene().name))
            data.locationFlags.Remove(SceneManager.GetActiveScene().name);
        data.locationFlags.Add(SceneManager.GetActiveScene().name);
        foreach (var i in SceneManager.GetActiveScene().GetRootGameObjects())
            foreach (var j in i.GetComponentsInChildren<ISave>(true))
                j.OnSave(data);
    }
    public void SaveDataOnDisk()
    {
        File.WriteAllText(path, JsonConvert.SerializeObject(data));
    }
}

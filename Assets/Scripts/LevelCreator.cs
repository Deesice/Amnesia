using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
//using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(FakeDatabase))]
public class LevelCreator : MonoBehaviour
{
#if UNITY_EDITOR
    XmlElement root;

    static string[] ignoreFiles = { "bb_lightyellow_lightray", "commentary_icon", "bb_white_halo" };

    private void Load()
    {
        var levelPath = "Assets/Resources/Maps/" + SceneManager.GetActiveScene().name + ".map";
        XmlDocument mainDoc = new XmlDocument();
        mainDoc.Load(levelPath);
        root = mainDoc.DocumentElement;
    }
    string ConvertPath(string input)
    {
        var s = input.Split('/');
        string result = "";
        for (int j = 0; j < s.Length - 1; j++)
        {
            var c = s[j][0];
            s[j] = s[j].Remove(0, 1);
            s[j] = s[j].Insert(0, ("" + c).ToUpper());
            result += s[j] + "/";
        }
        result += s[s.Length - 1].Replace(".dae", "").Replace(".ent", "");
        return result;
    }
    Vector3 StringToVector(string input)
    {
        input += " 0 0";
        input = input.Replace(".", ",");
        var s = input.Split(' ');
        return new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));
    }
    public void ClearLevel()
    {
        while (transform.childCount != 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }
    IEnumerable<GameObject> SpawnStaticObjects()
    {
        List<string> pathes = new List<string>();

        foreach (XmlNode i in root.GetAttributes("MapData", "MapContents", "FileIndex_StaticObjects"))
            pathes.Add(i.Attributes.GetNamedItem("Path").InnerText);

        foreach (XmlNode i in root.GetAttributes("MapData", "MapContents", "StaticObjects"))
            yield return FindAndSpawnPrefab(pathes, i);
    }
    IEnumerable<GameObject> SpawnPrimitives()
    {
        foreach (XmlNode i in root.GetAttributes("MapData", "MapContents", "Primitives"))
        {
            Vector3 scale = StringToVector(i.Attributes.GetNamedItem("EndCorner").InnerText);
            Vector3 position = StringToVector(i.Attributes.GetNamedItem("WorldPos").InnerText);
            position.x *= -1;
            Vector3 rotation = StringToVector(i.Attributes.GetNamedItem("Rotation").InnerText);
            rotation *= Mathf.Rad2Deg;

            var g = new GameObject();
            g.transform.position = position;

            var primitive = GameObject.CreatePrimitive(PrimitiveType.Plane);
            primitive.isStatic = true;
            primitive.name = i.Attributes.GetNamedItem("Name").InnerText;
            primitive.transform.localScale = scale * 0.1f + Vector3.up;
            primitive.transform.position = position + scale / 2;
            primitive.transform.SetParent(g.transform);
            var pos = primitive.transform.localPosition;
            pos.x *= -1;
            primitive.transform.localPosition = pos;

            g.transform.AlternateRotation(rotation);
            var s = primitive.AddComponent<SmartTiling>();
            s.tileAmount = StringToVector(i.Attributes.GetNamedItem("TileAmount").InnerText).x;
            s.tileOffset = StringToVector(i.Attributes.GetNamedItem("TileOffset").InnerText);
            primitive.transform.parent = null;
            primitive.layer = 11;
            DestroyImmediate(g);

            try { s.Compute(); } catch { }

            List<string> allPathes = new List<string>();

            foreach (var j in AssetDatabase.FindAssets(i.Attributes.GetNamedItem("Material").InnerText.Split('/').Last().Replace(".mat", "").Replace(" ", "")))
                allPathes.Add(AssetDatabase.GUIDToAssetPath(j));

            var p = allPathes.Find((a) => a.Contains(i.Attributes.GetNamedItem("Material").InnerText.Split('/').Last().Replace(" ", "")));

            if (p == null)
            {
                Debug.Log(i.Attributes.GetNamedItem("Material").InnerText + " DO NOT FIND");
            }
            else
            {
                var materialFile = File.ReadAllText(p).Split(' ');
                for (int j = 0; j < materialFile.Length; j++)
                    if (materialFile[j].Contains("PhysicsMaterial"))
                    {
                        var matter = primitive.AddComponent<Matter>();
                        matter.SetByString(materialFile[j].Replace("PhysicsMaterial", "").Replace("\"", "").Replace("=", ""));
                        break;
                    }
            }

            var mat = i.Attributes.GetNamedItem("Material").InnerText.Split('/').Last().Split('.')[0];
            Material material;

            List<string> pathes = new List<string>();
            foreach (var j in AssetDatabase.FindAssets(mat))
                pathes.Add(AssetDatabase.GUIDToAssetPath(j));

            pathes = pathes.FindAll((l) => l.Contains(" 1"));
            if (pathes.Count == 0)
            {
                yield return primitive;
                continue;
            }
            mat = pathes[0];

            try
            {
                material = AssetDatabase.LoadAssetAtPath<Material>(mat);
                if (material != null)
                    primitive.GetComponent<MeshRenderer>().material = material;
                else
                    Debug.Log(mat + " do not exist");
            }
            catch
            {
            }
            yield return primitive;
        }
    }    
    GameObject FindAndSpawnPrefab(List<string> pathes, XmlNode i)
    {
        var n = i.Attributes.GetNamedItem("FileIndex");
        if (n == null)
            return null;
        int fileIndex = int.Parse(n.InnerText);
        string name = i.Attributes.GetNamedItem("Name").InnerText;
        Vector3 position = StringToVector(i.Attributes.GetNamedItem("WorldPos").InnerText);
        Vector3 scale = StringToVector(i.Attributes.GetNamedItem("Scale").InnerText);
        if (scale.y == 0)
            scale.y = 1;
        Vector3 rotation = StringToVector(i.Attributes.GetNamedItem("Rotation").InnerText);
        rotation *= Mathf.Rad2Deg;
        position.x *= -1;
        var path = "Assets/Prefabs/" + ConvertPath(pathes[fileIndex]) + ".prefab";
        var g = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (g != null)
        {
            g = PrefabUtility.InstantiatePrefab(g) as GameObject;
            g.transform.position = position;
            g.transform.AlternateRotation(rotation);
            g.transform.localScale = scale;
            g.name = name;

            if (i.GetUserVariable("Lit") == "false")
            {
                var l = g.GetComponentInChildren<LampLitController>();
                if (l == null)
                    Debug.Log("Find unlit lamp, but LampLitController was lost");
                else
                    l.Relit();
            }

            g.SetUserVariable<MovableObject>("AngularOffsetArea", i, "angularOffsetAreaName");

            if (g.SetUserVariable<Interactable>("CallbackFunc", i, "Callback"))
                g.SetUserVariable<Interactable>("PlayerInteractCallbackAutoRemove", i, "removeCallbackAfterInteraction");
            else if (g.SetUserVariable<Interactable>("PlayerInteractCallback", i, "Callback"))
                g.SetUserVariable<Interactable>("PlayerInteractCallbackAutoRemove", i, "removeCallbackAfterInteraction");
            else g.SetUserVariable<Breakable>("CallbackFunc", i, "Callback");

            string s;
            if (!string.IsNullOrEmpty(s = i.GetUserVariable("NoteText")))
            {
                var c = g.GetComponentInChildren<PickUpObject>();
                if (c == null)
                    Debug.Log("WARNING: " + name + " does not contain PickUpObject");
                else
                {
                    var buf = c.item.ImageFile;
                    c.item = ScriptableObject.CreateInstance<Item>();
                    c.item.SubType = Item.EntitySubType.Note;
                    c.item.NameInLangFile = s;
                    c.item.ImageFile = buf;
                }
            }
            if (!string.IsNullOrEmpty(s = i.GetUserVariable("DiaryText")))
            {
                var c = g.GetComponentInChildren<PickUpObject>();
                if (c == null)
                    Debug.Log("WARNING: " + name + " does not contain PickUpObject");
                else
                {
                    c.item = ScriptableObject.CreateInstance<Item>();
                    c.item.SubType = Item.EntitySubType.Diary;
                    c.item.NameInLangFile = s;
                }
            }

            g.SetUserVariable<Interactable>("DiaryCallback", i, "Callback");

            if (!string.IsNullOrEmpty(i.GetUserVariable("SpinDir")))
            {
                if (bool.Parse(i.GetUserVariable("OverrideDefaults")))
                {
                    g.SetUserVariable<Wheel>("MinLimit", i, "minLimit");
                    g.SetUserVariable<Wheel>("MaxLimit", i, "maxLimit");
                }
            }
            else if (g.SetUserVariable<Lever>("StuckState", i, "stuckState"))
            {
                if (bool.Parse(i.GetUserVariable("OverrideDefaults")))
                {
                    g.SetUserVariable<Lever>("MiddleAngleAmount", i, "middleAngleAmount");
                    if (bool.Parse(i.GetUserVariable("AutoMoveToAngle")))
                    {
                        switch (i.GetUserVariable("AutoMoveGoal"))
                        {
                            case "Middle":
                                g.GetComponentInChildren<Lever>().autoMoveGoal = Lever.AutoMoveGoal.Middle;
                                break;
                            case "Min":
                                g.GetComponentInChildren<Lever>().autoMoveGoal = Lever.AutoMoveGoal.Min;
                                break;
                            case "Max":
                                g.GetComponentInChildren<Lever>().autoMoveGoal = Lever.AutoMoveGoal.Max;
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    else
                        g.GetComponentInChildren<Lever>().autoMoveGoal = Lever.AutoMoveGoal.None;
                }
            }

            if (g.GetComponentInChildren<Door>() == null && g.SetUserVariable<LevelDoor>("Locked", i, "locked"))
            {
                g.SetUserVariable<LevelDoor>("MapFile", i, "nextLevel");
                g.SetUserVariable<LevelDoor>("StartPos", i, "startPos");
                g.SetUserVariable<Sign>("TextEntry", i, "textEntry");
                g.SetUserVariable<LevelDoor>("LockedTextCat", i, "lockedTextCat");
                g.SetUserVariable<LevelDoor>("LockedTextEntry", i, "lockedTextEntry");
                g.SetUserVariable<LevelDoor>("LockedSound", i, "lockedSoundProperty");
            }
            else
            {
                g.SetUserVariable<Door>("Locked", i, "_locked");
                g.SetUserVariable<Door>("OpenAmount", i, "openAmount");
            }

            g.SetUserVariable<Enemy>("Hallucination", i, "hallucination");
            g.SetUserVariable<Enemy>("HallucinationEndDist", i, "hallucinationEndDist");
        }
        else
        {
            bool flag = false;
            foreach (var s in ignoreFiles)
                if (path.Contains(s))
                    flag = true;
            if (!flag)
                Debug.Log(path + " do not exist");
        }

        
        return g;
    }    
    IEnumerable<GameObject> SpawnEntities()
    {
        List<string> pathes = new List<string>();

        foreach (XmlNode i in root.GetAttributes("MapData", "MapContents", "FileIndex_Entities"))
            pathes.Add(i.Attributes.GetNamedItem("Path").InnerText);

        Vector3 pos, color, rotation;
        float radius, fov;
        string name, path;
        GameObject g;
        Light l;

        foreach (XmlNode i in root.GetAttributes("MapData", "MapContents", "Entities"))
        {
            g = null;
            //Debug.Log(i.Name);
            switch (i.Name)
            {
                case "BoxLight":
                    color = StringToVector(i.Attributes.GetNamedItem("DiffuseColor").InnerText);
                    pos = StringToVector(i.Attributes.GetNamedItem("WorldPos").InnerText);
                    radius = StringToVector(i.Attributes.GetNamedItem("Scale").InnerText).magnitude / 2;
                    name = i.Attributes.GetNamedItem("Name").InnerText;
                    g = new GameObject();
                    pos.x *= -1;
                    g.transform.position = pos;
                    g.name = name;
                    //l = g.AddComponent<Light>();
                    //l.type = LightType.Point;
                    //l.range = radius;
                    //l.color = new Color(color.x, color.y, color.z);
                    //g.tag = "BoxLight";
                    g.AddComponent<BoxLight>().color = new Color(color.x, color.y, color.z);
                    g.layer = 2;
                    var b = g.AddComponent<BoxCollider>();
                    b.size = StringToVector(i.Attributes.GetNamedItem("Scale").InnerText);
                    b.isTrigger = true;
                    break;
                case "PointLight":
                    //Debug.Log("Spawn PointLight");
                    pos = StringToVector(i.Attributes.GetNamedItem("WorldPos").InnerText);
                    color = StringToVector(i.Attributes.GetNamedItem("DiffuseColor").InnerText);
                    radius = StringToVector(i.Attributes.GetNamedItem("Radius").InnerText).x;
                    name = i.Attributes.GetNamedItem("Name").InnerText;
                    g = new GameObject();
                    pos.x *= -1;
                    g.transform.position = pos;
                    g.name = name;
                    l = g.AddComponent<Light>();
                    l.type = LightType.Point;
                    l.range = radius;
                    l.color = new Color(color.x, color.y, color.z);
                    if (i.Attributes.GetNamedItem("CastShadows").InnerText == "true")
                        l.shadows = LightShadows.Soft;
                    break;
                case "SpotLight":
                    pos = StringToVector(i.Attributes.GetNamedItem("WorldPos").InnerText);
                    color = StringToVector(i.Attributes.GetNamedItem("DiffuseColor").InnerText);
                    radius = StringToVector(i.Attributes.GetNamedItem("Radius").InnerText).x;
                    rotation = StringToVector(i.Attributes.GetNamedItem("Rotation").InnerText);
                    rotation *= Mathf.Rad2Deg;
                    name = i.Attributes.GetNamedItem("Name").InnerText;
                    fov = StringToVector(i.Attributes.GetNamedItem("FOV").InnerText).x * Mathf.Rad2Deg;
                    g = new GameObject();
                    pos.x *= -1;
                    g.transform.position = pos;
                    //g.transform.rotation = Quaternion.Euler(0, 180, 0);
                    g.transform.AlternateRotation(rotation);
                    g.transform.RotateAround(g.transform.position, g.transform.up, 180);
                    g.transform.RotateAround(g.transform.position, g.transform.forward, 180);
                    g.name = name;
                    l = g.AddComponent<Light>();
                    l.type = LightType.Spot;
                    l.range = radius;
                    l.spotAngle = fov;
                    l.color = new Color(color.x, color.y, color.z);
                    if (i.Attributes.GetNamedItem("CastShadows").InnerText == "true")
                        l.shadows = LightShadows.Soft;

                    var gobo = i.Attributes.GetNamedItem("Gobo").InnerText.Split('/');
                    var cookie = AssetDatabase.LoadAssetAtPath<Texture>("Assets/lights/" + gobo[gobo.Length-1].Replace(".dds", ".png"));
                    if (cookie != null)
                        l.cookie = cookie;

                    break;
                case "ParticleSystem":
                    name = i.Attributes.GetNamedItem("Name").InnerText;
                    pos = StringToVector(i.Attributes.GetNamedItem("WorldPos").InnerText);
                    pos.x *= -1;
                    path = "Assets/Prefabs/Particles/" + i.Attributes.GetNamedItem("File").InnerText.Split('/').Last().Replace(".ps",".prefab");
                    color = Vector3.one;
                    if (path.Contains("_large"))
                    {
                        path = path.Replace("_large", "");
                        color *= 2;
                    }
                    //Debug.Log(path);
                    g = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (g != null)
                    {
                        //Debug.Log("SPAWNING: " + path);
                        g = PrefabUtility.InstantiatePrefab(g) as GameObject;
                        g.transform.position = pos;
                        g.transform.localScale = color;
                        g.name = name;
                    }
                    else
                    {
                        bool flag = false;
                        foreach (var s in ignoreFiles)
                            if (path.Contains(s))
                                flag = true;
                        if (!flag)
                            Debug.Log(path + " do not exist. Check " + name + " in HPL2Editor");
                    }
                    break;
                case "Billboard":
                    name = i.Attributes.GetNamedItem("Name").InnerText;
                    pos = StringToVector(i.Attributes.GetNamedItem("WorldPos").InnerText);
                    pos.x *= -1;
                    rotation = StringToVector(i.Attributes.GetNamedItem("Rotation").InnerText);
                    rotation *= Mathf.Rad2Deg;
                    color = StringToVector(i.Attributes.GetNamedItem("BillboardSize").InnerText);
                    color.z = color.x;
                    color.x = color.y;
                    color.y = color.z;
                    color.z = 1;
                    color *= 2;



                    path = "Assets/Prefabs/Billboards/" + i.Attributes.GetNamedItem("MaterialFile").InnerText.Replace(".mat",".prefab").Split('/').Last();
                    g = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (g != null)
                    {
                        g = PrefabUtility.InstantiatePrefab(g) as GameObject;
                        g.transform.position = pos;
                        g.transform.localScale = color;
                        g.transform.AlternateRotation(rotation);
                        g.transform.RotateAround(g.transform.position, g.transform.right, 90);

                        g.transform.position += g.transform.forward * color.x / 2;
                        //rotation = g.transform.rotation.eulerAngles;
                        //rotation.z = -90;
                        //g.transform.rotation = Quaternion.Euler(rotation);
                        g.name = name;
                    }
                    else
                    {
                        bool flag = false;
                        foreach (var s in ignoreFiles)
                            if (path.Contains(s))
                                flag = true;
                        if (!flag)
                            Debug.Log(path + " do not exist. Check " + name + " in HPL2Editor");
                    }
                    break;
                case "Area":
                    name = i.Attributes.GetNamedItem("Name").InnerText;
                    pos = StringToVector(i.Attributes.GetNamedItem("WorldPos").InnerText);
                    pos.x *= -1;
                    rotation = StringToVector(i.Attributes.GetNamedItem("Rotation").InnerText);
                    rotation *= Mathf.Rad2Deg;
                    color = StringToVector(i.Attributes.GetNamedItem("Scale").InnerText);
                    g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    g.transform.position = pos;
                    g.transform.localScale = color;
                    g.transform.AlternateRotation(rotation);
                    DestroyImmediate(g.GetComponent<MeshFilter>());
                    DestroyImmediate(g.GetComponent<MeshRenderer>());
                    g.GetComponent<BoxCollider>().isTrigger = true;
                    g.name = name;
                    g.tag = "Area";
                    g.layer = 2;
                    foreach (XmlNode j in i.ChildNodes[0].ChildNodes)
                    {
                        switch (j.Attributes.GetNamedItem("Name").InnerText)
                        {
                            case "PlayerInteractCallback":
                                var cal = j.Attributes.GetNamedItem("Value").InnerText;
                                if (!string.IsNullOrEmpty(cal))
                                    g.AddComponent<Button>().SetCallback(cal);
                                break;
                            case "PlayerInteractCallbackAutoRemove":
                                cal = j.Attributes.GetNamedItem("Value").InnerText;
                                if (g.GetComponent<Button>() != null)
                                    g.GetComponent<Button>().removeCallbackAfterInteraction = bool.Parse(cal);
                                break;
                            default:
                                break;
                        }
                    }
                    switch (i.Attributes.GetNamedItem("AreaType").InnerText)
                    {
                        case "Script":
                            if (!string.IsNullOrEmpty(i.GetUserVariable("PlayerLookAtCallback")))
                                g.AddComponent<Sign>().playerLookAtCallback = i.GetUserVariable("PlayerLookAtCallback");
                            break;
                        case "PlayerStart":
                            break;
                        case "Sign":
                            g.AddComponent<Sign>();
                            g.SetUserVariable<Sign>("TextCat", i, "textCategory");
                            g.SetUserVariable<Sign>("TextEntry", i, "textEntry");
                            break;
                        case "Flashback":
                            var flash = g.AddComponent<Flashback>();
                            XmlDocument mainDoc = new XmlDocument();
                            path = i.GetUserVariable("FlashbackFile").Replace(".flash", "");
                            if (path.Contains("flashback"))
                                path = path.Substring(path.IndexOf("flashback") + 10);
                            mainDoc.Load("Assets/flashbacks/" + path + ".flash");
                            var root = mainDoc.DocumentElement;
                            foreach (XmlNode j in root.GetAttributes("Voices"))
                            {
                                flash.voices.Add(j.Attributes.GetNamedItem("VoiceSound").InnerText.Split('/').Last());
                                flash.textEntries.Add(j.Attributes.GetNamedItem("TextEntry").InnerText);
                                if (flash.sfx == null)
                                    flash.sfx = Resources.Load<AudioClip>("sounds/flashback_sfx/" + j.Attributes.GetNamedItem("EffectSound").InnerText.Replace(".ogg",""));
                            }
                            g.SetUserVariable<Flashback>("Callback", i, "callback");
                            break;
                        case "Insanity":
                            g.AddComponent<InsanityArea>();
                            g.SetUserVariable<InsanityArea>("AutoDisable", i, "autoDisable");
                            break;
                        case "Sticky":
                            g.AddComponent<StickyArea>();
                            g.SetUserVariable<StickyArea>("MoveBody", i, "moveBody");
                            g.SetUserVariable<StickyArea>("RotateBody", i, "rotateBody");
                            g.SetUserVariable<StickyArea>("CheckCenterInArea", i, "checkCenterInArea");
                            g.SetUserVariable<StickyArea>("CcanDetach", i, "canDetach");
                            g.SetUserVariable<StickyArea>("PoseTime", i, "poseTime");
                            g.SetUserVariable<StickyArea>("AttachableBodyName", i, "attachableBodyName");
                            g.SetUserVariable<StickyArea>("AttachFunction", i, "attachFunction");
                            g.SetUserVariable<StickyArea>("DetachFunction", i, "detachFunction");
                            g.SetUserVariable<StickyArea>("AttachSound", i, "attachSound");
                            g.SetUserVariable<StickyArea>("DetachSound", i, "detachSound");
                            g.SetUserVariable<StickyArea>("AttachPS", i, "attachPS");
                            g.SetUserVariable<StickyArea>("DetachPS", i, "detachPS");
                            break;
                        case "PathNode":
                            break;
                        case "Ladder":
                            switch (i.GetUserVariable("Material"))
                            {
                                case "metal":
                                    g.AddComponent<LadderArea>().steps = AssetDatabase.LoadAssetAtPath<SoundLibrary>("Assets/Prefabs/SoundLibraries/MetalLadderSteps.asset");
                                    break;
                                default:
                                    g.AddComponent<LadderArea>().steps = AssetDatabase.LoadAssetAtPath<SoundLibrary>("Assets/Prefabs/SoundLibraries/WoodLadderSteps.asset");
                                    break;
                            }
                            break;
                        default:
                            Debug.Log(g.name + " have unknown area type");
                            break;
                    }
                    break;
                case "Entity":
                    g = FindAndSpawnPrefab(pathes, i);
                    break;
                case "Sound":
                    g = new GameObject();
                    g.name = i.Attributes.GetNamedItem("Name").InnerText;
                    pos = StringToVector(i.Attributes.GetNamedItem("WorldPos").InnerText);
                    pos.x *= -1;
                    g.transform.position = pos;
                    g.AddComponent<AudioSource>();
                    var container = g.AddComponent<SoundPropetyContainer>();
                    container.sntView = i.Attributes.GetNamedItem("SoundEntityFile").InnerText;
                    container.volume = float.Parse(i.Attributes.GetNamedItem("Volume").InnerText.Replace(".",","));
                    container.max = float.Parse(i.Attributes.GetNamedItem("MaxDistance").InnerText);
                    container.min = float.Parse(i.Attributes.GetNamedItem("MinDistance").InnerText);
                    container.useDefaults = bool.Parse(i.Attributes.GetNamedItem("UseDefault").InnerText);
                    break;
                default:
                    Debug.Log("UNREGISTER TYPE: " + i.Name);
                    //g = FindAndSpawnPrefab(pathes, i);
                    break;            
            }
            if (g)
                g.SetActive(bool.Parse(i.Attributes.GetNamedItem("Active").InnerText));
            yield return g;
        }
    }
    public IEnumerable<GameObject> CreateLevel()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = Color.black;
        RenderSettings.skybox = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/unlit.mat");

        transform.position = Vector3.zero;
        gameObject.name = "LevelCreator";
        ClearLevel();
        Load();

        FindObjectOfType<FakeDatabase>().CollectAll();

        var staticObjects = new GameObject();
        staticObjects.transform.position = Vector3.zero;
        staticObjects.name = "StaticObjects";
        staticObjects.transform.SetParent(transform);
        var primitives = new GameObject();
        primitives.transform.position = Vector3.zero;
        primitives.name = "Primitives";
        primitives.transform.SetParent(transform);
        var entities = new GameObject();
        entities.name = "Entities";
        entities.transform.position = Vector3.zero;
        entities.transform.SetParent(transform);

        foreach (var i in SpawnStaticObjects())
            if (i != null)
                i.transform.SetParent(staticObjects.transform);

        foreach (var i in SpawnEntities())
            if (i != null)
            {
                i.transform.SetParent(entities.transform);
                if (i.CompareTag("Area"))
                    yield return i;
            }

        foreach (var i in SpawnPrimitives())
            if (i != null)
                i.transform.SetParent(primitives.transform);

        foreach (var i in FindObjectsOfType<Interactable>(true))
            i.RecordRoot();
    }
#endif    
}
public static class ExtensionMethods
{
    public static bool SetUserVariable<T>(this GameObject g, string variableName, XmlNode node, string nameInComponent)
    {
        string s;
        if (!string.IsNullOrEmpty(s = node.GetUserVariable(variableName)))
        {
            var c = g.GetComponentInChildren<T>() as ReflectableMonoBehaviour;
            if (c == null)
                Debug.Log("WARNING: " + g.name + " does not contain " + typeof(T) + " or it's not ReflectableClass");
            else
            {
                if (typeof(T).GetField(nameInComponent) == null)
                {
                    Debug.Log("Type " + typeof(T) + " does not countain field " + nameInComponent);
                    return false;
                }
                switch (typeof(T).GetField(nameInComponent).FieldType.FullName)
                {
                    case "System.String":
                        c[nameInComponent] = s;
                        return true;
                    case "System.Single":
                        c[nameInComponent] = float.Parse(s.Replace(".",","));
                        return true;
                    case "System.Int32":
                        c[nameInComponent] = int.Parse(s);
                        return true;
                    case "System.Boolean":
                        c[nameInComponent] = bool.Parse(s);
                        return true;
                    default:
                        Debug.Log(nameInComponent + " in " + typeof(T) + " have unknown type");
                        break;
                }
            }
        }
        return false;
    }
    public static string GetHierarchyPath<T>(this T behaviour)
    {
        string output = "";
        var t = (behaviour as MonoBehaviour).transform;
        while (t != null)
        {
            output = t.gameObject.name + "/" + output;
            t = t.parent;
        }
        output += typeof(T);
        return SceneManager.GetActiveScene().name + "/" + output;
    }
    public static void AlternateRotation(this Transform trans, Vector3 rotation)
    {
        trans.rotation = Quaternion.AngleAxis(rotation.z, -Vector3.forward) *
            Quaternion.AngleAxis(rotation.y, -Vector3.up) *
            Quaternion.AngleAxis(rotation.x, Vector3.right);
    }
    public static string AddToOnEnter(this string destination, string input)
    {
        var index = destination.IndexOf("OnEnter()");
        var sub = destination.Substring(destination.IndexOf("OnEnter()"));
        return destination.Insert(index + sub.IndexOf("{") + 1, input);
    }
    public static string GetUserVariable(this XmlNode i, string key)
    {
        if (i.ChildNodes.Count != 0)
            foreach (XmlNode j in i.ChildNodes[0].ChildNodes)
                if (j.Attributes.GetNamedItem("Name").InnerText == key)
                    return j.Attributes.GetNamedItem("Value").InnerText;

        return string.Empty;
    }
    public static XmlNodeList GetAttributes(this XmlElement root, params string[] param)
    {
        if (param.Length <= 0)
            throw new System.Exception("SYKA!");

        var observableNode = root.ChildNodes;

        foreach (var s in param)
        {
            foreach (XmlNode node in observableNode)
            {
                if (node.Name == s)
                {
                    observableNode = node.ChildNodes;
                    break;
                }
            }
        }

        return observableNode;
    }
    public static void LoadFromSoundProperty(this AudioSource source, SoundProperty property)
    {
        source.minDistance = property.MinDistance;
        source.maxDistance = property.MinDistance;
        source.volume = property.Volume;
        source.loop = (property.Loop && property.Interval == 0);
        source.spatialBlend = property.Use3D ? 1 : 0;
    }
    public static IEnumerable<Tuple<string, T>> GetValuesStartsWith<T>(this Dictionary<string, T> dictionary, string prefix)
    {
        foreach (var i in dictionary)
            if (i.Key.StartsWith(prefix))
                yield return new Tuple<string, T>(i.Key.Replace(prefix, ""), i.Value);
    }
    public static Vector3 Average(this List<Vector3> list)
    {
        var v = Vector3.zero;
        foreach (var c in list)
            v += c;
        if (list.Count == 0)
            return Vector3.zero;
        else
            return v / list.Count;
    }
}

using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

class LangAdapter : MonoBehaviour
{
    static LangAdapter _instance;
    static LangAdapter instance
    {
        get { if (_instance == null) { _instance = new GameObject().AddComponent<LangAdapter>(); _instance.gameObject.name = "LangAdapter"; _instance.Load(); DontDestroyOnLoad(_instance.gameObject); } return _instance; }
    }
    public enum Language { Brazilian_portuguese, Chinese, English, French, German, Italian, Russian, Spanish}
    public static Language CurrentLanguage = Language.English;
    XmlElement mainRoot;
    XmlElement baseRoot;
    string[] mainPathes = {
        "Assets/Resources/brazilian_portuguese.lang",
        "Assets/Resources/chinese.lang",
        "Assets/Resources/english.lang",
        "Assets/Resources/french.lang",
        "Assets/Resources/german.lang",
        "Assets/Resources/italian.lang",
        "Assets/Resources/russian.lang",
        "Assets/Resources/spanish.lang"};
    string[] basePathes = {
        "Assets/Resources/base_brazilian_portuguese.lang",
        "Assets/Resources/base_chinese.lang",
        "Assets/Resources/base_english.lang",
        "Assets/Resources/base_french.lang",
        "Assets/Resources/base_german.lang",
        "Assets/Resources/base_italian.lang",
        "Assets/Resources/base_russian.lang",
        "Assets/Resources/base_spanish.lang"};
    void Load()
    {
        XmlDocument mainDoc = new XmlDocument();
        var t = Resources.Load<TextAsset>(mainPathes[(int)CurrentLanguage].Replace("Assets/Resources/", "").Replace(".lang",""));
        mainDoc.LoadXml(t.text);
        //mainDoc.Load(mainPathes[(int)language]);
        mainRoot = mainDoc.DocumentElement;

        XmlDocument baseDoc = new XmlDocument();
        t = Resources.Load<TextAsset>(basePathes[(int)CurrentLanguage].Replace("Assets/Resources/", "").Replace(".lang", ""));
        baseDoc.LoadXml(t.text);
        //baseDoc.Load(basePathes[(int)language]);
        baseRoot = baseDoc.DocumentElement;
    }
    public static string FindEntry(string categoryName, string entryName)
    {
        foreach (var i in FindEntries(categoryName, entryName))
            return i;
        return null;
    }
    public static List<string> FindEntries(string categoryName, string entryName)
    {
        var output = instance.FindEntries(categoryName, entryName, instance.mainRoot);
        if (output.Count == 0)
            return instance.FindEntries(categoryName, entryName, instance.baseRoot);
        else
            return output;
    }
    List<string> FindEntries(string categoryName, string entryName, XmlElement root)
    {
        List<string> list = new List<string>();
        foreach (XmlNode xnode in root)
        {
            // получаем атрибут name
            if (xnode.Attributes.Count > 0)
            {
                XmlNode attr = xnode.Attributes.GetNamedItem("Name");
                if (attr != null && attr.Value == categoryName)
                {
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        attr = childnode.Attributes.GetNamedItem("Name");
                        if (attr != null && attr.Value == entryName)
                            foreach (var i in childnode.InnerText.Replace("[voice ", "&[voice ").Replace("[new_page]", "&").Split('&'))
                                if (!string.IsNullOrEmpty(i))
                                    list.Add(ProcessText(i));
                    }
                }
            }
        }
        return list;
    }
    string ProcessText(string input)
    {
        string output = "";
        foreach (var s in input.Split('[', ']'))
        {
            if (s.Length == 0)
                continue;
            if (s[0] == 'u')
            {
                output += Char.ConvertFromUtf32(Int32.Parse(s.Replace("u", "")));
                continue;
            }
            if (s == "br")
            {
                output += "\n";
                continue;
            }
            output += s;
        }
        return output;
    }
}
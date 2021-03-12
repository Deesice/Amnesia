using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class SoundProperty : ScriptableObject, IRandomClip
{
	public List<string> clipNames = new List<string>();
	List<AudioClip> Clips;
	public float Volume;
	public float MinDistance;
	public float MaxDistance;
	public float Random;
	public float Interval;
	public bool FadeEnd;
	public bool FadeStart;
	public bool Stream;
	public bool Loop;
	public bool Use3D;
	public bool Blockable;
	public float BlockVolumeMul;
	public float Priority;
	public void LoadFromSNT(string sntPath)
	{
		XmlDocument mainDoc = new XmlDocument();
        try
        {
			mainDoc.Load(sntPath);
		}
        catch (Exception ex)
        {
			Debug.Log(sntPath + " loading error: " + ex.Message);
			return;
        }
		
		var root = mainDoc.DocumentElement;

		var pathes = sntPath.Split('\\');
		this.name = pathes[pathes.Length - 1];
		string folder = "";
		for (int i = 0; i < pathes.Length - 1; i++)
			folder += pathes[i] + "\\";

		clipNames.Clear();

		foreach (XmlNode i in root.GetAttributes("SOUNDS", "Main"))
		{
			clipNames.Add(i.Attributes.GetNamedItem("File").InnerText.Replace(".ogg", "") + ".ogg");
		}

		//Debug.Log(clipNames.Count);

		foreach (XmlNode node in root)
		{
			if (node.Name == "PROPERTIES")
			{
				Volume = float.Parse(node.Attributes.GetNamedItem("Volume").InnerText.Replace(".", ","));
				MinDistance = float.Parse(node.Attributes.GetNamedItem("MinDistance").InnerText.Replace(".", ","));
				MaxDistance = float.Parse(node.Attributes.GetNamedItem("MaxDistance").InnerText.Replace(".", ","));
				Random = float.Parse(node.Attributes.GetNamedItem("Random").InnerText.Replace(".", ","));
				Interval = float.Parse(node.Attributes.GetNamedItem("Interval").InnerText.Replace(".", ","));
				FadeEnd = bool.Parse(node.Attributes.GetNamedItem("FadeEnd").InnerText);
				FadeStart = bool.Parse(node.Attributes.GetNamedItem("FadeStart").InnerText);
				Stream = bool.Parse(node.Attributes.GetNamedItem("Stream").InnerText);
				Loop = bool.Parse(node.Attributes.GetNamedItem("Loop").InnerText);
				Use3D = bool.Parse(node.Attributes.GetNamedItem("Use3D").InnerText);
				Blockable = bool.Parse(node.Attributes.GetNamedItem("Blockable").InnerText);
				BlockVolumeMul = float.Parse(node.Attributes.GetNamedItem("BlockVolumeMul").InnerText.Replace(".", ","));
				Priority = float.Parse(node.Attributes.GetNamedItem("Priority").InnerText.Replace(".", ","));
				break;
			}
		}
#if UNITY_EDITOR
		//PrefabUtility.RecordPrefabInstancePropertyModifications(this);
#endif
	}
	public void Preload()
    {
		if (Clips != null)
			return;
		Clips = new List<AudioClip>();
		AudioClip clip;
		foreach (var s in clipNames)
		{
			clip = FakeDatabase.FindClip(s);
			if (clip == null)
				clip = FakeDatabase.FindVoice(s.Split('/').Last());
			Clips.Add(clip);
		}
    }

	public AudioClip GetClip()
	{
		return Clips[UnityEngine.Random.Range(0, Clips.Count)];
	}

    public float GetVolume()
    {
		return Volume;
    }
}

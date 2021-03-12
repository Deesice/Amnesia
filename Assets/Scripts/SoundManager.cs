using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	static SoundManager _instance;
	static SoundManager Instance
	{
		get { if (_instance == null) { _instance = new GameObject().AddComponent<SoundManager>(); _instance.gameObject.name = "SoundManager"; DontDestroyOnLoad(_instance.gameObject); } return _instance; }
	}

	List<AudioSource> managingSources = new List<AudioSource>();
	List<AudioSource> impactSources = new List<AudioSource>();
	Dictionary<AudioSource, IEnumerator> volumeFaders = new Dictionary<AudioSource, IEnumerator>();
	Dictionary<string, GameObject> sounds = new Dictionary<string, GameObject>();
	IEnumerator listenerCoroutine = null;
	public static void AddToManagingSources(AudioSource source)
	{		
		Instance.managingSources.Add(source);
	}
	public static IEnumerable<AudioSource> GetUnmanagingSources()
    {
		foreach (var i in Instance.GetComponents<AudioSource>())
			if (!Instance.managingSources.Contains(i))
				yield return i;
    }
    private void OnLevelWasLoaded(int level)
    {
		//foreach (var i in volumeFaders)
		//	StopCoroutine(i.Value);
		//volumeFaders.Clear();
		//var a = GetComponents<AudioSource>();
		//foreach (var i in volumeFaders)
		//	if (!a.Contains(i.Key))
		//          {
		//		StopCoroutine(i.Value);
		//		volumeFaders.Remove(i.Key);
		//          }
		ManipulateAudioListener(1, 0);
		sounds.Clear();
		impactSources.Clear();
	}
    public static AudioSource PlaySound(IRandomClip lib)
    {
		if (lib == null)
			return null;
		return PlayClip(lib.GetClip(), false, lib.GetVolume());
    }
	public static AudioSource PlayClip(AudioClip clip, bool loop = false, float volume = 1, float fadeTime = 0, bool exclusive = false)
	{
		if (Instance == null)
		{
			Debug.Log("SOUNDMANAGER NOT YET DOWNLOAD");
			return null;
		}
		if (clip == null)
			return null;
		AudioSource s;
		if (!exclusive)
			s = Instance.FindFreeSource();
		else
			s = AllocAudioSource();
		s.clip = clip;
		if (fadeTime == 0)
			s.volume = volume;
		else
			s.volume = 0;
		s.loop = loop;
		s.playOnAwake = false;
		s.ignoreListenerVolume = true;
		s.Play();
		VolumeFaderExtern(s, fadeTime, volume);

		return s;
	}
	public static AudioSource AllocAudioSource()
    {
		return Instance.gameObject.AddComponent<AudioSource>();
	}
	AudioSource FindFreeSource()
    {
		foreach (var so in managingSources)
			if (!so.isPlaying)
				return so;

		var s = gameObject.AddComponent<AudioSource>();
		s.playOnAwake = false;
		managingSources.Add(s);
		return s;
    }
	AudioSource FindFreeImpactSource(bool isInitializing = false)
	{		
		if (!isInitializing)
		{
			foreach (var so in impactSources)
				if (!so.isPlaying)
					return so;
		}

		var s = new GameObject().AddComponent<AudioSource>();
		s.playOnAwake = false;
		s.dopplerLevel = 0;
		s.rolloffMode = AudioRolloffMode.Linear;
		s.spatialBlend = 1;
		s.spread = 30;
		impactSources.Add(s);
		s.gameObject.name = "IMPACT_SOUND_" + impactSources.Count;
		return s;
	}
	public static void PlayImpactSound(string asSoundFile, Vector3 position)
    {
		if (Time.timeSinceLevelLoad < 1)
			return;
		var source = Instance.FindFreeImpactSource();
		source.transform.position = position;
		var property = FakeDatabase.FindProperty(asSoundFile);
		source.volume = property.Volume / 2;
		source.minDistance = property.MinDistance;
		source.maxDistance = property.MaxDistance;
		source.clip = property.GetClip();
		source.Play();
	}
	/// <summary>
	/// Returns true if sound have loop behavior
	/// </summary>
	/// <param name="asSoundName"></param>
	/// <param name="asSoundFile"></param>
	/// <param name="asEntity"></param>
	/// <param name="afFadeTime"></param>
	/// <returns></returns>
	public static bool PlaySoundAtEntity(string asSoundName, string asSoundFile, string asEntity, float afFadeTime)
    {
		if (string.IsNullOrEmpty(asSoundFile))
        {
			Debug.Log("Cannot play empty .snt");
			return false;
        }
		var g = Finder.Find(asEntity);		
		if (g == null)
		{
			Debug.Log(asEntity + " do not found");
			return false;
		}
		AudioSource source;
		GameObject gNew;
		if (Instance.sounds.TryGetValue(asSoundName, out gNew))
		{
			source = gNew.GetComponent<AudioSource>();
			if (gNew != g)
			{
				gNew.transform.parent = g.transform;
				gNew.transform.localPosition = Vector3.zero;
			}
		}
		else
		{
			if (asEntity == asSoundName)
			{
				if ((source = g.GetComponent<AudioSource>()) == null)
					source = g.AddComponent<AudioSource>();
				Instance.sounds.Add(asSoundName, g);
			}
			else
            {
				gNew = new GameObject();
				source = gNew.AddComponent<AudioSource>();				
				gNew.transform.parent = g.transform;
				gNew.transform.localPosition = Vector3.zero;
				gNew.name = asSoundName;
				Instance.sounds.Add(asSoundName, gNew);
			}
		}		

		var sntView = FakeDatabase.FindProperty(asSoundFile);
		source.dopplerLevel = 0;
		source.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;
		source.volume = 0;
		source.minDistance = sntView.MinDistance;
		source.maxDistance = sntView.MaxDistance;
		source.rolloffMode = AudioRolloffMode.Linear;
		source.spatialBlend = sntView.Use3D ? 1 : 0;
		source.spread = 30;
		source.loop = (sntView.Loop && sntView.Interval == 0);

		bool b = source.loop;

		if (sntView.Loop && sntView.Interval != 0)
		{
			Instance.StartCoroutine(Instance.SmartLooping(source, sntView, sntView.Interval, sntView.Random));
			b = true;
		}
		else
		{
			source.clip = sntView.GetClip();
			source.Play();
		}
		VolumeFaderExtern(source, afFadeTime, sntView.Volume);
		return b;
	}
	public static void StopSound(string asSoundName, float afFadeTime)
    {
		var g = Finder.Find(asSoundName);
		if (g == null)
        {
			Debug.Log(asSoundName + " do not found");
			return;
        }
		var s = g.GetComponent<AudioSource>();
		if (s == null)
		{
			Debug.Log(asSoundName + " not contain audiosource");
			return;
		}
		VolumeFaderExtern(s, afFadeTime, 0);
	}
	public IEnumerator SmartLooping(AudioSource source, IRandomClip library, float interval, float randomTrashold)
    {
		if (randomTrashold == 0)
			randomTrashold = 1;
		float random;
		while (true)
        {
			source.clip = library.GetClip();
			source.Play();
			random = 0;
			while (random < randomTrashold)
			{
				yield return new WaitForSeconds(interval);
				random = Random.Range(0, 1);
			}
        }
	}
	public static void VolumeFaderExtern(AudioSource source, float time, float volume)
    {
		if (source == null)
			return;
		
		IEnumerator cor;
		if (time == 0)
		{
			if (volume == 0)
			{
				if (source.loop)
					source.Pause();
				else
					source.Stop();
			}
			else
				source.volume = volume;
		}
		if (Instance.volumeFaders.TryGetValue(source, out cor))
		{
			Instance.StopCoroutine(cor);
			Instance.volumeFaders.Remove(source);
		}
		if (time != 0)
		{
			cor = Instance.VolumeFader(source, time, volume);
			Instance.StartCoroutine(cor);
			Instance.volumeFaders.Add(source, cor);
		}
	}
	IEnumerator VolumeFader(AudioSource source, float time, float volume)
    {
		var curVolume = source.volume;
		float i = 0;
		yield return null;
		while (i < 1)
        {
			if (source == null)
			{
				volumeFaders.Remove(source);
				yield break;
			}
			source.volume = Mathf.Lerp(curVolume, volume, i);
			yield return null;
			i += Time.unscaledDeltaTime / time;
        }
		if (source != null)
		{
			if (volume == 0)
			{
				if (source.loop)
					source.Pause();
				else
					source.Stop();
				source.volume = curVolume;
			}
			else
				source.volume = volume;
		}
		volumeFaders.Remove(source);
    }
	public static void ManipulateAudioListener(float volume, float time)
    {
		if (Instance.listenerCoroutine != null)
			Instance.StopCoroutine(Instance.listenerCoroutine);
		Instance.listenerCoroutine = Instance.AudioListenerVolume(volume, time);
		Instance.StartCoroutine(Instance.listenerCoroutine);
    }
	IEnumerator AudioListenerVolume(float desireVolume, float time)
    {
		float curVolume = AudioListener.volume;
		float i = 0;
		while (i < 1)
        {
			AudioListener.volume = Mathf.Lerp(curVolume, desireVolume, i);
			yield return null;
			i += Time.unscaledDeltaTime / time;
        }
		AudioListener.volume = desireVolume;
		Instance.listenerCoroutine = null;
	}
}

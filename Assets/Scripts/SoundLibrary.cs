using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Sound Library", fileName = "NewSoundLibrary")]

public class SoundLibrary : ScriptableObject, IRandomClip
{
    [SerializeField] AudioClip[] clips;
    public float volume = 1;
    public AudioClip GetClip()
    {
        return clips[Random.Range(0, clips.Length)];
    }
    public float GetVolume()
    {
        return volume;
    }
}
public interface IRandomClip
{
    AudioClip GetClip();
    float GetVolume();
}
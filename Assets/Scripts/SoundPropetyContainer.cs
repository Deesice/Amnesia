using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class SoundPropetyContainer : MonoBehaviour
{
    public float volume;
    public float min;
    public float max;
    public bool useDefaults;
    public string sntView;

    AudioSource _source;
    [HideInInspector] public AudioSource Source { get { if (_source == null)
            {
                _source = GetComponent<AudioSource>();
                if (_source == null)
                    _source = gameObject.AddComponent<AudioSource>();
            }
            return _source;
        } }
    private void Start()
    {
        var l = GetComponentInParent<LampLitController>();
        if (l != null)
            gameObject.name = l.gameObject.name + "_fire_audio";
        SoundManager.PlaySoundAtEntity(gameObject.name, sntView, gameObject.name, 0);
        if (!useDefaults)
        {
            Source.minDistance = min;
            Source.maxDistance = max;
            Source.volume = volume;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Custom/Item", fileName = "NewItem")]
public class Item : ScriptableObject
{
    public enum EntitySubType { Lantern, Tinderbox, LampOil, Puzzle, Diary, Note, Health }
    public EntitySubType SubType;
    public string NameInLangFile;
    public Sprite ImageFile;
    public AudioClip PickSound;
    [HideInInspector] public string InternalName;
}

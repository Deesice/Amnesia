using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointSound : MonoBehaviour
{
    enum PreviousPlayedSound { MaxLimit, MinLimit, Move}
    PreviousPlayedSound previousPlayedSound;
    public MovableObject.MoveType type;
    public string moveSound;
    public string minLimitSound;
    public string maxLimitSound;
    public float minSoundSpeed = 0.1f;
    public float maxSoundSpeed = 2;
    public float maxVolume = 0.8f;
    public float minFreq = 0.95f;
    public float maxFreq = 1.1f;
    public float limitMaxSpeedInRadian = 1;
    AudioSource audioSource;
    Rigidbody rb;
    ILimit iLimit;
    SoundProperty moveSnt;
    SoundProperty minLimitSnt;
    SoundProperty maxLimitSnt;
    float previousAngularMagnitude;
    Vector3 velocity;
    Vector3 prevPositionForKinematicBodies;
    Quaternion prevRotationForKinematicBodies;
    void Start()
    {
        limitMaxSpeedInRadian = 1;
        prevPositionForKinematicBodies = transform.position;
        prevRotationForKinematicBodies = transform.rotation;
        iLimit = GetComponent<ILimit>();
        if (iLimit == null && GetComponent<HingeJoint>())
            iLimit = gameObject.AddComponent<JointLimitMonitor>();
        rb = GetComponent<Rigidbody>();
        if ((audioSource = GetComponent<AudioSource>()) == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.dopplerLevel = 0;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.spread = 30;
        moveSnt = FakeDatabase.FindProperty(moveSound);
        minLimitSnt = FakeDatabase.FindProperty(minLimitSound);
        maxLimitSnt = FakeDatabase.FindProperty(maxLimitSound);
        if (moveSnt)
        {
            audioSource.LoadFromSoundProperty(moveSnt);
            maxVolume *= moveSnt.Volume;
            audioSource.clip = moveSnt.GetClip();
        }
        audioSource.loop = false;
        audioSource.volume = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (iLimit != null)
        {
            if (/*joint.angle <= joint.limits.min + minSoundSpeed*/ iLimit.NearMin(minSoundSpeed) && previousPlayedSound != PreviousPlayedSound.MaxLimit && maxLimitSnt)
            {
                audioSource.Stop();
                audioSource.LoadFromSoundProperty(maxLimitSnt);
                audioSource.PlayOneShot(maxLimitSnt.GetClip(), Mathf.Clamp(previousAngularMagnitude / limitMaxSpeedInRadian, 0, 1) * maxVolume);
                previousPlayedSound = PreviousPlayedSound.MaxLimit;
                velocity = Vector3.zero;
                return;
            }
            if (/*joint.angle >= joint.limits.max - minSoundSpeed*/ iLimit.NearMax(minSoundSpeed) && previousPlayedSound != PreviousPlayedSound.MinLimit && minLimitSnt)
            {
                audioSource.Stop();
                audioSource.LoadFromSoundProperty(minLimitSnt);
                audioSource.PlayOneShot(minLimitSnt.GetClip(), Mathf.Clamp(previousAngularMagnitude / limitMaxSpeedInRadian, 0, 1) * maxVolume);
                previousPlayedSound = PreviousPlayedSound.MinLimit;
                velocity = Vector3.zero;
                return;
            }
            previousAngularMagnitude = velocity.magnitude;
        }
        if (moveSnt == null)
            return;
        
        if (velocity.magnitude > minSoundSpeed)
        {
            if (previousPlayedSound != PreviousPlayedSound.Move)
            {
                audioSource.minDistance = moveSnt.MinDistance;
                audioSource.maxDistance = moveSnt.MaxDistance;
                audioSource.loop = false;
            }
            previousPlayedSound = PreviousPlayedSound.Move;
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        if (previousPlayedSound == PreviousPlayedSound.Move)
        {
            var coef = (velocity.magnitude - minSoundSpeed) / (maxSoundSpeed - minSoundSpeed);
            audioSource.pitch = Mathf.Lerp(minFreq, maxFreq, coef);
            audioSource.volume = Mathf.Lerp(audioSource.volume, Mathf.Lerp(0, maxVolume, coef), Time.deltaTime * 10);
        }
    }
    private void FixedUpdate()
    {
        if (rb && !rb.isKinematic)
        {
            if (type == MovableObject.MoveType.Linear)
                velocity = rb.velocity;
            else
                velocity = rb.angularVelocity;
        }
        else
        {
            if (type == MovableObject.MoveType.Linear)
            {
                velocity = (transform.position - prevPositionForKinematicBodies) / Time.fixedDeltaTime;
                prevPositionForKinematicBodies = transform.position;
            }
            else
            {
                velocity = Vector3.up * Quaternion.Angle(transform.rotation, prevRotationForKinematicBodies) * Mathf.Deg2Rad / Time.fixedDeltaTime;
                prevRotationForKinematicBodies = transform.rotation;
            }
        }
    }
}

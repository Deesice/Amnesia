using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnim : MonoBehaviour
{
    public class OffsetProperty
    {
        public Vector3 offset;
        public float speed;
        public float slowDownDist;
    }
    OffsetProperty offsetProperty = new OffsetProperty();
    public enum MovingState { Idle, Walk, Run}
    public float speed;
    public float reaction;

    public float walkRadius;
    public float runRadius;
    public float walkRotationRadius;
    public float runRotationRadius;
    public Transform handObject;


    public MovingState currentState { get; private set; }
    Vector3 startPos;
    Vector3 offset;

    float inputMagnitude;
    float cumulativeAngle;
    int lastPhaseAngle;
    float isRun;
    float isNotCrouch;
    public bool IsRun { get { return isRun >= 0.5f; } }
    public event Action OnStepped;
    [HideInInspector] public Quaternion rotationOffset;
    Camera mainCamera;
    public static CameraAnim instance;
    float standartFOV = 70;
    float standartAspect;
    Vector3 constantOffset;
    Vector3 shakeOffset;

    IEnumerator fadingFOV = null;
    IEnumerator fadingAspect = null;
    bool _setCrouch;
    public bool SetCrouch { get { return _setCrouch; } set { _setCrouch = value; if (value && currentState == MovingState.Run) currentState = MovingState.Walk; } }
    void Awake()
    {
        instance = this;
        mainCamera = GetComponent<Camera>();
        standartAspect = mainCamera.aspect;
        lastPhaseAngle = 1;
        startPos = transform.localPosition;
    }
    public void SetState(MovingState state)
    {
        if (currentState == state)
            return;
        currentState = state;
        if (state == MovingState.Run)
            SetCrouch = false;
    }

    // Update is called once per frame
    void Update()
    {
        inputMagnitude = Mathf.Lerp(inputMagnitude, PlayerController.instance.input.magnitude, Time.deltaTime * reaction);

        isNotCrouch = Mathf.Lerp(isNotCrouch, SetCrouch ? 0 : 1, Time.deltaTime * reaction * 2);

        switch (currentState)
        {
            case MovingState.Idle:
            case MovingState.Walk:
                isRun = Mathf.Lerp(isRun, 0, Time.deltaTime * reaction);
                break;
            case MovingState.Run:
                isRun = Mathf.Lerp(isRun, 1, Time.deltaTime * reaction);             
                break;
            default:
                break;
        }
        cumulativeAngle += Time.deltaTime * speed * PlayerController.instance.totalWalkMultiplier
            * Mathf.Lerp(1, Mathf.LerpUnclamped(1, PlayerController.runMultiplier, PlayerController.instance.totalRunMultiplier), isRun)
            * Mathf.Lerp(0.65f, 1, isNotCrouch);
        if ((cumulativeAngle - lastPhaseAngle * Mathf.PI) >= 0)
        {
            lastPhaseAngle++;
            if (currentState != MovingState.Idle)
                OnStepped?.Invoke();
        }
        offset = -Vector3.up * Mathf.Abs(Mathf.Sin(cumulativeAngle)) + Mathf.Cos(cumulativeAngle) * transform.right;
        offset *= Mathf.Lerp(walkRadius, runRadius, isRun);
        rotationOffset = Quaternion.Euler(Mathf.Abs(Mathf.Sin(cumulativeAngle)) * Mathf.Lerp(walkRotationRadius, runRotationRadius, isRun),
            Mathf.Cos(cumulativeAngle) * Mathf.Lerp(walkRotationRadius, runRotationRadius, isRun),
            0);

        transform.localPosition = Vector3.Lerp(startPos, startPos + offset, inputMagnitude);
        var globalOffsetDelta = offsetProperty.offset - constantOffset;
        if (globalOffsetDelta.magnitude > offsetProperty.slowDownDist)
            constantOffset += globalOffsetDelta.normalized * Time.deltaTime * offsetProperty.speed;
        else
            constantOffset = Vector3.Lerp(constantOffset, offsetProperty.offset, Time.deltaTime * offsetProperty.speed);
        transform.localPosition += constantOffset;
        handObject.localPosition = startPos + constantOffset;

        var coef = Mathf.Lerp(0.2f, 1, isNotCrouch);

        handObject.localPosition *= coef;
        transform.localPosition *= coef;
        if (Time.timeScale != 0)
        {
            handObject.localPosition += shakeOffset * 0.85f;
            transform.localPosition += shakeOffset;
        }
    }
    public static void SetOffsetProperty(Vector3 offset, float speed, float slowDownDist)
    {
        instance.offsetProperty.offset = offset;
        instance.offsetProperty.speed = speed;
        instance.offsetProperty.slowDownDist = slowDownDist;
    }
    public static void FadePlayerFOVMulTo(float afX, float afSpeed)
    {
        if (instance.fadingFOV != null)
            instance.StopCoroutine(instance.fadingFOV);
        instance.fadingFOV = instance.FOVFading(afX * instance.standartFOV, afSpeed);
        instance.StartCoroutine(instance.fadingFOV);
    }
    public static void FadePlayerAspectMulTo(float afX, float afSpeed)
    {
        if (instance.fadingAspect != null)
            instance.StopCoroutine(instance.fadingAspect);
        instance.fadingAspect = instance.AspectFading(afX * instance.standartAspect, afSpeed);
        instance.StartCoroutine(instance.fadingAspect);
    }
    public static void StartScreenShake(float afAmount, float afTime, float afFadeInTime, float afFadeOutTime)
    {
        instance.StartCoroutine(instance.ScreenShake(afAmount, afTime, afFadeInTime, afFadeOutTime));
    }
    IEnumerator ScreenShake(float afAmount, float afTime, float afFadeInTime, float afFadeOutTime)
    {
        float i = 0;
        while (i < 1)
        {
            GenerateRandomShake(afAmount);
            shakeOffset *= i;
            yield return null;
            i += Time.deltaTime / afFadeInTime;
        }
        i = 0;
        while (i < 1)
        {
            GenerateRandomShake(afAmount);
            yield return null;
            i += Time.deltaTime / afTime;
        }
        while (i > 0)
        {
            GenerateRandomShake(afAmount);
            shakeOffset *= i;
            yield return null;
            i -= Time.deltaTime / afFadeOutTime;
        }
        shakeOffset = Vector3.zero;
    }
    void GenerateRandomShake(float afAmount)
    {
        shakeOffset.y = UnityEngine.Random.Range(-afAmount, afAmount);
        shakeOffset.x = UnityEngine.Random.Range(-afAmount, afAmount);
        shakeOffset.z = UnityEngine.Random.Range(-afAmount, afAmount);
    }
    IEnumerator FOVFading(float value, float time)
    {
        while (mainCamera.fieldOfView != value)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, value, Time.deltaTime * time);
            yield return null;
        }
        fadingFOV = null;
    }
    IEnumerator AspectFading(float value, float time)
    {
        while (mainCamera.aspect != value)
        {
            mainCamera.aspect = Mathf.Lerp(mainCamera.aspect, value, Time.deltaTime * time);
            yield return null;
        }
        fadingAspect = null;
    }
}

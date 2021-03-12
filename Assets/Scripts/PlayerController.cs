using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour, ISave
{
    public class RotateProperty
    {
        public float angle = 0;
        public float speed = 1000000;
        public float maxSpeed = 10000000;
        public Quaternion quaternion = Quaternion.identity;
    }
    public class LookAtProperty
    {
        public Transform target;
        public float speed;
        public float maxSpeed;
        public Quaternion quaternion = Quaternion.identity;
    }
    public class MultipliersProperty
    {
        public float moveSpeedMul;
        public float runSpeedMul;
        public float lookSpeedMul;
        public MultipliersProperty()
        {
            moveSpeedMul = 1;
            runSpeedMul = 1;
            lookSpeedMul = 1;
        }
    }
    public bool mobileControl;
    public Rigidbody hand;
    public float sensitivity = 100f;
    public Rigidbody playerBody;
    public float speed = 10;
    Joystick joystick;
    RectTransform handle;
    public float smoothViewLerpCoefficient = 10;
    public float VerticalAngleLimit;
    Vector2 handlePosition;
    [HideInInspector] public Vector2 input;
    public Quaternion rotationCumulative;
    Vector3 prevMousePos;
    float stationaryCounter = 0;
    float lastDownTime = 0;
    bool isPreviousFrameWithTouches;
    public Interactable grabObject = null;
    bool isStillStationary;
    public GameObject scannedObject;
    public event Action<GameObject> OnScanChanged;
    [HideInInspector] public bool FreezeMovement;
    [HideInInspector] public bool FreezeRotation;
    [HideInInspector] public bool BlockLantern;
    [HideInInspector] public bool BlockCrouch;
    public static PlayerController instance;
    [HideInInspector] public Fader fader;

    CameraAnim cameraAnim;

    Vector3 forward;
    Vector3 right;
    public const float runMultiplier = 1.7f;
    public RotateProperty rotateProperty = new RotateProperty();
    public LookAtProperty lookAtProperty = new LookAtProperty();
    public MultipliersProperty baseMultiplierProperty;
    public MultipliersProperty pushMultipliers;
    List<MultipliersProperty> allMultipliers = new List<MultipliersProperty>();

    public float totalRunMultiplier;
    public float totalWalkMultiplier;
    public float totalLookMultiplier;
    private void FixedUpdate()
    {
        TranslateBody();
    }
    private void Awake()
    {
#if UNITY_EDITOR
#else
        mobileControl = true;
#endif
        baseMultiplierProperty = new MultipliersProperty();
        pushMultipliers = new MultipliersProperty();
        allMultipliers.Add(baseMultiplierProperty);
        allMultipliers.Add(pushMultipliers);
        fader = Finder.Find("fader").GetComponent<Fader>();
        cameraAnim = GetComponent<CameraAnim>();
        instance = this;
        rotationCumulative = transform.rotation;
        //fader.FadeOn(3);
        joystick = FindObjectOfType<Joystick>();
        handle = joystick.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        Input.gyro.enabled = true;
    }
    private void OnEnable()
    {
        prevMousePos = Input.mousePosition;
        isPreviousFrameWithTouches = true;
    }
    public void DenyHandByForce()
    {
        if (grabObject != null)
        {
            grabObject.DenyHand();
        }
        isPreviousFrameWithTouches = true;
        grabObject = null;
        scannedObject = null;
        OnScanChanged?.Invoke(scannedObject);
        Crosshair.ApplyTagOfFocused("");
    }
    void TranslateBody()
    {
        forward = transform.forward;
        right = transform.right;
        forward.y = 0;
        forward.Normalize();
        right.y = 0;
        right.Normalize();
        playerBody.velocity = (forward * input.y + right * input.x)
            * (cameraAnim.currentState == CameraAnim.MovingState.Run ?
            speed * Mathf.LerpUnclamped(1, runMultiplier, totalRunMultiplier) * totalWalkMultiplier :
            speed * totalWalkMultiplier) * (cameraAnim.SetCrouch ? 0.65f : 1) + Vector3.up * playerBody.velocity.y;
    }
    void CalculateMultipliers()
    {
        totalRunMultiplier = 1;
        foreach (var i in allMultipliers)
            totalRunMultiplier *= i.runSpeedMul;
        totalWalkMultiplier = 1;
        foreach (var i in allMultipliers)
            totalWalkMultiplier *= i.moveSpeedMul;
        totalLookMultiplier = 1;
        foreach (var i in allMultipliers)
            totalLookMultiplier *= i.lookSpeedMul;
    }
    void DoubleClick()
    {
        if (Crosshair.instance.puzzleMode)
        {
            Crosshair.EnablePuzzleMode(null);
            Inventory.TryUsePuzzleItem();
        }
        else
            Grab();
    }
    void StationaryClick()
    {

    }
    void Update()
    {
        if (grabObject && (grabObject.transform.position - transform.position).magnitude > 4)
            DenyHandByForce();

        CalculateMultipliers();

        if (mobileControl)
        {
            input.y = joystick.Vertical;
            input.x = joystick.Horizontal;

            if (input.magnitude < 0.3f)
                input = Vector2.zero;
        }
        else
        {
            input.y = Input.GetAxis("Vertical");
            input.x = Input.GetAxis("Horizontal");
        }

        input = Vector2.ClampMagnitude(input, 1);

        var reciviedInput = input;
        if (FreezeMovement)
            input = Vector2.zero;        

        if (input.magnitude == 0)
            cameraAnim.SetState(CameraAnim.MovingState.Idle);
        else if (Input.GetKeyDown(KeyCode.LeftShift))
                cameraAnim.SetState(CameraAnim.MovingState.Run);
        else if (!Input.GetKey(KeyCode.LeftShift))
            cameraAnim.SetState(CameraAnim.MovingState.Walk);

        //Debug.Log("Gyro magnitude: " + Input.gyro.rotationRate.magnitude);

        if ((Input.GetKeyDown(KeyCode.F) || Input.gyro.rotationRate.magnitude > 5) && !BlockLantern)
            Lantern.Switch();
        if (Input.GetKeyDown(KeyCode.C) && !BlockCrouch)
            cameraAnim.SetCrouch = !cameraAnim.SetCrouch;
        Scan();
        var touches = ActualTouches();

        if (touches.Count() == 0)
        {
            isPreviousFrameWithTouches = false;
            stationaryCounter = 0;
            isStillStationary = true;
            if (grabObject != null)
            {
                if (!grabObject.disableDenying)
                {
                    grabObject.DenyHand();
                    grabObject = null;
                }
                else if (grabObject.blockView)
                    grabObject.RecieveFingerDelta(Vector2.zero, reciviedInput);
            }
        }
        foreach (var touch in touches)
        {
            if (mobileControl)
            {
                if (touch.tapCount > 1 && !isPreviousFrameWithTouches)
                {
                    DoubleClick();
                    isPreviousFrameWithTouches = true;
                }
            }
            else
            {
                if (!isPreviousFrameWithTouches)
                    if (Time.time - lastDownTime < 0.25f)
                        DoubleClick();
                    else
                        lastDownTime = Time.time;
                isPreviousFrameWithTouches = true;
            }

            if (isStillStationary)
            {
                if (touch.deltaPosition.magnitude < 10)
                    stationaryCounter += Time.deltaTime;
                else
                    isStillStationary = false;

                if (stationaryCounter > 0.25f)
                {
                    StationaryClick();
                    isStillStationary = false;
                }
            }

            var mouseInput = touch.deltaPosition * sensitivity * 1080 / Screen.currentResolution.height;
            mouseInput.x *= -1;
            if (grabObject != null && grabObject.blockView)
                grabObject.RecieveFingerDelta(mouseInput, reciviedInput);
            else if (!FreezeRotation)
            {
                rotationCumulative *= Quaternion.Euler(-mouseInput.y * totalLookMultiplier, -mouseInput.x * totalLookMultiplier, 0);
                var vector = rotationCumulative.eulerAngles;
                if (vector.x > 180)
                    vector.x -= 360;
                if (vector.x > VerticalAngleLimit)
                    vector.x = VerticalAngleLimit;
                else if (vector.x < -VerticalAngleLimit)
                    vector.x = -VerticalAngleLimit;
                rotationCumulative = Quaternion.Euler(vector.x, vector.y, 0);
            }
            break;
        }
        Rotate();
    }
    void Rotate()
    {
        float coef;
        Quaternion newQuaternion;
        newQuaternion = Quaternion.Lerp(rotateProperty.quaternion, Quaternion.Euler(0, 0, rotateProperty.angle), Time.deltaTime * rotateProperty.speed);
        coef = rotateProperty.maxSpeed / (Quaternion.Angle(newQuaternion, rotateProperty.quaternion) / Time.deltaTime);
        rotateProperty.quaternion = Quaternion.Lerp(rotateProperty.quaternion, newQuaternion, coef);

        if (lookAtProperty.target != null)
        {
            var newRotationCumulative = Quaternion.LookRotation(lookAtProperty.target.position - transform.position, Vector3.up);
            rotationCumulative = Quaternion.Lerp(rotationCumulative, newRotationCumulative, Time.deltaTime * lookAtProperty.speed);
        }

        var rotation = Quaternion.Lerp(transform.rotation, rotationCumulative
            * Quaternion.Lerp(Quaternion.identity, cameraAnim.rotationOffset, input.magnitude)
            * rotateProperty.quaternion
            ,Time.deltaTime * smoothViewLerpCoefficient);
        transform.rotation = rotation;
    }
    void Scan()
    {
        RaycastHit hit;
        if (grabObject == null && Physics.Raycast(transform.position, transform.forward, out hit, hand.transform.localPosition.magnitude))
        {
            if (scannedObject != hit.collider.gameObject)
            {
                scannedObject = hit.collider.gameObject;
                OnScanChanged?.Invoke(scannedObject);
                Crosshair.ApplyTagOfFocused(scannedObject.tag);
            }
        }
        else
        {
            if (scannedObject != null)
            {
                scannedObject = null;
                OnScanChanged?.Invoke(scannedObject);
                Crosshair.ApplyTagOfFocused(string.Empty);
            }
        }
    }
    void Grab()
    {
        if (scannedObject == null)
            return;
        grabObject = scannedObject.GetComponent<Interactable>();
        if (grabObject == null && scannedObject.transform.parent != null)
            grabObject = scannedObject.GetComponentInParent<Interactable>();
        if (grabObject != null && (grabObject.enabled || grabObject is Door))
            grabObject.ApplyHand(hand);
        else
            grabObject = null;
    }

    IEnumerable<Touch> ActualTouches()
    {
        if (mobileControl)
        {
            handlePosition.x = handle.position.x;
            handlePosition.y = handle.position.y;
            var query = from i in Input.touches
                        orderby (i.position - handlePosition).magnitude descending
                        select i;
            return query.Take(query.Count() - ((joystick.Vertical == 0 && joystick.Horizontal == 0) ? 0 : 1));
        }
        else
        {
            if (!Input.GetMouseButton(0))
                return new List<Touch>();
            var list = new List<Touch>();
            if (Input.GetMouseButtonDown(0))
            {
                prevMousePos = Input.mousePosition;
                var t = new Touch();
                t.deltaPosition = Input.mousePosition - prevMousePos;
                list.Add(t);
            }
            else
            {
                var t = new Touch();
                t.deltaPosition = Input.mousePosition - prevMousePos;
                list.Add(t);
                prevMousePos = Input.mousePosition;
            }
            return list;
        }
    }
    void ISave.OnLoad(Data data)
    {
        if (data.VectorKeys.TryGetValue(this.GetHierarchyPath() + "/position", out var pos, Vector3.zero))
            playerBody.position = pos;

        if (data.VectorKeys.TryGetValue(this.GetHierarchyPath() + "/rotation", out var rot, Vector3.zero))
        {
            transform.rotation = Quaternion.Euler(rot);
            rotationCumulative = Quaternion.Euler(rot);
        }
    }

    void ISave.OnSave(Data data)
    {
        data.VectorKeys.SetValueSafety(this.GetHierarchyPath() + "/position", playerBody.position);
        data.VectorKeys.SetValueSafety(this.GetHierarchyPath() + "/rotation", transform.rotation.eulerAngles);
    }
}

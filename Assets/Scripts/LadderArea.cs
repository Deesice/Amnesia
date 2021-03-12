using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderArea : Interactable
{
    Vector3 pos;
    GameObject lookTarget;
    float speed = 0.85f;
    float toNextStep = 0.5f;
    public SoundLibrary steps;
    new Collider collider;

    Vector3 lowPos;
    Vector3 highPos;
    Vector3 finalPos;
    Vector3 verticalOffset;

    bool isClimb;
    public override void DenyHand()
    {
        hand = null;
        Destroy(lookTarget);
        PlayerController.instance.FreezeMovement = false;
        PlayerController.instance.playerBody.isKinematic = false;
    }

    IEnumerator coroutine = null;

    public override void RecieveFingerDelta(Vector2 mouseInput, Vector2 joystickInput)
    {
        if (isClimb)
            joystickInput = Vector2.up;

        lookTarget.transform.RotateAround(PlayerController.instance.transform.position,
            PlayerController.instance.playerBody.transform.up, -mouseInput.x);
        lookTarget.transform.RotateAround(PlayerController.instance.transform.position,
            PlayerController.instance.transform.right, -mouseInput.y);

        if (coroutine == null && joystickInput.y != 0)
        {            
            var v = Vector3.up * joystickInput.y * Time.deltaTime * speed;
            toNextStep -= v.magnitude;
            pos += v;
            if (toNextStep <= 0)
            {
                toNextStep += 0.5f;
                SoundManager.PlaySound(steps);
            }
            if (pos.y > highPos.y)
            {
                OnUpperPoint();
                return;
            }
            if (pos.y < lowPos.y)
            {
                OnLowerPoint();
                return;
            }
            PlayerController.instance.playerBody.transform.position = pos + verticalOffset;
        }
    }
    void OnLowerPoint()
    {
        PlayerController.instance.DenyHandByForce();
    }

    void OnUpperPoint()
    {
        disableDenying = true;
        PlayerController.instance.playerBody.transform.position = highPos + verticalOffset;
        coroutine = MovingPlayer(finalPos, speed);
        StartCoroutine(coroutine);
        SmartInvoke.WhenTrue(() => coroutine == null, () => PlayerController.instance.DenyHandByForce());
    }
    IEnumerator MovingPlayer(Vector3 newPosition, float speed)
    {
        float i = 0;
        newPosition += verticalOffset;
        Vector3 oldPosition = PlayerController.instance.playerBody.position;
        float time = (newPosition - oldPosition).magnitude / speed;
        while (i < 1)
        {
            PlayerController.instance.playerBody.position = Vector3.Lerp(oldPosition, newPosition, i);
            yield return null;
            i += Time.deltaTime / time;
        }
        PlayerController.instance.playerBody.position = newPosition;
        disableDenying = isClimb;
        coroutine = null;
    }

    protected override void ApplyHandContent(Rigidbody hand)
    {
        disableDenying = true;
        this.hand = hand;
        
        lookTarget = new GameObject("ladderlooktarget");

        lookTarget.transform.position = PlayerController.instance.transform.position - transform.forward;
        lookTarget.transform.SetParent(PlayerController.instance.playerBody.transform);

        PlayerController.instance.lookAtProperty.target = lookTarget.transform;
        PlayerController.instance.lookAtProperty.speed = 5;
        PlayerController.instance.lookAtProperty.maxSpeed = 10;
        PlayerController.instance.FreezeMovement = true;

        var y = PlayerController.instance.playerBody.transform.position.y;
        y = Mathf.Clamp(y, lowPos.y, highPos.y);

        pos = lowPos;
        pos.y = y;

        PlayerController.instance.playerBody.isKinematic = true;

        coroutine = MovingPlayer(pos, speed * 5);
        StartCoroutine(coroutine);
    }

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
        gameObject.tag = "Ladder";
        gameObject.layer = 0;
        blockView = true;

        CalculateBounds();

        isClimb = (lowPos - highPos).magnitude <= (finalPos - highPos).magnitude;        
    }

    private void OnDrawGizmos()
    {
        CalculateBounds();

        Gizmos.DrawCube(lowPos, Vector3.one * 0.1f);
        Gizmos.DrawCube(highPos, Vector3.one * 0.1f);
        Gizmos.DrawCube(finalPos, Vector3.one * 0.1f);
    }
    void CalculateBounds()
    {
        if (collider == null)
            collider = GetComponent<Collider>();

        var upPoint = collider.bounds.ClosestPoint(transform.position + transform.up * 100);
        var frontPoint = collider.bounds.ClosestPoint(transform.position + transform.forward * 100);
        lowPos = -upPoint + transform.position + frontPoint;
        highPos = upPoint - transform.position + frontPoint;
        finalPos = upPoint + transform.position - frontPoint;

        Vector3 delta = Vector3.zero;

        if (PlayerController.instance != null)
        {
            var playerCollider = PlayerController.instance.playerBody.GetComponent<CapsuleCollider>();
            delta = transform.forward * playerCollider.radius;
            verticalOffset = Vector3.up * playerCollider.height;
            verticalOffset /= 2;
        }

        lowPos += delta;
        highPos += delta;
        finalPos -= delta;

    }
}

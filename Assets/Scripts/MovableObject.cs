using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : ReflectableMonoBehaviour, IState
{
    public enum MoveType { Linear, Angular}
    public enum MoveAxis { X, Y, Z}
    public MoveType movingType;
    public MoveAxis moveAxis;
    public float openAmount = 1;
    public float speed;
    Transform angularOffsetArea;
    public string angularOffsetAreaName;
    public event Action<float> OnStateChanged;
    public List<Transform> attachedObjects = new List<Transform>();

    Vector3 ActualMoveAxis
    {
        get
        {
            switch (moveAxis)
            {
                case MovableObject.MoveAxis.X:
                    return -Vector3.right;
                case MovableObject.MoveAxis.Y:
                    return Vector3.up;
                case MovableObject.MoveAxis.Z:
                    return Vector3.forward;
            }
            return Vector3.zero;
        }
    }

    float state;
    public void SetState(float state)
    {
        if (this.state == state)
            return;
        switch(movingType)
        {
            case MoveType.Angular:
                StartCoroutine(RotateOn((state - this.state) * openAmount * 90));
                break;
            case MoveType.Linear:
                StartCoroutine(MoveTo(transform.position + ActualMoveAxis * (state - this.state) * openAmount * 3));
                break;
        }
        this.state = state;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!attachedObjects.Contains(collision.transform))
            attachedObjects.Add(collision.transform);
    }
    private void OnCollisionExit(Collision collision)
    {
        if (attachedObjects.Contains(collision.transform))
            attachedObjects.Remove(collision.transform);
    }

    public float GetState()
    {
        return state;
    }
    private void Start()
    {
        if (string.IsNullOrEmpty(angularOffsetAreaName))
            angularOffsetArea = transform;
        else
            angularOffsetArea = Finder.Find(angularOffsetAreaName).transform;
    }

    IEnumerator RotateOn(float degrees)
    {
        float totalTime = degrees / speed / Mathf.Rad2Deg;
        float i = 0;
        while (i < 1)
        {
            foreach (var j in attachedObjects)
                j.RotateAround(angularOffsetArea.position, ActualMoveAxis, -speed * Time.deltaTime * Mathf.Rad2Deg);
            transform.RotateAround(angularOffsetArea.position, ActualMoveAxis, -speed * Time.deltaTime * Mathf.Rad2Deg);

            yield return null;
            i += Time.deltaTime / totalTime;
        }
    }
    IEnumerator MoveTo(Vector3 newPos)
    {        
        Vector3 startPos = transform.position;
        float totalTime = (newPos - startPos).magnitude / speed;
        float i = 0;
        while (i < 1)
        {
            foreach (var j in attachedObjects)
                j.position += (newPos - startPos) * Time.deltaTime / totalTime;
            transform.position = Vector3.Lerp(startPos, newPos, i);
            yield return null;
            i += Time.deltaTime / totalTime;
        }
        transform.position = newPos;
    }

    public void SetStuckState(float state)
    {
        throw new NotImplementedException();
    }

    void ISave.OnLoad(Data data)
    {
        if (data.VectorKeys.TryGetValue(this.GetHierarchyPath() + "/position", out var pos, Vector3.zero))
            transform.position = pos;

        if (data.VectorKeys.TryGetValue(this.GetHierarchyPath() + "/rotation", out var rot, Vector3.zero))
            transform.rotation = Quaternion.Euler(rot);

        data.FloatKeys.TryGetValue(this.GetHierarchyPath(), out state, state);
    }

    void ISave.OnSave(Data data)
    {
        data.VectorKeys.SetValueSafety(this.GetHierarchyPath() + "/position", transform.position);
        data.VectorKeys.SetValueSafety(this.GetHierarchyPath() + "/rotation", transform.rotation.eulerAngles);
        data.FloatKeys.SetValueSafety(this.GetHierarchyPath(), state);
    }
}

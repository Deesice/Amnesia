using System;

public interface IState: ISave
{
    public event Action<float> OnStateChanged;
    public void SetState(float state);
    public float GetState();
    public void SetStuckState(float state);
}

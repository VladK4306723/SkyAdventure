using UnityEngine;
using Zenject;

public interface IDangerModel
{
    float Value { get; }
    void Init(float increaseSpeed, float decreaseSpeed);
    void Update(float inputAbs, float dt);
    void Reset();
    bool IsMax { get; }
}

public class DangerModel : IDangerModel
{
    [Inject] private IGameProgressService _progress;

    public float Value { get; private set; }

    private float _increaseSpeed;
    private float _decreaseSpeed;

    public void Init(float increaseSpeed, float decreaseSpeed)
    {
        _increaseSpeed = increaseSpeed;
        _decreaseSpeed = decreaseSpeed;
    }

    public void Update(float inputAbs, float dt)
    {
        if (inputAbs > 0.1f)
        {
            Value += inputAbs * _increaseSpeed * dt;
        }
        else
        {
            Value -= _decreaseSpeed * dt;
        }

        Value = Mathf.Clamp01(Value);
        _progress.CurrentSession.UpdateDanger(Value);
    }

    public void Reset()
    {
        Value = 0f;
    }

    public bool IsMax => Value >= 1f;
}

using UnityEngine;
using Zenject;

public interface IMultiplierModel
{
    float Value { get; }
    float MaxValue { get; }

    void Init(
        float increaseSpeed,
        float decreaseSpeed,
        float maxMultiplier
    );

    void Update(float inputAbs, float danger01, float dt);
    void Reset();
}

public sealed class MultiplierModel : IMultiplierModel
{
    [Inject] IGameProgressService _progress;

    public float Value { get; private set; } = 1f;
    public float MaxValue { get; private set; } = 1f;

    private float _increaseSpeed;
    private float _decreaseSpeed;
    private float _maxMultiplier;

    public void Init(
        float increaseSpeed,
        float decreaseSpeed,
        float maxMultiplier)
    {
        _increaseSpeed = increaseSpeed;
        _decreaseSpeed = decreaseSpeed;
        _maxMultiplier = maxMultiplier;
    }

    public void Update(float inputAbs, float danger01, float dt)
    {
        if (inputAbs > 0.1f)
        {
            float dangerFactor = Mathf.Lerp(1f, 2.5f, danger01);
            Value += _increaseSpeed * dangerFactor * dt;
        }
        else
        {
            Value -= _decreaseSpeed * dt;
        }

        Value = Mathf.Clamp(Value, 1f, _maxMultiplier);

        _progress.CurrentSession.UpdateMultiplier(Value);

        if (Value > MaxValue)
            MaxValue = Value;
    }

    public void Reset()
    {
        Value = 1f;
        MaxValue = 1f;
    }
}


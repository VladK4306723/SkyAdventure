using System;
using UnityEngine;

public sealed class PlayerController : IGameTick
{
    private readonly PlayerModel _model;
    private readonly PlayerView _view;
    private readonly IDangerModel _danger;

    private float _currentTilt;
    private float _currentX;

    private const float BaseHorizontalSpeed = 1f;
    private const float MaxTiltFactor = 45f;

    private readonly float _minX;
    private readonly float _maxX;

    public PlayerView View => _view;

    public event Action DangerMaxed;

    public PlayerController(
        PlayerModel model,
        PlayerView view,
        CameraBounds bounds,
        IDangerModel danger)
    {
        _model = model;
        _view = view;
        _danger = danger;

        _currentX = view.transform.position.x;
        _minX = bounds.Left;
        _maxX = bounds.Right;

        _danger.Init(
            increaseSpeed: 0.2f,
            decreaseSpeed: 0.1f
        );
    }

    public void Tick(float dt)
    {
        float input = _view.HorizontalInput;

        UpdateDanger(input, dt);

        UpdateTilt(input, dt);
        UpdateHorizontalMovement(dt);
        ApplyTransform();
    }

    private void UpdateDanger(float input, float dt)
    {
        _danger.Update(Mathf.Abs(input), dt);

        if (_danger.IsMax)
        {
            DangerMaxed?.Invoke();
        }

        Debug.Log($"Danger Level: {_danger.Value:F2}");
    }

    private void UpdateTilt(float input, float dt)
    {
        float maxTilt = MaxTiltFactor * _model.SpeedMultiplier;
        float targetTilt = -input * maxTilt;

        _currentTilt = Mathf.MoveTowards(
            _currentTilt,
            targetTilt,
            _model.TurnSpeed * dt
        );
    }

    private void UpdateHorizontalMovement(float dt)
    {
        float maxTilt = MaxTiltFactor * _model.SpeedMultiplier;

        float tilt01 = _currentTilt / maxTilt * -1;

        float horizontalSpeed = BaseHorizontalSpeed * _model.SpeedMultiplier;

        _currentX += tilt01 * horizontalSpeed * dt;
    }

    public void Cleanup()
    {
        DangerMaxed = null;

        _danger.Reset();

        _currentTilt = 0f;
        _currentX = _view.transform.position.x;
    }


    private void ApplyTransform()
    {
        _view.SetRotation(_currentTilt);

        Vector3 pos = _view.transform.position;
        _currentX = Mathf.Clamp(_currentX, _minX, _maxX);
        pos.x = _currentX;
        _view.transform.position = pos;
    }
}

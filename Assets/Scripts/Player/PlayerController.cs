using UnityEngine;

public sealed class PlayerController : IGameTick
{
    private readonly PlayerModel _model;
    private readonly PlayerView _view;

    private float _currentTilt;
    private float _currentX;

    private const float BaseHorizontalSpeed = 1f;
    private const float MaxTiltFactor = 45f;

    public PlayerController(PlayerModel model, PlayerView view)
    {
        _model = model;
        _view = view;

        _currentX = view.transform.position.x;
    }

    public void Tick(float dt)
    {
        float input = _view.HorizontalInput;

        UpdateTilt(input, dt);
        UpdateHorizontalMovement(dt);
        ApplyTransform();
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


    private void ApplyTransform()
    {
        _view.SetRotation(_currentTilt);

        Vector3 pos = _view.transform.position;
        pos.x = _currentX;
        _view.transform.position = pos;
    }
}

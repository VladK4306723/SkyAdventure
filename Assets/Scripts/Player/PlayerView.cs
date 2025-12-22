using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerView : MonoBehaviour
{
    [SerializeField] private float _tiltSensitivity;
    [SerializeField] private float _deadZone = 0.05f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private PlayerInputActions _input;

    private float _keyboardInput;
    private float _gyroInput;
    private float _accelInput;

    public Transform Transform => transform;

    public event Action<ObstacleView> HitObstacle;

    public float HorizontalInput
    {
        get
        {
#if UNITY_ANDROID || UNITY_IOS
            if (_hasGyro)
                return _gyroInput;

            if (_hasAccel)
                return _accelInput;
#endif
            return _keyboardInput;
        }
    }

#if UNITY_ANDROID || UNITY_IOS
    private bool _hasGyro;
    private bool _hasAccel;
#endif

    private void Awake()
    {
        _input = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _input.Player.Enable();
        _input.Player.Move.performed += OnMove;
        _input.Player.Move.canceled += OnMove;

#if UNITY_ANDROID || UNITY_IOS
        _hasGyro = AttitudeSensor.current != null;
        _hasAccel = Accelerometer.current != null;

        if (_hasGyro)
            InputSystem.EnableDevice(AttitudeSensor.current);

        if (_hasAccel)
            InputSystem.EnableDevice(Accelerometer.current);
#endif
    }

    private void OnDisable()
    {
        _input.Player.Move.performed -= OnMove;
        _input.Player.Move.canceled -= OnMove;
        _input.Player.Disable();
    }

    private void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (_hasGyro)
        {
            UpdateGyro();
            return;
        }

        if (_hasAccel)
        {
            UpdateAccelerometer();
            return;
        }
#endif
    }

    private void UpdateGyro()
    {
        Quaternion q = AttitudeSensor.current.attitude.ReadValue();

        float tilt =
#if UNITY_IOS
            q.x;
#else
            q.y;
#endif

        _gyroInput = ApplyDeadZone(tilt * _tiltSensitivity);
    }

    private void UpdateAccelerometer()
    {
        Vector3 accel = Accelerometer.current.acceleration.ReadValue();
        _accelInput = ApplyDeadZone(accel.x * _tiltSensitivity);
    }

    private float ApplyDeadZone(float value)
    {
        if (Mathf.Abs(value) < _deadZone)
            return 0f;

        return Mathf.Clamp(value, -1f, 1f);
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        _keyboardInput = ctx.ReadValue<float>();
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void SetRotation(float zAngle)
    {
        transform.rotation = Quaternion.Euler(0f, 0f, zAngle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<ObstacleView>(out var obstacle))
        {
            HitObstacle?.Invoke(obstacle);
        }
    }
}

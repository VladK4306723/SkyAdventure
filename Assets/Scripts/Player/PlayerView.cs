using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerView : MonoBehaviour
{
    [SerializeField] private float _tiltSensitivity = 3f;
    [SerializeField] private float _deadZone = 0.08f;
    [SerializeField] private float _smoothing = 10f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private PlayerInputActions _input;

    private Vector3 _rawTilt;
    private float _smoothedTilt;

    private float _keyboardInput;
    private float _finalInput;

    private bool _hasAccel;

    public float HorizontalInput => _finalInput;

    public event Action<ObstacleView> HitObstacle;

    private void Awake()
    {
        _input = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _input.Enable();

        _input.Player.Move.performed += OnMove;
        _input.Player.Move.canceled += OnMove;

        _input.Player.Tilt.performed += OnTilt;
        _input.Player.Tilt.canceled += OnTilt;

#if UNITY_ANDROID || UNITY_IOS
        _hasAccel = Accelerometer.current != null;
        if (_hasAccel)
            InputSystem.EnableDevice(Accelerometer.current);
#endif
    }

    private void OnDisable()
    {
        _input.Player.Move.performed -= OnMove;
        _input.Player.Move.canceled -= OnMove;

        _input.Player.Tilt.performed -= OnTilt;
        _input.Player.Tilt.canceled -= OnTilt;

        _input.Disable();
    }

    private void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!_hasAccel)
        {
            _finalInput = 0f;
            return;
        }

        float raw = _rawTilt.x;

        _smoothedTilt = Mathf.Lerp(_smoothedTilt, raw, Time.deltaTime * _smoothing);

        float value = _smoothedTilt * _tiltSensitivity;

        if (Mathf.Abs(value) < _deadZone)
            value = 0f;

        _finalInput = Mathf.Clamp(value, -1f, 1f);
#else
        _finalInput = _keyboardInput;
#endif
    }

    private void OnTilt(InputAction.CallbackContext ctx)
    {
        _rawTilt = ctx.ReadValue<Vector3>();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        _keyboardInput = ctx.ReadValue<float>();
#endif
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
            HitObstacle?.Invoke(obstacle);
    }
}

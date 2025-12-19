using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerView : MonoBehaviour
{
    [SerializeField] private float _tiltSensitivity = 2f;
    [SerializeField] private float _deadZone = 0.05f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private PlayerInputActions _input;
    private float _horizontal;
    public Transform Transform => transform;

    public event Action<ObstacleView> HitObstacle;


    public float HorizontalInput => _horizontal;

    private void Awake()
    {
        _input = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _input.Player.Enable();
        _input.Player.Move.performed += OnMove;
        _input.Player.Move.canceled += OnMove;
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
        if (AttitudeSensor.current == null)
            return;

        Quaternion q = AttitudeSensor.current.attitude.ReadValue();

        float tilt =
#if UNITY_IOS
            q.x;
#else
            q.y;
#endif

        if (Mathf.Abs(tilt) < _deadZone)
            tilt = 0f;

        _horizontal = Mathf.Clamp(tilt * _tiltSensitivity, -1f, 1f);
#endif
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        _horizontal = ctx.ReadValue<float>();
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

using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private PlayerInputActions _input;
    private float _horizontal;

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
}

using UnityEngine;
public interface IInputService
{
    float Horizontal { get; }
}
public class InputService : IInputService
{
    public float Horizontal =>
        Input.GetAxisRaw("Horizontal");
}

using UnityEngine;

public sealed class CameraBounds
{
    public float Left { get; }
    public float Right { get; }
    public float Top { get; }
    public float Bottom { get; }

    public CameraBounds(Camera camera, float padding = 0f)
    {
        float height = camera.orthographicSize;
        float width = height * camera.aspect;

        Vector3 pos = camera.transform.position;

        Left = pos.x - width + padding;
        Right = pos.x + width - padding;
        Top = pos.y + height + padding;
        Bottom = pos.y - height - padding;
    }
}

using UnityEngine;

interface IObstacleView
{
    ObstacleType Type { get; }
    public void Consume();
}

public interface IDespawnable
{
    bool ShouldDespawn(float bottomY);
}


public class ObstacleView : MonoBehaviour, IObstacleView, IGameTick, IDespawnable
{
    private ObstacleType type;
    private float _speed;

    public ObstacleType Type => type;

    public void Init(ObstacleConfig config, float gameSpeed)
    {
        type = config.Key;
        _speed = gameSpeed;

        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = config.Sprite;
        }
    }

    public void Tick(float dt)
    {
        transform.position += Vector3.down * _speed * dt;
    }

    public void Consume()
    {
        Destroy(gameObject);
    }

    public bool ShouldDespawn(float bottomY)
    {
        return transform.position.y < bottomY;
    }

}


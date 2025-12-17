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
    private float _actionValue;
    private Sprite _sprite;
    private ObstacleConfig _config;

    public float ActionValue => _actionValue;
    public ObstacleType Type => type;
    public Sprite Sprite => _sprite;
    public ObstacleConfig Config => _config;

    public void Init(ObstacleConfig config, float gameSpeed)
    {
        _config = config;

        type = config.Key;
        _speed = gameSpeed;
        _actionValue = config.ActionValue;

        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            _sprite = config.Sprite;
            spriteRenderer.sprite = _sprite;
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


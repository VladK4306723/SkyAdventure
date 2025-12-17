using UnityEngine;
using Zenject;

public interface IObstacleFactory
{
    ObstacleView Create(ObstacleType type, float x, float y, float gameSpeed);
    void Release(ObstacleView obstacle);
}

public sealed class ObstacleFactory : IObstacleFactory
{
    private readonly ObstacleConfigRegistry _configs;
    private readonly ObstaclePool _pool;

    public ObstacleFactory(
        ObstacleConfigRegistry configs,
        ObstaclePool pool)
    {
        _configs = configs;
        _pool = pool;
    }

    public ObstacleView Create(
        ObstacleType type,
        float x,
        float y,
        float speed)
    {
        var config = _configs.Get(type);

        var obstacle = _pool.Get();
        obstacle.transform.position = new Vector3(x, y, 0f);
        obstacle.Init(config, speed);

        return obstacle;
    }

    public void Release(ObstacleView obstacle)
    {
        _pool.Release(obstacle);
    }
}


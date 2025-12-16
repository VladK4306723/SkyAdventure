using UnityEngine;
using Zenject;

public interface IObstacleFactory
{
    ObstacleView Create(ObstacleType type, float x, float y, float gameSpeed);
}

public class ObstacleFactory : IObstacleFactory
{
    private readonly DiContainer _container;
    private readonly ObstacleConfigRegistry _registry;
    private readonly ObstacleView _prefab;

    public ObstacleFactory(
        DiContainer container,
        ObstacleConfigRegistry registry,
        ObstacleView prefab)
    {
        _container = container;
        _registry = registry;
        _prefab = prefab;
    }

    public ObstacleView Create(ObstacleType type, float x, float y, float gameSpeed)
    {
        var config = _registry.Get(type);

        var view = _container
            .InstantiatePrefabForComponent<ObstacleView>(_prefab);

        view.transform.position = new Vector3(x, y, 0f);
        view.Init(config, gameSpeed);

        return view;
    }
}


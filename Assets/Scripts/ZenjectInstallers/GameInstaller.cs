using UnityEngine;
using Zenject;

public sealed class GameInstaller : MonoInstaller
{
    [SerializeField] private PlayerView playerPrefab;
    [SerializeField] private PlayerConfigRegistry playerConfigRegistry;

    [SerializeField] private ObstacleView obstaclePrefab;
    [SerializeField] private ObstacleConfigRegistry obstacleConfigRegistry;


    public override void InstallBindings()
    {
        var bounds = new CameraBounds(Camera.main, 0.5f);

        Container.Bind<CameraBounds>()
        .FromInstance(bounds)
        .AsSingle();

        Container.Bind<IPlayerFactory>()
            .To<PlayerFactory>()
            .AsSingle()
            .WithArguments(playerPrefab);

        Container.BindInstance(playerConfigRegistry);

        Container.Bind<ObstaclePool>()
            .AsSingle()
            .WithArguments(obstaclePrefab, 16);

        Container.Bind<IObstacleFactory>()
            .To<ObstacleFactory>()
            .AsSingle();


        Container.BindInstance(obstacleConfigRegistry);
    }
}

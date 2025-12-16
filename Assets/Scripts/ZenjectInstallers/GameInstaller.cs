using UnityEngine;
using Zenject;

public sealed class GameInstaller : MonoInstaller
{
    [SerializeField] private PlayerView playerPrefab;
    [SerializeField] private PlayerConfigRegistry playerConfigRegistry;

    public override void InstallBindings()
    {
        Container.Bind<IPlayerFactory>()
            .To<PlayerFactory>()
            .AsSingle()
            .WithArguments(playerPrefab);

        Container.BindInstance(playerConfigRegistry);
    }
}

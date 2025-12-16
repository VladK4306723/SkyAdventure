using Zenject;

public interface IPlayerFactory
{
    PlayerController Create(PlayerType type);
}

public sealed class PlayerFactory : IPlayerFactory
{
    private readonly DiContainer _container;
    private readonly PlayerView _playerPrefab;
    private readonly PlayerConfigRegistry _registry;
    private readonly CameraBounds _bounds;

    public PlayerFactory(
        DiContainer container,
        PlayerView playerPrefab,
        PlayerConfigRegistry registry,
        CameraBounds bounds)
    {
        _container = container;
        _playerPrefab = playerPrefab;
        _registry = registry;
        _bounds = bounds;
    }

    public PlayerController Create(PlayerType type)
    {
        var config = _registry.Get(type);

        var view = _container
            .InstantiatePrefabForComponent<PlayerView>(_playerPrefab);

        view.SetSprite(config.Sprite);

        var model = new PlayerModel(config);

        return _container.Instantiate<PlayerController>(
            new object[] { model, view, _bounds }
        );
    }
}


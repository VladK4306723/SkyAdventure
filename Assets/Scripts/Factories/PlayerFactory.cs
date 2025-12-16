using Zenject;

public interface IPlayerFactory
{
    PlayerController Create(PlayerConfig config);
}

public sealed class PlayerFactory : IPlayerFactory
{
    private readonly DiContainer _container;
    private readonly PlayerView _playerPrefab;

    public PlayerFactory(
        DiContainer container,
        PlayerView playerPrefab)
    {
        _container = container;
        _playerPrefab = playerPrefab;
    }

    public PlayerController Create(PlayerConfig config)
    {
        var view = _container
            .InstantiatePrefabForComponent<PlayerView>(_playerPrefab);

        view.SetSprite(config.Sprite);

        var model = new PlayerModel(config);

        return _container.Instantiate<PlayerController>(
            new object[] { model, view }
        );
    }
}

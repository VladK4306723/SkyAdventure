using System.Collections.Generic;
using UnityEngine;
using Zenject;

interface IGameManager
{
    void StartGame(PlayerType playerType);
}

public class GameManager : MonoBehaviour, IGameManager
{
    [Inject] private IPlayerFactory _playerFactory;
    [Inject] private IObstacleFactory _obstacleFactory;
    [Inject] private CameraBounds _bounds;
    [SerializeField] private GameObject coinEffectPrefab;
    [SerializeField] private float gameSpeed = 3f;

    private ObstacleSpawner _spawner;
    private PlayerController _player;
    private int _score;

    private readonly List<IGameTick> _ticks = new();

    private void Start()
    {
        StartGame(PlayerType.Default);

        _spawner = new ObstacleSpawner(
        _obstacleFactory,
        _bounds,
        this,
        patternInterval: 1f,
        verticalSpacing: 0.6f,
        gameSpeed: gameSpeed
    );

        _spawner.Spawned += RegisterObstacle;
        _player.View.HitObstacle += OnPlayerHitObstacle;
    }

    private void RegisterObstacle(ObstacleView obstacle)
    {
        _ticks.Add(obstacle);
    }


    public void StartGame(PlayerType playerType)
    {
        _player = _playerFactory.Create(playerType);
        _ticks.Add(_player);
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        _spawner.Tick(dt);

        for (int i = _ticks.Count - 1; i >= 0; i--)
        {
            var tickable = _ticks[i];
            tickable.Tick(dt);

            if (tickable is IDespawnable despawnable &&
    despawnable.ShouldDespawn(_bounds.Bottom))
            {
                var obstacle = (ObstacleView)tickable;
                _ticks.RemoveAt(i);
                _obstacleFactory.Release(obstacle);

            }

        }
    }


    private void OnPlayerHitObstacle(ObstacleView obstacle)
    {
        if (obstacle.Type == ObstacleType.Coin)
        {
            HandleCoin(obstacle);
        }
    }

    private void HandleCoin(ObstacleView coin)
    {
        _score += 1;

        SpawnCoinEffect(coin.transform.position);

        _ticks.Remove(coin);
        _obstacleFactory.Release(coin);

    }

    private void SpawnCoinEffect(Vector3 position)
    {
        Instantiate(coinEffectPrefab, position, Quaternion.identity);
    }

    private void OnDestroy()
    {
        _spawner.Spawned -= RegisterObstacle;

        if (_player != null)
            _player.View.HitObstacle -= OnPlayerHitObstacle;
    }

}

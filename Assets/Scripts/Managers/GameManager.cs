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

    private PlayerController _player;
    private int _score;

    private readonly List<IGameTick> _ticks = new();

    private void Start()
    {
        StartGame(PlayerType.Default);

        _player.View.HitObstacle += OnPlayerHitObstacle;
    }

    public void StartGame(PlayerType playerType)
    {
        _player = _playerFactory.Create(playerType);

        var obstacle = _obstacleFactory.Create(ObstacleType.Coin, 2f, 2f, gameSpeed);
        _ticks.Add(_player);
        _ticks.Add(obstacle);
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        for (int i = _ticks.Count - 1; i >= 0; i--)
        {
            var tickable = _ticks[i];
            tickable.Tick(dt);

            if (tickable is IDespawnable despawnable &&
    despawnable.ShouldDespawn(_bounds.Bottom))
            {
                _ticks.RemoveAt(i);
                ((ObstacleView)tickable).Consume();
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

        coin.Consume();
    }

    private void SpawnCoinEffect(Vector3 position)
    {
        Instantiate(coinEffectPrefab, position, Quaternion.identity);
    }

    private void OnDestroy()
    {
        _player.View.HitObstacle -= OnPlayerHitObstacle;
    }
}

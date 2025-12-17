using System.Collections.Generic;
using Unity.VisualScripting;
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
    [Inject] private PickupEffectView.Pool _starEffectPool;

    [SerializeField] private PickupEffectView starEffectPrefab;
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
        if (obstacle.Type == ObstacleType.Star)
        {
            HandleStar(obstacle);
        }
    }

    private void HandleStar(ObstacleView star)
    {
        var cfg = star.Config;
        Vector3 pos = star.transform.position;

        _score += (int)cfg.ActionValue;

        _ticks.Remove(star);
        _obstacleFactory.Release(star);

        SpawnStarEffect(pos, cfg);

        Debug.Log($"Score: {_score}");
    }

    private void SpawnStarEffect(Vector3 position, ObstacleConfig config)
    {
        var effect = _starEffectPool.Spawn();
        effect.transform.position = position;
        effect.SetPool(_starEffectPool);
        effect.Init(config.Sprite, config.ActionValue);
    }



    private void OnDestroy()
    {
        _spawner.Spawned -= RegisterObstacle;

        if (_player != null)
            _player.View.HitObstacle -= OnPlayerHitObstacle;
    }

}

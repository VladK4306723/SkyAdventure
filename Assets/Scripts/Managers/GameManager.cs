using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

interface IGameFlow
{
    void StartGame(PlayerType playerType);
}

interface IGameManager
{
}

public class GameManager : MonoBehaviour, IGameFlow, IGameManager
{
    [Inject] private IPlayerFactory _playerFactory;
    [Inject] private IObstacleFactory _obstacleFactory;
    [Inject] private CameraBounds _bounds;
    [Inject] private PickupEffectView.Pool _starEffectPool;
    [Inject] private IUIManager _uiManager;

    [SerializeField] private PickupEffectView starEffectPrefab;
    [SerializeField] private float gameSpeed = 3f;

    private ObstacleSpawner _spawner;
    private PlayerController _player;
    private int _score;
    private bool _isGameOver;
    private bool _isGameRunning;

    private readonly List<IGameTick> _ticks = new();

    private void Start()
    {
        _uiManager.Show(UIWindowId.Home);
    }

    private void RegisterObstacle(ObstacleView obstacle)
    {
        _ticks.Add(obstacle);
    }


    public void StartGame(PlayerType playerType)
    {
        _isGameRunning = true;

        _spawner = new ObstacleSpawner(
        _obstacleFactory,
        _bounds,
        this,
        patternInterval: 1f,
        verticalSpacing: 0.6f,
        gameSpeed: gameSpeed
        );

        _player = _playerFactory.Create(playerType);
        _ticks.Add(_player);

        _player.DangerMaxed += OnDangerLevelMaxed;
        _spawner.Spawned += RegisterObstacle;
        _player.View.HitObstacle += OnPlayerHitObstacle;

        
    }

    private void Update()
    {
        if (_isGameOver)
            return;

        if (!_isGameRunning)
            return;

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

    private void OnDangerLevelMaxed()
    {
        Debug.Log("BOOM");
        _isGameOver = true;
        _isGameRunning = false;

        StopGame();

        _uiManager.Show(UIWindowId.Home);
    }

    private void StopGame()
    {
        _isGameRunning = false;
        _isGameOver = false;

        _player.Cleanup();

        if (_player != null)
        {
            _player.DangerMaxed -= OnDangerLevelMaxed;
            _player.View.HitObstacle -= OnPlayerHitObstacle;
        }

        if (_spawner != null)
        {
            _spawner.Spawned -= RegisterObstacle;
        }

        for (int i = _ticks.Count - 1; i >= 0; i--)
        {
            if (_ticks[i] is ObstacleView obstacle)
            {
                _obstacleFactory.Release(obstacle);
            }
        }

        _ticks.Clear();

        if (_player != null)
        {
            Destroy(_player.View.gameObject);
            _player = null;
        }

        _spawner = null;
        _score = 0;
    }



    private void OnDestroy()
    {
        _spawner.Spawned -= RegisterObstacle;

        if (_player != null)
            _player.View.HitObstacle -= OnPlayerHitObstacle;
    }

}

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
    [Inject] private PlayerConfigRegistry _playerConfigs;

    private readonly List<IGameTick> _ticks = new();

    private void Start()
    {
        StartGame(PlayerType.Default);
    }

    public void StartGame(PlayerType playerType)
    {
        var config = _playerConfigs.Get(playerType);
        var controller = _playerFactory.Create(config);
        _ticks.Add(controller);
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        foreach (var tickable in _ticks)
        {
            tickable.Tick(dt);
        }
    }
}

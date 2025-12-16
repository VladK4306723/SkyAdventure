using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player/PlayerConfigRegistry")]
public sealed class PlayerConfigRegistry : ScriptableObject
{
    [SerializeField] private List<PlayerConfig> configs;

    private Dictionary<PlayerType, PlayerConfig> _cache;

    public PlayerConfig Get(PlayerType type)
    {
        if (_cache == null)
        {
            _cache = new Dictionary<PlayerType, PlayerConfig>();
            foreach (var config in configs)
            {
                _cache[config.Type] = config;
            }
        }

        return _cache[type];
    }
}

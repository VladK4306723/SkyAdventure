using System.Collections.Generic;
using UnityEngine;

public interface IConfigWithKey<TKey>
{
    TKey Key { get; }
}


public abstract class ConfigRegistry<TKey, TConfig> : ScriptableObject
    where TConfig : ScriptableObject, IConfigWithKey<TKey>
{
    [SerializeField] private List<TConfig> configs;

    private Dictionary<TKey, TConfig> _cache;

    public TConfig Get(TKey key)
    {
        if (_cache == null)
        {
            _cache = new Dictionary<TKey, TConfig>();
            foreach (var config in configs)
            {
                _cache[config.Key] = config;
            }
        }

        return _cache[key];
    }
}

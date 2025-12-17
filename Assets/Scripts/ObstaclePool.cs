using System.Collections.Generic;
using UnityEngine;

public sealed class ObstaclePool
{
    private readonly ObstacleView _prefab;
    private readonly Transform _root;
    private readonly Stack<ObstacleView> _pool = new();

    public ObstaclePool(ObstacleView prefab, int preload)
    {
        _prefab = prefab;
        _root = new GameObject("[ObstaclePool]").transform;

        for (int i = 0; i < preload; i++)
            CreateNew();
    }

    public ObstacleView Get()
    {
        if (_pool.Count == 0)
            CreateNew();

        var obj = _pool.Pop();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Release(ObstacleView obj)
    {
        obj.gameObject.SetActive(false);
        _pool.Push(obj);
    }

    private void CreateNew()
    {
        var obj = Object.Instantiate(_prefab, _root);
        obj.gameObject.SetActive(false);
        _pool.Push(obj);
    }
}

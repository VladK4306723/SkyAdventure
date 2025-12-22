using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class CloudSpawner : MonoBehaviour
{
    [Inject] private CameraBounds _bounds;

    [Header("Initial Spawn")]
    [SerializeField, Min(0)] private int _initialCloudCount = 4;


    [Header("References")]
    [SerializeField] private Transform _container;
    [SerializeField] private SpriteRenderer _cloudPrefab;

    [Header("Spawn")]
    [SerializeField] private float _spawnInterval = 1.5f;
    [SerializeField] private Vector2 _scaleRange = new Vector2(0.6f, 1.2f);

    [Header("Movement")]
    [SerializeField] private Vector2 _speedRange = new Vector2(0.5f, 1.5f);
    [SerializeField] private float _destroyOffset = 1.5f;

    private float _timer;
    private readonly List<Cloud> _clouds = new();

    private void Start()
    {
        SpawnInitialClouds();
    }


    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _spawnInterval)
        {
            _timer = 0f;
            SpawnCloud();
        }

        UpdateClouds();
    }

    private void SpawnInitialClouds()
    {
        for (int i = 0; i < _initialCloudCount; i++)
        {
            SpawnCloudAtRandomHeight();
        }
    }

    private void SpawnCloudAtRandomHeight()
    {
        var cloud = Instantiate(_cloudPrefab, _container);
        var t = cloud.transform;

        float scale = Random.Range(_scaleRange.x, _scaleRange.y);
        t.localScale = Vector3.one * scale;

        float x = Random.Range(_bounds.Left, _bounds.Right);
        float y = Random.Range(
            _bounds.Bottom - _destroyOffset,
            _bounds.Top + _destroyOffset
        );

        t.position = new Vector3(x, y, 0f);

        float speed = Random.Range(_speedRange.x, _speedRange.y);

        _clouds.Add(new Cloud
        {
            Transform = t,
            Speed = speed
        });
    }


    private void SpawnCloud()
    {
        var cloud = Instantiate(_cloudPrefab, _container);
        var t = cloud.transform;

        float scale = Random.Range(_scaleRange.x, _scaleRange.y);
        t.localScale = Vector3.one * scale;

        float x = Random.Range(_bounds.Left, _bounds.Right);
        float y = _bounds.Top + _destroyOffset;

        t.position = new Vector3(x, y, 0f);

        float speed = Random.Range(_speedRange.x, _speedRange.y);

        _clouds.Add(new Cloud
        {
            Transform = t,
            Speed = speed
        });
    }

    private void UpdateClouds()
    {
        for (int i = _clouds.Count - 1; i >= 0; i--)
        {
            var cloud = _clouds[i];
            var pos = cloud.Transform.position;

            pos.y -= cloud.Speed * Time.deltaTime;
            cloud.Transform.position = pos;

            if (pos.y < _bounds.Bottom - _destroyOffset)
            {
                Destroy(cloud.Transform.gameObject);
                _clouds.RemoveAt(i);
            }
        }
    }

    private struct Cloud
    {
        public Transform Transform;
        public float Speed;
    }
}

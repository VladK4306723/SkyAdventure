using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloudBackground : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _container;
    [SerializeField] private Image _cloudPrefab;

    [Header("Spawn")]
    [SerializeField] private float _spawnInterval = 1.5f;
    [SerializeField] private Vector2 _xRange = new Vector2(-180f, 180f);
    [SerializeField] private Vector2 _scaleRange = new Vector2(0.6f, 1.2f);

    [Header("Movement")]
    [SerializeField] private Vector2 _speedRange = new Vector2(20f, 60f);
    [SerializeField] private float _destroyOffset = 150f;

    private float _timer;
    private readonly List<Cloud> _clouds = new();

    private float TopY => _container.rect.height * 0.5f + _destroyOffset;
    private float BottomY => -_container.rect.height * 0.5f - _destroyOffset;

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

    private void SpawnCloud()
    {
        var cloudImage = Instantiate(_cloudPrefab, _container);
        var rect = cloudImage.rectTransform;

        float scale = Random.Range(_scaleRange.x, _scaleRange.y);
        rect.localScale = Vector3.one * scale;

        float x = Random.Range(_xRange.x, _xRange.y);
        rect.anchoredPosition = new Vector2(x, TopY);

        float speed = Random.Range(_speedRange.x, _speedRange.y);

        _clouds.Add(new Cloud
        {
            Rect = rect,
            Speed = speed
        });
    }

    private void UpdateClouds()
    {
        for (int i = _clouds.Count - 1; i >= 0; i--)
        {
            var cloud = _clouds[i];
            var pos = cloud.Rect.anchoredPosition;
            pos.y -= cloud.Speed * Time.deltaTime;
            cloud.Rect.anchoredPosition = pos;

            if (pos.y < BottomY)
            {
                Destroy(cloud.Rect.gameObject);
                _clouds.RemoveAt(i);
            }
        }
    }

    private struct Cloud
    {
        public RectTransform Rect;
        public float Speed;
    }
}

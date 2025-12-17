using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ObstacleSpawner
{
    private static readonly float[] X_POSITIONS =
    {
        -1.5f, -1.0f, -0.5f, 0f, 0.5f, 1.0f, 1.5f
    };

    private readonly IObstacleFactory _factory;
    private readonly CameraBounds _bounds;
    private readonly MonoBehaviour _coroutineHost;

    private readonly float _patternInterval;
    private readonly float _verticalSpacing;
    private readonly float _gameSpeed;

    private float _timer;
    private bool _isSpawning;


    public event System.Action<ObstacleView> Spawned;

    public ObstacleSpawner(
        IObstacleFactory factory,
        CameraBounds bounds,
        MonoBehaviour coroutineHost,
        float patternInterval = 1f,
        float verticalSpacing = 0.6f,
        float gameSpeed = 3f)
    {
        _factory = factory;
        _bounds = bounds;
        _coroutineHost = coroutineHost;
        _patternInterval = patternInterval;
        _verticalSpacing = verticalSpacing;
        _gameSpeed = gameSpeed;

    }

    public void Tick(float dt)
    {
        if (_isSpawning)
            return;

        _timer += dt;
        if (_timer >= _patternInterval)
        {
            _timer = 0f;
            _isSpawning = true;
            _coroutineHost.StartCoroutine(SpawnSequence());
        }
    }

    private IEnumerator SpawnSequence()
    {
        var type = (ObstaclePatternType)
            Random.Range(0, System.Enum.GetValues(typeof(ObstaclePatternType)).Length);

        yield return _coroutineHost.StartCoroutine(SpawnPatternRoutine(type));

        yield return new WaitForSeconds(_patternInterval);

        _isSpawning = false;
    }

    private float SpawnDelay =>
        _verticalSpacing / _gameSpeed;

    private IEnumerator SpawnPatternRoutine(ObstaclePatternType type)
    {
        switch (type)
        {
            case ObstaclePatternType.Single:
                yield return SpawnSingle();
                break;

            case ObstaclePatternType.Row:
                yield return SpawnRow();
                break;

            case ObstaclePatternType.Smile:
                yield return SpawnSmile();
                break;

            case ObstaclePatternType.Square:
                yield return SpawnSquare();
                break;

            case ObstaclePatternType.WaveRight:
                yield return SpawnWaveRight();
                break;

            case ObstaclePatternType.WaveLeft:
                yield return SpawnWaveLeft();
                break;
        }
    }

    // ───────────────────────── PATTERNS ─────────────────────────

    private IEnumerator SpawnSingle()
    {
        SpawnStar(RandomX());
        yield break;
    }

    private IEnumerator SpawnRow()
    {
        int count = Random.Range(2, 8);
        var indices = GetRandomUniqueIndices(count);

        foreach (int i in indices)
        {
            SpawnStar(X_POSITIONS[i]);
        }

        yield break;
    }

    private IEnumerator SpawnWaveRight()
    {
        for (int i = 0; i < X_POSITIONS.Length; i++)
        {
            SpawnStar(X_POSITIONS[i]);
            yield return new WaitForSeconds(SpawnDelay);
        }
    }

    private IEnumerator SpawnWaveLeft()
    {
        for (int i = X_POSITIONS.Length - 1; i >= 0; i--)
        {
            SpawnStar(X_POSITIONS[i]);
            yield return new WaitForSeconds(SpawnDelay);
        }
    }

    private IEnumerator SpawnSquare()
    {
        int start = Random.Range(0, X_POSITIONS.Length - 1);

        SpawnStar(X_POSITIONS[start]);
        SpawnStar(X_POSITIONS[start + 1]);

        yield return new WaitForSeconds(SpawnDelay);

        SpawnStar(X_POSITIONS[start]);
        SpawnStar(X_POSITIONS[start + 1]);
    }

    private IEnumerator SpawnSmile()
    {
        float[][] SMILE_PATTERN =
        {
            new[] { -0.5f,  0f,  0.5f }, 
            new[] { -1f,           1f }, 
            null,                        
            new[] { -0.5f,        0.5f } 
        };

        foreach (var row in SMILE_PATTERN)
        {
            if (row != null)
            {
                foreach (float x in row)
                {
                    SpawnStar(x);
                }
            }

            yield return new WaitForSeconds(SpawnDelay);
        }
    }

    // ───────────────────────── HELPERS ─────────────────────────

    private void SpawnStar(float x)
    {
        ObstacleView obstacle = _factory.Create(
            ObstacleType.Star,
            x,
            _bounds.Top + 0.5f,
            _gameSpeed
        );

        Spawned?.Invoke(obstacle);
    }

    private float RandomX()
    {
        return X_POSITIONS[Random.Range(0, X_POSITIONS.Length)];
    }

    private List<int> GetRandomUniqueIndices(int count)
    {
        var list = new List<int>(X_POSITIONS.Length);
        for (int i = 0; i < X_POSITIONS.Length; i++)
            list.Add(i);

        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }

        return list.GetRange(0, count);
    }
}

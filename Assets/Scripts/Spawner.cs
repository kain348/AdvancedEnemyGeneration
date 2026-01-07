using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[AddComponentMenu("Game/Spawners/Spawner")]
public class Spawner : MonoBehaviour
{
    [Header("Settings Map: Points")]
    [SerializeField] private List<SpawnPoint> _spawnPoints = new List<SpawnPoint>();

    [Header("Spawn Settings")]
    [SerializeField] private int _maxPoolSize = 50;
    [SerializeField] private int _prewarmPerType = 10;
    [SerializeField] private float _spawnDelay = 2f;

    private bool _canSpawn = true;
    private Coroutine _spawnCoroutine;
    private WaitForSeconds _spawnWait;

    private readonly Dictionary<Enemy, CustomPool<Enemy>> _poolsByPrefab = new Dictionary<Enemy, CustomPool<Enemy>>();
    private readonly Dictionary<Enemy, CustomPool<Enemy>> _poolByInstance = new Dictionary<Enemy, CustomPool<Enemy>>();
    private HashSet<Enemy> _activeEnemies = new HashSet<Enemy>();

    private void Start()
    {
        _spawnWait = new WaitForSeconds(_spawnDelay);
        _spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        _canSpawn = false;

        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }

        foreach (var enemy in _activeEnemies)
        {
            if (enemy != null)
            {
                enemy.ReachedTarget -= OnEnemyReachedTarget;

                if (_poolByInstance.TryGetValue(enemy, out var pool))
                {
                    enemy.Reset();
                    pool.Release(enemy);
                }
            }
        }

        _activeEnemies.Clear();
        _poolByInstance.Clear();
        _poolsByPrefab.Clear();
    }

    private IEnumerator SpawnRoutine()
    {
        yield return null;

        while (_canSpawn)
        {
            Spawned();

            yield return _spawnWait;
        }
    }

    private void Spawned()
    {
        SpawnPoint spawnPoint = GetRandomSpawnPoint();
        if (spawnPoint == null)
            return;

        Enemy prefab = spawnPoint.EnemyPrefab;
        if (prefab == null)
        {
            Debug.LogWarning($"SpawnPoint {spawnPoint.name} has no EnemyPrefab assigned.", spawnPoint);
            return;
        }

        CustomPool<Enemy> pool = GetOrCreatePool(prefab);
        if (pool == null)
            return;

        Enemy enemy = pool.Get();
        if (enemy == null)
            return;

        _poolByInstance[enemy] = pool;

        if (ConfigureEnemy(enemy, spawnPoint) == false)
        {
            _poolByInstance.Remove(enemy);
            pool.Release(enemy);

            return;
        }

        enemy.ReachedTarget += OnEnemyReachedTarget;
        _activeEnemies.Add(enemy);
    }

    private CustomPool<Enemy> GetOrCreatePool(Enemy prefab)
    {
        if (prefab == null)
            return null;

        if (_poolsByPrefab.TryGetValue(prefab, out var existingPool))
            return existingPool;

        var newPool = new CustomPool<Enemy>(prefab, _prewarmPerType, _maxPoolSize);
        _poolsByPrefab.Add(prefab, newPool);

        return newPool;
    }

    private bool ConfigureEnemy(Enemy enemy, SpawnPoint spawnPoint)
    {
        Target target = spawnPoint.Target;
        if (target == null)
        {
            Debug.LogWarning($"SpawnPoint {spawnPoint.name} has no Target assigned.", spawnPoint);
            return false;
        }

        enemy.Initialize(spawnPoint.GetPoint(), spawnPoint.GetRotation());
        enemy.Move(target);

        return true;
    }

    private void OnEnemyReachedTarget(Enemy enemy)
    {
        enemy.ReachedTarget -= OnEnemyReachedTarget;
        _activeEnemies.Remove(enemy);

        enemy.Reset();

        if (_poolByInstance.TryGetValue(enemy, out var pool))
        {
            _poolByInstance.Remove(enemy);
            pool.Release(enemy);
        }
        else
        {
            Debug.LogWarning($"No pool found for enemy instance {enemy.name}. Deactivating it.");
            enemy.gameObject.SetActive(false);
        }
    }

    private SpawnPoint GetRandomSpawnPoint()
    {
        if (_spawnPoints == null || _spawnPoints.Count == 0)
            throw new ArgumentException("spawnPoints");

        int minRandomIndex = 0;

        int randomIndex = Random.Range(minRandomIndex, _spawnPoints.Count);

        return _spawnPoints[randomIndex];
    }
}
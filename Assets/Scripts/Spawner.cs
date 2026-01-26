using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[AddComponentMenu("Game/Spawners/Spawner")]
public class Spawner : MonoBehaviour
{
    [Header("Settings Map: Points")]
    [Tooltip("List of spawn points at level.")]
    [SerializeField] private List<SpawnPoint> _spawnPoints = new List<SpawnPoint>();

    [Header("Spawn Settings")]
    [SerializeField] private int _maxPoolSize = 50;
    [SerializeField] private int _prewarmPerPoint = 10;
    [SerializeField] private float _spawnDelay = 2f;

    private bool _canSpawn = true;
    private Coroutine _spawnCoroutine;
    private WaitForSeconds _spawnWait;

    private void Start()
    {
        _spawnWait = new WaitForSeconds(_spawnDelay);

        foreach (SpawnPoint spawnPoint in _spawnPoints)
        {
            if (spawnPoint == null)
                continue;

            spawnPoint.InitializePool(_prewarmPerPoint, _maxPoolSize);
        }

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

        foreach (SpawnPoint spawnPoint in _spawnPoints)
        {
            if (spawnPoint == null)
                continue;

            spawnPoint.DeinitializePool();
        }
    }

    private IEnumerator SpawnRoutine()
    {
        yield return null;

        while (_canSpawn)
        {
            SpawnAtRandomPoint();

            yield return _spawnWait;
        }
    }

    private void SpawnAtRandomPoint()
    {
        SpawnPoint spawnPoint = GetRandomSpawnPoint();
        if (spawnPoint == null)
            return;

        spawnPoint.SpawnEnemy();
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
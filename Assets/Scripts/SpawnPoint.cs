using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Game/Spawners/Spawn Point")]
public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Config")]
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private Target _target;

    private CustomPool<Enemy> _pool;
    private readonly HashSet<Enemy> _activeEnemies = new HashSet<Enemy>();

    public Enemy EnemyPrefab => _enemyPrefab;
    public Target Target => _target;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }

    public void InitializePool(int prewarmCount, int maxPoolSize)
    {
        if (_enemyPrefab == null)
        {
            Debug.LogWarning($"{nameof(SpawnPoint)} on {name} has no EnemyPrefab assigned.", this);
            return;
        }

        _pool = new CustomPool<Enemy>(_enemyPrefab, prewarmCount, maxPoolSize);
    }

    public void DeinitializePool()
    {
        if (_pool == null)
            return;

        foreach (Enemy enemy in _activeEnemies)
        {
            if (enemy == null)
                continue;

            enemy.ReachedTarget -= OnEnemyReachedTarget;
            enemy.Reset();
            _pool.Release(enemy);
        }

        _activeEnemies.Clear();
    }

    public Enemy SpawnEnemy()
    {
        if (_pool == null)
            return null;

        Enemy enemy = _pool.Get();
        if (enemy == null)
            return null;

        if (ConfigureEnemy(enemy) == false)
        {
            _pool.Release(enemy);

            return null;
        }

        enemy.ReachedTarget += OnEnemyReachedTarget;
        _activeEnemies.Add(enemy);

        return enemy;
    }

    private bool ConfigureEnemy(Enemy enemy)
    {
        if (_target == null)
        {
            Debug.LogWarning($"{nameof(SpawnPoint)} on {name} has no Target assigned.", this);
            return false;
        }

        enemy.Initialize(transform.position, transform.rotation);
        enemy.Move(Target);

        return true;
    }

    private void OnEnemyReachedTarget(Enemy enemy)
    {
        enemy.ReachedTarget -= OnEnemyReachedTarget;
        _activeEnemies.Remove(enemy);

        enemy.Reset();
        _pool.Release(enemy);
    }
}
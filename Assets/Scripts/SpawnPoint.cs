using UnityEngine;

[AddComponentMenu("Game/Spawners/Spawn Point")]
public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private Target _target;

    public Enemy EnemyPrefab => _enemyPrefab;
    public Target Target => _target;

    public Vector3 GetPoint()
    {
        return transform.position;
    }

    public Quaternion GetRotation()
    {
        return transform.rotation;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
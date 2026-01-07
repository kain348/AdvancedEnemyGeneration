using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[AddComponentMenu("Game/Spawners/Spawn Point")]
public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private List<Target> _targets = new List<Target>();

    public Vector3 GetPoint()
    {
        return transform.position;
    }

    public Quaternion GetRotation()
    {
        return transform.rotation;
    }

    public Target GetTarget()
    {
        if(_targets == null || _targets.Count == 0)
            return null;

        int minCount = 0;

        if(_targets.Count == 1)
            return _targets[minCount];

        int index = Random.Range(minCount, _targets.Count);

        return _targets[index];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
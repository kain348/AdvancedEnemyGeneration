using UnityEngine;
using System;

[SelectionBase]
[RequireComponent(typeof(Mover))]
[RequireComponent(typeof(FollowTargetMover))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Game/Enemies/Enemy")]
public class Enemy : MonoBehaviour
{
    private Mover _mover;
    private FollowTargetMover _followTargetMover;
    private Collider _collider;
    private Rigidbody _rigidbody;

    public event Action<Enemy> ReachedTarget;

    private void Awake()
    {
        _mover = GetComponent<Mover>();
        _followTargetMover = GetComponent<FollowTargetMover>();
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.isKinematic = true;
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_followTargetMover.Target == null)
            return;

        if (other.TryGetComponent(out Target target))
        {
            if (target == _followTargetMover.Target)
            {
                ReachedTarget?.Invoke(this);
            }
        }
    }

    public void Reset()
    {
        _followTargetMover.ClearTarget();
        _mover.Stop();
        transform.rotation = Quaternion.identity;
    }

    public void Initialize(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    public void Move(Target target)
    {
        if (target == null)
            throw new ArgumentException(nameof(target));

        _followTargetMover.SetTarget(target);
    }
}
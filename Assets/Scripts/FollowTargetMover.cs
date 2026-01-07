using UnityEngine;

[RequireComponent(typeof(Mover))]
[AddComponentMenu("Game/Movement/Follow Target Mover")]
public class FollowTargetMover : MonoBehaviour
{
    [Header("Follow Settings")]
    [Tooltip("How far from the target to stop moving (so as not to shake in place).")]
    [SerializeField] private float _stopDistance = 0.1f;

    private IMovable _movable;
    private Target _target;

    public Target Target => _target;

    private void Awake()
    {
        _movable = GetComponent<IMovable>();

        if (_movable == null)
        {
            Debug.Log($"{nameof(FollowTargetMover)} requires a component that implements {nameof(IMovable)}", this);
        }
    }

    private void Update()
    {
        if (_movable == null)
            return;

        if (_target == null)
        {
            if (_movable.IsMoving)
                _movable.Stop();

            return;
        }

        Vector3 toTarget = _target.transform.position - transform.position;
        float sqrDistance = toTarget.sqrMagnitude;

        if (sqrDistance <= _stopDistance * _stopDistance)
        {
            if (_movable.IsMoving)
                _movable.Stop();

            return;
        }

        if (toTarget != Vector3.zero)
            _movable.Move(toTarget);
    }

    public void SetTarget(Target target)
    {
        _target = target;

        if (_target == null)
        {
            _movable?.Stop();
        }
    }

    public void ClearTarget()
    {
        _target = null;
        _movable?.Stop();
    }
}
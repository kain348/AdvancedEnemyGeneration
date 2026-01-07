using System;
using UnityEngine;

[AddComponentMenu("Game/Movement/Mover")]
public class Mover : MonoBehaviour, IMovable
{
    [Header("Movement Settings")]
    [SerializeField] private float _speed = 0.5f;

    [Tooltip("Automatically start motion when object is activated")]
    [SerializeField] private bool _isAutoStart = false;

    private Vector3 _movementDirection;

    public bool IsMoving { get; private set; }

    public float Speed
    {
        get => _speed;
        set => _speed = Mathf.Max(0.1f, value);
    }

    private void Start()
    {
        if (_isAutoStart && _movementDirection != Vector3.zero)
        {
            IsMoving = true;
        }
    }

    private void Update()
    {
        if (IsMoving == false || _movementDirection == Vector3.zero)
            return;

        transform.position += _movementDirection * (_speed * Time.deltaTime);
    }

    public void Move(Vector3 direction)
    {
        if (direction == Vector3.zero)
            throw new ArgumentNullException(nameof(direction));

        _movementDirection = direction.normalized;
        IsMoving = true;
    }

    public void Stop()
    {
        IsMoving = false;
        _movementDirection = Vector3.zero;
    }
}
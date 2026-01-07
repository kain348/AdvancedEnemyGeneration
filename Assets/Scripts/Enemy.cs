using UnityEngine;
using System;

[SelectionBase]
[RequireComponent(typeof(Mover))]
[RequireComponent(typeof(FollowTargetMover))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Game/Enemies/Enemy")]
public class Enemy : MonoBehaviour
{
    private Mover _mover;
    private FollowTargetMover _followTargetMover;
    private Collider _collider;
    private Renderer _renderer;
    private Rigidbody _rigidbody;

    private MaterialPropertyBlock _materialPropertyBlock;
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private Color _defaultColor;

    public event Action<Enemy> ReachedTarget;

    private void Awake()
    {
        _mover = GetComponent<Mover>();
        _followTargetMover = GetComponent<FollowTargetMover>();
        _collider = GetComponent<Collider>();
        _renderer = GetComponent<Renderer>();
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.isKinematic = true;
        _collider.isTrigger = true;

        _materialPropertyBlock = new MaterialPropertyBlock();

        if(_renderer.sharedMaterial != null)
        {
            _defaultColor = _renderer.sharedMaterial.color;
        }
        else
        {
            _defaultColor= Color.white;
        }
    }

    public void Reset()
    {
        _followTargetMover.ClearTarget();
        _mover.Stop();
        transform.rotation = Quaternion.identity;

        SetColor(_defaultColor);  
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

    public void SetColor(Color color)
    {
        _renderer.GetPropertyBlock(_materialPropertyBlock);
        _materialPropertyBlock.SetColor(ColorId, color);
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_followTargetMover.Target == null)
            return;

        if (other.TryGetComponent<Target>(out var target))
        {
            if (target == _followTargetMover.Target)
            {
                ReachedTarget?.Invoke(this);
            }
        }

        return;
    }
}
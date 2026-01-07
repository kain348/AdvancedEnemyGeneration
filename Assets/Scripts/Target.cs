using UnityEngine;

[RequireComponent (typeof(Collider))]
[RequireComponent (typeof(Renderer))]
[AddComponentMenu("Game/Targets/Target")]
public class Target : MonoBehaviour
{
    [SerializeField] private Color _color = Color.white;

    private Renderer _renderer;
    private MaterialPropertyBlock _materialPropertyBlock;
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    public Color Color => _color;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        ApplyColor();
    }

    private void OnValidate()
    {
        if (_renderer == null)
            _renderer = GetComponent<Renderer>();

        if (_materialPropertyBlock == null)
            _materialPropertyBlock = new MaterialPropertyBlock();

        ApplyColor();
    }

    private void ApplyColor()
    {
        if(_renderer == null|| _materialPropertyBlock == null) 
            return;

        _renderer.GetPropertyBlock(_materialPropertyBlock);
        _materialPropertyBlock.SetColor(ColorId, _color);
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }
}
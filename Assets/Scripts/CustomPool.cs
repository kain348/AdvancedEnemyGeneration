using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class CustomPool<T> where T : MonoBehaviour
{
    private readonly int _maxPoolSize;

    private readonly T _prefab;
    private readonly List<T> _allObjects = new List<T>();
    private readonly Queue<T> _availableObjects = new Queue<T>();

    public int Count => _allObjects.Count;
    public int Available => _availableObjects.Count;

    public CustomPool(T prefab, int prewarmObjects, int maxPoolSize = 100)
    {
        if (prefab is null)
            throw new System.ArgumentNullException(nameof(prefab));

        _prefab = prefab;
        _maxPoolSize = maxPoolSize;

        for (int i = 0; i < prewarmObjects; i++)
        {
            CreateNewObject();
        }
    }

    public T Get()
    {
        if (_availableObjects.Count > 0)
        {
            var @object = _availableObjects.Dequeue();
            InitializeObject(@object);

            return @object;
        }

        if (_allObjects.Count < _maxPoolSize)
        {
            return CreateNewObject();
        }

        return null;
    }

    public void Release(T @object)
    {
        if (@object == null) return;

        @object.gameObject.SetActive(false);

        _availableObjects.Enqueue(@object);
    }

    private void InitializeObject(T @object)
    {
        @object.gameObject.SetActive(true);
    }

    private T CreateNewObject()
    {
        var @object = Object.Instantiate(_prefab);
        @object.gameObject.SetActive(false);
        _allObjects.Add(@object);
        _availableObjects.Enqueue(@object);

        return @object;
    }
}
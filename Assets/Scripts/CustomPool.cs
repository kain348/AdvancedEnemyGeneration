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
            var @object = CreateNewObject();
            @object.gameObject.SetActive(false);
            _availableObjects.Enqueue(@object);
        }
    }

    public T Get()
    {
        while (_availableObjects.Count > 0)
        {
            var @object = _availableObjects.Dequeue();

            if (@object == null || @object.gameObject == null)
                continue;

            InitializeObject(@object);
           
            return @object;
        }

        if(_allObjects.Count>= _maxPoolSize)
            return null;

        var newObject = CreateNewObject();
        InitializeObject(newObject);

        return newObject;
    }

    public void Release(T @object)
    {
        if (@object == null) return;

        @object.gameObject.SetActive(false);

        _availableObjects.Enqueue(@object);
    }

    private void InitializeObject(T @object)
    {
        if (@object == null || @object.gameObject == null)
            return;

        @object.gameObject.SetActive(true);
    }

    private T CreateNewObject()
    {
        var @object = Object.Instantiate(_prefab);
        _allObjects.Add(@object);

        return @object;
    }
}
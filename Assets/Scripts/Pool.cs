using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    public int Count => _collection.Count;
    private Queue<GameObject> _collection;
    private GameObject _original;
    private Transform _root;
    private int _allElementsCount;

    public void Init(Transform root, GameObject obj, int count)
    {
        _collection = new Queue<GameObject>();
        _original = obj;
        _root = root;

        for (int i = 0; i < count; i++)
        {
            AddElement(i);
        } 
    }

    public GameObject Get()
    {
        if(_collection.Count > 1)
        {
            var poolable = _collection.Dequeue();
            return poolable;
        }
        // resize pool if needed
        else{
            AddElement(_allElementsCount);
            var poolable = _collection.Dequeue();
            return poolable;
        }
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        _collection.Enqueue(obj);
    }

    private void AddElement(int index)
    {
        var newPoolable = Instantiate(_original);
        newPoolable.transform.SetParent(_root);
        newPoolable.name = _original.name + index;
        newPoolable.SetActive(false);

        _collection.Enqueue(newPoolable);

        _allElementsCount++;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePool
{
    private StoneType _type;
    private GameObject _prefab;
    private Transform _parent;
    private Queue<GameObject> _pool = new Queue<GameObject>();

    public StonePool(StoneType type, GameObject prefab, int sizePool, Transform parent)
    {
        _type = type;
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < sizePool; i++)
        {
            CreateNewStone();
        }
    }

    private void CreateNewStone()
    {
        GameObject obj = GameObject.Instantiate(_prefab, _parent);
        Stone stone = obj.GetComponent<Stone>();
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }

    public GameObject GetOutOfPool()
    {
        GameObject obj = _pool.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    public void ReturnPool(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }
}

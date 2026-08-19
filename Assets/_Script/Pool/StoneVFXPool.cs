using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneVFXPool 
{
    private StoneVFXType _type;
    private GameObject _prefab;
    private Transform _parent;
    private Queue<GameObject> _pool = new Queue<GameObject>();

    public StoneVFXPool(StoneVFXType type, GameObject prefab, int sizePool, Transform parent)
    {
        _type = type;
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < sizePool; i++)
        {
            CreateNewStoneVFX();
        }
    }

    private void CreateNewStoneVFX()
    {
        GameObject obj = GameObject.Instantiate(_prefab, _parent);
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }

    public GameObject GetOutOfPool()
    {
        GameObject stoneVFXObj = _pool.Dequeue();
        
        stoneVFXObj.SetActive(true);
        return stoneVFXObj;
    }

    public void ReturnPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(_parent);
        _pool.Enqueue(obj);
    }

}

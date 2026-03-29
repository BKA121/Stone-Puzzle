using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePoolManager : MonoBehaviour
{
    private Dictionary<StoneType, StonePool> poolDict = new Dictionary<StoneType, StonePool>();

    public StoneConfig stoneConfig;

    public void InitializePools(List<StoneType> normalStone, List<StoneType> specialStone)
    {
        foreach (var type in normalStone)
        {
            if (!poolDict.ContainsKey(type))
            {
                StonePool stonePool = new StonePool(type, stoneConfig.GetStoneByType(type), 60, this.transform);
                poolDict.Add(type, stonePool);
            }
        }

        foreach (var type in specialStone)
        {
            if (!poolDict.ContainsKey(type))
            {
                StonePool stonePool = new StonePool(type, stoneConfig.GetStoneByType(type), 15, this.transform);
                poolDict.Add(type, stonePool);
            }
        }
    }

    public GameObject GetStoneByType(StoneType type)
    {
        return poolDict[type].GetOutOfPool();
    }

    public void ReturnStoneByType(StoneType type, GameObject stone)
    {
        poolDict[type].ReturnPool(stone);
    }
}

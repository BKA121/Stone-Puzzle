using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePoolManager : MonoBehaviour
{
    private Dictionary<StoneType, StonePool> poolStoneDict = new Dictionary<StoneType, StonePool>();
    private Dictionary<StoneVFXType, StoneVFXPool> poolStoneVFXDict = new Dictionary<StoneVFXType, StoneVFXPool>();

    public StoneConfig stoneConfig;
    public Transform StoneVFX;

    public void InitializePools(List<StoneType> normalStone, List<StoneType> specialStone)
    {
        foreach (var type in normalStone)
        {
            if (!poolStoneDict.ContainsKey(type))
            {
                StonePool stonePool = new StonePool(type, stoneConfig.GetStoneByType(type), 60, this.transform);
                poolStoneDict.Add(type, stonePool);
            }
        }

        foreach (var type in specialStone)
        {
            if (!poolStoneDict.ContainsKey(type))
            {
                StonePool stonePool = new StonePool(type, stoneConfig.GetStoneByType(type), 15, this.transform);
                poolStoneDict.Add(type, stonePool);
            }
        }

        StonePool stonePoolIce = new StonePool(StoneType.Ice, stoneConfig.GetStoneByType(StoneType.Ice), 15, this.transform);
        poolStoneDict.Add(StoneType.Ice, stonePoolIce);   

        foreach(StoneVFXType type in Enum.GetValues(typeof(StoneVFXType)))
        {
            if (!poolStoneVFXDict.ContainsKey(type))
            {
                StoneVFXPool stoneVFXPool = new StoneVFXPool(type, stoneConfig.GetStoneVFXByType(type), 50, StoneVFX);
                poolStoneVFXDict.Add(type, stoneVFXPool);
            }
        }
    }

    public Stone GetStoneByType(StoneType type, int r, int c)
    {
        return poolStoneDict[type].GetOutOfPool(r, c);
    }

    public void ReturnStoneByType(StoneType type, GameObject stone)
    {
        poolStoneDict[type].ReturnPool(stone);
    }

    public GameObject GetStoneVFXByType(StoneVFXType type)
    {
        return poolStoneVFXDict[type].GetOutOfPool();
    }

    public void ReturnStoneVFXByType(StoneVFXType type, GameObject stoneVFX)
    {
        poolStoneVFXDict[type].ReturnPool(stoneVFX);
    }
}

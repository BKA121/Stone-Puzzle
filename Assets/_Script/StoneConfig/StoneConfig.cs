using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StoneEntry
{
    public StoneType type;
    public GameObject stonePrefab; 
}

[System.Serializable]
public struct StoneVFXEntry
{
    public StoneVFXType type;
    public GameObject stoneVFXPrefab;
}

[CreateAssetMenu(fileName = "StoneConfig", menuName = "ScriptableObject/StoneConfig")]
public class StoneConfig : ScriptableObject
{
    public List<StoneEntry> allStones;

    public List<StoneVFXEntry> allStoneVFX;

    public GameObject GetStoneByType(StoneType type)
    {
        return allStones.Find(x => x.type == type).stonePrefab;
    }

    public GameObject GetStoneVFXByType(StoneVFXType type)
    {
        return allStoneVFX.Find(x => x.type == type).stoneVFXPrefab;
    }
}
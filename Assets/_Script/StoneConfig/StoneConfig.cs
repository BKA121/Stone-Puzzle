using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StoneEntry
{
    public StoneType type;
    public GameObject stonePrefab; 
    // Sau nay them sound, vfx
}

[CreateAssetMenu(fileName = "StoneConfig", menuName = "ScriptableObject/StoneConfig")]
public class StoneConfig : ScriptableObject
{
    public List<StoneEntry> allStones;

    public GameObject GetStoneByType(StoneType type)
    {
        return allStones.Find(x => x.type == type).stonePrefab;
    }
}
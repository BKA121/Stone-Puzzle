using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessMatch
{
    public void DestroyMatch(List<MatchGroup> allmatches, StonePoolManager stonePoolManager, Stone[,] boardStone)
    {
        foreach(var match in allmatches)
        {
            foreach(var stone in match.matchGroup)
            {
                Stone stoneScript = stone.GetComponent<Stone>();
                int r = (int)stoneScript.transform.localPosition.y;
                int c = (int)stoneScript.transform.localPosition.x;
                boardStone[r, c] = null;
                stonePoolManager.ReturnStoneByType(stone.type, stone.gameObject);
            }
        }
    }
}

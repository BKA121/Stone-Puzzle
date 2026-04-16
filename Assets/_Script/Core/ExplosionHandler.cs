using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionHandler
{
    private Stone[,] _boardStone;
    private StonePoolManager _stonePoolManager;

    public ExplosionHandler(StonePoolManager stonePoolManager, Stone[,] boardStone)
    {
        _boardStone = boardStone;
        _stonePoolManager = stonePoolManager;
    }

    public void DestroyMatch(List<MatchGroup> allmatches)
    {
        foreach(var match in allmatches)
        {
            foreach(var stone in match.matchGroup)
            {
                Stone stoneScript = stone.GetComponent<Stone>();
                int r = (int)stoneScript.transform.localPosition.y;
                int c = (int)stoneScript.transform.localPosition.x;
                _boardStone[r, c] = null;
                _stonePoolManager.ReturnStoneByType(stone.type, stone.gameObject);
            }
        }
    }
}

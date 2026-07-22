using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3 : IMatchProcess
{
    public void Process(MatchGroup group,Stone[,] boardStone, StonePoolManager stonePoolManager)
    {
        foreach(var stone in group.matchGroup)
        {
            stone.Explode(stonePoolManager, boardStone);
        }
    }
}

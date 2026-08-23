using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchProcesser
{
    private int _countStoneMatch5;
    private Stone[,] _boardStone;
    private StonePoolManager _stonePoolManager;

    public MatchProcesser(StonePoolManager stonePoolManager, Stone[,] boardStone)
    {
        _boardStone = boardStone;
        _stonePoolManager = stonePoolManager;
    }

    public void ProcessMatch(List<MatchGroup> allmatches, StoneExplosionManager stoneExplosionManager)
    {
        List<Stone> initialStonesToExplode = new List<Stone>();
        _countStoneMatch5 = 0;

        foreach(var match in allmatches)
        {
            foreach(var stone in match.matchGroup)
            {
                initialStonesToExplode.Add(stone);
                if(match.matchType == MatchType.match2 && stone.type == StoneType.StoneMatch5) _countStoneMatch5 ++;
                _boardStone[stone.r, stone.c] = null;
            }

            if(match.matchType == MatchType.match4)
            {
                int r = match.r;
                int c = match.c;
                _boardStone[r, c] = _stonePoolManager.GetStoneByType(GetMatch4StoneType(match.stoneType), r, c);
                _boardStone[r, c].isHorizontal = match.isHorizontal;
            }

            else if(match.matchType == MatchType.matchTorL)
            {
                int r = match.r;
                int c = match.c;
                _boardStone[r, c] = _stonePoolManager.GetStoneByType(GetMatchTorLStoneType(match.stoneType), r, c);
            }

            else if(match.matchType == MatchType.match5)
            {
                int r = match.r;
                int c = match.c;
                _boardStone[r, c] = _stonePoolManager.GetStoneByType(StoneType.StoneMatch5, r, c);
            }
        }
        
        stoneExplosionManager.HandleExplode(initialStonesToExplode, _countStoneMatch5);
    }

    public StoneType GetMatch4StoneType(StoneType type)
    {
        return type switch
        {
            StoneType.Red => StoneType.RedMatch4,
            StoneType.Green => StoneType.GreenMatch4,
            StoneType.Blue => StoneType.BlueMatch4,
            StoneType.Purple => StoneType.PurpleMatch4,
            StoneType.Yellow => StoneType.YellowMatch4,
            _ => type 
        };
    }

    public StoneType GetMatchTorLStoneType(StoneType type)
    {
        return type switch
        {
            StoneType.Red => StoneType.RedMatchTorL,
            StoneType.Green => StoneType.GreenMatchTorL,
            StoneType.Blue => StoneType.BlueMatchTorL,
            StoneType.Purple => StoneType.PurpleMatchTorL,
            StoneType.Yellow => StoneType.YellowMatchTorL,
            _ => type 
        };
    }
}

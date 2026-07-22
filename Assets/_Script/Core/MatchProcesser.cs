using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchProcesser
{
    private Stone[,] _boardStone;
    private StonePoolManager _stonePoolManager;
    private readonly Dictionary<MatchType, IMatchProcess> _strategies;

    public MatchProcesser(StonePoolManager stonePoolManager, Stone[,] boardStone)
    {
        _boardStone = boardStone;
        _stonePoolManager = stonePoolManager;

        _strategies = new Dictionary<MatchType, IMatchProcess>
        {
            { MatchType.match3, new Match3() },
            { MatchType.match4, new Match4() },
            { MatchType.matchTorL, new MatchTorL() },
            { MatchType.match5, new Match5() }
        };
    }

    public void ProcessMatch(List<MatchGroup> allmatches)
    {
        foreach(var match in allmatches)
        {
            if (_strategies.TryGetValue(match.matchType, out var strategy))
            {
                strategy.Process(match, _boardStone, _stonePoolManager);
            }
        }
    }
}

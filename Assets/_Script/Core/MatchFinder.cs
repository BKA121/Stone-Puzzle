using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchFinder 
{
    private int _row;
    private int _column;
    private Stone[,] _boardStone;
    private StoneManager _stoneManager;

    public MatchFinder(int row, int column, Stone[,] boardStone, StoneManager stoneManager)
    {
        _row = row;
        _column = column;
        _boardStone = boardStone;
        _stoneManager = stoneManager;
    }

    public MatchType GetMatchType(int length)
    {
        if (length >= 5) return MatchType.match5;
        if (length == 4) return MatchType.match4;
        if (length == 3) return MatchType.match3;
        return MatchType.none;
    }

    public List<MatchGroup> FindAllMatches()
    {
        var allMatches = new List<MatchGroup>();

        // Match Ngang
        for (int r = 0; r < _row/2; r++)
        {
            for (int c = 0; c < _column; c++)
            {
                if (_boardStone[r, c] == null || _boardStone[r, c].type == StoneType.Ice
                    || _boardStone[r, c].type == StoneType.StoneMatch5) continue;

                Stone curStone = _boardStone[r, c];
                List<Stone> horizontalMatch = new List<Stone> { curStone };

                for (int k = c + 1; k < _column; k++)
                {
                    if (_boardStone[r, k] != null && _stoneManager.IsSameNormalType(_boardStone[r, k].type, curStone.type))
                    {
                        horizontalMatch.Add(_boardStone[r, k]);
                    }
                    else break;
                }

                if (horizontalMatch.Count >= 3)
                {
                    allMatches.Add(new MatchGroup(horizontalMatch, GetMatchType(horizontalMatch.Count)));
                    
                    c += horizontalMatch.Count - 1; // bo qua cac stone trong match
                }
            }
        }
       
        // Match Doc
        for (int c = 0; c < _column; c++)
        {
            for (int r = 0; r < _row/2; r++)
            {
                if (_boardStone[r, c] == null || _boardStone[r, c].type == StoneType.Ice
                    || _boardStone[r, c].type == StoneType.StoneMatch5) continue;

                Stone curStone = _boardStone[r, c];
                List<Stone> verticalMatch = new List<Stone> { curStone };

                for (int k = r + 1; k < _row/2; k++)
                {
                    if (_boardStone[k, c] != null && _stoneManager.IsSameNormalType(_boardStone[k, c].type, curStone.type))
                    {
                        verticalMatch.Add(_boardStone[k, c]);
                    }
                    else break;
                }

                if (verticalMatch.Count >= 3)
                {
                    allMatches.Add(new MatchGroup(verticalMatch, GetMatchType(verticalMatch.Count)));
                    r += verticalMatch.Count - 1; 
                }
            }
        }

        return ProcessMatchesByPriority(allMatches);
    }

    private List<MatchGroup> ProcessMatchesByPriority(List<MatchGroup> allMatches)
    {
        List<MatchGroup> finalMatches = new List<MatchGroup>();
        HashSet<Stone> usedStones = new HashSet<Stone>();

        var match5s = allMatches.Where(m => m.matchType == MatchType.match5).ToList();
        foreach (var m5 in match5s)
        {
            int sumR = 0;
            int sumC = 0;

            foreach(var stone in m5.matchGroup)
            {
                m5.stoneType = _stoneManager.GetNormalStoneType(stone.type);
                break;
            }

            foreach (var stone in m5.matchGroup)
            {
                sumR += stone.r;
                sumC += stone.c;
                usedStones.Add(stone);
            }

            m5.r = sumR / m5.matchGroup.Count;
            m5.c = sumC / m5.matchGroup.Count;

            finalMatches.Add(m5);
        }

        var match4s = allMatches.Where(m => m.matchType == MatchType.match4).ToList();

        foreach (var m4 in match4s)
        {
            var cleanM4 = m4.matchGroup.Where(s => !usedStones.Contains(s)).ToList();

            if (cleanM4.Count == 4)
            {
                var validM4 = new MatchGroup(cleanM4, MatchType.match4);

                int sumR = 0;
                int sumC = 0;
                foreach (var stone in validM4.matchGroup)
                {
                    sumR += stone.r;
                    sumC += stone.c;
                    usedStones.Add(stone);
                }
                if(sumR%4 != 0) validM4.isHorizontal = false;
                else validM4.isHorizontal = true;
                
                validM4.r = sumR / 4; 
                validM4.c = sumC / 4;

                foreach(var stone in validM4.matchGroup)
                {
                    validM4.stoneType = _stoneManager.GetNormalStoneType(stone.type);
                    break;
                }

                finalMatches.Add(validM4);
            }
        }

        var match3s = allMatches.Where(m => m.matchType == MatchType.match3).ToList();

        List<MatchGroup> candidatesForMerge = new List<MatchGroup>();

        foreach (var m3 in match3s)
        {
            var cleanM3 = m3.matchGroup.Where(s => !usedStones.Contains(s)).ToList();
            if (cleanM3.Count == 3)
            {
                candidatesForMerge.Add(new MatchGroup(cleanM3, MatchType.match3));
            }
        }

        HashSet<int> processedIndices = new HashSet<int>();

        for (int i = 0; i < candidatesForMerge.Count; i++)
        {
            if (processedIndices.Contains(i)) continue;

            var validStonesI = candidatesForMerge[i].matchGroup.Where(s => !usedStones.Contains(s)).ToList();
            if (validStonesI.Count < 3) continue;

            bool merged = false;
            for (int j = i + 1; j < candidatesForMerge.Count; j++)
            {
                if (processedIndices.Contains(j)) continue;

                var validStonesJ = candidatesForMerge[j].matchGroup.Where(s => !usedStones.Contains(s)).ToList();
                if (validStonesJ.Count < 3) continue;

                var intersection = validStonesI.Intersect(validStonesJ);
                if (intersection.Any())
                {
                    var combinedStones = new HashSet<Stone>(validStonesI);
                    combinedStones.UnionWith(validStonesJ);

                    var matchTorL = new MatchGroup(combinedStones.ToList(), MatchType.matchTorL);
                    Stone centerStone = intersection.First(); 
                    matchTorL.r = centerStone.r;
                    matchTorL.c = centerStone.c;
                    matchTorL.stoneType = _stoneManager.GetNormalStoneType(centerStone.type);
                    finalMatches.Add(matchTorL);

                    foreach (var s in combinedStones) usedStones.Add(s);

                    processedIndices.Add(i);
                    processedIndices.Add(j);
                    merged = true;
                    break;
                }
            }

            if (!merged)
            {
                finalMatches.Add(new MatchGroup(validStonesI, MatchType.match3));
                foreach (var s in validStonesI) usedStones.Add(s);
            }
        }

        return finalMatches;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchFinder 
{
    private int _row;
    private int _column;

    public MatchFinder(int row, int column)
    {
        _row = row;
        _column = column;
    }

    public MatchType GetMatchType(int length)
    {
        if (length >= 5) return MatchType.match5;
        if (length == 4) return MatchType.match4;
        if (length == 3) return MatchType.match3;
        return MatchType.none;
    }

    public List<MatchGroup> FindAllMatches(Stone[,] boardStone)
    {
        var allMatches = new List<MatchGroup>();

        // Match Ngang
        for (int r = 0; r < _row/2; r++)
        {
            for (int c = 0; c < _column; c++)
            {
                if (boardStone[r, c] == null || boardStone[r, c].type == StoneType.Ice
                    || boardStone[r, c].type == StoneType.StoneMatch5) continue;

                Stone curStone = boardStone[r, c];
                List<Stone> horizontalMatch = new List<Stone> { curStone };

                for (int k = c + 1; k < _column; k++)
                {
                    if (boardStone[r, k] != null && boardStone[r, k].type == curStone.type)
                    {
                        horizontalMatch.Add(boardStone[r, k]);
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
                if (boardStone[r, c] == null || boardStone[r, c].type == StoneType.Ice
                    || boardStone[r, c].type == StoneType.StoneMatch5) continue;

                Stone curStone = boardStone[r, c];
                List<Stone> verticalMatch = new List<Stone> { curStone };

                for (int k = r + 1; k < _row/2; k++)
                {
                    if (boardStone[k, c] != null && boardStone[k, c].type == curStone.type)
                    {
                        verticalMatch.Add(boardStone[k, c]);
                    }
                    else break;
                }

                if (verticalMatch.Count >= 3)
                {
                    allMatches.Add(new MatchGroup(verticalMatch, GetMatchType(verticalMatch.Count)));
                    r += verticalMatch.Count - 1; // bo qua stone trong match
                }
            }
        }

        return ProcessMatchesByPriority(allMatches);
    }

    private List<MatchGroup> ProcessMatchesByPriority(List<MatchGroup> allMatches)
    {
        List<MatchGroup> finalMatches = new List<MatchGroup>();
        HashSet<Stone> usedStones = new HashSet<Stone>();

        // Uu tien match5 truoc
        var match5s = allMatches.Where(m => m.matchType == MatchType.match5).ToList();
        foreach (var m5 in match5s)
        {
            finalMatches.Add(m5);
            foreach (var s in m5.matchGroup) usedStones.Add(s);
        }

        var match4s = allMatches.Where(m => m.matchType == MatchType.match4).ToList();
        List<MatchGroup> validMatch4s = new List<MatchGroup>();

        foreach (var m4 in match4s)
        {
            var cleanM4 = m4.matchGroup.Where(s => !usedStones.Contains(s)).ToList();

            if (cleanM4.Count == 4)
            {
                validMatch4s.Add(new MatchGroup(cleanM4, MatchType.match4));
            }
        }

        var match3s = allMatches.Where(m => m.matchType == MatchType.match3).ToList();
        List<MatchGroup> candidatesForMerge = new List<MatchGroup>(validMatch4s);

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

            bool merged = false;
            for (int j = i + 1; j < candidatesForMerge.Count; j++)
            {
                if (processedIndices.Contains(j)) continue;

                // Kiem tra giao diem
                var intersection = candidatesForMerge[i].matchGroup.Intersect(candidatesForMerge[j].matchGroup);
                if (intersection.Any())
                {
                    var combinedStones = new HashSet<Stone>(candidatesForMerge[i].matchGroup);
                    combinedStones.UnionWith(candidatesForMerge[j].matchGroup);

                    finalMatches.Add(new MatchGroup(combinedStones.ToList(), MatchType.matchTorL));
                    processedIndices.Add(i);
                    processedIndices.Add(j);
                    merged = true;
                    break;
                }
            }

            if (!merged)
            {
                finalMatches.Add(candidatesForMerge[i]);
                foreach (var s in candidatesForMerge[i].matchGroup) usedStones.Add(s);
            }
        }

        return finalMatches;
    }
}

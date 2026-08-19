using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square3Explosion : BaseExplosionStrategy
{

    public override ExplosionResult GetExplosionResult(Stone[,] boardStone, Stone stone)
    {
        ExplosionResult result = new ExplosionResult();
        result.affectedStones = new List<Stone>();

        AddIce(result.affectedStones , boardStone, stone);

        result.type = GetNormalType(stone.type);

        int r = stone.r;
        int c = stone.c;
        
        int maxRow = boardStone.GetLength(0);
        int maxCol = boardStone.GetLength(1);

        for (int i = r - 1; i <= r + 1; i++)
        {
            for (int j = c - 1; j <= c + 1; j++)
            {
                if (i >= 0 && i < maxRow/2 && j >= 0 && j < maxCol)
                {
                    if (boardStone[i, j] != null && boardStone[i, j].type != StoneType.StoneMatch5)
                    {
                        result.affectedStones.Add(boardStone[i, j]);
                    }
                }
            }
        }

        result.vfxList = new List<StoneVFXType>();
        result.vfxList.Add(StoneVFXType.MatchTorLExplosion);
        return result;
    }
}

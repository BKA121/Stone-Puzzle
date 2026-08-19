using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ExplosionResult
{
    public List<Stone> affectedStones; 
    public StoneType type;
    public List<StoneVFXType> vfxList; 
}

public abstract class BaseExplosionStrategy
{
    public abstract ExplosionResult GetExplosionResult(Stone[,] boardStone, Stone stone);

    protected void AddIce(List<Stone> affectedStones, Stone[,] boardStone, Stone stone)
    {
        int maxRow = boardStone.GetLength(0);
        int maxCol = boardStone.GetLength(1);
        int[] dRow = { -1, 1, 0, 0 };
        int[] dCol = { 0, 0, -1, 1 };

        int r = stone.r;
        int c = stone.c;

        for (int i = 0; i < 4; i++)
        {
            int checkR = r + dRow[i];
            int checkC = c + dCol[i];

            if (checkR >= 0 && checkR < maxRow/2 && checkC >= 0 && checkC < maxCol)
            {
                if (boardStone[checkR, checkC] != null && boardStone[checkR, checkC].type == StoneType.Ice) 
                {
                    affectedStones.Add(boardStone[checkR, checkC]);
                }
            }
        }
    }

    protected StoneType GetNormalType(StoneType type)
    {
        return StoneManager.Instance.GetNormalStoneType(type);
    }
}

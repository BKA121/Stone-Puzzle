using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match5Explosion : BaseExplosionStrategy
{
   private Stone _targetStone;

    public void SetTargetStone(Stone targetStone)
    {
        _targetStone = targetStone;
    }

    public override ExplosionResult GetExplosionResult(Stone[,] boardStone, Stone stone)
    {
        int maxRow = boardStone.GetLength(0);
        int maxCol = boardStone.GetLength(1);

        ExplosionResult result = new ExplosionResult();
        result.affectedStones = new List<Stone>();

        if(_targetStone.type == StoneType.StoneMatch5)
        {
            for(int i=0; i<maxRow/2; i++)
            {
                for(int j=0; j<maxCol; j++)
                {
                    if(boardStone[i, j] != null)
                    {
                        result.affectedStones .Add(boardStone[i, j]);
                    }
                }
            }
        }
        else
        {
            StoneType targetType = GetNormalType(_targetStone.type);

            for(int i=0; i<maxRow/2; i++)
            {
                for(int j=0; j<maxCol; j++)
                {
                    if(boardStone[i, j] != null && GetNormalType(boardStone[i, j].type) == targetType)
                    {
                        result.affectedStones .Add(boardStone[i, j]);
                    }
                }
            }

        }

        return result;
    }
}
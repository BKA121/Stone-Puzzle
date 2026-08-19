using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LineExplosion : BaseExplosionStrategy
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

        bool isHorizontal = stone.isHorizontal;

        result.vfxList = new List<StoneVFXType>();
        result.vfxList.Add(StoneVFXType.Match4Explosion);

        if(isHorizontal)
        {
            for(int i=0; i<maxCol; i++)
            {
                if(boardStone[r, i] != null && boardStone[r, i].type != StoneType.StoneMatch5)
                    result.affectedStones .Add(boardStone[r, i]);
            }
            result.vfxList.Add(StoneVFXType.LightHorizontal);
        }
        else
        {
            for(int i=0; i<maxRow/2; i++)
            {
                if(boardStone[i, c] != null && boardStone[i, c].type != StoneType.StoneMatch5)
                    result.affectedStones .Add(boardStone[i, c]);
            }
            result.vfxList.Add(StoneVFXType.LightVertical);
        }
        return result;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalExplosion : BaseExplosionStrategy
{
    public override ExplosionResult GetExplosionResult(Stone[,] boardStone, Stone stone)
    {
        ExplosionResult result = new ExplosionResult();

        result.affectedStones = new List<Stone>();
        AddIce(result.affectedStones, boardStone, stone);

        result.type = GetNormalType(stone.type);
        result.vfxList = new List<StoneVFXType>();
        result.vfxList.Add(StoneVFXType.BaseExplosion);
        return result;
    }
}

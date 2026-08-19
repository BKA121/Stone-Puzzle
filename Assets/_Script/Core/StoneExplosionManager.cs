using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneExplosionManager 
{
    private StonePoolManager _stonePoolManager;
    private StoneVFXManager _stoneVFXManager;
    private Stone[,] _boardStone;

    private BaseExplosionStrategy _strategy;
    private BaseExplosionStrategy _normalExplosion;
    private BaseExplosionStrategy _lineExplosion;
    private BaseExplosionStrategy _square3Explosion;
    private BaseExplosionStrategy _iceExplosion;

    public StoneExplosionManager(StonePoolManager stonePoolManager, Stone[,] boardStone, StoneVFXManager stoneVFXManager)
    {
        _stonePoolManager = stonePoolManager;
        _boardStone = boardStone;
        _stoneVFXManager = stoneVFXManager;

        _normalExplosion = new NormalExplosion();
        _iceExplosion = new IceExplosion();
        _lineExplosion = new LineExplosion();
        _square3Explosion = new Square3Explosion();
    }

    public void HandleExplode(List<Stone> initialStones)
    {
        Queue<Stone> queue = new Queue<Stone>();

        foreach (Stone s in initialStones) queue.Enqueue(s);

        while (queue.Count > 0)
        {
            Stone currentStone = queue.Dequeue();

            _strategy = GetStrategyForStone(currentStone.type);

            ExplosionResult result = _strategy.GetExplosionResult(_boardStone, currentStone);

            if(result.affectedStones != null)
            {
                foreach (Stone victim in result.affectedStones)
                {
                    if (_boardStone[victim.r, victim.c] != null)
                    {
                        queue.Enqueue(victim);
                        _boardStone[victim.r, victim.c] = null; 
                    }
                }
            }

            _stoneVFXManager.PlayFullExplosion(currentStone.r, currentStone.c, result);
            
            _stonePoolManager.ReturnStoneByType(currentStone.type, currentStone.gameObject);
        }
    }

    private BaseExplosionStrategy GetStrategyForStone(StoneType type)
    {
        return type switch
        {
            StoneType.RedMatchTorL or StoneType.GreenMatchTorL or 
            StoneType.BlueMatchTorL or StoneType.PurpleMatchTorL or StoneType.YellowMatchTorL 
                => _square3Explosion,

            StoneType.RedMatch4 or StoneType.GreenMatch4 or 
            StoneType.BlueMatch4 or StoneType.PurpleMatch4 or StoneType.YellowMatch4 
                => _lineExplosion, 

            StoneType.Ice
                => _iceExplosion,
            _ => _normalExplosion
        };
    }
}

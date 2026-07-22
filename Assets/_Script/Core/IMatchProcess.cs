using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMatchProcess
{
    void Process(MatchGroup group, Stone[,] boardStone, StonePoolManager stonePoolManager);
}
